DELIMITER //

CREATE PROCEDURE InsertPaiementsSafely()
BEGIN
    DECLARE v_ticketId VARCHAR(255);
    DECLARE v_abonnementId VARCHAR(255);
    DECLARE v_montant DECIMAL(10, 2);
    DECLARE v_datePaiement DATETIME;
    DECLARE v_tarificationNiveau VARCHAR(50);
    DECLARE v_tarificationPrix DECIMAL(10, 2);
    DECLARE v_tarificationDureeMin INT;
    DECLARE v_tarificationDureeMax INT;
    DECLARE v_counter INT DEFAULT 0;

    -- Ajouter un label à la boucle WHILE
    insertion_loop: WHILE v_counter < 600 DO
        -- Sélectionner un TicketId ou un AbonnementId non utilisé
        IF RAND() < 0.5 THEN
            -- Paiement pour un ticket (AbonnementId IS NULL)
            SELECT Id, TempsSortie INTO v_ticketId, v_datePaiement
            FROM Ticket
            WHERE Id NOT IN (SELECT TicketId FROM Paiement WHERE TicketId IS NOT NULL)
              AND TempsSortie IS NOT NULL -- Garantir que TempsSortie n'est pas NULL
            ORDER BY RAND()
            LIMIT 1;

            IF v_ticketId IS NOT NULL THEN
                SET v_abonnementId = NULL;
            ELSE
                -- Aucun ticket valide trouvé, passer à l'itération suivante
                ITERATE insertion_loop; -- Utiliser le label de la boucle
            END IF;
        ELSE
            -- Paiement pour un abonnement (TicketId IS NULL)
            SELECT Id, DateDebut INTO v_abonnementId, v_datePaiement
            FROM Abonnement
            WHERE Id NOT IN (SELECT AbonnementId FROM Paiement WHERE AbonnementId IS NOT NULL)
              AND DateDebut IS NOT NULL -- Garantir que DateDebut n'est pas NULL
            ORDER BY RAND()
            LIMIT 1;

            IF v_abonnementId IS NOT NULL THEN
                SET v_ticketId = NULL;
            ELSE
                -- Aucun abonnement valide trouvé, passer à l'itération suivante
                ITERATE insertion_loop; -- Utiliser le label de la boucle
            END IF;
        END IF;

        -- Générer des valeurs aléatoires pour les autres colonnes
        SET v_montant = CASE 
            WHEN RAND() < 0.33 THEN 2.50  
            WHEN RAND() < 0.66 THEN 6.25 
            ELSE 10.75 
        END;

        SET v_tarificationNiveau = CASE 
            WHEN RAND() < 0.33 THEN 'Tarif horaire'
            WHEN RAND() < 0.66 THEN 'Demi-journée'
            ELSE 'Journée complète'
        END;

        SET v_tarificationPrix = CASE 
            WHEN RAND() < 0.33 THEN 2.50  
            WHEN RAND() < 0.66 THEN 6.25 
            ELSE 10.75 
        END;

        SET v_tarificationDureeMin = CASE 
            WHEN RAND() < 0.33 THEN 1  
            WHEN RAND() < 0.66 THEN 1 
            ELSE 4 
        END;

        SET v_tarificationDureeMax = CASE 
            WHEN RAND() < 0.33 THEN 1  
            WHEN RAND() < 0.66 THEN 4 
            ELSE 24 
        END;

        -- Insérer l'enregistrement dans Paiement
        INSERT INTO Paiement (TicketId, AbonnementId, Montant, DatePaiement, TarificationNiveau, TarificationPrix, TarificationDureeMin, TarificationDureeMax)
        VALUES (v_ticketId, v_abonnementId, v_montant, v_datePaiement, v_tarificationNiveau, v_tarificationPrix, v_tarificationDureeMin, v_tarificationDureeMax);

        -- Incrémenter le compteur
        SET v_counter = v_counter + 1;
    END WHILE insertion_loop; -- Fermer la boucle avec le label
END //

DELIMITER ;