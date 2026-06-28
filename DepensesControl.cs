using MySqlConnector;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;

namespace GestionUsine;

public sealed class DepensesControl : UserControl
{
    private const string ModePaiementParDefaut = "CASH";

    private ComboBox cmbCategorie = null!;
    private ComboBox cmbEmploye = null!;

    private Label lblCategorie = null!;
    private Label lblEmploye = null!;
    private Label lblMontant = null!;
    private Label lblDateDepense = null!;
    private Label lblMotif = null!;
    private Label lblSoldeCaisse = null!;
    private Label lblTotalPeriode = null!;

    private NumericUpDown numMontant = null!;
    private DateTimePicker dtpDateDepense = null!;
    private TextBox txtMotif = null!;

    private DateTimePicker dtpDateDebut = null!;
    private DateTimePicker dtpDateFin = null!;
    private TextBox txtRecherche = null!;

    private DataGridView dgvDepenses = null!;
    private TableLayoutPanel structurePrincipale = null!;
    private TableLayoutPanel formulaireSaisie = null!;

    private int depenseSelectionneeId;

    public DepensesControl()
    {
        Dock = DockStyle.Fill;
        BackColor = DrawingColor.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        Controls.Add(CreerInterface());

        ChargerEmployesActifs();
        ChargerHistorique();
        ActualiserSoldeCaisse();
        MettreAJourChampEmploye();
    }

