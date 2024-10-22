using Municipality_Services_App.Models;
using Municipality_Services_App.Workers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Municipality_Services_App.UserControls
{
    /// <summary>
    /// Interaction logic for UserPersonalisationDialog.xaml
    /// </summary>
    public partial class UserPersonalisationDialog: Window
    {
        private User user = UserSingleton.Instance.CurrentUser;

        public UserPersonalisationDialog()
        {
            InitializeComponent();
            if (user.Address != null && user.Address != string.Empty)
            {
                InputTextBox.Text = user.Address;
            }

        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Set the InputText property to the value in the TextBox
            user.Address = InputTextBox.Text;
            this.DialogResult = true; // Set dialog result as OK
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false; // Set dialog result as Cancel
            this.Close();

        }
    }
}
