//------------------------------------------------------------------
// NumberClashes
//
// Number new clash groups that have not been named
//
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CSharp;
using Microsoft.VisualBasic;

using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.Clash;
using System.Diagnostics;

namespace ClashNumber2016
{
    [Plugin("ClashNumber2016", "MORT", ToolTip = "Uniquely number clash groups that do not contain a '~' in the name", DisplayName = "Number Clashes")]

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
                if (clashPrefix == "")
                {
                    MessageBox.Show("Plugin Canceled");
                    return;
                }
                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                DocumentClashTests oDCT = oDoc.GetClash().TestsData;
                oDCT.TestsSortTests(ClashTestSortMode.DisplayNameSort, ClashSortDirection.SortAscending);
                var tests = oDCT.Tests;
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
                                oDCT.TestsEditDisplayName((ClashResultGroup)result, clashPrefix + "-" + (clashNumber+1).ToString("000") + " ~ " + result.DisplayName);

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
