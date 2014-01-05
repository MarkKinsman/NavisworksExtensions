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

namespace PairClashViewpoints
{
    [PluginAttribute("PairClashViewpoints.CreateViewpoint",
         "MORT",
         ToolTip = "Create a Viewpoint",
         DisplayName = "Create Viewpoint")]

    public class CreateViewpoint : AddInPlugin
    {
        const string spatialCoordinationFolderName = "02. Spatial Coordination";
        const string openIssuesFolderName = "01. Open Issues";
        const string closedIssuesFolderName = "02. Closed Issues";
        const string reviewedIssuesFolderName = "03. Reviewed Issues";

        public override int Execute(params string[] parameters)
        {
            InwOpState10 myState = ComApiBridge.State;

            ParseClash(myState);

            return 0;
        }

        private void ParseClash(InwOpState10 myState)
        {
            try
            {
                Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

                foreach (ClashTest test in oDoc.GetClash().TestsData.Tests)
                {
                    foreach (IClashResult result in test.Children)
                    {
                        if (result is ClashResultGroup)
                        {
                            CreateViewPointSet(myState, result);
                        }
                    }
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
                    oDoc.CurrentSelection.Clear();
                    oDoc.CurrentSelection.AddRange(result.Selection1);
                    oDoc.CurrentSelection.AddRange(result.Selection2);
                    ZoomCurrentSelection();

                    CreateViewPointFromCurrentView(myState, "All", viewPointSaveFolder);

                    CreateViewPointFromCurrentView(myState, "Isolated", viewPointSaveFolder);
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

        private static void CreateViewPointFromCurrentView(InwOpState10 myState, string name, InwOpFolderView viewPointSaveFolder)
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
            ModelItemCollection modelItemCollectionIn = new ModelItemCollection(Autodesk.Navisworks.Api.Application.ActiveDocument.CurrentSelection.SelectedItems);

            InwOpSelection comSelectionOut = ComApiBridge.ToInwOpSelection(modelItemCollectionIn);

            // zoom in to the specified selection
            comState.ZoomInCurViewOnSel(comSelectionOut);

        }


        #region GetFolderCom
        private static InwOpFolderView GetViewpointDestinationFolder(ClashResultStatus clashResult, string clashName, InwOpState10 myState)
        {

            var spatialCoordinationFolder = GetFolder(myState.SavedViews(), spatialCoordinationFolderName);
            
            string folderName = null;
            switch (clashResult)
            {
                case ClashResultStatus.Active:
                case ClashResultStatus.New:
                    folderName = openIssuesFolderName;
                    break;

                case ClashResultStatus.Approved:
                case ClashResultStatus.Resolved:
                    folderName = closedIssuesFolderName;
                    break;

                case ClashResultStatus.Reviewed:
                    folderName = reviewedIssuesFolderName;
                    break;
            }

            InwOpGroupView foundFolder = GetFolder(spatialCoordinationFolder, folderName);
            if (foundFolder != null)
            {
                InwOpFolderView newFolder = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpFolderView) as InwOpFolderView;
                newFolder.name = clashName;
                foundFolder.SavedViews().Add(newFolder);
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


        private InwOpSelection2 GetDefault(InwOpState10 myState)
        {
            try
            {
                foreach (InwOpSavedView mySavedView in myState.SavedViews())
                {
                    if (mySavedView.Type == nwESavedViewType.eSavedViewType_View & mySavedView.name == "00. Default")
                    {
                        //Apply the default view to grab visible objects
                        myState.ApplyView((InwOpView)mySavedView);
                        //Create a selection of all visible items
                        InwOpSelection2 myDefaultSelection = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpSelection, null, null) as InwOpSelection2;
                        myDefaultSelection.SelectAll();

                        myState.set_SelectionHidden(myDefaultSelection, true);
                        MessageBox.Show("Set Default Hide");
                        myState.set_SelectionHidden(myDefaultSelection, false);
                        MessageBox.Show("Set default Show");
                        return myDefaultSelection;
                    }
                }
                MessageBox.Show(string.Format("No '{0}' view at the root level.", "00. Default"));

            }
            catch (Exception loEx1)
            {
                MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
            return null;
        }

        #endregion


        private void HideUnselected(InwOpSelection2 myCurrentSelection, InwOpState10 myState)
        {
            try
            {
                // Create a new empty selection
                InwOpSelection2 myRestOfModel = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpSelection, null, null) as InwOpSelection2;
                // Get the new selection to contain the entire model
                myRestOfModel.SelectAll();
                // Subtract the current selection, so it contains the unselected part of model
                myRestOfModel.SubtractContents(myCurrentSelection);
                // Make the unselected part of model invisible
                myState.set_SelectionHidden(myRestOfModel, true);
            }
            catch (Exception loEx1)
            {
                MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }

        private void ShowDefault(InwOpSelection2 myDefaultSelection, InwOpState10 myState)
        {
            myState.set_SelectionHidden(myDefaultSelection, true);
            MessageBox.Show("Show method Hide");
            myState.set_SelectionHidden(myDefaultSelection, false);
            MessageBox.Show("Show Method Show");
            // Create a new empty selection
            InwOpSelection2 myRestOfModel = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpSelection, null, null) as InwOpSelection2;

            myState.set_SelectionHidden(myDefaultSelection, true);
            MessageBox.Show("Show method2 Hide");
            myState.set_SelectionHidden(myDefaultSelection, false);
            MessageBox.Show("Show Method2 Show");

            // Get all the visible elements
            myRestOfModel.SelectAll();
            // Make the unselected part of model invisible
            myState.set_SelectionHidden(myRestOfModel, true);
            MessageBox.Show("Hide");
            // Show the default Selection
            myState.set_SelectionHidden(myDefaultSelection, false);
            MessageBox.Show("Show");
        }
    }
}