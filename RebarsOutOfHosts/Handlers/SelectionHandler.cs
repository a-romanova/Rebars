using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace RebarsOutOfHosts
{
    enum SelectionType
    {
        HostByElement,
        ElementByHost
    }
    class SelectionHandler : IExternalEventHandler
    {
        public SelectionType selectionType;
        public void Execute(UIApplication app)
        {
            var uiDoc = app.ActiveUIDocument;
            var selected = uiDoc.Selection.GetElementIds();

            if(selectionType == SelectionType.HostByElement)
            {
                var selectedElements = ObjectsPicker.PickRebars(uiDoc);
                var hosts = ObjectsPicker.GetHost(selectedElements, uiDoc.Document);
                uiDoc.Selection.SetElementIds(hosts);
            }
            else
            {
                var selectedHosts = ObjectsPicker.PickHosts(uiDoc);
                var elements = ObjectsPicker.GetElements(selectedHosts, uiDoc.Document);
                uiDoc.Selection.SetElementIds(elements);
            }

        }
        
        
        public string GetName()
        {
            return "SelectionHandler";
        }
    }

}
