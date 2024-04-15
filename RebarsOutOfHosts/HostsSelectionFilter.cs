using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace RebarsOutOfHosts
{
    class HostsSelectionFilter : ISelectionFilter
    {
        Document doc = null;
        ElementMulticategoryFilter catFilter;
        public HostsSelectionFilter(Document document)
        {
            doc = document;
            catFilter = new ElementMulticategoryFilter(new List<BuiltInCategory> { BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors });
        }

        public bool AllowElement(Element elem)
        {
            if (catFilter.PassesFilter(elem))
                return true;

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            if (catFilter.PassesFilter(doc.GetElement(reference)))
                return true;

            return false;
        }
    }
}
