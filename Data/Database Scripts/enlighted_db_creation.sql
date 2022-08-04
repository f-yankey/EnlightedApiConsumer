CREATE SCHEMA `enlighteddb` ;

use enlighteddb;

CREATE TABLE Floor(
    FloorId          INT                      NOT NULL,
    Name             INT,
    Building         INT,
    Campus           INT,
    Company          INT,
    Description      NATIONAL VARCHAR(500),
    FloorPlanUrl     NATIONAL VARCHAR(100),
    ParentFloorId    NATIONAL VARCHAR(10),
    PRIMARY KEY (FloorId)
)ENGINE=MYISAM
;

use enlighteddb;
CREATE TABLE Fixture(
    FixtureId     INT                      NOT NULL,
    FloorId       INT                      NOT NULL,
    Name          NATIONAL VARCHAR(200)    NOT NULL,
    XAxis         INT,
    YAxis         INT,
    GroupId       INT,
    MacAddress    NATIONAL VARCHAR(200),
    ClassName     NATIONAL VARCHAR(100)    NOT NULL,
    PRIMARY KEY (FixtureId)
)ENGINE=MYISAM
;

ALTER TABLE Fixture ADD CONSTRAINT FixtureFloor
    FOREIGN KEY (FloorId)
    REFERENCES Floor(FloorId)
;
