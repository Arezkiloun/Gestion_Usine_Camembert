using MySqlConnector;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using DrawingColor = System.Drawing.Color;

namespace GestionUsine;

public sealed class ImpressionsControl : UserControl
{
    private ComboBox cmbRapport = null!;
    private DateTimePicker dtpDateDebut = null!;
    private DateTimePicker dtpDateFin = null!;
    private TextBox txtRecherche = null!;

    private Label lblResume = null!;
    private DataGridView dgvRapport = null!;

    private DataTable tableCourante = new();
    private string titreRapportCourant = "";

    public ImpressionsControl()
    {
        Dock = DockStyle.Fill;
        BackColor =
            DrawingColor.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        Controls.Add(CreerInterface());

        cmbRapport.SelectedIndex = 0;
        ChargerRapport();
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
            new RowStyle(SizeType.Absolute, 118)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var titre = new Label
        {
            Text = "Centre d’impressions",
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
            CreerCarteCommandes(),
            0,
            1
        );

        lblResume = new Label
        {
            Text = "Aucune donnée",
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            BorderStyle = BorderStyle.FixedSingle,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 15, 0),
            Margin = new Padding(0, 0, 0, 10)
        };

        structure.Controls.Add(
            lblResume,
            0,
            2
        );

        dgvRapport = CreerTableau();

        structure.Controls.Add(
            dgvRapport,
            0,
            3
        );

