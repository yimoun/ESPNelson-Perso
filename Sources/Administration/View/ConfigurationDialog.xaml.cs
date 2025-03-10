using Administration.ViewModel;
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

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour ConfigurationDialog.xaml
    /// </summary>
    public partial class ConfigurationDialog : Window
    {
        public ConfigurationDialog(ConfigurationDialogVM vm)
        {
            InitializeComponent();
            DataContext = vm;

            // On passe la méthode pour fermer la fenêtre à la VM
            vm.CloseDialogAction = CloseDialogWithResult;
        }

        private void CloseDialogWithResult(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }
    }
}
