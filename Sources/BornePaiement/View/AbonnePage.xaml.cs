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
using System.Windows.Navigation;
using System.Windows.Shapes;
using BornePaiement.ViewModel;

namespace BornePaiement.View
{
    /// <summary>
    /// Logique d'interaction pour AbonnePage.xaml
    /// </summary>
    public partial class AbonnePage : UserControl
    {
        public AbonnePage()
        {
            InitializeComponent();
            this.DataContext = new  AbonneVM();

            //// 🔹 Capture les événements clavier au niveau de la fenêtre principale
            //this.Loaded += (s, e) =>
            //{
            //    Window parentWindow = Window.GetWindow(this);
            //    if (parentWindow != null)
            //    {
            //        parentWindow.KeyDown += Page_KeyDown;
            //    }
            //};
        }

        ///// <summary>
        ///// Capture les événements clavier pour détecter un scan
        ///// </summary>
        //private void Page_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (DataContext is VisiteurVM vm)
        //    {
        //        vm.KeyPressed(sender, e);
        //    }
        //}
    }
}
