DELIMITER //

CREATE PROCEDURE SupprimerTicketsInactifs()
BEGIN
    DELETE FROM ticket
    WHERE EstPaye = 0 
    AND TIMESTAMPDIFF(HOUR, TempsArrive, NOW()) > 24
    AND Id IS NOT NULL; -- Vérification d'un index unique
END //

DELIMITER ;





-- Planification automatique de la procédure avec un MySQL Event

CREATE EVENT NettoyerTicketsInactifs
ON SCHEDULE EVERY 1 DAY
DO CALL SupprimerTicketsInactifs();

-- Exécution de la proécure stockée en mode "Safe Update"

SET SQL_SAFE_UPDATES = 0;

CALL SupprimerTicketsInactifs();

SET SQL_SAFE_UPDATES = 1;
