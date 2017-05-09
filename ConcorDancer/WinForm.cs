//#define debug1
//#define threads2
#define timers

using System ;
using System.Collections ;
using System.IO ;
using System.Drawing ;
using System.ComponentModel ;
using System.Data;
using System.Threading ;
using System.Text ;
using System.Globalization ;
using System.Windows.Forms ;
using System.Text.RegularExpressions ;
using System.Runtime.Serialization ;
using System.Runtime.Serialization.Formatters.Binary ;

namespace ConcorDancer
{
	public partial class
	ConcorDancerWinForm : Form
	{
        public MenuItem
		FindAppendTextMenuItem ( Menu menu )
		{
            foreach ( MenuItem mi in menu.MenuItems )
            {
                if ( mi.Text == "&Append Selected Text To File" ) return mi ;
            }  
            return null ;
		}

		public void
		AddContextMenuItems ( Control  control )
		{
			control.ContextMenu = new ContextMenu () ;
			control.ContextMenu.MenuItems.Add ( contextMenuItem.CloneMenu ( ) ) ;
		}

		public void
		AddContextMenuItems ( Menu  menu )
		{
			MenuItem FindSelectedTextMenuItem = new MenuItem();
			MenuItem BackMenuItem = new MenuItem();
			MenuItem ForwardMenuItem = new MenuItem();
            MenuItem MakeBookmarkMenuItem = new MenuItem();
            MenuItem NextBookmarkMenuItem = new MenuItem();
            MenuItem PreviousBookmarkMenuItem = new MenuItem();
            MenuItem AppendTextMenuItem = new MenuItem();
			MenuItem AppendToADifferentAppendFileMenuItem = new MenuItem();
			MenuItem DeleteCurrentFindListMenuItem  = new MenuItem();
			MenuItem ToggleListBoxFunctionMenuItem = new MenuItem();
			MenuItem SplitTextBoxMenuItem = new MenuItem();
			MenuItem HorizontalTextBoxSplitMenuItem = new MenuItem();
			MenuItem VerticalTextBoxSplitMenuItem = new MenuItem();
			MenuItem UnsplitTextBoxMenuItem = new MenuItem();
			MenuItem OpenAFileMenuItem = new MenuItem();

			menu.MenuItems.AddRange ( new MenuItem[] {
						FindSelectedTextMenuItem,
						AppendTextMenuItem,
						AppendToADifferentAppendFileMenuItem,
						BackMenuItem,
						ForwardMenuItem,
						//deleteCurrentFindListElementMenuItem,
						DeleteCurrentFindListMenuItem,
						MakeBookmarkMenuItem,
						NextBookmarkMenuItem,
						PreviousBookmarkMenuItem,
						ToggleListBoxFunctionMenuItem,
						UnsplitTextBoxMenuItem,
						SplitTextBoxMenuItem,
						OpenAFileMenuItem,
						SaveMenuItem.CloneMenu (),
						SaveAsMenuItem.CloneMenu (),
						});
			//
			// findSelectedTextMenuItem
			//
			FindSelectedTextMenuItem.Index = 0;
			FindSelectedTextMenuItem.Text = "Find &Selected Text";
			FindSelectedTextMenuItem.Click += new System.EventHandler ( FindSelectedTextMenuItem_Click ) ;
			//
			// backMenuItem
			//
			BackMenuItem.Index = 1;
			BackMenuItem.Text = "Bac&k";
			BackMenuItem.Click += new System.EventHandler ( BackMenuItem_Click ) ;
			//
			// forwardMenuItem
			//
			ForwardMenuItem.Index = 2 ;
			ForwardMenuItem.Text =              "Fo&rward" ;
			ForwardMenuItem.Click += new System.EventHandler ( ForwardMenuItem_Click ) ;
            //
            // makeBookmarkMenuItem
            //
            MakeBookmarkMenuItem.Index = 3;
            MakeBookmarkMenuItem.Text =                 "Make &Bookmark Here                  F2";
            MakeBookmarkMenuItem.Click += new System.EventHandler(MakeBookmarkMenuItem_Click);
            /*
            //
            // firstBookmarkMenuItem
            //
            FirstBookmarkMenuItem.Index = 4; 
            FirstBookmarkMenuItem.Text =                "Goto &First Bookmark                    F2";
            FirstBookmarkMenuItem.Click += new System.EventHandler(FirstBookmarkMenuItem_Click);
            */
            //
            // nextBookmarkMenuItem
            //
            NextBookmarkMenuItem.Index = 4;
            NextBookmarkMenuItem.Text =                 "Goto &Next Bookmark                    F3";
            NextBookmarkMenuItem.Click += new System.EventHandler(NextBookmarkMenuItem_Click);
            //
            // previousBookmarkMenuItem
            //
            PreviousBookmarkMenuItem.Index = 5;
            PreviousBookmarkMenuItem.Text =             "Goto &Previous Bookmark              F4";
            PreviousBookmarkMenuItem.Click += new System.EventHandler(PreviousBookmarkMenuItem_Click);
            //
			// appendTextMenuItem
			//
			AppendTextMenuItem.Index = 6;
			AppendTextMenuItem.Text =           "&Append Selected Text To File";
			AppendTextMenuItem.Click += new System.EventHandler ( AppendTextMenuItem_Click ) ;
			//
			// appendToADifferentAppendFileMenuItem
			//
			AppendToADifferentAppendFileMenuItem.Index = 7;
			AppendToADifferentAppendFileMenuItem.Text = "Append To A &Different File";
			AppendToADifferentAppendFileMenuItem.Click += new System.EventHandler (
				AppendToADifferentAppendFileMenuItem_Click ) ;
			/*
			//
			// deleteCurrentFindListElementMenuItem
			//
			deleteCurrentFindListElementMenuItem.Index = 5 ;
			deleteCurrentFindListElementMenuItem.Text = "Delete Current Find &List Element";
			deleteCurrentFindListElementMenuItem.Click += new System.EventHandler (
				deleteCurrentFindListElementMenuItem_Click ) ;
			//
			*/
			// deleteCurrentFindListMenuItem
			//
			DeleteCurrentFindListMenuItem.Index = 8 ;
			DeleteCurrentFindListMenuItem.Text =        "&Clear Current Find List";
			DeleteCurrentFindListMenuItem.Click += new System.EventHandler (
				DeleteCurrentFindListMenuItem_Click ) ;

			SplitTextBoxMenuItem.Index = 9 ;
			SplitTextBoxMenuItem.Text =                 "Sp&lit Text Box";
			SplitTextBoxMenuItem.MenuItems.AddRange ( new MenuItem[] {
                                HorizontalTextBoxSplitMenuItem,
                                VerticalTextBoxSplitMenuItem
				}) ;

			HorizontalTextBoxSplitMenuItem.Index = 0 ;
			HorizontalTextBoxSplitMenuItem.Text =       "&Horizontal         F9" ;
			HorizontalTextBoxSplitMenuItem.Click +=
				new System.EventHandler ( HorizontalTextBoxSplitMenuItem_Click ) ;

			VerticalTextBoxSplitMenuItem.Index = 1 ;
			VerticalTextBoxSplitMenuItem.Text =         "&Vertical             F10" ;
			//verticalTextBoxSplitMenuItem.Checked = true  ;
			VerticalTextBoxSplitMenuItem.Click += new System.EventHandler ( VerticalTextBoxSplitMenuItem_Click ) ;

			UnsplitTextBoxMenuItem.Index = 10 ;
			UnsplitTextBoxMenuItem.Text =
				                                        "&Unsplit Text                               F11" ;
			UnsplitTextBoxMenuItem.Click +=
				new System.EventHandler ( UnsplitTextBoxMenuItem_Click ) ;
			//
			// toggleListBoxFunctionMenuItem
			//
			ToggleListBoxFunctionMenuItem.Index = 11;
			ToggleListBoxFunctionMenuItem.Text =
				                                        "&Toggle List                                F12" ;
			//appendTextMenuItem.Text =                        "&Append Selected Text To File" ;
			ToggleListBoxFunctionMenuItem.Click += new System.EventHandler ( ToggleListBoxFunctionMenuItem_Click ) ;
			//
			// openFilesMenuItem
			//
			OpenAFileMenuItem.Index = 12;
			OpenAFileMenuItem.Text = "&Open A File To This TextBox";
			OpenAFileMenuItem.Click += new System.EventHandler ( OpenAFileToThisTextBoxMenuItem_Click ) ;
		}

        void
        OpenAFileToThisTextBoxMenuItem_Click(object sender, EventArgs e)
        {
            ConcorDancer.Cdm.OpenAFileToThisTextBoxMenuItem_Click();
        }

#if timers
		/*
		void
		ConcorDancerWinForm_Layout ( object sender, LayoutEventArgs e )
		{
			ConcorDancer.Cdm.ConcorDancerWinForm_Layout () ;
		}
		*/
#endif

        void
        ConcorDancerWinForm_Resize(object sender, EventArgs e)
        {
            ConcorDancer.Cdm.ConcorDancerWinForm_Resize();
        }

        void
		NewFileManagerMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.NewFileManagerMenuItem_Click () ;
		}

