using Municipality_Services_App.Models;
using Municipality_Services_App.Workers;
using System.Windows;

namespace Municipality_Services_App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App: Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize the CurrentUser object
            User user = new User();
            // Set up CurrentUser details here if needed

            // Set the CurrentUser object in the singleton
            UserSingleton.Instance.SetUser(user);
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//