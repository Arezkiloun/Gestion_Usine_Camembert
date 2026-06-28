using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GestionUsine;

public sealed class DashboardForm : Form
{
    private const string NomUsine =
        "LAITERIE MEDJDOUB";

    private const string LocaliteUsine =
        "TOGHZA";

    private const string NomFichierLogo =
        "laiterie.jpg";

    private readonly Image? logoUsine;

    private readonly Panel contenuPrincipal;
    private readonly Label lblTitrePage;
    private readonly Label lblDateHeure;
    private readonly Button btnActualiserPage;

    private readonly Dictionary<string, Button>
        boutonsMenu = new();

    private string moduleActuel =
        "Tableau de bord";

    public DashboardForm()
    {
        logoUsine =
            ChargerLogoUsine();

        Text =
            NomUsine +
            " - Tableau de bord";
        StartPosition =
            FormStartPosition.CenterScreen;
        WindowState =
            FormWindowState.Maximized;
        MinimumSize =
            new Size(1100, 700);
        BackColor =
            Color.FromArgb(242, 245, 248);
        AutoScaleMode =
            AutoScaleMode.Dpi;

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                310
            )
        );

        structure.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        Controls.Add(structure);

        structure.Controls.Add(
            CreerMenuLateral(),
            0,
            0
        );

        var zoneDroite =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        zoneDroite.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                82
            )
        );

        zoneDroite.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        structure.Controls.Add(
            zoneDroite,
            1,
            0
        );

        var barreSuperieure =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.White,
                Padding =
                    new Padding(28, 12, 28, 12),
                Margin = Padding.Empty
            };

        barreSuperieure.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        barreSuperieure.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                145
            )
        );

        barreSuperieure.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                180
            )
        );

        zoneDroite.Controls.Add(
            barreSuperieure,
            0,
            0
        );

        lblTitrePage = new Label
        {
            Text =
                NomUsine +
                " — Tableau de bord",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                18,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        btnActualiserPage = new Button
        {
            Text = "Actualiser",
            Dock = DockStyle.Fill,
            BackColor =
                Color.FromArgb(32, 113, 171),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            Cursor = Cursors.Hand,
            Margin = new Padding(5, 8, 5, 8)
        };

        btnActualiserPage
            .FlatAppearance
            .BorderSize = 0;

        btnActualiserPage.Click += (_, _) =>
        {
            if (
                moduleActuel ==
                "Tableau de bord")
            {
                AfficherTableauDeBord();
            }
            else
            {
                OuvrirModule(
                    moduleActuel
                );
            }
        };

        lblDateHeure = new Label
        {
            Text = DateTime.Now.ToString(
                "dd/MM/yyyy HH:mm"
            ),
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                10.5F,
                FontStyle.Regular
            ),
            ForeColor = Color.DimGray,
            TextAlign =
                ContentAlignment.MiddleRight
        };

        barreSuperieure.Controls.Add(
            lblTitrePage,
            0,
            0
        );

        barreSuperieure.Controls.Add(
            btnActualiserPage,
            1,
            0
        );

        barreSuperieure.Controls.Add(
            lblDateHeure,
            2,
            0
        );

        var horloge =
            new System.Windows.Forms.Timer
            {
                Interval = 30_000
            };

        horloge.Tick += (_, _) =>
        {
            lblDateHeure.Text =
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm"
                );
        };

        horloge.Start();

        contenuPrincipal = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor =
                Color.FromArgb(242, 245, 248),
            Padding = new Padding(24),
            AutoScroll = true
        };

        zoneDroite.Controls.Add(
            contenuPrincipal,
            0,
            1
        );

        AfficherTableauDeBord();
    }

    private Panel CreerMenuLateral()
    {
        var panneau = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor =
                Color.FromArgb(25, 49, 78),
            Padding = new Padding(12, 10, 12, 10)
        };

        var structureMenu =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor =
                    Color.FromArgb(25, 49, 78)
            };

        structureMenu.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                255
            )
        );

        structureMenu.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        panneau.Controls.Add(
            structureMenu
        );

        var zoneMarque = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor =
                Color.FromArgb(22, 44, 70),
            Padding =
                new Padding(10, 8, 10, 10),
            Margin =
                new Padding(0, 0, 0, 10)
        };

        var marqueLayout =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        marqueLayout.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                125
            )
        );

        marqueLayout.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                78
            )
        );

        marqueLayout.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                32
            )
        );

        var picLogo = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode =
                PictureBoxSizeMode.Zoom,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Margin =
                new Padding(28, 4, 28, 8),
            Image = logoUsine
        };

        var lblNomUsine = new Label
        {
            Text =
                "LAITERIE\r\nMEDJDOUB",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                13,
                FontStyle.Bold
            ),
            ForeColor = Color.White,
            TextAlign =
                ContentAlignment.MiddleCenter,
            Padding =
                new Padding(0, 4, 0, 4),
            Margin = Padding.Empty
        };

        var lblLocalite = new Label
        {
            Text = LocaliteUsine,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(120, 205, 130),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Padding = Padding.Empty,
            Margin = Padding.Empty
        };

        marqueLayout.Controls.Add(
            picLogo,
            0,
            0
        );

        marqueLayout.Controls.Add(
            lblNomUsine,
            0,
            1
        );

        marqueLayout.Controls.Add(
            lblLocalite,
            0,
            2
        );

        zoneMarque.Controls.Add(
            marqueLayout
        );

        structureMenu.Controls.Add(
            zoneMarque,
            0,
            0
        );

        var listeMenu =
            new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection =
                    FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding =
                    new Padding(0, 2, 0, 10),
                Margin = Padding.Empty,
                BackColor =
                    Color.FromArgb(25, 49, 78)
            };

        structureMenu.Controls.Add(
            listeMenu,
            0,
            1
        );

        string[] modules =
        {
            "Tableau de bord",
            "Employés",
            "Produits",
            "Stock produits",
            "Stock emballages",
            "Clients",
            "Ventes",
            "Crédits clients",
            "Dépenses",
            "Recettes",
            "Caisse",
            "Impressions",
            "Paramètres"
        };

        foreach (string module in modules)
        {
            Button bouton =
                CreerBoutonMenu(module);

            bouton.Click += (_, _) =>
            {
                OuvrirModule(module);
            };

            boutonsMenu[module] =
                bouton;

            listeMenu.Controls.Add(
                bouton
            );
        }

        var btnDeconnexion =
            CreerBoutonMenu(
                "Déconnexion"
            );

        btnDeconnexion.BackColor =
            Color.FromArgb(155, 48, 48);

        btnDeconnexion.Click += (_, _) =>
        {
            DialogResult resultat =
                MessageBox.Show(
                    "Voulez-vous vous déconnecter ?",
                    "Déconnexion",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (
                resultat ==
                DialogResult.Yes)
            {
                Close();
            }
        };

        listeMenu.Controls.Add(
            btnDeconnexion
        );

        return panneau;
    }

    private static Button CreerBoutonMenu(
        string texte)
    {
        var bouton = new Button
        {
            Text = texte,
            Width = 276,
            Height = 46,
            FlatStyle = FlatStyle.Flat,
            BackColor =
                Color.FromArgb(35, 66, 101),
            ForeColor = Color.White,
            Font = new Font(
                "Segoe UI",
                10
            ),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding =
                new Padding(16, 0, 0, 0),
            Cursor = Cursors.Hand,
            Margin =
                new Padding(5, 4, 5, 4)
        };

        bouton.FlatAppearance.BorderSize = 0;

        return bouton;
    }

    private void MettreMenuActif(
        string module)
    {
        foreach (
            KeyValuePair<string, Button>
            element
            in boutonsMenu)
        {
            bool actif =
                element.Key == module;

            element.Value.BackColor =
                actif
                    ? Color.FromArgb(
                        42,
                        125,
                        177
                    )
                    : Color.FromArgb(
                        35,
                        66,
                        101
                    );

            element.Value.Font =
                new Font(
                    "Segoe UI",
                    10,
                    actif
                        ? FontStyle.Bold
                        : FontStyle.Regular
                );
        }
    }

    private void OuvrirModule(
        string module)
    {
        module = module.Trim();

        moduleActuel = module;
        lblTitrePage.Text = module;
        contenuPrincipal.Controls.Clear();

        MettreMenuActif(module);

        if (
            module ==
            "Tableau de bord")
        {
            AfficherTableauDeBord();
            return;
        }

        try
        {
            Control? controle =
                module switch
                {
                    "Employés" =>
                        new EmployesControl(),

                    "Produits" =>
                        new ProduitsControl(),

                    "Stock produits" =>
                        new StockProduitsControl(),

                    "Stock emballages" =>
                        new StockEmballagesControl(),

                    "Clients" =>
                        new ClientsControl(),

                    "Ventes" =>
                        new VentesControl(),

                    "Crédits clients" =>
                        new CreditsClientsControl(),

                    "Dépenses" =>
                        new DepensesControl(),

                    "Recettes" =>
                        new RecettesControl(),

                    "Caisse" =>
                        new CaisseControl(),

                    "Impressions" =>
                        new ImpressionsControl(),

                    "Paramètres" =>
                        new ParametresControl(),

                    _ => null
                };

            if (controle == null)
            {
                AfficherModuleIndisponible(
                    module
                );

                return;
            }

            controle.Dock =
                DockStyle.Fill;

            contenuPrincipal.Controls.Add(
                controle
            );
        }
        catch (Exception ex)
        {
            AfficherErreurModule(
                module,
                ex
            );
        }
    }

    private void AfficherTableauDeBord()
    {
        moduleActuel =
            "Tableau de bord";

        lblTitrePage.Text =
            NomUsine +
            " — Tableau de bord";

        contenuPrincipal.Controls.Clear();

        MettreMenuActif(
            "Tableau de bord"
        );

        var page = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.Transparent
        };

        page.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                145
            )
        );

        page.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                46
            )
        );

        page.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                322
            )
        );

        page.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                48
            )
        );

        contenuPrincipal.Controls.Add(page);

        var entete = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Margin =
                new Padding(0, 0, 0, 12),
            Padding =
                new Padding(22, 14, 22, 12)
        };

        var enteteLayout =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        enteteLayout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                125
            )
        );

        enteteLayout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        enteteLayout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                220
            )
        );

        var texteEntete =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        texteEntete.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                45
            )
        );

        texteEntete.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                30
            )
        );

        texteEntete.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        var lblBienvenue = new Label
        {
            Text = NomUsine,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                20,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 125, 60),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var lblLocalisation = new Label
        {
            Text =
                LocaliteUsine +
                " — Tableau de bord de gestion",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(165, 45, 45),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var lblSousTitre = new Label
        {
            Text =
                "Suivi des ventes, recettes, dépenses, stocks, clients et crédits.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        texteEntete.Controls.Add(
            lblBienvenue,
            0,
            0
        );

        texteEntete.Controls.Add(
            lblLocalisation,
            0,
            1
        );

        texteEntete.Controls.Add(
            lblSousTitre,
            0,
            2
        );

        var badgeJour = new Label
        {
            Text =
                DateTime.Today.ToString(
                    "dddd dd MMMM yyyy"
                ),
            Dock = DockStyle.Fill,
            AutoSize = false,
            BackColor =
                Color.FromArgb(238, 244, 250),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            BorderStyle =
                BorderStyle.FixedSingle,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin =
                new Padding(15, 10, 0, 10)
        };

        var picLogoEntete = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode =
                PictureBoxSizeMode.Zoom,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Margin =
                new Padding(0, 4, 18, 4),
            Image = logoUsine
        };

        enteteLayout.Controls.Add(
            picLogoEntete,
            0,
            0
        );

        enteteLayout.Controls.Add(
            texteEntete,
            1,
            0
        );

        enteteLayout.Controls.Add(
            badgeJour,
            2,
            0
        );

        entete.Controls.Add(enteteLayout);

        page.Controls.Add(
            entete,
            0,
            0
        );

        var titreSection = new Label
        {
            Text = "INDICATEURS PRINCIPAUX",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding =
                new Padding(4, 0, 0, 0)
        };

        page.Controls.Add(
            titreSection,
            0,
            1
        );

        var grilleCartes =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                BackColor = Color.Transparent
            };

        for (int index = 0; index < 5; index++)
        {
            grilleCartes.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    20
                )
            );
        }

        grilleCartes.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                50
            )
        );

        grilleCartes.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                50
            )
        );

        page.Controls.Add(
            grilleCartes,
            0,
            2
        );

        var lblEtat = new Label
        {
            Text =
                "Chargement des données...",
            Dock = DockStyle.Fill,
            AutoSize = false,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Font = new Font(
                "Segoe UI",
                9
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding =
                new Padding(14, 0, 14, 0),
            Margin =
                new Padding(0, 12, 0, 0)
        };

        page.Controls.Add(
            lblEtat,
            0,
            3
        );

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            DateTime debutJour =
                DateTime.Today;

            DateTime finJour =
                debutJour.AddDays(1);

            DateTime debutMois =
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

            DateTime finMois =
                debutMois.AddMonths(1);

            decimal soldeCaisse =
                LireDecimal(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(
                            CASE
                                WHEN sens = 'ENTREE'
                                    THEN montant
                                WHEN sens = 'SORTIE'
                                    THEN -montant
                                ELSE 0
                            END
                        ),
                        0
                    )
                    FROM mouvements_caisse;
                    """
                );

            string sqlVentes =
                ColonneExiste(
                    connection,
                    "ventes",
                    "statut"
                )
                    ? """
                      SELECT COALESCE(
                          SUM(montant_total),
                          0
                      )
                      FROM ventes
                      WHERE date_vente >= @debut
                        AND date_vente < @fin
                        AND COALESCE(
                            statut,
                            'VALIDEE'
                        ) <> 'ANNULEE';
                      """
                    : """
                      SELECT COALESCE(
                          SUM(montant_total),
                          0
                      )
                      FROM ventes
                      WHERE date_vente >= @debut
                        AND date_vente < @fin;
                      """;

            decimal ventesJour =
                LireDecimalPeriode(
                    connection,
                    sqlVentes,
                    debutJour,
                    finJour
                );

            decimal creditsOuverts =
                LireDecimal(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(reste_a_payer),
                        0
                    )
                    FROM credits_clients
                    WHERE statut = 'OUVERT';
                    """
                );

            decimal recettesMois =
                LireDecimalPeriode(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(montant),
                        0
                    )
                    FROM recettes
                    WHERE statut = 'VALIDEE'
                      AND date_recette >= @debut
                      AND date_recette < @fin;
                    """,
                    debutMois,
                    finMois
                );

            decimal depensesMois =
                LireDecimalPeriode(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(montant),
                        0
                    )
                    FROM depenses
                    WHERE statut = 'VALIDEE'
                      AND date_depense >= @debut
                      AND date_depense < @fin;
                    """,
                    debutMois,
                    finMois
                );

            int employesActifs =
                LireEntier(
                    connection,
                    """
                    SELECT COUNT(*)
                    FROM employes
                    WHERE actif = TRUE;
                    """
                );

            int clientsActifs =
                LireEntier(
                    connection,
                    """
                    SELECT COUNT(*)
                    FROM clients
                    WHERE actif = TRUE;
                    """
                );

            int stockProduits =
                LireEntier(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(quantite_stock),
                        0
                    )
                    FROM produits
                    WHERE actif = TRUE;
                    """
                );

            int stockEmballages =
                LireEntier(
                    connection,
                    """
                    SELECT COALESCE(
                        SUM(quantite_stock),
                        0
                    )
                    FROM emballages
                    WHERE actif = TRUE;
                    """
                );

            int alertesStock =
                LireEntier(
                    connection,
                    """
                    SELECT
                        (
                            SELECT COUNT(*)
                            FROM produits
                            WHERE actif = TRUE
                              AND quantite_stock <=
                                  seuil_alerte
                        )
                        +
                        (
                            SELECT COUNT(*)
                            FROM emballages
                            WHERE actif = TRUE
                              AND quantite_stock <=
                                  seuil_alerte
                        );
                    """
                );

            Control[] cartes =
            {
                CreerCarte(
                    "Solde de la caisse",
                    $"{soldeCaisse:N2} DA",
                    "Caisse",
                    Color.FromArgb(32, 113, 171),
                    "Disponible"
                ),

                CreerCarte(
                    "Ventes du jour",
                    $"{ventesJour:N2} DA",
                    "Ventes",
                    Color.FromArgb(38, 138, 83),
                    "Aujourd’hui"
                ),

                CreerCarte(
                    "Crédits ouverts",
                    $"{creditsOuverts:N2} DA",
                    "Crédits clients",
                    Color.FromArgb(220, 145, 35),
                    "À encaisser"
                ),

                CreerCarte(
                    "Recettes du mois",
                    $"{recettesMois:N2} DA",
                    "Recettes",
                    Color.FromArgb(45, 150, 105),
                    "Ce mois"
                ),

                CreerCarte(
                    "Dépenses du mois",
                    $"{depensesMois:N2} DA",
                    "Dépenses",
                    Color.FromArgb(190, 58, 58),
                    "Ce mois"
                ),

                CreerCarte(
                    "Employés actifs",
                    employesActifs.ToString("N0"),
                    "Employés",
                    Color.FromArgb(93, 63, 137),
                    "Effectif"
                ),

                CreerCarte(
                    "Clients actifs",
                    clientsActifs.ToString("N0"),
                    "Clients",
                    Color.FromArgb(45, 120, 150),
                    "Enregistrés"
                ),

                CreerCarte(
                    "Stock produits",
                    stockProduits.ToString("N0"),
                    "Stock produits",
                    Color.FromArgb(74, 101, 133),
                    "Unités disponibles"
                ),

                CreerCarte(
                    "Stock emballages",
                    stockEmballages.ToString("N0"),
                    "Stock emballages",
                    Color.FromArgb(118, 92, 66),
                    "Unités disponibles"
                ),

                CreerCarte(
                    "Alertes de stock",
                    alertesStock.ToString("N0"),
                    "Stock produits",
                    alertesStock > 0
                        ? Color.FromArgb(205, 92, 42)
                        : Color.FromArgb(38, 138, 83),
                    alertesStock > 0
                        ? "À vérifier"
                        : "Stock normal"
                )
            };

            for (int index = 0; index < cartes.Length; index++)
            {
                grilleCartes.Controls.Add(
                    cartes[index],
                    index % 5,
                    index / 5
                );
            }

            lblEtat.Text =
                "Dernière actualisation : " +
                DateTime.Now.ToString(
                    "dd/MM/yyyy HH:mm:ss"
                );
        }
        catch (Exception ex)
        {
            grilleCartes.Controls.Clear();

            var erreur = new Label
            {
                Text =
                    "Impossible de charger les indicateurs du tableau de bord.\n\n" +
                    ex.Message,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle =
                    BorderStyle.FixedSingle,
                Font = new Font(
                    "Segoe UI",
                    11,
                    FontStyle.Bold
                ),
                ForeColor =
                    Color.FromArgb(190, 58, 58),
                TextAlign =
                    ContentAlignment.MiddleCenter,
                Margin = new Padding(6)
            };

            grilleCartes.Controls.Add(
                erreur,
                0,
                0
            );

            grilleCartes.SetColumnSpan(
                erreur,
                5
            );

            grilleCartes.SetRowSpan(
                erreur,
                2
            );

            lblEtat.Text =
                "Échec de l’actualisation.";
        }
    }


    private Panel CreerCarte(
        string titre,
        string valeur,
        string module,
        Color couleur,
        string sousTitre)
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(6),
            Padding = new Padding(15, 12, 15, 10),
            BorderStyle =
                BorderStyle.FixedSingle,
            Cursor = Cursors.Hand
        };

        var bande = new Panel
        {
            Dock = DockStyle.Left,
            Width = 6,
            BackColor = couleur,
            Cursor = Cursors.Hand
        };

        var contenu =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = new Padding(10, 0, 0, 0),
                Cursor = Cursors.Hand
            };

        contenu.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                34
            )
        );

        contenu.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        contenu.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                27
            )
        );

        var lblTitre = new Label
        {
            Text = titre,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(75, 85, 95),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand
        };

        var lblValeur = new Label
        {
            Text = valeur,
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign =
                ContentAlignment.MiddleLeft,
            Font = new Font(
                "Segoe UI",
                17,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            Cursor = Cursors.Hand,
            AutoEllipsis = true
        };

        var lblSousTitre = new Label
        {
            Text = sousTitre,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                8,
                FontStyle.Italic
            ),
            ForeColor =
                Color.FromArgb(115, 125, 135),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Cursor = Cursors.Hand
        };

        contenu.Controls.Add(
            lblTitre,
            0,
            0
        );

        contenu.Controls.Add(
            lblValeur,
            0,
            1
        );

        contenu.Controls.Add(
            lblSousTitre,
            0,
            2
        );

        carte.Controls.Add(contenu);
        carte.Controls.Add(bande);

        void ouvrir(
            object? sender,
            EventArgs e)
        {
            OuvrirModule(module);
        }

        carte.Click += ouvrir;
        bande.Click += ouvrir;
        contenu.Click += ouvrir;
        lblTitre.Click += ouvrir;
        lblValeur.Click += ouvrir;
        lblSousTitre.Click += ouvrir;

        carte.MouseEnter += (_, _) =>
        {
            carte.BackColor =
                Color.FromArgb(
                    248,
                    250,
                    252
                );
        };

        carte.MouseLeave += (_, _) =>
        {
            carte.BackColor =
                Color.White;
        };

        return carte;
    }


    private static Image? ChargerLogoUsine()
    {
        string dossierApplication =
            AppContext.BaseDirectory;

        var cheminsPossibles =
            new List<string>
            {
                Path.Combine(
                    dossierApplication,
                    "Images",
                    NomFichierLogo
                ),

                Path.Combine(
                    dossierApplication,
                    NomFichierLogo
                ),

                Path.Combine(
                    Environment.CurrentDirectory,
                    "Images",
                    NomFichierLogo
                ),

                Path.Combine(
                    Environment.CurrentDirectory,
                    NomFichierLogo
                )
            };

        DirectoryInfo? dossier =
            new DirectoryInfo(
                dossierApplication
            );

        for (
            int niveau = 0;
            niveau < 6 &&
            dossier != null;
            niveau++)
        {
            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
                    "Images",
                    NomFichierLogo
                )
            );

            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
                    NomFichierLogo
                )
            );

            dossier = dossier.Parent;
        }

        foreach (
            string chemin
            in cheminsPossibles)
        {
            if (!File.Exists(chemin))
            {
                continue;
            }

            try
            {
                byte[] donnees =
                    File.ReadAllBytes(
                        chemin
                    );

                using var flux =
                    new MemoryStream(
                        donnees
                    );

                using Image imageTemporaire =
                    Image.FromStream(
                        flux
                    );

                return new Bitmap(
                    imageTemporaire
                );
            }
            catch
            {
                // Essayer le chemin suivant.
            }
        }

        return null;
    }

    private static bool ColonneExiste(
        MySqlConnection connection,
        string table,
        string colonne)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE TABLE_SCHEMA = DATABASE()
              AND TABLE_NAME = @table
              AND COLUMN_NAME = @colonne;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        command.Parameters.AddWithValue(
            "@table",
            table
        );

        command.Parameters.AddWithValue(
            "@colonne",
            colonne
        );

        return Convert.ToInt32(
            command.ExecuteScalar()
        ) > 0;
    }

    private static decimal LireDecimal(
        MySqlConnection connection,
        string sql)
    {
        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        object? valeur =
            command.ExecuteScalar();

        return valeur == null ||
               valeur == DBNull.Value
            ? 0
            : Convert.ToDecimal(valeur);
    }

    private static decimal LireDecimalPeriode(
        MySqlConnection connection,
        string sql,
        DateTime debut,
        DateTime fin)
    {
        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        command.Parameters.AddWithValue(
            "@debut",
            debut
        );

        command.Parameters.AddWithValue(
            "@fin",
            fin
        );

        object? valeur =
            command.ExecuteScalar();

        return valeur == null ||
               valeur == DBNull.Value
            ? 0
            : Convert.ToDecimal(valeur);
    }

    private static int LireEntier(
        MySqlConnection connection,
        string sql)
    {
        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        object? valeur =
            command.ExecuteScalar();

        return valeur == null ||
               valeur == DBNull.Value
            ? 0
            : Convert.ToInt32(valeur);
    }

    private void AfficherModuleIndisponible(
        string module)
    {
        var message = new Label
        {
            Text =
                module +
                "\n\nCe module est indisponible.",

            Dock = DockStyle.Fill,

            TextAlign =
                ContentAlignment.MiddleCenter,

            Font = new Font(
                "Segoe UI",
                18,
                FontStyle.Bold
            ),

            ForeColor =
                Color.FromArgb(
                    80,
                    90,
                    100
                )
        };

        contenuPrincipal.Controls.Add(
            message
        );
    }

    private void AfficherErreurModule(
        string module,
        Exception ex)
    {
        var message = new Label
        {
            Text =
                "Impossible d’ouvrir le module : " +
                module +
                "\n\n" +
                ex.Message,

            Dock = DockStyle.Fill,

            TextAlign =
                ContentAlignment.MiddleCenter,

            Font = new Font(
                "Segoe UI",
                13,
                FontStyle.Bold
            ),

            ForeColor =
                Color.FromArgb(
                    190,
                    58,
                    58
                )
        };

        contenuPrincipal.Controls.Add(
            message
        );
    }

    protected override void Dispose(
        bool disposing)
    {
        if (disposing)
        {
            logoUsine?.Dispose();
        }

        base.Dispose(
            disposing
        );
    }

}
