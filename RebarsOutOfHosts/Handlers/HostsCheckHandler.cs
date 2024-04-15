using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace RebarsOutOfHosts
{
    enum HostsCheckType
    {
        All,
        OnActiveView,
        Selected
    }
    class HostsCheckHandler : IExternalEventHandler
    {
        TableControl tabWin;
        public HostsCheckType CheckType;
        public void Execute(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var doc = uiDoc.Document;

            List<FailedRebars> resultList;
            switch(CheckType)
            {
                case HostsCheckType.All:
                    resultList = FailedRebars.GetFailedRebarsFromDocument(uiDoc.Document);
                    break;
                case HostsCheckType.OnActiveView:
                    var ids = new FilteredElementCollector(uiDoc.Document, uiDoc.ActiveView.Id).ToElementIds().ToList();
                    resultList = FailedRebars.GetFailedRebarsFromDocument(uiDoc.Document, ids);
                    break;
                case HostsCheckType.Selected:
                    var selectedIds = ObjectsPicker.PickRebars(uiDoc);
                    resultList = FailedRebars.GetFailedRebarsFromDocument(uiDoc.Document, selectedIds);
                    break;
                default:
                    resultList = FailedRebars.GetFailedRebarsFromDocument(uiDoc.Document);
                    break;
            }
            if (resultList == null || resultList.Count == 0)
            {
                TaskDialog.Show("Арматура вне основы", "Предупреждений об арматуре вне основы не найдено");
                if (tabWin != null)
                    tabWin.Close();
                return;
            }
            var tabModel = new TableModel(resultList, this);
            var tabVM = new TableViewModel(tabModel);

            if (tabWin == null)
            {
                tabWin = new TableControl() { DataContext = tabVM };
                tabWin.Closed += (s, e) => tabWin = null;
                tabWin.Show();
            }
            else
            {
                tabWin.DataContext = tabVM;
            }
            

        }
        
        public string GetName()
        {
            return "HostsCheckHandler";
        }
    }
}
