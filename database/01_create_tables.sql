-- HRMS: доменные таблицы (MySQL 8.4)
-- Identity-таблицы (AspNetUsers, AspNetRoles, ...) создаются EF Core-миграциями.
-- Скрипт рассчитан на чистую базу hrms: сначала удаляются доменные таблицы.

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

DROP TABLE IF EXISTS `InterviewInterviewers`;
DROP TABLE IF EXISTS `Interviews`;
DROP TABLE IF EXISTS `ResumeVacancyMatches`;
DROP TABLE IF EXISTS `JobApplications`;
DROP TABLE IF EXISTS `ResumeDetails`;
DROP TABLE IF EXISTS `Resumes`;
DROP TABLE IF EXISTS `Interviewers`;
DROP TABLE IF EXISTS `Vacancies`;
DROP TABLE IF EXISTS `Candidates`;

SET FOREIGN_KEY_CHECKS = 1;

CREATE TABLE `Candidates` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `LastName` varchar(100) NOT NULL,
    `FirstName` varchar(100) NOT NULL,
    `Email` varchar(256) NOT NULL,
    `Phone` varchar(40) DEFAULT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Candidates_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Vacancies` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `Title` varchar(200) NOT NULL,
    `Department` varchar(100) NOT NULL,
    `Description` varchar(4000) DEFAULT NULL,
    `SalaryMin` decimal(12,2) NOT NULL,
    `SalaryMax` decimal(12,2) DEFAULT NULL,
    `MinExperience` int NOT NULL DEFAULT 0,
    `RequiredSkills` varchar(1000) DEFAULT NULL,
    `Status` int NOT NULL,
    `PublishedAt` date NOT NULL,
    `AcceptingResumesUntil` date DEFAULT NULL,
    PRIMARY KEY (`Id`),
    KEY `IX_Vacancies_Status` (`Status`),
    CONSTRAINT `CK_Vacancy_SalaryRange` CHECK (`SalaryMax` IS NULL OR `SalaryMax` >= `SalaryMin`),
    CONSTRAINT `CK_Vacancy_MinExperience` CHECK (`MinExperience` >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Одно резюме на кандидата: уникальный CandidateId реализует связь 1:1.
CREATE TABLE `Resumes` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CandidateId` int NOT NULL,
    `Headline` varchar(200) NOT NULL,
    `Summary` varchar(2000) NOT NULL,
    `ExperienceYears` int NOT NULL,
    `DesiredSalary` decimal(12,2) DEFAULT NULL,
    `CreatedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Resumes_CandidateId` (`CandidateId`),
    KEY `IX_Resumes_ExperienceYears` (`ExperienceYears`),
    CONSTRAINT `FK_Resumes_Candidates_CandidateId` FOREIGN KEY (`CandidateId`) REFERENCES `Candidates` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `CK_Resume_Experience` CHECK (`ExperienceYears` >= 0)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `ResumeDetails` (
    `ResumeId` int NOT NULL,
    `Education` varchar(500) DEFAULT NULL,
    `Skills` varchar(1000) DEFAULT NULL,
    `FilePath` varchar(1000) DEFAULT NULL,
    PRIMARY KEY (`ResumeId`),
    CONSTRAINT `FK_ResumeDetails_Resumes_ResumeId` FOREIGN KEY (`ResumeId`) REFERENCES `Resumes` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Отклик: связь N:N между кандидатом и вакансией с этапом воронки.
CREATE TABLE `JobApplications` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `CandidateId` int NOT NULL,
    `VacancyId` int NOT NULL,
    `Stage` int NOT NULL,
    `AppliedAt` datetime(6) NOT NULL,
    `UpdatedAt` datetime(6) NOT NULL,
    `Source` varchar(100) DEFAULT NULL,
    `Notes` varchar(1000) DEFAULT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_JobApplications_CandidateId_VacancyId` (`CandidateId`, `VacancyId`),
    KEY `IX_JobApplications_AppliedAt` (`AppliedAt`),
    KEY `IX_JobApplications_Stage` (`Stage`),
    KEY `IX_JobApplications_VacancyId` (`VacancyId`),
    CONSTRAINT `FK_JobApplications_Candidates_CandidateId` FOREIGN KEY (`CandidateId`) REFERENCES `Candidates` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_JobApplications_Vacancies_VacancyId` FOREIGN KEY (`VacancyId`) REFERENCES `Vacancies` (`Id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Подбор: связь N:N между резюме и вакансией с оценкой соответствия.
CREATE TABLE `ResumeVacancyMatches` (
    `ResumeId` int NOT NULL,
    `VacancyId` int NOT NULL,
    `Status` int NOT NULL,
    `MatchScore` int DEFAULT NULL,
    `MatchedAt` datetime(6) NOT NULL,
    `Comment` varchar(500) DEFAULT NULL,
    PRIMARY KEY (`ResumeId`, `VacancyId`),
    KEY `IX_ResumeVacancyMatches_Status` (`Status`),
    KEY `IX_ResumeVacancyMatches_VacancyId` (`VacancyId`),
    CONSTRAINT `FK_ResumeVacancyMatches_Resumes_ResumeId` FOREIGN KEY (`ResumeId`) REFERENCES `Resumes` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_ResumeVacancyMatches_Vacancies_VacancyId` FOREIGN KEY (`VacancyId`) REFERENCES `Vacancies` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `CK_Match_Score` CHECK (`MatchScore` IS NULL OR (`MatchScore` >= 0 AND `MatchScore` <= 100))
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `Interviewers` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `FullName` varchar(200) NOT NULL,
    `Email` varchar(256) DEFAULT NULL,
    `Position` varchar(200) DEFAULT NULL,
    PRIMARY KEY (`Id`),
    UNIQUE KEY `IX_Interviewers_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Собеседование принадлежит отклику: кандидат и вакансия однозначно следуют из него.
CREATE TABLE `Interviews` (
    `Id` int NOT NULL AUTO_INCREMENT,
    `ApplicationId` int NOT NULL,
    `ScheduledAt` datetime(6) NOT NULL,
    `Mode` int NOT NULL,
    `Result` int NOT NULL,
    `Notes` longtext,
    PRIMARY KEY (`Id`),
    KEY `IX_Interviews_ScheduledAt` (`ScheduledAt`),
    KEY `IX_Interviews_ApplicationId` (`ApplicationId`),
    CONSTRAINT `FK_Interviews_JobApplications_ApplicationId` FOREIGN KEY (`ApplicationId`) REFERENCES `JobApplications` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `InterviewInterviewers` (
    `InterviewId` int NOT NULL,
    `InterviewerId` int NOT NULL,
    `Role` varchar(50) NOT NULL,
    PRIMARY KEY (`InterviewId`, `InterviewerId`),
    KEY `IX_InterviewInterviewers_InterviewerId` (`InterviewerId`),
    CONSTRAINT `FK_InterviewInterviewers_Interviews_InterviewId` FOREIGN KEY (`InterviewId`) REFERENCES `Interviews` (`Id`) ON DELETE CASCADE,
    CONSTRAINT `FK_InterviewInterviewers_Interviewers_InterviewerId` FOREIGN KEY (`InterviewerId`) REFERENCES `Interviewers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
