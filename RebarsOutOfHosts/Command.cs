using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RebarsOutOfHosts
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        static Window win;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            if (win != null)
                return Result.Succeeded;

            var model = new MainControlModel();
            var vm = new MainControlViewModel(model);

            win = new MainControl() { DataContext = vm };
            win.Topmost = true;
            win.Closed += (s, e) => win = null;
            win.Show();

            return Result.Succeeded;
        }
    }
}
