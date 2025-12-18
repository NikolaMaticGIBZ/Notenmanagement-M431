namespace Notenmanagement.Web.Pages
{
    public partial class Index
    {
    private string targetRole = "teacher";

        private void SimulateLogin()
        {
            // Wir "schmuggeln" die Rolle als Query-Parameter mit, damit die 2FA Seite weiß, wohin.
            // Das ist eine einfache Frontend-Lösung ohne komplexen State.
            NavManager.NavigateTo($"2fa?role={targetRole}");
        }
    }
}

