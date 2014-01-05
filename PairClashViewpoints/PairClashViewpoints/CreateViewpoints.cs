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
        const string defaulViewpointName = "00. Default";
        const string spatialCoordinationFolderName = "02. Spatial Coordination";
        const string openIssuesFolderName = "01. Open Issues";
        const string closedIssuesFolderName = "02. Closed Issues";
        const string reviewedIssuesFolderName = "03. Reviewed Issues";

        InwOpFolderView openIssuesFolder = null;
        InwOpFolderView closedIssuesFolder = null;
        InwOpFolderView reviewedIssuesFolder = null;

        InwOpView defaultView = null;
        int createViewCount = 0;

        public override int Execute(params string[] parameters)
        {
            InwOpState10 myState = ComApiBridge.State;

            GetAndClearClashFolders(myState);
            Stopwatch totalTime = new Stopwatch();
            totalTime.Start();
            ParseClash(myState);
            totalTime.Stop();
            MessageBox.Show(Autodesk.Navisworks.Api.Application.Gui.MainWindow, "Done.  Elapsed: " + totalTime.Elapsed.TotalSeconds + "s");
            return 0;
        }

        private void GetAndClearClashFolders(InwOpState10 myState)
        {
            var spatialCoordinationFolder = GetFolder(myState.SavedViews(), spatialCoordinationFolderName);
            openIssuesFolder = GetFolder(spatialCoordinationFolder, openIssuesFolderName);
            openIssuesFolder.SavedViews().Clear();
            closedIssuesFolder = GetFolder(spatialCoordinationFolder, closedIssuesFolderName);
            closedIssuesFolder.SavedViews().Clear();
            reviewedIssuesFolder = GetFolder(spatialCoordinationFolder, reviewedIssuesFolderName);
            reviewedIssuesFolder.SavedViews().Clear();
        }

        private void ParseClash(InwOpState10 myState)
        {
            try
            {
                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
                createViewCount = 0;
                foreach (ClashTest test in oDoc.GetClash().TestsData.Tests)
                {
                    foreach (IClashResult result in test.Children)
                    {
                        if (result is ClashResultGroup)
                        {
                            CreateViewPointSet(myState, result);
                        }
                        ((NativeHandle)result).Dispose();                            
                    }

                    test.Dispose();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CreateViewPointSet(InwOpState10 myState, IClashResult result)
        {
            Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;
            InwOpFolderView viewPointSaveFolder = GetViewpointDestinationFolder(result.Status, result.DisplayName, myState);
            if (viewPointSaveFolder != null)
            {
                try
                {
                    Stopwatch stopWatch = new Stopwatch();
                    stopWatch.Start();
                    Debug.Write(string.Format("Starting #{0} - {1}", ++createViewCount, result.DisplayName));
                    oDoc.BeginTransaction 
                    ShowDefaultViewpoint(myState);
                    oDoc.CurrentSelection.Clear();
                    oDoc.CurrentSelection.AddRange(result.Selection1);
                    oDoc.CurrentSelection.AddRange(result.Selection2);
                    ZoomCurrentSelection();

                    CreateViewPointFromCurrentView(myState, "All", viewPointSaveFolder);

                    oDoc.CurrentSelection.Clear();
                    oDoc.CurrentSelection.AddRange(result.Selection1);
                    oDoc.CurrentSelection.AddRange(result.Selection2);
                    HideUnselected(myState);

                    CreateViewPointFromCurrentView(myState, "Isolated", viewPointSaveFolder);
                    stopWatch.Stop();
                    Debug.WriteLine(" Elapsed: {0}s", stopWatch.Elapsed.TotalSeconds);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
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
                myState.ZoomInCurViewOnCurSel();
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