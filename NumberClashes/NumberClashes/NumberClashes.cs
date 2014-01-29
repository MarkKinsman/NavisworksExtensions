//------------------------------------------------------------------
// NumberClashes
//------------------------------------------------------------------

// (C) Copyright 2009 by Autodesk Inc.

// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.

// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//------------------------------------------------------------------
//
// Number new clash groups that have not been named
//
//------------------------------------------------------------------

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