		void
		UnsplitTextBoxMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.UnsplitTextBoxMenuItem_Click () ;
		}

		void
		VerticalTextBoxSplitMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.VerticalTextBoxSplitMenuItem_Click ( ) ;
		}

		void
		HorizontalTextBoxSplitMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.HorizontalTextBoxSplitMenuItem_Click ( ) ;
		}

		void
		ToggleListBoxFunctionMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.ToggleListBoxFunctionMenuItem_Click () ;
		}

/*
		void
		splitTextBoxMenuItem_Click  ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.splitTextBoxMenuItem_Click () ;
		}
*/
		void
		BackButton_Click ( object sender, System.EventArgs e)
		{
			ConcorDancer.Cdm.BackButton_Click () ;
		}

		void
		ForwardButton_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.ForwardButton_Click ();
		}

		void
		SortFunctionMenuItem_Click ( object sender, System.EventArgs e )
		// one shot sort ; maintain option state
		{
			ConcorDancer.Cdm.SortFunctionMenuItem_Click ();
		}

		void
		SortOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SortOptionMenuItem_Click ();
		}

		void
		Start0WordsBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start0WordsBackMenuItem_Click ();
		}

		void
		Start1WordBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start1WordBackMenuItem_Click ();
		}

		void
		Start2WordsBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start2WordsBackMenuItem_Click ();
		}

		void
		Start3WordsBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start3WordsBackMenuItem_Click ();
		}

		void
		Start4WordsBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start4WordsBackMenuItem_Click ();
		}

		void
		Start5WordsBackMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.Start5WordsBackMenuItem_Click ();
		}

		void
		CaseSensitiveFindsMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.CaseSensitiveFindsMenuItem_Click ();
		}

		void
		RestoreStateMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RestoreStateMenuItem_Click ();
		}

		void
		ShowVersionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.ShowVersionMenuItem_Click ();
		}

		void
		ShowListBoxHorizontalScrollBarMenuItem_Click ( object sender, System.EventArgs e )
		{
           ConcorDancer.Cdm.ShowListBoxHorizontalScrollBarMenuItem_Click();
		}

		// Menu.Options.Read Only
		void
		ReadOnlyMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.ReadOnlyMenuItem_Click ();
		}

		// set opton to Find words anywhere within a sentence
		void
		FindWordsAnyOrderMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.FindWordsAnyOrderMenuItem_Click ();
		}

		// set opton to Find words anywhere within a sentence
		void
		FindWordsAnyPlaceMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.FindWordsAnyPlaceMenuItem_Click ();
		}

		void
		AppendTextMenuItem_Click(object sender, System.EventArgs e )
		{
			AppendTextMenuItem_Click();
		}

		void
		DeleteCurrentFindListMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.DeleteCurrentFindListMenuItem_Click ();
		}

        public void
        _SaveAppendFile()
        {
            ConcorDancer.Cdm.CurrentConcorDancerTabPage._SaveAppendFile(ConcorDancer.Cdm.AppendFileName);
        }

        public void
        SaveAppendFile()
        {
            if (ConcorDancer.Cdm.AppendFileName == "append.txt")
            {
                ConcorDancer.Cdwf.SaveAsAppendFile();
            }
            else
            {
                _SaveAppendFile();
            }
        }

        public void
        AppendTextMenuItem_Click()
        {
            try
            {
                SaveAppendFile();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        SaveAppendfileMenuFileItem_Click()
        {
            try
            {
                SaveAppendFile();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        void
		SaveAsAppendFileMenuFileItem_Click(object sender, System.EventArgs e)
		{
			SaveAsAppendFileMenuFileItem_Click () ;
		}

		void
		AppendToADifferentAppendFileMenuItem_Click(object sender, System.EventArgs e )
		{
			AppendToADifferentAppendFileMenuItem_Click();
		}

		void
		SaveAppendfileMenuFileItem_Click(object sender, System.EventArgs e )
		{
			SaveAppendfileMenuFileItem_Click();
		}

        // respond to append file menu selection item
		public void
		AppendFileMenuFileItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.AppendFileMenuFileItem_Click ( sender );
		}

		void
		RefreshDisplayMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RefreshDisplayMenuItem_Click ();
		}

		void
		SetListBoxBackgroundColorMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetListBoxBackgroundColorMenuItem_Click ();
		}

		void
		SetFileListBackgroundColorMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetFileListBackgroundColorMenuItem_Click ();
		}

		void
		SetDirectoryTreeBackgroundColorMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetDirectoryTreeBackgroundColorMenuItem_Click ();
		}

		void
		SetTextBoxBackgroundColorMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetTextBoxBackgroundColorMenuItem_Click ();
		}

		void
		RefreshListMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RefreshListMenuItem_Click ();
		}

		void
		ShiftFocusToTextMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.ShiftFocusToTextMenuItem_Click();
		}

		void
		KeepListFocusMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.KeepListFocusMenuItem_Click();
		}

		void
		UseRegularExpressionsMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.UseRegularExpressionsMenuItem_Click();
		}

		void
		RemoveHtmlTagsOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RemoveHtmlTagsOptionMenuItem_Click ();
		}

		void
		RemoveHtmlAnchorsOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RemoveHtmlAnchorsOptionMenuItem_Click ();
		}

		void
		InsertFileNamesOnJoinOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.InsertFileNamesOnJoinOptionMenuItem_Click ();
		}

		void
		SetListBoxViewCenteredOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetListBoxViewCenteredOptionMenuItem_Click ();
		}

		void
		RecurseFoldersOptionMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.RecurseFoldersOptionMenuItem_Click ();
		}

		public void
		MultiColumnFileListMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.MultiColumnFileListMenuItem_Click ();
		}

		void
		FindSelectedTextMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.FindSelectedTextMenuItem_Click();
		}

		void
		BackMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.BackMenuItem_Click();
		}

		void
		ForwardMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.ForwardMenuItem_Click();
		}

        void
        MakeBookmarkMenuItem_Click(object sender, System.EventArgs e)
        {
            ConcorDancer.Cdm.MakeBookmarkMenuItem_Click();
        }
