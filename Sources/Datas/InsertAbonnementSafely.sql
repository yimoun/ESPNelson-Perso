-- 2. Insérer les abonnements (150 abonnements sans DateFin)
INSERT INTO abonnement (Id, DateDebut, DateFin, Type, UtilisateurId)
SELECT LEFT(UUID(), 12), -- Génère un identifiant unique de 6 caractères
       NOW() - INTERVAL FLOOR(RAND() * 6) DAY, -- Début aléatoire dans les 6 derniers jours
       '2025-03-28 18:03:01.147746', -- DateFin sera mise à jour après l’insertion
       CASE WHEN RAND() > 0.5 THEN 'Mensuel' ELSE 'Hebdomadaire' END,
       FLOOR(RAND() * 37) + 1 -- ID utilisateur entre 1 et 37
FROM (SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9 UNION SELECT 10) a,
     (SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5 UNION SELECT 6 UNION SELECT 7 UNION SELECT 8 UNION SELECT 9 UNION SELECT 10) b,
     (SELECT 1 UNION SELECT 2 UNION SELECT 3 UNION SELECT 4 UNION SELECT 5) c
LIMIT 150;

-- 2.1 Mettre à jour DateFin en fonction du type d’abonnement
UPDATE abonnement 
SET DateFin = CASE 
    WHEN Type = 'Mensuel' THEN DATE_ADD(DateDebut, INTERVAL 30 DAY)
    WHEN Type = 'Hebdomadaire' THEN DATE_ADD(DateDebut, INTERVAL 7 DAY)
    ELSE DateDebut END
WHERE DateFin IS NULL;

  
