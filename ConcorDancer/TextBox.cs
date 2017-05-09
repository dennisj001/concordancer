using System;
using System.Drawing ;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions ;

namespace ConcorDancer
{
	public class
	ConcorDancerTextBox : RichTextBox
	{
		public int FilterIndexFileType ;
		//public ConcorDancerTextBox previousBeforeSplit ;
		public string FullPathFilename ;
        public ListBoxItemStringArray ListBoxItemStringArray;
        public int PixelWidthPerCharacter;
        int BoxWidth_inCharacters ;

        public ConcorDancerTextBox(string filename)
        {
            FullPathFilename = filename;
            FilterIndexFileType = 1; // default - ascii
            Init();
        }
        public ConcorDancerTextBox()
        {
            FullPathFilename = null ;
            FilterIndexFileType = 1; // default - ascii
            Init();
        }

        public void
        Init()
        {
            ListBoxItemStringArray = null;
            //TextBoxText = text;
            Modified = false;
            Font = ConcorDancer.TextBoxFont;
            System.Drawing.Size size = TextRenderer.MeasureText(ConcorDancer.MeasuredText, Font);
            PixelWidthPerCharacter = (int)(size.Width / ConcorDancer.MeasuredText.Length);// (384.0 / 53.0);	// fraction derived by experiment with fixed font size 9, same for 8 and 10
            BoxWidth_inCharacters = Width / PixelWidthPerCharacter;
            //WidthInCharacters = (int)(Width / PixelWidthPerCharacter) + 1;
        }

    }

