using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace GestionUsine;

public sealed class StockProduitsControl : UserControl
{
    private readonly ComboBox cmbProduits;
    private readonly ComboBox cmbTypeMouvement;
    private readonly NumericUpDown numQuantite;
    private readonly TextBox txtMotif;
    private readonly Label lblStockActuel;
    private readonly DataGridView dgvMouvements;

    public StockProduitsControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 260)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 280)
        );

        Controls.Add(structure);

        var lblTitre = new Label
        {
            Text = "Gestion du stock des produits finis",
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
            CreerLabel("Produit"),
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

        cmbProduits = new ComboBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 11),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Margin = new Padding(5)
        };

        cmbProduits.SelectionChangeCommitted += (_, _) =>
        {
            AfficherStockActuel();
            ChargerHistorique();
        };

        formulaire.Controls.Add(cmbProduits, 0, 1);

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
                "Production",
                "Perte"
            }
        );

        cmbTypeMouvement.SelectedIndex = 0;

        formulaire.Controls.Add(
            cmbTypeMouvement,
            1,
            1
        );

        numQuantite = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 1,
            Maximum = 100000000,
            ThousandsSeparator = true,
            Value = 1,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(numQuantite, 2, 1);

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
            220
        );

        var btnActualiser = CreerBouton(
            "Actualiser",
            Color.FromArgb(32, 113, 171),
            140
        );

        btnEnregistrer.Click += EnregistrerMouvement;

        btnActualiser.Click += (_, _) =>
        {
            ChargerProduits();
            ChargerHistorique();
        };

        panneauBoutons.Controls.Add(btnEnregistrer);
        panneauBoutons.Controls.Add(btnActualiser);

        dgvMouvements = new DataGridView
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
            Margin = new Padding(0, 15, 0, 0)
        };

        dgvMouvements.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        dgvMouvements.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(25, 49, 78);

        dgvMouvements.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        dgvMouvements.EnableHeadersVisualStyles = false;

        structure.Controls.Add(dgvMouvements, 0, 2);

        ChargerProduits();
        ChargerHistorique();
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

    private void ChargerProduits()
    {
        try
        {
            int? ancienProduitId = null;

            if (cmbProduits.SelectedValue != null &&
                cmbProduits.SelectedValue is not DataRowView)
            {
                ancienProduitId =
                    Convert.ToInt32(
                        cmbProduits.SelectedValue
                    );
            }

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
                    ) AS produit
                FROM produits
                WHERE actif = TRUE
                ORDER BY poids_grammes, nom;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            cmbProduits.DisplayMember = "produit";
            cmbProduits.ValueMember = "id";
            cmbProduits.DataSource = table;

            if (ancienProduitId.HasValue)
            {
                cmbProduits.SelectedValue =
                    ancienProduitId.Value;
            }

            if (cmbProduits.Items.Count > 0 &&
                cmbProduits.SelectedIndex < 0)
            {
                cmbProduits.SelectedIndex = 0;
            }

            AfficherStockActuel();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger les produits.\n\n" +
                ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private int? ObtenirProduitSelectionneId()
    {
        if (cmbProduits.SelectedValue == null ||
            cmbProduits.SelectedValue is DataRowView)
        {
            return null;
        }

        return Convert.ToInt32(
            cmbProduits.SelectedValue
        );
    }

    private void AfficherStockActuel()
    {
        int? produitId =
            ObtenirProduitSelectionneId();

        if (!produitId.HasValue)
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
                SELECT quantite_stock
                FROM produits
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                produitId.Value
            );

            object? resultat =
                command.ExecuteScalar();

            int stock = resultat == null
                ? 0
                : Convert.ToInt32(resultat);

            lblStockActuel.Text =
                $"{stock:N0} unité(s)";
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
        int? produitId =
            ObtenirProduitSelectionneId();

        if (!produitId.HasValue)
        {
            MessageBox.Show(
                "Veuillez sélectionner un produit.",
                "Produit obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        int quantite =
            Convert.ToInt32(numQuantite.Value);

        string typeAffiche =
            cmbTypeMouvement.Text;

        string typeBase;
        int variation;

        switch (typeAffiche)
        {
            case "Production":
                typeBase = "ENTREE_PRODUCTION";
                variation = quantite;
                break;

            case "Perte":
                typeBase = "PERTE";
                variation = -quantite;

                if (string.IsNullOrWhiteSpace(txtMotif.Text))
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
                    FROM produits
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
                    produitId.Value
                );

                object? resultat =
                    stockCommand.ExecuteScalar();

                if (resultat == null)
                {
                    throw new InvalidOperationException(
                        "Le produit sélectionné n’existe plus."
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
                        $"Stock actuel : {ancienStock}\n" +
                        $"Quantité demandée : {quantite}",
                        "Mouvement impossible",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );

                    transaction.Rollback();
                    return;
                }

                const string sqlUpdate = """
                    UPDATE produits
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
                    produitId.Value
                );

                updateCommand.ExecuteNonQuery();

                const string sqlMouvement = """
                    INSERT INTO mouvements_stock_produits
                    (
                        produit_id,
                        type_mouvement,
                        quantite,
                        ancien_stock,
                        nouveau_stock,
                        motif
                    )
                    VALUES
                    (
                        @produitId,
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
                    "@produitId",
                    produitId.Value
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
                    "Le mouvement de stock a été enregistré.\n\n" +
                    $"Ancien stock : {ancienStock}\n" +
                    $"Nouveau stock : {nouveauStock}",
                    "Stock mis à jour",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                numQuantite.Value = 1;
                txtMotif.Clear();

                AfficherStockActuel();
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
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            string sql = """
                SELECT
                    m.id,
                    CONCAT(
                        p.nom,
                        ' - ',
                        p.poids_grammes,
                        ' g'
                    ) AS produit,

                    CASE m.type_mouvement
                        WHEN 'ENTREE_PRODUCTION'
                            THEN 'Production'
                        WHEN 'PERTE'
                            THEN 'Perte'
                        WHEN 'SORTIE_VENTE'
                            THEN 'Sortie vente'
                        ELSE m.type_mouvement
                    END AS type_mouvement,

                    m.quantite,
                    m.ancien_stock,
                    m.nouveau_stock,
                    m.motif,
                    m.date_mouvement

                FROM mouvements_stock_produits m

                INNER JOIN produits p
                    ON p.id = m.produit_id
                """;

            int? produitId =
                ObtenirProduitSelectionneId();

            if (produitId.HasValue)
            {
                sql += """
                    
                    WHERE m.produit_id = @produitId
                    """;
            }

            sql += """
                
                ORDER BY m.date_mouvement DESC,
                         m.id DESC;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            if (produitId.HasValue)
            {
                command.Parameters.AddWithValue(
                    "@produitId",
                    produitId.Value
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

            if (dgvMouvements.Columns["produit"]
                is DataGridViewColumn colonneProduit)
            {
                colonneProduit.HeaderText = "Produit";
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
}