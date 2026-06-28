using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace GestionUsine;

public sealed class StockEmballagesControl : UserControl
{
    private TextBox txtNom = null!;
    private TextBox txtUnite = null!;
    private NumericUpDown numSeuil = null!;

    private ComboBox cmbEmballages = null!;
    private ComboBox cmbTypeMouvement = null!;
    private NumericUpDown numQuantite = null!;
    private TextBox txtMotif = null!;
    private Label lblStockActuel = null!;

    private DataGridView dgvEmballages = null!;
    private DataGridView dgvMouvements = null!;

    private int emballageSelectionneId;

    public StockEmballagesControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        Controls.Add(structure);

        var lblTitre = new Label
        {
            Text = "Gestion du stock des emballages",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(lblTitre, 0, 0);

        var onglets = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10),
            Padding = new Point(18, 7)
        };

        var ongletEmballages = new TabPage
        {
            Text = "Liste des emballages",
            BackColor = Color.FromArgb(242, 245, 248),
            Padding = new Padding(10)
        };

        var ongletMouvements = new TabPage
        {
            Text = "Mouvements de stock",
            BackColor = Color.FromArgb(242, 245, 248),
            Padding = new Padding(10)
        };

        onglets.TabPages.Add(ongletEmballages);
        onglets.TabPages.Add(ongletMouvements);

        structure.Controls.Add(onglets, 0, 1);

        CreerOngletEmballages(ongletEmballages);
        CreerOngletMouvements(ongletMouvements);

        ChargerEmballages();
        ChargerHistorique();
    }

    private void CreerOngletEmballages(TabPage page)
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 210)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        page.Controls.Add(structure);

        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20),
            BorderStyle = BorderStyle.FixedSingle
        };

        structure.Controls.Add(carte, 0, 0);

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3
        };

        for (int i = 0; i < 3; i++)
        {
            formulaire.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 33.33F)
            );
        }

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carte.Controls.Add(formulaire);

        formulaire.Controls.Add(
            CreerLabel("Nom de l’emballage"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Unité"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Seuil d’alerte"),
            2,
            0
        );

        txtNom = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Margin = new Padding(5)
        };

        txtUnite = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Text = "unité",
            Margin = new Padding(5)
        };

        numSeuil = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = 100000000,
            Value = 20,
            ThousandsSeparator = true,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(txtNom, 0, 1);
        formulaire.Controls.Add(txtUnite, 1, 1);
        formulaire.Controls.Add(numSeuil, 2, 1);

        var panneauBoutons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 12, 0, 0)
        };

        formulaire.Controls.Add(panneauBoutons, 0, 2);
        formulaire.SetColumnSpan(panneauBoutons, 3);

        var btnAjouter = CreerBouton(
            "Ajouter",
            Color.FromArgb(38, 138, 83),
            140
        );

        var btnModifier = CreerBouton(
            "Modifier",
            Color.FromArgb(32, 113, 171),
            140
        );

        var btnSupprimer = CreerBouton(
            "Supprimer",
            Color.FromArgb(190, 58, 58),
            140
        );

        var btnNouveau = CreerBouton(
            "Nouveau",
            Color.FromArgb(95, 105, 115),
            140
        );

        btnAjouter.Click += AjouterEmballage;
        btnModifier.Click += ModifierEmballage;
        btnSupprimer.Click += SupprimerEmballage;
        btnNouveau.Click += (_, _) => ViderFormulaire();

        panneauBoutons.Controls.Add(btnAjouter);
        panneauBoutons.Controls.Add(btnModifier);
        panneauBoutons.Controls.Add(btnSupprimer);
        panneauBoutons.Controls.Add(btnNouveau);

        dgvEmballages = CreerTableau();
        dgvEmballages.CellClick += SelectionnerEmballage;

        structure.Controls.Add(dgvEmballages, 0, 1);
    }

    private void CreerOngletMouvements(TabPage page)
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 260)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 280)
        );

        page.Controls.Add(structure);

        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20),
            BorderStyle = BorderStyle.FixedSingle
        };

        structure.Controls.Add(carte, 0, 0);

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4
        };

        for (int i = 0; i < 4; i++)
        {
            formulaire.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 25)
            );
        }

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carte.Controls.Add(formulaire);

        formulaire.Controls.Add(
            CreerLabel("Emballage"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Type de mouvement"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Quantité"),
            2,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Stock actuel"),
            3,
            0
        );

        cmbEmballages = new ComboBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(5)
        };

        cmbEmballages.SelectionChangeCommitted += (_, _) =>
        {
            AfficherStockActuel();
            ChargerHistorique();
        };

        cmbTypeMouvement = new ComboBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(5)
        };

        cmbTypeMouvement.Items.AddRange(
            new object[]
            {
                "Achat / entrée",
                "Utilisation production",
                "Perte / emballage abîmé"
            }
        );

        cmbTypeMouvement.SelectedIndex = 0;

        numQuantite = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 1,
            Maximum = 100000000,
            Value = 1,
            ThousandsSeparator = true,
            Margin = new Padding(5)
        };

        lblStockActuel = new Label
        {
            Text = "0 unité",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                16,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(cmbEmballages, 0, 1);
        formulaire.Controls.Add(cmbTypeMouvement, 1, 1);
        formulaire.Controls.Add(numQuantite, 2, 1);
        formulaire.Controls.Add(lblStockActuel, 3, 1);

        formulaire.Controls.Add(
            CreerLabel("Motif / observation"),
            0,
            2
        );

        txtMotif = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(txtMotif, 1, 2);
        formulaire.SetColumnSpan(txtMotif, 3);

        var panneauBoutons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };

        formulaire.Controls.Add(panneauBoutons, 0, 3);
        formulaire.SetColumnSpan(panneauBoutons, 4);

        var btnEnregistrer = CreerBouton(
            "Enregistrer le mouvement",
            Color.FromArgb(38, 138, 83),
            230
        );

        var btnActualiser = CreerBouton(
            "Actualiser",
            Color.FromArgb(32, 113, 171),
            140
        );

        btnEnregistrer.Click += EnregistrerMouvement;

        btnActualiser.Click += (_, _) =>
        {
            ChargerEmballages();
            ChargerHistorique();
        };

        panneauBoutons.Controls.Add(btnEnregistrer);
        panneauBoutons.Controls.Add(btnActualiser);

        dgvMouvements = CreerTableau();

        structure.Controls.Add(dgvMouvements, 0, 1);
    }

    private static Label CreerLabel(string texte)
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
            ForeColor = Color.FromArgb(50, 60, 70),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = new Padding(5)
        };
    }

    private static Button CreerBouton(
        string texte,
        Color couleur,
        int largeur)
    {
        var bouton = new Button
        {
            Text = texte,
            Width = largeur,
            Height = 42,
            BackColor = couleur,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            Cursor = Cursors.Hand,
            Margin = new Padding(5)
        };

        bouton.FlatAppearance.BorderSize = 0;

        return bouton;
    }

    private static DataGridView CreerTableau()
    {
        var tableau = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 10),
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 36 },
            Margin = new Padding(0, 12, 0, 0)
        };

        tableau.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        tableau.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(25, 49, 78);

        tableau.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        tableau.EnableHeadersVisualStyles = false;

        return tableau;
    }

    private bool ValiderEmballage()
    {
        if (string.IsNullOrWhiteSpace(txtNom.Text))
        {
            MessageBox.Show(
                "Veuillez saisir le nom de l’emballage.",
                "Champ obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtNom.Focus();
            return false;
        }

        if (string.IsNullOrWhiteSpace(txtUnite.Text))
        {
            MessageBox.Show(
                "Veuillez saisir l’unité.",
                "Champ obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtUnite.Focus();
            return false;
        }

        return true;
    }

    private void AjouterEmballage(
        object? sender,
        EventArgs e)
    {
        if (!ValiderEmballage())
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                INSERT INTO emballages
                (
                    nom,
                    unite,
                    seuil_alerte
                )
                VALUES
                (
                    @nom,
                    @unite,
                    @seuil
                );
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nom",
                txtNom.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@unite",
                txtUnite.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@seuil",
                Convert.ToInt32(numSeuil.Value)
            );

            command.ExecuteNonQuery();

            int nouvelId =
                Convert.ToInt32(command.LastInsertedId);

            MessageBox.Show(
                "L’emballage a été ajouté.",
                "Ajout réussi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerEmballages(nouvelId);
        }
        catch (MySqlException ex)
        {
            string message = ex.Number == 1062
                ? "Cet emballage existe déjà."
                : ex.Message;

            MessageBox.Show(
                message,
                "Impossible d’ajouter l’emballage",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ModifierEmballage(
        object? sender,
        EventArgs e)
    {
        if (emballageSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un emballage dans le tableau.",
                "Emballage non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        if (!ValiderEmballage())
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                UPDATE emballages
                SET
                    nom = @nom,
                    unite = @unite,
                    seuil_alerte = @seuil
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nom",
                txtNom.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@unite",
                txtUnite.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@seuil",
                Convert.ToInt32(numSeuil.Value)
            );

            command.Parameters.AddWithValue(
                "@id",
                emballageSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "L’emballage a été modifié.",
                "Modification réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerEmballages(emballageSelectionneId);
        }
        catch (MySqlException ex)
        {
            string message = ex.Number == 1062
                ? "Un emballage avec ce nom existe déjà."
                : ex.Message;

            MessageBox.Show(
                message,
                "Modification impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SupprimerEmballage(
        object? sender,
        EventArgs e)
    {
        if (emballageSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un emballage dans le tableau.",
                "Emballage non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        DialogResult reponse = MessageBox.Show(
            "Voulez-vous supprimer cet emballage ?\n\n" +
            "La suppression est impossible s’il possède déjà un historique.",
            "Confirmation",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question
        );

        if (reponse != DialogResult.Yes)
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                DELETE FROM emballages
                WHERE id = @id
                  AND quantite_stock = 0;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                emballageSelectionneId
            );

            int lignes = command.ExecuteNonQuery();

            if (lignes == 0)
            {
                MessageBox.Show(
                    "Cet emballage possède encore du stock.\n" +
                    "Le stock doit être égal à zéro avant sa suppression.",
                    "Suppression impossible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            MessageBox.Show(
                "L’emballage a été supprimé.",
                "Suppression réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ViderFormulaire();
            ChargerEmballages();
            ChargerHistorique();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de supprimer cet emballage.\n\n" +
                "Il possède probablement un historique de mouvements.\n\n" +
                ex.Message,
                "Suppression impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SelectionnerEmballage(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        DataGridViewRow ligne =
            dgvEmballages.Rows[e.RowIndex];

        emballageSelectionneId =
            Convert.ToInt32(ligne.Cells["id"].Value);

        txtNom.Text =
            Convert.ToString(ligne.Cells["nom"].Value) ?? "";

        txtUnite.Text =
            Convert.ToString(ligne.Cells["unite"].Value)
            ?? "unité";

        numSeuil.Value =
            Convert.ToDecimal(
                ligne.Cells["seuil_alerte"].Value
            );

        cmbEmballages.SelectedValue =
            emballageSelectionneId;

        AfficherStockActuel();
        ChargerHistorique();
    }

    private void ViderFormulaire()
    {
        emballageSelectionneId = 0;

        txtNom.Clear();
        txtUnite.Text = "unité";
        numSeuil.Value = 20;

        dgvEmballages.ClearSelection();
        txtNom.Focus();
    }

    private void ChargerEmballages(
        int? emballageIdASelectionner = null)
    {
        try
        {
            int? ancienId =
                ObtenirEmballageSelectionneId();

            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sqlListe = """
                SELECT
                    id,
                    nom,
                    unite,
                    quantite_stock,
                    seuil_alerte,
                    date_creation
                FROM emballages
                WHERE actif = TRUE
                ORDER BY nom;
                """;

          
        using MySqlDataAdapter adapterListe =
    new MySqlDataAdapter(sqlListe, connection);

            var tableListe = new DataTable();
            adapterListe.Fill(tableListe);

            dgvEmballages.DataSource = tableListe;
            if (dgvMouvements.Columns["id"]
                is DataGridViewColumn colonneId)
            {
                colonneId.Visible = false;
            }

            if (dgvMouvements.Columns["emballage"]
                is DataGridViewColumn colonneEmballage)
            {
                colonneEmballage.HeaderText = "Emballage";
            }

            if (dgvMouvements.Columns["type_mouvement"]
                is DataGridViewColumn colonneType)
            {
                colonneType.HeaderText = "Type";
            }

            if (dgvMouvements.Columns["quantite"]
                is DataGridViewColumn colonneQuantite)
            {
                colonneQuantite.HeaderText = "Quantité";
                colonneQuantite.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["ancien_stock"]
                is DataGridViewColumn colonneAncienStock)
            {
                colonneAncienStock.HeaderText = "Ancien stock";
                colonneAncienStock.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["nouveau_stock"]
                is DataGridViewColumn colonneNouveauStock)
            {
                colonneNouveauStock.HeaderText = "Nouveau stock";
                colonneNouveauStock.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["motif"]
                is DataGridViewColumn colonneMotif)
            {
                colonneMotif.HeaderText = "Motif";
            }

            if (dgvMouvements.Columns["date_mouvement"]
                is DataGridViewColumn colonneDate)
            {
                colonneDate.HeaderText = "Date";
                colonneDate.DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";
            }

            dgvMouvements.ClearSelection();

            const string sqlCombo = """
                SELECT
                    id,
                    CONCAT(
                        nom,
                        ' (',
                        unite,
                        ')'
                    ) AS emballage
                FROM emballages
                WHERE actif = TRUE
                ORDER BY nom;
                """;

            using MySqlDataAdapter adapterCombo =
                new MySqlDataAdapter(
                    sqlCombo,
                    connection
                );

            var tableCombo = new DataTable();
            adapterCombo.Fill(tableCombo);

            cmbEmballages.DisplayMember = "emballage";
            cmbEmballages.ValueMember = "id";
            cmbEmballages.DataSource = tableCombo;

            int? idCible =
                emballageIdASelectionner ?? ancienId;

            if (idCible.HasValue)
            {
                cmbEmballages.SelectedValue =
                    idCible.Value;
            }

            if (cmbEmballages.Items.Count > 0 &&
                cmbEmballages.SelectedIndex < 0)
            {
                cmbEmballages.SelectedIndex = 0;
            }

            dgvEmballages.ClearSelection();

            if (emballageIdASelectionner.HasValue)
            {
                foreach (
                    DataGridViewRow ligne
                    in dgvEmballages.Rows)
                {
                    int id = Convert.ToInt32(
                        ligne.Cells["id"].Value
                    );

                    if (id !=
                        emballageIdASelectionner.Value)
                    {
                        continue;
                    }

                    ligne.Selected = true;

                    dgvEmballages.CurrentCell =
                        ligne.Cells["nom"];

                    emballageSelectionneId = id;

                    txtNom.Text =
                        Convert.ToString(
                            ligne.Cells["nom"].Value
                        ) ?? "";

                    txtUnite.Text =
                        Convert.ToString(
                            ligne.Cells["unite"].Value
                        ) ?? "unité";

                    numSeuil.Value =
                        Convert.ToDecimal(
                            ligne.Cells[
                                "seuil_alerte"
                            ].Value
                        );

                    dgvEmballages
                        .FirstDisplayedScrollingRowIndex =
                        ligne.Index;

                    break;
                }
            }

            AfficherStockActuel();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger les emballages.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private int? ObtenirEmballageSelectionneId()
    {
        if (cmbEmballages == null ||
            cmbEmballages.SelectedValue == null ||
            cmbEmballages.SelectedValue is DataRowView)
        {
            return null;
        }

        return Convert.ToInt32(
            cmbEmballages.SelectedValue
        );
    }

    private void AfficherStockActuel()
    {
        int? emballageId =
            ObtenirEmballageSelectionneId();

        if (!emballageId.HasValue)
        {
            lblStockActuel.Text = "0 unité";
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    quantite_stock,
                    unite
                FROM emballages
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                emballageId.Value
            );

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (reader.Read())
            {
                int stock =
                    reader.GetInt32("quantite_stock");

                string unite =
                    reader.GetString("unite");

                lblStockActuel.Text =
                    $"{stock:N0} {unite}";
            }
            else
            {
                lblStockActuel.Text = "0 unité";
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de lire le stock.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void EnregistrerMouvement(
        object? sender,
        EventArgs e)
    {
        int? emballageId =
            ObtenirEmballageSelectionneId();

        if (!emballageId.HasValue)
        {
            MessageBox.Show(
                "Veuillez sélectionner un emballage.",
                "Emballage obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        int quantite =
            Convert.ToInt32(numQuantite.Value);

        string typeBase;
        int variation;

        switch (cmbTypeMouvement.Text)
        {
            case "Achat / entrée":
                typeBase = "ENTREE_ACHAT";
                variation = quantite;
                break;

            case "Utilisation production":
                typeBase = "SORTIE_UTILISATION";
                variation = -quantite;
                break;

            case "Perte / emballage abîmé":
                typeBase = "PERTE";
                variation = -quantite;
                break;

            default:
                MessageBox.Show(
                    "Type de mouvement incorrect.",
                    "Erreur",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                return;
        }

        if (typeBase == "PERTE" &&
            string.IsNullOrWhiteSpace(txtMotif.Text))
        {
            MessageBox.Show(
                "Veuillez indiquer le motif de la perte.",
                "Motif obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtMotif.Focus();
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
                const string sqlStock = """
                    SELECT quantite_stock
                    FROM emballages
                    WHERE id = @id
                    FOR UPDATE;
                    """;

                using MySqlCommand stockCommand =
                    new MySqlCommand(
                        sqlStock,
                        connection,
                        transaction
                    );

                stockCommand.Parameters.AddWithValue(
                    "@id",
                    emballageId.Value
                );

                object? resultat =
                    stockCommand.ExecuteScalar();

                if (resultat == null)
                {
                    throw new InvalidOperationException(
                        "L’emballage n’existe plus."
                    );
                }

                int ancienStock =
                    Convert.ToInt32(resultat);

                int nouveauStock =
                    ancienStock + variation;

                if (nouveauStock < 0)
                {
                    MessageBox.Show(
                        "Stock insuffisant.\n\n" +
                        $"Stock actuel : {ancienStock:N0}\n" +
                        $"Quantité demandée : {quantite:N0}",
                        "Mouvement impossible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    transaction.Rollback();
                    return;
                }

                const string sqlUpdate = """
                    UPDATE emballages
                    SET quantite_stock = @nouveauStock
                    WHERE id = @id;
                    """;

                using MySqlCommand updateCommand =
                    new MySqlCommand(
                        sqlUpdate,
                        connection,
                        transaction
                    );

                updateCommand.Parameters.AddWithValue(
                    "@nouveauStock",
                    nouveauStock
                );

                updateCommand.Parameters.AddWithValue(
                    "@id",
                    emballageId.Value
                );

                updateCommand.ExecuteNonQuery();

                const string sqlMouvement = """
                    INSERT INTO mouvements_stock_emballages
                    (
                        emballage_id,
                        type_mouvement,
                        quantite,
                        ancien_stock,
                        nouveau_stock,
                        motif
                    )
                    VALUES
                    (
                        @emballageId,
                        @typeMouvement,
                        @quantite,
                        @ancienStock,
                        @nouveauStock,
                        @motif
                    );
                    """;

                using MySqlCommand mouvementCommand =
                    new MySqlCommand(
                        sqlMouvement,
                        connection,
                        transaction
                    );

                mouvementCommand.Parameters.AddWithValue(
                    "@emballageId",
                    emballageId.Value
                );

                mouvementCommand.Parameters.AddWithValue(
                    "@typeMouvement",
                    typeBase
                );

                mouvementCommand.Parameters.AddWithValue(
                    "@quantite",
                    quantite
                );

                mouvementCommand.Parameters.AddWithValue(
                    "@ancienStock",
                    ancienStock
                );

                mouvementCommand.Parameters.AddWithValue(
                    "@nouveauStock",
                    nouveauStock
                );

                mouvementCommand.Parameters.AddWithValue(
                    "@motif",
                    string.IsNullOrWhiteSpace(txtMotif.Text)
                        ? DBNull.Value
                        : txtMotif.Text.Trim()
                );

                mouvementCommand.ExecuteNonQuery();

                transaction.Commit();

                MessageBox.Show(
                    "Le mouvement a été enregistré.\n\n" +
                    $"Ancien stock : {ancienStock:N0}\n" +
                    $"Nouveau stock : {nouveauStock:N0}",
                    "Stock mis à jour",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                numQuantite.Value = 1;
                txtMotif.Clear();

                ChargerEmballages(emballageId.Value);
                ChargerHistorique();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible d’enregistrer le mouvement.\n\n" +
                ex.Message,
                "Erreur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ChargerHistorique()
    {
        if (dgvMouvements == null)
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            string sql = """
                SELECT
                    m.id,
                    e.nom AS emballage,

                    CASE m.type_mouvement
                        WHEN 'ENTREE_ACHAT'
                            THEN 'Achat / entrée'
                        WHEN 'SORTIE_UTILISATION'
                            THEN 'Utilisation production'
                        WHEN 'PERTE'
                            THEN 'Perte / abîmé'
                        WHEN 'AJUSTEMENT_ENTREE'
                            THEN 'Ajustement entrée'
                        WHEN 'AJUSTEMENT_SORTIE'
                            THEN 'Ajustement sortie'
                        ELSE m.type_mouvement
                    END AS type_mouvement,

                    m.quantite,
                    m.ancien_stock,
                    m.nouveau_stock,
                    m.motif,
                    m.date_mouvement

                FROM mouvements_stock_emballages m

                INNER JOIN emballages e
                    ON e.id = m.emballage_id
                """;

            int? emballageId =
                ObtenirEmballageSelectionneId();

            if (emballageId.HasValue)
            {
                sql += """
                    
                    WHERE m.emballage_id = @emballageId
                    """;
            }

            sql += """
                
                ORDER BY
                    m.date_mouvement DESC,
                    m.id DESC;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            if (emballageId.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@emballageId",
                    emballageId.Value
                );
            }
            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(command);

            var table = new DataTable();
            adapter.Fill(table);

            dgvMouvements.DataSource = table;

            if (dgvMouvements.Columns["id"]
                is DataGridViewColumn colonneId)
            {
                colonneId.Visible = false;
            }

            if (dgvMouvements.Columns["emballage"]
                is DataGridViewColumn colonneEmballage)
            {
                colonneEmballage.HeaderText = "Emballage";
            }

            if (dgvMouvements.Columns["type_mouvement"]
                is DataGridViewColumn colonneType)
            {
                colonneType.HeaderText = "Type";
            }

            if (dgvMouvements.Columns["quantite"]
                is DataGridViewColumn colonneQuantite)
            {
                colonneQuantite.HeaderText = "Quantité";
                colonneQuantite.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["ancien_stock"]
                is DataGridViewColumn colonneAncienStock)
            {
                colonneAncienStock.HeaderText = "Ancien stock";
                colonneAncienStock.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["nouveau_stock"]
                is DataGridViewColumn colonneNouveauStock)
            {
                colonneNouveauStock.HeaderText = "Nouveau stock";
                colonneNouveauStock.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvMouvements.Columns["motif"]
                is DataGridViewColumn colonneMotif)
            {
                colonneMotif.HeaderText = "Motif";
            }

            if (dgvMouvements.Columns["date_mouvement"]
                is DataGridViewColumn colonneDate)
            {
                colonneDate.HeaderText = "Date";
                colonneDate.DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";
            }

            dgvMouvements.ClearSelection();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger l’historique.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ColorerStocksFaibles()
    {
        foreach (
            DataGridViewRow ligne
            in dgvEmballages.Rows)
        {
            int stock = Convert.ToInt32(
                ligne.Cells["quantite_stock"].Value
            );

            int seuil = Convert.ToInt32(
                ligne.Cells["seuil_alerte"].Value
            );

            if (stock <= seuil)
            {
                ligne.DefaultCellStyle.BackColor =
                    Color.FromArgb(255, 235, 235);

                ligne.DefaultCellStyle.ForeColor =
                    Color.FromArgb(150, 30, 30);
            }
        }
    }
}