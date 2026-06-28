using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace GestionUsine;

public sealed class ProduitsControl : UserControl
{
    private readonly TextBox txtNom;
    private readonly NumericUpDown numPoids;
    private readonly NumericUpDown numPrix;
    private readonly NumericUpDown numStock;
    private readonly NumericUpDown numSeuil;
    private readonly DataGridView dgvProduits;

    private int produitSelectionneId;

    public ProduitsControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.Transparent
        };

        structure.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));
        structure.RowStyles.Add(new RowStyle(SizeType.Absolute, 250));
        structure.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Controls.Add(structure);

        var lblTitre = new Label
        {
            Text = "Gestion des produits",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 22, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(lblTitre, 0, 0);

        var panneauFormulaire = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(20),
            BorderStyle = BorderStyle.FixedSingle
        };

        structure.Controls.Add(panneauFormulaire, 0, 1);

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 4
        };

        formulaire.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        formulaire.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        formulaire.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        formulaire.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

        formulaire.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));
        formulaire.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        formulaire.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        formulaire.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        panneauFormulaire.Controls.Add(formulaire);

        formulaire.Controls.Add(CreerLabel("Nom du produit"), 0, 0);
        formulaire.Controls.Add(CreerLabel("Poids en grammes"), 1, 0);
        formulaire.Controls.Add(CreerLabel("Prix de vente"), 2, 0);
        formulaire.Controls.Add(CreerLabel("Quantité en stock"), 3, 0);

        txtNom = new TextBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Margin = new Padding(5)
        };

        numPoids = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 1,
            Maximum = 100000,
            Margin = new Padding(5)
        };

        numPrix = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = 100000000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            Margin = new Padding(5)
        };

        numStock = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = 100000000,
            ThousandsSeparator = true,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(txtNom, 0, 1);
        formulaire.Controls.Add(numPoids, 1, 1);
        formulaire.Controls.Add(numPrix, 2, 1);
        formulaire.Controls.Add(numStock, 3, 1);

        formulaire.Controls.Add(CreerLabel("Seuil d’alerte"), 0, 2);

        numSeuil = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 12),
            Minimum = 0,
            Maximum = 1000000,
            Value = 10,
            ThousandsSeparator = true,
            Margin = new Padding(5)
        };

        formulaire.Controls.Add(numSeuil, 1, 2);

        var panneauBoutons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Padding = new Padding(0, 10, 0, 0)
        };

        formulaire.Controls.Add(panneauBoutons, 0, 3);
        formulaire.SetColumnSpan(panneauBoutons, 4);

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

        var btnVider = CreerBouton(
            "Nouveau",
            Color.FromArgb(95, 105, 115)
        );

        btnAjouter.Click += AjouterProduit;
        btnModifier.Click += ModifierProduit;
        btnSupprimer.Click += SupprimerProduit;
        btnVider.Click += (_, _) => ViderFormulaire();

        panneauBoutons.Controls.Add(btnAjouter);
        panneauBoutons.Controls.Add(btnModifier);
        panneauBoutons.Controls.Add(btnSupprimer);
        panneauBoutons.Controls.Add(btnVider);

        dgvProduits = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 10),
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 36 },
            Margin = new Padding(0, 15, 0, 0)
        };

        dgvProduits.ColumnHeadersDefaultCellStyle.Font =
            new Font("Segoe UI", 10, FontStyle.Bold);

        dgvProduits.ColumnHeadersDefaultCellStyle.BackColor =
            Color.FromArgb(25, 49, 78);

        dgvProduits.ColumnHeadersDefaultCellStyle.ForeColor =
            Color.White;

        dgvProduits.EnableHeadersVisualStyles = false;
        dgvProduits.CellClick += SelectionnerProduit;

        structure.Controls.Add(dgvProduits, 0, 2);

        ChargerProduits();
    }

    private static Label CreerLabel(string texte)
    {
        return new Label
        {
            Text = texte,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
            ForeColor = Color.FromArgb(50, 60, 70),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = new Padding(5)
        };
    }

    private static Button CreerBouton(string texte, Color couleur)
    {
        var bouton = new Button
        {
            Text = texte,
            Width = 140,
            Height = 42,
            BackColor = couleur,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10, FontStyle.Bold),
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
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    id,
                    nom,
                    poids_grammes,
                    prix_vente,
                    quantite_stock,
                    seuil_alerte,
                    date_creation
                FROM produits
                ORDER BY poids_grammes, nom;
                """;

            using MySqlDataAdapter adapter =
       new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            dgvProduits.DataSource = table;

            if (dgvProduits.Columns["id"] is DataGridViewColumn colonneId)
            {
                colonneId.Visible = false;
            }

            if (dgvProduits.Columns["nom"] is DataGridViewColumn colonneNom)
            {
                colonneNom.HeaderText = "Produit";
            }

            if (dgvProduits.Columns["poids_grammes"]
                is DataGridViewColumn colonnePoids)
            {
                colonnePoids.HeaderText = "Poids (g)";
            }

            if (dgvProduits.Columns["prix_vente"]
                is DataGridViewColumn colonnePrix)
            {
                colonnePrix.HeaderText = "Prix de vente";
                colonnePrix.DefaultCellStyle.Format = "N2";
                colonnePrix.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (dgvProduits.Columns["quantite_stock"]
                is DataGridViewColumn colonneStock)
            {
                colonneStock.HeaderText = "Stock";
                colonneStock.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvProduits.Columns["seuil_alerte"]
                is DataGridViewColumn colonneSeuil)
            {
                colonneSeuil.HeaderText = "Seuil d’alerte";
                colonneSeuil.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvProduits.Columns["date_creation"]
                is DataGridViewColumn colonneDate)
            {
                colonneDate.HeaderText = "Date de création";
                colonneDate.DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";
            }

            dgvProduits.ClearSelection();
            ColorerStocksFaibles();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de charger les produits.\n\n" + ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void AjouterProduit(object? sender, EventArgs e)
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
                INSERT INTO produits
                (
                    nom,
                    poids_grammes,
                    prix_vente,
                    quantite_stock,
                    seuil_alerte
                )
                VALUES
                (
                    @nom,
                    @poids,
                    @prix,
                    @stock,
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
                "@poids",
                (int)numPoids.Value
            );

            command.Parameters.AddWithValue(
                "@prix",
                numPrix.Value
            );

            command.Parameters.AddWithValue(
                "@stock",
                (int)numStock.Value
            );

            command.Parameters.AddWithValue(
                "@seuil",
                (int)numSeuil.Value
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le produit a été ajouté.",
                "Produit ajouté",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ViderFormulaire();
            ChargerProduits();
        }
        catch (MySqlException ex)
        {
            string message = ex.Number == 1062
                ? "Ce produit avec ce poids existe déjà."
                : ex.Message;

            MessageBox.Show(
                message,
                "Impossible d’ajouter le produit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void ModifierProduit(object? sender, EventArgs e)
    {
        if (produitSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un produit dans le tableau.",
                "Produit non sélectionné",
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
                UPDATE produits
                SET
                    nom = @nom,
                    poids_grammes = @poids,
                    prix_vente = @prix,
                    quantite_stock = @stock,
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
                "@poids",
                (int)numPoids.Value
            );

            command.Parameters.AddWithValue(
                "@prix",
                numPrix.Value
            );

            command.Parameters.AddWithValue(
                "@stock",
                (int)numStock.Value
            );

            command.Parameters.AddWithValue(
                "@seuil",
                (int)numSeuil.Value
            );

            command.Parameters.AddWithValue(
                "@id",
                produitSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le produit a été modifié.",
                "Modification réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ViderFormulaire();
            ChargerProduits();
        }
        catch (MySqlException ex)
        {
            string message = ex.Number == 1062
                ? "Un produit avec ce nom et ce poids existe déjà."
                : ex.Message;

            MessageBox.Show(
                message,
                "Impossible de modifier le produit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SupprimerProduit(object? sender, EventArgs e)
    {
        if (produitSelectionneId == 0)
        {
            MessageBox.Show(
                "Sélectionnez un produit dans le tableau.",
                "Produit non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        DialogResult reponse = MessageBox.Show(
            "Voulez-vous réellement supprimer ce produit ?",
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
                DELETE FROM produits
                WHERE id = @id;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue(
                "@id",
                produitSelectionneId
            );

            command.ExecuteNonQuery();

            MessageBox.Show(
                "Le produit a été supprimé.",
                "Suppression réussie",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ViderFormulaire();
            ChargerProduits();
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de supprimer ce produit.\n\n" +
                "Il est peut-être déjà utilisé dans une vente.\n\n" +
                ex.Message,
                "Suppression impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }

    private void SelectionnerProduit(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        DataGridViewRow ligne =
            dgvProduits.Rows[e.RowIndex];

        produitSelectionneId =
            Convert.ToInt32(ligne.Cells["id"].Value);

        txtNom.Text =
            Convert.ToString(ligne.Cells["nom"].Value) ?? "";

        numPoids.Value =
            Convert.ToDecimal(ligne.Cells["poids_grammes"].Value);

        numPrix.Value =
            Convert.ToDecimal(ligne.Cells["prix_vente"].Value);

        numStock.Value =
            Convert.ToDecimal(ligne.Cells["quantite_stock"].Value);

        numSeuil.Value =
            Convert.ToDecimal(ligne.Cells["seuil_alerte"].Value);
    }

    private bool ValiderFormulaire()
    {
        if (string.IsNullOrWhiteSpace(txtNom.Text))
        {
            MessageBox.Show(
                "Veuillez saisir le nom du produit.",
                "Champ obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtNom.Focus();
            return false;
        }

        if (numPoids.Value <= 0)
        {
            MessageBox.Show(
                "Le poids doit être supérieur à zéro.",
                "Poids incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            numPoids.Focus();
            return false;
        }

        if (numPrix.Value < 0)
        {
            MessageBox.Show(
                "Le prix ne peut pas être négatif.",
                "Prix incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            numPrix.Focus();
            return false;
        }

        return true;
    }

    private void ViderFormulaire()
    {
        produitSelectionneId = 0;

        txtNom.Clear();
        numPoids.Value = 1;
        numPrix.Value = 0;
        numStock.Value = 0;
        numSeuil.Value = 10;

        dgvProduits.ClearSelection();
        txtNom.Focus();
    }

    private void ColorerStocksFaibles()
    {
        foreach (DataGridViewRow ligne in dgvProduits.Rows)
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
