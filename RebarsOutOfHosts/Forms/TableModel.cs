using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RebarsOutOfHosts
{
    internal class TableModel:ViewModelBase
    {
        ViewCreationHandler ViewHandler;
        ExternalEvent viewEvent;

        HostsCheckHandler checkHandler;
        ExternalEvent checkEvent;

        public ObservableCollection<FailedRebars> FailedElements;
        public IList selected;

        public string[] ParameterNames;

        ExcelExporter exporter;

        public TableModel(List<FailedRebars> Failed, HostsCheckHandler handler)
        {
            FailedElements = new ObservableCollection<FailedRebars>(Failed);
            ViewHandler = new ViewCreationHandler();
            viewEvent = ExternalEvent.Create(ViewHandler);
            selected = new FailedRebars[] { FailedElements.FirstOrDefault() };

            checkHandler = handler;
            checkEvent = ExternalEvent.Create(checkHandler);

            exporter = new ExcelExporter();

            ParameterNames = Failed.First().Parameters.Keys.OrderBy(x => x).ToArray();
        }

        public void CopySelectedElementId()
        {
            var selectedIds = from FailedRebars x in selected select x.ElementId;
            System.Windows.Clipboard.SetText(string.Join(",", selectedIds));
        }
        public void CopySelectedElementsHostId()
        {
            var selectedIds = from FailedRebars x in selected select x.HostId;
            System.Windows.Clipboard.SetText(string.Join(",", selectedIds));
        }

        public void SetView()
        {
            if (selected == null)
                return;
            
            var selectedIds = (from FailedRebars x in selected select x.ElementId).ToList();
            selectedIds.AddRange(from FailedRebars x in selected select x.HostId);
            ViewHandler.ids = selectedIds;
            viewEvent.Raise();
        }
        public void SetViewForAll()
        {
            ViewHandler.ids = new List<ElementId>();
            foreach(var fr in FailedElements)
            {
                ViewHandler.ids.Add(fr.ElementId); 
                ViewHandler.ids.Add(fr.HostId);
            }
            viewEvent.Raise();
        }

        public void Refresh()
        {
            checkEvent.Raise();
        }

        public void Export()
        {
            var selectedPath = "";
            using (var dialog = new FolderBrowserDialog() { ShowNewFolderButton = false })
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    selectedPath = dialog.SelectedPath;
                }
            }
            if(selectedPath == "")
            {
                TaskDialog.Show("Путь не выбран", "Путь не выбран");
                return;
            }

            var fileName = selectedPath + "\\" + "Арматура вне основы";
            var table = FailedElementsToTable();

            exporter.ExportTable(table, "Арматура вне основы", fileName);
            
        }

        public void SetSelectedParameter(string parameterName, Action onChanged)
        {
            foreach (var f in FailedElements)
                f.SelectedParameter = parameterName;
            FailedElements = new ObservableCollection<FailedRebars>(FailedElements.OrderBy(x => x.SelectedParameterValue));
            OnPropertyChanged(nameof(FailedElements));
            onChanged();
        }
        DataTable FailedElementsToTable()
        {
            var table = new DataTable();

            table.Columns.Add("id арматурного элемента", typeof(int));
            table.Columns.Add("Имя арматурного элемента", typeof(string));
            table.Columns.Add("id элемента основы", typeof(int));
            table.Columns.Add("Имя элемента основы", typeof(string));
            table.Columns.Add("Категория элемента основы", typeof(string));

            foreach(var e in FailedElements)
                table.Rows.Add(e.ElementId.IntegerValue, e.ElementName, e.HostId.IntegerValue, e.HostName, e.HostCategory);

            return table;
        }
        
    }
}