        return structure;
    }

    private Control CreerCarteCommandes()
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(0, 0, 0, 10)
        };

        var grille = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 7,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 23)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 13)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 13)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 22)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 10)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 9)
        );

        grille.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 10)
        );

        carte.Controls.Add(grille);

        cmbRapport = new ComboBox
        {
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10)
        };

        cmbRapport.Items.AddRange(
            new object[]
            {
                "Journal de caisse",
                "Dépenses validées",
                "Recettes validées",
                "Ventes validées",
                "Crédits clients",
                "Liste des clients",
                "Liste des employés",
                "Stock des produits",
                "Stock des emballages"
            }
        );

        cmbRapport.SelectedIndexChanged +=
            (_, _) =>
            {
                AdapterFiltresAuRapport();
                ChargerRapport();
            };

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
                "Rechercher dans le rapport"
        };

        txtRecherche.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChargerRapport();
                e.SuppressKeyPress = true;
            }
        };

        var btnAfficher = CreerBouton(
            "Afficher",
            DrawingColor.FromArgb(32, 113, 171)
        );

        var btnCeMois = CreerBouton(
            "Ce mois",
            DrawingColor.FromArgb(95, 105, 115)
        );

        var btnPdf = CreerBouton(
            "Imprimer PDF",
            DrawingColor.FromArgb(93, 63, 137)
        );

        btnAfficher.Click += (_, _) =>
        {
            ChargerRapport();
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

            ChargerRapport();
        };

        btnPdf.Click += ImprimerRapportPdf;

        grille.Controls.Add(
            CreerBlocCommande(
                "Rapport",
                cmbRapport
            ),
            0,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Date début",
                dtpDateDebut
            ),
            1,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Date fin",
                dtpDateFin
            ),
            2,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Recherche",
                txtRecherche
            ),
            3,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Charger",
                btnAfficher
            ),
            4,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Période",
                btnCeMois
            ),
            5,
            0
        );

        grille.Controls.Add(
            CreerBlocCommande(
                "Impression",
                btnPdf
            ),
            6,
            0
        );

        return carte;
    }

    private static Control CreerBlocCommande(
        string titre,
        Control controle)
    {
        var bloc = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(4, 0, 4, 0),
            Padding = Padding.Empty
        };

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 30)
        );

        bloc.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 43)
        );

        var label = new Label
        {
            Text = titre,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                9,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.MiddleLeft,
            Padding = new Padding(2, 0, 0, 0),
            Margin = Padding.Empty
        };

        controle.Dock = DockStyle.Fill;
        controle.Margin =
            new Padding(0, 3, 0, 3);

        bloc.Controls.Add(label, 0, 0);
        bloc.Controls.Add(controle, 0, 1);

        return bloc;
    }

    private DataGridView CreerTableau()
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
            AutoGenerateColumns = true,
            AutoSizeColumnsMode =
                DataGridViewAutoSizeColumnsMode.Fill,
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

        tableau.DataBindingComplete += (_, _) =>
        {
            FormaterTableau();
        };

        return tableau;
    }

    private void AdapterFiltresAuRapport()
    {
        string rapport =
            cmbRapport.Text;

        bool avecDates =
            rapport == "Journal de caisse" ||
            rapport == "Dépenses validées" ||
            rapport == "Recettes validées" ||
            rapport == "Ventes validées";

        dtpDateDebut.Enabled = avecDates;
        dtpDateFin.Enabled = avecDates;

        dtpDateDebut.BackColor =
            avecDates
                ? DrawingColor.White
                : DrawingColor.FromArgb(235, 235, 235);

        dtpDateFin.BackColor =
            avecDates
                ? DrawingColor.White
                : DrawingColor.FromArgb(235, 235, 235);
    }

    private void ChargerRapport()
    {
        if (
            cmbRapport == null ||
            dgvRapport == null)
        {
            return;
        }

        bool avecDates =
            dtpDateDebut.Enabled;

        if (
            avecDates &&
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

            string recherche =
                txtRecherche.Text.Trim();

            string sql =
                ConstruireRequete(
                    cmbRapport.Text,
                    recherche
                );

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            if (avecDates)
            {
                command.Parameters.AddWithValue(
                    "@dateDebut",
                    dtpDateDebut.Value.Date
                );

                command.Parameters.AddWithValue(
                    "@dateFinExclusive",
                    dtpDateFin.Value.Date.AddDays(1)
                );
            }

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                command.Parameters.AddWithValue(
                    "@recherche",
                    $"%{recherche}%"
                );
            }

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(command);

            tableCourante = new DataTable();
            adapter.Fill(tableCourante);

            dgvRapport.DataSource =
                tableCourante;

            dgvRapport.ClearSelection();

            titreRapportCourant =
                cmbRapport.Text;

            ActualiserResume();
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger le rapport.",
                ex
            );
        }
    }

    private static string ConstruireRequete(
        string rapport,
        string recherche)
    {
        bool filtrer =
            !string.IsNullOrWhiteSpace(recherche);

        return rapport switch
        {
            "Journal de caisse" =>
                ConstruireSqlCaisse(filtrer),

            "Dépenses validées" =>
                ConstruireSqlDepenses(filtrer),

            "Recettes validées" =>
                ConstruireSqlRecettes(filtrer),

            "Ventes validées" =>
                ConstruireSqlVentes(filtrer),

            "Crédits clients" =>
                ConstruireSqlCredits(filtrer),

            "Liste des clients" =>
                ConstruireSqlClients(filtrer),

            "Liste des employés" =>
                ConstruireSqlEmployes(filtrer),

            "Stock des produits" =>
                ConstruireSqlProduits(filtrer),

            "Stock des emballages" =>
                ConstruireSqlEmballages(filtrer),

            _ => throw new InvalidOperationException(
                "Rapport non reconnu."
            )
        };
    }

    private static string ConstruireSqlCaisse(
        bool filtrer)
    {
        string sql = """
            SELECT
                mc.id AS `N°`,
                mc.date_mouvement AS `Date`,

                CASE mc.sens
                    WHEN 'ENTREE' THEN 'Entrée'
                    WHEN 'SORTIE' THEN 'Sortie'
                    ELSE mc.sens
                END AS `Sens`,

                CASE mc.type_mouvement
                    WHEN 'VENTE' THEN 'Vente'
                    WHEN 'VERSEMENT_CLIENT'
                        THEN 'Versement client'
                    WHEN 'RECETTE' THEN 'Recette'
                    WHEN 'DEPENSE' THEN 'Dépense'
                    ELSE REPLACE(
                        mc.type_mouvement,
                        '_',
                        ' '
                    )
                END AS `Type`,

                mc.montant AS `Montant`,
                mc.motif AS `Motif`,

                CASE
                    WHEN mc.reference_type IS NULL
                        OR mc.reference_type = ''
                        THEN '—'
                    WHEN mc.reference_id IS NULL
                        THEN mc.reference_type
                    ELSE CONCAT(
                        mc.reference_type,
                        ' n° ',
                        mc.reference_id
                    )
                END AS `Référence`

            FROM mouvements_caisse mc

            WHERE mc.date_mouvement >= @dateDebut
              AND mc.date_mouvement < @dateFinExclusive

              AND mc.type_mouvement NOT IN
              (
                  'ANNULATION_VENTE',
                  'ANNULATION_DEPENSE',
                  'ANNULATION_RECETTE'
              )

              AND NOT
              (
                  mc.type_mouvement = 'VENTE'
                  AND mc.reference_type = 'VENTE'
                  AND EXISTS
                  (
                      SELECT 1
                      FROM ventes v
                      WHERE v.id = mc.reference_id
                        AND COALESCE(
                            v.statut,
                            'VALIDEE'
                        ) = 'ANNULEE'
                  )
              )

              AND NOT
              (
                  mc.type_mouvement = 'DEPENSE'
                  AND mc.reference_type = 'DEPENSE'
                  AND EXISTS
                  (
                      SELECT 1
                      FROM depenses d
                      WHERE d.id = mc.reference_id
                        AND d.statut = 'ANNULEE'
                  )
              )

              AND NOT
              (
                  mc.type_mouvement = 'RECETTE'
                  AND mc.reference_type = 'RECETTE'
                  AND EXISTS
                  (
                      SELECT 1
                      FROM recettes r
                      WHERE r.id = mc.reference_id
                        AND r.statut = 'ANNULEE'
                  )
              )
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  mc.motif LIKE @recherche
                  OR mc.type_mouvement LIKE @recherche
                  OR mc.reference_type LIKE @recherche
                  OR CAST(mc.reference_id AS CHAR)
                      LIKE @recherche
                  OR CAST(mc.id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY
                mc.date_mouvement DESC,
                mc.id DESC;
            """;

        return sql;
    }

    private static string ConstruireSqlDepenses(
        bool filtrer)
    {
        string sql = """
            SELECT
                d.id AS `N°`,
                d.date_depense AS `Date`,
                d.categorie AS `Catégorie`,

                COALESCE(
                    CONCAT(
                        e.nom,
                        ' ',
                        e.prenom
                    ),
                    '—'
                ) AS `Employé`,

                d.montant AS `Montant`,
                d.motif AS `Motif`

            FROM depenses d

            LEFT JOIN employes e
                ON e.id = d.employe_id

            WHERE d.statut = 'VALIDEE'
              AND d.date_depense >= @dateDebut
              AND d.date_depense < @dateFinExclusive
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  d.categorie LIKE @recherche
                  OR d.motif LIKE @recherche
                  OR e.nom LIKE @recherche
                  OR e.prenom LIKE @recherche
                  OR CAST(d.id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY
                d.date_depense DESC,
                d.id DESC;
            """;

        return sql;
    }

    private static string ConstruireSqlRecettes(
        bool filtrer)
    {
        string sql = """
            SELECT
                id AS `N°`,
                date_recette AS `Date`,
                categorie AS `Catégorie`,
                montant AS `Montant`,
                motif AS `Motif`

            FROM recettes

            WHERE statut = 'VALIDEE'
              AND date_recette >= @dateDebut
              AND date_recette < @dateFinExclusive
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  categorie LIKE @recherche
                  OR motif LIKE @recherche
                  OR CAST(id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY
                date_recette DESC,
                id DESC;
            """;

        return sql;
    }

    private static string ConstruireSqlVentes(
        bool filtrer)
    {
        string sql = """
            SELECT
                v.id AS `N°`,
                v.date_vente AS `Date`,
                c.nom AS `Client`,
                v.montant_total AS `Total`,
                v.montant_paye AS `Payé`,
                v.reste_a_payer AS `Reste`,
                v.type_paiement AS `Paiement`

            FROM ventes v

            INNER JOIN clients c
                ON c.id = v.client_id

            WHERE COALESCE(
                    v.statut,
                    'VALIDEE'
                  ) <> 'ANNULEE'

              AND v.date_vente >= @dateDebut
              AND v.date_vente < @dateFinExclusive
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  c.nom LIKE @recherche
                  OR c.telephone LIKE @recherche
                  OR v.type_paiement LIKE @recherche
                  OR CAST(v.id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY
                v.date_vente DESC,
                v.id DESC;
            """;

        return sql;
    }

    private static string ConstruireSqlCredits(
        bool filtrer)
    {
        string sql = """
            SELECT
                cr.id AS `N°`,
                c.nom AS `Client`,
                cr.montant_total AS `Total`,
                cr.montant_paye AS `Payé`,
                cr.reste_a_payer AS `Reste`,

                CASE cr.statut
                    WHEN 'OUVERT' THEN 'Ouvert'
                    WHEN 'SOLDE' THEN 'Soldé'
                    ELSE cr.statut
                END AS `Statut`,

                cr.date_creation AS `Date création`,
                cr.date_dernier_versement
                    AS `Dernier versement`

            FROM credits_clients cr

            INNER JOIN clients c
                ON c.id = cr.client_id

            WHERE cr.statut <> 'ANNULE'
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  c.nom LIKE @recherche
                  OR c.telephone LIKE @recherche
                  OR cr.statut LIKE @recherche
                  OR CAST(cr.id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY
                cr.reste_a_payer DESC,
                c.nom;
            """;

        return sql;
    }

    private static string ConstruireSqlClients(
        bool filtrer)
    {
        string sql = """
            SELECT
                id AS `N°`,
                nom AS `Nom du client`,
                COALESCE(
                    telephone,
                    '—'
                ) AS `Téléphone`,
                COALESCE(
                    adresse,
                    '—'
                ) AS `Adresse`,
                date_creation AS `Date création`

            FROM clients

            WHERE actif = TRUE
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  nom LIKE @recherche
                  OR telephone LIKE @recherche
                  OR adresse LIKE @recherche
                  OR CAST(id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY nom;
            """;

        return sql;
    }

    private static string ConstruireSqlEmployes(
        bool filtrer)
    {
        string sql = """
            SELECT
                id AS `N°`,
                CONCAT(
                    nom,
                    ' ',
                    prenom
                ) AS `Employé`,
                COALESCE(
                    telephone,
                    '—'
                ) AS `Téléphone`,
                poste AS `Poste`,
                salaire_base AS `Salaire`,
                date_embauche AS `Date embauche`

            FROM employes

            WHERE actif = TRUE
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  nom LIKE @recherche
                  OR prenom LIKE @recherche
                  OR telephone LIKE @recherche
                  OR poste LIKE @recherche
                  OR CAST(id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY nom, prenom;
            """;

        return sql;
    }

    private static string ConstruireSqlProduits(
        bool filtrer)
    {
        string sql = """
            SELECT
                id AS `N°`,
                nom AS `Produit`,
                poids_grammes AS `Poids (g)`,
                prix_vente AS `Prix de vente`,
                quantite_stock AS `Stock`,
                seuil_alerte AS `Seuil`,

                CASE
                    WHEN quantite_stock <= 0
                        THEN 'Rupture'
                    WHEN quantite_stock <= seuil_alerte
                        THEN 'Stock faible'
                    ELSE 'Disponible'
                END AS `État`

            FROM produits

            WHERE actif = TRUE
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  nom LIKE @recherche
                  OR CAST(poids_grammes AS CHAR)
                      LIKE @recherche
                  OR CAST(id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY nom, poids_grammes;
            """;

        return sql;
    }

    private static string ConstruireSqlEmballages(
        bool filtrer)
    {
        string sql = """
            SELECT
                id AS `N°`,
                nom AS `Emballage`,
                unite AS `Unité`,
                quantite_stock AS `Stock`,
                seuil_alerte AS `Seuil`,

                CASE
                    WHEN quantite_stock <= 0
                        THEN 'Rupture'
                    WHEN quantite_stock <= seuil_alerte
                        THEN 'Stock faible'
                    ELSE 'Disponible'
                END AS `État`

            FROM emballages

            WHERE actif = TRUE
            """;

        if (filtrer)
        {
            sql += """

              AND
              (
                  nom LIKE @recherche
                  OR unite LIKE @recherche
                  OR CAST(id AS CHAR)
                      LIKE @recherche
              )
            """;
        }

        sql += """

            ORDER BY nom;
            """;

        return sql;
    }

    private void FormaterTableau()
    {
        foreach (
            DataGridViewColumn colonne
            in dgvRapport.Columns)
        {
            if (
                EstColonneMontant(
                    colonne.HeaderText
                ))
            {
                colonne.DefaultCellStyle.Format =
                    "#,##0.00 'DA'";

                colonne.DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleRight;
            }

            if (
                colonne.ValueType ==
                typeof(DateTime))
            {
                colonne.DefaultCellStyle.Format =
                    "dd/MM/yyyy HH:mm";
            }
        }

        if (
            dgvRapport.Columns.Count > 0)
        {
            dgvRapport.Columns[
                dgvRapport.Columns.Count - 1
            ].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.Fill;
        }
    }

    private static bool EstColonneMontant(
        string titre)
    {
        return titre == "Montant" ||
               titre == "Total" ||
               titre == "Payé" ||
               titre == "Reste" ||
               titre == "Salaire" ||
               titre == "Prix de vente";
    }

    private void ActualiserResume()
    {
        int nombre =
            tableCourante.Rows.Count;

        decimal total = 0;
        decimal entrees = 0;
        decimal sorties = 0;

        if (
            titreRapportCourant ==
            "Journal de caisse")
        {
            foreach (
                DataRow ligne
                in tableCourante.Rows)
            {
                decimal montant =
                    Convert.ToDecimal(
                        ligne["Montant"]
                    );

                string sens =
                    Convert.ToString(
                        ligne["Sens"]
                    ) ?? "";

                if (sens == "Entrée")
                {
                    entrees += montant;
                }
                else if (sens == "Sortie")
                {
                    sorties += montant;
                }
            }

            lblResume.Text =
                $"Mouvements : {nombre}   |   " +
                $"Entrées : {entrees:N2} DA   |   " +
                $"Sorties : {sorties:N2} DA   |   " +
                $"Solde période : {(entrees - sorties):N2} DA";

            return;
        }

        string? colonneMontant =
            TrouverColonneTotal();

        if (colonneMontant != null)
        {
            foreach (
                DataRow ligne
                in tableCourante.Rows)
            {
                if (
                    ligne[colonneMontant] != DBNull.Value)
                {
                    total += Convert.ToDecimal(
                        ligne[colonneMontant]
                    );
                }
            }

            lblResume.Text =
                $"Lignes : {nombre}   |   " +
                $"Total : {total:N2} DA";

            return;
        }

        lblResume.Text =
            $"Nombre de lignes : {nombre}";
    }

    private string? TrouverColonneTotal()
    {
        string[] priorites =
        {
            "Montant",
            "Reste",
            "Total",
            "Stock"
        };

        foreach (string colonne in priorites)
        {
            if (
                tableCourante.Columns.Contains(
                    colonne
                ))
            {
                return colonne;
            }
        }

        return null;
    }

    private void ImprimerRapportPdf(
        object? sender,
        EventArgs e)
    {
        if (tableCourante.Rows.Count == 0)
        {
            MessageBox.Show(
                "Le rapport ne contient aucune donnée.",
                "PDF impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        try
        {
            using var dialogue =
                new SaveFileDialog
                {
                    Title =
                        "Enregistrer le rapport",

                    Filter =
                        "Fichier PDF (*.pdf)|*.pdf",

                    FileName =
                        NettoyerNomFichier(
                            titreRapportCourant
                        ) +
                        "_" +
                        DateTime.Now.ToString(
                            "yyyyMMdd_HHmm"
                        ) +
                        ".pdf",

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

            GenererPdf(
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

    private void GenererPdf(
        string chemin)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        string periode =
            dtpDateDebut.Enabled
                ? $"Période : " +
                  $"{dtpDateDebut.Value:dd/MM/yyyy}" +
                  " au " +
                  $"{dtpDateFin.Value:dd/MM/yyyy}"
                : "Rapport général";

        string recherche =
            txtRecherche.Text.Trim();

        List<string> colonnes =
            new List<string>();

        foreach (
            DataColumn colonne
            in tableCourante.Columns)
        {
            colonnes.Add(
                colonne.ColumnName
            );
        }

        Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(
                    PageSizes.A4.Landscape()
                );

                page.Margin(22);

                page.DefaultTextStyle(
                    style =>
                        style.FontFamily("Arial")
                             .FontSize(8)
                );

                page.Header()
                    .Column(entete =>
                    {
                        entete.Item()
                            .Text(
                                titreRapportCourant.ToUpperInvariant()
                            )
                            .Bold()
                            .FontSize(18)
                            .FontColor(
                                Colors.Blue.Darken3
                            );

                        entete.Item()
                            .PaddingTop(4)
                            .Text(
                                periode +
                                "   |   Imprimé le : " +
                                $"{DateTime.Now:dd/MM/yyyy HH:mm}"
                            );

                        if (
                            !string.IsNullOrWhiteSpace(
                                recherche
                            ))
                        {
                            entete.Item()
                                .Text(
                                    "Recherche : " +
                                    recherche
                                );
                        }
                    });

                page.Content()
                    .PaddingVertical(10)
                    .Column(contenu =>
                    {
                        contenu.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(
                                    definition =>
                                    {
                                        foreach (
                                            string colonne
                                            in colonnes)
                                        {
                                            if (
                                                colonne == "N°")
                                            {
                                                definition
                                                    .ConstantColumn(
                                                        38
                                                    );
                                            }
                                            else
                                            {
                                                definition
                                                    .RelativeColumn();
                                            }
                                        }
                                    }
                                );

                                table.Header(header =>
                                {
                                    foreach (
                                        string colonne
                                        in colonnes)
                                    {
                                        header.Cell()
                                            .Element(
                                                StyleEntetePdf
                                            )
                                            .Text(colonne);
                                    }
                                });

                                foreach (
                                    DataRow ligne
                                    in tableCourante.Rows)
                                {
                                    foreach (
                                        string colonne
                                        in colonnes)
                                    {
                                        table.Cell()
                                            .Element(
                                                StyleCellulePdf
                                            )
                                            .Text(
                                                FormaterValeurPdf(
                                                    ligne[colonne],
                                                    colonne
                                                )
                                            );
                                    }
                                }
                            });

                        contenu.Item()
                            .PaddingTop(10)
                            .Text(lblResume.Text)
                            .Bold();
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

    private static string FormaterValeurPdf(
        object valeur,
        string colonne)
    {
        if (
            valeur == DBNull.Value ||
            valeur == null)
        {
            return "—";
        }

        if (valeur is DateTime date)
        {
            if (
                colonne.Contains(
                    "Date",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                return date.ToString(
                    "dd/MM/yyyy HH:mm"
                );
            }
        }

        if (
            EstColonneMontant(colonne) &&
            decimal.TryParse(
                Convert.ToString(
                    valeur,
                    CultureInfo.InvariantCulture
                ),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out decimal montant
            ))
        {
            return $"{montant:N2} DA";
        }

        return Convert.ToString(valeur) ?? "—";
    }

    private static string NettoyerNomFichier(
        string texte)
    {
        foreach (
            char caractere
            in System.IO.Path.GetInvalidFileNameChars())
        {
            texte = texte.Replace(
                caractere,
                '_'
            );
        }

        return texte.Replace(
            ' ',
            '_'
        );
    }

    private static IContainer StyleEntetePdf(
        IContainer container)
    {
        return container
            .Background(Colors.Blue.Darken3)
            .Border(1)
            .BorderColor(Colors.White)
            .Padding(5)
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
            .Padding(4)
            .AlignMiddle();
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
            Margin = new Padding(4)
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
