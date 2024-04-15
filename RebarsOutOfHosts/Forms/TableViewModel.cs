using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RebarsOutOfHosts
{
    internal class TableViewModel : ViewModelBase
    {
        TableModel model;
        public ObservableCollection<FailedRebars> FailedElements => model.FailedElements;
        public string[] ParameterNames => model.ParameterNames;
        
        private IList _selected = new ArrayList();
        public IList Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                model.selected = value;
                OnPropertyChanged("TestSelected");
            }
        }
        
        string selectedParameter = "Орг.КомплектЧертежей";
        public string SelectedParameter
        {
            get { return selectedParameter; }
            set 
            {
                selectedParameter = value;
                model.SetSelectedParameter(value, Changed);
                
            }
        }
        void Changed()
        {
            OnPropertyChanged(nameof(model));
            OnPropertyChanged(nameof(FailedElements));
        }
        public TableViewModel(TableModel tableModel)
        {
            model = tableModel;
            _selected = new FailedRebars[] { FailedElements.FirstOrDefault() };
            CreateView = new RelayCommand(obj => model.SetView());

            CopyElementId = new RelayCommand(obj => model.CopySelectedElementId());
            CopyHostId = new RelayCommand(obj => model.CopySelectedElementsHostId());

            CreateViewForAll = new RelayCommand(obj => model.SetViewForAll());
            Refresh = new RelayCommand(obj => model.Refresh());
            Export = new RelayCommand(obj => model.Export());
        }

        public RelayCommand CreateView { get; set; }
        public RelayCommand CopyElementId { get; set; }
        public RelayCommand CopyHostId { get; set; }
        
        public RelayCommand CreateViewForAll { get; set; }
        public RelayCommand Export { get; set; }
        public RelayCommand Refresh { get; set; }
        
    }
}
