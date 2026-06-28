using MySqlConnector;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace GestionUsine;

public sealed class TarifsClientForm : Form
{
    private readonly int clientId;
    private readonly string nomClient;

    private readonly ComboBox cmbProduit;
    private readonly Label lblPrixNormal;
    private readonly Label lblEtatTarif;
    private readonly NumericUpDown numPrixSpecial;
    private readonly DataGridView dgvTarifs;

    public TarifsClientForm(
        int clientId,
        string nomClient)
    {
        this.clientId = clientId;
        this.nomClient = nomClient;

        Text =
            $"Tarifs spéciaux - {nomClient}";

        StartPosition =
            FormStartPosition.CenterParent;

        Size = new Size(1000, 650);
        MinimumSize = new Size(850, 560);

        BackColor =
            Color.FromArgb(242, 245, 248);

        Font = new Font("Segoe UI", 10);
        AutoScaleMode = AutoScaleMode.Dpi;

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(15),
            Margin = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                72
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                170
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        structure.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                62
            )
        );

        Controls.Add(structure);

        var entete = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding =
                new Padding(18, 8, 18, 8),
            Margin =
                new Padding(0, 0, 0, 10)
        };

        var lblTitre = new Label
        {
            Text =
                "Tarifs spéciaux du client",
            Dock = DockStyle.Top,
            Height = 32,
            Font = new Font(
                "Segoe UI",
                16,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        var lblClient = new Label
        {
            Text = $"Client : {nomClient}",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(80, 90, 100),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        entete.Controls.Add(lblClient);
        entete.Controls.Add(lblTitre);

        structure.Controls.Add(
            entete,
            0,
            0
        );

        var carteSaisie = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding = new Padding(18),
            Margin =
                new Padding(0, 0, 0, 10)
        };

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        formulaire.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                45
            )
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                25
            )
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                30
            )
        );

        formulaire.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                35
            )
        );

        formulaire.RowStyles.Add(
            new RowStyle(
                SizeType.Absolute,
                52
            )
        );

        formulaire.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        carteSaisie.Controls.Add(formulaire);

        formulaire.Controls.Add(
            CreerLabel("Produit"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Prix normal"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Prix spécial du client"),
            2,
            0
        );

        cmbProduit = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        cmbProduit.SelectedIndexChanged +=
            (_, _) =>
            {
                AfficherProduitSelectionne();
            };

        lblPrixNormal = new Label
        {
            Text = "0,00 DA",
            Dock = DockStyle.Fill,
            BackColor =
                Color.FromArgb(238, 244, 250),
            BorderStyle =
                BorderStyle.FixedSingle,
            Font = new Font(
                "Segoe UI",
                12,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin = new Padding(5)
        };

        numPrixSpecial = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 100_000_000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            Font = new Font(
                "Segoe UI",
                12,
                FontStyle.Bold
            ),
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(
            cmbProduit,
            0,
            1
        );

        formulaire.Controls.Add(
            lblPrixNormal,
            1,
            1
        );

        formulaire.Controls.Add(
            numPrixSpecial,
            2,
            1
        );

        lblEtatTarif = new Label
        {
            Text =
                "Sélectionnez un produit.",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding = new Padding(5, 4, 5, 0)
        };

        formulaire.Controls.Add(
            lblEtatTarif,
            0,
            2
        );

        formulaire.SetColumnSpan(
            lblEtatTarif,
            3
        );

        structure.Controls.Add(
            carteSaisie,
            0,
            1
        );

        dgvTarifs = CreerTableau();

        structure.Controls.Add(
            dgvTarifs,
            0,
            2
        );

        var panneauBoutons =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding =
                    new Padding(0, 8, 0, 0)
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

        var btnEnregistrer =
            CreerBouton(
                "Enregistrer / Modifier",
                Color.FromArgb(38, 138, 83)
            );

        var btnDesactiver =
            CreerBouton(
                "Désactiver le tarif",
                Color.FromArgb(190, 58, 58)
            );

        var btnActualiser =
            CreerBouton(
                "Actualiser",
                Color.FromArgb(32, 113, 171)
            );

        var btnFermer =
            CreerBouton(
                "Fermer",
                Color.FromArgb(95, 105, 115)
            );

        btnEnregistrer.Click +=
            EnregistrerTarif;

        btnDesactiver.Click +=
            DesactiverTarif;

        btnActualiser.Click += (_, _) =>
        {
            ChargerProduits();
            ChargerTarifs();
        };

        btnFermer.Click += (_, _) =>
        {
            Close();
        };

        panneauBoutons.Controls.Add(
            btnEnregistrer,
            0,
            0
        );

        panneauBoutons.Controls.Add(
            btnDesactiver,
            1,
            0
        );

        panneauBoutons.Controls.Add(
            btnActualiser,
            2,
            0
        );

        panneauBoutons.Controls.Add(
            btnFermer,
            3,
            0
        );

        structure.Controls.Add(
            panneauBoutons,
            0,
            3
        );

        ChargerProduits();
        ChargerTarifs();
    }

    private DataGridView CreerTableau()
    {
        var tableau = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
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
            ColumnHeadersHeight = 44,
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
            Color.FromArgb(25, 49, 78);

        tableau.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        tableau.EnableHeadersVisualStyles = false;

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "produit_id",
                DataPropertyName = "produit_id",
                Visible = false
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "produit",
                DataPropertyName = "produit",
                HeaderText = "Produit",
                AutoSizeMode =
                    DataGridViewAutoSizeColumnMode.Fill
            }
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "prix_normal",
                DataPropertyName = "prix_normal",
                HeaderText = "Prix normal",
                Width = 170,
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
                Name = "prix_special",
                DataPropertyName = "prix_special",
                HeaderText = "Prix spécial",
                Width = 170,
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
                Name = "statut",
                DataPropertyName = "statut",
                HeaderText = "Statut",
                Width = 120
            }
        );

        tableau.CellClick += (_, e) =>
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow ligne =
                tableau.Rows[e.RowIndex];

            int produitId =
                Convert.ToInt32(
                    ligne.Cells["produit_id"].Value
                );

            cmbProduit.SelectedValue =
                produitId;

            numPrixSpecial.Value =
                Math.Min(
                    Convert.ToDecimal(
                        ligne.Cells["prix_special"].Value
                    ),
                    numPrixSpecial.Maximum
                );
        };

        return tableau;
    }

    private void ChargerProduits()
    {
        try
        {
            int? ancienProduitId =
                ObtenirProduitSelectionneId();

            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    id,
                    CONCAT(
                        nom,
                        ' - ',
                        poids_grammes,
                        ' g'
                    ) AS produit,
                    prix_vente
                FROM produits
                WHERE actif = TRUE
                ORDER BY
                    poids_grammes,
                    nom;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(
                    sql,
                    connection
                );

            var table = new DataTable();
            adapter.Fill(table);

            cmbProduit.DisplayMember =
                "produit";

            cmbProduit.ValueMember =
                "id";

            cmbProduit.DataSource =
                table;

            if (ancienProduitId.HasValue)
            {
                cmbProduit.SelectedValue =
                    ancienProduitId.Value;
            }

            if (
                cmbProduit.Items.Count > 0 &&
                cmbProduit.SelectedIndex < 0)
            {
                cmbProduit.SelectedIndex = 0;
            }

            AfficherProduitSelectionne();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger les produits.",
                ex
            );
        }
    }

    private void AfficherProduitSelectionne()
    {
        if (
            cmbProduit.SelectedItem
            is not DataRowView produit)
        {
            lblPrixNormal.Text =
                "0,00 DA";

            numPrixSpecial.Value = 0;

            lblEtatTarif.Text =
                "Sélectionnez un produit.";

            return;
        }

        decimal prixNormal =
            Convert.ToDecimal(
                produit["prix_vente"]
            );

        lblPrixNormal.Text =
            $"{prixNormal:N2} DA";

        int? produitId =
            ObtenirProduitSelectionneId();

        if (!produitId.HasValue)
        {
            numPrixSpecial.Value =
                Math.Min(
                    prixNormal,
                    numPrixSpecial.Maximum
                );

            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    prix_special,
                    actif
                FROM tarifs_clients
                WHERE client_id = @clientId
                  AND produit_id = @produitId
                LIMIT 1;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@clientId",
                clientId
            );

            command.Parameters.AddWithValue(
                "@produitId",
                produitId.Value
            );

            using MySqlDataReader reader =
                command.ExecuteReader();

            if (!reader.Read())
            {
                numPrixSpecial.Value =
                    Math.Min(
                        prixNormal,
                        numPrixSpecial.Maximum
                    );

                lblEtatTarif.Text =
                    "Aucun tarif spécial enregistré. " +
                    "Le prix normal est proposé.";

                lblEtatTarif.ForeColor =
                    Color.FromArgb(95, 105, 115);

                return;
            }

            decimal prixSpecial =
                reader.GetDecimal(
                    "prix_special"
                );

            bool actif =
                reader.GetBoolean("actif");

            numPrixSpecial.Value =
                Math.Min(
                    prixSpecial,
                    numPrixSpecial.Maximum
                );

            lblEtatTarif.Text =
                actif
                    ? "Tarif spécial actif."
                    : "Tarif désactivé. Cliquez sur " +
                      "Enregistrer / Modifier pour le réactiver.";

            lblEtatTarif.ForeColor =
                actif
                    ? Color.FromArgb(38, 138, 83)
                    : Color.FromArgb(190, 58, 58);
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de lire le tarif spécial.",
                ex
            );
        }
    }

    private void ChargerTarifs()
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    tc.produit_id,

                    CONCAT(
                        p.nom,
                        ' - ',
                        p.poids_grammes,
                        ' g'
                    ) AS produit,

                    p.prix_vente
                        AS prix_normal,

                    tc.prix_special,

                    CASE
                        WHEN tc.actif = TRUE
                            THEN 'Actif'
                        ELSE 'Désactivé'
                    END AS statut

                FROM tarifs_clients tc

                INNER JOIN produits p
                    ON p.id = tc.produit_id

                WHERE tc.client_id = @clientId

                ORDER BY
                    p.poids_grammes,
                    p.nom;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@clientId",
                clientId
            );

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(
                    command
                );

            var table = new DataTable();
            adapter.Fill(table);

            dgvTarifs.DataSource = table;
            dgvTarifs.ClearSelection();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger les tarifs spéciaux.",
                ex
            );
        }
    }

    private void EnregistrerTarif(
        object? sender,
        EventArgs e)
    {
        int? produitId =
            ObtenirProduitSelectionneId();

        if (!produitId.HasValue)
        {
            AfficherAvertissement(
                "Sélectionnez un produit."
            );

            return;
        }

        decimal prixSpecial =
            numPrixSpecial.Value;

        if (prixSpecial <= 0)
        {
            AfficherAvertissement(
                "Le prix spécial doit être supérieur à zéro."
            );

            numPrixSpecial.Focus();
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                INSERT INTO tarifs_clients
                (
                    client_id,
                    produit_id,
                    prix_special,
                    actif
                )
                VALUES
                (
                    @clientId,
                    @produitId,
                    @prixSpecial,
                    TRUE
                )

                ON DUPLICATE KEY UPDATE
                    prix_special =
                        VALUES(prix_special),
                    actif = TRUE,
                    date_modification =
                        CURRENT_TIMESTAMP;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@clientId",
                clientId
            );

            command.Parameters.AddWithValue(
                "@produitId",
                produitId.Value
            );

            command.Parameters.AddWithValue(
                "@prixSpecial",
                prixSpecial
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le tarif spécial a été enregistré.\n\n" +
                $"Client : {nomClient}\n" +
                $"Produit : {cmbProduit.Text}\n" +
                $"Prix spécial : {prixSpecial:N2} DA",
                "Tarif enregistré",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerTarifs();
            AfficherProduitSelectionne();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible d’enregistrer le tarif spécial.",
                ex
            );
        }
    }

    private void DesactiverTarif(
        object? sender,
        EventArgs e)
    {
        int? produitId =
            ObtenirProduitSelectionneId();

        if (!produitId.HasValue)
        {
            AfficherAvertissement(
                "Sélectionnez un produit."
            );

            return;
        }

        DialogResult confirmation =
            MessageBox.Show(
                "Voulez-vous désactiver le tarif spécial " +
                "de ce produit ?\n\n" +
                $"Client : {nomClient}\n" +
                $"Produit : {cmbProduit.Text}\n\n" +
                "Le prix normal sera utilisé dans les ventes.",
                "Confirmation",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

        if (
            confirmation !=
            DialogResult.Yes)
        {
            return;
        }

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                UPDATE tarifs_clients
                SET
                    actif = FALSE,
                    date_modification =
                        CURRENT_TIMESTAMP
                WHERE client_id = @clientId
                  AND produit_id = @produitId;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@clientId",
                clientId
            );

            command.Parameters.AddWithValue(
                "@produitId",
                produitId.Value
            );

            int lignes =
                command.ExecuteNonQuery();

            if (lignes == 0)
            {
                AfficherAvertissement(
                    "Aucun tarif spécial n’est enregistré " +
                    "pour ce produit."
                );

                return;
            }

            ChargerTarifs();
            AfficherProduitSelectionne();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de désactiver le tarif spécial.",
                ex
            );
        }
    }

    private int? ObtenirProduitSelectionneId()
    {
        if (
            cmbProduit.SelectedValue == null ||
            cmbProduit.SelectedValue
                is DataRowView)
        {
            return null;
        }

        return Convert.ToInt32(
            cmbProduit.SelectedValue
        );
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
                Color.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.BottomLeft,
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
            Dock = DockStyle.Fill,
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
