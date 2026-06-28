using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using DrawingColor = System.Drawing.Color;
using System.IO;
using System.Windows.Forms;
using MySqlConnector;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionUsine;

public sealed class EmployesControl : UserControl
{
    private TextBox txtNom = null!;
    private TextBox txtPrenom = null!;
    private TextBox txtTelephone = null!;
    private TextBox txtPoste = null!;
    private TextBox txtRecherche = null!;

    private NumericUpDown numSalaireBase = null!;
    private DateTimePicker dtpDateEmbauche = null!;
    private ComboBox cmbFiltreStatut = null!;

    private DataGridView dgvEmployes = null!;

    private int employeSelectionneId;

    public EmployesControl()
    {
        Dock = DockStyle.Fill;
        BackColor = DrawingColor.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        Controls.Add(CreerInterface());
        ChargerEmployes();
    }

    private Control CreerInterface()
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = DrawingColor.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 60)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 285)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var titre = new Label
        {
            Text = "Gestion des employés",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = DrawingColor.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(titre, 0, 0);
        structure.Controls.Add(CreerCarteFormulaire(), 0, 1);

        dgvEmployes = CreerTableauEmployes();
        structure.Controls.Add(dgvEmployes, 0, 2);

        return structure;
    }

    private Control CreerCarteFormulaire()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            Padding = new Padding(15),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 10)
        };

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 5,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        for (int i = 0; i < 4; i++)
        {
            formulaire.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25)
            );
        }

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 38)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 52)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 38)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 52)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carte.Controls.Add(formulaire);

        formulaire.Controls.Add(CreerLabel("Nom"), 0, 0);
        formulaire.Controls.Add(CreerLabel("Prénom"), 1, 0);
        formulaire.Controls.Add(CreerLabel("Téléphone"), 2, 0);
        formulaire.Controls.Add(CreerLabel("Poste"), 3, 0);

        txtNom = CreerChampTexte();
        txtPrenom = CreerChampTexte();
        txtTelephone = CreerChampTexte();
        txtPoste = CreerChampTexte();

        formulaire.Controls.Add(txtNom, 0, 1);
        formulaire.Controls.Add(txtPrenom, 1, 1);
        formulaire.Controls.Add(txtTelephone, 2, 1);
        formulaire.Controls.Add(txtPoste, 3, 1);

        formulaire.Controls.Add(
            CreerLabel("Salaire de base (DA)"),
            0,
            2
        );

        formulaire.Controls.Add(
            CreerLabel("Date d’embauche"),
            1,
            2
        );

        formulaire.Controls.Add(
            CreerLabel("Recherche"),
            2,
            2
        );

        formulaire.Controls.Add(
            CreerLabel("Filtre des employés"),
            3,
            2
        );

        numSalaireBase = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 1_000_000_000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        dtpDateEmbauche = new DateTimePicker
        {
            Dock = DockStyle.Fill,
            Format = DateTimePickerFormat.Custom,
            CustomFormat = "dd/MM/yyyy",
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5),
            Value = DateTime.Today
        };

        txtRecherche = CreerChampTexte();
        txtRecherche.PlaceholderText =
            "ID, nom, prénom, téléphone ou poste";

        txtRecherche.TextChanged += (_, _) =>
        {
            ChargerEmployes();
        };

        cmbFiltreStatut = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10),
            Margin = new Padding(5)
        };

        cmbFiltreStatut.Items.AddRange(
            new object[]
            {
                "Tous les employés",
                "Employés actifs",
                "Employés inactifs"
            }
        );

        cmbFiltreStatut.SelectedIndex = 1;

        cmbFiltreStatut.SelectedIndexChanged += (_, _) =>
        {
            ChargerEmployes();
        };

        formulaire.Controls.Add(numSalaireBase, 0, 3);
        formulaire.Controls.Add(dtpDateEmbauche, 1, 3);
        formulaire.Controls.Add(txtRecherche, 2, 3);
        formulaire.Controls.Add(cmbFiltreStatut, 3, 3);

        var panneauBas = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = new Padding(0, 8, 0, 0)
        };

        for (int i = 0; i < 6; i++)
        {
            panneauBas.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    16.66F
                )
            );
        }

        var btnAjouter = CreerBouton(
            "Ajouter",
            DrawingColor.FromArgb(38, 138, 83)
        );

        var btnModifier = CreerBouton(
            "Modifier",
            DrawingColor.FromArgb(32, 113, 171)
        );

        var btnDesactiver = CreerBouton(
            "Désactiver",
            DrawingColor.FromArgb(190, 58, 58)
        );

        var btnReactiver = CreerBouton(
            "Réactiver",
            DrawingColor.FromArgb(199, 125, 27)
        );

        var btnNouveau = CreerBouton(
            "Nouveau",
            DrawingColor.FromArgb(95, 105, 115)
        );

        var btnImprimer = CreerBouton(
            "Imprimer PDF",
            DrawingColor.FromArgb(93, 63, 137)
        );

        btnAjouter.Click += AjouterEmploye;
        btnModifier.Click += ModifierEmploye;

        btnDesactiver.Click += (_, _) =>
        {
            ChangerStatutEmploye(false);
        };

        btnReactiver.Click += (_, _) =>
        {
            ChangerStatutEmploye(true);
        };

        btnNouveau.Click += (_, _) =>
        {
            ViderFormulaire();
        };

        btnImprimer.Click += ImprimerListeEmployes;

        panneauBas.Controls.Add(btnAjouter, 0, 0);
        panneauBas.Controls.Add(btnModifier, 1, 0);
        panneauBas.Controls.Add(btnDesactiver, 2, 0);
        panneauBas.Controls.Add(btnReactiver, 3, 0);
        panneauBas.Controls.Add(btnNouveau, 4, 0);
        panneauBas.Controls.Add(btnImprimer, 5, 0);

        formulaire.Controls.Add(panneauBas, 0, 4);
        formulaire.SetColumnSpan(panneauBas, 4);

        return carte;
    }

    private DataGridView CreerTableauEmployes()
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
            ColumnHeadersHeight = 50,
            ColumnHeadersHeightSizeMode =
                DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
            RowTemplate = { Height = 36 },
            Margin = new Padding(0, 5, 0, 0)
        };

        tableau.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        tableau.ColumnHeadersDefaultCellStyle.BackColor =
            DrawingColor.FromArgb(25, 49, 78);

        tableau.ColumnHeadersDefaultCellStyle.ForeColor =
            DrawingColor.White;

        tableau.ColumnHeadersDefaultCellStyle.Alignment =
            DataGridViewContentAlignment.MiddleLeft;

        tableau.ColumnHeadersDefaultCellStyle.WrapMode =
            DataGridViewTriState.False;

        tableau.EnableHeadersVisualStyles = false;

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "id",
                DataPropertyName = "id",
                Visible = false
            }
        );

        tableau.Columns.Add(
            CreerColonne(
                "id_affiche",
                "id",
                "ID",
                90
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "nom_complet",
                "nom_complet",
                "Nom complet",
                230,
                true
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "telephone",
                "telephone",
                "Téléphone",
                150
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "poste",
                "poste",
                "Poste",
                180
            )
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "salaire_base",
                DataPropertyName = "salaire_base",
                HeaderText = "Salaire",
                Width = 185,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format = "#,##0.00 'DA'",
                        Alignment =
                            DataGridViewContentAlignment
                                .MiddleRight
                    }
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "date_embauche",
                DataPropertyName = "date_embauche",
                HeaderText = "Date d’embauche",
                Width = 185,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format = "dd/MM/yyyy"
                    }
            }
        );

        tableau.Columns.Add(
            CreerColonne(
                "statut",
                "statut",
                "Statut",
                110
            )
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "nom",
                DataPropertyName = "nom",
                Visible = false
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "prenom",
                DataPropertyName = "prenom",
                Visible = false
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "actif",
                DataPropertyName = "actif",
                Visible = false
            }
        );

        tableau.CellClick += SelectionnerEmploye;
        tableau.CellFormatting += ColorerStatut;

        return tableau;
    }

    private void AjouterEmploye(
        object? sender,
        EventArgs e)
    {
        if (!ValiderFormulaire())
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                INSERT INTO employes
                (
                    nom,
                    prenom,
                    telephone,
                    poste,
                    salaire_base,
                    date_embauche,
                    actif
                )
                VALUES
                (
                    @nom,
                    @prenom,
                    @telephone,
                    @poste,
                    @salaire,
                    @dateEmbauche,
                    TRUE
                );
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            AjouterParametresEmploye(command);
            command.ExecuteNonQuery();

            int nouvelId =
                Convert.ToInt32(command.LastInsertedId);

            MessageBox.Show(
                "L’employé a été ajouté.",
                "Ajout réussi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerEmployes(nouvelId);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                ex.Message,
                "Ajout impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ModifierEmploye(
        object? sender,
        EventArgs e)
    {
        if (employeSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un employé dans le tableau.",
                "Employé non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        if (!ValiderFormulaire())
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                UPDATE employes
                SET
                    nom = @nom,
                    prenom = @prenom,
                    telephone = @telephone,
                    poste = @poste,
                    salaire_base = @salaire,
                    date_embauche = @dateEmbauche
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            AjouterParametresEmploye(command);

            command.Parameters.AddWithValue(
                "@id",
                employeSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "L’employé a été modifié.",
                "Modification réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerEmployes(employeSelectionneId);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                ex.Message,
                "Modification impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ChangerStatutEmploye(
        bool actif)
    {
        if (employeSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un employé dans le tableau.",
                "Employé non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        string action =
            actif ? "réactiver" : "désactiver";

        DialogResult confirmation =
            MessageBox.Show(
                $"Voulez-vous {action} cet employé ?",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
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

            const string sql = """
                UPDATE employes
                SET actif = @actif
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@actif",
                actif
            );

            command.Parameters.AddWithValue(
                "@id",
                employeSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                actif
                    ? "L’employé a été réactivé."
                    : "L’employé a été désactivé.",
                "Statut mis à jour",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerEmployes(employeSelectionneId);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de modifier le statut.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ChargerEmployes(
        int? employeIdASelectionner = null)
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            string sql = """
                SELECT
                    id,
                    nom,
                    prenom,
                    CONCAT(nom, ' ', prenom) AS nom_complet,
                    telephone,
                    poste,
                    salaire_base,
                    date_embauche,
                    actif,
                    CASE
                        WHEN actif = TRUE THEN 'Actif'
                        ELSE 'Inactif'
                    END AS statut
                FROM employes
                WHERE 1 = 1
                """;

            if (cmbFiltreStatut != null)
            {
                if (cmbFiltreStatut.SelectedIndex == 1)
                {
                    sql += "\n AND actif = TRUE";
                }
                else if (cmbFiltreStatut.SelectedIndex == 2)
                {
                    sql += "\n AND actif = FALSE";
                }
            }

            string recherche =
                txtRecherche?.Text.Trim() ?? "";

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                sql += """

                    AND
                    (
                        CAST(id AS CHAR) LIKE @recherche
                        OR nom LIKE @recherche
                        OR prenom LIKE @recherche
                        OR poste LIKE @recherche
                        OR telephone LIKE @recherche
                    )
                    """;
            }

            sql += """

                ORDER BY
                    actif DESC,
                    nom,
                    prenom;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

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

            dgvEmployes.DataSource = table;
            dgvEmployes.ClearSelection();

            if (employeIdASelectionner.HasValue)
            {
                SelectionnerEmployeDansTableau(
                    employeIdASelectionner.Value
                );
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger les employés.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SelectionnerEmploye(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        RemplirFormulaireDepuisLigne(
            dgvEmployes.Rows[e.RowIndex]
        );
    }

    private void SelectionnerEmployeDansTableau(
        int employeId)
    {
        foreach (DataGridViewRow ligne in dgvEmployes.Rows)
        {
            if (Convert.ToInt32(
                    ligne.Cells["id"].Value
                ) != employeId)
            {
                continue;
            }

            ligne.Selected = true;
            dgvEmployes.CurrentCell =
                ligne.Cells["id_affiche"];

            RemplirFormulaireDepuisLigne(ligne);
            break;
        }
    }

    private void RemplirFormulaireDepuisLigne(
        DataGridViewRow ligne)
    {
        employeSelectionneId =
            Convert.ToInt32(
                ligne.Cells["id"].Value
            );

        txtNom.Text =
            Convert.ToString(
                ligne.Cells["nom"].Value
            ) ?? "";

        txtPrenom.Text =
            Convert.ToString(
                ligne.Cells["prenom"].Value
            ) ?? "";

        txtTelephone.Text =
            Convert.ToString(
                ligne.Cells["telephone"].Value
            ) ?? "";

        txtPoste.Text =
            Convert.ToString(
                ligne.Cells["poste"].Value
            ) ?? "";

        numSalaireBase.Value =
            Math.Min(
                Convert.ToDecimal(
                    ligne.Cells["salaire_base"].Value
                ),
                numSalaireBase.Maximum
            );

        dtpDateEmbauche.Value =
            Convert.ToDateTime(
                ligne.Cells["date_embauche"].Value
            );
    }

    private bool ValiderFormulaire()
    {
        if (string.IsNullOrWhiteSpace(txtNom.Text))
        {
            AfficherChampObligatoire(
                "Veuillez saisir le nom.",
                txtNom
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPrenom.Text))
        {
            AfficherChampObligatoire(
                "Veuillez saisir le prénom.",
                txtPrenom
            );

            return false;
        }

        if (string.IsNullOrWhiteSpace(txtPoste.Text))
        {
            AfficherChampObligatoire(
                "Veuillez saisir le poste.",
                txtPoste
            );

            return false;
        }

        if (dtpDateEmbauche.Value.Date > DateTime.Today)
        {
            MessageBox.Show(
                "La date d’embauche ne peut pas être future.",
                "Date incorrecte",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            dtpDateEmbauche.Focus();
            return false;
        }

        return true;
    }

    private void AjouterParametresEmploye(
        MySqlCommand command)
    {
        command.Parameters.AddWithValue(
            "@nom",
            txtNom.Text.Trim()
        );

        command.Parameters.AddWithValue(
            "@prenom",
            txtPrenom.Text.Trim()
        );

        command.Parameters.AddWithValue(
            "@telephone",
            string.IsNullOrWhiteSpace(txtTelephone.Text)
                ? DBNull.Value
                : txtTelephone.Text.Trim()
        );

        command.Parameters.AddWithValue(
            "@poste",
            txtPoste.Text.Trim()
        );

        command.Parameters.AddWithValue(
            "@salaire",
            numSalaireBase.Value
        );

        command.Parameters.AddWithValue(
            "@dateEmbauche",
            dtpDateEmbauche.Value.Date
        );
    }

    private void ViderFormulaire()
    {
        employeSelectionneId = 0;

        txtNom.Clear();
        txtPrenom.Clear();
        txtTelephone.Clear();
        txtPoste.Clear();

        numSalaireBase.Value = 0;
        dtpDateEmbauche.Value = DateTime.Today;

        dgvEmployes.ClearSelection();
        txtNom.Focus();
    }

    private sealed class LigneEmployePdf
    {
        public int Id { get; init; }
        public string NomComplet { get; init; } = "";
        public string Telephone { get; init; } = "";
        public string Poste { get; init; } = "";
        public decimal SalaireBase { get; init; }
        public DateTime DateEmbauche { get; init; }
        public string Statut { get; init; } = "";
    }

    private void ImprimerListeEmployes(
        object? sender,
        EventArgs e)
    {
        if (dgvEmployes.Rows.Count == 0)
        {
            MessageBox.Show(
                "La liste des employés est vide.",
                "PDF impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        try
        {
            List<LigneEmployePdf> lignes =
                ObtenirLignesEmployesPourPdf();

            using var dialogue =
                new SaveFileDialog
                {
                    Title =
                        "Enregistrer la liste des employés en PDF",

                    Filter =
                        "Fichier PDF (*.pdf)|*.pdf",

                    FileName =
                        $"Liste_Employes_" +
                        $"{DateTime.Now:yyyyMMdd_HHmmss}.pdf",

                    AddExtension = true,
                    DefaultExt = "pdf",
                    OverwritePrompt = true
                };

            if (dialogue.ShowDialog(this) !=
                DialogResult.OK)
            {
                return;
            }

            string chemin =
                dialogue.FileName;

            GenererPdfEmployes(
                lignes,
                chemin
            );

            DialogResult resultat =
                MessageBox.Show(
                    "Le PDF a été créé correctement.\n\n" +
                    chemin +
                    "\n\nVoulez-vous l’ouvrir maintenant ?",
                    "PDF créé",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Information
                );

            if (resultat == DialogResult.Yes)
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = chemin,
                        UseShellExecute = true
                    }
                );
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible de créer le PDF.\n\n" +
                ex.Message,
                "Erreur PDF",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private List<LigneEmployePdf>
        ObtenirLignesEmployesPourPdf()
    {
        var lignes =
            new List<LigneEmployePdf>();

        foreach (
            DataGridViewRow ligne
            in dgvEmployes.Rows)
        {
            if (ligne.IsNewRow)
            {
                continue;
            }

            lignes.Add(
                new LigneEmployePdf
                {
                    Id = Convert.ToInt32(
                        ligne.Cells["id_affiche"].Value
                    ),

                    NomComplet =
                        Convert.ToString(
                            ligne.Cells["nom_complet"].Value
                        ) ?? "",

                    Telephone =
                        Convert.ToString(
                            ligne.Cells["telephone"].Value
                        ) ?? "",

                    Poste =
                        Convert.ToString(
                            ligne.Cells["poste"].Value
                        ) ?? "",

                    SalaireBase =
                        Convert.ToDecimal(
                            ligne.Cells["salaire_base"].Value
                        ),

                    DateEmbauche =
                        Convert.ToDateTime(
                            ligne.Cells["date_embauche"].Value
                        ),

                    Statut =
                        Convert.ToString(
                            ligne.Cells["statut"].Value
                        ) ?? ""
                }
            );
        }

        return lignes;
    }

    private void GenererPdfEmployes(
        List<LigneEmployePdf> lignes,
        string chemin)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        string filtre =
            cmbFiltreStatut.Text;

        string recherche =
            txtRecherche.Text.Trim();

        decimal totalSalaires = 0;

        foreach (
            LigneEmployePdf ligne
            in lignes)
        {
            totalSalaires +=
                ligne.SalaireBase;
        }

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
                                "LISTE DES EMPLOYÉS"
                            )
                            .Bold()
                            .FontSize(20)
                            .FontColor(
                                Colors.Blue.Darken3
                            );

                        entete.Item()
                            .PaddingTop(4)
                            .Text(texte =>
                            {
                                texte.Span(
                                    "Date d’impression : "
                                );

                                texte.Span(
                                    DateTime.Now.ToString(
                                        "dd/MM/yyyy HH:mm"
                                    )
                                );

                                texte.Span(
                                    "   |   Filtre : "
                                );

                                texte.Span(filtre);

                                if (
                                    !string.IsNullOrWhiteSpace(
                                        recherche
                                    )
                                )
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
                                        columns.ConstantColumn(42);
                                        columns.RelativeColumn(2.2F);
                                        columns.RelativeColumn(1.35F);
                                        columns.RelativeColumn(1.65F);
                                        columns.RelativeColumn(1.4F);
                                        columns.RelativeColumn(1.25F);
                                        columns.RelativeColumn(1.0F);
                                    }
                                );

                                table.Header(header =>
                                {
                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text("ID");

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text("Nom complet");

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text("Téléphone");

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text("Poste");

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text(
                                            "Salaire de base"
                                        );

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text(
                                            "Date d’embauche"
                                        );

                                    header.Cell()
                                        .Element(
                                            StyleEntetePdf
                                        )
                                        .Text("Statut");
                                });

                                foreach (
                                    LigneEmployePdf ligne
                                    in lignes)
                                {
                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .AlignCenter()
                                        .Text(
                                            ligne.Id.ToString()
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .Text(
                                            ValeurPdf(
                                                ligne.NomComplet
                                            )
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .Text(
                                            ValeurPdf(
                                                ligne.Telephone
                                            )
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .Text(
                                            ValeurPdf(
                                                ligne.Poste
                                            )
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .AlignRight()
                                        .Text(
                                            $"{ligne.SalaireBase:N2} DA"
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
                                        .AlignCenter()
                                        .Text(
                                            ligne.DateEmbauche
                                                .ToString(
                                                    "dd/MM/yyyy"
                                                )
                                        );

                                    table.Cell()
                                        .Element(
                                            StyleCellulePdf
                                        )
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
                                        $"Nombre d’employés : " +
                                        $"{lignes.Count}"
                                    )
                                    .Bold();

                                resume.RelativeItem()
                                    .AlignRight()
                                    .Text(
                                        "Total des salaires de base : " +
                                        $"{totalSalaires:N2} DA"
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
            .Padding(7)
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
            .Padding(6)
            .AlignMiddle();
    }

    private static string ValeurPdf(
        string valeur)
    {
        return string.IsNullOrWhiteSpace(valeur)
            ? "-"
            : valeur.Trim();
    }

    private static void ColorerStatut(
        object? sender,
        DataGridViewCellFormattingEventArgs e)
    {
        if (sender is not DataGridView tableau ||
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

        if (statut == "Inactif")
        {
            tableau.Rows[e.RowIndex]
                .DefaultCellStyle.BackColor =
                DrawingColor.FromArgb(238, 238, 238);

            tableau.Rows[e.RowIndex]
                .DefaultCellStyle.ForeColor =
                DrawingColor.FromArgb(115, 115, 115);
        }
    }

    private static TextBox CreerChampTexte()
    {
        return new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };
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
            ForeColor = DrawingColor.FromArgb(50, 60, 70),
            TextAlign = ContentAlignment.MiddleLeft,
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
            Height = 42,
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

    private static DataGridViewTextBoxColumn CreerColonne(
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

    private static void AfficherChampObligatoire(
        string message,
        Control controle)
    {
        MessageBox.Show(
            message,
            "Champ obligatoire",
            MessageBoxButtons.OK,
            MessageBoxIcon.Warning
        );

        controle.Focus();
    }
}