/*
        void
        FirstBookmarkMenuItem_Click(object sender, System.EventArgs e)
        {
            ConcorDancer.Cdm.FirstBookmarkMenuItem_Click();
        }
*/
        void
        PreviousBookmarkMenuItem_Click(object sender, System.EventArgs e)
        {
            ConcorDancer.Cdm.PreviousBookmarkMenuItem_Click();
        }

        void
        NextBookmarkMenuItem_Click(object sender, System.EventArgs e)
        {
            ConcorDancer.Cdm.NextBookmarkMenuItem_Click();
        }

        void
		SetListBoxFontMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetListBoxFontMenuItem_Click ();
		}

		void
		SetDirectoryTreeFontMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetDirectoryTreeFontMenuItem_Click () ;
		}

		void
		SetFileListFontMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetFileListFontMenuItem_Click () ;
		}

		void
		SetTextBoxFontMenuItem_Click (object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.SetTextBoxFontMenuItem_Click () ;
		}

		void
		NewFileMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.NewFileMenuItem_Click();
		}

		// Menu.Options.Word Wrap
		void
		WordWrapMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.WordWrapMenuItem_Click();
		}

		void
		CloseFileMenuItem_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.CloseFileMenuItem_Click();
		}

		// Find button function
		void
		FindListButton_Click(object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.FindListButton_Click();
		}

		// Open Folder
		void
		OpenFolderMenuItem_Click ( object sender, System.EventArgs e )
		{
			ConcorDancer.Cdm.OpenFolderMenuItem_Click ();
		}

		public void
		WinForm_KeyUp ( object sender, KeyEventArgs e )
		{
			ConcorDancer.Cdm.WinForm_KeyUp ( e ) ;
		}

        public void
		SetCounterLabel ()
		{
			try
			{
				int count ;
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				if ( ( ctp != null ) && ( ctp.ListBox.MatchCollection != null ) )
				{
					count = ctp.ListBox.MatchCollection.Count ;
				}
				else
				{
					count = 0 ;
				}
				//counterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
				CounterLabel.Text = count.ToString () ;
				//SetLocationsAndHeightsForBoxes () ;
                AdjustDisplay();
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

        void
        OpenAppendfileMenuFileItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                ConcorDancer.Cdwf.OpenFileDialog.InitialDirectory = @".";
                ConcorDancer.Cdwf.OpenFileDialog.Filter = "*.txt|*.txt";
                ConcorDancer.Cdwf.OpenFileDialog.FilterIndex = 1;
                ConcorDancer.Cdwf.OpenFileDialog.RestoreDirectory = true;
                ConcorDancer.Cdwf.OpenFileDialog.Multiselect = false;
                ConcorDancer.Cdwf.OpenFileDialog.FileName = ConcorDancer.Cdm.AppendFileName;
                ConcorDancer.Cdwf.OpenFileDialog.CheckFileExists = false;
                if (ConcorDancer.Cdwf.OpenFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ConcorDancer.Cdm.AppendFileName = ConcorDancer.Cdwf.OpenFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        // Open File
        void
        OpenFilesMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                ConcorDancer.Cdm.OpenFiles();
                ConcorDancer.Cdm.SetWindowTitle();

            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

		// Open File
		void
		OpenAFileMenuItem_Click(object sender, System.EventArgs e )
        {
            try
            {
                ConcorDancer.Cdm.OpenAFile();
                ConcorDancer.Cdm.SetWindowTitle();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        // Menu.File.Save
        void
        SaveAsMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                SaveFileDialog.InitialDirectory = @".";
                SaveFileDialog.Filter =
                //      1          |     2     |    3      |     4     |     5                 |     6      |    7          |     8
                "Text - ASCII|*.txt|Html|*.htm*|UTF-7|*.txt|UTF-8|*.txt|Unicode - UTF-16 |*.txt|UTF-32|*.txt|Rich Text|*.rtf|All files|*";
                SaveFileDialog.FilterIndex = 1;
                SaveFileDialog.RestoreDirectory = true;
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                if (ctp.CurrentTextBox.FullPathFilename != null)
                {
                    SaveFileDialog.FileName = ConcorDancer.MakeFilenameFromFullPathName(ctp.CurrentTextBox.FullPathFilename);
                }
                if (SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ctp.TextBox.FilterIndexFileType = SaveFileDialog.FilterIndex;
                    ctp.SaveTextBoxToFile(SaveFileDialog.FileName);
                    //if ( ctp != null )
                    // in case of saving a folder join 'no name' file
                    {
                        ctp.CurrentTextBox.FullPathFilename = SaveFileDialog.FileName;
                        string filename = ctp.MakeFilenameFromFullPathFilename(
                            ctp.CurrentTextBox.FullPathFilename);
                        //ctp.MenuItem.Text =  filename ;
                        ctp.Text = filename;
                    }
                    ConcorDancer.Cdm.SetWindowTitle();
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        void
        SaveMenuItem_Click(object sender, System.EventArgs e)
        {
            try
            {
                SaveFileDialog.FileName = ConcorDancer.Cdm.CurrentConcorDancerTabPage.CurrentTextBox.FullPathFilename;
                if (ConcorDancer.Cdm.CurrentConcorDancerTabPage.CurrentTextBox.FullPathFilename == null) SaveAsMenuItem_Click(sender, e);
                else
                {
                    ConcorDancer.Cdm.CurrentConcorDancerTabPage.SaveTextBoxToFile(
                        ConcorDancer.Cdm.CurrentConcorDancerTabPage.CurrentTextBox.FullPathFilename);
                    ConcorDancer.Cdm.SetWindowTitle();
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        SaveAsAppendFile()
        {
            try
            {
                SaveFileDialog.InitialDirectory = @".";
                //SaveFileDialog.Filter = "*.txt|*.txt|*.html|*.htm;*.html|All files|*.*";
                SaveFileDialog.Filter =
                //      1          |     2     |    3      |     4     |     5                 |     6      |    7          |     8
                "Text - ASCII|*.txt|Html|*.htm*|UTF-7|*.txt|UTF-8|*.txt|Unicode - UTF-16 |*.txt|UTF-32|*.txt|Rich Text|*.rtf|All files|*";
                SaveFileDialog.FilterIndex = 1;
                SaveFileDialog.RestoreDirectory = true;
                SaveFileDialog.FileName = ConcorDancer.Cdm.AppendFileName;
                if (SaveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    ConcorDancer.Cdm.AppendFileName = SaveFileDialog.FileName;
                    MenuAppendFileElement mafe =
                        new MenuAppendFileElement(ConcorDancer.Cdm.AppendFileName);
                    ConcorDancer.Cdm.CurrentConcorDancerTabPage._SaveAppendFile(mafe.MafeFileName);
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        AppendToADifferentAppendFileMenuItem_Click()
        {
            try
            {
                SaveAsAppendFile();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        SaveAsAppendFileMenuFileItem_Click()
        {
            SaveAsAppendFile();
        }
/*
        public void
        ComboBoxForHistory_MouseEnter(Object sender, EventArgs e)
        {
            //if (!ComboBoxForHistory.Focused) ComboBoxForHistory.Focus();
        }

        public void
        ComboBoxForFind_MouseEnter(Object sender, EventArgs e)
        {
            if (!ComboBoxForFind.Focused) ComboBoxForFind.Focus();
        }
*/
        protected override void Dispose(bool disposing)
		{
			if ( disposing && ( components != null ) )
			{
				components.Dispose ();
			}
			base.Dispose ( disposing );
		}

        public void
		UncheckStartWordsBackMenuItems ()
		{
			Start0WordsBackMenuItem.Checked = false ;
			Start1WordBackMenuItem.Checked = false ;
			Start2WordsBackMenuItem.Checked = false ;
			Start3WordsBackMenuItem.Checked = false ;
			Start4WordsBackMenuItem.Checked = false ;
			Start5WordsBackMenuItem.Checked = false ;
		}

        public void
        InitializeTopRowBoxes ()
        {
			//
			// forwardButton
			//
			ForwardButton.Width = ConcorDancer.ButtonHeight ;
			ForwardButton.Height = ConcorDancer.ButtonHeight ;
			ForwardButton.Location = new System.Drawing.Point( TabControl.Right -
				ForwardButton.Width, ConcorDancer.YPositionOfComboBox );
			//forwardButton.Name = "forwardButton";
        	//ForwardButton.Anchor = AnchorStyles.Right | AnchorStyles.Top ;
            //forwardButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft ;
			//forwardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter  = default
			//
			// backButton
			//
			BackButton.Width = ConcorDancer.ButtonHeight ;
			BackButton.Height = ConcorDancer.ButtonHeight ;
			BackButton.Location = new System.Drawing.Point( ForwardButton.Left -
				ConcorDancer.SpaceBetweenForwardButtonAndBackButton - BackButton.Width,
				ConcorDancer.YPositionOfComboBox);
			//backButton.Name = "backButton";
			//BackButton.Anchor = AnchorStyles.Right | AnchorStyles.Top ;
           //backButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft ;
			//backButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft ;
			//
			// findHistoryComboBox
			//
			ComboBoxForHistory.Width = 300 ; // ConcorDancer.ListBoxStartWidth / 2;
            //ComboBoxForHistory.Font = new System.Drawing.Font("Courier New", 9F);
            ComboBoxForHistory.Font = ConcorDancer.ListBoxFont;
            ComboBoxForHistory.Location = new System.Drawing.Point(BackButton.Left -
				ComboBoxForHistory.Width - ConcorDancer.SplitterWidth,
				ConcorDancer.YPositionOfComboBox);
			ComboBoxForHistory.MaxLength = 128 ;
			//findHistoryComboBox.Name = "findHistoryComboBox";
			ComboBoxForHistory.Height = ConcorDancer.ComboBoxHeight ;
            ComboBoxForHistory.Init();// need to calculate PixelWidthPerCharacter 

			//
			// findListButton
			//
            System.Drawing.Size size = TextRenderer.MeasureText(FindButton.Text, FindButton.Font);
            FindButton.Width = size.Width + ( 2 * ConcorDancer.SpaceBetweenForwardButtonAndBackButton );
            FindButton.Location = new System.Drawing.Point(
	            ComboBoxForHistory.Left - ConcorDancer.SplitterWidth -
	            FindButton.Width, ConcorDancer.YPositionOfComboBox);
			//findListButton.Name = "findListButton";
			FindButton.Height = ConcorDancer.ButtonHeight ;
    		FindButton.Anchor = AnchorStyles.Left | AnchorStyles.Top ;
            //FindButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            FindButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            //
			// counterLabel
			//
			CounterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
			//counterLabel.Location = new System.Drawing.Point(432, ConcorDancer.YPositionOfComboBox);
			//counterLabel.Name = "counterLabel";
			//CounterLabel.Text = "Counter";
            size = TextRenderer.MeasureText(CounterLabel.Text, CounterLabel.Font);
            CounterLabel.Width = size.Width + ( 2 * ConcorDancer.SpaceBetweenForwardButtonAndBackButton );
            CounterLabel.Height = ConcorDancer.ButtonHeight;
			//counterLabel.TabIndex = 6;
			CounterLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			CounterLabel.Anchor = AnchorStyles.Left | AnchorStyles.Top ;
			//
			// comboBox1
			//
			//ComboBoxForFind.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            ComboBoxForFind.Font = ConcorDancer.ListBoxFont;
            ComboBoxForFind.Location = new System.Drawing.Point(1, //ConcorDancer.SpaceBetweenFormAndTabControl, // +
				// ConcorDancer.LeftAndTopSpaceBetweenTabControlAndTabPage,
				ConcorDancer.YPositionOfComboBox);
			ComboBoxForFind.MaxLength = 128 ;
			//comboBox1.Name = "comboBox1";
			//comboBox1.Width = 300 ; // ConcorDancer.ListBoxStartWidth / 2 ;
			ComboBoxForFind.Width = ConcorDancer.ListBoxStartWidth - ( CounterLabel.Width +
					FindButton.Width + 2 * ConcorDancer.SpaceBetweenListBoxAndTextBox ) ;
            ComboBoxForFind.Height = ConcorDancer.ComboBoxHeight;
			ComboBoxForFind.Anchor = AnchorStyles.Left | AnchorStyles.Top ;

			CounterLabel.Location = new System.Drawing.Point ( ComboBoxForFind.Right +
				( ( FindButton.Left - ComboBoxForFind.Right ) / 2 ) - ( CounterLabel.Width / 2 ), ConcorDancer.YPositionOfComboBox ) ;
            CounterLabel.TextAlign =
                System.Drawing.ContentAlignment.MiddleCenter;
            CounterLabel.Height = ConcorDancer.ButtonHeight;
        }

        public void
        CalculateLocationsAndHeightsForBoxes()
        {
            try
            {
                /*
                TabControl.Height =
                    ClientSize.Height
                    - ConcorDancer.YPositionOfBottomOfComboBox
                    - ConcorDancer.SpaceBetweenComboBoxAndTabControl
                    - ConcorDancer.SpaceBetweenFormAndTabControl;
                TabControl.Width =
                    ClientSize.Width
                    - (2 * ConcorDancer.SpaceBetweenFormAndTabControl);
                 */
                System.Drawing.Size size = TextRenderer.MeasureText(CounterLabel.Text, CounterLabel.Font);
                CounterLabel.Width = size.Width + (2 * ConcorDancer.SpaceBetweenForwardButtonAndBackButton);
                ComboBoxForFind.Width = (int)
                    //ctp.splitContainer1.Panel1.Width -
                    (TabControl.Width / 2) -
                    (CounterLabel.Width +
                    //findListButton.Width +
                    2 * ConcorDancer.SpaceBetweenForwardButtonAndBackButton);
                CounterLabel.Location = new System.Drawing.Point(
                    ComboBoxForFind.Right + ConcorDancer.SpaceBetweenForwardButtonAndBackButton,
                    ConcorDancer.YPositionOfComboBox);
                FindButton.Location = new System.Drawing.Point(
                        CounterLabel.Right +
                        ConcorDancer.SpaceBetweenForwardButtonAndBackButton,
                        ConcorDancer.YPositionOfComboBox);
                ComboBoxForHistory.Location =
                    new System.Drawing.Point(FindButton.Right +
                    ConcorDancer.SpaceBetweenForwardButtonAndBackButton,
                    ConcorDancer.YPositionOfComboBox);
                ComboBoxForHistory.Width =
                    (TabControl.Width / 2) -
                    FindButton.Width -
                    ConcorDancer.SpaceBetweenForwardButtonAndBackButton -
                    2 * (BackButton.Width +
                    ConcorDancer.SpaceBetweenForwardButtonAndBackButton);
                //+ ( 2 * ConcorDancer.SpaceBetweenTabPageAndSplitContainer ) ;
                CounterLabel.TextAlign =
                    System.Drawing.ContentAlignment.MiddleCenter;
                if (BackButton.Image.Height <= (BackButton.Height - 8))
                {
                    BackButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else
                {
                    BackButton.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
                }
                if (ForwardButton.Image.Height < (BackButton.Height - 8))
                {
                    ForwardButton.ImageAlign = System.Drawing.ContentAlignment.MiddleCenter;
                }
                else
                {
                    ForwardButton.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
                }
                TabControl.Location = new System.Drawing.Point(0, ComboBoxForFind.Bottom + 2);//ConcorDancer.SpaceBetweenForwardButtonAndBackButton);
                ClientSize = new System.Drawing.Size(TabControl.Size.Width, TabControl.Size.Height + FindButton.Size.Height + 4 );
               // refresh listbox
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                //if (ctp == null) return;
                //ctp.ListBox.SelectedIndex = ctp.ListBox.SelectedIndex; // cause a SelectedIndexChanged event
            }
            catch (Exception e)
            {
                ConcorDancer.Cdm.HandleException(e, false); // this exception is called on startup with no
            }
        }

        public void
        SetLocationsAndHeightsForBoxes()
        {
            try
            {
                if (WindowState == FormWindowState.Minimized) return;
                TabControl.SuspendLayout();
                SuspendLayout();
                CalculateLocationsAndHeightsForBoxes();
                TabControl.ResumeLayout(false);
                ResumeLayout(false);
                PerformLayout();
                //if (resumeFlag ) ResumeLayout () ;
            }
            catch (Exception e)
            {
                ConcorDancer.Cdm.HandleException(e, false); // this exception is called on startup with no
            }
        }
/*
        public void
        SetBoundariesAndLocationsForBoxes()
        {
            try
            {
                SuspendLayout();
                //setProportionalWidthsForBoxes () ;
                SetLocationsAndHeightsForBoxes();
                //if ( resumeFlag )
                ResumeLayout();
            }
            catch (Exception e)
            {
                ConcorDancer.Cdm.HandleException(e, false); // this exception is called on startup with no
            }
        }
*/
        public void
        AdjustDisplay()
        {
            try
            {
                InitializeTopRowBoxes();
                SetLocationsAndHeightsForBoxes();
                //SetBoundariesAndLocationsForBoxes();
                //ComboBoxForHistory.AdjustDisplay();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }
/*
        public void
        Startup ()
        {
            try
            {
                Stream stream = new FileStream ( "ConcorDancer.bin", FileMode.Open,
                        FileAccess.Read, FileShare.Read ) ;
                IFormatter formatter = new BinaryFormatter () ;
                ConcorDancerModel cdm = (ConcorDancerModel) formatter.Deserialize ( stream ) ;
                if ( cdm.RestoreStateFlag == true )
                {
                    ConcorDancer.Cdm = cdm ;
                    ConcorDancer.Cdm.openFileDialog1 = new OpenFileDialog(); // not serializeable
                    ConcorDancer.Cdm.saveFileDialog1 = new SaveFileDialog();
                    restoreStateMenuItem.Checked = true ;
                }
                else
                {
                    restoreStateMenuItem.Checked = false ;
                }
                stream.Close () ;
                if ( cdm.RestoreStateFlag == true )
                {
                    ConcorDancer.Cdm.RestoreState () ;
                }
            }
            catch ( Exception e )
            {
                ConcorDancer.Cdm.HandleException ( e, false ) ;// false : don't report these errors
            }
        }

        public void
        ShutDown ()
        {
            try
            {
                if ( ConcorDancer.Cdm.RestoreStateFlag == true )
                {
                    IFormatter formatter = new BinaryFormatter () ;
                    Stream stream = new FileStream ( "ConcorDancer.bin", FileMode.Create,
                        FileAccess.Write, FileShare.None ) ;
                    formatter.Serialize ( stream, ConcorDancer.Cdm ) ;
                    stream.Close () ;
                }
                else File.Delete ( "ConcorDancer.bin" ) ;
            }
            catch ( Exception e )
            {
                ConcorDancer.Cdm.HandleException ( e, true ) ;
            }
        }
        */
	}
}
