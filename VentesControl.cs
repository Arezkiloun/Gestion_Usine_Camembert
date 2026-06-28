using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;
using System.Diagnostics;
namespace GestionUsine;

public sealed class VentesControl : UserControl
{
    // =========================================================
    // DONNÉES DU PANIER
    // =========================================================

    private readonly DataTable panierTable = new();

    // =========================================================
    // CONTRÔLES — NOUVELLE VENTE
    // =========================================================

    private ComboBox cmbClient = null!;
    private ComboBox cmbProduit = null!;
    private ComboBox cmbTypePaiement = null!;

    private NumericUpDown numQuantite = null!;
    private NumericUpDown numPrixUnitaire = null!;
    private NumericUpDown numMontantPaye = null!;

    private Label lblStockProduit = null!;
    private Label lblTypeTarif = null!;
    private Label lblTotal = null!;
    private Label lblReste = null!;

    private Button btnEnregistrerVente = null!;

    private DataGridView dgvPanier = null!;
    private DataGridView dgvVentes = null!;

    // =========================================================
    // CONSTRUCTEUR
    // =========================================================

    public VentesControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        InitialiserTablePanier();
        Controls.Add(CreerInterfacePrincipale());

        ChargerClients();
        ChargerProduits();
        ChargerHistorique();
        RecalculerPaiement();
    }

    // =========================================================
    // CRÉATION DE L’INTERFACE
    // =========================================================

    private Control CreerInterfacePrincipale()
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 60)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var lblTitre = new Label
        {
            Text = "Gestion des ventes",
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

        var ongletNouvelleVente = new TabPage
        {
            Text = "Nouvelle vente",
            BackColor = Color.FromArgb(242, 245, 248),
            Padding = new Padding(10),
            AutoScroll = true
        };

        var ongletHistorique = new TabPage
        {
            Text = "Historique des ventes",
            BackColor = Color.FromArgb(242, 245, 248),
            Padding = new Padding(10)
        };

        onglets.TabPages.Add(ongletNouvelleVente);
        onglets.TabPages.Add(ongletHistorique);

        structure.Controls.Add(onglets, 0, 1);

        CreerOngletNouvelleVente(ongletNouvelleVente);
        CreerOngletHistorique(ongletHistorique);

        return structure;
    }

    private void CreerOngletNouvelleVente(TabPage page)
    {
        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Color.Transparent,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 175)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 205)
        );

        page.Controls.Add(structure);

        structure.Controls.Add(
            CreerCarteSelectionProduit(),
            0,
            0
        );

        dgvPanier = CreerTableauPanier();
        structure.Controls.Add(dgvPanier, 0, 1);

        structure.Controls.Add(
            CreerCartePaiement(),
            0,
            2
        );
    }

    private Control CreerCarteSelectionProduit()
    {
        var carte = CreerCarte();

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 7,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 18)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 23)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 11)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 11)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 14)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 13)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 10)
        );

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
            CreerLabel("Client"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Produit"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Stock"),
            2,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Quantité"),
            3,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Prix unitaire"),
            4,
            0
        );

        cmbClient = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        cmbClient.SelectedIndexChanged += (_, _) =>
        {
            AfficherProduitSelectionne();
        };

        cmbProduit = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        cmbProduit.SelectedIndexChanged += (_, _) =>
        {
            AfficherProduitSelectionne();
        };

        lblStockProduit = new Label
        {
            Text = "0",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 15, FontStyle.Bold),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(5)
        };

        numQuantite = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 1,
            Maximum = 1_000_000,
            Value = 1,
            ThousandsSeparator = true,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        numPrixUnitaire = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 100_000_000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            ReadOnly = true,
            InterceptArrowKeys = false,
            TabStop = false,
            Font = new Font(
                "Segoe UI",
                11,
                FontStyle.Bold
            ),
            BackColor = Color.White,
            Margin = new Padding(5)
        };

        lblTypeTarif = new Label
        {
            Text = "Tarif normal",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(95, 105, 115),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding = new Padding(5, 0, 0, 0),
            Margin = new Padding(5, 0, 5, 0)
        };

        var btnAjouter = CreerBouton(
            "Ajouter",
            Color.FromArgb(38, 138, 83)
        );

        var btnRetirer = CreerBouton(
            "Retirer",
            Color.FromArgb(190, 58, 58)
        );

        btnAjouter.Click += AjouterAuPanier;
        btnRetirer.Click += RetirerDuPanier;

        formulaire.Controls.Add(cmbClient, 0, 1);
        formulaire.Controls.Add(cmbProduit, 1, 1);
        formulaire.Controls.Add(lblStockProduit, 2, 1);
        formulaire.Controls.Add(numQuantite, 3, 1);
        formulaire.Controls.Add(numPrixUnitaire, 4, 1);
        formulaire.Controls.Add(btnAjouter, 5, 1);
        formulaire.Controls.Add(btnRetirer, 6, 1);

        formulaire.Controls.Add(
            lblTypeTarif,
            4,
            2
        );

        formulaire.SetColumnSpan(
            lblTypeTarif,
            3
        );

        return carte;
    }

    private Control CreerCartePaiement()
    {
        var carte = CreerCarte();

        var paiement = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 6,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        for (int i = 0; i < 6; i++)
        {
            paiement.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 16.66F)
            );
        }

        paiement.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42)
        );

        paiement.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carte.Controls.Add(paiement);

        paiement.Controls.Add(
            CreerLabel("Type de paiement"),
            0,
            0
        );

        paiement.Controls.Add(
            CreerLabel("Total"),
            1,
            0
        );

        paiement.Controls.Add(
            CreerLabel("Montant payé"),
            2,
            0
        );

        paiement.Controls.Add(
            CreerLabel("Reste à payer"),
            3,
            0
        );

        cmbTypePaiement = new ComboBox
        {
            Dock = DockStyle.Fill,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 11),
            Margin = new Padding(5)
        };

        cmbTypePaiement.Items.AddRange(
            new object[]
            {
                "Paiement cash",
                "Paiement avec versement",
                "Paiement à crédit"
            }
        );

        cmbTypePaiement.SelectedIndex = 0;

        cmbTypePaiement.SelectedIndexChanged += (_, _) =>
        {
            RecalculerPaiement();
        };

        lblTotal = CreerLabelMontant();
        lblReste = CreerLabelMontant();

        numMontantPaye = new NumericUpDown
        {
            Dock = DockStyle.Fill,
            Minimum = 0,
            Maximum = 1_000_000_000,
            DecimalPlaces = 2,
            ThousandsSeparator = true,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            Margin = new Padding(5)
        };

        numMontantPaye.ValueChanged += (_, _) =>
        {
            MettreAJourReste();
        };

        btnEnregistrerVente = CreerBouton(
            "Enregistrer la vente",
            Color.FromArgb(38, 138, 83)
        );

        var btnNouvelleVente = CreerBouton(
            "Nouvelle vente",
            Color.FromArgb(95, 105, 115)
        );

        btnEnregistrerVente.Click += EnregistrerVente;

        btnNouvelleVente.Click += (_, _) =>
        {
            ViderVente();
        };

        paiement.Controls.Add(cmbTypePaiement, 0, 1);
        paiement.Controls.Add(lblTotal, 1, 1);
        paiement.Controls.Add(numMontantPaye, 2, 1);
        paiement.Controls.Add(lblReste, 3, 1);
        paiement.Controls.Add(btnEnregistrerVente, 4, 1);
        paiement.Controls.Add(btnNouvelleVente, 5, 1);

        return carte;
    }

    private void CreerOngletHistorique(TabPage page)
    {
        dgvVentes = CreerTableauBase();
        dgvVentes.AutoGenerateColumns = false;

        dgvVentes.Columns.Add(
            CreerColonneTexte(
                "id",
                "id",
                "N° vente",
                100
            )
        );

        dgvVentes.Columns.Add(
            CreerColonneTexte(
                "client",
                "client",
                "Client",
                220,
                true
            )
        );

        dgvVentes.Columns.Add(
            CreerColonneMontant(
                "montant_total",
                "montant_total",
                "Total"
            )
        );

        dgvVentes.Columns.Add(
            CreerColonneMontant(
                "montant_paye",
                "montant_paye",
                "Payé"
            )
        );

        dgvVentes.Columns.Add(
            CreerColonneMontant(
                "reste_a_payer",
                "reste_a_payer",
                "Reste"
            )
        );

        dgvVentes.Columns.Add(
            CreerColonneTexte(
                "type_paiement",
                "type_paiement",
                "Paiement",
                190
            )
        );

        dgvVentes.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "date_vente",
                DataPropertyName = "date_vente",
                HeaderText = "Date",
                Width = 170,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "dd/MM/yyyy HH:mm"
                }
            }
        );

        dgvVentes.Columns.Add(
            CreerColonneTexte(
                "statut",
                "statut",
                "Statut",
                110
            )
        );
        var colonnePdf =
    new DataGridViewButtonColumn
    {
        Name = "pdf_action",
        HeaderText = "Document",
        Text = "Bon de livraison",
        UseColumnTextForButtonValue = false,
        Width = 150,
        FlatStyle = FlatStyle.Flat,

        DefaultCellStyle =
            new DataGridViewCellStyle
            {
                BackColor =
                    Color.FromArgb(38, 138, 83),

                ForeColor = Color.White,

                SelectionBackColor =
                    Color.FromArgb(32, 113, 171),

                SelectionForeColor =
                    Color.White,

                Alignment =
                    DataGridViewContentAlignment
                        .MiddleCenter
            }
    };

        dgvVentes.Columns.Add(colonnePdf);

        dgvVentes.CellContentClick +=
            OuvrirPdfCommande;

        var colonneAnnuler =
            new DataGridViewButtonColumn
            {
                Name = "annuler_action",
                HeaderText = "Annulation",
                Text = "Annuler",
                UseColumnTextForButtonValue = false,
                Width = 120,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        BackColor =
                            Color.FromArgb(190, 58, 58),
                        ForeColor = Color.White,
                        SelectionBackColor =
                            Color.FromArgb(155, 48, 48),
                        SelectionForeColor = Color.White,
                        Alignment =
                            DataGridViewContentAlignment.MiddleCenter
                    }
            };

        dgvVentes.Columns.Add(colonneAnnuler);

        dgvVentes.CellContentClick +=
            AnnulerVenteDepuisHistorique;

        dgvVentes.CellFormatting +=
            FormaterBoutonsHistorique;

        page.Controls.Add(dgvVentes);
    }

    // =========================================================
    // CONFIGURATION DU PANIER
    // =========================================================

    private void InitialiserTablePanier()
    {
        panierTable.Columns.Add(
            "produit_id",
            typeof(int)
        );

        panierTable.Columns.Add(
            "produit",
            typeof(string)
        );

        panierTable.Columns.Add(
            "quantite",
            typeof(int)
        );

        panierTable.Columns.Add(
            "prix_unitaire",
            typeof(decimal)
        );

        panierTable.Columns.Add(
            "sous_total",
            typeof(decimal)
        );
    }
    private void OuvrirPdfCommande(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 ||
            e.ColumnIndex < 0)
        {
            return;
        }

        if (dgvVentes.Columns[e.ColumnIndex].Name
            != "pdf_action")
        {
            return;
        }

        DataGridViewRow ligne =
            dgvVentes.Rows[e.RowIndex];

        string statutAffiche =
            Convert.ToString(
                ligne.Cells["statut"].Value
            )?.Trim() ?? "";

        // Blocage immédiat, y compris pour :
        // Annulée, ANNULEE, ANNULE, etc.
        if (statutAffiche.StartsWith(
                "Annul",
                StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                "Cette vente est annulée.\n\n" +
                "Le bon de livraison et l’impression sont désactivés.",
                "Document indisponible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        int venteId =
            Convert.ToInt32(
                ligne.Cells["id"].Value
            );

        try
        {
            // Vérification directe dans MySQL :
            // l'impression est bloquée même si l'affichage
            // du tableau n'a pas encore été actualisé.
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sqlStatut = """
                SELECT
                    CASE
                        WHEN UPPER(
                            TRIM(
                                COALESCE(statut, '')
                            )
                        ) LIKE 'ANNUL%'
                        OR date_annulation IS NOT NULL
                        THEN 1
                        ELSE 0
                    END
                FROM ventes
                WHERE id = @venteId
                LIMIT 1;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sqlStatut,
                    connection
                );

            command.Parameters.AddWithValue(
                "@venteId",
                venteId
            );

            object? resultat =
                command.ExecuteScalar();

            if (resultat == null ||
                resultat == DBNull.Value)
            {
                MessageBox.Show(
                    "La vente sélectionnée n'existe plus.",
                    "Vente introuvable",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            bool venteAnnulee =
                Convert.ToInt32(resultat) == 1;

            if (venteAnnulee)
            {
                MessageBox.Show(
                    "Cette vente est annulée.\n\n" +
                    "Le bon de livraison et l’impression sont désactivés.",
                    "Document indisponible",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ChargerHistorique();
                return;
            }

            Cursor = Cursors.WaitCursor;

            string cheminPdf =
                FacturePdfService.ObtenirOuGenerer(
                    venteId
                );

            Process.Start(
                new ProcessStartInfo
                {
                    FileName = cheminPdf,
                    UseShellExecute = true
                }
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Impossible de créer ou d’ouvrir le bon de livraison.\n\n" +
                ex.Message,
                "Erreur bon de livraison",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        finally
        {
            Cursor = Cursors.Default;
        }
    }

    private void AnnulerVenteDepuisHistorique(
        object? sender,
        DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0 ||
            e.ColumnIndex < 0)
        {
            return;
        }

        string nomColonne =
            dgvVentes.Columns[e.ColumnIndex].Name;

        if (nomColonne != "annuler_action")
        {
            return;
        }

        DataGridViewRow ligne =
            dgvVentes.Rows[e.RowIndex];

        int venteId =
            Convert.ToInt32(
                ligne.Cells["id"].Value
            );

        string statut =
            Convert.ToString(
                ligne.Cells["statut"].Value
            ) ?? "";

        if (statut == "Annulée")
        {
            MessageBox.Show(
                "Cette vente est déjà annulée.",
                "Annulation impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        string client =
            Convert.ToString(
                ligne.Cells["client"].Value
            ) ?? "";

        decimal total =
            Convert.ToDecimal(
                ligne.Cells["montant_total"].Value
            );

        DialogResult confirmation =
            MessageBox.Show(
                "Voulez-vous vraiment annuler cette vente ?\n\n" +
                $"N° vente : {venteId}\n" +
                $"Client : {client}\n" +
                $"Total : {total:N2} DA\n\n" +
                "Cette opération va :\n" +
                "• remettre les produits dans le stock ;\n" +
                "• diminuer la caisse du montant encaissé ;\n" +
                "• annuler le crédit éventuel.\n\n" +
                "La vente restera visible dans l’historique avec le statut « Annulée ».",
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
            AnnulerVenteTransactionnelle(venteId);

            MessageBox.Show(
                "La vente a été annulée correctement.\n\n" +
                "Les produits ont été remis en stock et la caisse a été corrigée.",
                "Vente annulée",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            ChargerProduits();
            ChargerHistorique();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible d’annuler la vente.",
                ex
            );
        }
    }

    private static void AnnulerVenteTransactionnelle(
        int venteId)
    {
        using MySqlConnection connection =
            Database.CreateConnection();

        connection.Open();

        using MySqlTransaction transaction =
            connection.BeginTransaction();

        try
        {
            decimal montantPaye;
            string statutVente;

            const string sqlVente = """
                SELECT
                    montant_paye,
                    statut
                FROM ventes
                WHERE id = @venteId
                FOR UPDATE;
                """;

            using (
                MySqlCommand command =
                    new MySqlCommand(
                        sqlVente,
                        connection,
                        transaction
                    )
            )
            {
                command.Parameters.AddWithValue(
                    "@venteId",
                    venteId
                );

                using MySqlDataReader reader =
                    command.ExecuteReader();

                if (!reader.Read())
                {
                    throw new InvalidOperationException(
                        "La vente sélectionnée n’existe plus."
                    );
                }

                montantPaye =
                    reader.GetDecimal("montant_paye");

                statutVente =
                    reader.GetString("statut");
            }

            if (statutVente == "ANNULEE")
            {
                throw new InvalidOperationException(
                    "Cette vente est déjà annulée."
                );
            }

            int? creditId = null;

            const string sqlCredit = """
                SELECT id
                FROM credits_clients
                WHERE vente_id = @venteId
                LIMIT 1;
                """;

            using (
                MySqlCommand command =
                    new MySqlCommand(
                        sqlCredit,
                        connection,
                        transaction
                    )
            )
            {
                command.Parameters.AddWithValue(
                    "@venteId",
                    venteId
                );

                object? resultat =
                    command.ExecuteScalar();

                if (resultat != null &&
                    resultat != DBNull.Value)
                {
                    creditId =
                        Convert.ToInt32(resultat);
                }
            }

            if (creditId.HasValue)
            {
                const string sqlCompterVersements = """
                    SELECT COUNT(*)
                    FROM versements_clients
                    WHERE credit_id = @creditId;
                    """;

                using MySqlCommand command =
                    new MySqlCommand(
                        sqlCompterVersements,
                        connection,
                        transaction
                    );

                command.Parameters.AddWithValue(
                    "@creditId",
                    creditId.Value
                );

                int nombreVersements =
                    Convert.ToInt32(
                        command.ExecuteScalar()
                    );

                if (nombreVersements > 0)
                {
                    throw new InvalidOperationException(
                        "Cette vente possède déjà un ou plusieurs versements client.\n\n" +
                        "Annulez d’abord les versements concernés avant d’annuler la vente."
                    );
                }
            }

            var lignes =
                new List<(int ProduitId, int Quantite)>();

            const string sqlDetails = """
                SELECT
                    produit_id,
                    quantite
                FROM details_ventes
                WHERE vente_id = @venteId;
                """;

            using (
                MySqlCommand command =
                    new MySqlCommand(
                        sqlDetails,
                        connection,
                        transaction
                    )
            )
            {
                command.Parameters.AddWithValue(
                    "@venteId",
                    venteId
                );

                using MySqlDataReader reader =
                    command.ExecuteReader();

                while (reader.Read())
                {
                    lignes.Add(
                        (
                            reader.GetInt32("produit_id"),
                            reader.GetInt32("quantite")
                        )
                    );
                }
            }

            foreach (
                (int produitId, int quantite)
                in lignes
            )
            {
                int ancienStock;

                const string sqlStock = """
                    SELECT quantite_stock
                    FROM produits
                    WHERE id = @produitId
                    FOR UPDATE;
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlStock,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@produitId",
                        produitId
                    );

                    object? resultat =
                        command.ExecuteScalar();

                    if (resultat == null)
                    {
                        throw new InvalidOperationException(
                            "Un produit de la vente n’existe plus."
                        );
                    }

                    ancienStock =
                        Convert.ToInt32(resultat);
                }

                int nouveauStock =
                    ancienStock + quantite;

                const string sqlUpdateStock = """
                    UPDATE produits
                    SET quantite_stock = @nouveauStock
                    WHERE id = @produitId;
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlUpdateStock,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@nouveauStock",
                        nouveauStock
                    );

                    command.Parameters.AddWithValue(
                        "@produitId",
                        produitId
                    );

                    command.ExecuteNonQuery();
                }

                const string sqlMouvementStock = """
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
                        'ENTREE_ANNULATION_VENTE',
                        @quantite,
                        @ancienStock,
                        @nouveauStock,
                        @motif
                    );
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlMouvementStock,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@produitId",
                        produitId
                    );

                    command.Parameters.AddWithValue(
                        "@quantite",
                        quantite
                    );

                    command.Parameters.AddWithValue(
                        "@ancienStock",
                        ancienStock
                    );

                    command.Parameters.AddWithValue(
                        "@nouveauStock",
                        nouveauStock
                    );

                    command.Parameters.AddWithValue(
                        "@motif",
                        $"Annulation vente n° {venteId}"
                    );

                    command.ExecuteNonQuery();
                }
            }

            if (montantPaye > 0)
            {
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
                        'ANNULATION_VENTE',
                        @montant,
                        @motif,
                        'VENTE',
                        @venteId
                    );
                    """;

                using MySqlCommand command =
                    new MySqlCommand(
                        sqlCaisse,
                        connection,
                        transaction
                    );

                command.Parameters.AddWithValue(
                    "@montant",
                    montantPaye
                );

                command.Parameters.AddWithValue(
                    "@motif",
                    $"Annulation vente n° {venteId}"
                );

                command.Parameters.AddWithValue(
                    "@venteId",
                    venteId
                );

                command.ExecuteNonQuery();
            }

            if (creditId.HasValue)
            {
                const string sqlAnnulerCredit = """
                    UPDATE credits_clients
                    SET
                        statut = 'ANNULE',
                        reste_a_payer = 0
                    WHERE id = @creditId;
                    """;

                using MySqlCommand command =
                    new MySqlCommand(
                        sqlAnnulerCredit,
                        connection,
                        transaction
                    );

                command.Parameters.AddWithValue(
                    "@creditId",
                    creditId.Value
                );

                command.ExecuteNonQuery();
            }

            const string sqlAnnulerVente = """
                UPDATE ventes
                SET
                    statut = 'ANNULEE',
                    date_annulation = CURRENT_TIMESTAMP,
                    pdf_path = NULL
                WHERE id = @venteId;
                """;

            using (
                MySqlCommand command =
                    new MySqlCommand(
                        sqlAnnulerVente,
                        connection,
                        transaction
                    )
            )
            {
                command.Parameters.AddWithValue(
                    "@venteId",
                    venteId
                );

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private void FormaterBoutonsHistorique(
        object? sender,
        DataGridViewCellFormattingEventArgs e)
    {
        if (e.RowIndex < 0 ||
            e.ColumnIndex < 0)
        {
            return;
        }

        string nomColonne =
            dgvVentes.Columns[e.ColumnIndex].Name;

        if (nomColonne != "pdf_action" &&
            nomColonne != "annuler_action")
        {
            return;
        }

        string statut =
            Convert.ToString(
                dgvVentes.Rows[e.RowIndex]
                    .Cells["statut"]
                    .Value
            )?.Trim() ?? "";

        bool venteAnnulee =
            statut.StartsWith(
                "Annul",
                StringComparison.OrdinalIgnoreCase
            );

        if (nomColonne == "pdf_action")
        {
            e.Value =
                venteAnnulee
                    ? "Indisponible"
                    : "Bon de livraison";

            e.CellStyle.BackColor =
                venteAnnulee
                    ? Color.FromArgb(190, 190, 190)
                    : Color.FromArgb(32, 113, 171);

            e.CellStyle.ForeColor =
                venteAnnulee
                    ? Color.FromArgb(80, 80, 80)
                    : Color.White;

            e.CellStyle.SelectionBackColor =
                venteAnnulee
                    ? Color.FromArgb(190, 190, 190)
                    : Color.FromArgb(25, 90, 140);

            e.CellStyle.SelectionForeColor =
                venteAnnulee
                    ? Color.FromArgb(80, 80, 80)
                    : Color.White;

            e.CellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            e.FormattingApplied = true;
            return;
        }

        e.Value =
            venteAnnulee
                ? "Déjà annulée"
                : "Annuler";

        e.CellStyle.BackColor =
            venteAnnulee
                ? Color.FromArgb(190, 190, 190)
                : Color.FromArgb(190, 58, 58);

        e.CellStyle.ForeColor =
            venteAnnulee
                ? Color.FromArgb(80, 80, 80)
                : Color.White;

        e.CellStyle.SelectionBackColor =
            venteAnnulee
                ? Color.FromArgb(190, 190, 190)
                : Color.FromArgb(155, 48, 48);

        e.CellStyle.SelectionForeColor =
            venteAnnulee
                ? Color.FromArgb(80, 80, 80)
                : Color.White;

        e.CellStyle.Alignment =
            DataGridViewContentAlignment.MiddleCenter;

        e.FormattingApplied = true;
    }

    private void MettreAJourBoutonsHistorique()
    {
        if (dgvVentes.Columns["pdf_action"] == null ||
            dgvVentes.Columns["annuler_action"] == null ||
            dgvVentes.Columns["statut"] == null)
        {
            return;
        }

        foreach (DataGridViewRow ligne in dgvVentes.Rows)
        {
            string statut =
                Convert.ToString(
                    ligne.Cells["statut"].Value
                )?.Trim() ?? "";

            bool venteAnnulee =
                statut.StartsWith(
                    "Annul",
                    StringComparison.OrdinalIgnoreCase
                );

            DataGridViewCell cellulePdf =
                ligne.Cells["pdf_action"];

            DataGridViewCell celluleAnnuler =
                ligne.Cells["annuler_action"];

            if (venteAnnulee)
            {
                cellulePdf.Value = "Indisponible";
                cellulePdf.Style.BackColor =
                    Color.FromArgb(215, 215, 215);
                cellulePdf.Style.ForeColor =
                    Color.FromArgb(105, 105, 105);
                cellulePdf.Style.SelectionBackColor =
                    Color.FromArgb(215, 215, 215);
                cellulePdf.Style.SelectionForeColor =
                    Color.FromArgb(105, 105, 105);

                celluleAnnuler.Value = "Déjà annulée";
                celluleAnnuler.Style.BackColor =
                    Color.FromArgb(215, 215, 215);
                celluleAnnuler.Style.ForeColor =
                    Color.FromArgb(105, 105, 105);
                celluleAnnuler.Style.SelectionBackColor =
                    Color.FromArgb(215, 215, 215);
                celluleAnnuler.Style.SelectionForeColor =
                    Color.FromArgb(105, 105, 105);
            }
            else
            {
                cellulePdf.Value = "Bon de livraison";
                cellulePdf.Style.BackColor =
                    Color.FromArgb(38, 138, 83);
                cellulePdf.Style.ForeColor =
                    Color.White;
                cellulePdf.Style.SelectionBackColor =
                    Color.FromArgb(32, 113, 171);
                cellulePdf.Style.SelectionForeColor =
                    Color.White;

                celluleAnnuler.Value = "Annuler";
                celluleAnnuler.Style.BackColor =
                    Color.FromArgb(190, 58, 58);
                celluleAnnuler.Style.ForeColor =
                    Color.White;
                celluleAnnuler.Style.SelectionBackColor =
                    Color.FromArgb(155, 48, 48);
                celluleAnnuler.Style.SelectionForeColor =
                    Color.White;
            }
        }
    }

    private void ColorerVentesAnnulees()
    {
        foreach (
            DataGridViewRow ligne
            in dgvVentes.Rows
        )
        {
            string statut =
                Convert.ToString(
                    ligne.Cells["statut"].Value
                ) ?? "";

            if (!statut.StartsWith(
                    "Annul",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            ligne.DefaultCellStyle.BackColor =
                Color.FromArgb(235, 235, 235);

            ligne.DefaultCellStyle.ForeColor =
                Color.FromArgb(120, 120, 120);
        }
    }

    private DataGridView CreerTableauPanier()
    {
        var tableau = CreerTableauBase();

        tableau.AutoGenerateColumns = false;

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "produit_id",
                DataPropertyName = "produit_id",
                Visible = false
            }
        );

        tableau.Columns.Add(
            CreerColonneTexte(
                "produit",
                "produit",
                "Produit",
                300,
                true
            )
        );

        tableau.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "quantite",
                DataPropertyName = "quantite",
                HeaderText = "Quantité",
                Width = 130,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment =
                        DataGridViewContentAlignment.MiddleCenter,
                    Format = "N0"
                }
            }
        );

        tableau.Columns.Add(
            CreerColonneMontant(
                "prix_unitaire",
                "prix_unitaire",
                "Prix unitaire"
            )
        );

        tableau.Columns.Add(
            CreerColonneMontant(
                "sous_total",
                "sous_total",
                "Sous-total"
            )
        );

        tableau.DataSource = panierTable;

        return tableau;
    }

    // =========================================================
    // CHARGEMENT DES DONNÉES
    // =========================================================

    private void ChargerClients()
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    id,
                    nom
                FROM clients
                WHERE actif = TRUE
                ORDER BY nom;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            cmbClient.DisplayMember = "nom";
            cmbClient.ValueMember = "id";
            cmbClient.DataSource = table;
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger les clients.",
                ex
            );
        }
    }

    private void ChargerProduits()
    {
        try
        {
            int? ancienProduitId =
                ObtenirValeurCombo(cmbProduit);

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
                    prix_vente,
                    quantite_stock
                FROM produits
                WHERE actif = TRUE
                ORDER BY poids_grammes, nom;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            cmbProduit.DisplayMember = "produit";
            cmbProduit.ValueMember = "id";
            cmbProduit.DataSource = table;

            if (ancienProduitId.HasValue)
            {
                cmbProduit.SelectedValue =
                    ancienProduitId.Value;
            }

            if (cmbProduit.Items.Count > 0 &&
                cmbProduit.SelectedIndex < 0)
            {
                cmbProduit.SelectedIndex = 0;
            }

            AfficherProduitSelectionne();
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger les produits.",
                ex
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

            const string sql = """
                SELECT
                    v.id,
                    c.nom AS client,
                    v.montant_total,
                    v.montant_paye,
                    v.reste_a_payer,

                    CASE v.type_paiement
                        WHEN 'CASH'
                            THEN 'Paiement cash'
                        WHEN 'VERSEMENT'
                            THEN 'Avec versement'
                        WHEN 'CREDIT'
                            THEN 'À crédit'
                        ELSE v.type_paiement
                    END AS type_paiement,

                    v.date_vente,

                    CASE v.statut
                        WHEN 'VALIDEE'
                            THEN 'Validée'
                        WHEN 'ANNULEE'
                            THEN 'Annulée'
                        ELSE v.statut
                    END AS statut

                FROM ventes v

                INNER JOIN clients c
                    ON c.id = v.client_id

                ORDER BY
                    v.date_vente DESC,
                    v.id DESC

                LIMIT 500;
                """;

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(sql, connection);

            var table = new DataTable();
            adapter.Fill(table);

            dgvVentes.DataSource = table;
            ColorerVentesAnnulees();
            MettreAJourBoutonsHistorique();
            dgvVentes.Invalidate();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger l’historique des ventes.",
                ex
            );
        }
    }

    private void AfficherProduitSelectionne()
    {
        if (
            cmbProduit.SelectedItem
            is not DataRowView ligne)
        {
            lblStockProduit.Text = "0";
            numPrixUnitaire.Value = 0;

            if (lblTypeTarif != null)
            {
                lblTypeTarif.Text =
                    "Aucun produit sélectionné";

                lblTypeTarif.ForeColor =
                    Color.FromArgb(95, 105, 115);
            }

            return;
        }

        int produitId =
            Convert.ToInt32(
                ligne["id"]
            );

        int stock =
            Convert.ToInt32(
                ligne["quantite_stock"]
            );

        decimal prixNormal =
            Convert.ToDecimal(
                ligne["prix_vente"]
            );

        int? clientId =
            ObtenirValeurCombo(
                cmbClient
            );

        decimal prixApplique =
            prixNormal;

        bool tarifSpecial =
            false;

        if (clientId.HasValue)
        {
            try
            {
                using MySqlConnection connection =
                    Database.CreateConnection();

                connection.Open();

                const string sql = """
                    SELECT prix_special
                    FROM tarifs_clients
                    WHERE client_id = @clientId
                      AND produit_id = @produitId
                      AND actif = TRUE
                    LIMIT 1;
                    """;

                using MySqlCommand command =
                    new MySqlCommand(
                        sql,
                        connection
                    );

                command.Parameters.AddWithValue(
                    "@clientId",
                    clientId.Value
                );

                command.Parameters.AddWithValue(
                    "@produitId",
                    produitId
                );

                object? resultat =
                    command.ExecuteScalar();

                if (
                    resultat != null &&
                    resultat != DBNull.Value)
                {
                    prixApplique =
                        Convert.ToDecimal(
                            resultat
                        );

                    tarifSpecial = true;
                }
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine(
                    "Tarif spécial indisponible : " +
                    ex.Message
                );
            }
        }

        lblStockProduit.Text =
            $"{stock:N0}";

        numPrixUnitaire.Value =
            Math.Min(
                prixApplique,
                numPrixUnitaire.Maximum
            );

        if (tarifSpecial)
        {
            lblTypeTarif.Text =
                $"Tarif spécial client appliqué : " +
                $"{prixApplique:N2} DA";

            lblTypeTarif.ForeColor =
                Color.FromArgb(38, 138, 83);
        }
        else
        {
            lblTypeTarif.Text =
                $"Tarif normal appliqué : " +
                $"{prixNormal:N2} DA";

            lblTypeTarif.ForeColor =
                Color.FromArgb(95, 105, 115);
        }
    }

    // =========================================================
    // GESTION DU PANIER
    // =========================================================

    private void AjouterAuPanier(
        object? sender,
        EventArgs e)
    {
        int? clientId =
            ObtenirValeurCombo(cmbClient);

        if (!clientId.HasValue)
        {
            MessageBox.Show(
                "Veuillez sélectionner un client avant d’ajouter des produits.",
                "Client obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            cmbClient.Focus();
            return;
        }

        if (cmbProduit.SelectedItem is not DataRowView produit)
        {
            MessageBox.Show(
                "Veuillez sélectionner un produit.",
                "Produit obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        int produitId =
            Convert.ToInt32(produit["id"]);

        string nomProduit =
            Convert.ToString(produit["produit"]) ?? "";

        int stockDisponible =
            Convert.ToInt32(
                produit["quantite_stock"]
            );

        int quantite =
            Convert.ToInt32(numQuantite.Value);

        decimal prix =
            numPrixUnitaire.Value;

        DataRow? ligneExistante =
            TrouverLignePanier(produitId);

        int quantiteExistante =
            ligneExistante == null
                ? 0
                : Convert.ToInt32(
                    ligneExistante["quantite"]
                );

        int quantiteTotale =
            quantiteExistante + quantite;

        if (quantiteTotale > stockDisponible)
        {
            MessageBox.Show(
                "Stock insuffisant.\n\n" +
                $"Stock disponible : {stockDisponible:N0}\n" +
                $"Déjà dans le panier : {quantiteExistante:N0}\n" +
                $"Quantité demandée : {quantite:N0}",
                "Quantité impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        if (ligneExistante == null)
        {
            panierTable.Rows.Add(
                produitId,
                nomProduit,
                quantite,
                prix,
                quantite * prix
            );
        }
        else
        {
            ligneExistante["quantite"] =
                quantiteTotale;

            ligneExistante["prix_unitaire"] =
                prix;

            ligneExistante["sous_total"] =
                quantiteTotale * prix;
        }

        // Dès le premier produit ajouté, le client est verrouillé.
        VerrouillerClient();

        numQuantite.Value = 1;

        SelectionnerLignePanier(produitId);
        RecalculerPaiement();
    }

    private void RetirerDuPanier(
        object? sender,
        EventArgs e)
    {
        if (dgvPanier.CurrentRow?.DataBoundItem
            is not DataRowView ligne)
        {
            MessageBox.Show(
                "Sélectionnez un produit dans le panier.",
                "Produit non sélectionné",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        panierTable.Rows.Remove(ligne.Row);
        RecalculerPaiement();
    }

    private DataRow? TrouverLignePanier(
        int produitId)
    {
        foreach (DataRow ligne in ObtenirLignesPanier())
        {
            if (Convert.ToInt32(
                    ligne["produit_id"]
                ) == produitId)
            {
                return ligne;
            }
        }

        return null;
    }

    private DataRow[] ObtenirLignesPanier()
    {
        return panierTable.Select(
            null,
            null,
            DataViewRowState.CurrentRows
        );
    }

    private void SelectionnerLignePanier(
        int produitId)
    {
        dgvPanier.ClearSelection();

        foreach (DataGridViewRow ligne in dgvPanier.Rows)
        {
            if (ligne.Cells["produit_id"].Value == null)
            {
                continue;
            }

            int id = Convert.ToInt32(
                ligne.Cells["produit_id"].Value
            );

            if (id != produitId)
            {
                continue;
            }

            ligne.Selected = true;
            dgvPanier.CurrentCell =
                ligne.Cells["produit"];

            break;
        }
    }

    // =========================================================
    // CALCUL DU PAIEMENT
    // =========================================================

    private decimal ObtenirTotal()
    {
        decimal total = 0;

        foreach (DataRow ligne in ObtenirLignesPanier())
        {
            total += Convert.ToDecimal(
                ligne["sous_total"]
            );
        }

        return total;
    }

    private void RecalculerPaiement()
    {
        decimal total = ObtenirTotal();

        lblTotal.Text =
            $"{total:N2} DA";

        numMontantPaye.Maximum =
            total;

        switch (cmbTypePaiement.Text)
        {
            case "Paiement cash":
                numMontantPaye.Enabled = false;
                numMontantPaye.Value = total;
                break;

            case "Paiement à crédit":
                numMontantPaye.Enabled = false;
                numMontantPaye.Value = 0;
                break;

            case "Paiement avec versement":
                numMontantPaye.Enabled = true;

                if (numMontantPaye.Value > total)
                {
                    numMontantPaye.Value = total;
                }

                break;
        }

        MettreAJourReste();
    }

    private void MettreAJourReste()
    {
        decimal total = ObtenirTotal();
        decimal montantPaye =
            numMontantPaye.Value;

        decimal reste =
            Math.Max(
                total - montantPaye,
                0
            );

        lblReste.Text =
            $"{reste:N2} DA";
    }

    // =========================================================
    // ENREGISTREMENT DE LA VENTE
    // =========================================================

    private void EnregistrerVente(
        object? sender,
        EventArgs e)
    {
        int? clientId =
            ObtenirValeurCombo(cmbClient);

        if (!clientId.HasValue)
        {
            MessageBox.Show(
                "Veuillez sélectionner un client.",
                "Client obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        DataRow[] lignesPanier =
            ObtenirLignesPanier();

        if (lignesPanier.Length == 0)
        {
            MessageBox.Show(
                "Le panier est vide.",
                "Aucun produit",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        decimal total =
            ObtenirTotal();

        if (total <= 0)
        {
            MessageBox.Show(
                "Le montant total doit être supérieur à zéro.",
                "Montant incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        decimal montantPaye =
            numMontantPaye.Value;

        string typePaiement =
            ObtenirTypePaiementBase();

        if (!ValiderPaiement(
                typePaiement,
                total,
                montantPaye))
        {
            return;
        }

        decimal reste =
            total - montantPaye;

        btnEnregistrerVente.Enabled = false;

        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                int venteId =
                    InsererVente(
                        connection,
                        transaction,
                        clientId.Value,
                        total,
                        montantPaye,
                        reste,
                        typePaiement
                    );

                EnregistrerDetailsEtStocks(
                    connection,
                    transaction,
                    venteId,
                    lignesPanier
                );

                if (montantPaye > 0)
                {
                    EnregistrerEntreeCaisse(
                        connection,
                        transaction,
                        venteId,
                        montantPaye
                    );
                }

                if (reste > 0)
                {
                    CreerCreditClient(
                        connection,
                        transaction,
                        venteId,
                        clientId.Value,
                        total,
                        montantPaye,
                        reste
                    );
                }

                transaction.Commit();
                try
                {
                    FacturePdfService.ObtenirOuGenerer(
                        venteId
                    );
                }
                catch (Exception pdfException)
                {
                    MessageBox.Show(
                        "La vente a bien été enregistrée, " +
                        "mais le bon de livraison n’a pas pu être créé.\n\n" +
                        pdfException.Message,
                        "Avertissement bon de livraison",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }

                MessageBox.Show(
                    "La vente a été enregistrée.\n\n" +
                    $"N° vente : {venteId}\n" +
                    $"Total : {total:N2} DA\n" +
                    $"Payé : {montantPaye:N2} DA\n" +
                    $"Reste : {reste:N2} DA",
                    "Vente réussie",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                ViderVente();
                ChargerProduits();
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
            AfficherErreur(
                "Impossible d’enregistrer la vente.",
                ex
            );
        }
        finally
        {
            btnEnregistrerVente.Enabled = true;
        }
    }

    private static int InsererVente(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int clientId,
        decimal total,
        decimal montantPaye,
        decimal reste,
        string typePaiement)
    {
        const string sql = """
            INSERT INTO ventes
            (
                client_id,
                montant_total,
                montant_paye,
                reste_a_payer,
                type_paiement
            )
            VALUES
            (
                @clientId,
                @total,
                @paye,
                @reste,
                @typePaiement
            );
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@clientId",
            clientId
        );

        command.Parameters.AddWithValue(
            "@total",
            total
        );

        command.Parameters.AddWithValue(
            "@paye",
            montantPaye
        );

        command.Parameters.AddWithValue(
            "@reste",
            reste
        );

        command.Parameters.AddWithValue(
            "@typePaiement",
            typePaiement
        );

        command.ExecuteNonQuery();

        return Convert.ToInt32(
            command.LastInsertedId
        );
    }

    private static void EnregistrerDetailsEtStocks(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int venteId,
        DataRow[] lignesPanier)
    {
        foreach (DataRow ligne in lignesPanier)
        {
            int produitId =
                Convert.ToInt32(
                    ligne["produit_id"]
                );

            int quantite =
                Convert.ToInt32(
                    ligne["quantite"]
                );

            decimal prix =
                Convert.ToDecimal(
                    ligne["prix_unitaire"]
                );

            decimal sousTotal =
                Convert.ToDecimal(
                    ligne["sous_total"]
                );

            int ancienStock =
                LireStockProduit(
                    connection,
                    transaction,
                    produitId
                );

            if (ancienStock < quantite)
            {
                throw new InvalidOperationException(
                    "Stock insuffisant pour un produit.\n\n" +
                    $"Stock disponible : {ancienStock:N0}\n" +
                    $"Quantité demandée : {quantite:N0}"
                );
            }

            int nouveauStock =
                ancienStock - quantite;

            InsererDetailVente(
                connection,
                transaction,
                venteId,
                produitId,
                quantite,
                prix,
                sousTotal
            );

            MettreAJourStockProduit(
                connection,
                transaction,
                produitId,
                nouveauStock
            );

            InsererMouvementStockVente(
                connection,
                transaction,
                venteId,
                produitId,
                quantite,
                ancienStock,
                nouveauStock
            );
        }
    }

    private static int LireStockProduit(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int produitId)
    {
        const string sql = """
            SELECT quantite_stock
            FROM produits
            WHERE id = @produitId
            FOR UPDATE;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@produitId",
            produitId
        );

        object? resultat =
            command.ExecuteScalar();

        if (resultat == null)
        {
            throw new InvalidOperationException(
                "Un produit du panier n’existe plus."
            );
        }

        return Convert.ToInt32(resultat);
    }

    private static void InsererDetailVente(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int venteId,
        int produitId,
        int quantite,
        decimal prix,
        decimal sousTotal)
    {
        const string sql = """
            INSERT INTO details_ventes
            (
                vente_id,
                produit_id,
                quantite,
                prix_unitaire,
                sous_total
            )
            VALUES
            (
                @venteId,
                @produitId,
                @quantite,
                @prix,
                @sousTotal
            );
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        command.Parameters.AddWithValue(
            "@produitId",
            produitId
        );

        command.Parameters.AddWithValue(
            "@quantite",
            quantite
        );

        command.Parameters.AddWithValue(
            "@prix",
            prix
        );

        command.Parameters.AddWithValue(
            "@sousTotal",
            sousTotal
        );

        command.ExecuteNonQuery();
    }

    private static void MettreAJourStockProduit(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int produitId,
        int nouveauStock)
    {
        const string sql = """
            UPDATE produits
            SET quantite_stock = @nouveauStock
            WHERE id = @produitId;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@nouveauStock",
            nouveauStock
        );

        command.Parameters.AddWithValue(
            "@produitId",
            produitId
        );

        command.ExecuteNonQuery();
    }

    private static void InsererMouvementStockVente(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int venteId,
        int produitId,
        int quantite,
        int ancienStock,
        int nouveauStock)
    {
        const string sql = """
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
                'SORTIE_VENTE',
                @quantite,
                @ancienStock,
                @nouveauStock,
                @motif
            );
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@produitId",
            produitId
        );

        command.Parameters.AddWithValue(
            "@quantite",
            quantite
        );

        command.Parameters.AddWithValue(
            "@ancienStock",
            ancienStock
        );

        command.Parameters.AddWithValue(
            "@nouveauStock",
            nouveauStock
        );

        command.Parameters.AddWithValue(
            "@motif",
            $"Vente n° {venteId}"
        );

        command.ExecuteNonQuery();
    }

    private static void EnregistrerEntreeCaisse(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int venteId,
        decimal montant)
    {
        const string sql = """
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
                'VENTE',
                @montant,
                @motif,
                'VENTE',
                @venteId
            );
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@montant",
            montant
        );

        command.Parameters.AddWithValue(
            "@motif",
            $"Paiement vente n° {venteId}"
        );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        command.ExecuteNonQuery();
    }

    private static void CreerCreditClient(
        MySqlConnection connection,
        MySqlTransaction transaction,
        int venteId,
        int clientId,
        decimal total,
        decimal montantPaye,
        decimal reste)
    {
        const string sql = """
            INSERT INTO credits_clients
            (
                vente_id,
                client_id,
                montant_total,
                montant_paye,
                reste_a_payer,
                statut
            )
            VALUES
            (
                @venteId,
                @clientId,
                @total,
                @paye,
                @reste,
                'OUVERT'
            );
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection,
                transaction
            );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        command.Parameters.AddWithValue(
            "@clientId",
            clientId
        );

        command.Parameters.AddWithValue(
            "@total",
            total
        );

        command.Parameters.AddWithValue(
            "@paye",
            montantPaye
        );

        command.Parameters.AddWithValue(
            "@reste",
            reste
        );

        command.ExecuteNonQuery();
    }

    // =========================================================
    // VALIDATION ET RÉINITIALISATION
    // =========================================================

    private string ObtenirTypePaiementBase()
    {
        return cmbTypePaiement.Text switch
        {
            "Paiement cash" => "CASH",
            "Paiement avec versement" => "VERSEMENT",
            "Paiement à crédit" => "CREDIT",
            _ => "CASH"
        };
    }

    private static bool ValiderPaiement(
        string typePaiement,
        decimal total,
        decimal montantPaye)
    {
        if (montantPaye < 0 ||
            montantPaye > total)
        {
            MessageBox.Show(
                "Le montant payé doit être compris entre 0 et le total.",
                "Montant incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }

        if (typePaiement == "CASH" &&
            montantPaye != total)
        {
            MessageBox.Show(
                "Pour un paiement cash, le montant payé doit être égal au total.",
                "Paiement incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }

        if (typePaiement == "CREDIT" &&
            montantPaye != 0)
        {
            MessageBox.Show(
                "Pour une vente à crédit, le montant payé doit être égal à zéro.",
                "Paiement incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }

        if (typePaiement == "VERSEMENT" &&
            (montantPaye <= 0 ||
             montantPaye >= total))
        {
            MessageBox.Show(
                "Pour un paiement avec versement, le montant payé doit être supérieur à zéro et inférieur au total.",
                "Versement incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return false;
        }

        return true;
    }

    private void VerrouillerClient()
    {
        cmbClient.Enabled = false;
        cmbClient.TabStop = false;
    }

    private void DeverrouillerClient()
    {
        cmbClient.Enabled = true;
        cmbClient.TabStop = true;
    }

    private void ViderVente()
    {
        panierTable.Clear();

        // Le client peut être changé seulement pour une nouvelle vente
        // ou après l’enregistrement de la vente précédente.
        DeverrouillerClient();

        if (cmbTypePaiement.Items.Count > 0)
        {
            cmbTypePaiement.SelectedIndex = 0;
        }

        numQuantite.Value = 1;
        RecalculerPaiement();

        if (cmbProduit.Items.Count > 0)
        {
            cmbProduit.SelectedIndex = 0;
        }

        cmbClient.Focus();
    }

    // =========================================================
    // OUTILS D’INTERFACE
    // =========================================================

    private static Panel CreerCarte()
    {
        return new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(15),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 0, 0, 10)
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
            ForeColor = Color.FromArgb(50, 60, 70),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = new Padding(5)
        };
    }

    private static Label CreerLabelMontant()
    {
        return new Label
        {
            Text = "0,00 DA",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                15,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft,
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
            Height = 45,
            BackColor = couleur,
            ForeColor = Color.White,
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

    private static DataGridView CreerTableauBase()
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
                DataGridViewAutoSizeColumnsMode.None,
            SelectionMode =
                DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 10),
            ColumnHeadersHeight = 42,
            RowTemplate = { Height = 36 },
            Margin = new Padding(0, 10, 0, 10)
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

        tableau.ColumnHeadersDefaultCellStyle.Alignment =
            DataGridViewContentAlignment.MiddleLeft;

        tableau.EnableHeadersVisualStyles = false;

        return tableau;
    }

    private static DataGridViewTextBoxColumn CreerColonneTexte(
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
            AutoSizeMode = remplir
                ? DataGridViewAutoSizeColumnMode.Fill
                : DataGridViewAutoSizeColumnMode.None
        };
    }

    private static DataGridViewTextBoxColumn CreerColonneMontant(
        string nom,
        string propriete,
        string titre)
    {
        return new DataGridViewTextBoxColumn
        {
            Name = nom,
            DataPropertyName = propriete,
            HeaderText = titre,
            Width = 170,
            DefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment =
                    DataGridViewContentAlignment.MiddleRight,
                Format = "N2"
            }
        };
    }

    private static int? ObtenirValeurCombo(
        ComboBox comboBox)
    {
        if (comboBox.SelectedValue == null ||
            comboBox.SelectedValue is DataRowView)
        {
            return null;
        }

        return Convert.ToInt32(
            comboBox.SelectedValue
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
