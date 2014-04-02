//------------------------------------------------------------------
// Create Viewpoint from Clash Detective
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
// 
//
//------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Drawing;
using Microsoft.CSharp;

//Add new Autodesk namespaces
using Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.ComApi;
using Autodesk.Navisworks.Api.Interop;
using Autodesk.Navisworks.Api.Interop.ComApi;
using Autodesk.Navisworks.Api.Clash;
using System.Diagnostics;

namespace PairClashViewpoints
{
    [PluginAttribute("PairClashViewpoints.CreateViewpoint",
         "MORT",
         ToolTip = "Create a Viewpoint",
         DisplayName = "Create Viewpoint")]

    public class CreateViewpoint : AddInPlugin
    {
        #region Global Variables
        const string defaulViewpointName = "00. Default";
        const string spatialCoordinationFolderName = "02. Spatial Coordination";
        const string openIssuesFolderName = "01. Open Issues";
        const string closedIssuesFolderName = "02. Closed Issues";
        const string reviewedIssuesFolderName = "03. Reviewed Issues";

        InwOpFolderView openIssuesFolder = null;
        InwOpFolderView closedIssuesFolder = null;
        InwOpFolderView reviewedIssuesFolder = null;
        Dictionary<String, InwOpFolderView> oldViewpoints = new Dictionary<string,InwOpFolderView>();

        InwOpView defaultView = null;
        int createViewCount = 0;
        #endregion

