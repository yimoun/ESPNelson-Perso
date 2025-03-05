using Administration.Model;
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
using Administration.ViewModel;

namespace Administration.View
{
    /// <summary>
    /// Logique d'interaction pour TarificationDialog.xaml
    /// </summary>
    public partial class TarificationDialog : Window
    {
        public TarificationDialog(Tarification tarification, bool estNouvelle)
        {
            InitializeComponent();
            DataContext = new TarificationDialogVM(tarification, CloseDialogWithResult);
        }

        private void CloseDialogWithResult(bool result)
        {
            this.DialogResult = result;
            this.Close();
        }

    }
}
