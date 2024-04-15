using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;

namespace RebarsOutOfHosts
{
    class MainControlModel:ViewModelBase
    {
        SelectionHandler selectionHandler;
        ExternalEvent selectionEvent;

        HostsCheckHandler hostsCheckHandler;
        ExternalEvent hostsCheckEvent;

        public MainControlModel()
        {
            selectionHandler = new SelectionHandler();
            selectionEvent = ExternalEvent.Create(selectionHandler);

            hostsCheckHandler = new HostsCheckHandler();
            hostsCheckEvent = ExternalEvent.Create(hostsCheckHandler);
        }

        public void SelectElements(SelectionType selectionType)
        {
            selectionHandler.selectionType = selectionType;
            selectionEvent.Raise();
        }

        public void CheckHosts(HostsCheckType checkType)
        {
            hostsCheckHandler.CheckType = checkType;
            hostsCheckEvent.Raise();
        }
    }


}
