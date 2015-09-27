using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Add Navisworks namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;


namespace SendClashToAnalytics
{
    [PluginAttribute("SendClashToAnalytics.SendClashToAnalytics",                           //Plugin name
                 "MORT",                                                                    //4 character Developer ID or GUID
                 ToolTip = "Send the exported .XML file to the web analytics program",      //The tooltip for the item in the ribbon
                 DisplayName = "Send .XML to Analytics")]                                   //Display name for the Plugin in the Ribbon

    public class SendClashToAnalytics : AddInPlugin
    {
        public override int Execute(params string[] parameters)
        {
            SendForm sendForm = new SendForm();
            if (sendForm.ShowDialog() != DialogResult.OK)
                return 0;

            return 0;
        }
    }
}
