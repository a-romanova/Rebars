using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;

namespace RebarsOutOfHosts
{
    class FailedRebars : ViewModelBase
    {
        Element failedElement;
        public ElementId ElementId => failedElement.Id;
        public string ElementName => failedElement.Name;
        
        Element host;
        public ElementId HostId => host.Id;
        public string HostName => host.Name;
        public string HostCategory => host.Category.Name;

        public Dictionary<string, string> Parameters;

        string selectedParameter = "Орг.КомплектЧертежей";
        public string SelectedParameter 
        { 
            get { return selectedParameter; } 
            set { selectedParameter = value; OnPropertyChanged(nameof(SelectedParameterValue)); } 
        }
        public string SelectedParameterValue => Parameters.TryGetValue(SelectedParameter, out var value)? value : "";
        

        FailedRebars(Element e)
        {
            failedElement = e;
            if (failedElement is Rebar r)
                host = e.Document.GetElement(r.GetHostId());
            else if(failedElement is AreaReinforcement ar)
                host = e.Document.GetElement(ar.GetHostId());
        }


        public static List<FailedRebars> GetFailedRebarsFromDocument(Document doc, List<ElementId> SelectedElements = null)
        {
            var result = new List<FailedRebars>();
            var warnings = doc.GetWarnings();

            Regex reg = new Regex(@"арм\w*полностьювне\w*основы\w*", RegexOptions.IgnoreCase);
            
            foreach(var w in warnings)
            {
                if (reg.IsMatch(w.GetDescriptionText().Replace(" ", "")))
                {
                    foreach(var id in w.GetFailingElements())
                    {
                        var el = doc.GetElement(id);
                        if (el != null && el.IsValidObject && (SelectedElements == null || SelectedElements.Contains(el.Id)))
                            result.Add(new FailedRebars(el));
                    }
                }
            }
            var parameterNames = GetAllParameterNames(result);
            SetParametersDict(parameterNames, result);

            return result;
        }

        static HashSet<string> GetAllParameterNames(List<FailedRebars> rebars)
        {
            var result = new HashSet<string>(new string[] {"Орг.КомплектЧертежей"});
            foreach (var r in rebars)
                foreach (Parameter p in r.failedElement.Parameters)
                    result.Add(p.Definition.Name);
            
            return result;
        }
        static void SetParametersDict(HashSet<string> parametersNames, List<FailedRebars> rebars)
        {
            foreach(var r in rebars)
            {
                r.Parameters = new Dictionary<string, string>();
                foreach (var p in parametersNames)
                {
                    var parameter = r.failedElement.GetParameters(p).FirstOrDefault();
                    var value = "Параметр отсутствует";
                    if (parameter != null)
                    {
                        value = parameter.AsValueString();
                        if (value == null || value == "")
                            value = parameter.AsString();
                    }
                    r.Parameters.Add(p, value);
                }
            }
        }

    }
}
