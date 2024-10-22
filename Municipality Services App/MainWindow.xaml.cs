using System.Media;
using System.Windows;

namespace Municipality_Services_App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        private ReportIssuesWindow reportIssuesWindow;
        private LocalEventsWindow localEventsWindow;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Check if the window is null or has been closed
            if (reportIssuesWindow == null || !reportIssuesWindow.IsLoaded)
            {
                // Create a new instance of the ReportIssuesWindow
                reportIssuesWindow = new ReportIssuesWindow();
                reportIssuesWindow.Show();
            }
            else
            {
                // If the window is still open, bring it to the foreground
                reportIssuesWindow.Activate();
            }
        }

        private void btnEvents_Click(object sender, RoutedEventArgs e)
        {
            if (localEventsWindow == null || !localEventsWindow.IsLoaded)

            {
                localEventsWindow = new LocalEventsWindow();
                localEventsWindow.Show();
            }
            else
            {
                localEventsWindow.Activate();
            }

        }

        private void btnRequestStatus_Click(object sender, RoutedEventArgs e)
        {
            var AlertPlayer = new SoundPlayer(@"Sounds\RESPONSIVE UI AUDIO QUE.wav");
            AlertPlayer.Play();
            MessageBox.Show("This Feature Is Not Implemented Yet");
        }
    }
}
//----------------------------------------------------End_of_File----------------------------------------------------//