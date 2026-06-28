using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace GestionUsine;

public sealed class LoginForm : Form
{
    private const string NomUsine =
        "LAITERIE MEDJDOUB";

    private const string LocaliteUsine =
        "TOGHZA";

    private const string NomFichierLogo =
        "laiterie.jpg";

    private readonly TextBox txtPassword;
    private readonly Button btnConnexion;
    private readonly Image? logoUsine;

    public LoginForm()
    {
        logoUsine = ChargerLogoUsine();

        Text = "Connexion - Laiterie Medjdoub";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(980, 580);
        MinimumSize = new Size(920, 540);
        BackColor = Color.FromArgb(242, 245, 248);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        Font = new Font("Segoe UI", 10F);

        var structurePrincipale = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.FromArgb(242, 245, 248)
        };

        structurePrincipale.ColumnStyles.Add(
            new ColumnStyle(SizeType.Absolute, 390)
        );

        structurePrincipale.ColumnStyles.Add(
            new ColumnStyle(SizeType.Percent, 100)
        );

        Controls.Add(structurePrincipale);

        // =====================================================
        // PANNEAU GAUCHE : IDENTITÉ DE LA LAITERIE
        // =====================================================
        var panneauGauche = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(25, 49, 78),
            Padding = new Padding(28)
        };

        structurePrincipale.Controls.Add(
            panneauGauche,
            0,
            0
        );

        var identiteLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Margin = Padding.Empty,
            Padding = Padding.Empty,
            BackColor = Color.FromArgb(25, 49, 78)
        };

        identiteLayout.RowStyles.Add(
            new RowStyle(SizeType.Percent, 18)
        );

        identiteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 190)
        );

        identiteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 70)
        );

        identiteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 38)
        );

        identiteLayout.RowStyles.Add(
            new RowStyle(SizeType.Percent, 82)
        );

        panneauGauche.Controls.Add(identiteLayout);

        var picLogo = new PictureBox
        {
            Dock = DockStyle.Fill,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(36, 8, 36, 12),
            Image = logoUsine
        };

        identiteLayout.Controls.Add(
            picLogo,
            0,
            1
        );

        var lblNomUsine = new Label
        {
            Text = "LAITERIE\r\nMEDJDOUB",
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(
                "Segoe UI",
                22F,
                FontStyle.Bold
            ),
            ForeColor = Color.White,
            Margin = Padding.Empty
        };

        identiteLayout.Controls.Add(
            lblNomUsine,
            0,
            2
        );

        var lblLocalite = new Label
        {
            Text = LocaliteUsine,
            Dock = DockStyle.Fill,
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font(
                "Segoe UI",
                12F,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(116, 205, 128),
            Margin = Padding.Empty
        };

        identiteLayout.Controls.Add(
            lblLocalite,
            0,
            3
        );

        // =====================================================
        // PANNEAU DROIT : CONNEXION
        // =====================================================
        var panneauDroit = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(242, 245, 248),
            Padding = new Padding(55, 45, 55, 45)
        };

        structurePrincipale.Controls.Add(
            panneauDroit,
            1,
            0
        );

        var carteConnexion = new Panel
        {
            Width = 460,
            Height = 440,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle
        };

        panneauDroit.Controls.Add(carteConnexion);

        panneauDroit.Resize += (_, _) =>
        {
            carteConnexion.Left =
                Math.Max(
                    0,
                    (panneauDroit.ClientSize.Width -
                     carteConnexion.Width) / 2
                );

            carteConnexion.Top =
                Math.Max(
                    0,
                    (panneauDroit.ClientSize.Height -
                     carteConnexion.Height) / 2
                );
        };

        var carteLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 8,
            Padding = new Padding(40, 30, 40, 28),
            Margin = Padding.Empty,
            BackColor = Color.White
        };

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 65)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 48)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 42)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 55)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 24)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 58)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Absolute, 54)
        );

        carteLayout.RowStyles.Add(
            new RowStyle(SizeType.Percent, 100)
        );

        carteConnexion.Controls.Add(carteLayout);

        var lblTitre = new Label
        {
            Text = "Connexion",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                27F,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(25, 49, 78),
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = Padding.Empty
        };

        carteLayout.Controls.Add(
            lblTitre,
            0,
            0
        );

        var lblDescription = new Label
        {
            Text =
                "Accédez au logiciel de gestion de " +
                "la Laiterie Medjdoub.",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.DimGray,
            TextAlign = ContentAlignment.TopLeft,
            Margin = new Padding(0, 0, 0, 6)
        };

        carteLayout.Controls.Add(
            lblDescription,
            0,
            1
        );

        var lblMotDePasse = new Label
        {
            Text = "Mot de passe",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                10.5F,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(45, 55, 65),
            TextAlign = ContentAlignment.BottomLeft,
            Margin = Padding.Empty
        };

        carteLayout.Controls.Add(
            lblMotDePasse,
            0,
            2
        );

        txtPassword = new TextBox
        {
            Name = "txtPassword",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F),
            UseSystemPasswordChar = true,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(0, 7, 0, 4)
        };

        carteLayout.Controls.Add(
            txtPassword,
            0,
            3
        );

        var ligneAccent = new Panel
        {
            Dock = DockStyle.Top,
            Height = 3,
            BackColor = Color.FromArgb(25, 135, 65),
            Margin = new Padding(0, 0, 0, 14)
        };

        carteLayout.Controls.Add(
            ligneAccent,
            0,
            4
        );

        btnConnexion = new Button
        {
            Name = "btnConnexion",
            Text = "SE CONNECTER",
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(25, 135, 65),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font(
                "Segoe UI",
                11F,
                FontStyle.Bold
            ),
            Cursor = Cursors.Hand,
            Margin = new Padding(0, 0, 0, 8)
        };

        btnConnexion.FlatAppearance.BorderSize = 0;

        btnConnexion.FlatAppearance.MouseOverBackColor =
            Color.FromArgb(20, 115, 55);

        btnConnexion.Click +=
            BtnConnexion_Click;

        carteLayout.Controls.Add(
            btnConnexion,
            0,
            5
        );

        var lblInfo = new Label
        {
            Text = "Mot de passe provisoire : 1234",
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                9F,
                FontStyle.Italic
            ),
            ForeColor = Color.Gray,
            TextAlign = ContentAlignment.MiddleCenter,
            Margin = Padding.Empty
        };

        carteLayout.Controls.Add(
            lblInfo,
            0,
            6
        );

        var lblSignature = new Label
        {
            Text =
                NomUsine +
                " • " +
                LocaliteUsine,
            Dock = DockStyle.Fill,
            AutoSize = false,
            Font = new Font(
                "Segoe UI",
                8.5F,
                FontStyle.Bold
            ),
            ForeColor = Color.FromArgb(165, 45, 45),
            TextAlign = ContentAlignment.BottomCenter,
            Margin = Padding.Empty
        };

        carteLayout.Controls.Add(
            lblSignature,
            0,
            7
        );

        AcceptButton = btnConnexion;

        Shown += (_, _) =>
        {
            panneauDroit.PerformLayout();

            carteConnexion.Left =
                Math.Max(
                    0,
                    (panneauDroit.ClientSize.Width -
                     carteConnexion.Width) / 2
                );

            carteConnexion.Top =
                Math.Max(
                    0,
                    (panneauDroit.ClientSize.Height -
                     carteConnexion.Height) / 2
                );

            txtPassword.Focus();
        };
    }

    private static Image? ChargerLogoUsine()
    {
        string dossierApplication =
            AppContext.BaseDirectory;

        var cheminsPossibles =
            new List<string>
            {
                Path.Combine(
                    dossierApplication,
                    "images",
                    NomFichierLogo
                ),

                Path.Combine(
                    dossierApplication,
                    "Images",
                    NomFichierLogo
                ),

                Path.Combine(
                    Environment.CurrentDirectory,
                    "images",
                    NomFichierLogo
                ),

                Path.Combine(
                    Environment.CurrentDirectory,
                    "Images",
                    NomFichierLogo
                )
            };

        DirectoryInfo? dossier =
            new DirectoryInfo(
                dossierApplication
            );

        for (
            int niveau = 0;
            niveau < 7 &&
            dossier != null;
            niveau++)
        {
            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
                    "images",
                    NomFichierLogo
                )
            );

            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
                    "Images",
                    NomFichierLogo
                )
            );

            dossier = dossier.Parent;
        }

        foreach (
            string chemin
            in cheminsPossibles)
        {
            if (!File.Exists(chemin))
            {
                continue;
            }

            try
            {
                byte[] donnees =
                    File.ReadAllBytes(
                        chemin
                    );

                using var flux =
                    new MemoryStream(
                        donnees
                    );

                using Image imageTemporaire =
                    Image.FromStream(
                        flux
                    );

                return new Bitmap(
                    imageTemporaire
                );
            }
            catch
            {
                // Essayer le chemin suivant.
            }
        }

        return null;
    }

    private void BtnConnexion_Click(
        object? sender,
        EventArgs e)
    {
        if (
            string.IsNullOrWhiteSpace(
                txtPassword.Text))
        {
            MessageBox.Show(
                "Veuillez saisir votre mot de passe.",
                "Champ obligatoire",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );

            txtPassword.Focus();
            return;
        }

        if (txtPassword.Text != "1234")
        {
            MessageBox.Show(
                "Mot de passe incorrect.",
                "Connexion refusée",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            txtPassword.Clear();
            txtPassword.Focus();
            return;
        }

        Hide();

        using var dashboard =
            new DashboardForm();

        dashboard.ShowDialog();

        // Quand le tableau de bord est fermé,
        // fermer également la fenêtre de connexion
        // et terminer complètement l'application.
        Close();
    }

    protected override void Dispose(
        bool disposing)
    {
        if (disposing)
        {
            logoUsine?.Dispose();
        }

        base.Dispose(
            disposing
        );
    }
}
