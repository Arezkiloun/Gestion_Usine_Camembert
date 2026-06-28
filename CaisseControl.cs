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

public sealed class CaisseControl : UserControl
{
    private DateTimePicker dtpDateDebut = null!;
    private DateTimePicker dtpDateFin = null!;
    private ComboBox cmbSens = null!;
    private ComboBox cmbType = null!;
    private TextBox txtRecherche = null!;

    private Label lblSoldeGlobal = null!;
    private Label lblTotalEntrees = null!;
    private Label lblTotalSorties = null!;
    private Label lblSoldePeriode = null!;

    private DataGridView dgvMouvements = null!;

    public CaisseControl()
    {
        Dock = DockStyle.Fill;
        BackColor =
            DrawingColor.FromArgb(242, 245, 248);
        Padding = new Padding(5);
        AutoScaleMode = AutoScaleMode.Dpi;

        Controls.Add(CreerInterface());

        ChargerTypesMouvements();
        ChargerMouvements();
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
            new RowStyle(SizeType.Absolute, 125)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 120)
        );

        structure.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        var titre = new Label
        {
            Text = "Gestion de la caisse",
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
            CreerCartesResume(),
            0,
            1
        );

        structure.Controls.Add(
            CreerCarteFiltres(),
            0,
            2
        );

        dgvMouvements =
            CreerTableauMouvements();

        structure.Controls.Add(
            dgvMouvements,
            0,
            3
        );

        return structure;
    }

    private Control CreerCartesResume()
    {
        var zone = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 4,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        for (int index = 0; index < 4; index++)
        {
            zone.ColumnStyles.Add(
                new ColumnStyle(
                    SizeType.Percent,
                    25
                )
            );
        }

        lblSoldeGlobal =
            CreerValeurResume();

        lblTotalEntrees =
            CreerValeurResume();

        lblTotalSorties =
            CreerValeurResume();

        lblSoldePeriode =
            CreerValeurResume();

        zone.Controls.Add(
            CreerCarteResume(
                "Solde global",
                lblSoldeGlobal
            ),
            0,
            0
        );

        zone.Controls.Add(
            CreerCarteResume(
                "Entrées de la période",
                lblTotalEntrees
            ),
            1,
            0
        );

        zone.Controls.Add(
            CreerCarteResume(
                "Sorties de la période",
                lblTotalSorties
            ),
            2,
            0
        );

        zone.Controls.Add(
            CreerCarteResume(
                "Solde de la période",
                lblSoldePeriode
            ),
            3,
            0
        );

        return zone;
    }

    private static Control CreerCarteResume(
        string titre,
        Label valeur)
    {
        var carte = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = DrawingColor.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(15),
            Margin = new Padding(5, 0, 5, 10)
        };

        var lblTitre = new Label
        {
            Text = titre,
            Dock = DockStyle.Top,
            Height = 34,
            Font = new Font(
                "Segoe UI",
                10,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(85, 95, 105),
            TextAlign =
                ContentAlignment.MiddleLeft
        };

        carte.Controls.Add(valeur);
        carte.Controls.Add(lblTitre);

        return carte;
    }

    private static Label CreerValeurResume()
    {
        return new Label
        {
            Text = "0,00 DA",
            Dock = DockStyle.Fill,
            Font = new Font(
                "Segoe UI",
                17,
                FontStyle.Bold
            ),
            ForeColor =
                DrawingColor.FromArgb(25, 49, 78),
            TextAlign =
                ContentAlignment.MiddleLeft
        };
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
            ColumnCount = 8,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 13)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 13)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 11)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 17)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 20)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 10)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 8)
        );

        filtre.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 8)
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

        cmbSens = new ComboBox
        {
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10)
        };

        cmbSens.Items.AddRange(
            new object[]
            {
                "Tous",
                "Entrées",
                "Sorties"
            }
        );

        cmbSens.SelectedIndex = 0;

        cmbType = new ComboBox
        {
            DropDownStyle =
                ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10)
        };

        txtRecherche = new TextBox
        {
            Font = new Font("Segoe UI", 10),
            PlaceholderText =
                "Motif, type ou référence"
        };

        txtRecherche.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                ChargerMouvements();
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
            "PDF",
            DrawingColor.FromArgb(93, 63, 137)
        );

        btnAfficher.Click += (_, _) =>
        {
            ChargerMouvements();
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
            cmbSens.SelectedIndex = 0;
            cmbType.SelectedIndex = 0;
            txtRecherche.Clear();

            ChargerMouvements();
        };

        btnPdf.Click += ImprimerHistoriquePdf;

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
                "Sens",
                cmbSens
            ),
            2,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Type",
                cmbType
            ),
            3,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Recherche",
                txtRecherche
            ),
            4,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Filtrer",
                btnAfficher
            ),
            5,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Période",
                btnCeMois
            ),
            6,
            0
        );

        filtre.Controls.Add(
            CreerBlocFiltre(
                "Imprimer",
                btnPdf
            ),
            7,
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
            Margin = new Padding(4, 0, 4, 0),
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

        champ.Dock = DockStyle.Fill;
        champ.Margin = new Padding(0, 3, 0, 3);

        bloc.Controls.Add(label, 0, 0);
        bloc.Controls.Add(champ, 0, 1);

        return bloc;
    }

    private DataGridView CreerTableauMouvements()
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
                Name = "date_mouvement",
                DataPropertyName = "date_mouvement",
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
                "sens_affiche",
                "sens_affiche",
                "Sens",
                100
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "type_affiche",
                "type_affiche",
                "Type",
                190
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
                360,
                true
            )
        );

        tableau.Columns.Add(
            CreerColonne(
                "reference",
                "reference",
                "Référence",
                170
            )
        );

        tableau.CellFormatting +=
            ColorerMouvement;

        return tableau;
    }

    private void ChargerTypesMouvements()
    {
        try
        {
            using MySqlConnection connection =
                Database.CreateConnection();

            connection.Open();

            const string sql = """
                SELECT DISTINCT type_mouvement
                FROM mouvements_caisse
                WHERE type_mouvement IS NOT NULL
                  AND type_mouvement <> ''
                  AND type_mouvement NOT IN
                  (
                      'ANNULATION_VENTE',
                      'ANNULATION_DEPENSE',
                      'ANNULATION_RECETTE'
                  )
                ORDER BY type_mouvement;
                """;

            using MySqlCommand command =
                new MySqlCommand(sql, connection);

            using MySqlDataReader reader =
                command.ExecuteReader();

            cmbType.Items.Clear();
            cmbType.Items.Add("Tous les types");

            while (reader.Read())
            {
                cmbType.Items.Add(
                    reader.GetString(0)
                );
            }

            cmbType.SelectedIndex = 0;
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger les types de mouvements.",
                ex
            );
        }
    }

    private void ChargerMouvements()
    {
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
                    date_mouvement,
                    sens,
                    type_mouvement,
                    montant,
                    motif,

                    CASE sens
                        WHEN 'ENTREE' THEN 'Entrée'
                        WHEN 'SORTIE' THEN 'Sortie'
                        ELSE sens
                    END AS sens_affiche,

                    CASE type_mouvement
                        WHEN 'VENTE'
                            THEN 'Vente'
                        WHEN 'VERSEMENT_CLIENT'
                            THEN 'Versement client'
                        WHEN 'RECETTE'
                            THEN 'Recette'
                        WHEN 'DEPENSE'
                            THEN 'Dépense'
                        ELSE REPLACE(
                            type_mouvement,
                            '_',
                            ' '
                        )
                    END AS type_affiche,

                    CASE
                        WHEN reference_type IS NULL
                            OR reference_type = ''
                            THEN '—'
                        WHEN reference_id IS NULL
                            THEN reference_type
                        ELSE CONCAT(
                            reference_type,
                            ' n° ',
                            reference_id
                        )
                    END AS reference

                FROM mouvements_caisse

                WHERE date_mouvement >= @dateDebut
                  AND date_mouvement < @dateFinExclusive

                  AND type_mouvement NOT IN
                  (
                      'ANNULATION_VENTE',
                      'ANNULATION_DEPENSE',
                      'ANNULATION_RECETTE'
                  )

                  AND NOT
                  (
                      type_mouvement = 'VENTE'
                      AND reference_type = 'VENTE'
                      AND EXISTS
                      (
                          SELECT 1
                          FROM ventes v
                          WHERE v.id = reference_id
                            AND v.statut = 'ANNULEE'
                      )
                  )

                  AND NOT
                  (
                      type_mouvement = 'DEPENSE'
                      AND reference_type = 'DEPENSE'
                      AND EXISTS
                      (
                          SELECT 1
                          FROM depenses d
                          WHERE d.id = reference_id
                            AND d.statut = 'ANNULEE'
                      )
                  )

                  AND NOT
                  (
                      type_mouvement = 'RECETTE'
                      AND reference_type = 'RECETTE'
                      AND EXISTS
                      (
                          SELECT 1
                          FROM recettes r
                          WHERE r.id = reference_id
                            AND r.statut = 'ANNULEE'
                      )
                  )
                """;

            string sens =
                cmbSens.SelectedIndex switch
                {
                    1 => "ENTREE",
                    2 => "SORTIE",
                    _ => ""
                };

            string type =
                cmbType.SelectedIndex > 0
                    ? cmbType.Text.Trim()
                    : "";

            string recherche =
                txtRecherche.Text.Trim();

            if (!string.IsNullOrWhiteSpace(sens))
            {
                sql += """

                    AND sens = @sens
                    """;
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                sql += """

                    AND type_mouvement = @type
                    """;
            }

            if (!string.IsNullOrWhiteSpace(recherche))
            {
                sql += """

                    AND
                    (
                        motif LIKE @recherche
                        OR type_mouvement LIKE @recherche
                        OR reference_type LIKE @recherche
                        OR CAST(reference_id AS CHAR)
                            LIKE @recherche
                        OR CAST(id AS CHAR)
                            LIKE @recherche
                    )
                    """;
            }

            sql += """

                ORDER BY
                    date_mouvement DESC,
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

            if (!string.IsNullOrWhiteSpace(sens))
            {
                command.Parameters.AddWithValue(
                    "@sens",
                    sens
                );
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                command.Parameters.AddWithValue(
                    "@type",
                    type
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

            var table = new DataTable();
            adapter.Fill(table);

            dgvMouvements.DataSource = table;
            dgvMouvements.ClearSelection();

            ActualiserResume(connection);
        }
        catch (MySqlException ex)
        {
            AfficherErreur(
                "Impossible de charger les mouvements de caisse.",
                ex
            );
        }
    }

    private void ActualiserResume(
        MySqlConnection connection)
    {
        // Le solde global utilise tous les mouvements,
        // y compris les contre-mouvements d'annulation.
        // Les annulations restent invisibles dans le tableau,
        // mais leur effet financier est conservé.
        const string sqlSolde = """
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

        using MySqlCommand soldeCommand =
            new MySqlCommand(
                sqlSolde,
                connection
            );

        decimal soldeGlobal =
            Convert.ToDecimal(
                soldeCommand.ExecuteScalar()
            );

        decimal totalEntrees = 0;
        decimal totalSorties = 0;

        foreach (
            DataGridViewRow ligne
            in dgvMouvements.Rows)
        {
            string sens =
                Convert.ToString(
                    ligne.Cells["sens_affiche"].Value
                ) ?? "";

            decimal montant =
                Convert.ToDecimal(
                    ligne.Cells["montant"].Value
                );

            if (
                sens.Equals(
                    "Entrée",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                totalEntrees += montant;
            }
            else if (
                sens.Equals(
                    "Sortie",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                totalSorties += montant;
            }
        }

        decimal soldePeriode =
            totalEntrees - totalSorties;

        lblSoldeGlobal.Text =
            $"{soldeGlobal:N2} DA";

        lblTotalEntrees.Text =
            $"+ {totalEntrees:N2} DA";

        lblTotalSorties.Text =
            $"- {totalSorties:N2} DA";

        lblSoldePeriode.Text =
            $"{soldePeriode:N2} DA";

        lblSoldeGlobal.ForeColor =
            soldeGlobal < 0
                ? DrawingColor.FromArgb(190, 58, 58)
                : DrawingColor.FromArgb(25, 49, 78);

        lblTotalEntrees.ForeColor =
            DrawingColor.FromArgb(38, 138, 83);

        lblTotalSorties.ForeColor =
            DrawingColor.FromArgb(190, 58, 58);

        lblSoldePeriode.ForeColor =
            soldePeriode < 0
                ? DrawingColor.FromArgb(190, 58, 58)
                : DrawingColor.FromArgb(25, 49, 78);
    }

    private static void ColorerMouvement(
        object? sender,
        DataGridViewCellFormattingEventArgs e)
    {
        if (
            sender is not DataGridView tableau ||
            e.RowIndex < 0)
        {
            return;
        }

        string sens =
            Convert.ToString(
                tableau.Rows[e.RowIndex]
                    .Cells["sens_affiche"]
                    .Value
            ) ?? "";

        DataGridViewRow ligne =
            tableau.Rows[e.RowIndex];

        if (
            sens.Equals(
                "Entrée",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            ligne.DefaultCellStyle.BackColor =
                DrawingColor.FromArgb(
                    236,
                    248,
                    241
                );

            ligne.DefaultCellStyle.ForeColor =
                DrawingColor.FromArgb(
                    25,
                    105,
                    65
                );
        }
        else
        {
            ligne.DefaultCellStyle.BackColor =
                DrawingColor.FromArgb(
                    253,
                    239,
                    239
                );

            ligne.DefaultCellStyle.ForeColor =
                DrawingColor.FromArgb(
                    150,
                    45,
                    45
                );
        }
    }

    private sealed class LigneCaissePdf
    {
        public int Id { get; init; }
        public DateTime DateMouvement { get; init; }
        public string Sens { get; init; } = "";
        public string Type { get; init; } = "";
        public decimal Montant { get; init; }
        public string Motif { get; init; } = "";
        public string Reference { get; init; } = "";
    }

    private void ImprimerHistoriquePdf(
        object? sender,
        EventArgs e)
    {
        if (dgvMouvements.Rows.Count == 0)
        {
            MessageBox.Show(
                "Aucun mouvement n’est affiché pour cette période.",
                "PDF impossible",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            return;
        }

        try
        {
            List<LigneCaissePdf> lignes =
                ObtenirLignesPdf();

            using var dialogue =
                new SaveFileDialog
                {
                    Title =
                        "Enregistrer l’historique de caisse",

                    Filter =
                        "Fichier PDF (*.pdf)|*.pdf",

                    FileName =
                        $"Caisse_" +
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

            GenererPdfCaisse(
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

    private List<LigneCaissePdf> ObtenirLignesPdf()
    {
        var lignes =
            new List<LigneCaissePdf>();

        foreach (
            DataGridViewRow ligne
            in dgvMouvements.Rows)
        {
            if (ligne.IsNewRow)
            {
                continue;
            }

            lignes.Add(
                new LigneCaissePdf
                {
                    Id = Convert.ToInt32(
                        ligne.Cells["id"].Value
                    ),

                    DateMouvement =
                        Convert.ToDateTime(
                            ligne.Cells["date_mouvement"].Value
                        ),

                    Sens =
                        Convert.ToString(
                            ligne.Cells["sens_affiche"].Value
                        ) ?? "",

                    Type =
                        Convert.ToString(
                            ligne.Cells["type_affiche"].Value
                        ) ?? "",

                    Montant =
                        Convert.ToDecimal(
                            ligne.Cells["montant"].Value
                        ),

                    Motif =
                        Convert.ToString(
                            ligne.Cells["motif"].Value
                        ) ?? "",

                    Reference =
                        Convert.ToString(
                            ligne.Cells["reference"].Value
                        ) ?? ""
                }
            );
        }

        return lignes;
    }

    private void GenererPdfCaisse(
        List<LigneCaissePdf> lignes,
        string chemin)
    {
        QuestPDF.Settings.License =
            LicenseType.Community;

        decimal totalEntrees = 0;
        decimal totalSorties = 0;

        foreach (LigneCaissePdf ligne in lignes)
        {
            if (
                ligne.Sens.Equals(
                    "Entrée",
                    StringComparison.OrdinalIgnoreCase
                ))
            {
                totalEntrees += ligne.Montant;
            }
            else
            {
                totalSorties += ligne.Montant;
            }
        }

        decimal solde =
            totalEntrees - totalSorties;

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
                             .FontSize(8.5F)
                );

                page.Header()
                    .Column(entete =>
                    {
                        entete.Item()
                            .Text(
                                "HISTORIQUE DE LA CAISSE"
                            )
                            .Bold()
                            .FontSize(19)
                            .FontColor(
                                Colors.Blue.Darken3
                            );

                        entete.Item()
                            .PaddingTop(4)
                            .Text(
                                $"Période : " +
                                $"{dtpDateDebut.Value:dd/MM/yyyy}" +
                                " au " +
                                $"{dtpDateFin.Value:dd/MM/yyyy}" +
                                "   |   Imprimé le : " +
                                $"{DateTime.Now:dd/MM/yyyy HH:mm}"
                            );
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
                                        columns.RelativeColumn(1.2F);
                                        columns.RelativeColumn(0.9F);
                                        columns.RelativeColumn(1.5F);
                                        columns.RelativeColumn(1.2F);
                                        columns.RelativeColumn(3.0F);
                                        columns.RelativeColumn(1.5F);
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
                                        .Text("Sens");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Type");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Montant");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Motif");

                                    header.Cell()
                                        .Element(StyleEntetePdf)
                                        .Text("Référence");
                                });

                                foreach (
                                    LigneCaissePdf ligne
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
                                            ligne.DateMouvement.ToString(
                                                "dd/MM/yyyy HH:mm"
                                            )
                                        );

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .AlignCenter()
                                        .Text(ligne.Sens);

                                    table.Cell()
                                        .Element(StyleCellulePdf)
                                        .Text(ligne.Type);

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
                                        .Text(
                                            ValeurPdf(
                                                ligne.Reference
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
                                        $"Entrées : {totalEntrees:N2} DA"
                                    )
                                    .Bold();

                                resume.RelativeItem()
                                    .Text(
                                        $"Sorties : {totalSorties:N2} DA"
                                    )
                                    .Bold();

                                resume.RelativeItem()
                                    .AlignRight()
                                    .Text(
                                        $"Solde période : {solde:N2} DA"
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
