using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace RebarsOutOfHosts
{
    class ObjectsPicker
    {
        public static List<ElementId> PickHosts(UIDocument uiDoc)
        {
            var ids = new List<ElementId>();
            try 
            { 
                var picked = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new HostsSelectionFilter(uiDoc.Document), "Выберите элементы основы");
                foreach (var r in picked)
                    ids.Add(uiDoc.Document.GetElement(r).Id);
            }
            catch
            {
                TaskDialog.Show("Элементы не выбраны", "Элементы не выбраны");
            }

            return ids;
        }
        public static List<ElementId> PickRebars(UIDocument uiDoc)
        {
            var ids = new List<ElementId>();
            try 
            { 
                var picked = uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, 
                    new RebarsSelectionFilter(uiDoc.Document), "Выберите арматурные элементы");
                foreach (var r in picked)
                    ids.Add(uiDoc.Document.GetElement(r).Id);
            }
            catch
            {
                TaskDialog.Show("Элементы не выбраны", "Элементы не выбраны");
            }
            return ids;
        }
        public static HashSet<ElementId> GetHost(List<ElementId> selectedElements, Document doc)
        {
            var result = new HashSet<ElementId>();

            foreach (var id in selectedElements)
            {
                var el = doc.GetElement(id);
                if (el is Rebar r)
                    result.Add(r.GetHostId());
                else if (el is AreaReinforcement ar)
                    result.Add(ar.GetHostId());
            }

            return result;
        }
        public static HashSet<ElementId> GetElements(List<ElementId> selectedHosts, Document doc)
        {
            var result = new HashSet<ElementId>();

            var rebar = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Rebar))
                .ToElements();

            foreach (var e in rebar)
            {
                var r = e as Rebar;
                if (r == null || !r.IsValidObject)
                    continue;

                var hostId = r.GetHostId();
                if (selectedHosts.Contains(hostId))
                    result.Add(e.Id);
            }

            var arearein = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_AreaRein))
                .ToElements();

            foreach (var e in arearein)
            {
                var r = e as AreaReinforcement;
                if (r == null || !r.IsValidObject)
                    continue;

                var hostId = r.GetHostId();
                if (selectedHosts.Contains(hostId))
                    result.Add(e.Id);
            }

            return result;
        }

    }
}
