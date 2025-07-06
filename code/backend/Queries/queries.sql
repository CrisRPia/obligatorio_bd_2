-- name: InsertCitizenVoteInPollingDistrictElection :exec
INSERT
INTO citizen_votes_in_polling_district_election (citizen_id, election_id, polling_district_number, establishment_id)
VALUES (?, ?, ?, ?);

-- name: InsertCitizen :exec
INSERT
INTO citizen (citizen_id, uruguayan_id, credencial_civica, name, surname, birth, password_hash)
VALUES (?, ?, ?, ?, ?, ?, ?);

-- name: SelectUserAssignment :many
SELECT a.election_id, a.polling_district_number, a.establishment_id
FROM citizen_assigned_int_polling_district_election a
         LEFT JOIN citizen_votes_in_polling_district_election v
                   ON v.election_id = a.election_id AND v.citizen_id = a.citizen_id AND
                      v.polling_district_number = a.polling_district_number
         INNER JOIN election e ON a.election_id = e.election_id
WHERE a.citizen_id = ?
  AND e.date < CURRENT_DATE()
  AND e.state = 'open'
  AND v.election_id IS NULL;

-- name: SelectCitizen :one
SELECT *
FROM citizen
WHERE citizen_id = ?;

-- name: LoginCitizen :one
SELECT c.*
     , psp.polling_station_president_id
     , pdiehps.polling_district_number
     , pdiehps.establishment_id
     , e.address
     , e.name as 'establishment_name'
     , z.zone_id
     , z.postal_code
     , l.locality_id
     , l.type
     , l.name as 'locality_name'
     , d.department_id
     , d.name as 'department_name'
FROM citizen c
         JOIN polling_station_president psp ON psp.polling_station_president_id = c.citizen_id
         JOIN polling_district_in_election_has_polling_station pdiehps
              ON pdiehps.polling_station_president_id = c.citizen_id
         join establishment e on e.establishment_id = pdiehps.establishment_id
         join zone z on e.zone_id = z.zone_id
         join locality l on z.locality_id = l.locality_id
         join department d on l.department_id = d.department_id
WHERE c.credencial_civica = ?
  AND c.uruguayan_id = ?;

-- name: InsertVote :exec
INSERT
INTO vote (vote_id, state, polling_district_number, establishment_id)
VALUES (?, ?, ?, ?);

-- name: InsertBallot :exec
INSERT
INTO vote_contains_ballot (vote_id, ballot_id)
VALUES (?, ?);

-- name: GetElections :many
SELECT e.*
     , b.election_id  AS 'ballotage_id'
     , pl.election_id AS 'pleibiscite_id'
     , r.election_id  AS 'referendum_id'
     , pr.election_id AS 'presidential_id'
     , m.election_id  AS 'municipal_id'
     , l.department_id
FROM election e
         INNER JOIN citizen_assigned_int_polling_district_election caipde
                    ON e.election_id = caipde.election_id and caipde.polling_district_number = ? and
                       caipde.establishment_id = ?
         LEFT JOIN ballotage b ON e.election_id = b.election_id
         LEFT JOIN pleibiscite pl ON e.election_id = pl.election_id
         LEFT JOIN referendum r ON e.election_id = r.election_id
         LEFT JOIN presidential pr ON e.election_id = pr.election_id
         LEFT JOIN municipal m ON e.election_id = m.election_id
         LEFT JOIN locality l ON l.locality_id = m.locality_id
group by e.election_id;

-- name: GetPollingStationDistrict :one
SELECT *
FROM polling_district_in_election_has_polling_station pdiehps
WHERE pdiehps.polling_station_president_id = ?;

-- name: GetDepartments :many
SELECT *
FROM department;

-- name: InsertDepartment :exec
INSERT
INTO department (department_id, name)
VALUES (?, ?);

-- name: UpdatePollingDistrict :exec
UPDATE polling_district pd
SET pd.is_open = ?
WHERE pd.establishment_id = ?
  AND pd.polling_district_number = ?;

