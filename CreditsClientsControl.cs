using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySqlConnector;

namespace GestionUsine;

public sealed class CreditsClientsControl : UserControl
{
    private readonly TextBox txtRecherche;
    private readonly NumericUpDown numVersement;
    private readonly TextBox txtMotif;

    private readonly Label lblClient;
    private readonly Label lblTotalCreditsClient;
    private readonly Label lblTotalVerseClient;
    private readonly Label lblResteGlobalClient;
    private readonly Label lblTotalGlobal;

    private readonly DataGridView dgvCreditsClients;
    private readonly DataGridView dgvVersements;

    private int clientSelectionneId;
    private decimal resteGlobalSelectionne;

    public CreditsClientsControl()
    {
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        var structure = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 62)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 82)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 55)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 225)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 45)
        );

        Controls.Add(structure);

        var titre = new Label
        {
            Text = "Gestion des crédits clients",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                22,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        structure.Controls.Add(
            titre,
            0,
            0
        );

        var barreRecherche = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 8, 12, 8),
            BorderStyle = BorderStyle.FixedSingle
        };

        structure.Controls.Add(
            barreRecherche,
            0,
            1
        );

        var dispositionRecherche =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        dispositionRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                125
            )
        );

        dispositionRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                45
            )
        );

        dispositionRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Absolute,
                155
            )
        );

        dispositionRecherche.ColumnStyles.Add(
            new ColumnStyle(
                SizeType.Percent,
                55
            )
        );

        dispositionRecherche.RowStyles.Add(
            new RowStyle(
                SizeType.Percent,
                100
            )
        );

        barreRecherche.Controls.Add(
            dispositionRecherche
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

        dispositionRecherche.Controls.Add(
            lblRecherche,
            0,
            0
        );

        var cadreRecherche = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            BorderStyle =
                BorderStyle.FixedSingle,
            Padding =
                new Padding(10, 5, 10, 5),
            Margin = new Padding(5)
        };

        txtRecherche = new TextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            Multiline = true,
            WordWrap = false,
            AcceptsReturn = false,
            Font = new Font("Segoe UI", 11),
            ForeColor = Color.FromArgb(25, 25, 25),
            BackColor = Color.White,
            TextAlign = HorizontalAlignment.Left,
            RightToLeft = RightToLeft.No,
            Margin = Padding.Empty
        };

        txtRecherche.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        };

        txtRecherche.TextChanged += (_, _) =>
        {
            ChargerCreditsClients();
        };

        cadreRecherche.Controls.Add(
            txtRecherche
        );

        dispositionRecherche.Controls.Add(
            cadreRecherche,
            1,
            0
        );

        var btnActualiser = CreerBouton(
            "Actualiser",
            Color.FromArgb(32, 113, 171)
        );

        btnActualiser.Font = new Font(
            "Segoe UI",
            10,
            FontStyle.Bold
        );

        btnActualiser.Margin =
            new Padding(5, 6, 5, 6);

        btnActualiser.Click += (_, _) =>
        {
            txtRecherche.Clear();
            ChargerCreditsClients();
            ChargerVersements();
            txtRecherche.Focus();
        };

        dispositionRecherche.Controls.Add(
            btnActualiser,
            2,
            0
        );

        lblTotalGlobal = new Label
        {
            Text =
                "TOUS LES CLIENTS\n" +
                "Total crédits : 0,00 DA   |   Reste : 0,00 DA",
            Dock = DockStyle.Fill,
            AutoSize = false,
            AutoEllipsis = false,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(190, 58, 58),
            TextAlign =
                ContentAlignment.MiddleCenter,
            Margin =
                new Padding(10, 2, 5, 2)
        };

        dispositionRecherche.Controls.Add(
            lblTotalGlobal,
            3,
            0
        );

        dgvCreditsClients = CreerTableau();
        ConfigurerTableCreditsClients();

        dgvCreditsClients.CellClick +=
            SelectionnerClient;

        structure.Controls.Add(
            dgvCreditsClients,
            0,
            2
        );

        var carteVersement = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(18),
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 10, 0, 10)
        };

        structure.Controls.Add(
            carteVersement,
            0,
            3
        );

        var formulaire = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 5,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 25)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 18)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 18)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 23)
        );

        formulaire.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 16)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 35)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65)
        );

        formulaire.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carteVersement.Controls.Add(
            formulaire
        );

        formulaire.Controls.Add(
            CreerLabel("Client"),
            0,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Total crédits"),
            1,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Total versé"),
            2,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Reste global"),
            3,
            0
        );

        formulaire.Controls.Add(
            CreerLabel("Action"),
            4,
            0
        );

        lblClient = CreerValeur("-");
        lblTotalCreditsClient =
            CreerValeur("0,00 DA");
        lblTotalVerseClient =
            CreerValeur("0,00 DA");
        lblResteGlobalClient =
            CreerValeur("0,00 DA");

        var zoneAction =
            new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                Margin = Padding.Empty,
                Padding = Padding.Empty
            };

        zoneAction.RowStyles.Add(
            new RowStyle(SizeType.Percent, 50)
        );

        zoneAction.RowStyles.Add(
            new RowStyle(SizeType.Percent, 50)
        );

        numVersement = new NumericUpDown
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
            Margin = new Padding(5, 0, 5, 3)
        };

        var btnVersement = CreerBouton(
            "Enregistrer",
            Color.FromArgb(38, 138, 83)
        );

        btnVersement.Margin =
            new Padding(5, 3, 5, 0);

        btnVersement.Click +=
            EnregistrerVersementClient;

        zoneAction.Controls.Add(
            numVersement,
            0,
            0
        );

        zoneAction.Controls.Add(
            btnVersement,
            0,
            1
        );

        formulaire.Controls.Add(
            lblClient,
            0,
            1
        );

        formulaire.Controls.Add(
            lblTotalCreditsClient,
            1,
            1
        );

        formulaire.Controls.Add(
            lblTotalVerseClient,
            2,
            1
        );

        formulaire.Controls.Add(
            lblResteGlobalClient,
            3,
            1
        );

        formulaire.Controls.Add(
            zoneAction,
            4,
            1
        );

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

        formulaire.Controls.Add(
            txtMotif,
            1,
            2
        );

        formulaire.SetColumnSpan(
            txtMotif,
            4
        );

        dgvVersements = CreerTableau();
        ConfigurerTableVersements();

        structure.Controls.Add(
            dgvVersements,
            0,
            4
        );

        ChargerCreditsClients();
    }

    private void ConfigurerTableCreditsClients()
    {
        dgvCreditsClients.AutoGenerateColumns = false;

        dgvCreditsClients.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "client_id",
                DataPropertyName = "client_id",
                Visible = false
            }
        );

        dgvCreditsClients.Columns.Add(
            CreerColonne(
                "client",
                "client",
                "Client",
                300,
                true
            )
        );

        dgvCreditsClients.Columns.Add(
            CreerColonne(
                "nombre_credits",
                "nombre_credits",
                "Nombre de crédits",
                215
            )
        );

        dgvCreditsClients.Columns.Add(
            CreerColonneMontant(
                "total_credits",
                "total_credits",
                "Total crédits"
            )
        );

        dgvCreditsClients.Columns.Add(
            CreerColonneMontant(
                "total_verse",
                "total_verse",
                "Total versé"
            )
        );

        dgvCreditsClients.Columns.Add(
            CreerColonneMontant(
                "reste_global",
                "reste_global",
                "Reste global"
            )
        );

        dgvCreditsClients.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "dernier_versement",
                DataPropertyName =
                    "dernier_versement",
                HeaderText =
                    "Dernier versement",
                Width = 195,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format =
                            "dd/MM/yyyy HH:mm"
                    }
            }
        );
    }

    private void ConfigurerTableVersements()
    {
        dgvVersements.AutoGenerateColumns = false;

        dgvVersements.Columns.Add(
            CreerColonne(
                "id",
                "id",
                "N°",
                70
            )
        );

        dgvVersements.Columns.Add(
            CreerColonne(
                "client",
                "client",
                "Client",
                230,
                true
            )
        );

        dgvVersements.Columns.Add(
            CreerColonne(
                "credit_id",
                "credit_id",
                "N° crédit",
                100
            )
        );

        dgvVersements.Columns.Add(
            CreerColonne(
                "vente_id",
                "vente_id",
                "N° vente",
                100
            )
        );

        dgvVersements.Columns.Add(
            CreerColonneMontant(
                "montant",
                "montant",
                "Versement"
            )
        );

        dgvVersements.Columns.Add(
            CreerColonneMontant(
                "ancien_reste",
                "ancien_reste",
                "Ancien reste"
            )
        );

        dgvVersements.Columns.Add(
            CreerColonneMontant(
                "nouveau_reste",
                "nouveau_reste",
                "Nouveau reste"
            )
        );

        dgvVersements.Columns.Add(
            CreerColonne(
                "motif",
                "motif",
                "Motif",
                220,
                true
            )
        );

        dgvVersements.Columns.Add(
            new DataGridViewTextBoxColumn
            {
                Name = "date_versement",
                DataPropertyName =
                    "date_versement",
                HeaderText = "Date",
                Width = 170,
                DefaultCellStyle =
                    new DataGridViewCellStyle
                    {
                        Format =
                            "dd/MM/yyyy HH:mm"
                    }
            }
        );
    }

    private void ChargerCreditsClients(
        int? clientIdASelectionner = null)
    {
        try
        {
            bool rechercheAvaitFocus =
                txtRecherche.Focused;

            int positionCurseur =
                txtRecherche.SelectionStart;

            string recherche =
                txtRecherche.Text.Trim();

            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT
                    cr.client_id,
                    c.nom AS client,

                    SUM(
                        CASE
                            WHEN cr.statut <> 'ANNULE'
                                THEN 1
                            ELSE 0
                        END
                    ) AS nombre_credits,

                    COALESCE(
                        SUM(
                            CASE
                                WHEN cr.statut <> 'ANNULE'
                                    THEN cr.montant_total
                                ELSE 0
                            END
                        ),
                        0
                    ) AS total_credits,

                    COALESCE(
                        SUM(
                            CASE
                                WHEN cr.statut <> 'ANNULE'
                                    THEN cr.montant_paye
                                ELSE 0
                            END
                        ),
                        0
                    ) AS total_verse,

                    COALESCE(
                        SUM(
                            CASE
                                WHEN cr.statut = 'OUVERT'
                                    THEN cr.reste_a_payer
                                ELSE 0
                            END
                        ),
                        0
                    ) AS reste_global,

                    MAX(
                        cr.date_dernier_versement
                    ) AS dernier_versement

                FROM credits_clients cr

                INNER JOIN clients c
                    ON c.id = cr.client_id

                WHERE
                    (
                        @recherche = ''
                        OR c.nom LIKE
                            CONCAT(
                                '%',
                                @recherche,
                                '%'
                            )
                    )

                GROUP BY
                    cr.client_id,
                    c.nom

                HAVING
                    SUM(
                        CASE
                            WHEN cr.statut <> 'ANNULE'
                                THEN 1
                            ELSE 0
                        END
                    ) > 0

                ORDER BY
                    reste_global DESC,
                    c.nom;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            command.Parameters.AddWithValue(
                "@recherche",
                recherche
            );

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(
                    command
                );

            var table = new DataTable();
            adapter.Fill(table);

            dgvCreditsClients.DataSource =
                table;

            decimal totalCredits = 0;
            decimal resteGlobal = 0;

            foreach (DataRow row in table.Rows)
            {
                totalCredits +=
                    Convert.ToDecimal(
                        row["total_credits"]
                    );

                resteGlobal +=
                    Convert.ToDecimal(
                        row["reste_global"]
                    );
            }

            string prefixe =
                string.IsNullOrWhiteSpace(
                    recherche
                )
                    ? "Tous les clients"
                    : "Résultat recherché";

            lblTotalGlobal.Text =
                prefixe.ToUpperInvariant() +
                "\n" +
                $"Total crédits : {totalCredits:N2} DA" +
                $"   |   Reste : {resteGlobal:N2} DA";

            dgvCreditsClients.ClearSelection();

            if (clientIdASelectionner.HasValue)
            {
                SelectionnerClientParId(
                    clientIdASelectionner.Value
                );
            }
            else if (dgvCreditsClients.Rows.Count > 0)
            {
                DataGridViewRow premiereLigne =
                    dgvCreditsClients.Rows[0];

                premiereLigne.Selected = true;

                dgvCreditsClients.CurrentCell =
                    premiereLigne.Cells["client"];

                AppliquerSelectionClient(
                    premiereLigne
                );
            }
            else
            {
                ViderSelectionClient();
                ChargerVersements();
            }

            if (rechercheAvaitFocus)
            {
                BeginInvoke(
                    new Action(() =>
                    {
                        txtRecherche.Focus();

                        txtRecherche.SelectionStart =
                            Math.Min(
                                positionCurseur,
                                txtRecherche.TextLength
                            );

                        txtRecherche.SelectionLength =
                            0;
                    })
                );
            }
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger les crédits clients.",
                ex
            );
        }
    }

    private void ChargerVersements()
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            string sql = """
                SELECT
                    v.id,
                    v.credit_id,
                    cr.vente_id,
                    c.nom AS client,
                    v.montant,
                    v.ancien_reste,
                    v.nouveau_reste,
                    v.motif,
                    v.date_versement

                FROM versements_clients v

                INNER JOIN credits_clients cr
                    ON cr.id = v.credit_id

                INNER JOIN clients c
                    ON c.id = v.client_id
                """;

            if (clientSelectionneId > 0)
            {
                sql += """

                    WHERE v.client_id = @clientId
                    """;
            }

            sql += """

                ORDER BY
                    v.date_versement DESC,
                    v.id DESC

                LIMIT 500;
                """;

            using MySqlCommand command =
                new MySqlCommand(
                    sql,
                    connection
                );

            if (clientSelectionneId > 0)
            {
                command.Parameters.AddWithValue(
                    "@clientId",
                    clientSelectionneId
                );
            }

            using MySqlDataAdapter adapter =
                new MySqlDataAdapter(
                    command
                );

            var table = new DataTable();
            adapter.Fill(table);

            dgvVersements.DataSource =
                table;
        }
        catch (Exception ex)
        {
            AfficherErreur(
                "Impossible de charger les versements.",
                ex
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

        AppliquerSelectionClient(
            dgvCreditsClients.Rows[e.RowIndex]
        );
    }

    private void AppliquerSelectionClient(
        DataGridViewRow ligne)
    {
        if (
            ligne.DataBoundItem
            is not DataRowView donnees)
        {
            return;
        }

        clientSelectionneId =
            Convert.ToInt32(
                donnees["client_id"]
            );

        resteGlobalSelectionne =
            Convert.ToDecimal(
                donnees["reste_global"]
            );

        lblClient.Text =
            Convert.ToString(
                donnees["client"]
            ) ?? "-";

        lblTotalCreditsClient.Text =
            $"{Convert.ToDecimal(donnees["total_credits"]):N2} DA";

        lblTotalVerseClient.Text =
            $"{Convert.ToDecimal(donnees["total_verse"]):N2} DA";

        lblResteGlobalClient.Text =
            $"{resteGlobalSelectionne:N2} DA";

        numVersement.Maximum =
            Math.Max(
                resteGlobalSelectionne,
                0
            );

        numVersement.Value = 0;

        ChargerVersements();
    }

    private void SelectionnerClientParId(
        int clientId)
    {
        foreach (
            DataGridViewRow ligne
            in dgvCreditsClients.Rows)
        {
            int id =
                Convert.ToInt32(
                    ligne.Cells[
                        "client_id"
                    ].Value
                );

            if (id != clientId)
            {
                continue;
            }

            ligne.Selected = true;

            dgvCreditsClients.CurrentCell =
                ligne.Cells["client"];

            AppliquerSelectionClient(
                ligne
            );

            return;
        }

        ViderSelectionClient();
    }

    private void ViderSelectionClient()
    {
        clientSelectionneId = 0;
        resteGlobalSelectionne = 0;

        lblClient.Text = "-";
        lblTotalCreditsClient.Text =
            "0,00 DA";
        lblTotalVerseClient.Text =
            "0,00 DA";
        lblResteGlobalClient.Text =
            "0,00 DA";

        numVersement.Maximum =
            1_000_000_000;

        numVersement.Value = 0;
    }

    private void EnregistrerVersementClient(
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

        decimal montantTotal =
            numVersement.Value;

        if (montantTotal <= 0)
        {
            MessageBox.Show(
                "Le versement doit être supérieur à zéro.",
                "Montant incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        if (
            montantTotal >
            resteGlobalSelectionne)
        {
            MessageBox.Show(
                "Le versement ne peut pas dépasser " +
                "le reste global du client.\n\n" +
                $"Reste global : {resteGlobalSelectionne:N2} DA",
                "Montant incorrect",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            return;
        }

        string nomClient =
            lblClient.Text;

        DialogResult confirmation =
            MessageBox.Show(
                "Confirmer ce versement global ?\n\n" +
                $"Client : {nomClient}\n" +
                $"Versement : {montantTotal:N2} DA\n" +
                $"Reste avant : {resteGlobalSelectionne:N2} DA\n" +
                $"Reste après : " +
                $"{resteGlobalSelectionne - montantTotal:N2} DA\n\n" +
                "Le montant sera automatiquement réparti " +
                "sur les crédits les plus anciens.",
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

            using MySqlTransaction transaction =
                connection.BeginTransaction();

            try
            {
                var credits =
                    new List<CreditOuvert>();

                const string sqlLecture = """
                    SELECT
                        id,
                        vente_id,
                        montant_paye,
                        reste_a_payer

                    FROM credits_clients

                    WHERE client_id = @clientId
                      AND statut = 'OUVERT'
                      AND reste_a_payer > 0

                    ORDER BY
                        date_creation ASC,
                        id ASC

                    FOR UPDATE;
                    """;

                using (
                    MySqlCommand command =
                        new MySqlCommand(
                            sqlLecture,
                            connection,
                            transaction
                        )
                )
                {
                    command.Parameters.AddWithValue(
                        "@clientId",
                        clientSelectionneId
                    );

                    using MySqlDataReader reader =
                        command.ExecuteReader();

                    while (reader.Read())
                    {
                        credits.Add(
                            new CreditOuvert
                            {
                                CreditId =
                                    reader.GetInt32(
                                        "id"
                                    ),

                                VenteId =
                                    reader.GetInt32(
                                        "vente_id"
                                    ),

                                MontantPaye =
                                    reader.GetDecimal(
                                        "montant_paye"
                                    ),

                                Reste =
                                    reader.GetDecimal(
                                        "reste_a_payer"
                                    )
                            }
                        );
                    }
                }

                if (credits.Count == 0)
                {
                    throw new InvalidOperationException(
                        "Ce client n’a aucun crédit ouvert."
                    );
                }

                decimal montantRestant =
                    montantTotal;

                int premierVersementId = 0;

                foreach (
                    CreditOuvert credit
                    in credits)
                {
                    if (montantRestant <= 0)
                    {
                        break;
                    }

                    decimal part =
                        Math.Min(
                            montantRestant,
                            credit.Reste
                        );

                    decimal nouveauReste =
                        credit.Reste - part;

                    decimal nouveauMontantPaye =
                        credit.MontantPaye + part;

                    string nouveauStatut =
                        nouveauReste == 0
                            ? "FERME"
                            : "OUVERT";

                    const string sqlCredit = """
                        UPDATE credits_clients
                        SET
                            montant_paye =
                                @montantPaye,
                            reste_a_payer =
                                @nouveauReste,
                            statut =
                                @statut,
                            date_dernier_versement =
                                CURRENT_TIMESTAMP
                        WHERE id = @creditId;
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
                            "@montantPaye",
                            nouveauMontantPaye
                        );

                        command.Parameters.AddWithValue(
                            "@nouveauReste",
                            nouveauReste
                        );

                        command.Parameters.AddWithValue(
                            "@statut",
                            nouveauStatut
                        );

                        command.Parameters.AddWithValue(
                            "@creditId",
                            credit.CreditId
                        );

                        command.ExecuteNonQuery();
                    }

                    const string sqlVente = """
                        UPDATE ventes
                        SET
                            montant_paye =
                                montant_paye + @montant,
                            reste_a_payer =
                                @nouveauReste
                        WHERE id = @venteId;
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
                            "@montant",
                            part
                        );

                        command.Parameters.AddWithValue(
                            "@nouveauReste",
                            nouveauReste
                        );

                        command.Parameters.AddWithValue(
                            "@venteId",
                            credit.VenteId
                        );

                        command.ExecuteNonQuery();
                    }

                    const string sqlVersement = """
                        INSERT INTO versements_clients
                        (
                            credit_id,
                            client_id,
                            montant,
                            ancien_reste,
                            nouveau_reste,
                            motif
                        )
                        VALUES
                        (
                            @creditId,
                            @clientId,
                            @montant,
                            @ancienReste,
                            @nouveauReste,
                            @motif
                        );
                        """;

                    using (
                        MySqlCommand command =
                            new MySqlCommand(
                                sqlVersement,
                                connection,
                                transaction
                            )
                    )
                    {
                        command.Parameters.AddWithValue(
                            "@creditId",
                            credit.CreditId
                        );

                        command.Parameters.AddWithValue(
                            "@clientId",
                            clientSelectionneId
                        );

                        command.Parameters.AddWithValue(
                            "@montant",
                            part
                        );

                        command.Parameters.AddWithValue(
                            "@ancienReste",
                            credit.Reste
                        );

                        command.Parameters.AddWithValue(
                            "@nouveauReste",
                            nouveauReste
                        );

                        string motif =
                            string.IsNullOrWhiteSpace(
                                txtMotif.Text
                            )
                                ? "Versement global client"
                                : txtMotif.Text.Trim();

                        command.Parameters.AddWithValue(
                            "@motif",
                            motif
                        );

                        command.ExecuteNonQuery();

                        if (premierVersementId == 0)
                        {
                            premierVersementId =
                                Convert.ToInt32(
                                    command.LastInsertedId
                                );
                        }
                    }

                    montantRestant -= part;
                }

                if (montantRestant > 0)
                {
                    throw new InvalidOperationException(
                        "Le montant n’a pas pu être entièrement " +
                        "réparti sur les crédits ouverts."
                    );
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
                        'VERSEMENT_CLIENT',
                        @montant,
                        @motif,
                        'VERSEMENT_CLIENT',
                        @versementId
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
                        montantTotal
                    );

                    command.Parameters.AddWithValue(
                        "@motif",
                        $"Versement global client - {nomClient}"
                    );

                    command.Parameters.AddWithValue(
                        "@versementId",
                        premierVersementId
                    );

                    command.ExecuteNonQuery();
                }

                transaction.Commit();

                decimal nouveauResteGlobal =
                    resteGlobalSelectionne -
                    montantTotal;

                MessageBox.Show(
                    "Le versement global a été enregistré.\n\n" +
                    $"Client : {nomClient}\n" +
                    $"Versement : {montantTotal:N2} DA\n" +
                    $"Nouveau reste global : {nouveauResteGlobal:N2} DA",
                    "Versement réussi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                txtMotif.Clear();

                int clientId =
                    clientSelectionneId;

                ChargerCreditsClients(
                    clientId
                );

                ChargerVersements();
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
                "Impossible d’enregistrer le versement.",
                ex
            );
        }
    }

    private sealed class CreditOuvert
    {
        public int CreditId { get; init; }
        public int VenteId { get; init; }
        public decimal MontantPaye { get; init; }
        public decimal Reste { get; init; }
    }

    private static DataGridView CreerTableau()
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
            ColumnHeadersHeight = 48,
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

        tableau.ColumnHeadersDefaultCellStyle.WrapMode =
            DataGridViewTriState.False;

        tableau.EnableHeadersVisualStyles = false;

        return tableau;
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
            Width = 175,
            DefaultCellStyle =
                new DataGridViewCellStyle
                {
                    Alignment =
                        DataGridViewContentAlignment
                            .MiddleRight,
                    Format = "N2"
                }
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
            ForeColor =
                Color.FromArgb(50, 60, 70),
            TextAlign =
                ContentAlignment.BottomLeft,
            Margin = new Padding(5)
        };
    }

    private static Label CreerValeur(
        string texte)
    {
        return new Label
        {
            Text = texte,
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                13,
                FontStyle.Bold
            ),
            ForeColor =
                Color.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft,
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
