using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Add namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;

namespace CreateAnalyticProject
{
    [PluginAttribute("CreateAnalyticsProject.CreateAnalyticsProject",                               //Plugin name
             "MORT",                                                                                //4 character Developer ID or GUID
             ToolTip = "Create a new project to send analytics to by creating a config file",       //The tooltip for the item in the ribbon
             DisplayName = "Create Analytics Project")]                                             //Display name for the Plugin in the Ribbon

    public class CreateAnalyticProject
    {
        public override int Execute(params string[] parameters)
        {
            CreateProjectForm createForm = new CreateProjectForm();
            if (createForm.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("Plugin Canceled");
                return 0;
            }

            return 0;
        }
    }
}
