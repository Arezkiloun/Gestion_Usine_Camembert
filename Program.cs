using MySqlConnector;
using QuestPDF.Infrastructure;

namespace GestionUsine;



internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        QuestPDF.Settings.License = LicenseType.Community;

        try
        {
            Database.Initialize();
            Application.Run(new LoginForm());
        }
        catch (MySqlException ex)
        {
            MessageBox.Show(
                "Impossible de se connecter à MySQL.\n\n" +
                "Vérifiez que WampServer est démarré.\n\n" +
                "Détail : " + ex.Message,
                "Erreur MySQL",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "Une erreur est survenue.\n\n" + ex.Message,
                "Erreur",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }
    }
}
