using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

//Add new Autodesk namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.ComApi;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Api.Clash;
using System.Diagnostics;

namespace NumberClashes
{
    [PluginAttribute("PairClashViewpoints.CreateViewpoint",
         "MORT",
         ToolTip = "Create a Viewpoint",
         DisplayName = "Create Viewpoint")]

    public class NumberClashes : AddInPlugin
    {
        int createViewCount = 0;
        int clashNumber = 0;
        
        public override int Execute(params string[] parameters)
        {
            InwOpState10 myState = ComApiBridge.State;

            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();
            ParseClash(myState);
            totalTime.Stop();
            MessageBox.Show(Autodesk.Navisworks.Api.Application.Gui.MainWindow, "Done.  Clashes:" + clashNumber +  "    Elapsed: " + totalTime.Elapsed.TotalSeconds + "s");
            return 0;
        }

        private void ParseClash(InwOpState10 myState)
        {
            try
            {
                bool cancel = false;
                string clashPrefix = Interaction.InputBox("Clash Interation Prefix", "Number Clashes");

                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                createViewCount = 0;
                var tests = oDoc.GetClash().TestsData.Tests;
                double totalTests = tests.Count;
                double currentTest = 0;
                Progress progress = Autodesk.Navisworks.Api.Application.BeginProgress("Numbering Clashes");
                
                foreach (ClashTest test in tests)
                {
                    cancel = !progress.Update(++currentTest / totalTests);
                    if (cancel)
                        break;

                    progress.BeginSubOperation(1 / totalTests, "Clash Batch: " + test.DisplayName);
                    double totalClashes = test.Children.Count;
                    double currentClash = 0;
                    foreach (IClashResult result in test.Children)
                    {
                        cancel = !progress.Update(++currentClash / totalClashes);
                        if (cancel)
                            break;

                        if (result is ClashResultGroup)
                        {
                            if (result.Status == ClashResultStatus.New)
                            {
                                result.DisplayName = clashPrefix + clashNumber + " - " + result.DisplayName;
                                clashNumber++;
                            }
                        }
                        ((NativeHandle)result).Dispose();
                    }
                    progress.EndSubOperation();
                    test.Dispose();
                }
                Autodesk.Navisworks.Api.Application.EndProgress();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}