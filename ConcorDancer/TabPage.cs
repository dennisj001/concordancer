using System;
using System.Text.RegularExpressions ;
using System.Collections;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace ConcorDancer
{
	[Serializable]
	public partial class
	ConcorDancerTabPage
	{
		public SplitContainer SplitContainer = new SplitContainer () ;
		public ConcorDancerTextBox TextBox = new ConcorDancerTextBox () ;
        public ConcorDancerListBox ListBox = new ConcorDancerListBox();
		public ArrayList TextBoxArrayList = new ArrayList () ;
		public ConcorDancerTextBox _CurrentTextBox ; //= textBox1 ;
        //public string FullPathFilenameWithModifiedIndicator;
        public RegexOptions RegularExpressionOptions = RegexOptions.IgnoreCase;
        public static string AppendRegExpString = "";
        bool noMouseUp = false;
    
        public ConcorDancerTextBox
        CurrentTextBox 
        {
            get 
            {
                return _CurrentTextBox ;
            }
            set 
            {
                _CurrentTextBox = value ;
                ConcorDancer.CurrentTextBox = value ;
            }
        }

        public string 
		MakeFilenameFromFullPathFilename ( string fullPathFilename )
		{
			
			string filename = ConcorDancer.MakeFilenameFromFullPathName ( fullPathFilename ) ;
			return filename ;
		}

		public string
		Filename ()
		{
            string filename = CurrentTextBox.FullPathFilename.Substring(CurrentTextBox.FullPathFilename.LastIndexOf('\\') + 1);
			return filename ;
		}

        public int
        CountSameFilenameOnTabs ()
        {
            int count = 0;
            foreach (TabPage tp in ConcorDancer.Cdwf.TabControl.TabPages)
            {
                if (tp.GetType().FullName == "ConcorDancer.FileManagerTabPage") continue;
                if (((ConcorDancerTabPage)tp).CurrentTextBox.FullPathFilename == CurrentTextBox.FullPathFilename) count++;
            }
            return count;
        }

        public new void
        Focus()
        {
            if (ConcorDancer.Cdwf.ShiftFocusToTextMenuItem.Checked == true)
            {
                TextBox.Focus();
            }
            else
            {
                ListBox.Focus();
            }
        }

        public void
        MakeBookmark()
        {
            try
            {
                StringFindElement stringFindElement = new StringFindElement(null, _CurrentTextBox.FullPathFilename,
                    CurrentTextBox.Text.Substring(TextBox.SelectionStart,
                    CurrentTextBox.SelectedText.Length), ListBox.SelectedIndex, ListBox.WordsBack);
                stringFindElement.BookmarkFlag = true;
                CurrentTextBox.Focus();
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }
/*
        public void
        FirstBookmark()
        {
            DllNode currentNode = ConcorDancer.Cdm.FindList.Head;
            for (DllNode dllNode = (StringFindElement)currentNode.AfterReference; dllNode != null; dllNode = dllNode.AfterReference)
            {
                if (((StringFindElement)dllNode).BookmarkFlag == true)
                {
                    ConcorDancer.Cdm.ActivateStringFindElement((StringFindElement)dllNode);
                    ConcorDancer.Cdm.FindList.Current = (StringFindElement)dllNode;
                    break;
                }
            }
            TextBox.Focus();
        }
*/
        public void
        NextBookmark()
        {
            DLLNode<StringFindElement> sfe ;
            do
            {
                sfe = ConcorDancer.Cdm.FindList.After();
                if (sfe.Value.BookmarkFlag == true)
                {
                    ConcorDancer.Cdm.ActivateStringFindElement(sfe.Value);
                    //ConcorDancer.Cdm.FindList.Current = sfe;
                    break;
                }
            }
            while (sfe != ConcorDancer.Cdm.FindList.Last);
            TextBox.Focus();
        }

        public void
        PreviousBookmark()
        {
            DLLNode<StringFindElement> sfe;
            do
            {
                sfe = ConcorDancer.Cdm.FindList.Before();
                if (sfe.Value.BookmarkFlag == true)
                {
                    ConcorDancer.Cdm.ActivateStringFindElement(sfe.Value);
                    //ConcorDancer.Cdm.FindList.Current = sfe;
                    break;
                }
            }
            while (sfe != ConcorDancer.Cdm.FindList.First);
            TextBox.Focus();
        }

        public void 
		CheckToMakeCurrent ()
		{
			ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
			if ( ( ctp == null ) || ( this.Text != ctp.Text ) || ( this.ListBox.MatchCollection != ctp.ListBox.MatchCollection ) )
			{
				ConcorDancer.Cdm.State.CalculateAndDisplayListBox = true ; // must be before next statement so updating will be done
				ConcorDancer.Cdm.CurrentConcorDancerTabPage = this ;
			}
		}

        public void
        ReadAFileToTextBox(string filename)
        {
            //ConcorDancer.Cdm.PushNewState () ;
            ConcorDancer.Cdm.State.TextBoxTextChangedGuard = true;
            try
            {
                if ((ConcorDancer.Cdm.OpenFileDialogFilterIndex == 7) || (filename.EndsWith(".rtf"))) // RichText
                {
                    TextBox.LoadFile(filename, RichTextBoxStreamType.RichText); // .Text = fileString ;
                }
                else
                {
                    TextBox.Text = ConcorDancer.Cdm.ReadFileToString(filename);
                }
                TextBox.Modified = false;
                TextBox.FilterIndexFileType = ConcorDancer.Cdm.OpenFileDialogFilterIndex;
                TextBox.FullPathFilename = filename;
                TextBox.ListBoxItemStringArray = null;
                ListBox.TextBoxText = TextBox.Text;
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
            //ConcorDancer.Cdm.ResetState () ;
        }

        public void
        ReadAFile(string filename)
        {
            try
            {
                ReadAFileToTextBox(filename);
                CurrentTextBox.Modified = false;
                CurrentTextBox.TextChanged += new EventHandler(TextBox_TextChanged);
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        _SaveAppendFile(string filename)
        {
            try
            {
                //ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                using (StreamWriter sw = File.AppendText(filename))
                {
                    if (sw != null)
                    {
                        sw.Write("\n\n" + TextBox.SelectedText);
                        sw.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        // Menu.File.Save
        public void
        SaveTextBoxToFile(string filename)
        {
            try
            {
                if (TextBox.FilterIndexFileType == 7) //rtf
                {
                    TextBox.SaveFile(filename, RichTextBoxStreamType.RichText);
                    //,	TextBoxStreamType.PlainText ) ;
                }
                /*
                else if ((TextBox.FilterIndexFileType == 1) || (TextBox.FilterIndexFileType == 2) || (TextBox.FilterIndexFileType == 8))//ascii, htm*, all others
                {
                    TextBox.SaveFile(filename, RichTextBoxStreamType.PlainText);
                    //,	TextBoxStreamType.PlainText ) ;
                }
                */
                else
                {
                    System.Text.Encoding textEncoding;
                    if (TextBox.FilterIndexFileType == 3)
                    {
                        textEncoding = System.Text.Encoding.UTF7;
                    }
                    if (TextBox.FilterIndexFileType == 4)
                    {
                        textEncoding = System.Text.Encoding.UTF8;
                    }
                    if (TextBox.FilterIndexFileType == 5)
                    {
                        textEncoding = System.Text.Encoding.Unicode;
                    }
                    if (TextBox.FilterIndexFileType == 6)
                    {
                        textEncoding = System.Text.Encoding.UTF32;
                    }
                    else // if TextBox.filterIndexFileType == 1,2,8 
                    {
                        textEncoding = System.Text.Encoding.UTF8;
                    }
                    //StreamWriter sw = new StreamWriter ( filename, false, System.Text.Encoding.UTF8 ) ;
                    StreamWriter sw = new StreamWriter(filename, false, textEncoding);
                    {
                        sw.Write(TextBox.Text);
                        sw.Close();
                    }
                }
                TextBox.Modified = false;
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        // Menu.File.Save
        public void
        Delete()
        {
            try
            {
                //ConcorDancer.Cdwf.fileMenuItem.MenuItems.Remove ( MenuItem ) ;
                ConcorDancer.Cdm.RemoveTabPageListItems(this);
                //ConcorDancer.Cdm.ConcorDancerTabPageList.Remove (this) ;
                ConcorDancer.Cdwf.TabControl.TabPages.Remove(this);
                ConcorDancer.Cdwf.SetCounterLabel();
                ConcorDancer.Cdm.SetWindowTitle();
                GC.Collect(); // free the memory
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
        SplitContainer_SplitterMoved(Object sender, EventArgs e)
        {
            ConcorDancer.Cdm.Refresh();
        }

        public void
        TextBox_MouseEnter(Object sender, EventArgs e)
        {
            //if ((ConcorDancer.Cdm.State.TextBoxMouseEnterGuard == false) && (!CurrentTextBox.Focused)) CurrentTextBox.Focus();
            CurrentTextBox.Focus();
        }

        public void
        ListBox_MouseEnter(Object sender, EventArgs e)
        {
            if ( ! ListBox.Focused )ListBox.Focus();
        }

        public
        ConcorDancerTabPage(string fullPathFilename)
        {
            //Cdm = ConcorDancer.Cdm ;
            //Cdwf = ConcorDancer.Cdwf ;

            // splitContainer1
            // 
            SplitContainer.Location = new System.Drawing.Point(4, 4);
            SplitContainer.Dock = DockStyle.Fill;
            // 
            // splitContainer1.Panel1
            // 
            SplitContainer.Panel1.Controls.Add(ListBox);
            // 
            // splitContainer1.Panel2
            // 
            SplitContainer.Panel2.Controls.Add(TextBox);
            SplitContainer.TabIndex = 0;
            SplitContainer.Size = new System.Drawing.Size(
                ConcorDancer.TabPageStartWidth - ConcorDancer.SplitterWidth,
                ConcorDancer.TabPageStartHeight);
            SplitContainer.SplitterDistance = ((ConcorDancer.TabPageStartWidth -
                ConcorDancer.SplitterWidth) / 2);
            SplitContainer.SplitterMoved += new SplitterEventHandler(SplitContainer_SplitterMoved); 
            //
            // listBox1
            //
            ListBox.Dock = DockStyle.Fill;
            ConcorDancer.CurrentListBox = ListBox;

            ListBox.Font = ConcorDancer.ListBoxFont;
            ListBox.BackColor = ConcorDancer.ListBoxBackColor;
            ListBox.ForeColor = ConcorDancer.ListBoxForeColor;

            ListBox.IntegralHeight = false;
            ListBox.ItemHeight = 15;
            ListBox.TabIndex = 6;
            ListBox.TabStop = false;
            ListBox.UseTabStops = false;
            ListBox.MouseEnter += new System.EventHandler(ListBox_MouseEnter);
            ListBox.SelectedIndexChanged += new System.EventHandler(
                ListBox1_SelectedIndexChanged);
            //listBox1.DoubleClick += new System.EventHandler ( listBox1_DoubleClick ) ;
            //ConcorDancer.Cdwf.AddContextMenuItems ( listBox1 ) ;
            //ListBox.TextBoxText = CurrentTextBox.Text;
            ListBox.ContextMenu = new ContextMenu();
            ConcorDancer.Cdwf.AddContextMenuItems(ListBox.ContextMenu);
            //
            // textBox1
            //
            TextBox.Dock = DockStyle.Fill;
            TextBox.AutoSize = true;
            TextBox.HideSelection = false;

            TextBox.Font = ConcorDancer.TextBoxFont;
            TextBox.ForeColor = ConcorDancer.TextBoxForeColor;
            TextBox.BackColor = ConcorDancer.TextBoxBackColor; // FromName ( "Ivory" ) ;

            TextBox.ReadOnly = ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked;
            TextBox.Multiline = true;
            //textBox1.ScrollBars = ScrollBars.Vertical ;
            //textBox1.IntegralHeight = false;
            TextBox.TabIndex = 7;
            TextBox.TabStop = false;
            TextBox.Text = "";
            TextBox.Enabled = true;
            TextBox.MouseEnter += new EventHandler(TextBox_MouseEnter);
            TextBox.MouseUp += new MouseEventHandler(TextBox_MouseUp);
            //textBox1.MouseEnter += new EventHandler ( textBox_MouseEnter ) ;
            TextBox.DoubleClick += new System.EventHandler(TextBox_DoubleClick);
            TextBox.KeyUp += new KeyEventHandler(TextBox_KeyUp);
            TextBox.TextChanged += new EventHandler(TextBox_TextChanged);
            //ConcorDancer.Cdwf.AddContextMenuItems ( textBox1 ) ;
            TextBox.ContextMenu = new ContextMenu();
            ConcorDancer.Cdwf.AddContextMenuItems(TextBox.ContextMenu);
            TextBoxArrayList.Add(TextBox);
            //textBox1.previousBeforeSplit = textBox1 ;
            //ConcorDancer.CurrentTextBox = TextBox ;
            CurrentTextBox = TextBox;
            TextBox.ListBoxItemStringArray = ListBox.ListBoxItemStringArray;
            Dock = DockStyle.Fill;
            TabIndex = 0;
            Controls.Add(SplitContainer);
            //MakeFilenameFromFullPathName ( fullPathFilename ) ;
            CurrentTextBox.FullPathFilename = fullPathFilename;
            //TextBox.Filename = fullPathFilename ;


            ConcorDancer.Cdwf.TabControl.Controls.Add(this);
            ConcorDancer.Cdwf.TabControl.SelectedTab = this; // initialized in SetupANewConcorDancerTabPage
        }
    }
}
