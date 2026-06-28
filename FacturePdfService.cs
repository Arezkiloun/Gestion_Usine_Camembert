using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MySqlConnector;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionUsine;

public sealed class LigneCommandePdf
{
    public string Produit { get; init; } = "";
    public int Quantite { get; init; }
    public decimal PrixUnitaire { get; init; }
    public decimal SousTotal { get; init; }
}

public sealed class CommandePdf
{
    public int Numero { get; init; }
    public string Client { get; init; } = "";
    public string Telephone { get; init; } = "";
    public string Adresse { get; init; } = "";
    public DateTime DateVente { get; init; }
    public decimal MontantTotal { get; init; }
    public decimal MontantPaye { get; init; }
    public decimal ResteAPayer { get; init; }
    public decimal AncienCredit { get; init; }
    public decimal NouveauSoldeClient { get; init; }
    public string TypePaiement { get; init; } = "";
    public List<LigneCommandePdf> Lignes { get; init; } = new();
}

public static class FacturePdfService
{
    private const string NomUsine =
        "LAITERIE MEDJDOUB";

    private const string LocaliteUsine =
        "TOGHZA";

    private const string NomFichierLogo =
        "laiterie.jpg";

    private static byte[]? ChargerLogoUsine()
    {
        var cheminsPossibles =
            new List<string>();

        string dossierApplication =
            AppContext.BaseDirectory;

        cheminsPossibles.Add(
            Path.Combine(
                dossierApplication,
                "Images",
                NomFichierLogo
            )
        );

        cheminsPossibles.Add(
            Path.Combine(
                dossierApplication,
                NomFichierLogo
            )
        );

        cheminsPossibles.Add(
            Path.Combine(
                Environment.CurrentDirectory,
                "Images",
                NomFichierLogo
            )
        );

        cheminsPossibles.Add(
            Path.Combine(
                Environment.CurrentDirectory,
                NomFichierLogo
            )
        );

        DirectoryInfo? dossier =
            new DirectoryInfo(
                dossierApplication
            );

        for (
            int niveau = 0;
            niveau < 6 && dossier != null;
            niveau++)
        {
            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
                    "Images",
                    NomFichierLogo
                )
            );

            cheminsPossibles.Add(
                Path.Combine(
                    dossier.FullName,
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

            return File.ReadAllBytes(
                chemin
            );
        }

        return null;
    }

    private static readonly CultureInfo Culture =
        CultureInfo.GetCultureInfo("fr-DZ");

