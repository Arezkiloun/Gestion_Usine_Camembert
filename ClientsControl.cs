using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace GestionUsine;

public sealed class ClientsControl : UserControl
{
    private readonly TextBox txtNom;
    private readonly TextBox txtTelephone;
    private readonly TextBox txtAdresse;
    private readonly TextBox txtRecherche;
    private readonly DataGridView dgvClients;
    private readonly System.Windows.Forms.Timer timerRecherche;

    private int clientSelectionneId;

    public ClientsControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        timerRecherche =
            new System.Windows.Forms.Timer
            {
                Interval = 350
            };

        timerRecherche.Tick += (_, _) =>
        {
            timerRecherche.Stop();
            ChargerClients();
        };

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 250)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 72)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        Controls.Add(structure);

        var lblTitre = new Label
        {
            Text = "Gestion des clients",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(lblTitre, 0, 0);

        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20),
            BorderStyle = BorderStyle.FixedSingle
        };

        structure.Controls.Add(carte, 0, 1);

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 4
        };

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 33.33F)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 33.33F)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 33.34F)
        );

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
            CreerLabel("Nom du client"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Téléphone"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Adresse"),
            2,
            0
        );

        txtNom = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Margin = new Padding(5)
        };

        txtTelephone = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Margin = new Padding(5)
        };

        txtAdresse = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(txtNom, 0, 1);
        formulaire.Controls.Add(txtTelephone, 1, 1);
        formulaire.Controls.Add(txtAdresse, 2, 1);

        var lblInformation = new Label
        {
            Text = "Le nom est obligatoire. Le téléphone et l’adresse sont facultatifs.",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9),
            ForeColor = Color.DimGray,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(lblInformation, 0, 2);
        formulaire.SetColumnSpan(lblInformation, 3);

        var panneauBoutons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };

        formulaire.Controls.Add(panneauBoutons, 0, 3);
        formulaire.SetColumnSpan(panneauBoutons, 3);

        var btnAjouter = CreerBouton(
            "Ajouter",
            Color.FromArgb(38, 138, 83)
        );

        var btnModifier = CreerBouton(
            "Modifier",
            Color.FromArgb(32, 113, 171)
        );

        var btnSupprimer = CreerBouton(
            "Supprimer",
            Color.FromArgb(190, 58, 58)
        );

        var btnNouveau = CreerBouton(
            "Nouveau",
            Color.FromArgb(95, 105, 115)
        );

        var btnTarifs = CreerBouton(
            "Tarifs spéciaux",
            Color.FromArgb(93, 63, 137)
        );

        btnTarifs.Width = 170;

        btnAjouter.Click += AjouterClient;
        btnModifier.Click += ModifierClient;
        btnSupprimer.Click += SupprimerClient;
        btnNouveau.Click += (_, _) => ViderFormulaire();
        btnTarifs.Click += OuvrirTarifsSpeciaux;

        panneauBoutons.Controls.Add(btnAjouter);
        panneauBoutons.Controls.Add(btnModifier);
        panneauBoutons.Controls.Add(btnSupprimer);
        panneauBoutons.Controls.Add(btnNouveau);
        panneauBoutons.Controls.Add(btnTarifs);

        var panneauRecherche =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.White,
                Padding = new Padding(15, 10, 15, 10),
                Margin = Padding.Empty
            };

        panneauRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                115
            )
        );

        panneauRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                100
            )
        );

        panneauRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                150
            )
        );

        panneauRecherche.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        structure.Controls.Add(
            panneauRecherche,
            0,
            2
        );

        var lblRecherche = new Label
        {
            Text = "Rechercher :",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Margin = new Padding(5)
        };

        txtRecherche = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                11
            ),
            PlaceholderText =
                "Nom, téléphone ou adresse",
            Margin = new Padding(5, 7, 5, 7)
        };

        txtRecherche.TextChanged += (_, _) =>
        {
            timerRecherche.Stop();
            timerRecherche.Start();
        };

        txtRecherche.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                timerRecherche.Stop();
                ChargerClients();
                e.SuppressKeyPress = true;
            }
        };

        var btnActualiser = new Button
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
            Margin = new Padding(5)
        };

        btnActualiser.FlatAppearance.BorderSize = 0;

        btnActualiser.Click += (_, _) =>
        {
            timerRecherche.Stop();
            txtRecherche.Clear();
            ChargerClients();
            txtRecherche.Focus();
        };

        panneauRecherche.Controls.Add(
            lblRecherche,
            0,
            0
        );

        panneauRecherche.Controls.Add(
            txtRecherche,
            1,
            0
        );

        panneauRecherche.Controls.Add(
            btnActualiser,
            2,
            0
        );

        dgvClients = new DataGridView
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

        dgvClients.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        dgvClients.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(25, 49, 78);

        dgvClients.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        dgvClients.EnableHeadersVisualStyles = false;
        dgvClients.CellClick += SelectionnerClient;

        structure.Controls.Add(dgvClients, 0, 3);

        ChargerClients();
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
        Color couleur)
    {
        var bouton = new Button
        {
            Text = texte,
            Width = 140,
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


    private void OuvrirTarifsSpeciaux(
        object? sender,
        EventArgs e)
    {
        if (clientSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez d’abord un client dans le tableau.",
                "Client obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        string nomClient =
            txtNom.Text.Trim();

        using var fenetre =
            new TarifsClientForm(
                clientSelectionneId,
                nomClient
            );

        fenetre.ShowDialog(this);
    }

    private bool ValiderFormulaire()
    {
        if (string.IsNullOrWhiteSpace(txtNom.Text))
        {
            MessageBox.Show(
                "Veuillez saisir le nom du client.",
                "Champ obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtNom.Focus();
            return false;
        }

        return true;
    }

    private void AjouterClient(
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
                INSERT INTO clients
                (
                    nom,
                    telephone,
                    adresse
                )
                VALUES
                (
                    @nom,
                    @telephone,
                    @adresse
                );
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nom",
                txtNom.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@telephone",
                string.IsNullOrWhiteSpace(txtTelephone.Text)
                    ? DBNull.Value
                    : txtTelephone.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@adresse",
                string.IsNullOrWhiteSpace(txtAdresse.Text)
                    ? DBNull.Value
                    : txtAdresse.Text.Trim()
            );

            command.ExecuteNonQuery();

            int nouvelId =
                Convert.ToInt32(command.LastInsertedId);

            MessageBox.Show(
                "Le client a été ajouté.",
                "Ajout réussi",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerClients(nouvelId);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible d’ajouter le client.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ModifierClient(
        object? sender,
        EventArgs e)
    {
        if (clientSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un client dans le tableau.",
                "Client non sélectionné",
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
                UPDATE clients
                SET
                    nom = @nom,
                    telephone = @telephone,
                    adresse = @adresse
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@nom",
                txtNom.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@telephone",
                string.IsNullOrWhiteSpace(txtTelephone.Text)
                    ? DBNull.Value
                    : txtTelephone.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@adresse",
                string.IsNullOrWhiteSpace(txtAdresse.Text)
                    ? DBNull.Value
                    : txtAdresse.Text.Trim()
            );

            command.Parameters.AddWithValue(
                "@id",
                clientSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le client a été modifié.",
                "Modification réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerClients(clientSelectionneId);
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de modifier le client.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SupprimerClient(
        object? sender,
        EventArgs e)
    {
        if (clientSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un client dans le tableau.",
                "Client non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        DialogResult reponse = MessageBox.Show(
            "Voulez-vous supprimer ce client ?",
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
                DELETE FROM clients
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                clientSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le client a été supprimé.",
                "Suppression réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ViderFormulaire();
            ChargerClients();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de supprimer ce client.\n\n" +
                "Il est peut-être déjà utilisé dans une vente.\n\n" +
                ex.Message,
                "Suppression impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SelectionnerClient(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        DataGridViewRow ligne =
            dgvClients.Rows[e.RowIndex];

        clientSelectionneId =
            Convert.ToInt32(ligne.Cells["id"].Value);

        txtNom.Text =
            Convert.ToString(ligne.Cells["nom"].Value) ?? "";

        txtTelephone.Text =
            Convert.ToString(
                ligne.Cells["telephone"].Value
            ) ?? "";

        txtAdresse.Text =
            Convert.ToString(
                ligne.Cells["adresse"].Value
            ) ?? "";
    }

    private void ChargerClients(
        int? clientIdASelectionner = null)
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    id,
                    nom,
                    telephone,
                    adresse,
                    date_creation
                FROM clients
                WHERE actif = TRUE
                  AND
                  (
                      @recherche = ''
                      OR nom LIKE CONCAT('%', @recherche, '%')
                      OR telephone LIKE CONCAT('%', @recherche, '%')
                      OR adresse LIKE CONCAT('%', @recherche, '%')
                  )
                ORDER BY nom;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@recherche",
                txtRecherche?.Text.Trim() ?? ""
            );

            using MySqlDataAdapter adapter =
      new MySqlDataAdapter(command);

            var table = new DataTable();
            adapter.Fill(table);

            dgvClients.DataSource = table;

            if (dgvClients.Columns["id"] is DataGridViewColumn colonneId)
            {
                colonneId.Visible = false;
            }

            if (dgvClients.Columns["nom"] is DataGridViewColumn colonneNom)
            {
                colonneNom.HeaderText = "Nom du client";
            }

            if (dgvClients.Columns["telephone"] is DataGridViewColumn colonneTelephone)
            {
                colonneTelephone.HeaderText = "Téléphone";
            }

            if (dgvClients.Columns["adresse"] is DataGridViewColumn colonneAdresse)
            {
                colonneAdresse.HeaderText = "Adresse";
            }

            if (dgvClients.Columns["date_creation"] is DataGridViewColumn colonneDate)
            {
                colonneDate.HeaderText = "Date de création";
                colonneDate.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            }

            dgvClients.ClearSelection();

            if (!clientIdASelectionner.HasValue)
            {
                return;
            }

            foreach (
                DataGridViewRow ligne
                in dgvClients.Rows)
            {
                int id = Convert.ToInt32(
                    ligne.Cells["id"].Value
                );

                if (id != clientIdASelectionner.Value)
                {
                    continue;
                }

                ligne.Selected = true;

                dgvClients.CurrentCell =
                    ligne.Cells["nom"];

                clientSelectionneId = id;

                txtNom.Text =
                    Convert.ToString(
                        ligne.Cells["nom"].Value
                    ) ?? "";

                txtTelephone.Text =
                    Convert.ToString(
                        ligne.Cells["telephone"].Value
                    ) ?? "";

                txtAdresse.Text =
                    Convert.ToString(
                        ligne.Cells["adresse"].Value
                    ) ?? "";

                dgvClients.FirstDisplayedScrollingRowIndex =
                    ligne.Index;

                break;
            }
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger les clients.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ViderFormulaire()
    {
        clientSelectionneId = 0;

        txtNom.Clear();
        txtTelephone.Clear();
        txtAdresse.Clear();

        dgvClients.ClearSelection();
        txtNom.Focus();
    }
}