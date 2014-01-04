﻿using System;
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
        public override int Execute(params string[] parameters)
        {
            InwOpState10 myState = ComApiBridge.State;

            ParseClash(myState);

            /* InwOpSelection2 myDefaultView = GetDefault(myState);   ModelItemCollection
            System.Diagnostics.Debugger.Break();
            myState.set_SelectionHidden(myDefaultView, true);
            MessageBox.Show("Main method Hide");
            myState.set_SelectionHidden(myDefaultView, false);
            MessageBox.Show("Main Method Show");

            foreach(InwOpSavedView oEachItem in myState.SavedViews())
            {
                if (oEachItem.name == "00. Reset Appearance")
                {
                    myState.ApplyView((InwOpView)oEachItem);
                }
            }

            myState.set_SelectionHidden(myDefaultView, true);
            MessageBox.Show("Main method2 Hide");
            myState.set_SelectionHidden(myDefaultView, false);
            MessageBox.Show("Main Method2 Show");

            ShowDefault(myDefaultView, myState); */
            return 0;
        }

        private void ParseClash(InwOpState10 myState)
        {
            try
            {
                //find the clash detective plugin
                InwOpClashElement m_clash = GetClashPlugin(myState);
                if (m_clash != null)
                {
                    /*//Iterate through every test
                    foreach (InwOclClashTest oClashTest in m_clash.Tests())
                    {
                        foreach (InwOclTestResult oClashResult in oClashTest.results())
                        {
                            CreateViewpointSet(myState, oClashResult);
                        }
                    }*/

                    InwOclClashTest clashTest = m_clash.Tests()[1];
                    InwOclTestResult clashResult = clashTest.results()[1];
                    CreateViewpointSet(myState, clashResult);
                }
            }
            catch (Exception loEx1)
            {
                MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }

        private static InwOpClashElement GetClashPlugin(InwOpState10 myState)
        {
            InwOpClashElement m_clash = null;
            foreach (InwBase oPlugin in myState.Plugins())
            {
                if (oPlugin.ObjectName == "nwOpClashElement")
                {
                    m_clash = (InwOpClashElement)oPlugin;
                    break;
                }
            }

            if (m_clash == null)
            {
                MessageBox.Show("cannot find clash test plugin!");
            }
            return m_clash;
        }

        /*private void CreateViewpointSet(InwOpState10 myState)
        {
            try
            {
                // assume the document has a saved viewpoint folder named  "02. Spactial Coordination"
                // inside that folder is "01. Open Issues" "02. Closed Issues" "03. Reviewed"

                // get the saved viewpoints
                InwSavedViewsColl oSavePts = myState.SavedViews();

                //Follow the tree to the right folder
                InwOpSavedView oFolder = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpFolderView, null, null) as InwOpSavedView;
                foreach (InwOpSavedView oEachItem in oSavePts)
                {
                    if (oEachItem.Type == nwESavedViewType.eSavedViewType_Folder & oEachItem.name == "02. Spatial Coordination")
                    {

                        foreach (InwOpSavedView oEachItem2 in ((InwOpGroupView)oEachItem).SavedViews())
                        {
                            if (oEachItem2.Type == nwESavedViewType.eSavedViewType_Folder & oEachItem2.name == "01. Open Issues")
                            {
                                //Capture the folder including the position
                                oFolder = oEachItem2;
                                break;
                            }
                        }

                    }
                }
                        // Create a new viewpoint
                        InwOpView cNewViewPt1 = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpView, null, null) as InwOpView;
                        cNewViewPt1.anonview = myState.CurrentView;
                        // Set Hide/Required to true
                        cNewViewPt1.ApplyHideAttribs = true;
                        // Set name
                        cNewViewPt1.name = "Display Name";

                        long[] test = new long[2] { 1, 0 };
                        // add the new saved viewpoint to the collection
                        //myState.SavedViews().Add(cNewViewPt1);
                        myState.SavedViews().Insert(test, cNewViewPt1);
                    
                        GC.KeepAlive(myState);
                        GC.KeepAlive(oFolder);
            }
            catch (Exception loEx1)
            {
                MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            }
        }*/

        const string spatialCoordinationFolderName = "02. Spatial Coordination";
        const string openIssuesFolderName = "01. Open Issues";
        const string closedIssuesFolderName = "02. Closed Issues";
        const string reviewedIssuesFolderName = "03. Reviewed Issues";


        private void CreateViewpointSet(InwOpState10 myState, InwOclTestResult oClash)
        {
            // assume the document has a saved viewpoint folder named  "02. Spactial Coordination"
            // inside that folder is "01. Open Issues" "02. Closed Issues" "03. Reviewed"

            // get document
            Document oDoc = Autodesk.Navisworks.Api.Application.ActiveDocument;

            // get the saved viewpoints
            Autodesk.Navisworks.Api.DocumentParts.DocumentSavedViewpoints oSavePts = oDoc.SavedViewpoints;

            // Find the folder it belongs to
            GroupItem oFolder = GetViewpointDestinationFolder(oClash, oSavePts);
            if (oFolder != null)
            {
                try
                {                   
                    // create a saved viewpoint
                    InwOpView oClashViewpoint = myState.ObjectFactory(nwEObjectType.eObjectType_nwOpView);

                    // Set viewpoint camera and apply it
                    oClashViewpoint.anonview.ViewPoint = oClash.ViewPoint;
                    myState.ApplyView(oClashViewpoint);
                    
                    // Name the Viewpoint
                    oClashViewpoint.name = oClash.name + " - All";
                    // Set Hide/Required of viewpoint
                    oClashViewpoint.ApplyHideAttribs = true;
                    myState.SavedViews().Add(oClashViewpoint);
                    oFolder.Children.Add(oClashViewpoint);
                    // move the last saved viewpoint to the folder
                    oSavePts.Move(oSavePts.RootItem, oSavePts.Value.Count - 1, oFolder, oFolder.Children.Count);


                    // Name the Viewpoint
                    oClashViewpoint.name = oClash.name + " - Isolated";
                    // Set Hide/Required of viewpoint
                    oClashViewpoint.ApplyHideAttribs = true;
                    myState.SavedViews().Add(oClashViewpoint);

                    // move the last saved viewpoint to the folder
                    oSavePts.Move(oSavePts.RootItem, oSavePts.Value.Count - 1, oFolder, oFolder.Children.Count);

                }
                catch (Exception loEx1)
                {
                    MessageBox.Show(String.Format("Exception caught : '{0}'.", loEx1.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                }
            }
        }

        private static GroupItem GetViewpointDestinationFolder(InwOclTestResult oClash, Autodesk.Navisworks.Api.DocumentParts.DocumentSavedViewpoints oSavePts)
        {
            var spatialCoordinationFolderIndex = oSavePts.Value.IndexOfDisplayName(spatialCoordinationFolderName);
            if (spatialCoordinationFolderIndex >= 0)
            {
                var spatialCoordinationFolder = (GroupItem)oSavePts.Value[spatialCoordinationFolderIndex];
                switch (oClash.status)
                {
                    case nwETestResultStatus.eTestResultStatus_ACTIVE:
                    case nwETestResultStatus.eTestResultStatus_NEW:
                        return GetGroupItem(spatialCoordinationFolder, openIssuesFolderName);
                       
                    case nwETestResultStatus.eTestResultStatus_APPROVED:
                    case nwETestResultStatus.eTestResultStatus_RESOLVED:
                        return GetGroupItem(spatialCoordinationFolder, closedIssuesFolderName);
                        
                    case nwETestResultStatus.eTestResultStatus_REVIEWED:
                        return GetGroupItem(spatialCoordinationFolder, reviewedIssuesFolderName);  
                }
            }
            return null;
        }

        private static GroupItem GetGroupItem(GroupItem spatialCoordinationFolder, string folderName)
        {
            var folderIndex = spatialCoordinationFolder.Children.IndexOfDisplayName(folderName);
            if (folderIndex >= 0)
            {
                return spatialCoordinationFolder.Children[folderIndex] as GroupItem;
            }
            else
            {
                MessageBox.Show("Missing Folder: " + folderName);
                return null;
            }
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