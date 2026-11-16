-- HRMS: схема базы данных (MySQL 8.4)
-- Создание таблиц, связей (1:1, 1:N, N:N), ограничений, ключей.
-- Скрипт повторяет логическую схему; миграции EF создают ту же схему при старте приложения.

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
    Id        INT PRIMARY KEY AUTO_INCREMENT,
    LastName  VARCHAR(100) NOT NULL CHECK (CHAR_LENGTH(LastName) BETWEEN 2 AND 100),
    FirstName VARCHAR(100) NOT NULL CHECK (CHAR_LENGTH(FirstName) BETWEEN 2 AND 100),
    Email     VARCHAR(256) NOT NULL UNIQUE,
    Phone     TEXT
);

CREATE TABLE Vacancies (
    Id                    INT PRIMARY KEY AUTO_INCREMENT,
    Title                 VARCHAR(200)  NOT NULL CHECK (CHAR_LENGTH(Title) BETWEEN 3 AND 200),
    Department            VARCHAR(100)  NOT NULL CHECK (CHAR_LENGTH(Department) BETWEEN 2 AND 100),
    Description           TEXT,
    SalaryMin             DECIMAL(12,2) NOT NULL CHECK (SalaryMin >= 0),
    SalaryMax             DECIMAL(12,2),
    Status                INT NOT NULL DEFAULT 1 CHECK (Status IN (0, 1, 2, 3)), -- Draft/Open/Paused/Closed
    PublishedAt           DATE NOT NULL,
    AcceptingResumesUntil DATE,
    CHECK (SalaryMax IS NULL OR SalaryMax >= SalaryMin)
);

CREATE TABLE Resumes (
    Id              INT PRIMARY KEY AUTO_INCREMENT,
    CandidateId     INT NOT NULL,
    VacancyId       INT,
    Summary         VARCHAR(2000) NOT NULL CHECK (CHAR_LENGTH(Summary) BETWEEN 10 AND 2000),
    ExperienceYears INT NOT NULL DEFAULT 0 CHECK (ExperienceYears >= 0),
    SubmittedAt     DATETIME NOT NULL,
    Status          INT NOT NULL DEFAULT 0 CHECK (Status IN (0, 1, 2, 3)), -- New/InReview/Rejected/Accepted
    FOREIGN KEY (CandidateId) REFERENCES Candidates (Id)  ON DELETE CASCADE,
    FOREIGN KEY (VacancyId)   REFERENCES Vacancies (Id)   ON DELETE SET NULL
);
CREATE INDEX IX_Resumes_Status      ON Resumes (Status);
CREATE INDEX IX_Resumes_CandidateId ON Resumes (CandidateId);

-- 1:1  Resume <-> ResumeDetails (PK = FK на Resume, cascade)
CREATE TABLE ResumeDetails (
    ResumeId  INT PRIMARY KEY,
    Education TEXT,
    Skills    TEXT,
    FilePath  TEXT,
    FOREIGN KEY (ResumeId) REFERENCES Resumes (Id) ON DELETE CASCADE
);

CREATE TABLE Interviews (
    Id          INT PRIMARY KEY AUTO_INCREMENT,
    ResumeId    INT NOT NULL,
    VacancyId   INT NOT NULL,
    ScheduledAt DATETIME NOT NULL,
    Mode        INT NOT NULL DEFAULT 0 CHECK (Mode IN (0, 1, 2)),        -- Onsite/Phone/Video
    Result      INT NOT NULL DEFAULT 0 CHECK (Result IN (0, 1, 2, 3)),   -- Scheduled/Passed/Failed/Cancelled
    Notes       TEXT,
    FOREIGN KEY (ResumeId)  REFERENCES Resumes (Id)  ON DELETE CASCADE,
    FOREIGN KEY (VacancyId) REFERENCES Vacancies (Id) ON DELETE RESTRICT
);
CREATE INDEX IX_Interviews_ScheduledAt ON Interviews (ScheduledAt);

CREATE TABLE Interviewers (
    Id       INT PRIMARY KEY AUTO_INCREMENT,
    FullName VARCHAR(200) NOT NULL CHECK (CHAR_LENGTH(FullName) BETWEEN 3 AND 200),
    Email    VARCHAR(256) UNIQUE,
    Position TEXT
);

-- N:N  Interview <-> Interviewer через junction-таблицу с составным PK
CREATE TABLE InterviewInterviewers (
    InterviewId   INT NOT NULL,
    InterviewerId INT NOT NULL,
    Role          VARCHAR(50) NOT NULL DEFAULT 'Interviewer' CHECK (CHAR_LENGTH(Role) <= 50),
    PRIMARY KEY (InterviewId, InterviewerId),
    FOREIGN KEY (InterviewId)   REFERENCES Interviews (Id)   ON DELETE CASCADE,
    FOREIGN KEY (InterviewerId) REFERENCES Interviewers (Id) ON DELETE CASCADE
);
