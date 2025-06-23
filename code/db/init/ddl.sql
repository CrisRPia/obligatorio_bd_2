-- USE votes_db;

CREATE TABLE IF NOT EXISTS citizen
(
    citizen_id        BINARY(16) PRIMARY KEY,
    credencial_civica VARCHAR(100) UNIQUE NOT NULL, -- No english equivalent.
    uruguayan_id      INT UNIQUE          NOT NULL,
    name              TEXT                NOT NULL,
    surname           TEXT                NOT NULL,
    birth             DATE                NOT NULL,
    password_hash     TEXT                NOT NULL
);

CREATE TABLE IF NOT EXISTS department
(
    department_id BINARY(16) PRIMARY KEY,
    name          VARCHAR(100) UNIQUE NOT NULL -- Needs to be varchar for unique check.
);

CREATE TABLE IF NOT EXISTS locality
(
    locality_id   BINARY(16) PRIMARY KEY,
    name          TEXT                           NOT NULL,
    type          ENUM ('city', 'town', 'other') NOT NULL,
    department_id BINARY(16)                     NOT NULL,
    foreign key (department_id) references department (department_id)
);

CREATE TABLE IF NOT EXISTS zone
(
    zone_id     BINARY(16) PRIMARY KEY,
    name        TEXT       NOT NULL,
    postal_code TEXT       NOT NULL,
    locality_id BINARY(16) NOT NULL,
    foreign key (locality_id) references locality (locality_id)
);

