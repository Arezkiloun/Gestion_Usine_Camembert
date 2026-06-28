# GestionUsine — Laiterie Medjdoub

Application de gestion développée en **C# avec Windows Forms** pour la **Laiterie Medjdoub à Toghza**.

Le logiciel permet de centraliser la gestion quotidienne de l’usine à travers une interface simple, claire et adaptée aux besoins réels de l’entreprise.

## Fonctionnalités principales

* Tableau de bord avec indicateurs
* Gestion des employés
* Gestion des clients
* Gestion des produits
* Gestion du stock produits
* Gestion du stock emballages
* Gestion des ventes
* Tarifs spéciaux par client
* Gestion des crédits et versements
* Gestion des dépenses et recettes
* Suivi de la caisse
* Génération de bons de livraison
* Export de rapports PDF

## Technologies utilisées

* C#
* Windows Forms
* .NET 10
* MySQL
* MySqlConnector
* QuestPDF
* WampServer
* Git et GitHub

## Aperçu du projet

Le projet comprend notamment :

* une page de connexion personnalisée ;
* un tableau de bord avec les principales statistiques ;
* un module complet de gestion des ventes ;
* un suivi des crédits clients ;
* un système de gestion des stocks ;
* des bons de livraison avec deux exemplaires sur une feuille A4 ;
* des rapports PDF pour les principales opérations.

## Installation

Cloner le dépôt :

```bash
git clone https://github.com/Arezkiloun/Gestion_Usine_Camembert.git
```

Ouvrir ensuite :

```text
GestionUsine.csproj
```

Restaurer les packages :

```bash
dotnet restore
```

Compiler le projet :

```bash
dotnet build
```

Lancer l’application :

```bash
dotnet run
```

## Base de données

L’application utilise une base MySQL nommée :

```text
gestion_usine
```

Avant de lancer le projet :

1. Démarrer WampServer
2. Vérifier que MySQL fonctionne
3. Vérifier que la base `gestion_usine` existe
4. Contrôler les paramètres de connexion dans `Database.cs`

## Logo

Le logo de la Laiterie Medjdoub doit être placé dans :

```text
images/laiterie.jpg
```

## Liens


* **Portfolio :**
  https://lounis-arezki-portfolio.netlify.app

* **Publication LinkedIn :**
  https://www.linkedin.com/posts/arezki-lounis-4849b0369_csharp-dotnet-windowsforms-ugcPost-7477112954925113344-fhue/

## Projet

**LAITERIE MEDJDOUB — TOGHZA**

Application destinée à la gestion interne de la laiterie.
::: 
