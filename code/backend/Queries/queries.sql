-- name: InsertCitizenVoteInPollingDistrictElection :exec
INSERT
  INTO citizen_votes_in_polling_district_election (citizen_id, election_id, polling_district_number)
VALUES (?, ?, ?);

-- name: InsertCitizen :exec
INSERT
  INTO citizen (citizen_id, uruguayan_id, credencial_civica, name, surname, birth, password_hash)
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: SelectUserAssignment :many
SELECT a.election_id, a.polling_district_number
  FROM citizen_assigned_int_polling_district_election a
           LEFT JOIN citizen_votes_in_polling_district_election v
                     ON v.election_id = a.election_id AND v.citizen_id = a.citizen_id AND
                        v.polling_district_number = a.polling_district_number
           INNER JOIN election e ON a.election_id = e.election_id
 WHERE a.citizen_id = ?
   AND e.date < CURRENT_DATE()
   AND e.is_open
   AND v.election_id IS NULL;

-- name: SelectCitizen :one
SELECT *
  FROM citizen
 WHERE citizen_id = ?;

-- name: LoginCitizen :one
SELECT c.*
     , po.police_officer_id
     , psp.polling_station_president_id
     , psv.polling_station_vocal_id
     , pss.polling_station_secretary_id
  FROM citizen c
           LEFT JOIN police_officer po ON po.police_officer_id = c.citizen_id
           LEFT JOIN polling_station_president psp ON psp.polling_station_president_id = c.citizen_id
           LEFT JOIN polling_station_vocal psv ON psv.polling_station_vocal_id = c.citizen_id
           LEFT JOIN polling_station_secretary pss ON pss.polling_station_secretary_id = c.citizen_id
 WHERE c.credencial_civica = ?
   AND c.uruguayan_id = ?;

SELECT *
  FROM citizen c
 WHERE c.credencial_civica = 'NEU10000'
   AND c.uruguayan_id = 83500295
   AND c.password_hash =
       '$argon2id$v=19$m=65536,t=3,p=1$ER7MtGNwv3p9b8aNV2mw2A$iQmB1nOEZOTszZ55+7OToKFiOz2nNbofL1iEMEi1/pw';

-- name: InsertVote :exec
INSERT
  INTO vote (vote_id, state)
VALUES (?, ?);

-- name: InsertBallot :exec
INSERT
  INTO vote_contains_ballot (vote_id, ballot_id)
VALUES (?, ?);

-- name: GetElectionsForCitizen :many
SELECT e.*
     , b.election_id  AS 'ballotage_id'
     , pl.election_id AS 'pleibiscite_id'
     , r.election_id  AS 'referndum_id'
     , pr.election_id AS 'presidential_id'
     , m.election_id  AS 'municipal_id'
     , department_id
  FROM election e
           INNER JOIN citizen_assigned_int_polling_district_election caipde
                      ON e.election_id = caipde.election_id AND caipde.citizen_id = ?
           LEFT JOIN ballotage b ON e.election_id = b.election_id
           LEFT JOIN pleibiscite pl ON e.election_id = pl.election_id
           LEFT JOIN referndum r ON e.election_id = r.election_id
           LEFT JOIN presidential pr ON e.election_id = pr.election_id
           LEFT JOIN municipal m ON e.election_id = m.election_id
           LEFT JOIN locality l ON l.locality_id = m.locality_id
 WHERE (sqlc.arg(start_date) <= e.date)
   AND (sqlc.arg(end_date) >= e.date);