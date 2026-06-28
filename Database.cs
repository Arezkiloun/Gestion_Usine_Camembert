using MySqlConnector;

namespace GestionUsine;

public static class Database
{
    private const string ServerConnectionString =
        "Server=127.0.0.1;" +
        "Port=3306;" +
        "User ID=root;" +
        "Password=;" +
        "SslMode=None;";

    private const string DatabaseName = "gestion_usine";

    private static readonly string DatabaseConnectionString =
        ServerConnectionString +
        $"Database={DatabaseName};";

    public static MySqlConnection CreateConnection()
    {
        return new MySqlConnection(DatabaseConnectionString);
    }

    public static void Initialize()
    {
        CreerBaseDeDonnees();

        CreerTableProduits();
        CreerTableMouvementsStockProduits();

        CreerTableEmballages();
        CreerTableMouvementsStockEmballages();

        CreerTableClients();
        CreerTableTarifsClients();

        CreerTableEmployes();
        SupprimerAncienneColonneMatriculeEmployes();
        CreerTableDepenses();
        CreerTableRecettes();

        CreerTableVentes();
      
        AjouterColonnesAnnulationVentes();
    
        CreerTableDetailsVentes();

        CreerTableCreditsClients();
        CreerTableMouvementsCaisse();

        CreerTableVersementsClients();
    }
    private static void CreerBaseDeDonnees()
    {
        using MySqlConnection connection =
            new MySqlConnection(ServerConnectionString);

        connection.Open();

        string sql = $"""
            CREATE DATABASE IF NOT EXISTS {DatabaseName}
            CHARACTER SET utf8mb4
            COLLATE utf8mb4_unicode_ci;
            """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableProduits()
    {
        using MySqlConnection connection = CreateConnection();

        connection.Open();

        const string sql = """
            CREATE TABLE IF NOT EXISTS produits
            (
                id INT NOT NULL AUTO_INCREMENT,
                nom VARCHAR(120) NOT NULL,
                poids_grammes INT NOT NULL,
                prix_vente DECIMAL(15,2) NOT NULL DEFAULT 0,
                quantite_stock INT NOT NULL DEFAULT 0,
                seuil_alerte INT NOT NULL DEFAULT 10,
                actif BOOLEAN NOT NULL DEFAULT TRUE,
                date_creation DATETIME NOT NULL
                    DEFAULT CURRENT_TIMESTAMP,

                PRIMARY KEY (id),

                UNIQUE KEY uq_produit
                (
                    nom,
                    poids_grammes
                )
            )
            ENGINE = InnoDB
            DEFAULT CHARACTER SET utf8mb4
            COLLATE utf8mb4_unicode_ci;
            """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }
    private static void CreerTableMouvementsStockProduits()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS mouvements_stock_produits
        (
            id INT NOT NULL AUTO_INCREMENT,
            produit_id INT NOT NULL,
            type_mouvement VARCHAR(40) NOT NULL,
            quantite INT NOT NULL,
            ancien_stock INT NOT NULL,
            nouveau_stock INT NOT NULL,
            motif VARCHAR(255) NULL,
            date_mouvement DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            CONSTRAINT fk_mouvement_stock_produit
                FOREIGN KEY (produit_id)
                REFERENCES produits(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }
    private static void CreerTableEmballages()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS emballages
        (
            id INT NOT NULL AUTO_INCREMENT,
            nom VARCHAR(150) NOT NULL,
            unite VARCHAR(40) NOT NULL DEFAULT 'unité',
            quantite_stock INT NOT NULL DEFAULT 0,
            seuil_alerte INT NOT NULL DEFAULT 20,
            actif BOOLEAN NOT NULL DEFAULT TRUE,
            date_creation DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),
            UNIQUE KEY uq_emballage_nom (nom)
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableMouvementsStockEmballages()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS mouvements_stock_emballages
        (
            id INT NOT NULL AUTO_INCREMENT,
            emballage_id INT NOT NULL,
            type_mouvement VARCHAR(50) NOT NULL,
            quantite INT NOT NULL,
            ancien_stock INT NOT NULL,
            nouveau_stock INT NOT NULL,
            motif VARCHAR(255) NULL,
            date_mouvement DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            CONSTRAINT fk_mouvement_stock_emballage
                FOREIGN KEY (emballage_id)
                REFERENCES emballages(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }
    private static void CreerTableClients()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS clients
        (
            id INT NOT NULL AUTO_INCREMENT,
            nom VARCHAR(150) NOT NULL,
            telephone VARCHAR(30) NULL,
            adresse VARCHAR(255) NULL,
            actif BOOLEAN NOT NULL DEFAULT TRUE,
            date_creation DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_clients_nom (nom),
            INDEX idx_clients_telephone (telephone)
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableTarifsClients()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sql = """
            CREATE TABLE IF NOT EXISTS tarifs_clients
            (
                id INT NOT NULL AUTO_INCREMENT,
                client_id INT NOT NULL,
                produit_id INT NOT NULL,
                prix_special DECIMAL(15,2) NOT NULL,
                actif BOOLEAN NOT NULL DEFAULT TRUE,
                date_creation DATETIME NOT NULL
                    DEFAULT CURRENT_TIMESTAMP,
                date_modification DATETIME NULL
                    DEFAULT NULL
                    ON UPDATE CURRENT_TIMESTAMP,

                PRIMARY KEY (id),

                UNIQUE KEY uq_tarif_client_produit
                (
                    client_id,
                    produit_id
                ),

                INDEX idx_tarifs_client
                (
                    client_id
                ),

                INDEX idx_tarifs_produit
                (
                    produit_id
                ),

                CONSTRAINT fk_tarif_client
                    FOREIGN KEY (client_id)
                    REFERENCES clients(id)
                    ON UPDATE CASCADE
                    ON DELETE CASCADE,

                CONSTRAINT fk_tarif_produit
                    FOREIGN KEY (produit_id)
                    REFERENCES produits(id)
                    ON UPDATE CASCADE
                    ON DELETE CASCADE
            )
            ENGINE = InnoDB
            DEFAULT CHARACTER SET utf8mb4
            COLLATE utf8mb4_unicode_ci;
            """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableEmployes()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS employes
        (
            id INT NOT NULL AUTO_INCREMENT,
            nom VARCHAR(100) NOT NULL,
            prenom VARCHAR(100) NOT NULL,
            telephone VARCHAR(30) NULL,
            poste VARCHAR(100) NOT NULL,
            salaire_base DECIMAL(15,2) NOT NULL DEFAULT 0,
            date_embauche DATE NOT NULL,
            actif BOOLEAN NOT NULL DEFAULT TRUE,
            date_creation DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,
            date_modification DATETIME NULL
                DEFAULT NULL
                ON UPDATE CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_employes_nom (nom),
            INDEX idx_employes_prenom (prenom),
            INDEX idx_employes_poste (poste),
            INDEX idx_employes_actif (actif)
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void SupprimerAncienneColonneMatriculeEmployes()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sqlColonne = """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'employes'
          AND COLUMN_NAME = 'matricule';
        """;

        using MySqlCommand colonneCommand =
            new MySqlCommand(sqlColonne, connection);

        bool colonneExiste =
            Convert.ToInt32(
                colonneCommand.ExecuteScalar()
            ) > 0;

        if (!colonneExiste)
        {
            return;
        }

        const string sqlIndex = """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.STATISTICS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = 'employes'
          AND INDEX_NAME = 'uq_employe_matricule';
        """;

        using MySqlCommand indexCommand =
            new MySqlCommand(sqlIndex, connection);

        bool indexExiste =
            Convert.ToInt32(
                indexCommand.ExecuteScalar()
            ) > 0;

        if (indexExiste)
        {
            using MySqlCommand supprimerIndex =
                new MySqlCommand(
                    """
                    ALTER TABLE employes
                    DROP INDEX uq_employe_matricule;
                    """,
                    connection
                );

            supprimerIndex.ExecuteNonQuery();
        }

        using MySqlCommand supprimerColonne =
            new MySqlCommand(
                """
                ALTER TABLE employes
                DROP COLUMN matricule;
                """,
                connection
            );

        supprimerColonne.ExecuteNonQuery();
    }

    private static void CreerTableDepenses()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS depenses
        (
            id INT NOT NULL AUTO_INCREMENT,
            categorie VARCHAR(100) NOT NULL,
            employe_id INT NULL,
            montant DECIMAL(15,2) NOT NULL,
            motif VARCHAR(255) NOT NULL,
            mode_paiement VARCHAR(30) NOT NULL
                DEFAULT 'CASH',
            date_depense DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,
            statut VARCHAR(20) NOT NULL
                DEFAULT 'VALIDEE',
            date_annulation DATETIME NULL,
            date_creation DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_depenses_date (date_depense),
            INDEX idx_depenses_categorie (categorie),
            INDEX idx_depenses_employe (employe_id),
            INDEX idx_depenses_statut (statut),

            CONSTRAINT fk_depense_employe
                FOREIGN KEY (employe_id)
                REFERENCES employes(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }


    private static void CreerTableRecettes()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS recettes
        (
            id INT NOT NULL AUTO_INCREMENT,
            categorie VARCHAR(150) NOT NULL,
            montant DECIMAL(15,2) NOT NULL,
            motif VARCHAR(255) NOT NULL DEFAULT '',
            mode_paiement VARCHAR(30) NOT NULL
                DEFAULT 'CASH',
            date_recette DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,
            statut VARCHAR(20) NOT NULL
                DEFAULT 'VALIDEE',
            date_annulation DATETIME NULL,
            date_creation DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_recettes_date (date_recette),
            INDEX idx_recettes_categorie (categorie),
            INDEX idx_recettes_statut (statut)
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableVentes()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS ventes
        (
            id INT NOT NULL AUTO_INCREMENT,
            client_id INT NOT NULL,
            montant_total DECIMAL(15,2) NOT NULL,
            montant_paye DECIMAL(15,2) NOT NULL DEFAULT 0,
            reste_a_payer DECIMAL(15,2) NOT NULL DEFAULT 0,
            type_paiement VARCHAR(40) NOT NULL,
            date_vente DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_ventes_client (client_id),
            INDEX idx_ventes_date (date_vente),

            CONSTRAINT fk_vente_client
                FOREIGN KEY (client_id)
                REFERENCES clients(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableDetailsVentes()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS details_ventes
        (
            id INT NOT NULL AUTO_INCREMENT,
            vente_id INT NOT NULL,
            produit_id INT NOT NULL,
            quantite INT NOT NULL,
            prix_unitaire DECIMAL(15,2) NOT NULL,
            sous_total DECIMAL(15,2) NOT NULL,

            PRIMARY KEY (id),

            INDEX idx_detail_vente (vente_id),
            INDEX idx_detail_produit (produit_id),

            CONSTRAINT fk_detail_vente
                FOREIGN KEY (vente_id)
                REFERENCES ventes(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT,

            CONSTRAINT fk_detail_produit
                FOREIGN KEY (produit_id)
                REFERENCES produits(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableCreditsClients()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS credits_clients
        (
            id INT NOT NULL AUTO_INCREMENT,
            vente_id INT NOT NULL,
            client_id INT NOT NULL,
            montant_total DECIMAL(15,2) NOT NULL,
            montant_paye DECIMAL(15,2) NOT NULL DEFAULT 0,
            reste_a_payer DECIMAL(15,2) NOT NULL,
            statut VARCHAR(20) NOT NULL DEFAULT 'OUVERT',
            date_creation DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
            date_dernier_versement DATETIME NULL,

            PRIMARY KEY (id),
            UNIQUE KEY uq_credit_vente (vente_id),

            INDEX idx_credit_client (client_id),
            INDEX idx_credit_statut (statut),

            CONSTRAINT fk_credit_vente
                FOREIGN KEY (vente_id)
                REFERENCES ventes(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT,

            CONSTRAINT fk_credit_client
                FOREIGN KEY (client_id)
                REFERENCES clients(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }

    private static void CreerTableMouvementsCaisse()
    {
        using MySqlConnection connection = CreateConnection();
        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS mouvements_caisse
        (
            id INT NOT NULL AUTO_INCREMENT,
            sens VARCHAR(10) NOT NULL,
            type_mouvement VARCHAR(50) NOT NULL,
            montant DECIMAL(15,2) NOT NULL,
            motif VARCHAR(255) NOT NULL,
            reference_type VARCHAR(50) NULL,
            reference_id INT NULL,
            date_mouvement DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_caisse_date (date_mouvement),
            INDEX idx_caisse_reference
                (reference_type, reference_id)
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);


        command.ExecuteNonQuery();
    }
    private static void AjouterColonnePdfAuxVentes()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sqlVerification = """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = 'gestion_usine'
          AND TABLE_NAME = 'ventes'
          AND COLUMN_NAME = 'pdf_path';
        """;

        using MySqlCommand verificationCommand =
            new MySqlCommand(
                sqlVerification,
                connection
            );

        int colonneExiste =
            Convert.ToInt32(
                verificationCommand.ExecuteScalar()
            );

        if (colonneExiste > 0)
        {
            return;
        }

        const string sqlAjout = """
        ALTER TABLE ventes
        ADD COLUMN pdf_path VARCHAR(500) NULL;
        """;

        using MySqlCommand ajoutCommand =
            new MySqlCommand(
                sqlAjout,
                connection
            );

        ajoutCommand.ExecuteNonQuery();
    }

    private static void CreerTableVersementsClients()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        const string sql = """
        CREATE TABLE IF NOT EXISTS versements_clients
        (
            id INT NOT NULL AUTO_INCREMENT,
            credit_id INT NOT NULL,
            client_id INT NOT NULL,
            montant DECIMAL(15,2) NOT NULL,
            ancien_reste DECIMAL(15,2) NOT NULL,
            nouveau_reste DECIMAL(15,2) NOT NULL,
            motif VARCHAR(255) NULL,
            date_versement DATETIME NOT NULL
                DEFAULT CURRENT_TIMESTAMP,

            PRIMARY KEY (id),

            INDEX idx_versement_credit (credit_id),
            INDEX idx_versement_client (client_id),
            INDEX idx_versement_date (date_versement),

            CONSTRAINT fk_versement_credit
                FOREIGN KEY (credit_id)
                REFERENCES credits_clients(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT,

            CONSTRAINT fk_versement_client
                FOREIGN KEY (client_id)
                REFERENCES clients(id)
                ON UPDATE CASCADE
                ON DELETE RESTRICT
        )
        ENGINE = InnoDB
        DEFAULT CHARACTER SET utf8mb4
        COLLATE utf8mb4_unicode_ci;
        """;

        using MySqlCommand command =
            new MySqlCommand(sql, connection);

        command.ExecuteNonQuery();
    }
    private static void AjouterColonnesAnnulationVentes()
    {
        using MySqlConnection connection =
            CreateConnection();

        connection.Open();

        AjouterColonneSiAbsente(
            connection,
            "ventes",
            "statut",
            "VARCHAR(20) NOT NULL DEFAULT 'VALIDEE'"
        );

        AjouterColonneSiAbsente(
            connection,
            "ventes",
            "date_annulation",
            "DATETIME NULL"
        );
    }

    private static void AjouterColonneSiAbsente(
        MySqlConnection connection,
        string table,
        string colonne,
        string definition)
    {
        const string sqlVerification = """
        SELECT COUNT(*)
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_SCHEMA = DATABASE()
          AND TABLE_NAME = @table
          AND COLUMN_NAME = @colonne;
        """;

        using MySqlCommand verificationCommand =
            new MySqlCommand(
                sqlVerification,
                connection
            );

        verificationCommand.Parameters.AddWithValue(
            "@table",
            table
        );

        verificationCommand.Parameters.AddWithValue(
            "@colonne",
            colonne
        );

        int existe =
            Convert.ToInt32(
                verificationCommand.ExecuteScalar()
            );

        if (existe > 0)
        {
            return;
        }

        // Les noms et définitions transmis ici sont définis
        // uniquement dans le code de l'application.
        string sqlAjout =
            $"ALTER TABLE `{table}` " +
            $"ADD COLUMN `{colonne}` {definition};";

        using MySqlCommand ajoutCommand =
            new MySqlCommand(
                sqlAjout,
                connection
            );

        ajoutCommand.ExecuteNonQuery();
    }



}
