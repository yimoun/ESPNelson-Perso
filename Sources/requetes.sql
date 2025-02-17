DROP TABLE IF EXISTS Paiement;
DROP TABLE IF EXISTS Ticket;
DROP TABLE IF EXISTS Abonnement;
DROP TABLE IF EXISTS Utilisateur;
DROP TABLE IF EXISTS Tarification;
DROP TABLE IF EXISTS Configuration;
DROP TABLE IF EXISTS Rapport;

-- Table Utilisateur
CREATE TABLE Utilisateur (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    NomUtilisateur VARCHAR(205) NOT NULL,
    MotDePasse VARCHAR(205) NOT NULL,
    Role VARCHAR(205) NOT NULL,
    Email VARCHAR(205) UNIQUE NOT NULL,
    BadgeId VARCHAR(50) NULL
);

-- Table Tarification
CREATE TABLE Tarification (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    Niveau VARCHAR(50) NOT NULL,
    Prix DECIMAL(10,2) NOT NULL,
    DureeMin INT NOT NULL,
    DureeMax INT NOT NULL
);

-- 🟢 Table Ticket
CREATE TABLE Ticket (
    Id VARCHAR(36) PRIMARY KEY,  -- UUID au lieu d'Auto-Incrément
    TempsArrive DATETIME NOT NULL DEFAULT NOW(),
    EstPaye BOOLEAN DEFAULT FALSE,
    TempsSortie DATETIME NULL,
    EstConverti BOOLEAN DEFAULT FALSE  -- Indique si le ticket a été utilisé pour souscrire à un abonnement
);

-- Table Abonnement
CREATE TABLE Abonnement (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    UtilisateurId INT NOT NULL,
    DateDebut DATETIME NOT NULL DEFAULT NOW(),
    DateFin DATETIME NOT NULL,
    FOREIGN KEY (UtilisateurId) REFERENCES Utilisateur(Id) ON DELETE CASCADE
);

-- Table Paiement
CREATE TABLE Paiement (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    TicketId VARCHAR(36) NULL,
    AbonnementId INT NULL,
    Montant DECIMAL(10,2) NOT NULL,
    DatePaiement DATETIME NOT NULL DEFAULT NOW(),
    TarificationId INT NULL,
    FOREIGN KEY (TicketId) REFERENCES Ticket(Id) ON DELETE CASCADE,
    FOREIGN KEY (AbonnementId) REFERENCES Abonnement(Id) ON DELETE CASCADE,
    FOREIGN KEY (TarificationId) REFERENCES Tarification(Id) ON DELETE SET NULL
);

-- Table Configuration
CREATE TABLE Configuration (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    CapaciteMax INT NOT NULL,
    DureeGratuite INT NOT NULL,
    TaxeFederale DECIMAL(10,2) NOT NULL,
    TaxeProvinciale DECIMAL(10,2) NOT NULL,
    DateModification DATETIME NOT NULL DEFAULT NOW()
);

-- Table Rapport
CREATE TABLE Rapport (
    Id INT PRIMARY KEY AUTO_INCREMENT,
    DateGeneration DATETIME NOT NULL DEFAULT NOW(),
    DateDebut DATETIME NOT NULL,
    DateFin DATETIME NOT NULL,
    Fichier VARCHAR(205) NULL,
    UtilisateurId INT NOT NULL,
    FOREIGN KEY (UtilisateurId) REFERENCES Utilisateur(Id) ON DELETE CASCADE
);

-- Insertion des tarifcation prédéfinies
INSERT INTO Tarification (Niveau, Prix, DureeMin, DureeMax)
VALUES 
('Tarif horaire', 0, 0, 2),
('Demi-journée', 6.25, 1, 4),
('Journée complète', 10.75, 4, 24);


-- Vue pour afficher uniquement les abonnements actifs
DROP VIEW IF EXISTS Vue_AbonnementsActifs;
CREATE VIEW Vue_AbonnementsActifs AS
SELECT a.Id, u.Email, a.DateDebut, a.DateFin, p.Montant
FROM Abonnement a
JOIN Utilisateur u ON a.UtilisateurId = u.Id
LEFT JOIN Paiement p ON a.Id = p.AbonnementId
WHERE a.DateFin > NOW();
