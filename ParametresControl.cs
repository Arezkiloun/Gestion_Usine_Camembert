using MySqlConnector;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace GestionUsine;

public sealed class ParametresControl : UserControl
{
    private readonly Label lblEtatBase;
    private readonly Label lblDetailConnexion;
    private readonly Button btnTesterConnexion;

    public ParametresControl()
    {
        Dock = DockStyle.Fill;
        BackColor =
            Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode =
            AutoScaleMode.Dpi;

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                60
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                150
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                175
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        Controls.Add(structure);

        var titre = new Label
        {
            Text = "Paramètres",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                22,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(
            titre,
            0,
            0
        );

        structure.Controls.Add(
            CreerCarteApplication(),
            0,
            1
        );

        lblEtatBase = new Label
        {
            Text = "Vérification...",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold
            ),
            ForeColor = Color.DimGray,
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        lblDetailConnexion = new Label
        {
            Text =
                "État de la connexion à la base de données.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                9
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        btnTesterConnexion = new Button
        {
            Text = "Tester la connexion",
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
            Margin =
                new Padding(0, 8, 0, 0)
        };

        btnTesterConnexion
            .FlatAppearance
            .BorderSize = 0;

        btnTesterConnexion.Click +=
            (_, _) =>
            {
                TesterConnexion();
            };

        structure.Controls.Add(
            CreerCarteConnexion(),
            0,
            2
        );

        structure.Controls.Add(
            CreerCarteInformation(),
            0,
            3
        );

        TesterConnexion();
    }

    private Control CreerCarteApplication()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding =
                new Padding(20, 14, 20, 14),
            Margin =
                new Padding(0, 0, 0, 10)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        layout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                170
            )
        );

        layout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        var badge = new Label
        {
            Text = "GESTION\nUSINE",
            Dock = DockStyle.Fill,
            AutoSize = false,
            BackColor =
                Color.FromArgb(25, 49, 78),
            ForeColor = Color.White,
            Font = new Font(
                "Segoe UI",
                15,
                FontStyle.Bold
            ),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin =
                new Padding(0, 0, 18, 0)
        };

        var informations = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        informations.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                42
            )
        );

        informations.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                32
            )
        );

        informations.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        var lblApplication = new Label
        {
            Text =
                "Application : Gestion Usine",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                14,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var lblBase = new Label
        {
            Text =
                "Base de données : gestion_usine",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10
            ),
            ForeColor =
                Color.FromArgb(75, 85, 95),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var lblDescription = new Label
        {
            Text =
                "Application de gestion des ventes, stocks, clients, employés, recettes, dépenses et caisse.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                9
            ),
            ForeColor =
                Color.FromArgb(105, 115, 125),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        informations.Controls.Add(
            lblApplication,
            0,
            0
        );

        informations.Controls.Add(
            lblBase,
            0,
            1
        );

        informations.Controls.Add(
            lblDescription,
            0,
            2
        );

        layout.Controls.Add(
            badge,
            0,
            0
        );

        layout.Controls.Add(
            informations,
            1,
            0
        );

        carte.Controls.Add(layout);

        return carte;
    }

    private Control CreerCarteConnexion()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding =
                new Padding(20, 14, 20, 14),
            Margin =
                new Padding(0, 0, 0, 10)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        layout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        layout.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                230
            )
        );

        var zoneTexte =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        zoneTexte.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                30
            )
        );

        zoneTexte.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                45
            )
        );

        zoneTexte.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        var lblTitreConnexion = new Label
        {
            Text =
                "Connexion à la base de données",
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
                ContentAlignment.MiddleLeft
        };

        zoneTexte.Controls.Add(
            lblTitreConnexion,
            0,
            0
        );

        zoneTexte.Controls.Add(
            lblEtatBase,
            0,
            1
        );

        zoneTexte.Controls.Add(
            lblDetailConnexion,
            0,
            2
        );

        var zoneBouton =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Margin =
                    new Padding(18, 0, 0, 0),
                Padding = Padding.Empty
            };

        zoneBouton.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                50
            )
        );

        zoneBouton.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                48
            )
        );

        zoneBouton.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                50
            )
        );

        zoneBouton.Controls.Add(
            btnTesterConnexion,
            0,
            1
        );

        layout.Controls.Add(
            zoneTexte,
            0,
            0
        );

        layout.Controls.Add(
            zoneBouton,
            1,
            0
        );

        carte.Controls.Add(layout);

        return carte;
    }

    private static Control CreerCarteInformation()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Top,
            Height = 150,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding =
                new Padding(20, 16, 20, 16),
            Margin = Padding.Empty
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                32
            )
        );

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                34
            )
        );

        layout.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        var titre = new Label
        {
            Text =
                "Fonctionnement général",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var ligne1 = new Label
        {
            Text =
                "Toutes les données des modules sont centralisées dans la base gestion_usine.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10
            ),
            ForeColor =
                Color.FromArgb(70, 80, 90),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var ligne2 = new Label
        {
            Text =
                "Les ventes, recettes, dépenses, crédits et mouvements de caisse sont reliés automatiquement.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10
            ),
            ForeColor =
                Color.FromArgb(70, 80, 90),
            TextAlign =
                ContentAlignment.TopLeft
        };

        layout.Controls.Add(
            titre,
            0,
            0
        );

        layout.Controls.Add(
            ligne1,
            0,
            1
        );

        layout.Controls.Add(
            ligne2,
            0,
            2
        );

        carte.Controls.Add(layout);

        return carte;
    }

    private void TesterConnexion()
    {
        btnTesterConnexion.Enabled = false;
        btnTesterConnexion.Text =
            "Vérification...";

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            lblEtatBase.Text =
                "Base de données connectée";

            lblEtatBase.ForeColor =
                Color.FromArgb(38, 138, 83);

            lblDetailConnexion.Text =
                "La connexion MySQL fonctionne correctement.";
        }
        catch (Exception ex)
        {
            lblEtatBase.Text =
                "Connexion impossible";

            lblEtatBase.ForeColor =
                Color.FromArgb(190, 58, 58);

            lblDetailConnexion.Text =
                ex.Message;
        }
        finally
        {
            btnTesterConnexion.Enabled = true;
            btnTesterConnexion.Text =
                "Tester la connexion";
        }
    }
}