	public partial class
	ConcorDancerTabPage : TabPage
	{
		public void
		SplitTextBox ( Orientation orientation )
		{
            ConcorDancerTextBox textBox = new ConcorDancerTextBox( CurrentTextBox.FullPathFilename );
			TextBoxArrayList.Add ( textBox ) ;
			//textBox.previousBeforeSplit = currentTextBox ;
			textBox.Dock = DockStyle.Fill ;
			textBox.AutoSize = true ;
			//textBox1.IntegralHeight = false;
			textBox.HideSelection = false;
			textBox.ReadOnly = ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked ;
			
			textBox.ContextMenu = new ContextMenu() ;
			ConcorDancer.Cdwf.AddContextMenuItems ( textBox.ContextMenu ) ;
			MenuItem appendTextMenuItem = 
			    ConcorDancer.Cdwf.FindAppendTextMenuItem ( ConcorDancer.Cdwf.contextMenuItem ) ;
			foreach ( MenuItem menuItem in appendTextMenuItem.MenuItems )
			{
			    ConcorDancer.Cdwf.FindAppendTextMenuItem ( textBox.ContextMenu ).
			        MenuItems.Add ( menuItem.CloneMenu () ) ;     
			}

			textBox.Font = ConcorDancer.TextBoxFont ;
			textBox.BackColor = ConcorDancer.TextBoxBackColor ;
			textBox.ForeColor = ConcorDancer.TextBoxForeColor ;
			//textBox.Multiline = true ;
			textBox.FilterIndexFileType = CurrentTextBox.FilterIndexFileType ;

			//textBox.ScrollBars = ScrollBars.Vertical ;
			//ReadAFileToTextBox ( CurrentTextBox.Filename ) ;
            textBox.Text = CurrentTextBox.Text;

            textBox.ListBoxItemStringArray = CurrentTextBox.ListBoxItemStringArray;
			//ConcorDancer.Cdm.ReadAFileToTextBox ( 
				//ConcorDancer.Cdm.CurrentConcorDancerTabPage.FullPathName, currentTextBox ) ;
			//textBox.Text = currentTextBox.Text ;
			textBox.SelectionStart = CurrentTextBox.SelectionStart ;
			textBox.TextChanged += new EventHandler ( TextBox_TextChanged ) ;
			//textBox.MouseEnter += new EventHandler ( textBox_MouseEnter ) ;
            textBox.MouseEnter += new EventHandler(TextBox_MouseEnter);
            textBox.MouseUp += new MouseEventHandler(TextBox_MouseUp);
			textBox.DoubleClick += new System.EventHandler (TextBox_DoubleClick) ;
			textBox.KeyUp += new KeyEventHandler ( TextBox_KeyUp ) ;
			
			SplitContainer splitContainer = new SplitContainer () ;
			splitContainer.Dock = DockStyle.Fill ;
			splitContainer.Orientation = orientation ;
			
			SplitterPanel splitterPanel = (SplitterPanel) CurrentTextBox.Parent ;
			//SplitContainer splitContainerParent = (SplitContainer) splitterPanel.Parent ;
			splitterPanel.Controls.Remove (CurrentTextBox) ; //  splitContainer1.Panel2.Controls.Remove ( currentTextBox ) ; 
			splitContainer.Panel1.Controls.Add ( textBox ) ; 
			splitContainer.Panel2.Controls.Add ( CurrentTextBox ) ; 
			splitterPanel.Controls.Add ( splitContainer ) ; 
			// this (below) needs to be done after the splitContainer is added for some reason ??
			if ( orientation == Orientation.Vertical ) 
			splitContainer.SplitterDistance = 
				splitterPanel.Width / 2 - splitContainer.SplitterWidth / 2; 
			else splitContainer.SplitterDistance = 
				splitterPanel.Height / 2 - splitContainer.SplitterWidth / 2  ; 
			//ConcorDancer.Cdwf.SetBoundariesAndLocationsForBoxes () ;
            //CurrentTextBox = textBox ;
		}
		
		public void
		UnsplitTextBox ()
		{
			SplitterPanel splitterPanel = (SplitterPanel) CurrentTextBox.Parent ;
			SplitContainer splitContainer = (SplitContainer) splitterPanel.Parent ;
			SplitterPanel parentSplitterPanel = (SplitterPanel) splitContainer.Parent ;
			if ( splitContainer != SplitContainer )
			{
				parentSplitterPanel.Controls.Remove ( splitContainer ) ;
				parentSplitterPanel.Controls.Add ( CurrentTextBox ) ;
			}
			//((ConcorDancerTabPage)ConcorDancer.Cdm.CurrentConcorDancerTabPage).FullPathName = currentTextBox.filename ;
			ReadAFileToTextBox ( CurrentTextBox.FullPathFilename ) ;
			//ConcorDancer.Cdwf.SetBoundariesAndLocationsForBoxes () ;
			//ConcorDancer.Cdm.SetWindowTitle () ;
		}
                
        public void
		TextBox_KeyUp ( object sender, KeyEventArgs e )
		{
			try
			{
				if ( ! e.Control )
				{
					switch ( e.KeyCode )
					{
						case Keys.Enter :
						{
							if  ( ((ConcorDancerTextBox)sender).ReadOnly == true ) 
								ConcorDancer.Cdm.DisplayConcordanceList () ;
							break ;
						}
						default : break ;
					}
				}
				else if ( e.Control )
				{
					switch ( e.KeyCode )
					{
                        case Keys.M: // Bookmark Set Before
                        {
                            MakeBookmark();
                            break;
                        }
                        case Keys.B: // Bookmark Set Before
                        {
                            PreviousBookmark();
                            break;
                        }
                        case Keys.A: // Bookmark Set After
                        {
                            NextBookmark () ;
                            break;
                        }
                        case Keys.C:
                        {
                            ((ConcorDancerTextBox)sender).Copy () ;
                            break;
                        }
                        case Keys.X:
						{
							((ConcorDancerTextBox)sender).Cut () ;
 							break ;
						}
						case Keys.V :
						{
                            if (((ConcorDancerTextBox)sender).ReadOnly == true)
                            {
                                ((ConcorDancerTextBox)sender).Paste();
                                //sync Text
                                foreach ( Object o in TextBoxArrayList )
                                {
                                    if ( ((ConcorDancerTextBox)o).FullPathFilename == ((ConcorDancerTextBox)sender).FullPathFilename )
                                    {
                                        ((ConcorDancerTextBox)o).Text = ((ConcorDancerTextBox)sender).Text ; 
                                    }
                                }
                            }
							break ;
						}
						default : return ;
					}
				}
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		TextBox_TextChanged (object sender, System.EventArgs e)
		{
			try
			{
				if ( ConcorDancer.Cdm.State.TextBoxTextChangedGuard == false )
				{
                    int count = 0 ;
                    foreach ( ConcorDancerTextBox tb in TextBoxArrayList ) if ( tb == CurrentTextBox ) count++ ; // in case of after a SplitTextBox 
                    if ( ( sender == CurrentTextBox ) && ( count < 2 ) )
                    {
                        ConcorDancer.Cdm.RemoveTabPageListItems(
                            ConcorDancer.Cdm.CurrentConcorDancerTabPage);
                    }
					ConcorDancer.Cdwf.SetCounterLabel () ;
					ConcorDancer.Cdm.SetWindowTitle () ;
				}
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
			//ConcorDancer.Cdm.State.TextBoxTextChangedGuard = false ;
		}
		
		public void
		TextBox_DoubleClick (object sender, System.EventArgs e)
		{
			try
			{
				string selected = ((ConcorDancerTextBox)sender).SelectedText ; 
				ConcorDancer.Cdm.SetComboBoxText ( selected.Trim () ) ;
				noMouseUp = true ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		TextBox_MouseUp ( object sender, MouseEventArgs e )
		{
			if ( noMouseUp )
			{
				noMouseUp = false ;
				return ;
			}
			try
			{
                ConcorDancer.Cdm.State.TextBoxMouseEnterGuard = true;
				CurrentTextBox = (ConcorDancerTextBox) sender ;
                ListBox.TextBoxText = CurrentTextBox.Text;
				//ConcorDancer.CurrentTextBox = CurrentTextBox ;
				if ( CurrentTextBox.SelectionLength > 0 ) 
				{
					ConcorDancer.Cdm.SetComboBoxText ( CurrentTextBox.SelectedText.Trim () ) ;
				}
                //ConcorDancer.Cdm.SetWindowTitle();
				//ConcorDancer.Cdm.SetComboBoxText ( currentTextBox.SelectedText ) ;
					//Regex.Match ( currentTextBox.SelectedText , "[^!-@` ]*" ).Value  ) ;
                ConcorDancer.Cdm.State.TextBoxMouseEnterGuard = false;
            }
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
	}
}
