// #define timers
//#define layoutTimer

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Data;
//using System.Threading ;
using System.Text ;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions ;
using System.Runtime.Serialization ;
using System.Runtime.Serialization.Formatters.Binary ;

namespace ConcorDancer
{
	public class
	ConcorDancerTabControl : TabControl
	{
		public void
		NextSelectedIndex ()
		{
			if ( SelectedIndex > 0 ) SelectedIndex -= 1 ;
			else if ( TabPages.Count > 1 ) SelectedIndex = 1 ;
			//else SelectedIndex = -1 ;
		}
	}

	public partial class 
	ConcorDancerModel
	{
        public string ProgramName = "ConcorDancer";
        public string ConcorDancerVersion = "2.6.8.13 © 2013 integralcreativity@yahoo.com";

        public DLList<StringFindElement> FindList = new DLList<StringFindElement>();
        Stack Stack = new Stack();
        private CdmState CurrentState = new CdmState();
        public string AppendFileName = "append.txt";
        public DLList<MenuAppendFileElement> AppendFileList = new DLList<MenuAppendFileElement>();
        public int OpenFileDialogFilterIndex = 1; // Ascii (*.txt) = default
        public FileManagerTabPage Fmtp;
        public StringBuilder AppendString;
        public bool RestoreStateFlag = false, SortFlag = false;
        public string CurrentDirectory = ".";
        public int InsertPoint;
        public int WordsBack;
#if timers
        public Timer ResizeTimer = new Timer();
        bool InResizeEvent = false;
#if layoutTimer
            public Timer LayoutTimer = new Timer();
            bool InLayoutEvent = false;
#endif
#endif
        public void
        SetWindowTextWithActiveFileManager( string text )
        {
            ConcorDancerWinForm cdwf = ConcorDancer.Cdwf;
            if (cdwf.ShowVersionMenuItem.Checked)
            {
                //State.CurrentFileManagerTabPageAdjustDisplayGuard = true;
                cdwf.Text = String.Format("{0} [ {1} ]", text, ConcorDancerVersion);
            }
            else
            {
                cdwf.Text = String.Format("{0}", text);
            }
        }

        public void
        SetWindowTitle()
        {
            try
            {
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                ConcorDancerWinForm cdwf = ConcorDancer.Cdwf;
                string fullPathFilenameWithModifiedIndicator = "", filename = "";
                bool modified = false;
                if (ctp != null)
                {
                    filename = ConcorDancer.MakeFilenameFromFullPathName(ctp.CurrentTextBox.FullPathFilename);
                    foreach (ConcorDancerTextBox ctb in ctp.TextBoxArrayList)
                    {
                        if (ctb.Modified == true)
                        {
                            modified = true;
                            break;
                        }
                    }
                    fullPathFilenameWithModifiedIndicator = String.Format(" {0}{1}", ctp.CurrentTextBox.FullPathFilename,
                        (modified ? " : [Modified]" : ""));
                    if (cdwf.ShowVersionMenuItem.Checked)
                    {
                        cdwf.Text = String.Format("{0} {1} : {2}  [{3}]", ProgramName,
                            ConcorDancerVersion, filename, fullPathFilenameWithModifiedIndicator);
                    }
                    else
                    {
                        cdwf.Text = String.Format("{0}  [{1}]", filename, fullPathFilenameWithModifiedIndicator);
                    }
                    //ctp.FullPathFilenameWithModifiedIndicator = fullPathFilenameWithModifiedIndicator;

                    if (ctp.Text == "" || ctp.Text == "... reading file ...") // from : DataGridView_CellClick 

                    {
                        string text;
                        int count = ctp.CountSameFilenameOnTabs();
                        if (count > 1) text = String.Format("{0} ({1})", filename, count);
                        else text = filename;
                        ctp.Text = text;
                    }
                }
                else // if ( ctp == null )
                {
                    SetWindowTextWithActiveFileManager(ConcorDancer.Cdm.ProgramName);
                }
                //cdwf.AdjustDisplay();
                //cdwf.Refresh();
                Refresh();
            }
            catch (Exception e)
            {
                HandleException(e, false);
            }
        }

        public ConcorDancerTabPage
        CurrentConcorDancerTabPage
        {
            get
            {
                TabPage tp = CurrentSelectedTabPage;
                if (tp != null)
                {
                    if (tp.GetType().FullName == "ConcorDancer.FileManagerTabPage")
                    {
                        return null;
                    }
                }
                return (ConcorDancerTabPage)tp;
            }
            set
            {
                do
                {
                    CurrentSelectedTabPage = value;
                }
                while (CurrentSelectedTabPage != value);
            }
        }

        public TabPage
        CurrentSelectedTabPage
        {
            get
            {
                try
                {
                    return ConcorDancer.Cdwf.TabControl.SelectedTab;
                }
                catch //() //(Exception ex)
                {
                    //HandleException(ex, true);
                    return null;
                }
            }

            set
            {
                try
                {
                    do
                    {
                        ConcorDancer.Cdwf.TabControl.SelectTab((TabPage)value);
                    }
                    while (ConcorDancer.Cdwf.TabControl.SelectedTab != (TabPage)value);
                }
                catch (Exception ex)
                {
                    HandleException(ex, true);
                }
            }
        }

        public void
        PushState()
        {
            CdmState saved = CurrentState.MemberWiseClone();
            Stack.Push(saved);
        }

        public void
        PushNewState()
        {
            CdmState saved = new CdmState();
            Stack.Push(saved);
        }

        public void
        PopState()
        {
            CurrentState = (CdmState)Stack.Pop();
        }

        public void
        ResetState()
        {
            CurrentState = new CdmState();
        }

        public CdmState
        State
        {
            get { return CurrentState; }
        }

        public FileManagerTabPage 
		CurrentFileManagerTabPage 
		{
			get
			{
				foreach ( TabPage tp in ConcorDancer.Cdwf.TabControl.TabPages )
				{
					if (  tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
					{
						return (FileManagerTabPage) tp ;
					}
				}
				return null ;
			}
		}

		public void
		NewFileManagerTab ()
		{
			try
			{
				if ( CurrentFileManagerTabPage != null ) return;
				FileManagerTabPage fmtp = new FileManagerTabPage ();
				ConcorDancer.Cdm.Fmtp = fmtp ;
				ConcorDancer.Cdwf.TabControl.Controls.Add ( fmtp ) ;
				ConcorDancer.Cdwf.TabControl.SelectedTab = fmtp ;
				fmtp.AddDirectoryTabPage (  "C:\\" ) ;

			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		NewFileManagerMenuItem_Click ()
		{
			NewFileManagerTab () ;
		}

		public void
		UnsplitTextBoxMenuItem_Click ()
		{
			try
			{
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.UnsplitTextBox ( ) ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		VerticalTextBoxSplitMenuItem_Click ( )
		{
			try
			{
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.SplitTextBox (  Orientation.Vertical ) ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		HorizontalTextBoxSplitMenuItem_Click ( )
		{
			try
			{
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.SplitTextBox (  Orientation.Horizontal ) ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		BackButton_Click ()
		{
			try
			{
				ConcorDancer.Cdm.Back () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		ForwardButton_Click ()
		{
			try
			{
				ConcorDancer.Cdm.Forward () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

        // respond to append file menu selection item
        public void
        AppendFileMenuFileItem_Click(Object sender)
        {
            try
            {
                //foreach (MenuAppendFileElement mafe in AppendFileList)
                foreach (DLLNode<MenuAppendFileElement> mafe in AppendFileList) 
                {
                    if (mafe.Value.MafeMenuItemText == ((MenuItem)sender).Text)
                    {
                        CurrentConcorDancerTabPage._SaveAppendFile(mafe.Value.MafeFileName);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }

        public void
		SortFunctionMenuItem_Click ()
		// one shot sort ; maintain option state
		{
			try
			{
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.ListBox.SortFlag = true ;
				ctp.ListBox.WordsBack = WordsBack ;
				//State.calculateAndDisplayListBox = true ;
                ctp.ListBox.BuildAndDisplayListBoxItemStringArray();
				ctp.ListBox.ClearSelected () ;
				//ctp.textBox1.Select ( 0, 0 ) ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		SortOptionMenuItem_Click ()
		{
			try
			{
				if ( ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true )
				{
					ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked = false ;
					SetSortOptionMenuItemTextEtc ( false, WordsBack ) ;
				}
				else
				{
                    SortFunctionMenuItem_Click();
					ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked = true ;
					SetSortOptionMenuItemTextEtc ( true, WordsBack ) ;

                    /*
					ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
					ctp.ListBox.ListBoxSelectedIndex = -1 ;
					ctp.ListBox.BuildAndDisplayListBoxItemStringArray() ;
                    */
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		Start0WordsBackMenuItem_Click ()
		{
			try
			{
				if ( ConcorDancer.Cdwf.Start0WordsBackMenuItem.Checked == true )
				{
					ConcorDancer.Cdwf.Start0WordsBackMenuItem.Checked = false ;
					SetSortOptionMenuItemTextEtc ( false, 0 ) ;
				}
				else
				{
					ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
					ConcorDancer.Cdwf.Start0WordsBackMenuItem.Checked = true ;
					SetSortOptionMenuItemTextEtc ( true, 0 ) ;
                    SortFunctionMenuItem_Click();
				}
            }
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		Start1WordBackMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.Start1WordBackMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.Start1WordBackMenuItem.Checked = false ;
				SetSortOptionMenuItemTextEtc ( false, 0 ) ;
			}
			else
			{
				ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
				ConcorDancer.Cdwf.Start1WordBackMenuItem.Checked = true ;
				SetSortOptionMenuItemTextEtc ( true, 1 ) ;
                SortFunctionMenuItem_Click();
			}
        }

		public void
		Start2WordsBackMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.Start2WordsBackMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.Start2WordsBackMenuItem.Checked = false ;
				SetSortOptionMenuItemTextEtc ( false, 0 ) ;
			}
			else
			{
				ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
				ConcorDancer.Cdwf.Start2WordsBackMenuItem.Checked = true ;
				SetSortOptionMenuItemTextEtc ( true, 2 ) ;
                SortFunctionMenuItem_Click();
			}
        }

		public void
		Start3WordsBackMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.Start3WordsBackMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.Start3WordsBackMenuItem.Checked = false ;
				SetSortOptionMenuItemTextEtc ( false, 0 ) ;
			}
			else
			{
				ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
				ConcorDancer.Cdwf.Start3WordsBackMenuItem.Checked = true ;
				SetSortOptionMenuItemTextEtc ( true, 3 ) ;
                SortFunctionMenuItem_Click();
			}
        }

		public void
		Start4WordsBackMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.Start4WordsBackMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.Start4WordsBackMenuItem.Checked = false ;
				SetSortOptionMenuItemTextEtc ( false, 0 ) ;
			}
			else
			{
				ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
				ConcorDancer.Cdwf.Start4WordsBackMenuItem.Checked = true ;
				SetSortOptionMenuItemTextEtc ( true, 4 ) ;
                SortFunctionMenuItem_Click();
			}
        }

		public void
		Start5WordsBackMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.Start5WordsBackMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.Start5WordsBackMenuItem.Checked = false ;
				SetSortOptionMenuItemTextEtc ( false, 0 ) ;
			}
			else
			{
				ConcorDancer.Cdwf.UncheckStartWordsBackMenuItems () ; // reset
				ConcorDancer.Cdwf.Start5WordsBackMenuItem.Checked = true ;
				SetSortOptionMenuItemTextEtc ( true, 5 ) ;
                SortFunctionMenuItem_Click();
			}
        }

		public void
		MultiColumnFileListMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked = false ;
                if (ConcorDancer.Cdm.Fmtp != null)
                {
                    ConcorDancer.Cdm.Fmtp.AddDirectoryAndFilesToDataGridViewDetailsMode();
                }
			}
			else
			{
				ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked = true ;
                if (ConcorDancer.Cdm.Fmtp != null)
                {
                    ConcorDancer.Cdm.Fmtp.AddDirectoryAndFilesToDataGridViewListMode();
                }
			}
		}

		public void
		FinderComboBox_KeyUp ( KeyEventArgs e )
		{
			try
			{
				if ( e.KeyCode == Keys.Enter )
				{
					DisplayConcordanceList () ;
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		CaseSensitiveFindsMenuItem_Click ()
		{
            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
            if (ConcorDancer.Cdwf.CaseSensitiveFindsMenuItem.Checked == true)
			{
				ConcorDancer.Cdwf.CaseSensitiveFindsMenuItem.Checked = false ;
                if (ctp == null) return;
                ctp.RegularExpressionOptions = RegexOptions.IgnoreCase;
			}
			else
			{
				ConcorDancer.Cdwf.CaseSensitiveFindsMenuItem.Checked = true ;
                if (ctp == null) return;
                ctp.RegularExpressionOptions = RegexOptions.None;
			}
		}
		 
		public void
		ToggleListBoxFunctionMenuItem_Click ()
		{
			try
			{
                TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                if (tp == null) return;
				if (  tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
				{
					FileManagerTabPage fmtp = (FileManagerTabPage) tp ; 
					if ( fmtp.FileManagerSplitContainer.Panel1Collapsed == true )
					{
						fmtp.FileManagerSplitContainer.Panel1Collapsed = false ;
						//ConcorDancer.Cdwf..toggleListBoxFunctionMenuItem.Text = "Co&llapse List Box";
					}
					else 
					{
						fmtp.FileManagerSplitContainer.Panel1Collapsed = true ;
						//ConcorDancer.Cdwf.toggleListBoxFunctionMenuItem.Text = "&Show List Box";
					}
				}
				else
				{
					ConcorDancerTabPage ctp = (ConcorDancerTabPage) tp ;
					if ( ctp.SplitContainer.Panel1Collapsed == true )
					{
						ctp.SplitContainer.Panel1Collapsed = false ;
						//ConcorDancer.Cdwf..toggleListBoxFunctionMenuItem.Text = "Co&llapse List Box";
					}
					else 
					{
						ctp.SplitContainer.Panel1Collapsed = true ;
						//ConcorDancer.Cdwf.toggleListBoxFunctionMenuItem.Text = "&Show List Box";
					}
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		RestoreStateMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.RestoreStateMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.RestoreStateMenuItem.Checked = false ;
				RestoreStateFlag = false ;
			}
			else
			{
				ConcorDancer.Cdwf.RestoreStateMenuItem.Checked = true ;
				RestoreStateFlag = true ;
			}
		}

		public void
		ShowVersionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.ShowVersionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.ShowVersionMenuItem.Checked = false ;
			}
			else
			{
				ConcorDancer.Cdwf.ShowVersionMenuItem.Checked = true ;
			}
			SetWindowTitle () ;
		}

		public void
		ShowListBoxHorizontalScrollBarMenuItem_Click ()
		{
            if (ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked == true)
			{
				ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked = false ;
				//ConcorDancer.Cdwf.setListBoxViewCenteredOptionMenuItem.Checked = true ;
			}
			else
			{
				//ConcorDancer.Cdwf.setListBoxViewCenteredOptionMenuItem.Checked = false ;
				ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked = true ;
			}
            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
            if (ctp == null) return;
            ctp.ListBox.HorizontalScrollbar = 
				ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked ;
			ctp.ListBox.BuildAndDisplayListBoxItemStringArray() ;
		}

		// Menu.Options.Read Only
		public void
		ReadOnlyMenuItem_Click()
		{
			try
			{
				if ( ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked == true )
				{
					ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked = false ;
				}
				else ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked = true ;
				//foreach ( ConcorDancerTabPage ctp in ConcorDancerTabPageList.List )
				{
					ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
					foreach ( Object o in ctp.TextBoxArrayList )  
						((ConcorDancerTextBox)o).ReadOnly = 
						ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked ; 
					//ctp.textBox1.ReadOnly = ConcorDancer.Cdwf.readOnlyMenuItem.Checked ;
					//ctp.textBox1.Refresh () ;
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		// set opton to Find words anywhere within a sentence
		public void
		FindWordsAnyOrderMenuItem_Click()
		{
			if ( ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked = false ;
			}
			else ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked = true ;
			if ( ConcorDancer.Cdm.CurrentConcorDancerTabPage != null )
			{
				ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.FindWordsAnyOrderWithinSentenceFlag =
					ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked ;
			}
		}

		// set opton to Find words anywhere within a sentence
		public void
		FindWordsAnyPlaceMenuItem_Click()
		{
			if ( ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked = false ;
			}
			else ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked = true ;
			ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
			if ( ctp != null )
			{
				ctp.ListBox.FindWordsAnyPlaceWithinSentenceFlag =
					ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked ;
			}
		}

		public void
		DeleteCurrentFindListMenuItem_Click ()
		{
			try
			{
				ConcorDancer.Cdwf.ComboBoxForHistory.ClearList () ;
				// reset the list after the Clear () above
				ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.SelectedIndex = -1 ; // -1 : no item is selected
				FindList.Reset () ; 
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		RefreshDisplayMenuItem_Click ()
		{
			try
			{
				//ConcorDancer.Cdwf.SetBoundariesAndLocationsForBoxes () ;
				//ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.BuildAndDisplayListBoxItemStringArray () ;
				Refresh () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		RefreshListMenuItem_Click ()
		{
			try
			{
                //Refresh();
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.BuildAndDisplayListBoxItemStringArray();
                ConcorDancer.Cdwf.ComboBoxForHistory.AdjustDisplay();
				Refresh () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		ShiftFocusToTextMenuItem_Click()
		{
			if ( ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked = false ;
				ConcorDancer.Cdwf.KeepListFocusMenuItem.Checked = true ;
			}
			else
			{
				ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked = true ;
				ConcorDancer.Cdwf.KeepListFocusMenuItem.Checked = false ;
			}
		}

		public void
		KeepListFocusMenuItem_Click()
		{
			if ( ConcorDancer.Cdwf.KeepListFocusMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.KeepListFocusMenuItem.Checked = false ;
				ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked = true ;
			}
			else
			{
				ConcorDancer.Cdwf.KeepListFocusMenuItem.Checked = true ;
				ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked = false ;
			}
		}

		public void
		UseRegularExpressionsMenuItem_Click()
		{
			if ( ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked = false ;
			}
			else
			{
				ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked = true ;
			}
		}

		public void
		RemoveHtmlTagsOptionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.RemoveHtmlTagsOptionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.RemoveHtmlTagsOptionMenuItem.Checked = false ;
			}
			else
			{
				ConcorDancer.Cdwf.RemoveHtmlTagsOptionMenuItem.Checked = true ;
			}
		}

		public void
		RemoveHtmlAnchorsOptionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.RemoveHtmlAnchorsOptionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.RemoveHtmlAnchorsOptionMenuItem.Checked = false ;
			}
			else
			{
				ConcorDancer.Cdwf.RemoveHtmlAnchorsOptionMenuItem.Checked = true ;
			}
		}

		public void
		InsertFileNamesOnJoinOptionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.insertFileNamesOnJoinOptionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.insertFileNamesOnJoinOptionMenuItem.Checked = false ;
			}
			else
			{
				ConcorDancer.Cdwf.insertFileNamesOnJoinOptionMenuItem.Checked = true ;
			}
		}

		public void
		SetListBoxViewCenteredOptionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.SetListBoxViewCenteredOptionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.SetListBoxViewCenteredOptionMenuItem.Checked = false ;
			}
			else
			{
				//ConcorDancer.Cdwf.showListBoxHorizontalScrollBarMenuItem.Checked = false ;
				ConcorDancer.Cdwf.SetListBoxViewCenteredOptionMenuItem.Checked = true ;
			}
            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
            if (ctp == null) return;
			ctp.ListBox.BuildAndDisplayListBoxItemStringArray() ;
		}

		public void
		RecurseFoldersOptionMenuItem_Click ()
		{
			if ( ConcorDancer.Cdwf.RecurseFoldersOptionMenuItem.Checked == true )
			{
				ConcorDancer.Cdwf.RecurseFoldersOptionMenuItem.Checked = false ;
				//openFolderMenuItem.Text = "Open All Files in Folder" ;
				ConcorDancer.Cdwf.OpenFolderMenuItem.Text = "O&pen - Join Files - Extension Selected in Find Box";
			}
			else
			{
				ConcorDancer.Cdwf.RecurseFoldersOptionMenuItem.Checked = true ;
				//openFolderMenuItem.Text = "Open - Join Files - Recursively" ;
				ConcorDancer.Cdwf.OpenFolderMenuItem.Text =
				"O&pen - Join Files - Including SubDirectories Recursively";
			}
		}

		public void
		FindSelectedTextMenuItem_Click()
		{
			try
			{
				DisplayConcordanceList () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		BackMenuItem_Click()
		{
			try
			{
				ConcorDancer.Cdm.Back () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		ForwardMenuItem_Click()
		{
			try
			{
				ConcorDancer.Cdm.Forward () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

        public void
        MakeBookmarkMenuItem_Click()
        {
            try
            {
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.MakeBookmark();
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }
/*
        public void
        FirstBookmarkMenuItem_Click()
        {
            try
            {
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.FirstBookmark();
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }
*/
        public void
        NextBookmarkMenuItem_Click()
        {
            try
            {
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.NextBookmark();
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }

        public void
        PreviousBookmarkMenuItem_Click()
        {
            try
            {
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.PreviousBookmark();
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }

        public void
		SetBackColor ( Control control ) 
		{
			try
			{
				ColorDialog colorDialog = new ColorDialog () ;
				colorDialog.AllowFullOpen = true;
				colorDialog.AnyColor = true;
				colorDialog.Color = control.BackColor ;
				if( colorDialog.ShowDialog() != DialogResult.Cancel )
				{
					control.BackColor = colorDialog.Color ;
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		SetListBoxBackgroundColorMenuItem_Click () 
		{
			try
			{
				SetBackColor ( ConcorDancer.CurrentListBox  ) ;
				ConcorDancer.ListBoxBackColor = ConcorDancer.CurrentListBox.BackColor ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		SetFileListBackgroundColorMenuItem_Click () 
		{
			try
			{
				SetBackColor ( ConcorDancer.CurrentDataGridView  ) ;
				ConcorDancer.FileListBackColor = ConcorDancer.CurrentDataGridView.BackColor ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		SetDirectoryTreeBackgroundColorMenuItem_Click () 
		{
			try
			{
				SetBackColor ( ConcorDancer.CurrentTreeView  ) ;
				ConcorDancer.DirectoryTreeBackColor = ConcorDancer.CurrentTreeView.BackColor ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		SetTextBoxBackgroundColorMenuItem_Click () 
		{
			try
			{
				SetBackColor ( ConcorDancer.CurrentTextBox  ) ;
				ConcorDancer.TextBoxBackColor = ConcorDancer.CurrentTextBox.BackColor ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
/*		
		private void 
		fontDialog_Apply(object sender, System.EventArgs e)
		{
		}
*/	
		public void
		SetFont ( Control control ) 
		{
			try
			{
				FontDialog fontDialog = new FontDialog () ;
				fontDialog.ShowApply = true;
				fontDialog.ShowColor = true;
				fontDialog.Font = control.Font ;
				fontDialog.Color = control.ForeColor;
				//fontDialog.Apply += new System.EventHandler ( fontDialog_Apply ) ;
				if ( fontDialog.ShowDialog() != DialogResult.Cancel )
				{
					control.ForeColor = fontDialog.Color;
					control.Font = fontDialog.Font ;
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		SetListBoxFontMenuItem_Click () 
		{
			try
			{
				SetFont ( ConcorDancer.CurrentListBox  ) ;
				ConcorDancer.ListBoxFont = ConcorDancer.CurrentListBox.Font ;
				ConcorDancer.ListBoxForeColor = ConcorDancer.CurrentListBox .ForeColor ;
                ConcorDancer.Init();
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.Init();
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.ListBox.BuildAndDisplayListBoxItemStringArray();
                ConcorDancer.Cdwf.AdjustDisplay();
            }
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		SetTextBoxFontMenuItem_Click ()
		{
			try
			{
                bool saved = ConcorDancer.Cdm.State.TextBoxTextChangedGuard;
				ConcorDancer.Cdm.State.TextBoxTextChangedGuard = true ; // setting Font is not 'Modifying'
				SetFont ( ConcorDancer.CurrentTextBox  ) ;
				ConcorDancer.TextBoxFont = ConcorDancer.CurrentTextBox.Font ;
				ConcorDancer.TextBoxForeColor = ConcorDancer.CurrentTextBox.ForeColor ;
				ConcorDancer.TextBoxPixelHeightPerCharacter = 
					(double) ConcorDancer.TextBoxFont.Height ;
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.TextBox.Init();
                ConcorDancer.Cdwf.AdjustDisplay();
                ConcorDancer.Cdm.State.TextBoxTextChangedGuard = saved; // setting Font is not 'Modifying'
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		SetDirectoryTreeFontMenuItem_Click ()
		{
			try
			{
				SetFont ( ConcorDancer.CurrentTreeView  ) ;
				ConcorDancer.TreeViewFont = ConcorDancer.CurrentTreeView.Font ;
				ConcorDancer.TreeViewForeColor = ConcorDancer.CurrentTreeView .ForeColor ;
				ConcorDancer.DirectoryForeColor = ConcorDancer.TreeViewForeColor  ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		SetFileListFontMenuItem_Click ()
		{
			try
			{
				SetFont ( ConcorDancer.CurrentDataGridView  ) ;
				ConcorDancer.DataGridViewFileFont = ConcorDancer.CurrentDataGridView.Font ;
				ConcorDancer.FileForeColor = ConcorDancer.CurrentDataGridView .ForeColor ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		// Open File
		public void
		NewFileMenuItem_Click()
		{
			try
			{
				SetupANewConcorDancerTabPage ( "untitled.txt" ) ;
				((ConcorDancerTabPage)ConcorDancer.Cdm.CurrentConcorDancerTabPage).TextBox.ReadOnly = false ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

        // Open File
        public void
        OpenAFileToThisTextBoxMenuItem_Click()
        {
            try
            {
                bool saved = State.ListBox_SelectedIndexChangedGuard ;
                State.ListBox_SelectedIndexChangedGuard = true;
                OpenAFile();
                SetWindowTitle();
                State.ListBox_SelectedIndexChangedGuard = saved ;
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }

        // Menu.Options.Word Wrap
		public void
		WordWrapMenuItem_Click()
		{
			try
			{
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				if ( ConcorDancer.Cdwf.WordWrapMenuItem.Checked == true )
				{
					ConcorDancer.Cdwf.WordWrapMenuItem.Checked = false ;
				}
				else ConcorDancer.Cdwf.WordWrapMenuItem.Checked = true ;
				ctp.TextBox.WordWrap = ConcorDancer.Cdwf.WordWrapMenuItem.Checked ;
				ctp.TextBox.Refresh () ;
				ConcorDancer.Cdwf.ComboBoxForFind.Refresh () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}
		
		public void
		CloseFileMenuItem_Click()
		{
			try
			{
				TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
				if (  tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
				{
					ConcorDancer.Cdwf.TabControl.TabPages.Remove ( tp ) ;
					ConcorDancer.Cdwf.TabControl.NextSelectedIndex () ;
					return ;
				}
				else
				//foreach ( ConcorDancerTabPage ctp in ConcorDancerTabPageList )
				{
					//if ( ctp == ConcorDancer.Cdwf.tabControl1.SelectedTab )
					ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
					if ( ctp == null ) return ;
					else
					{
						ConcorDancer.Cdwf.TabControl.NextSelectedIndex () ; // -= 1 ;
						ctp.Delete () ;
						//break ;
					}
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

        public void
        OpenFiles()
        {
            try
            {
                ConcorDancer.Cdwf.OpenFileDialog.InitialDirectory = ConcorDancer.Cdm.CurrentDirectory;
                ConcorDancer.Cdwf.OpenFileDialog.Filter =
                //      1          |     2     |    3      |     4     |     5                 |     6      |    7          |     8
                "Text - ASCII|*.txt|Html|*.htm*|UTF-7|*.txt|UTF-8|*.txt|Unicode - UTF-16 |*.txt|UTF-32|*.txt|Rich Text|*.rtf|All files|*";
                ConcorDancer.Cdwf.OpenFileDialog.FilterIndex = OpenFileDialogFilterIndex;
                ConcorDancer.Cdwf.OpenFileDialog.RestoreDirectory = true;
                ConcorDancer.Cdwf.OpenFileDialog.Multiselect = true;
                if (ConcorDancer.Cdwf.OpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Refresh () ;
                    OpenFileDialogFilterIndex = ConcorDancer.Cdwf.OpenFileDialog.FilterIndex;
                    ReadFiles();
                }
            }
            catch (Exception e)
            {
                HandleException(e, true);
            }
        }

        public void
        OpenAFile()
        {
            try
            {
                ConcorDancer.Cdwf.OpenFileDialog.InitialDirectory = ConcorDancer.Cdm.CurrentDirectory;
                ConcorDancer.Cdwf.OpenFileDialog.Filter =
                //      1          |     2     |    3      |     4     |     5                 |     6      |    7          |     8
                "Text - ASCII|*.txt|Html|*.htm*|UTF-7|*.txt|UTF-8|*.txt|Unicode - UTF-16 |*.txt|UTF-32|*.txt|Rich Text|*.rtf|All files|*";
                ConcorDancer.Cdwf.OpenFileDialog.FilterIndex = OpenFileDialogFilterIndex;
                ConcorDancer.Cdwf.OpenFileDialog.RestoreDirectory = true;
                ConcorDancer.Cdwf.OpenFileDialog.Multiselect = true;
                if (ConcorDancer.Cdwf.OpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Refresh () ;
                    OpenFileDialogFilterIndex = ConcorDancer.Cdwf.OpenFileDialog.FilterIndex;
                    CurrentConcorDancerTabPage.ReadAFile(ConcorDancer.Cdwf.OpenFileDialog.FileNames[0]);
                }
            }
            catch (Exception e)
            {
                HandleException(e, true);
            }
        }

        // Find button function
		public void
		FindListButton_Click ()
		{
			try
			{
				DisplayConcordanceList () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public ConcorDancerTabPage
		SetupANewConcorDancerTabPage ( string pathName )
		{
			try
			{
				ConcorDancerTabPage ctp = new ConcorDancerTabPage ( pathName ) ;
				SetSortOptionMenuItemTextEtc ( SortFlag, WordsBack ) ;
				SetWindowTitle () ;
				//ConcorDancer.Cdwf.findHistoryComboBox.ClearTextBox () ; // 
				return ctp ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
				return (ConcorDancerTabPage) null ;
			}
		}

		public void
		TabControl1_DoubleClick (ConcorDancerTabControl tabControl )
		{
			try
			{
				TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
				if ( tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
				{
					tabControl.TabPages.Remove ( tp ) ;
					tabControl.NextSelectedIndex () ;
					ConcorDancer.Cdwf.newFileManagerMenuItem.Enabled = true ;
					return ;
				}
				else CloseFileMenuItem_Click () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		SetSortOptionMenuItemTextEtc ( bool psortOptionChecked, int pWordsBack )
		{
			try
			{
				string formatString =
					"Sort Find List - Starting {0} " + ( pWordsBack == 1 ? "Word" : "Words" ) + " Back" ;
				ConcorDancer.Cdwf.sortFunctionMenuItem.Text = String.Format (
					formatString, pWordsBack );
				//ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked = psortOptionChecked ;
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.ListBox.SortFlag = psortOptionChecked ;// local per page sort flag
				ctp.ListBox.WordsBack = pWordsBack ;
				WordsBack = pWordsBack ;
			}
			catch ( Exception e )
			{
				HandleException ( e, false ) ;
			}
		}


		public void
		SetComboBoxText ( string word )
		{
			ConcorDancer.Cdwf.ComboBoxForFind.Text = word ;
			ConcorDancer.Cdwf.ComboBoxForFind.Focus () ;
		}


		public void
		DisplayRequestedRegExpWordList ( )
		{
			#if threads
			RunAsNewThread ( 
				new ThreadStart ( ConcorDancer.Cdm.CurrentConcorDancerTabPage._DisplayRequestedRegExpWordList ) ) ;
			#else
			ConcorDancer.Cdm.CurrentConcorDancerTabPage._DisplayRequestedRegExpWordList () ;
			#endif
		}

		public void
		_DisplayConcordanceList ()
		{
			try
			{
				if ( ConcorDancer.Cdwf.ComboBoxForFind.Text.Length == 0 ) return ;
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ctp.ListBox.RequestedWord = ConcorDancer.Cdwf.ComboBoxForFind.Text ;
				DisplayRequestedRegExpWordList () ;
				ConcorDancer.Cdwf.ComboBoxForFind.MoveCurrentItemToTopOfItemsList () ;
				ConcorDancer.Cdwf.ComboBoxForFind.ClearTextBox () ;
				//ConcorDancer.Cdwf.comboBox1.Focus () ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
		}

		public void
		DisplayConcordanceList ()
		{
			#if threads
			RunAsNewThread ( new ThreadStart ( _DisplayConcordanceList ) ) ;
			#else
			_DisplayConcordanceList () ;
			#endif
		}
	
		public void
		Refresh ()
		{
            try
            {
                ConcorDancer.Cdwf.AdjustDisplay();
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                if (ctp != null )
                {
                    ctp.ListBox.BuildAndDisplayListBoxItemStringArray();
                    ConcorDancer.Cdwf.ComboBoxForHistory.AdjustDisplay();
                }
                else
                {
                    if (ConcorDancer.Cdm.State.CurrentFileManagerTabPageAdjustDisplayGuard == false)
                    {
                        ConcorDancer.Cdm.CurrentFileManagerTabPage.AdjustDisplay();
                    }
                }
                ConcorDancer.Cdwf.Refresh();
            }
            catch
            {
                return;
            }
		}
		
		public void
		WinForm_KeyUp ( KeyEventArgs e )
		{
			try
			{
				switch ( e.KeyCode )
				{
                        /*
                    case Keys.F2:
                        {
                            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                            ctp.FirstBookmark();
                            break;
                        }
                         */
                    case Keys.F2:
                        {
                            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                            ctp.MakeBookmark();
                            break;
                        }
                    case Keys.F3:
                        {
                            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                            ctp.NextBookmark();
                            break;
                        }
                    case Keys.F4:
                        {
                            ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                            ctp.PreviousBookmark();
                            break;
                        }
                    case Keys.F9:
					{
						HorizontalTextBoxSplitMenuItem_Click () ;
						break ;
					}
					case Keys.F10 :
					{
						VerticalTextBoxSplitMenuItem_Click () ;
						break ;
					}
					case Keys.F11 :
					{
						UnsplitTextBoxMenuItem_Click () ;
						break ;
					}
					case Keys.F12 :
					{
						ToggleListBoxFunctionMenuItem_Click () ;
						break ;
					}
					case Keys.F5 :
					{
						NewFileManagerTab () ;
						break ;
					}
				}
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
        }

        #if threads
                public void
                RunAsNewThread ( ThreadStart ts )
                {
                    Thread t = new Thread ( ts );
                    t.Start();
                }
        #endif
        
        #if timers
        #if layoutTimer
                public void
                ConcorDancerWinForm_Layout ()
                {
                    if ( InLayoutEvent == false )
                    {
                        LayoutTimer.Start () ;
                        InLayoutEvent = true ;
                    }
                    else
                    {
                        LayoutTimer.Stop () ;
                        LayoutTimer.Start () ;
                    }
                }

                void
                LayoutTimer_Tick ( object sender, EventArgs e )
                {
                    LayoutTimer.Stop ();
                    if ( InResizeEvent == false ) //ConcorDancer.Cdwf.SetBoundariesAndLocationsForBoxes () ;
                        ConcorDancer.Cdwf.AdjustDisplay ();
                    InLayoutEvent = false;
                }
        #endif // layoutTimer

        void
        ResizeTimer_Tick ( object sender, EventArgs e )
        {
            ResizeTimer.Stop ();
            ConcorDancer.Cdwf.AdjustDisplay () ;
            InResizeEvent = false ;
        }

        public
        ConcorDancerModel ()
        {
#if layoutTimer
            LayoutTimer.Tick += new EventHandler ( LayoutTimer_Tick ) ;
            LayoutTimer.Interval = ConcorDancer.MilliSecondDelayForLayout ;
#endif
            ResizeTimer.Tick += new EventHandler ( ResizeTimer_Tick ) ;
            ResizeTimer.Interval = ConcorDancer.MilliSecondDelayForResize ;
        }
        #endif // timers
        public void
        ConcorDancerWinForm_Resize()
        {
#if timers
            if (InResizeEvent == false)
            {
                InResizeEvent = true;
                ResizeTimer.Start();
            }
            else
            {
                ResizeTimer.Stop();
                ResizeTimer.Start();
            }
#endif
            Refresh();
        }
    }
}
