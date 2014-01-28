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
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Clash;
using System.Diagnostics;

namespace NumberClashes
{
    [PluginAttribute("NumberClashes.NumberClashes",
         "MORT",
         ToolTip = "Number clash groups. Clashes must not contain a '~' in the name",
         DisplayName = "Number Clashes")]

    public class NumberClashes : AddInPlugin
    {
        int clashNumber = 0;

        public override int Execute(params string[] parameters)
        {
            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();
            ParseClash();
            totalTime.Stop();
            MessageBox.Show(Autodesk.Navisworks.Api.Application.Gui.MainWindow, "Clashes Renamed: " + clashNumber + Environment.NewLine + "Elapsed: " + totalTime.Elapsed.TotalSeconds + "s");
            return 0;
        }

        private void ParseClash()
        {
            try
            {
                bool cancel = false;
                string clashPrefix = Interaction.InputBox("Clash Interation Prefix", "Number Clashes");

                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                DocumentClashTests oDCT = oDoc.GetClash().TestsData;
                var tests = oDCT.Tests;
                tests.Sort();
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
                            if (!result.DisplayName.Contains("~"))
                            {
                                oDCT.TestsEditDisplayName((ClashResultGroup)result, clashPrefix + "-" + clashNumber.ToString("000") + " ~ " + result.DisplayName);

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