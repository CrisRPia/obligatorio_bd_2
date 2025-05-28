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

-- name: InsertVote :exec
INSERT
  INTO vote (vote_id, state)
VALUES (?, ?);

-- name: InsertBallot :exec
INSERT
  INTO vote_contains_ballot (vote_id, ballot_id)
VALUES (?, ?);

-- name: GetCitizen :one
SELECT *
  FROM citizen;