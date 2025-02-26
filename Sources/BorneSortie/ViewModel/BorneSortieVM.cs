using BorneSortie.View;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BorneSortie.ViewModel
{
    public partial class BorneSortieVM : ObservableObject
    {
        [ObservableProperty]
        private Page selectedView; // Stocke la page actuelle (Visiteur ou Abonné)

        public IRelayCommand SetModeVisiteurCommand { get; }
        public IRelayCommand SetModeAbonneCommand { get; }


        public BorneSortieVM()
        {
            SetModeVisiteurCommand = new RelayCommand(() => SelectedView = new VisiteurView());
            SetModeAbonneCommand = new RelayCommand(() => SelectedView = new AbonneView());
        }
    }

    
}
