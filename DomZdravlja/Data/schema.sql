-- Kreiranje baze podataka za Dom Zdravlja aplikaciju
-- Pokreni: mysql -u root -p < schema.sql

CREATE DATABASE IF NOT EXISTS dom_zdravlja
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE dom_zdravlja;

CREATE TABLE IF NOT EXISTS ambulances (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS users (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Username VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Role INT NOT NULL,
    AmbulanceId INT NULL,
    FullName VARCHAR(200) NOT NULL,
    IsActive TINYINT(1) NOT NULL DEFAULT 1,
    CONSTRAINT FK_users_ambulances FOREIGN KEY (AmbulanceId) REFERENCES ambulances(Id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS medicines (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description VARCHAR(500) NOT NULL,
    Quantity INT NOT NULL,
    Unit VARCHAR(50) NOT NULL,
    MinimumStock INT NOT NULL,
    ExpiryDate DATETIME NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS requests (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    UserId INT NOT NULL,
    AmbulanceId INT NOT NULL,
    MedicineId INT NOT NULL,
    Quantity INT NOT NULL,
    Status INT NOT NULL,
    CreatedAt DATETIME NOT NULL,
    ProcessedAt DATETIME NULL,
    ModeratorId INT NULL,
    Note VARCHAR(500) NOT NULL DEFAULT '',
    CONSTRAINT FK_requests_users FOREIGN KEY (UserId) REFERENCES users(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_requests_ambulances FOREIGN KEY (AmbulanceId) REFERENCES ambulances(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_requests_medicines FOREIGN KEY (MedicineId) REFERENCES medicines(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_requests_moderators FOREIGN KEY (ModeratorId) REFERENCES users(Id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS stock_intakes (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    MedicineId INT NOT NULL,
    Quantity INT NOT NULL,
    ReceivedAt DATETIME NOT NULL,
    ReceivedByUserId INT NOT NULL,
    Note VARCHAR(500) NOT NULL DEFAULT '',
    UpdatedExpiryDate DATETIME NULL,
    CONSTRAINT FK_stock_intakes_medicines FOREIGN KEY (MedicineId) REFERENCES medicines(Id) ON DELETE RESTRICT,
    CONSTRAINT FK_stock_intakes_users FOREIGN KEY (ReceivedByUserId) REFERENCES users(Id) ON DELETE RESTRICT
) ENGINE=InnoDB;