CREATE TABLE IF NOT EXISTS police_officer
(
    police_officer_id BINARY(16) PRIMARY KEY,
    foreign key (police_officer_id) references citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS police_station
(
    police_station_id BINARY(16) PRIMARY KEY,
    zone_id           BINARY(16) NOT NULL,
    name              TEXT       NOT NULL,
    address           TEXT       NOT NULL,
    foreign key (zone_id) references zone (zone_id)
);

CREATE TABLE IF NOT EXISTS police_officer_assigned_police_station
(
    police_officer_id  BINARY(16) NOT NULL,
    police_station_id  BINARY(16) NOT NULL,
    assigned_date      DATE       NOT NULL,
    decomissioned_date DATE       NULL,
    PRIMARY KEY (police_officer_id, police_station_id, assigned_date),
    foreign key (police_officer_id) references police_officer (police_officer_id),
    foreign key (police_station_id) references police_station (police_station_id)
);

CREATE TABLE IF NOT EXISTS establishment
(
    establishment_id BINARY(16) PRIMARY KEY,
    name             TEXT       NOT NULL,
    address          TEXT       NOT NULL,
    zone_id          BINARY(16) NOT null,
    foreign key (zone_id) references zone (zone_id)
);

CREATE TABLE IF NOT EXISTS police_officer_assigned_establishment
(
    police_officer_id BINARY(16) NOT NULL,
    establishment_id  BINARY(16) NOT NULL,
    assigned_date     DATE       NOT NULL,
    PRIMARY KEY (police_officer_id, establishment_id, assigned_date),
    foreign key (police_officer_id) REFERENCES police_officer (police_officer_id),
    foreign key (establishment_id) references establishment (establishment_id)
);

CREATE TABLE IF NOT EXISTS polling_station_president
(
    polling_station_president_id BINARY(16) PRIMARY KEY,
    org                          TEXT NOT NULL,
    foreign key (polling_station_president_id) REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS polling_station_secretary
(
    polling_station_secretary_id BINARY(16) PRIMARY KEY,
    org                          TEXT NOT NULL,
    foreign key (polling_station_secretary_id) REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS polling_station_vocal
(
    polling_station_vocal_id BINARY(16) PRIMARY KEY,
    org                      TEXT NOT NULL,
    foreign key (polling_station_vocal_id) REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS election
(
    election_id BINARY(16) PRIMARY KEY,
    description TEXT NOT NULL,
    date        DATE NOT NULL,
    state enum ('notStarted', 'open', 'closed') not null default 'notStarted'
);

-- Circuito
CREATE TABLE IF NOT EXISTS polling_district
(
    polling_district_number INT,
    is_open                 BOOL       NOT NULL DEFAULT FALSE,
    establishment_id        BINARY(16) NOT NULL,
    PRIMARY KEY (polling_district_number, establishment_id),
    foreign key (establishment_id) REFERENCES establishment (establishment_id)
);

CREATE TABLE IF NOT EXISTS election_has_polling_district
(
    election_id             BINARY(16) NOT NULL,
    establishment_id        BINARY(16) NOT NULL,
    polling_district_number int        NOT NULL,
    PRIMARY KEY (election_id, polling_district_number),
    foreign key (election_id) REFERENCES election (election_id),
    foreign key (polling_district_number, establishment_id) REFERENCES polling_district (polling_district_number, establishment_id)
);

CREATE TABLE IF NOT EXISTS polling_district_in_election_has_polling_station
(
    polling_station_president_id BINARY(16) NOT NULL,
    polling_station_secretary_id BINARY(16) NOT NULL,
    polling_station_vocal_id     BINARY(16) NOT NULL,
    polling_district_number      INT        NOT NULL,
    establishment_id             BINARY(16) NOT NULL,
    election_id                  BINARY(16) NOT NULL,
    PRIMARY KEY (polling_station_president_id, polling_district_number, election_id),
    foreign key (polling_station_president_id) REFERENCES polling_station_president (polling_station_president_id),
    foreign key (polling_station_secretary_id) REFERENCES polling_station_secretary (polling_station_secretary_id),
    foreign key (polling_station_vocal_id) REFERENCES polling_station_vocal (polling_station_vocal_id),
    foreign key (polling_district_number, establishment_id) REFERENCES polling_district (polling_district_number, establishment_id),
    foreign key (election_id) REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS citizen_votes_in_polling_district_election
(
    citizen_id              BINARY(16) NOT NULL,
    election_id             BINARY(16) NOT NULL,
    polling_district_number INT        NOT NULL,
    establishment_id        BINARY(16) NOT NULL,
    PRIMARY KEY (citizen_id, election_id),
    foreign key (citizen_id) REFERENCES citizen (citizen_id),
    foreign key (election_id) REFERENCES election (election_id),
    foreign key (polling_district_number, establishment_id) REFERENCES polling_district (polling_district_number, establishment_id)
);

CREATE TABLE IF NOT EXISTS citizen_assigned_int_polling_district_election
(
    citizen_id              BINARY(16) NOT NULL,
    election_id             BINARY(16) NOT NULL,
    polling_district_number INT        NOT NULL,
    establishment_id        BINARY(16) NOT NULL,
    PRIMARY KEY (citizen_id, election_id),
    foreign key (citizen_id) REFERENCES citizen (citizen_id),
    foreign key (election_id) REFERENCES election (election_id),
    foreign key (polling_district_number, establishment_id) REFERENCES polling_district (polling_district_number, establishment_id)
);

CREATE TABLE IF NOT EXISTS vote
(
    vote_id BINARY(16) PRIMARY KEY,
    state   ENUM ('valid', 'out_of_district', 'approved_out_of_district') not null
);

CREATE TABLE IF NOT EXISTS ballot
(
    ballot_id BINARY(16) PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS vote_contains_ballot
(
    vote_id   BINARY(16) NOT NULL,
    ballot_id BINARY(16) NOT NULL,
    PRIMARY KEY (vote_id, ballot_id),
    foreign key (vote_id) REFERENCES vote (vote_id),
    foreign key (ballot_id) REFERENCES ballot (ballot_id)
);

CREATE TABLE IF NOT EXISTS boolean_ballot
(
    boolean_ballot_id      BINARY(16) NOT NULL,
    value                  BOOL       NOT NULL,
    total_votes_with_value INT        NOT NULL DEFAULT 0,
    PRIMARY KEY (boolean_ballot_id),
    foreign key (boolean_ballot_id) REFERENCES ballot (ballot_id)
);

CREATE TABLE IF NOT EXISTS list_ballot
(
    list_ballot_id BINARY(16) PRIMARY KEY,
    list_number    INT NOT NULL,
    foreign key (list_ballot_id) REFERENCES ballot (ballot_id)
);

CREATE TABLE IF NOT EXISTS list_ballot_has_candidate
(
    list_ballot_id BINARY(16)                                                                                not null,
    candidate_id   BINARY(16)                                                                                not null,
    -- Starts at 0
    index_in_list  INT                                                                                       NOT NULL,
    org            ENUM ('deputy', 'senator', 'departmental_board', 'municipal_councilor', 'main_candidate') NOT NULL,
    PRIMARY KEY (list_ballot_id, candidate_id),
    foreign key (list_ballot_id) REFERENCES list_ballot (list_ballot_id),
    foreign key (candidate_id) REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS party
(
    party_id           BINARY(16) PRIMARY KEY,
    hedquarters_adress TEXT,
    name               text
);


CREATE TABLE IF NOT EXISTS party_has_citizen
(
    party_id       BINARY(16),
    citizen_id     BINARY(16),
    role           ENUM ('president', 'vice_president') NOT NULL,
    admission_date DATE                                 NOT NULL,
    exit_date      DATE                                 NULL,
    UNIQUE (party_id, role),
    PRIMARY KEY (party_id, admission_date),
    foreign key (party_id) REFERENCES party (party_id),
    foreign key (citizen_id) REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS list_ballot_belongs_to_department
(
    list_id      BINARY(16) PRIMARY KEY,
    deparment_id BINARY(16) NOT NULL,
    foreign key (list_id) REFERENCES list_ballot (list_ballot_id),
    foreign key (deparment_id) REFERENCES department (department_id)
);

CREATE TABLE IF NOT EXISTS pleibiscite
(
    election_id BINARY(16) PRIMARY KEY,
    foreign key (election_id) REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS referendum
(
    election_id BINARY(16) PRIMARY KEY,
    foreign key (election_id) REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS presidential
(
    election_id BINARY(16) PRIMARY KEY,
    foreign key (election_id) REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS municipal
(
    election_id BINARY(16) PRIMARY KEY,
    locality_id BINARY(16) NOT NULL REFERENCES locality (locality_id),
    foreign key (election_id) REFERENCES election (election_id),
    foreign key (locality_id) REFERENCES locality (locality_id)
);

CREATE TABLE IF NOT EXISTS ballotage
(
    election_id BINARY(16) PRIMARY KEY,
    foreign key (election_id) REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS election_allows_ballots
(
    election_id BINARY(16) NOT NULL,
    ballot_id   BINARY(16) NOT NULL,
    primary key (election_id, ballot_id),
    foreign key (election_id) REFERENCES election (election_id),
    foreign key (ballot_id) REFERENCES ballot (ballot_id)
);