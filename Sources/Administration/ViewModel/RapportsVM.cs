using Administration.Data;
using Administration.Data.Context;
using Administration.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Administration.ViewModel
{
    public partial class RapportsVM : ObservableObject
    {
        private readonly AdministrationContext _dbContext;

        [ObservableProperty]
        private DateTime dateDebut = DateTime.Now; // Date de début par défaut

        [ObservableProperty]
        private DateTime dateFin = DateTime.Now; // Date de fin par défaut

        public ICommand GenererRapportCommand { get; }
        public ICommand ExporterRapportCommand { get; }



        public RapportsVM()
        {
            AdministrationContextFactory factory = new AdministrationContextFactory();
            _dbContext = factory.CreateDbContext(new string[0]);

            GenererRapportCommand = new RelayCommand(GenererRapport);
            ExporterRapportCommand = new RelayCommand(ExporterRapport);
        }

        private void GenererRapport()
        {
            // Vérifier que les dates sont valides
            if (DateDebut > DateFin)
            {
                MessageBox.Show("La date de début ne peut pas être postérieure à la date de fin.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Générer le rapport
            var rapport = GenererRapportRevenus(DateDebut, DateFin);

            // Afficher les résultats dans l'interface (à implémenter)
            // Par exemple, vous pouvez stocker les résultats dans des propriétés ObservableProperty
            // et les afficher dans un DataGrid ou un TextBlock.
        }

        private void ExporterRapport()
        {
            // Vérifier que les dates sont valides
            if (DateDebut > DateFin)
            {
                MessageBox.Show("La date de début ne peut pas être postérieure à la date de fin.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Générer le rapport
            var rapport = GenererRapportRevenus(DateDebut, DateFin);

            // Exporter le rapport en PDF
            ExporterRapportEnPDF(rapport.TicketsParTarification, rapport.RevenusParTarification, rapport.TotalRevenus,
                rapport.RevenusParConfiguration, rapport.RevenusAbonnements, "RapportRevenus.pdf");
        }

        public Tarification GetTarificationApplicable(Ticket ticket)
        {
            if (ticket.TempsSortie == null)
                throw new InvalidOperationException("Le ticket n'a pas de temps de sortie.");

            TimeSpan dureeStationnement = ticket.TempsSortie.Value - ticket.TempsArrive;
            double dureeEnHeures = dureeStationnement.TotalHours;

            return _dbContext.Tarifications
                .FirstOrDefault(t => dureeEnHeures >= t.DureeMin && dureeEnHeures <= t.DureeMax);
        }


        public (Dictionary<string, int> TicketsParTarification, Dictionary<string, decimal> RevenusParTarification, decimal TotalRevenus,
            List<(Configuration Config, decimal Revenus)> RevenusParConfiguration, decimal RevenusAbonnements)
            GenererRapportRevenus(DateTime dateDebut, DateTime dateFin)
        {
            // Récupérer les paiements dans la période donnée
            var paiements = _dbContext.Paiements
                .Include(p => p.Ticket)
                .Where(p => p.DatePaiement >= dateDebut && p.DatePaiement <= dateFin)
                .ToList();

            // Récupérer les paiements associés aux abonnements
            var paiementsAbonnements = paiements
                .Where(p => p.AbonnementId != null)
                .ToList();

            // Récupérer les configurations actives pendant cette période
            var configurationsActives = _dbContext.Configurations
                .Where(c => c.DateModification >= dateDebut && c.DateModification <= dateFin)
                .OrderBy(c => c.DateModification)
                .ToList();

            // Dictionnaires pour stocker les résultats
            var ticketsParTarification = new Dictionary<string, int>();
            var revenusParTarification = new Dictionary<string, decimal>();
            decimal totalRevenus = 0;
            decimal revenusAbonnements = paiementsAbonnements.Sum(p => p.Montant);

            // Liste pour stocker les revenus segmentés par configuration
            var revenusParConfiguration = new List<(Configuration Config, decimal Revenus)>();

            // Parcourir les configurations actives pour segmenter les revenus
            for (int i = 0; i < configurationsActives.Count; i++)
            {
                var config = configurationsActives[i];
                DateTime debutPeriode = i == 0 ? dateDebut : config.DateModification;
                DateTime finPeriode = i == configurationsActives.Count - 1 ? dateFin : configurationsActives[i + 1].DateModification;

                // Récupérer les paiements pour cette période (hors abonnements)
                var paiementsPeriode = paiements
                    .Where(p => p.DatePaiement >= debutPeriode && p.DatePaiement < finPeriode && p.AbonnementId == null)
                    .ToList();

                // Calculer les revenus pour cette période
                decimal revenusPeriode = paiementsPeriode.Sum(p => p.Montant);
                revenusParConfiguration.Add((config, revenusPeriode));

                // Parcourir les paiements pour agréger les données par tarification
                foreach (var paiement in paiementsPeriode)
                {
                    string niveauTarification = paiement.TarificationNiveau;

                    // Mettre à jour le nombre de tickets par tarification
                    if (ticketsParTarification.ContainsKey(niveauTarification))
                        ticketsParTarification[niveauTarification]++;
                    else
                        ticketsParTarification[niveauTarification] = 1;

                    // Mettre à jour les revenus par tarification
                    if (revenusParTarification.ContainsKey(niveauTarification))
                        revenusParTarification[niveauTarification] += paiement.Montant;
                    else
                        revenusParTarification[niveauTarification] = paiement.Montant;

                    // Ajouter au total des revenus
                    totalRevenus += paiement.Montant;
                }
            }

            return (ticketsParTarification, revenusParTarification, totalRevenus, revenusParConfiguration, revenusAbonnements);
        }



        public void ExporterRapportEnPDF(Dictionary<string, int> ticketsParTarification, Dictionary<string, decimal> revenusParTarification, decimal totalRevenus, List<(Configuration Config, decimal Revenus)> revenusParConfiguration, decimal revenusAbonnements, string cheminFichier)
            {
                // Créer un nouveau document PDF
                PdfDocument document = new PdfDocument();
                PdfPage page = document.AddPage();
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Définir les polices
                XFont fontTitre = new XFont("Arial", 20/*, XFontStyle.Bold*/);
                XFont fontSousTitre = new XFont("Arial", 14/*, XFontStyle.Bold*/);
                XFont fontTexte = new XFont("Arial", 12);
                XFont fontTableHeader = new XFont("Arial", 12/*, XFontStyle.Bold*/);
                XFont fontTableContent = new XFont("Arial", 12);

                // Définir les marges
                double margeGauche = 50;
                double margeHaut = 50;
                double margeDroite = 50;
                double margeBas = 50;
                double largeurPage = page.Width;
                double hauteurPage = page.Height;

                // Position verticale actuelle
                double y = margeHaut;

                // Ajouter l'en-tête
                gfx.DrawString("Hôpital de Chicoutimi", fontTitre, XBrushes.Black, new XPoint((largeurPage - gfx.MeasureString("Hôpital de Chicoutimi", fontTitre).Width) / 2, y));
                y += 40;

                // Ajouter l'image (remplacez "logo.png" par le chemin de votre image)
                string cheminImage = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                if (File.Exists(cheminImage))
                {
                    XImage image = XImage.FromFile(cheminImage);
                    double largeurImage = 100; // Ajustez la largeur de l'image
                    double hauteurImage = image.PixelHeight * (largeurImage / image.PixelWidth);
                    gfx.DrawImage(image, (largeurPage - largeurImage) / 2, y, largeurImage, hauteurImage);
                    y += hauteurImage + 20;
                }

                // Ajouter le titre du rapport
                string titreRapport = $"Rapport de revenus du {DateDebut:dd/MM/yyyy} au {DateFin:dd/MM/yyyy}";
                gfx.DrawString(titreRapport, fontSousTitre, XBrushes.Black, new XPoint((largeurPage - gfx.MeasureString(titreRapport, fontSousTitre).Width) / 2, y));
                y += 40;

                // Ajouter le tableau des tickets payés par type de tarification
                double largeurColonne1 = 200; // Largeur de la colonne "Type de tarification"
                double largeurColonne2 = 150; // Largeur de la colonne "Nombre de tickets"
                double largeurColonne3 = 150; // Largeur de la colonne "Revenu total"

                // En-tête du tableau
                gfx.DrawString("Type de tarification", fontTableHeader, XBrushes.Black, new XPoint(margeGauche, y));
                gfx.DrawString("Nombre de tickets", fontTableHeader, XBrushes.Black, new XPoint(margeGauche + largeurColonne1, y));
                gfx.DrawString("Revenu total", fontTableHeader, XBrushes.Black, new XPoint(margeGauche + largeurColonne1 + largeurColonne2, y));
                y += 20;

                // Ligne de séparation
                gfx.DrawLine(XPens.Black, margeGauche, y, largeurPage - margeDroite, y);
                y += 10;

                // Contenu du tableau
                foreach (var kvp in ticketsParTarification)
                {
                    gfx.DrawString(kvp.Key, fontTableContent, XBrushes.Black, new XPoint(margeGauche, y));
                    gfx.DrawString(kvp.Value.ToString(), fontTableContent, XBrushes.Black, new XPoint(margeGauche + largeurColonne1, y));
                    gfx.DrawString($"{revenusParTarification[kvp.Key]:C}", fontTableContent, XBrushes.Black, new XPoint(margeGauche + largeurColonne1 + largeurColonne2, y));
                    y += 20;
                }

                // Ligne de séparation
                gfx.DrawLine(XPens.Black, margeGauche, y, largeurPage - margeDroite, y);
                y += 20;

                // Ajouter le total des revenus
                gfx.DrawString($"Total des revenus: {totalRevenus:C}", fontTexte, XBrushes.Black, new XPoint(margeGauche, y));
                y += 20;

                // Ajouter les revenus des abonnements
                gfx.DrawString($"Revenus des abonnements: {revenusAbonnements:C}", fontTexte, XBrushes.Black, new XPoint(margeGauche, y));
                y += 20;

                // Ajouter le total intégrant les taxes
                decimal totalAvecTaxes = totalRevenus + revenusAbonnements;
                gfx.DrawString($"Total intégrant les taxes: {totalAvecTaxes:C}", fontTexte, XBrushes.Black, new XPoint(margeGauche, y));
                y += 40;

                // Ajouter le pied de page avec la pagination
                string piedDePage = "Page 1"; // Vous pouvez ajouter une logique de pagination si nécessaire
                gfx.DrawString(piedDePage, fontTexte, XBrushes.Black, new XPoint((largeurPage - gfx.MeasureString(piedDePage, fontTexte).Width) / 2, hauteurPage - margeBas));

                // Sauvegarder le document
                document.Save(cheminFichier);

                // Ouvrir le fichier PDF
                Process.Start(new ProcessStartInfo(cheminFichier) { UseShellExecute = true });
            }

        }
       
    }