    public static string ObtenirOuGenerer(
        int venteId)
    {
        VerifierVenteImprimable(
            venteId
        );

        string? ancienChemin =
            LireCheminPdf(
                venteId
            );

        // Les anciens fichiers "Commande_..." sont régénérés
        // avec le nouveau format de bon de livraison.
        if (
            !string.IsNullOrWhiteSpace(
                ancienChemin
            ) &&
            File.Exists(
                ancienChemin
            ) &&
            Path.GetFileName(
                ancienChemin
            ).StartsWith(
                "Bon_Livraison_Medjdoub_",
                StringComparison.OrdinalIgnoreCase
            ))
        {
            return ancienChemin;
        }

        CommandePdf commande =
            ChargerCommande(
                venteId
            );

        string dossier =
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.MyDocuments
                ),
                "GestionUsine",
                "BonsLivraison"
            );

        Directory.CreateDirectory(
            dossier
        );

        string clientFichier =
            NettoyerNomFichier(
                commande.Client
            );

        string nomFichier =
            $"Bon_Livraison_Medjdoub_" +
            $"{commande.Numero:D6}_" +
            $"{clientFichier}_" +
            $"{commande.DateVente:yyyyMMdd_HHmmss}.pdf";

        string chemin =
            Path.Combine(
                dossier,
                nomFichier
            );

        GenererPdf(
            commande,
            chemin
        );

        EnregistrerCheminPdf(
            venteId,
            chemin
        );

        return chemin;
    }

    private static void VerifierVenteImprimable(
        int venteId)
    {
        using MySqlConnection connection =
            Database.CreateConnection();

        connection.Open();

        const string sql = """
            SELECT
                statut,
                date_annulation
            FROM ventes
            WHERE id = @venteId
            LIMIT 1;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        using MySqlDataReader reader =
            command.ExecuteReader();

        if (!reader.Read())
        {
            throw new InvalidOperationException(
                "La vente demandée n’existe pas."
            );
        }

        string statut =
            reader.IsDBNull(
                reader.GetOrdinal("statut")
            )
                ? ""
                : reader.GetString(
                    "statut"
                ).Trim();

        bool venteAnnulee =
            statut.Contains(
                "ANNULE",
                StringComparison.OrdinalIgnoreCase
            )
            ||
            !reader.IsDBNull(
                reader.GetOrdinal(
                    "date_annulation"
                )
            );

        if (venteAnnulee)
        {
            throw new InvalidOperationException(
                "Cette vente est annulée.\n\n" +
                "Son bon de livraison et son impression " +
                "sont désactivés."
            );
        }
    }

    private static CommandePdf ChargerCommande(
        int venteId)
    {
        using MySqlConnection connection =
            Database.CreateConnection();

        connection.Open();

        const string sqlVente = """
            SELECT
                v.id,
                c.nom AS client,
                COALESCE(
                    c.telephone,
                    ''
                ) AS telephone,
                COALESCE(
                    c.adresse,
                    ''
                ) AS adresse,
                v.montant_total,
                v.montant_paye,
                v.reste_a_payer,
                v.type_paiement,
                v.date_vente,

                COALESCE(
                    (
                        SELECT
                            SUM(
                                cr.reste_a_payer
                            )
                        FROM credits_clients cr
                        WHERE cr.client_id = v.client_id
                          AND cr.statut = 'OUVERT'
                          AND cr.vente_id <> v.id
                    ),
                    0
                ) AS ancien_credit

            FROM ventes v

            INNER JOIN clients c
                ON c.id = v.client_id

            WHERE v.id = @venteId;
            """;

        CommandePdf commande;

        using (
            MySqlCommand command =
                new MySqlCommand(
                    sqlVente,
                    connection
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
                    "La vente demandée n’existe pas."
                );
            }

            decimal ancienCredit =
                reader.GetDecimal(
                    "ancien_credit"
                );

            decimal resteBon =
                reader.GetDecimal(
                    "reste_a_payer"
                );

            commande =
                new CommandePdf
                {
                    Numero =
                        reader.GetInt32(
                            "id"
                        ),

                    Client =
                        reader.GetString(
                            "client"
                        ),

                    Telephone =
                        reader.GetString(
                            "telephone"
                        ),

                    Adresse =
                        reader.GetString(
                            "adresse"
                        ),

                    MontantTotal =
                        reader.GetDecimal(
                            "montant_total"
                        ),

                    MontantPaye =
                        reader.GetDecimal(
                            "montant_paye"
                        ),

                    ResteAPayer =
                        resteBon,

                    AncienCredit =
                        ancienCredit,

                    NouveauSoldeClient =
                        ancienCredit +
                        resteBon,

                    TypePaiement =
                        TraduirePaiement(
                            reader.GetString(
                                "type_paiement"
                            )
                        ),

                    DateVente =
                        reader.GetDateTime(
                            "date_vente"
                        )
                };
        }

        const string sqlDetails = """
            SELECT
                CONCAT(
                    p.nom,
                    ' - ',
                    p.poids_grammes,
                    ' g'
                ) AS produit,
                d.quantite,
                d.prix_unitaire,
                d.sous_total

            FROM details_ventes d

            INNER JOIN produits p
                ON p.id = d.produit_id

            WHERE d.vente_id = @venteId

            ORDER BY d.id;
            """;

        using (
            MySqlCommand command =
                new MySqlCommand(
                    sqlDetails,
                    connection
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
                commande.Lignes.Add(
                    new LigneCommandePdf
                    {
                        Produit =
                            reader.GetString(
                                "produit"
                            ),

                        Quantite =
                            reader.GetInt32(
                                "quantite"
                            ),

                        PrixUnitaire =
                            reader.GetDecimal(
                                "prix_unitaire"
                            ),

                        SousTotal =
                            reader.GetDecimal(
                                "sous_total"
                            )
                    }
                );
            }
        }

        if (commande.Lignes.Count == 0)
        {
            throw new InvalidOperationException(
                "Cette vente ne contient aucun produit."
            );
        }

        return commande;
    }

    private static void GenererPdf(
        CommandePdf commande,
        string chemin)
    {
        Document
            .Create(document =>
            {
                document.Page(page =>
                {
                    page.Size(
                        PageSizes.A4
                    );

                    page.Margin(16);

                    page.DefaultTextStyle(
                        style =>
                            style
                                .FontFamily(
                                    "Arial"
                                )
                                .FontSize(
                                    ObtenirTailleTexte(
                                        commande.Lignes.Count
                                    )
                                )
                    );

                    page.Content()
                        .Column(column =>
                        {
                            column.Spacing(5);

                            column.Item()
                                .Element(
                                    container =>
                                        ConstruireBon(
                                            container,
                                            commande,
                                            "EXEMPLAIRE CLIENT"
                                        )
                                );

                            column.Item()
                                .PaddingVertical(1)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .LineHorizontal(1)
                                        .LineColor(
                                            Colors.Grey.Lighten1
                                        );

                                    row.ConstantItem(120)
                                        .AlignCenter()
                                        .Text(
                                            "LIGNE DE COUPE"
                                        )
                                        .FontSize(7)
                                        .FontColor(
                                            Colors.Grey.Darken1
                                        );

                                    row.RelativeItem()
                                        .LineHorizontal(1)
                                        .LineColor(
                                            Colors.Grey.Lighten1
                                        );
                                });

                            column.Item()
                                .Element(
                                    container =>
                                        ConstruireBon(
                                            container,
                                            commande,
                                            "EXEMPLAIRE USINE"
                                        )
                                );
                        });
                });
            })
            .GeneratePdf(
                chemin
            );
    }

    private static void ConstruireBon(
        IContainer container,
        CommandePdf commande,
        string exemplaire)
    {
        float paddingLigne =
            ObtenirPaddingLigne(
                commande.Lignes.Count
            );

        byte[]? logoUsine =
            ChargerLogoUsine();

        container
            .Border(1)
            .BorderColor(
                Colors.Grey.Darken1
            )
            .Padding(7)
            .Column(column =>
            {
                column.Spacing(4);

                column.Item()
                    .Row(row =>
                    {
                        if (logoUsine != null)
                        {
                            row.ConstantItem(72)
                                .Height(58)
                                .AlignMiddle()
                                .AlignCenter()
                                .Image(
                                    logoUsine
                                )
                                .FitArea();
                        }
                        else
                        {
                            row.ConstantItem(72)
                                .Height(58)
                                .AlignMiddle()
                                .AlignCenter()
                                .Border(1)
                                .BorderColor(
                                    Colors.Grey.Lighten1
                                )
                                .Text(
                                    "LOGO"
                                )
                                .FontSize(9)
                                .SemiBold()
                                .FontColor(
                                    Colors.Grey.Darken1
                                );
                        }

                        row.RelativeItem()
                            .PaddingLeft(8)
                            .AlignMiddle()
                            .Column(identite =>
                            {
                                identite.Item()
                                    .Text(
                                        NomUsine
                                    )
                                    .FontSize(14)
                                    .Bold()
                                    .FontColor(
                                        Colors.Green.Darken2
                                    );

                                identite.Item()
                                    .Text(
                                        LocaliteUsine
                                    )
                                    .FontSize(8)
                                    .SemiBold()
                                    .FontColor(
                                        Colors.Green.Darken3
                                    );

                                identite.Item()
                                    .PaddingTop(2)
                                    .Text(
                                        "BON DE LIVRAISON"
                                    )
                                    .FontSize(11)
                                    .Bold()
                                    .FontColor(
                                        Colors.Red.Darken2
                                    );
                            });

                        row.ConstantItem(135)
                            .AlignRight()
                            .AlignMiddle()
                            .Column(info =>
                            {
                                info.Item()
                                    .AlignRight()
                                    .Text(
                                        $"N° {commande.Numero:D6}"
                                    )
                                    .FontSize(10)
                                    .Bold();

                                info.Item()
                                    .AlignRight()
                                    .Text(
                                        commande.DateVente
                                            .ToString(
                                                "dd/MM/yyyy HH:mm"
                                            )
                                    )
                                    .FontSize(8);
                            });
                    });

                column.Item()
                    .LineHorizontal(1.2F)
                    .LineColor(
                        Colors.Green.Darken2
                    );

                column.Item()
                    .Row(row =>
                    {
                        row.RelativeItem(2)
                            .Column(client =>
                            {
                                client.Item()
                                    .Text(
                                        $"Client : {commande.Client}"
                                    )
                                    .Bold();

                                if (
                                    !string.IsNullOrWhiteSpace(
                                        commande.Telephone
                                    ))
                                {
                                    client.Item()
                                        .Text(
                                            "Téléphone : " +
                                            commande.Telephone
                                        );
                                }

                                if (
                                    !string.IsNullOrWhiteSpace(
                                        commande.Adresse
                                    ))
                                {
                                    client.Item()
                                        .Text(
                                            "Adresse : " +
                                            commande.Adresse
                                        );
                                }
                            });

                        row.RelativeItem()
                            .AlignRight()
                            .Column(info =>
                            {
                                info.Item()
                                    .AlignRight()
                                    .Text(
                                        "Paiement : " +
                                        commande.TypePaiement
                                    );

                                info.Item()
                                    .AlignRight()
                                    .Text(
                                        exemplaire
                                    )
                                    .Bold()
                                    .FontColor(
                                        Colors.Blue.Darken3
                                    );
                            });
                    });

                column.Item()
                    .Table(table =>
                    {
                        table.ColumnsDefinition(
                            columns =>
                            {
                                columns.RelativeColumn(5);
                                columns.ConstantColumn(55);
                                columns.ConstantColumn(82);
                                columns.ConstantColumn(90);
                            });

                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(CelluleEntete)
                                .Text("Produit");

                            header.Cell()
                                .Element(CelluleEntete)
                                .AlignCenter()
                                .Text("Qté");

                            header.Cell()
                                .Element(CelluleEntete)
                                .AlignRight()
                                .Text("Prix U.");

                            header.Cell()
                                .Element(CelluleEntete)
                                .AlignRight()
                                .Text("Montant");
                        });

                        foreach (
                            LigneCommandePdf ligne
                            in commande.Lignes)
                        {
                            table.Cell()
                                .Element(
                                    cellule =>
                                        CelluleValeur(
                                            cellule,
                                            paddingLigne
                                        )
                                )
                                .Text(
                                    ligne.Produit
                                );

                            table.Cell()
                                .Element(
                                    cellule =>
                                        CelluleValeur(
                                            cellule,
                                            paddingLigne
                                        )
                                )
                                .AlignCenter()
                                .Text(
                                    ligne.Quantite
                                        .ToString(
                                            "N0",
                                            Culture
                                        )
                                );

                            table.Cell()
                                .Element(
                                    cellule =>
                                        CelluleValeur(
                                            cellule,
                                            paddingLigne
                                        )
                                )
                                .AlignRight()
                                .Text(
                                    FormaterMontant(
                                        ligne.PrixUnitaire
                                    )
                                );

                            table.Cell()
                                .Element(
                                    cellule =>
                                        CelluleValeur(
                                            cellule,
                                            paddingLigne
                                        )
                                )
                                .AlignRight()
                                .Text(
                                    FormaterMontant(
                                        ligne.SousTotal
                                    )
                                );
                        }
                    });

                column.Item()
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Border(1)
                            .BorderColor(
                                Colors.Grey.Lighten1
                            )
                            .Padding(5)
                            .Column(credit =>
                            {
                                credit.Item()
                                    .Text(
                                        "SITUATION DU CLIENT"
                                    )
                                    .Bold()
                                    .FontColor(
                                        Colors.Blue.Darken3
                                    );

                                credit.Item()
                                    .Text(
                                        "Ancien crédit : " +
                                        FormaterMontant(
                                            commande.AncienCredit
                                        )
                                    );

                                credit.Item()
                                    .Text(
                                        "Nouveau solde client : " +
                                        FormaterMontant(
                                            commande.NouveauSoldeClient
                                        )
                                    )
                                    .Bold()
                                    .FontColor(
                                        commande.NouveauSoldeClient > 0
                                            ? Colors.Red.Darken2
                                            : Colors.Green.Darken2
                                    );
                            });

                        row.ConstantItem(255)
                            .PaddingLeft(6)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(
                                    columns =>
                                    {
                                        columns.RelativeColumn();
                                        columns.ConstantColumn(105);
                                    });

                                AjouterTotal(
                                    table,
                                    "Total du bon",
                                    commande.MontantTotal,
                                    true
                                );

                                AjouterTotal(
                                    table,
                                    "Payé sur ce bon",
                                    commande.MontantPaye,
                                    false
                                );

                                AjouterTotal(
                                    table,
                                    "Reste de ce bon",
                                    commande.ResteAPayer,
                                    commande.ResteAPayer > 0
                                );
                            });
                    });

                column.Item()
                    .PaddingTop(3)
                    .Row(row =>
                    {
                        row.RelativeItem()
                            .Text(
                                "Signature client : __________________"
                            );

                        row.RelativeItem()
                            .AlignRight()
                            .Text(
                                "Signature usine : __________________"
                            );
                    });
            });
    }

    private static float ObtenirTailleTexte(
        int nombreLignes)
    {
        if (nombreLignes <= 8)
        {
            return 8.2F;
        }

        if (nombreLignes <= 14)
        {
            return 7.2F;
        }

        return 6.2F;
    }

    private static float ObtenirPaddingLigne(
        int nombreLignes)
    {
        if (nombreLignes <= 8)
        {
            return 2.5F;
        }

        if (nombreLignes <= 14)
        {
            return 1.5F;
        }

        return 0.7F;
    }

    private static IContainer CelluleEntete(
        IContainer container)
    {
        return container
            .Background(
                Colors.Blue.Darken3
            )
            .PaddingVertical(3)
            .PaddingHorizontal(4)
            .DefaultTextStyle(
                style =>
                    style
                        .FontColor(
                            Colors.White
                        )
                        .SemiBold()
            );
    }

    private static IContainer CelluleValeur(
        IContainer container,
        float paddingVertical)
    {
        return container
            .BorderBottom(1)
            .BorderColor(
                Colors.Grey.Lighten2
            )
            .PaddingVertical(
                paddingVertical
            )
            .PaddingHorizontal(4);
    }

    private static void AjouterTotal(
        TableDescriptor table,
        string titre,
        decimal montant,
        bool important)
    {
        IContainer celluleTitre =
            table.Cell()
                .PaddingVertical(2)
                .PaddingHorizontal(3);

        IContainer celluleMontant =
            table.Cell()
                .PaddingVertical(2)
                .PaddingHorizontal(3)
                .AlignRight();

        if (important)
        {
            celluleTitre =
                celluleTitre.Background(
                    Colors.Grey.Lighten3
                );

            celluleMontant =
                celluleMontant.Background(
                    Colors.Grey.Lighten3
                );
        }

        celluleTitre
            .Text(titre)
            .SemiBold();

        celluleMontant
            .Text(
                FormaterMontant(
                    montant
                )
            )
            .Bold();
    }

    private static string? LireCheminPdf(
        int venteId)
    {
        using MySqlConnection connection =
            Database.CreateConnection();

        connection.Open();

        const string sql = """
            SELECT pdf_path
            FROM ventes
            WHERE id = @venteId;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        object? resultat =
            command.ExecuteScalar();

        return
            resultat == null ||
            resultat == DBNull.Value
                ? null
                : Convert.ToString(
                    resultat
                );
    }

    private static void EnregistrerCheminPdf(
        int venteId,
        string chemin)
    {
        using MySqlConnection connection =
            Database.CreateConnection();

        connection.Open();

        const string sql = """
            UPDATE ventes
            SET pdf_path = @chemin
            WHERE id = @venteId;
            """;

        using MySqlCommand command =
            new MySqlCommand(
                sql,
                connection
            );

        command.Parameters.AddWithValue(
            "@chemin",
            chemin
        );

        command.Parameters.AddWithValue(
            "@venteId",
            venteId
        );

        command.ExecuteNonQuery();
    }

    private static string TraduirePaiement(
        string typePaiement)
    {
        return typePaiement switch
        {
            "CASH" =>
                "Paiement cash",

            "VERSEMENT" =>
                "Paiement avec versement",

            "CREDIT" =>
                "Paiement à crédit",

            _ =>
                typePaiement
        };
    }

    private static string FormaterMontant(
        decimal montant)
    {
        return montant.ToString(
            "N2",
            Culture
        ) + " DA";
    }

    private static string NettoyerNomFichier(
        string texte)
    {
        string resultat =
            texte.Trim();

        foreach (
            char caractere
            in Path.GetInvalidFileNameChars())
        {
            resultat =
                resultat.Replace(
                    caractere,
                    '_'
                );
        }

        return
            string.IsNullOrWhiteSpace(
                resultat
            )
                ? "Client"
                : resultat;
    }
}
