-- HRMS: схема базы данных (SQLite)
-- Создание таблиц, связей (1:1, 1:N, N:N), ограничений, ключей.

PRAGMA foreign_keys = ON;

DROP TABLE IF EXISTS InterviewInterviewers;
DROP TABLE IF EXISTS Interviews;
DROP TABLE IF EXISTS ResumeDetails;
DROP TABLE IF EXISTS Resumes;
DROP TABLE IF EXISTS Candidates;
DROP TABLE IF EXISTS Vacancies;
DROP TABLE IF EXISTS Interviewers;
DROP TABLE IF EXISTS AspNetRoles;
DROP TABLE IF EXISTS AspNetUsers;

-- 1:N  Candidate -> Resumes (cascade), Vacancy -> Resumes (set null, опциональная связь)
CREATE TABLE Candidates (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    LastName  TEXT    NOT NULL CHECK (length(LastName) BETWEEN 2 AND 100),
    FirstName TEXT    NOT NULL CHECK (length(FirstName) BETWEEN 2 AND 100),
    Email     TEXT    NOT NULL UNIQUE,
    Phone     TEXT
);

CREATE TABLE Vacancies (
    Id                   INTEGER PRIMARY KEY AUTOINCREMENT,
    Title                TEXT    NOT NULL CHECK (length(Title) BETWEEN 3 AND 200),
    Department           TEXT    NOT NULL CHECK (length(Department) BETWEEN 2 AND 100),
    Description          TEXT,
    SalaryMin            NUMERIC NOT NULL CHECK (SalaryMin >= 0),
    SalaryMax            NUMERIC,
    Status               INTEGER NOT NULL DEFAULT 1 CHECK (Status IN (0, 1, 2, 3)), -- Draft/Open/Paused/Closed
    PublishedAt          TEXT    NOT NULL,
    AcceptingResumesUntil TEXT,
    CHECK (SalaryMax IS NULL OR SalaryMax >= SalaryMin)
);

CREATE TABLE Resumes (
    Id             INTEGER PRIMARY KEY AUTOINCREMENT,
    CandidateId    INTEGER NOT NULL,
    VacancyId      INTEGER,
    Summary        TEXT    NOT NULL CHECK (length(Summary) BETWEEN 10 AND 2000),
    ExperienceYears INTEGER NOT NULL DEFAULT 0 CHECK (ExperienceYears >= 0),
    SubmittedAt    TEXT    NOT NULL,
    Status         INTEGER NOT NULL DEFAULT 0 CHECK (Status IN (0, 1, 2, 3)), -- New/InReview/Rejected/Accepted
    FOREIGN KEY (CandidateId) REFERENCES Candidates (Id)  ON DELETE CASCADE,
    FOREIGN KEY (VacancyId)   REFERENCES Vacancies (Id)   ON DELETE SET NULL
);
CREATE INDEX IX_Resumes_Status      ON Resumes (Status);
CREATE INDEX IX_Resumes_CandidateId ON Resumes (CandidateId);

-- 1:1  Resume <-> ResumeDetails (PK = FK на Resume, cascade)
CREATE TABLE ResumeDetails (
    ResumeId  INTEGER PRIMARY KEY,
    Education TEXT,
    Skills    TEXT,
    FilePath  TEXT,
    FOREIGN KEY (ResumeId) REFERENCES Resumes (Id) ON DELETE CASCADE
);

CREATE TABLE Interviews (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    ResumeId    INTEGER NOT NULL,
    VacancyId   INTEGER NOT NULL,
    ScheduledAt TEXT    NOT NULL,
    Mode        INTEGER NOT NULL DEFAULT 0 CHECK (Mode IN (0, 1, 2)),     -- Onsite/Phone/Video
    Result      INTEGER NOT NULL DEFAULT 0 CHECK (Result IN (0, 1, 2, 3)), -- Scheduled/Passed/Failed/Cancelled
    Notes       TEXT,
    FOREIGN KEY (ResumeId)  REFERENCES Resumes (Id)  ON DELETE CASCADE,
    FOREIGN KEY (VacancyId) REFERENCES Vacancies (Id) ON DELETE RESTRICT
);
CREATE INDEX IX_Interviews_ScheduledAt ON Interviews (ScheduledAt);

CREATE TABLE Interviewers (
    Id       INTEGER PRIMARY KEY AUTOINCREMENT,
    FullName TEXT NOT NULL CHECK (length(FullName) BETWEEN 3 AND 200),
    Email    TEXT UNIQUE,
    Position TEXT
);

-- N:N  Interview <-> Interviewer через junction-таблицу с составным PK
CREATE TABLE InterviewInterviewers (
    InterviewId   INTEGER NOT NULL,
    InterviewerId INTEGER NOT NULL,
    Role          TEXT    NOT NULL DEFAULT 'Interviewer' CHECK (length(Role) <= 50),
    PRIMARY KEY (InterviewId, InterviewerId),
    FOREIGN KEY (InterviewId)   REFERENCES Interviews (Id)   ON DELETE CASCADE,
    FOREIGN KEY (InterviewerId) REFERENCES Interviewers (Id) ON DELETE CASCADE
);
