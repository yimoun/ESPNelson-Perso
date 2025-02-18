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
using ESPNelson.ViewModel;

namespace ESPNelson.View
{
    /// <summary>
    /// Logique d'interaction pour BorneEntreeView.xaml
    /// </summary>
    public partial class BorneEntreeView : Window
    {
        public BorneEntreeView()
        {
            InitializeComponent();
            this.DataContext = new BorneEntreeVM();
        }
    }
}