        public override int Execute(params string[] parameters)
        {
            try
            {
                InwOpState10 myState = ComApiBridge.State;

                GetClashFolders(myState);
                ClearClashFolders(myState);
                Stopwatch totalTime = new Stopwatch();
                totalTime.Start();
                ParseClash(myState);
                ShowDefaultViewpoint(myState);
                oldViewpoints.Clear();
                totalTime.Stop();
                MessageBox.Show(Autodesk.Navisworks.Api.Application.Gui.MainWindow, "Done.  Elapsed: " + totalTime.Elapsed.TotalSeconds + "s");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return 0;
        }

        #region Viewpont Folder Management
        private void GetClashFolders(InwOpState10 myState)
        {
            try
            {
                var spatialCoordinationFolder = GetFolder(myState.SavedViews(), spatialCoordinationFolderName);
                openIssuesFolder = GetFolder(spatialCoordinationFolder, openIssuesFolderName);
                closedIssuesFolder = GetFolder(spatialCoordinationFolder, closedIssuesFolderName);
                reviewedIssuesFolder = GetFolder(spatialCoordinationFolder, reviewedIssuesFolderName);
                oldViewpoints = GetOldViewpoints();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private Dictionary<string, InwOpFolderView> GetOldViewpoints()
        {
            Dictionary<string, InwOpFolderView> viewpointsDictionary = new Dictionary<string, InwOpFolderView>();

            foreach (InwOpSavedView viewpoint in openIssuesFolder.SavedViews())
            {
                if (viewpoint.Type == nwESavedViewType.eSavedViewType_Folder)
                {
                    if (viewpointsDictionary.ContainsKey(viewpoint.name))
                    {
                        MessageBox.Show(viewpoint.name + " is not a unique clash name. This may cause errors in the exported viewpoints.");
                    }
                    else
                    {
                        viewpointsDictionary.Add(viewpoint.name, (InwOpFolderView)viewpoint.Copy());
                    }
                }
            }
            foreach (InwOpSavedView viewpoint in closedIssuesFolder.SavedViews())
            {
                if (viewpoint.Type == nwESavedViewType.eSavedViewType_Folder)
                {
                    if (viewpointsDictionary.ContainsKey(viewpoint.name))
                    {
                        MessageBox.Show(viewpoint.name + " is not a unique clash name. This may cause errors in the exported viewpoints.");
                    }
                    else
                    {
                        viewpointsDictionary.Add(viewpoint.name, (InwOpFolderView)viewpoint.Copy());
                    }
                }
            }
            foreach (InwOpSavedView viewpoint in reviewedIssuesFolder.SavedViews())
            {
                if (viewpoint.Type == nwESavedViewType.eSavedViewType_Folder)
                {
                    if (viewpointsDictionary.ContainsKey(viewpoint.name))
                    {
                        MessageBox.Show(viewpoint.name + " is not a unique clash name. This may cause errors in the exported viewpoints.");
                    }
                    else
                    {
                        viewpointsDictionary.Add(viewpoint.name, (InwOpFolderView)viewpoint.Copy());
                    }
                }
            }

            return viewpointsDictionary;
        }

        private void ClearClashFolders(InwOpState10 myState)
        {
            try
            {
                openIssuesFolder.SavedViews().Clear();
                closedIssuesFolder.SavedViews().Clear();
                reviewedIssuesFolder.SavedViews().Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowViewpointSelector()
        {
            if (Autodesk.Navisworks.Api.Application.IsAutomated)
            {
                throw new InvalidOperationException("Invalid when running using Automation");
            }

            //Find the plugin
            PluginRecord pr =
               Autodesk.Navisworks.Api.Application.Plugins.FindPlugin("AppInfo.AppInfoDockPane.ADSK");

            if (pr != null && pr is DockPanePluginRecord && pr.IsEnabled)
            {
                //check if it needs loading
                if (pr.LoadedPlugin == null)
                {
                    pr.LoadPlugin();
                }

                DockPanePlugin dpp = pr.LoadedPlugin as DockPanePlugin;
                if (dpp != null)
                {
                    //switch the Visible flag
                    dpp.Visible = !dpp.Visible;
                }
            }
        }
        #endregion
        
        private void ParseClash(InwOpState10 myState)
        {
            try
            {
                bool cancel = false;
                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                createViewCount = 0;
                var tests = oDoc.GetClash().TestsData.Tests;
                double totalTests = tests.Count;
                double currentTest = 0;
                Progress progress = Autodesk.Navisworks.Api.Application.BeginProgress("Creating ViewPoints");
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
                            CreateViewPointSet(myState, result);
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
        
        #region Viewpoint Creation
        private void CreateViewPointSet(InwOpState10 myState, IClashResult result)
        {
            Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            InwOpFolderView viewPointSaveFolder = GetViewpointDestinationFolder(result.Status, result.DisplayName, myState);
            if (viewPointSaveFolder != null)
            {
                Transaction tran = oDoc.BeginTransaction(result.DisplayName);
                try
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    Debug.Write(string.Format("Starting #{0} - {1}", ++createViewCount, result.DisplayName));
                    myState.BeginEdit(result.DisplayName);

                    if (oldViewpoints.ContainsKey(result.DisplayName) & result.DisplayName.Contains("~"))
                    {
                        myState.ApplyView(oldViewpoints[result.DisplayName].SavedViews()[1]);
                    }
                    else
                    {
                        ShowDefaultViewpoint(myState);
                        oDoc.CurrentSelection.Clear();
                        oDoc.CurrentSelection.AddRange(result.Selection1);
                        oDoc.CurrentSelection.AddRange(result.Selection2);
                        ZoomCurrentSelection();
                    }
                    

                    CreateViewPointFromCurrentView(myState, "All", viewPointSaveFolder);

                    oDoc.CurrentSelection.Clear();
                    oDoc.CurrentSelection.AddRange(result.Selection1);
                    oDoc.CurrentSelection.AddRange(result.Selection2);
                    HideUnselected(myState);

                    CreateViewPointFromCurrentView(myState, "Isolated", viewPointSaveFolder);
                    myState.EndEdit();
                    stopWatch.Stop();
                    Debug.WriteLine(" Elapsed: {0}s", stopWatch.Elapsed.TotalSeconds);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    tran.Dispose();
                }
                finally
                {
                    tran = null;
                }
            }
            else
            {
                MessageBox.Show("Missing folder: " + spatialCoordinationFolderName);
            }
            
        }

        private void CreateViewPointFromCurrentView(InwOpState10 myState, string name, InwOpFolderView viewPointSaveFolder)
        {
            InwOpView allViewPoint = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpView);

            // Set viewpoint camera and apply it
            allViewPoint.anonview = myState.CurrentView;
            // Name the viewpoint
            allViewPoint.name = name;
            // Set Hide/Required of viewpoint
            allViewPoint.ApplyHideAttribs = true;
            allViewPoint.ApplyMaterialAttribs = false;

            viewPointSaveFolder.SavedViews().Add(allViewPoint);
        }

        public void ZoomCurrentSelection()
        {
            InwOpState10 comState = ComApiBridge.State;

            //Create a collection
            using (ModelItemCollection modelItemCollectionIn = new ModelItemCollection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems))
            {
                InwOpSelection comSelectionOut = ComApiBridge.ToInwOpSelection(modelItemCollectionIn);

                // zoom in to the specified selection
                comState.ZoomInCurViewOnSel(comSelectionOut);
            }
        }

        private InwOpFolderView GetViewpointDestinationFolder(ClashResultStatus clashResult, string clashName, InwOpState10 myState)
        {
            InwOpFolderView folder = null;
            switch (clashResult)
            {
                case ClashResultStatus.Active:
                case ClashResultStatus.New:
                    folder = openIssuesFolder;
                    break;

                case ClashResultStatus.Approved:
                case ClashResultStatus.Resolved:
                    folder = closedIssuesFolder;
                    break;

                case ClashResultStatus.Reviewed:
                    folder = reviewedIssuesFolder;
                    break;
            }

            if (folder != null)
            {
                InwOpFolderView newFolder = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpFolderView) as InwOpFolderView;
                newFolder.name = clashName;
                folder.SavedViews().Add(newFolder);
                return newFolder;
            }
            else
            {
                return null;
            }
        }
        #endregion

        private static InwOpFolderView GetFolder(InwSavedViewsColl rootSavedViews, string folderName)
        {
            foreach (InwOpSavedView savedView in rootSavedViews)
            {
                if (savedView.Type == nwESavedViewType.eSavedViewType_Folder)
                {
                    if (savedView.name == folderName)
                    {
                        return (InwOpFolderView)savedView;
                    }
                    else
                    {
                        var foundFolder = GetFolder(((InwOpFolderView)savedView).SavedViews(), folderName);
                        if (foundFolder != null)
                        {
                            return foundFolder;
                        }
                    }
                }
            }

            return null;
        }

        private static InwOpFolderView GetFolder(InwOpGroupView parentSavedView, string folderName)
        {
            return GetFolder(parentSavedView.SavedViews(), folderName);
        }

        private void HideUnselected(InwOpState10 myState)
        {
            try
            {
                InwOpSelection2 myCurrentSelection = myState.CurrentSelection.Copy() as InwOpSelection2;
                // Create a new empty selection
                InwOpSelection2 myRestOfModel = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpSelection, null, null) as InwOpSelection2;
                // Get the new selection to contain the entire model
                myRestOfModel.SelectAll();
                // Subtract the current selection, so it contains the unselected part of model
                myRestOfModel.SubtractContents(myCurrentSelection);
                // Make the unselected part of model invisible
                myState.set_SelectionHidden(myRestOfModel, true);
                // Zoom on the currently selected part of model
                // myState.ZoomInCurViewOnCurSel();
            }
            catch (Exception loEx1)
            {
                MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }

        private void ShowDefaultViewpoint(InwOpState10 myState)
        {
            if (defaultView == null)
            {

                foreach (InwOpSavedView mySavedView in myState.SavedViews())
                {
                    if (mySavedView.Type == nwESavedViewType.eSavedViewType_View & mySavedView.name == defaulViewpointName)
                    {
                        defaultView = (InwOpView)mySavedView;
                    }                    
                }
            }

            if (defaultView == null)
            {
                MessageBox.Show(string.Format("No '{0}' view at the root level.", defaulViewpointName));
            }
            else
            {
                //Apply the default view to set geometry
                myState.ApplyView(defaultView);
            }
        }

    }
}