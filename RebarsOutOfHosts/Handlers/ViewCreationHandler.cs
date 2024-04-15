using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RebarsOutOfHosts
{
    public class ViewCreationHandler : IExternalEventHandler
    {
        public  List<ElementId> ids;
        string viewName = "Арматура вне основы";

        public void Execute(UIApplication app)
        {
            if(ids == null || ids.Count ==0)
            {
                TaskDialog.Show("Нет элементов", "Нет элементов для отображения на 3D виде");
                return;
            }

            Document doc = app.ActiveUIDocument.Document;
            using (Transaction tr = new Transaction(doc, "Вид " + viewName))
            {
                tr.Start();

                var view = GetOrCreateView(doc);
                view.TemporaryViewModes.DeactivateAllModes();
                doc.Regenerate();

                var sectionBox = GetCropBox(doc, ids);
                
                
                view.SetSectionBox(sectionBox);
                view.CropBoxActive = false;
                view.CropBoxVisible = false;

                view.IsolateElementsTemporary(ids);
                
                tr.Commit();
                
                app.ActiveUIDocument.ActiveView = view;
                var openView = app.ActiveUIDocument.GetOpenUIViews().FirstOrDefault(x=> x.ViewId == view.Id);
                if (openView != null)
                    openView.ZoomToFit();
            }


        }

        View3D GetOrCreateView(Document doc)
        {
            var views3d = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View3D)).ToList();
            var i = views3d.FindIndex(x => x.Name == viewName);
            if (i != -1)
                return views3d[i] as View3D;

            var vType = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewFamilyType))
                .WhereElementIsElementType()
                .ToList().ConvertAll(new Converter<Element, ViewFamilyType>(x=> x as ViewFamilyType))
                .FirstOrDefault(x=>x.ViewFamily == ViewFamily.ThreeDimensional);
            
            var newView = View3D.CreateIsometric(doc, vType.Id);
            newView.Name = viewName;
            
            return newView;
        }
        BoundingBoxXYZ GetCropBox(Document doc, List<ElementId> ids)
        {
            var offset = new XYZ(100 / 304.8, 100 / 304.8, 100 / 304.8);

            var minX = double.MaxValue;
            var minY = double.MaxValue;
            var minZ = double.MaxValue;

            var maxX = double.MinValue;
            var maxY = double.MinValue;
            var maxZ = double.MinValue;

            foreach(var id in ids)
            {
                var e = doc.GetElement(id);
                if (e == null || !e.IsValidObject)
                    continue;

                var bb = e.get_BoundingBox(null);
                

                minX = Math.Min(minX, bb.Min.X);
                minY = Math.Min(minY, bb.Min.Y);
                minZ = Math.Min(minZ, bb.Min.Z);

                maxX = Math.Max(maxX, bb.Max.X);
                maxY = Math.Max(maxY, bb.Max.Y);
                maxZ = Math.Max(maxZ, bb.Max.Z);
            }

            var min = new XYZ(minX, minY, minZ)-offset;
            var max = new XYZ(maxX, maxY, maxZ)+offset;

            var resultBB = new BoundingBoxXYZ() { Min = min, Max = max};
            
            return resultBB;

        }

        public string GetName()
        {
            return "ViewCreationHandler";
        }

        

    }


}

