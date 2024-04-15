using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarsOutOfHosts
{
    internal class MainControlViewModel:ViewModelBase
    {
        MainControlModel model;

        public MainControlViewModel(MainControlModel Model)
        {
            model = Model;
            SelectElementsByHosts = new RelayCommand(obj =>
            model.SelectElements(SelectionType.ElementByHost));

            SelectHostsByElements = new RelayCommand(obj =>
            model.SelectElements(SelectionType.HostByElement));

            CheckBySelected = new RelayCommand(obj =>
            model.CheckHosts(HostsCheckType.Selected));

            CheckByActiveView = new RelayCommand(obj =>
            model.CheckHosts(HostsCheckType.OnActiveView));

            CheckAll = new RelayCommand(obj =>
            model.CheckHosts(HostsCheckType.All));
        }

        public RelayCommand CheckBySelected { get; set; }
        public RelayCommand CheckByActiveView { get; set; }
        public RelayCommand CheckAll { get; set; }

        public RelayCommand SelectElementsByHosts { get; set; }
        public RelayCommand SelectHostsByElements { get; set; }

    }
}