    private Control CreerInterface()
    {
        structurePrincipale = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = DrawingColor.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structurePrincipale.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 60)
        );

        structurePrincipale.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 285)
        );

        structurePrincipale.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 115)
        );

        structurePrincipale.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var titre = new Label
        {
            Text = "Gestion des dépenses",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                22,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        structurePrincipale.Controls.Add(
            titre,
            0,
            0
        );

        structurePrincipale.Controls.Add(
            CreerCarteSaisie(),
            0,
            1
        );

        structurePrincipale.Controls.Add(
            CreerCarteFiltres(),
            0,
            2
        );

        dgvDepenses = CreerTableauDepenses();

        structurePrincipale.Controls.Add(
            dgvDepenses,
            0,
            3
        );

        return structurePrincipale;
    }


    private Control CreerCarteSaisie()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            Padding = new Padding(16),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 10)
        };

        formulaireSaisie = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 6,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        for (int i = 0; i < 4; i++)
        {
            formulaireSaisie.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25)
            );
        }

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 34)
        );

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48)
        );

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 28)
        );

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 32)
        );

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 44)
        );

        formulaireSaisie.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 56)
        );

        carte.Controls.Add(formulaireSaisie);

        lblCategorie =
            CreerLabel("Catégorie");

        lblEmploye =
            CreerLabel("Employé concerné");

        lblMontant =
            CreerLabel("Montant");

        lblDateDepense =
            CreerLabel("Date de la dépense");

        formulaireSaisie.Controls.Add(
            lblCategorie,
            0,
            0
        );

        formulaireSaisie.Controls.Add(
            lblEmploye,
            1,
            0
        );

        formulaireSaisie.Controls.Add(
            lblMontant,
            2,
            0
        );

        formulaireSaisie.Controls.Add(
            lblDateDepense,
            3,
            0
        );

        cmbCategorie = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 3, 5, 5)
        };

        cmbCategorie.Items.AddRange(
            new object[]
            {
                "Salaire",
                "Avance sur salaire",
                "Prime",
                "Achat de matières premières",
                "Achat d’emballages",
                "Transport et livraison",
                "Électricité",
                "Gaz",
                "Eau",
                "Maintenance et réparation",
                "Carburant",
                "Loyer",
                "Fournitures administratives",
                "Nettoyage et hygiène",
                "Impôts et taxes",
                "Publicité et marketing",

                "Alimentation des vaches",
                "Achat de fourrage",
                "Soins vétérinaires",
                "Médicaments et vaccins",
                "Insémination et reproduction",
                "Achat de vaches ou de veaux",
                "Entretien de l’étable",
                "Matériel de traite",
                "Transport du lait ou du bétail",
                "Eau et électricité de l’élevage",

                "Autre dépense"
            }
        );

        cmbCategorie.SelectedIndex = 0;

        cmbCategorie.SelectedIndexChanged +=
            (_, _) =>
            {
                MettreAJourChampEmploye();
            };

        cmbCategorie.SelectionChangeCommitted +=
            (_, _) =>
            {
                string choix =
                    Convert.ToString(
                        cmbCategorie.SelectedItem
                    ) ?? "";

                if (
                    choix.Equals(
                        "Autre dépense",
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    cmbCategorie.DropDownStyle =
                        ComboBoxStyle.DropDown;

                    lblCategorie.Text =
                        "Catégorie — écrivez la dépense *";

                    cmbCategorie.BackColor =
                        DrawingColor.FromArgb(
                            255,
                            249,
                            214
                        );

                    BeginInvoke(
                        new Action(() =>
                        {
                            cmbCategorie.Focus();
                            cmbCategorie.SelectAll();
                        })
                    );
                }
                else
                {
                    cmbCategorie.DropDownStyle =
                        ComboBoxStyle.DropDownList;

                    lblCategorie.Text =
                        "Catégorie";

                    cmbCategorie.BackColor =
                        DrawingColor.White;
                }

                MettreAJourChampEmploye();
            };

        cmbEmploye = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 3, 5, 5)
        };

        numMontant = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 1_000_000_000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            ),
            Margin = new Padding(5, 3, 5, 5)
        };

        dtpDateDepense = new DateTimePicker
        {
            Dock = DockStyle.Fill,
            Format =
                DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy HH:mm",
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 3, 5, 5),
            Value = DateTime.Now
        };

        formulaireSaisie.Controls.Add(
            cmbCategorie,
            0,
            1
        );

        formulaireSaisie.Controls.Add(
            cmbEmploye,
            1,
            1
        );

        formulaireSaisie.Controls.Add(
            numMontant,
            2,
            1
        );

        formulaireSaisie.Controls.Add(
            dtpDateDepense,
            3,
            1
        );

        lblSoldeCaisse = new Label
        {
            Text = "Solde caisse : 0,00 DA",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleRight,
            Padding = new Padding(0, 0, 5, 0),
            Margin = Padding.Empty
        };

        formulaireSaisie.Controls.Add(
            lblSoldeCaisse,
            2,
            2
        );

        formulaireSaisie.SetColumnSpan(
            lblSoldeCaisse,
            2
        );

        lblMotif =
            CreerLabel("Motif / observation");

        formulaireSaisie.Controls.Add(
            lblMotif,
            0,
            3
        );

        formulaireSaisie.SetColumnSpan(
            lblMotif,
            4
        );

        txtMotif = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 2, 5, 4),
            PlaceholderText =
                "Observation facultative"
        };

        formulaireSaisie.Controls.Add(
            txtMotif,
            0,
            4
        );

        formulaireSaisie.SetColumnSpan(
            txtMotif,
            4
        );

        var panneauBoutons =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = new Padding(0, 5, 0, 0)
            };

        for (int i = 0; i < 4; i++)
        {
            panneauBoutons.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    25
                )
            );
        }

        var btnEnregistrer = CreerBouton(
            "Enregistrer la dépense",
            DrawingColor.FromArgb(38, 138, 83)
        );

        var btnNouvelle = CreerBouton(
            "Nouvelle dépense",
            DrawingColor.FromArgb(95, 105, 115)
        );

        var btnAnnuler = CreerBouton(
            "Annuler la dépense",
            DrawingColor.FromArgb(190, 58, 58)
        );

        var btnActualiser = CreerBouton(
            "Actualiser",
            DrawingColor.FromArgb(32, 113, 171)
        );

        btnEnregistrer.Click += EnregistrerDepense;

        btnNouvelle.Click += (_, _) =>
        {
            ViderFormulaire();
        };

        btnAnnuler.Click +=
            AnnulerDepenseSelectionnee;

        btnActualiser.Click += (_, _) =>
        {
            ChargerEmployesActifs();
            ChargerHistorique();
            ActualiserSoldeCaisse();
        };

        panneauBoutons.Controls.Add(
            btnEnregistrer,
            0,
            0
        );

        panneauBoutons.Controls.Add(
            btnNouvelle,
            1,
            0
        );

        panneauBoutons.Controls.Add(
            btnAnnuler,
            2,
            0
        );

        panneauBoutons.Controls.Add(
            btnActualiser,
            3,
            0
        );

        formulaireSaisie.Controls.Add(
            panneauBoutons,
            0,
            5
        );

        formulaireSaisie.SetColumnSpan(
            panneauBoutons,
            4
        );

        return carte;
    }


    private Control CreerCarteFiltres()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            Padding = new Padding(12, 8, 12, 8),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 10)
        };

        var filtre = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 16)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 16)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 24)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 14)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 14)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 16)
        );

        filtre.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carte.Controls.Add(filtre);

        DateTime aujourdHui =
            DateTime.Today;

        dtpDateDebut = new DateTimePicker
        {
            Format =
                DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Font = new Font("Segoe UI", 10),
            Value = new DateTime(
                aujourdHui.Year,
                aujourdHui.Month,
                1
            )
        };

        dtpDateFin = new DateTimePicker
        {
            Format =
                DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Font = new Font("Segoe UI", 10),
            Value = aujourdHui
        };

        txtRecherche = new TextBox
        {
            Font = new Font("Segoe UI", 10),
            PlaceholderText =
                "Catégorie, employé ou motif"
        };

        txtRecherche.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChargerHistorique();
                e.SuppressKeyPress = true;
            }
        };

        var btnAfficher = CreerBouton(
            "Afficher période",
            DrawingColor.FromArgb(32, 113, 171)
        );

        var btnCeMois = CreerBouton(
            "Ce mois",
            DrawingColor.FromArgb(95, 105, 115)
        );

        var btnImprimer = CreerBouton(
            "Imprimer PDF",
            DrawingColor.FromArgb(93, 63, 137)
        );

        btnAfficher.Click += (_, _) =>
        {
            ChargerHistorique();
        };

        btnCeMois.Click += (_, _) =>
        {
            DateTime date = DateTime.Today;

            dtpDateDebut.Value =
                new DateTime(
                    date.Year,
                    date.Month,
                    1
                );

            dtpDateFin.Value = date;
            txtRecherche.Clear();
            ChargerHistorique();
        };

        btnImprimer.Click +=
            ImprimerHistoriquePdf;

        lblTotalPeriode = new Label
        {
            Text = "Total période : 0,00 DA",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin = Padding.Empty
        };

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Date début",
                dtpDateDebut
            ),
            0,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Date fin",
                dtpDateFin
            ),
            1,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Recherche",
                txtRecherche
            ),
            2,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Filtrer",
                btnAfficher
            ),
            3,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Période rapide",
                btnCeMois
            ),
            4,
            0
        );

        filtre.Controls.Add(
            CreerBlocTotalEtImpression(
                lblTotalPeriode,
                btnImprimer
            ),
            5,
            0
        );

        return carte;
    }


    private static Control CreerBlocFiltre(
        string titre,
        Control champ)
    {
        var bloc = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(5, 0, 5, 0),
            Padding = Padding.Empty
        };

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30)
        );

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42)
        );

        var label = new Label
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
                DrawingColor.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding = new Padding(2, 0, 0, 0),
            Margin = Padding.Empty
        };

        champ.Dock = DockStyle.Fill;
        champ.Margin = new Padding(0, 3, 0, 3);

        bloc.Controls.Add(label, 0, 0);
        bloc.Controls.Add(champ, 0, 1);

        return bloc;
    }


    private static Control CreerBlocTotalEtImpression(
        Label total,
        Button bouton)
    {
        var bloc = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(5, 0, 5, 0),
            Padding = Padding.Empty
        };

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30)
        );

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42)
        );

        total.Dock = DockStyle.Fill;
        total.Margin = Padding.Empty;

        bouton.Dock = DockStyle.Fill;
        bouton.Margin = new Padding(0, 3, 0, 3);

        bloc.Controls.Add(total, 0, 0);
        bloc.Controls.Add(bouton, 0, 1);

        return bloc;
    }


    private DataGridView CreerTableauDepenses()
    {
        var tableau = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = DrawingColor.White,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoGenerateColumns = false,
            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.None,
            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 10),
            ColumnHeadersHeight = 45,
            RowTemplate = { Height = 36 },
            Margin = Padding.Empty
        };

        tableau.ColumnHeadersDefaultCellStyle.Font =
            new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            );

        tableau.ColumnHeadersDefaultCellStyle.BackColor =
            DrawingColor.FromArgb(25, 49, 78);

        tableau.ColumnHeadersDefaultCellStyle.ForeColor =
            DrawingColor.White;

        tableau.EnableHeadersVisualStyles = false;

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                HeaderText = "N°",
                Width = 70
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "date_depense",
                DataPropertyName = "date_depense",
                HeaderText = "Date",
                Width = 155,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format = "dd/MM/yyyy HH:mm"
                    }
            }
        );

        tableau.Columns.Add(
            CreerColonne(
                "categorie",
                "categorie",
                "Catégorie",
                200
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "employe",
                "employe",
                "Employé",
                190
            )
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "montant",
                DataPropertyName = "montant",
                HeaderText = "Montant",
                Width = 155,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format = "#,##0.00 'DA'",
                        Alignment =
                            DataGridViewContentAlignment.MiddleRight
                    }
            }
        );

        tableau.Columns.Add(
            CreerColonne(
                "motif",
                "motif",
                "Motif",
                260,
                true
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "statut",
                "statut",
                "Statut",
                110
            )
        );

        tableau.CellClick += SelectionnerDepense;
        tableau.CellFormatting += ColorerDepense;

        return tableau;
    }

    private void ChargerEmployesActifs()
    {
        try
        {
            int? ancienEmployeId =
                ObtenirEmployeSelectionneId();

            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    id,
                    CONCAT(
                        nom,
                        ' ',
                        prenom,
                        ' — ',
                        poste
                    ) AS employe
                FROM employes
                WHERE actif = TRUE
                ORDER BY nom, prenom;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            cmbEmploye.DisplayMember = "employe";
            cmbEmploye.ValueMember = "id";
            cmbEmploye.DataSource = table;

            if (ancienEmployeId.HasValue)
            {
                cmbEmploye.SelectedValue =
                    ancienEmployeId.Value;
            }

            if (cmbEmploye.Items.Count > 0 &&
                cmbEmploye.SelectedIndex < 0)
            {
                cmbEmploye.SelectedIndex = 0;
            }
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger les employés.",
                ex
            );
        }
    }

    private void MettreAJourChampEmploye()
    {
        if (
            cmbCategorie == null ||
            formulaireSaisie == null ||
            structurePrincipale == null)
        {
            return;
        }

        string categorie =
            cmbCategorie.Text.Trim();

        bool employeObligatoire =
            EstCategorieEmploye(categorie);

        bool estSalaire =
            categorie.Equals(
                "Salaire",
                StringComparison.OrdinalIgnoreCase
            );

        bool saisiePersonnalisee =
            cmbCategorie.DropDownStyle ==
                ComboBoxStyle.DropDown &&
            cmbCategorie.SelectedIndex < 0;

        lblEmploye.Visible =
            employeObligatoire;

        cmbEmploye.Visible =
            employeObligatoire;

        cmbEmploye.Enabled =
            employeObligatoire;

        if (!employeObligatoire)
        {
            cmbEmploye.SelectedIndex = -1;

            formulaireSaisie.SetColumn(
                lblMontant,
                1
            );

            formulaireSaisie.SetColumn(
                numMontant,
                1
            );

            formulaireSaisie.SetColumn(
                lblDateDepense,
                2
            );

            formulaireSaisie.SetColumn(
                dtpDateDepense,
                2
            );

            formulaireSaisie.SetColumnSpan(
                lblDateDepense,
                2
            );

            formulaireSaisie.SetColumnSpan(
                dtpDateDepense,
                2
            );
        }
        else
        {
            if (
                cmbEmploye.Items.Count > 0 &&
                cmbEmploye.SelectedIndex < 0)
            {
                cmbEmploye.SelectedIndex = 0;
            }

            formulaireSaisie.SetColumn(
                lblMontant,
                2
            );

            formulaireSaisie.SetColumn(
                numMontant,
                2
            );

            formulaireSaisie.SetColumn(
                lblDateDepense,
                3
            );

            formulaireSaisie.SetColumn(
                dtpDateDepense,
                3
            );

            formulaireSaisie.SetColumnSpan(
                lblDateDepense,
                1
            );

            formulaireSaisie.SetColumnSpan(
                dtpDateDepense,
                1
            );
        }

        // Le motif disparaît uniquement pour Salaire.
        // Pour une dépense personnalisée, le nom est écrit
        // directement dans le champ Catégorie.
        lblMotif.Visible =
            !estSalaire;

        txtMotif.Visible =
            !estSalaire;

        txtMotif.Enabled =
            !estSalaire;

        if (estSalaire)
        {
            lblMotif.Text =
                "Motif / observation";

            txtMotif.PlaceholderText = "";
            txtMotif.Clear();
        }
        else
        {
            lblMotif.Text =
                "Motif / observation (facultatif)";

            txtMotif.PlaceholderText =
                "Observation facultative";
        }

        if (saisiePersonnalisee)
        {
            lblCategorie.Text =
                "Catégorie — écrivez la dépense *";

            cmbCategorie.BackColor =
                DrawingColor.FromArgb(
                    255,
                    249,
                    214
                );
        }

        formulaireSaisie.SuspendLayout();

        formulaireSaisie.RowStyles[3].Height =
            estSalaire ? 0 : 32;

        formulaireSaisie.RowStyles[4].Height =
            estSalaire ? 0 : 44;

        structurePrincipale.RowStyles[1].Height =
            estSalaire ? 210 : 285;

        formulaireSaisie.ResumeLayout(true);
        formulaireSaisie.PerformLayout();
        formulaireSaisie.Invalidate();
    }


    private static bool EstCategorieEmploye(
        string categorie)
    {
        return categorie == "Salaire" ||
               categorie == "Avance sur salaire" ||
               categorie == "Prime";
    }

    private int? ObtenirEmployeSelectionneId()
    {
        if (
            cmbEmploye == null ||
            cmbEmploye.SelectedValue == null ||
            cmbEmploye.SelectedValue is DataRowView)
        {
            return null;
        }

        return Convert.ToInt32(
            cmbEmploye.SelectedValue
        );
    }

    private void EnregistrerDepense(
        object? sender,
        EventArgs e)
    {
        string categorie =
            cmbCategorie.Text.Trim();

        if (string.IsNullOrWhiteSpace(categorie))
        {
            AfficherAvertissement(
                "Veuillez sélectionner une catégorie."
            );

            cmbCategorie.Focus();
            return;
        }

        bool categorieEmploye =
            EstCategorieEmploye(categorie);

        int? employeId =
            categorieEmploye
                ? ObtenirEmployeSelectionneId()
                : null;

        if (
            categorieEmploye &&
            !employeId.HasValue)
        {
            AfficherAvertissement(
                "Veuillez sélectionner l’employé concerné."
            );

            cmbEmploye.Focus();
            return;
        }

        decimal montant =
            numMontant.Value;

        if (montant <= 0)
        {
            AfficherAvertissement(
                "Le montant doit être supérieur à zéro."
            );

            numMontant.Focus();
            return;
        }

        if (
            dtpDateDepense.Value >
            DateTime.Now.AddMinutes(5))
        {
            AfficherAvertissement(
                "La date de la dépense ne peut pas être future."
            );

            dtpDateDepense.Focus();
            return;
        }

        bool categorieNonRemplacee =
            categorie.Equals(
                "Autre dépense",
                StringComparison.OrdinalIgnoreCase
            );

        if (
            string.IsNullOrWhiteSpace(categorie) ||
            categorieNonRemplacee)
        {
            AfficherAvertissement(
                "Écrivez directement la nature de la dépense " +
                "dans le champ Catégorie."
            );

            cmbCategorie.DropDownStyle =
                ComboBoxStyle.DropDown;

            lblCategorie.Text =
                "Catégorie — écrivez la dépense *";

            cmbCategorie.BackColor =
                DrawingColor.FromArgb(
                    255,
                    249,
                    214
                );

            cmbCategorie.Focus();
            cmbCategorie.SelectAll();
            return;
        }

        // Motif automatique pour Salaire.
        // Motif facultatif pour toutes les autres dépenses.
        string motif =
            categorie.Equals(
                "Salaire",
                StringComparison.OrdinalIgnoreCase
            )
                ? "Paiement du salaire"
                : txtMotif.Text.Trim();

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                decimal soldeCaisse =
                    LireSoldeCaisse(
                        connection,
                        transaction
                    );

                if (montant > soldeCaisse)
                {
                    throw new InvalidOperationException(
                        "Le solde de la caisse est insuffisant.\n\n" +
                        $"Solde disponible : {soldeCaisse:N2} DA\n" +
                        $"Dépense demandée : {montant:N2} DA"
                    );
                }

                const string sqlDepense = """
                    INSERT INTO depenses
                    (
                        categorie,
                        employe_id,
                        montant,
                        motif,
                        mode_paiement,
                        date_depense,
                        statut
                    )
                    VALUES
                    (
                        @categorie,
                        @employeId,
                        @montant,
                        @motif,
                        @modePaiement,
                        @dateDepense,
                        'VALIDEE'
                    );
                    """;

                using MySqlCommand depenseCommand =
                    new MySqlCommand(
                        sqlDepense,
                        connection,
                        transaction
                    );

                depenseCommand.Parameters.AddWithValue(
                    "@categorie",
                    categorie
                );

                depenseCommand.Parameters.AddWithValue(
                    "@employeId",
                    employeId.HasValue
                        ? employeId.Value
                        : DBNull.Value
                );

                depenseCommand.Parameters.AddWithValue(
                    "@montant",
                    montant
                );

                depenseCommand.Parameters.AddWithValue(
                    "@motif",
                    motif
                );

                depenseCommand.Parameters.AddWithValue(
                    "@modePaiement",
                    ModePaiementParDefaut
                );

                depenseCommand.Parameters.AddWithValue(
                    "@dateDepense",
                    dtpDateDepense.Value
                );

                depenseCommand.ExecuteNonQuery();

                int depenseId =
                    Convert.ToInt32(
                        depenseCommand.LastInsertedId
                    );

                string employe =
                    categorieEmploye
                        ? cmbEmploye.Text
                        : "";

                string motifCaisse =
                    ConstruireMotifCaisse(
                        categorie,
                        employe,
                        motif,
                        depenseId
                    );

                const string sqlCaisse = """
                    INSERT INTO mouvements_caisse
                    (
                        sens,
                        type_mouvement,
                        montant,
                        motif,
                        reference_type,
                        reference_id,
                        date_mouvement
                    )
                    VALUES
                    (
                        'SORTIE',
                        'DEPENSE',
                        @montant,
                        @motif,
                        'DEPENSE',
                        @depenseId,
                        @dateDepense
                    );
                    """;

                using MySqlCommand caisseCommand =
                    new MySqlCommand(
                        sqlCaisse,
                        connection,
                        transaction
                    );

                caisseCommand.Parameters.AddWithValue(
                    "@montant",
                    montant
                );

                caisseCommand.Parameters.AddWithValue(
                    "@motif",
                    motifCaisse
                );

                caisseCommand.Parameters.AddWithValue(
                    "@depenseId",
                    depenseId
                );

                caisseCommand.Parameters.AddWithValue(
                    "@dateDepense",
                    dtpDateDepense.Value
                );

                caisseCommand.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show(
                    "La dépense a été enregistrée.\n\n" +
                    $"N° : {depenseId}\n" +
                    $"Catégorie : {categorie}\n" +
                    $"Montant : {montant:N2} DA\n" +
                    "Mode de paiement : Cash",
                    "Dépense enregistrée",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ViderFormulaire();
                ChargerHistorique();
                ActualiserSoldeCaisse();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible d’enregistrer la dépense.",
                ex
            );
        }
    }

    private static decimal LireSoldeCaisse(
        MySqlConnection connection,
        MySqlTransaction transaction)
    {
        const string sql = """
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
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        return Convert.ToDecimal(
            command.ExecuteScalar()
        );
    }

    private static string ConstruireMotifCaisse(
        string categorie,
        string employe,
        string motif,
        int depenseId)
    {
        string texte =
            $"Dépense n° {depenseId} - {categorie}";

        if (!string.IsNullOrWhiteSpace(employe))
        {
            texte += $" - {employe}";
        }

        if (!string.IsNullOrWhiteSpace(motif))
        {
            texte += $" - {motif}";
        }

        if (texte.Length > 255)
        {
            texte = texte[..255];
        }

        return texte;
    }

    private void AnnulerDepenseSelectionnee(
        object? sender,
        EventArgs e)
    {
        if (depenseSelectionneeId == 0)
        {
            AfficherAvertissement(
                "Sélectionnez une dépense dans le tableau."
            );

            return;
        }

        DataGridViewRow? ligne =
            dgvDepenses.CurrentRow;

        string statut =
            Convert.ToString(
                ligne?.Cells["statut"].Value
            ) ?? "";

        if (
            statut.StartsWith(
                "Annul",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            MessageBox.Show(
                "Cette dépense est déjà annulée.",
                "Annulation impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        decimal montant =
            Convert.ToDecimal(
                ligne?.Cells["montant"].Value ?? 0
            );

        string categorie =
            Convert.ToString(
                ligne?.Cells["categorie"].Value
            ) ?? "";

        DialogResult confirmation =
            MessageBox.Show(
                "Voulez-vous vraiment annuler cette dépense ?\n\n" +
                $"N° : {depenseSelectionneeId}\n" +
                $"Catégorie : {categorie}\n" +
                $"Montant : {montant:N2} DA\n\n" +
                "Le montant sera réintégré dans la caisse.",
                "Confirmation d’annulation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2
            );

        if (confirmation != DialogResult.Yes)
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                const string sqlLire = """
                    SELECT
                        montant,
                        categorie,
                        statut
                    FROM depenses
                    WHERE id = @id
                    FOR UPDATE;
                    """;

                decimal montantBase;
                string categorieBase;
                string statutBase;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlLire,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@id",
                        depenseSelectionneeId
                    );

                    using MySqlDataReader reader =
                        command.ExecuteReader();

                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "La dépense sélectionnée n’existe plus."
                        );
                    }

                    montantBase =
                        reader.GetDecimal("montant");

                    categorieBase =
                        reader.GetString("categorie");

                    statutBase =
                        reader.GetString("statut");
                }

                if (
                    statutBase.StartsWith(
                        "ANNU",
                        StringComparison.OrdinalIgnoreCase
                    ))
                {
                    throw new InvalidOperationException(
                        "Cette dépense est déjà annulée."
                    );
                }

                const string sqlUpdate = """
                    UPDATE depenses
                    SET
                        statut = 'ANNULEE',
                        date_annulation = CURRENT_TIMESTAMP
                    WHERE id = @id;
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlUpdate,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@id",
                        depenseSelectionneeId
                    );

                    command.ExecuteNonQuery();
                }

                const string sqlCaisse = """
                    INSERT INTO mouvements_caisse
                    (
                        sens,
                        type_mouvement,
                        montant,
                        motif,
                        reference_type,
                        reference_id
                    )
                    VALUES
                    (
                        'ENTREE',
                        'ANNULATION_DEPENSE',
                        @montant,
                        @motif,
                        'DEPENSE',
                        @id
                    );
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlCaisse,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@montant",
                        montantBase
                    );

                    command.Parameters.AddWithValue(
                        "@motif",
                        $"Annulation dépense n° " +
                        $"{depenseSelectionneeId} - " +
                        categorieBase
                    );

                    command.Parameters.AddWithValue(
                        "@id",
                        depenseSelectionneeId
                    );

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                MessageBox.Show(
                    "La dépense a été annulée.\n\n" +
                    "Le montant a été réintégré dans la caisse.",
                    "Dépense annulée",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                depenseSelectionneeId = 0;
                ChargerHistorique();
                ActualiserSoldeCaisse();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible d’annuler la dépense.",
                ex
            );
        }
    }

    private void ChargerHistorique()
    {
        if (dgvDepenses == null)
        {
            return;
        }

        if (
            dtpDateDebut.Value.Date >
            dtpDateFin.Value.Date)
        {
            AfficherAvertissement(
                "La date début doit être antérieure ou égale à la date fin."
            );

            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            string sql = """
                SELECT
                    d.id,
                    d.date_depense,
                    d.categorie,

                    COALESCE(
                        CONCAT(
                            e.nom,
                            ' ',
                            e.prenom
                        ),
                        '—'
                    ) AS employe,

                    d.montant,
                    d.motif,

                    CASE d.statut
                        WHEN 'VALIDEE'
                            THEN 'Validée'
                        WHEN 'ANNULEE'
                            THEN 'Annulée'
                        ELSE d.statut
                    END AS statut

                FROM depenses d

                LEFT JOIN employes e
                    ON e.id = d.employe_id

                WHERE d.date_depense >= @dateDebut
                  AND d.date_depense < @dateFinExclusive
                """;

            string recherche =
                txtRecherche?.Text.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                sql += """

                    AND
                    (
                        d.categorie LIKE @recherche
                        OR d.motif LIKE @recherche
                        OR e.nom LIKE @recherche
                        OR e.prenom LIKE @recherche
                        OR CAST(d.id AS CHAR) LIKE @recherche
                    )
                    """;
            }

            sql += """

                ORDER BY
                    d.date_depense DESC,
                    d.id DESC;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@dateDebut",
                dtpDateDebut.Value.Date
            );

            command.Parameters.AddWithValue(
                "@dateFinExclusive",
                dtpDateFin.Value.Date.AddDays(1)
            );

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                command.Parameters.AddWithValue(
                    "@recherche",
                    $"%{recherche}%"
                );
            }

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(command);

            var table = new DataTable();
            adapter.Fill(table);

            dgvDepenses.DataSource = table;
            dgvDepenses.ClearSelection();
            depenseSelectionneeId = 0;

            ActualiserTotalPeriode();
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger l’historique des dépenses.",
                ex
            );
        }
    }

    private void ActualiserTotalPeriode()
    {
        decimal total = 0;

        foreach (
            DataGridViewRow ligne
            in dgvDepenses.Rows)
        {
            string statut =
                Convert.ToString(
                    ligne.Cells["statut"].Value
                ) ?? "";

            if (
                statut.StartsWith(
                    "Annul",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                continue;
            }

            total += Convert.ToDecimal(
                ligne.Cells["montant"].Value
            );
        }

        lblTotalPeriode.Text =
            $"Total période : {total:N2} DA";
    }

    private void ActualiserSoldeCaisse()
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            decimal solde =
                LireSoldeCaisse(
                    connection,
                    transaction
                );

            transaction.Commit();

            lblSoldeCaisse.Text =
                $"Solde caisse : {solde:N2} DA";

            lblSoldeCaisse.ForeColor =
                solde < 0
                    ? DrawingColor.FromArgb(190, 58, 58)
                    : DrawingColor.FromArgb(25, 49, 78);
        }
        catch (Exception ex)
        {
            lblSoldeCaisse.Text =
                "Solde caisse indisponible";

            lblSoldeCaisse.ForeColor =
                DrawingColor.FromArgb(190, 58, 58);

            System.Diagnostics.Debug.WriteLine(
                ex.Message
            );
        }
    }

    private void SelectionnerDepense(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        depenseSelectionneeId =
            Convert.ToInt32(
                dgvDepenses.Rows[e.RowIndex]
                    .Cells["id"]
                    .Value
            );
    }

    private static void ColorerDepense(
        object? sender,
        DataGridViewCellFormattingEventArgs e)
    {
        if (
            sender is not DataGridView tableau ||
            e.RowIndex < 0)
        {
            return;
        }

        string statut =
            Convert.ToString(
                tableau.Rows[e.RowIndex]
                    .Cells["statut"]
                    .Value
            ) ?? "";

        if (
            statut.StartsWith(
                "Annul",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            tableau.Rows[e.RowIndex]
                .DefaultCellStyle.BackColor =
                DrawingColor.FromArgb(235, 235, 235);

            tableau.Rows[e.RowIndex]
                .DefaultCellStyle.ForeColor =
                DrawingColor.FromArgb(115, 115, 115);
        }
    }

    private sealed class LigneDepensePdf
    {
        public int Id { get; init; }
        public DateTime DateDepense { get; init; }
        public string Categorie { get; init; } = "";
        public string Employe { get; init; } = "";
        public decimal Montant { get; init; }
        public string Motif { get; init; } = "";
        public string Statut { get; init; } = "";
    }

    private void ImprimerHistoriquePdf(
        object? sender,
        EventArgs e)
    {
        if (dgvDepenses.Rows.Count == 0)
        {
            MessageBox.Show(
                "Aucune dépense n’est affichée pour cette période.",
                "PDF impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        try
        {
            List<LigneDepensePdf> lignes =
                ObtenirLignesPdf();

            using var dialogue =
                new SaveFileDialog
                {
                    Title =
                        "Enregistrer l’historique des dépenses",

                    Filter =
                        "Fichier PDF (*.pdf)|*.pdf",

                    FileName =
                        $"Depenses_" +
                        $"{dtpDateDebut.Value:yyyyMMdd}_" +
                        $"{dtpDateFin.Value:yyyyMMdd}.pdf",

                    AddExtension = true,
                    DefaultExt = "pdf",
                    OverwritePrompt = true
                };

            if (
                dialogue.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }

            GenererPdfDepenses(
                lignes,
                dialogue.FileName
            );

            DialogResult ouvrir =
                MessageBox.Show(
                    "Le PDF a été créé correctement.\n\n" +
                    dialogue.FileName +
                    "\n\nVoulez-vous l’ouvrir maintenant ?",
                    "PDF créé",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

            if (ouvrir == DialogResult.Yes)
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            dialogue.FileName,

                        UseShellExecute = true
                    }
                );
            }
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de créer le PDF.",
                ex
            );
        }
    }

    private List<LigneDepensePdf> ObtenirLignesPdf()
    {
        var lignes =
            new List<LigneDepensePdf>();

        foreach (
            DataGridViewRow ligne
            in dgvDepenses.Rows)
        {
            if (ligne.IsNewRow)
            {
                continue;
            }

            lignes.Add(
                new LigneDepensePdf
                {
                    Id = Convert.ToInt32(
                        ligne.Cells["id"].Value
                    ),

                    DateDepense =
                        Convert.ToDateTime(
                            ligne.Cells["date_depense"].Value
                        ),

                    Categorie =
                        Convert.ToString(
                            ligne.Cells["categorie"].Value
                        ) ?? "",

                    Employe =
                        Convert.ToString(
                            ligne.Cells["employe"].Value
                        ) ?? "",

                    Montant =
                        Convert.ToDecimal(
                            ligne.Cells["montant"].Value
                        ),

                    Motif =
                        Convert.ToString(
                            ligne.Cells["motif"].Value
                        ) ?? "",

                    Statut =
                        Convert.ToString(
                            ligne.Cells["statut"].Value
                        ) ?? ""
                }
            );
        }

        return lignes;
    }

    private void GenererPdfDepenses(
        List<LigneDepensePdf> lignes,
        string chemin)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        decimal totalValide = 0;

        foreach (
            LigneDepensePdf ligne
            in lignes)
        {
            if (
                !ligne.Statut.StartsWith(
                    "Annul",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                totalValide += ligne.Montant;
            }
        }

        string recherche =
            txtRecherche.Text.Trim();

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(
                    PageSizes.A4.Landscape()
                );

                page.Margin(24);

                page.DefaultTextStyle(
                    style =>
                        style.FontFamily("Arial")
                             .FontSize(8.5F)
                );

                page.Header()
                    .Column(entete =>
                    {
                        entete.Item()
                            .Text(
                                "HISTORIQUE DES DÉPENSES"
                            )
                            .Bold()
                            .FontSize(19)
                            .FontColor(
                                Colors.Blue.Darken3
                            );

                        entete.Item()
                            .PaddingTop(4)
                            .Text(texte =>
                            {
                                texte.Span("Période : ");

                                texte.Span(
                                    $"{dtpDateDebut.Value:dd/MM/yyyy}" +
                                    " au " +
                                    $"{dtpDateFin.Value:dd/MM/yyyy}"
                                );

                                texte.Span(
                                    "   |   Imprimé le : "
                                );

                                texte.Span(
                                    DateTime.Now.ToString(
                                        "dd/MM/yyyy HH:mm"
                                    )
                                );

                                if (
                                    !string.IsNullOrWhiteSpace(
                                        recherche
                                    ))
                                {
                                    texte.Span(
                                        "   |   Recherche : "
                                    );

                                    texte.Span(recherche);
                                }
                            });
                    });

                page.Content()
                    .PaddingVertical(12)
                    .Column(contenu =>
                    {
                        contenu.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(
                                    columns =>
                                    {
                                        columns.ConstantColumn(40);
                                        columns.RelativeColumn(1.2F);
                                        columns.RelativeColumn(1.8F);
                                        columns.RelativeColumn(1.6F);
                                        columns.RelativeColumn(1.2F);
                                        columns.RelativeColumn(2.6F);
                                        columns.RelativeColumn(1.0F);
                                    }
                                );

                                table.Header(header =>
                                {
                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("N°");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Date");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Catégorie");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Employé");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Montant");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Motif");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Statut");
                                });

                                foreach (
                                    LigneDepensePdf ligne
                                    in lignes)
                                {
                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .AlignCenter()
                                        .Text(
                                            ligne.Id.ToString()
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .AlignCenter()
                                        .Text(
                                            ligne.DateDepense
                                                .ToString(
                                                    "dd/MM/yyyy HH:mm"
                                                )
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .Text(
                                            ValeurPdf(
                                                ligne.Categorie
                                            )
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .Text(
                                            ValeurPdf(
                                                ligne.Employe
                                            )
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .AlignRight()
                                        .Text(
                                            $"{ligne.Montant:N2} DA"
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .Text(
                                            ValeurPdf(
                                                ligne.Motif
                                            )
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .AlignCenter()
                                        .Text(
                                            ValeurPdf(
                                                ligne.Statut
                                            )
                                        );
                                }
                            });

                        contenu.Item()
                            .PaddingTop(12)
                            .Row(resume =>
                            {
                                resume.RelativeItem()
                                    .Text(
                                        "Nombre de dépenses : " +
                                        lignes.Count
                                    )
                                    .Bold();

                                resume.RelativeItem()
                                    .AlignRight()
                                    .Text(
                                        "Total des dépenses validées : " +
                                        $"{totalValide:N2} DA"
                                    )
                                    .Bold();
                            });
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(texte =>
                    {
                        texte.Span("Page ");
                        texte.CurrentPageNumber();
                        texte.Span(" / ");
                        texte.TotalPages();
                    });
            });
        })
        .GeneratePdf(chemin);
    }

    private static IContainer StyleEntetePdf(
        IContainer container)
    {
        return container
            .Background(Colors.Blue.Darken3)
            .Border(1)
            .BorderColor(Colors.White)
            .Padding(6)
            .AlignCenter()
            .AlignMiddle()
            .DefaultTextStyle(
                style =>
                    style.Bold()
                         .FontColor(Colors.White)
            );
    }

    private static IContainer StyleCellulePdf(
        IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey.Lighten2
            )
            .Padding(5)
            .AlignMiddle();
    }

    private static string ValeurPdf(
        string valeur)
    {
        return string.IsNullOrWhiteSpace(valeur)
            ? "—"
            : valeur.Trim();
    }

    private void ViderFormulaire()
    {
        depenseSelectionneeId = 0;

        cmbCategorie.DropDownStyle =
            ComboBoxStyle.DropDownList;

        lblCategorie.Text =
            "Catégorie";

        cmbCategorie.BackColor =
            DrawingColor.White;

        if (cmbCategorie.Items.Count > 0)
        {
            cmbCategorie.SelectedIndex = 0;
        }

        numMontant.Value = 0;
        dtpDateDepense.Value = DateTime.Now;
        txtMotif.Clear();

        MettreAJourChampEmploye();

        dgvDepenses.ClearSelection();
        cmbCategorie.Focus();
    }

    private static Label CreerLabel(
        string texte)
    {
        return new Label
        {
            Text = texte,
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.BottomLeft,
            Margin = new Padding(5)
        };
    }

    private static Button CreerBouton(
        string texte,
        DrawingColor couleur)
    {
        var bouton = new Button
        {
            Text = texte,
            Dock = DockStyle.Fill,
            BackColor = couleur,
            ForeColor = DrawingColor.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold
            ),
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };

        bouton.FlatAppearance.BorderSize = 0;
        return bouton;
    }

    private static DataGridViewTextBoxColumn
        CreerColonne(
            string nom,
            string propriete,
            string titre,
            int largeur,
            bool remplir = false)
    {
        return new DataGridViewTextBoxColumn
        {
            Name = nom,
            DataPropertyName = propriete,
            HeaderText = titre,
            Width = largeur,
            AutoSizeMode =
                remplir
                    ? DataGridViewAutoSizeColumnMode.Fill
                    : DataGridViewAutoSizeColumnMode.None
        };
    }

    private static void AfficherAvertissement(
        string message)
    {
        MessageBox.Show(
            message,
            "Vérification",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        );
    }

    private static void AfficherErreur(
        string message,
        Exception ex)
    {
        MessageBox.Show(
            message + "\n\n" + ex.Message,
            "Erreur",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error
        );
    }
}
