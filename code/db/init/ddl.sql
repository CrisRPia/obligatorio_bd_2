CREATE TABLE IF NOT EXISTS citizen (
    citizen_id        BINARY(16) PRIMARY KEY,
    credencial_civica TEXT UNIQUE NOT NULL, -- No english equivalent.
    name              TEXT        NOT NULL,
    surname           TEXT        NOT NULL,
    birth             DATE        NOT NULL,
    password_hash     TEXT        NOT NULL
);

CREATE TABLE IF NOT EXISTS department (
    department_id BINARY(16) PRIMARY KEY,
    name          VARCHAR(100) UNIQUE NOT NULL -- Needs to be varchar for unique check.
);

CREATE TABLE IF NOT EXISTS locality (
    locality_id   BINARY(16) PRIMARY KEY,
    name          TEXT                           NOT NULL,
    type          ENUM ('city', 'town', 'other') NOT NULL,
    department_id BINARY(16)                     NOT NULL REFERENCES department (department_id)
);

CREATE TABLE IF NOT EXISTS zone (
    zone_id     BINARY(16) PRIMARY KEY,
    name        TEXT       NOT NULL,
    postal_code TEXT       NOT NULL,
    locality_id BINARY(16) NOT NULL REFERENCES locality (locality_id)
);

CREATE TABLE IF NOT EXISTS police_officer (
    police_officer_id BINARY(16) PRIMARY KEY REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS police_station (
    police_station_id BINARY(16) PRIMARY KEY,
    zone_id           BINARY(16) NOT NULL REFERENCES zone (zone_id),
    name              TEXT       NOT NULL,
    address           TEXT       NOT NULL
);

CREATE TABLE IF NOT EXISTS police_officer_assigned_police_station (
    police_officer_id  BINARY(16) NOT NULL REFERENCES police_officer (police_officer_id),
    police_station_id  BINARY(16) NOT NULL REFERENCES police_station (police_station_id),
    assigned_date      DATE       NOT NULL,
    decomissioned_date DATE       NULL,
    PRIMARY KEY (police_officer_id, police_station_id, assigned_date)
);

CREATE TABLE IF NOT EXISTS establishment (
    establishment_id BINARY(16) PRIMARY KEY,
    name             TEXT       NOT NULL,
    address          TEXT       NOT NULL,
    zone_id          BINARY(16) NOT NULL REFERENCES zone (zone_id)
);

CREATE TABLE IF NOT EXISTS police_officer_assigned_establishment (
    police_officer_id BINARY(16) NOT NULL REFERENCES police_officer (police_officer_id),
    establishment_id  BINARY(16) NOT NULL REFERENCES establishment (establishment_id),
    assigned_date     DATE       NOT NULL,
    PRIMARY KEY (police_officer_id, establishment_id, assigned_date)
);

CREATE TABLE IF NOT EXISTS polling_station_president (
    polling_station_president_id BINARY(16) PRIMARY KEY REFERENCES citizen (citizen_id),
    org                          TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS polling_station_secretary (
    polling_station_secretary_id BINARY(16) PRIMARY KEY REFERENCES citizen (citizen_id),
    org                          TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS polling_station_vocal (
    polling_station_vocal_id BINARY(16) PRIMARY KEY REFERENCES citizen (citizen_id),
    org                      TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS election (
    election_id BINARY(16) PRIMARY KEY,
    description TEXT NOT NULL,
    date        DATE NOT NULL,
    is_open     BOOL NOT NULL DEFAULT TRUE
);

-- Circuito
CREATE TABLE IF NOT EXISTS polling_district (
    polling_district_number BINARY(16) PRIMARY KEY,
    is_open                 BOOL       NOT NULL DEFAULT TRUE,
    establishment_id        BINARY(16) NOT NULL REFERENCES establishment (establishment_id)
);

CREATE TABLE IF NOT EXISTS election_has_polling_district (
    election_id             BINARY(16) NOT NULL REFERENCES election (election_id),
    polling_district_number BINARY(16) NOT NULL REFERENCES polling_district (polling_district_number),
    PRIMARY KEY (election_id, polling_district_number)
);

CREATE TABLE IF NOT EXISTS polling_district_in_election_has_polling_station (
    polling_station_president_id BINARY(16) NOT NULL REFERENCES polling_station_president (polling_station_president_id),
    polling_station_secretary_id BINARY(16) NOT NULL REFERENCES polling_station_secretary (polling_station_secretary_id),
    polling_station_vocal_id     BINARY(16) NOT NULL REFERENCES polling_station_vocal (polling_station_vocal_id),
    polling_district_number      BINARY(16) NOT NULL REFERENCES polling_district (polling_district_number),
    election_id                  BINARY(16) NOT NULL REFERENCES election (election_id),
    PRIMARY KEY (polling_station_president_id, polling_district_number, election_id)
);

CREATE TABLE IF NOT EXISTS citizen_votes_in_polling_district_election (
    citizen_id              BINARY(16) NOT NULL REFERENCES citizen (citizen_id),
    election_id             BINARY(16) NOT NULL REFERENCES election (election_id),
    polling_district_number BINARY(16) NOT NULL REFERENCES polling_district (polling_district_number),
    PRIMARY KEY (citizen_id, election_id)
);

CREATE TABLE IF NOT EXISTS citizen_assigned_int_polling_district_election (
    citizen_id              BINARY(16) NOT NULL REFERENCES citizen (citizen_id),
    election_id             BINARY(16) NOT NULL REFERENCES election (election_id),
    polling_district_number BINARY(16) NOT NULL REFERENCES polling_district (polling_district_number),
    PRIMARY KEY (citizen_id, election_id)
);

CREATE TABLE IF NOT EXISTS vote (
    vote_id BINARY(16) PRIMARY KEY,
    state   ENUM ('valid', 'out_of_district', 'approved_out_of_district')
);

CREATE TABLE IF NOT EXISTS ballot (
    ballot_id BINARY(16) PRIMARY KEY
);

CREATE TABLE IF NOT EXISTS vote_contains_ballot (
    vote_id   BINARY(16) NOT NULL REFERENCES vote (vote_id),
    ballot_id BINARY(16) NOT NULL REFERENCES ballot (ballot_id),
    PRIMARY KEY (vote_id, ballot_id)
);

CREATE TABLE IF NOT EXISTS boolean_ballot (
    boolean_ballot_id      BINARY(16) NOT NULL REFERENCES ballot (ballot_id),
    value                  BOOL       NOT NULL,
    total_votes_with_value INT        NOT NULL DEFAULT 0,
    PRIMARY KEY (boolean_ballot_id)
);

CREATE TABLE IF NOT EXISTS list_ballot (
    list_ballot_id BINARY(16) PRIMARY KEY REFERENCES ballot (ballot_id),
    list_number    BINARY(16) NOT NULL
);

CREATE TABLE IF NOT EXISTS candidate (
    candidate_id BINARY(16) PRIMARY KEY REFERENCES citizen (citizen_id)
);

CREATE TABLE IF NOT EXISTS list_ballot_has_candidate (
    list_ballot_id BINARY(16) REFERENCES list_ballot (list_ballot_id),
    candidate_id   BINARY(16) REFERENCES candidate (candidate_id),
    -- Starts at 0
    index_in_list  BINARY(16)                                                                                NOT NULL,
    org            ENUM ('deputy', 'senator', 'departmental_board', 'municipal_councilor', 'main_candidate') NOT NULL,
    PRIMARY KEY (list_ballot_id, candidate_id)
);

CREATE TABLE IF NOT EXISTS party (
    party_id           BINARY(16) PRIMARY KEY DEFAULT (UUID_TO_BIN(UUID())),
    hedquarters_adress TEXT
);


CREATE TABLE IF NOT EXISTS party_has_citizen (
    list_ballot_id BINARY(16) REFERENCES list_ballot (list_ballot_id),
    party_id       BINARY(16) REFERENCES party (party_id),
    role           ENUM ('president', 'vice_president') NOT NULL,
    admission_date DATE                                 NOT NULL,
    exit_date      DATE                                 NULL,
    UNIQUE (party_id, role),
    PRIMARY KEY (list_ballot_id, party_id, admission_date)
);

CREATE TABLE IF NOT EXISTS list_ballot_belongs_to_department (
    list_id      BINARY(16) PRIMARY KEY REFERENCES list_ballot (list_ballot_id),
    deparment_id BINARY(16) NOT NULL REFERENCES department (department_id)
);

CREATE TABLE IF NOT EXISTS pleibiscite (
    election_id BINARY(16) PRIMARY KEY REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS referndum (
    election_id BINARY(16) PRIMARY KEY REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS presidential (
    election_id BINARY(16) PRIMARY KEY REFERENCES election (election_id)
);

CREATE TABLE IF NOT EXISTS municipal (
    election_id BINARY(16) PRIMARY KEY REFERENCES election (election_id),
    locality_id BINARY(16) NOT NULL REFERENCES locality (locality_id)
);

CREATE TABLE IF NOT EXISTS ballotage (
    election_id BINARY(16) PRIMARY KEY REFERENCES election (election_id)
);