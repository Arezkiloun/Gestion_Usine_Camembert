using MySqlConnector;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;

namespace GestionUsine;

public sealed class RecettesControl : UserControl
{
    private const string ModePaiementParDefaut = "CASH";

    private ComboBox cmbCategorie = null!;
    private NumericUpDown numMontant = null!;
    private DateTimePicker dtpDateRecette = null!;
    private TextBox txtMotif = null!;

    private Label lblCategorie = null!;
    private Label lblSoldeCaisse = null!;
    private Label lblTotalPeriode = null!;

    private DateTimePicker dtpDateDebut = null!;
    private DateTimePicker dtpDateFin = null!;
    private TextBox txtRecherche = null!;

    private DataGridView dgvRecettes = null!;

    private int recetteSelectionneeId;

    public RecettesControl()
    {
        Dock = DockStyle.Fill;
        BackColor =
            DrawingColor.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        Controls.Add(CreerInterface());

        ChargerHistorique();
        ActualiserSoldeCaisse();
    }

    private Control CreerInterface()
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = DrawingColor.Transparent
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 60)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 270)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 115)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var titre = new Label
        {
            Text = "Gestion des recettes",
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

        structure.Controls.Add(titre, 0, 0);
        structure.Controls.Add(
            CreerCarteSaisie(),
            0,
            1
        );

        structure.Controls.Add(
            CreerCarteFiltres(),
            0,
            2
        );

        dgvRecettes = CreerTableauRecettes();

        structure.Controls.Add(
            dgvRecettes,
            0,
            3
        );

        return structure;
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

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 32)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 20)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 26)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 22)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 36)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 50)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 84)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 58)
        );

        carte.Controls.Add(formulaire);

        lblCategorie =
            CreerLabel("Catégorie");

        formulaire.Controls.Add(
            lblCategorie,
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Montant"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Date de la recette"),
            2,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Situation de la caisse"),
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
                "Vente de lait",
                "Vente de vaches",
                "Vente de veaux",
                "Vente de fumier",
                "Vente de matériel",
                "Subvention agricole",
                "Indemnisation",
                "Remboursement reçu",
                "Apport du propriétaire",
                "Autre recette"
            }
        );

        cmbCategorie.SelectedIndex = 0;

        cmbCategorie.SelectionChangeCommitted +=
            (_, _) =>
            {
                GererChoixCategorie();
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

        dtpDateRecette = new DateTimePicker
        {
            Dock = DockStyle.Fill,
            Format =
                DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy HH:mm",
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 3, 5, 5),
            Value = DateTime.Now
        };

        lblSoldeCaisse = new Label
        {
            Text = "Solde : 0,00 DA",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            BackColor =
                DrawingColor.FromArgb(238, 244, 250),
            TextAlign =
                ContentAlignment.MiddleCenter,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(5, 3, 5, 5)
        };

        formulaire.Controls.Add(
            cmbCategorie,
            0,
            1
        );

        formulaire.Controls.Add(
            numMontant,
            1,
            1
        );

        formulaire.Controls.Add(
            dtpDateRecette,
            2,
            1
        );

        formulaire.Controls.Add(
            lblSoldeCaisse,
            3,
            1
        );

        var blocMotif = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = new Padding(0)
        };

        blocMotif.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 36)
        );

        blocMotif.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 44)
        );

        var lblMotif = new Label
        {
            Text = "Motif / observation (facultatif)",
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
            Padding = new Padding(5, 0, 0, 0),
            Margin = Padding.Empty
        };

        txtMotif = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5, 3, 5, 4),
            PlaceholderText =
                "Observation facultative"
        };

        blocMotif.Controls.Add(
            lblMotif,
            0,
            0
        );

        blocMotif.Controls.Add(
            txtMotif,
            0,
            1
        );

        formulaire.Controls.Add(
            blocMotif,
            0,
            2
        );

        formulaire.SetColumnSpan(
            blocMotif,
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

        for (int index = 0; index < 4; index++)
        {
            panneauBoutons.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    25
                )
            );
        }

        var btnEnregistrer = CreerBouton(
            "Enregistrer la recette",
            DrawingColor.FromArgb(38, 138, 83)
        );

        var btnNouvelle = CreerBouton(
            "Nouvelle recette",
            DrawingColor.FromArgb(95, 105, 115)
        );

        var btnAnnuler = CreerBouton(
            "Annuler la recette",
            DrawingColor.FromArgb(190, 58, 58)
        );

        var btnActualiser = CreerBouton(
            "Actualiser",
            DrawingColor.FromArgb(32, 113, 171)
        );

        btnEnregistrer.Click +=
            EnregistrerRecette;

        btnNouvelle.Click += (_, _) =>
        {
            ViderFormulaire();
        };

        btnAnnuler.Click +=
            AnnulerRecetteSelectionnee;

        btnActualiser.Click += (_, _) =>
        {
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

        formulaire.Controls.Add(
            panneauBoutons,
            0,
            3
        );

        formulaire.SetColumnSpan(
            panneauBoutons,
            4
        );

        return carte;
    }

    private void GererChoixCategorie()
    {
        string choix =
            Convert.ToString(
                cmbCategorie.SelectedItem
            ) ?? "";

        if (
            choix.Equals(
                "Autre recette",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            cmbCategorie.DropDownStyle =
                ComboBoxStyle.DropDown;

            cmbCategorie.SelectedIndex = -1;
            cmbCategorie.Text = "";

            lblCategorie.Text =
                "Catégorie — écrivez la recette *";

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

            return;
        }

        ReinitialiserApparenceCategorie();
    }

    private void ReinitialiserApparenceCategorie()
    {
        lblCategorie.Text = "Catégorie";

        cmbCategorie.BackColor =
            DrawingColor.White;

        if (
            cmbCategorie.SelectedIndex >= 0)
        {
            cmbCategorie.DropDownStyle =
                ComboBoxStyle.DropDownList;
        }
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
                "Catégorie, motif ou numéro"
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

    private DataGridView CreerTableauRecettes()
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
                Name = "date_recette",
                DataPropertyName = "date_recette",
                HeaderText = "Date",
                Width = 165,
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
                260
            )
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "montant",
                DataPropertyName = "montant",
                HeaderText = "Montant",
                Width = 165,
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
                300,
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

        tableau.CellClick +=
            SelectionnerRecette;

        tableau.CellFormatting +=
            ColorerRecette;

        return tableau;
    }

    private void EnregistrerRecette(
        object? sender,
        EventArgs e)
    {
        string categorie =
            cmbCategorie.Text.Trim();

        if (
            string.IsNullOrWhiteSpace(categorie) ||
            categorie.Equals(
                "Autre recette",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            AfficherAvertissement(
                "Sélectionnez une catégorie ou écrivez " +
                "directement la nature de la recette."
            );

            cmbCategorie.DropDownStyle =
                ComboBoxStyle.DropDown;

            lblCategorie.Text =
                "Catégorie — écrivez la recette *";

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
            dtpDateRecette.Value >
            DateTime.Now.AddMinutes(5))
        {
            AfficherAvertissement(
                "La date de la recette ne peut pas être future."
            );

            dtpDateRecette.Focus();
            return;
        }

        string motif =
            txtMotif.Text.Trim();

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                const string sqlRecette = """
                    INSERT INTO recettes
                    (
                        categorie,
                        montant,
                        motif,
                        mode_paiement,
                        date_recette,
                        statut
                    )
                    VALUES
                    (
                        @categorie,
                        @montant,
                        @motif,
                        @modePaiement,
                        @dateRecette,
                        'VALIDEE'
                    );
                    """;

                using MySqlCommand recetteCommand =
                    new MySqlCommand(
                        sqlRecette,
                        connection,
                        transaction
                    );

                recetteCommand.Parameters.AddWithValue(
                    "@categorie",
                    categorie
                );

                recetteCommand.Parameters.AddWithValue(
                    "@montant",
                    montant
                );

                recetteCommand.Parameters.AddWithValue(
                    "@motif",
                    motif
                );

                recetteCommand.Parameters.AddWithValue(
                    "@modePaiement",
                    ModePaiementParDefaut
                );

                recetteCommand.Parameters.AddWithValue(
                    "@dateRecette",
                    dtpDateRecette.Value
                );

                recetteCommand.ExecuteNonQuery();

                int recetteId =
                    Convert.ToInt32(
                        recetteCommand.LastInsertedId
                    );

                string motifCaisse =
                    ConstruireMotifCaisse(
                        categorie,
                        motif,
                        recetteId
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
                        'ENTREE',
                        'RECETTE',
                        @montant,
                        @motif,
                        'RECETTE',
                        @recetteId,
                        @dateRecette
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
                    "@recetteId",
                    recetteId
                );

                caisseCommand.Parameters.AddWithValue(
                    "@dateRecette",
                    dtpDateRecette.Value
                );

                caisseCommand.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show(
                    "La recette a été enregistrée.\n\n" +
                    $"N° : {recetteId}\n" +
                    $"Catégorie : {categorie}\n" +
                    $"Montant : {montant:N2} DA\n" +
                    "Le montant a été ajouté à la caisse.",
                    "Recette enregistrée",
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
                "Impossible d’enregistrer la recette.",
                ex
            );
        }
    }

    private static string ConstruireMotifCaisse(
        string categorie,
        string motif,
        int recetteId)
    {
        string texte =
            $"Recette n° {recetteId} - {categorie}";

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

    private void AnnulerRecetteSelectionnee(
        object? sender,
        EventArgs e)
    {
        if (recetteSelectionneeId == 0)
        {
            AfficherAvertissement(
                "Sélectionnez une recette dans le tableau."
            );

            return;
        }

        DataGridViewRow? ligne =
            dgvRecettes.CurrentRow;

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
                "Cette recette est déjà annulée.",
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
                "Voulez-vous vraiment annuler cette recette ?\n\n" +
                $"N° : {recetteSelectionneeId}\n" +
                $"Catégorie : {categorie}\n" +
                $"Montant : {montant:N2} DA\n\n" +
                "Le montant sera retiré de la caisse.",
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
                    FROM recettes
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
                        recetteSelectionneeId
                    );

                    using MySqlDataReader reader =
                        command.ExecuteReader();

                    if (!reader.Read())
                    {
                        throw new InvalidOperationException(
                            "La recette sélectionnée n’existe plus."
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
                        "Cette recette est déjà annulée."
                    );
                }

                decimal soldeCaisse =
                    LireSoldeCaisse(
                        connection,
                        transaction
                    );

                if (montantBase > soldeCaisse)
                {
                    throw new InvalidOperationException(
                        "Le solde de la caisse est insuffisant " +
                        "pour annuler cette recette.\n\n" +
                        $"Solde disponible : {soldeCaisse:N2} DA\n" +
                        $"Montant à retirer : {montantBase:N2} DA"
                    );
                }

                const string sqlUpdate = """
                    UPDATE recettes
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
                        recetteSelectionneeId
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
                        'SORTIE',
                        'ANNULATION_RECETTE',
                        @montant,
                        @motif,
                        'RECETTE',
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
                        $"Annulation recette n° " +
                        $"{recetteSelectionneeId} - " +
                        categorieBase
                    );

                    command.Parameters.AddWithValue(
                        "@id",
                        recetteSelectionneeId
                    );

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                MessageBox.Show(
                    "La recette a été annulée.\n\n" +
                    "Le montant a été retiré de la caisse.",
                    "Recette annulée",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                recetteSelectionneeId = 0;
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
                "Impossible d’annuler la recette.",
                ex
            );
        }
    }

    private void ChargerHistorique()
    {
        if (dgvRecettes == null)
        {
            return;
        }

        if (
            dtpDateDebut.Value.Date >
            dtpDateFin.Value.Date)
        {
            AfficherAvertissement(
                "La date début doit être antérieure " +
                "ou égale à la date fin."
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
                    id,
                    date_recette,
                    categorie,
                    montant,
                    motif,

                    CASE statut
                        WHEN 'VALIDEE'
                            THEN 'Validée'
                        WHEN 'ANNULEE'
                            THEN 'Annulée'
                        ELSE statut
                    END AS statut

                FROM recettes

                WHERE date_recette >= @dateDebut
                  AND date_recette < @dateFinExclusive
                """;

            string recherche =
                txtRecherche?.Text.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                sql += """

                    AND
                    (
                        categorie LIKE @recherche
                        OR motif LIKE @recherche
                        OR CAST(id AS CHAR) LIKE @recherche
                    )
                    """;
            }

            sql += """

                ORDER BY
                    date_recette DESC,
                    id DESC;
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

            dgvRecettes.DataSource = table;
            dgvRecettes.ClearSelection();
            recetteSelectionneeId = 0;

            ActualiserTotalPeriode();
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger l’historique des recettes.",
                ex
            );
        }
    }

    private void ActualiserTotalPeriode()
    {
        decimal total = 0;

        foreach (
            DataGridViewRow ligne
            in dgvRecettes.Rows)
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
                $"Solde : {solde:N2} DA";

            lblSoldeCaisse.ForeColor =
                solde < 0
                    ? DrawingColor.FromArgb(190, 58, 58)
                    : DrawingColor.FromArgb(25, 49, 78);
        }
        catch (Exception ex)
        {
            lblSoldeCaisse.Text =
                "Solde indisponible";

            lblSoldeCaisse.ForeColor =
                DrawingColor.FromArgb(190, 58, 58);

            Debug.WriteLine(ex.Message);
        }
    }

    private void SelectionnerRecette(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        recetteSelectionneeId =
            Convert.ToInt32(
                dgvRecettes.Rows[e.RowIndex]
                    .Cells["id"]
                    .Value
            );
    }

    private static void ColorerRecette(
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

    private sealed class LigneRecettePdf
    {
        public int Id { get; init; }
        public DateTime DateRecette { get; init; }
        public string Categorie { get; init; } = "";
        public decimal Montant { get; init; }
        public string Motif { get; init; } = "";
        public string Statut { get; init; } = "";
    }

    private void ImprimerHistoriquePdf(
        object? sender,
        EventArgs e)
    {
        if (dgvRecettes.Rows.Count == 0)
        {
            MessageBox.Show(
                "Aucune recette n’est affichée pour cette période.",
                "PDF impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        try
        {
            List<LigneRecettePdf> lignes =
                ObtenirLignesPdf();

            using var dialogue =
                new SaveFileDialog
                {
                    Title =
                        "Enregistrer l’historique des recettes",

                    Filter =
                        "Fichier PDF (*.pdf)|*.pdf",

                    FileName =
                        $"Recettes_" +
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

            GenererPdfRecettes(
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

    private List<LigneRecettePdf> ObtenirLignesPdf()
    {
        var lignes =
            new List<LigneRecettePdf>();

        foreach (
            DataGridViewRow ligne
            in dgvRecettes.Rows)
        {
            if (ligne.IsNewRow)
            {
                continue;
            }

            lignes.Add(
                new LigneRecettePdf
                {
                    Id = Convert.ToInt32(
                        ligne.Cells["id"].Value
                    ),

                    DateRecette =
                        Convert.ToDateTime(
                            ligne.Cells["date_recette"].Value
                        ),

                    Categorie =
                        Convert.ToString(
                            ligne.Cells["categorie"].Value
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

    private void GenererPdfRecettes(
        List<LigneRecettePdf> lignes,
        string chemin)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        decimal totalValide = 0;

        foreach (
            LigneRecettePdf ligne
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
                             .FontSize(9)
                );

                page.Header()
                    .Column(entete =>
                    {
                        entete.Item()
                            .Text(
                                "HISTORIQUE DES RECETTES"
                            )
                            .Bold()
                            .FontSize(19)
                            .FontColor(
                                Colors.Green.Darken2
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
                                        columns.ConstantColumn(45);
                                        columns.RelativeColumn(1.3F);
                                        columns.RelativeColumn(2.3F);
                                        columns.RelativeColumn(1.3F);
                                        columns.RelativeColumn(3.0F);
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
                                        .Text("Montant");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Motif");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Statut");
                                });

                                foreach (
                                    LigneRecettePdf ligne
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
                                            ligne.DateRecette.ToString(
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
                                        "Nombre de recettes : " +
                                        lignes.Count
                                    )
                                    .Bold();

                                resume.RelativeItem()
                                    .AlignRight()
                                    .Text(
                                        "Total des recettes validées : " +
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
            .Background(Colors.Green.Darken2)
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
            .BorderColor(Colors.Grey.Lighten2)
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
        recetteSelectionneeId = 0;

        cmbCategorie.DropDownStyle =
            ComboBoxStyle.DropDownList;

        lblCategorie.Text = "Catégorie";

        cmbCategorie.BackColor =
            DrawingColor.White;

        if (cmbCategorie.Items.Count > 0)
        {
            cmbCategorie.SelectedIndex = 0;
        }

        numMontant.Value = 0;
        dtpDateRecette.Value = DateTime.Now;
        txtMotif.Clear();

        dgvRecettes.ClearSelection();
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