-- name: GetCitizenByCredencialCivica :one
SELECT *
FROM citizen
WHERE credencial_civica = ?;

SELECT *
FROM citizen;

-- name: GetBallotsForElections :many
SELECT ballot.*
     , eab.election_id
     , list_ballot.list_number
     , boolean_ballot.value
     , boolean_ballot.total_votes_with_value
FROM ballot
         JOIN election_allows_ballots eab ON ballot.ballot_id = eab.ballot_id
         LEFT JOIN list_ballot ON list_ballot_id = eab.ballot_id
         LEFT JOIN boolean_ballot ON boolean_ballot_id = eab.ballot_id
WHERE eab.election_id IN (sqlc.slice(elections));

-- name: InsertParty :exec
INSERT
INTO party (party_id, hedquarters_adress, name)
VALUES (?, ?, ?);

-- name: InsertPartyMember :exec
INSERT
INTO party_has_citizen (party_id, role, admission_date, exit_date)
VALUES (?, ?, ?, ?);

-- name: AssignCandidateToListBallot :exec
INSERT
INTO list_ballot_has_candidate (list_ballot_id, candidate_id, index_in_list, org)
VALUES (?, ?, ?, ?);

-- name: CreateBallot :exec
insert into ballot (ballot_id)
values (?);

-- name: CreateListBallot :exec
insert into list_ballot (list_ballot_id, list_number)
values (?, ?);

-- name: AddListBallotToDepartment :exec
insert into list_ballot_belongs_to_department (list_id, deparment_id)
values (?, ?);

-- name: InsertElection :exec
insert into election (election_id, description, date)
VALUES (?, ?, ?);

-- name: AllowBallotInElection :exec
insert into election_allows_ballots (election_id, ballot_id)
VALUES (?, ?);

-- name: InsertLocality :exec
insert into locality (locality_id, name, type, department_id)
VALUES (?, ?, ?, ?);

-- name: InsertZone :exec
insert into zone (zone_id, name, postal_code, locality_id)
VALUES (?, ?, ?, ?);

-- name: InsertEstablishment :exec
insert into establishment (establishment_id, name, address, zone_id)
VALUES (?, ?, ?, ?);

-- name: InsertCircuit :exec
insert into polling_district (polling_district_number, establishment_id)
VALUES (?, ?);

-- name: InsertBoardPresident :exec
insert into polling_station_president (polling_station_president_id, org)
VALUES (?, ?);

-- name: InsertBoardSecretary :exec
insert into polling_station_secretary (polling_station_secretary_id, org)
VALUES (?, ?);

-- name: InsertBoardVocal :exec
insert into polling_station_vocal (polling_station_vocal_id, org)
VALUES (?, ?);

-- name: InsertBoardInCircuitElection :exec
insert into polling_district_in_election_has_polling_station (polling_station_president_id,
                                                              polling_station_secretary_id, polling_station_vocal_id,
                                                              polling_district_number, establishment_id, election_id)
VALUES (?, ?, ?, ?, ?, ?);

-- name: AssignCitizenIntoPollingDistrictElection :exec
insert into citizen_assigned_int_polling_district_election (citizen_id, election_id, polling_district_number, establishment_id)
VALUES (?, ?, ?, ?);

-- name: InsertMunicipalElection :exec
insert into municipal (election_id, locality_id)
values (?, ?);

-- name: GetMunicipalElectionResult :many
with votes_per_ballot as (select count(*) as amount_of_votes, e.election_id, lb.*
                          from vote v
                                   join vote_contains_ballot vcb on v.vote_id = vcb.vote_id
                                   join ballot b on b.ballot_id = vcb.ballot_id
                                   join list_ballot lb on lb.list_ballot_id = b.ballot_id
                                   join election_allows_ballots eab on lb.list_ballot_id = eab.ballot_id
                                   join election e on eab.election_id = e.election_id
                          where v.polling_district_number = ?
                            and v.establishment_id = ?
                          group by lb.list_ballot_id, lb.list_number, e.election_id)
select *
from votes_per_ballot
order by election_id, amount_of_votes desc;