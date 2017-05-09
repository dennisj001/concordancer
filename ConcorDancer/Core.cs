//#define threads2

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Threading ;
using System.Text ;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions ;
using System.Runtime.Serialization ;
using System.Runtime.Serialization.Formatters.Binary ;

namespace ConcorDancer
{
	public partial class
	ConcorDancerTabPage : TabPage
	{
 		StringFindElement 
		AddStringFindElementToLists ( Match match, int currentIndex )
		{
            StringFindElement sfe = null ;
			//ConcorDancer.Cdm.PushNewState () ;
            if (ConcorDancer.Cdm.State.AddStringFindElementToListGuard == false)
            {
                if (ListBox.ListBoxSelectedIndex != currentIndex)
                {
                    string text = (string)ListBox.ListBoxItemStringArray.StringArray[currentIndex];
                    sfe = new StringFindElement(match, CurrentTextBox.FullPathFilename, text, currentIndex, ListBox.WordsBack);
                    ConcorDancer.Cdwf.ComboBoxForHistory.ReplaceAddStringFindElementToList(sfe);
                }
                //ConcorDancer.Cdm.ResetState();
            }
            ConcorDancer.Cdm.State.AddStringFindElementToListGuard = false;
            return sfe;
		}

        public void
        DoSelectedListIndexOnTextBoxArrayList(StringFindElement sfe)
        {
            try
            {
                int selectionStart = sfe.SelectionStart;
                int selectionLength = sfe.SelectionLength;
                foreach (ConcorDancerTextBox tb in TextBoxArrayList)
                {
                    if ((tb.FullPathFilename == sfe.Filename) && (tb.Modified == false ))
                        //if ((tb.Filename == sfe.Filename) && (tb.ListBoxItemStringArray == sfe.ListBoxItemStringArray))
                        //if ( tb.Text == sfe.Ctp.TextBox.Text )
                    {
                        tb.SelectionLength = 0;
                        int visibleTextLength = (int)((((double)tb.DisplayRectangle.Width) /
                            TextBox.PixelWidthPerCharacter) *
                            (((double)tb.DisplayRectangle.Height) / ConcorDancer.TextBoxPixelHeightPerCharacter)); // ?? todo ??
                        int halfVisibleTextLength = visibleTextLength / 2;
                        if (selectionStart < tb.SelectionStart)
                        {
                            if (selectionStart < halfVisibleTextLength)
                            {
                                tb.SelectionStart = 0;
                            }
                            else //if  ( mIndex > visibleTextLength )
                            {
                                tb.SelectionStart = selectionStart - halfVisibleTextLength;
                            }
                        }
                        else
                        {
                            tb.SelectionStart = selectionStart + visibleTextLength;
                            tb.SelectionStart = selectionStart - halfVisibleTextLength;
                        }
                        tb.SelectionStart = selectionStart;
                        tb.SelectionLength = selectionLength;
                    }
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
        }

        public void
		ListBox1_SelectedIndexChanged (object sender, System.EventArgs e)
		{
            if (ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard == false)
            {
                try
                {
                    //ConcorDancerTabPage ctp = (ConcorDancerTabPage) 
                    //ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
                    int currentIndex = ListBox.SelectedIndex;
                    if (currentIndex < 0) return;
                    ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard = true;
                    Match match = ListBox.DetermineMatch();
                    // determine the Text and add the index item to the StringFindElement Lists if its new
                    StringFindElement sfe = AddStringFindElementToLists(match, currentIndex);
                    DoSelectedListIndexOnTextBoxArrayList(sfe);

                    Focus();
                    //ConcorDancer.Cdm.State.listBox1_SelectedIndexChangedGuard = false ;
                }
                catch (Exception ex)
                {
                    ConcorDancer.Cdm.HandleException(ex, false); // we're getting some unexplained exceptions here - bugs in .NET Framework ??
                }
            }
			ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard = false ;
		}
	}
		
	[Serializable]
	public partial class
	ConcorDancerModel
	{
		public void
		ActivateTabPage ( ConcorDancerTabPage ctp )
		{
			//PushNewState () ;
			try
			{
				ConcorDancer.Cdwf.ReadOnlyMenuItem.Checked = ctp.TextBox.ReadOnly ;
				ConcorDancer.Cdwf.SetCounterLabel () ;
				//setButtonText () ;
				ConcorDancer.Cdm.SetWindowTitle () ;
				ConcorDancer.Cdm.Fmtp.Text = "*" ;
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
			//ResetState () ;
		}

		public void
		FindHistoryComboBox_SelectedIndexChanged ()
		{
            // the logic here prevent loops thru this block; SelectedIndex 0 is the top of the list where we put the
            // last item that came thru here
            if (State.FindHistoryComboBoxSelectedIndexChangedGuard == false)
            {
                try
                {
                    StringFindElement sfe =
                        ConcorDancer.Cdwf.ComboBoxForHistory.GetSelectedItemStringFindElement();
                    ActivateStringFindElement(sfe);
                }
                catch (Exception ex)
                {
                    ConcorDancer.Cdm.HandleException(ex, true);
                }
            }
			State.FindHistoryComboBoxSelectedIndexChangedGuard = false ;
		}
		
		public void
		TabControl1_SelectedIndexChanged ()
		{
			//ConcorDancer.Cdm.PushNewState () ;
			//State.calculateAndDisplayListBox = true ;
			try
			{
                //TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                TabPage tp = ConcorDancer.Cdm.CurrentSelectedTabPage;
				if ( tp != null )
                {
                    if ( tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
				    {
                        if (ConcorDancer.Cdm.Fmtp.CurrentDirectoryTabPage != null)
                        {
                            ConcorDancer.Cdwf.Text = ConcorDancer.Cdm.Fmtp.CurrentDirectoryName;
                            ConcorDancer.Cdm.Fmtp.Text =
                                ConcorDancer.Cdm.Fmtp.CurrentDirectoryTabPage.Text;
                            if ((ConcorDancer.Cdm.Fmtp.Width != ConcorDancer.Cdm.Fmtp.SavedWidth) || (ConcorDancer.Cdm.Fmtp.Height != ConcorDancer.Cdm.Fmtp.SavedHeight))
                            {
                                ConcorDancer.Cdm.Fmtp.AdjustDisplay();
                            }
                        }
				    }
				    else 
				    {
					    ConcorDancer.Cdm.ActivateTabPage ( ((ConcorDancerTabPage)tp) ) ;
				    }
                }
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
			//ConcorDancer.Cdm.ResetState () ;
		}

		public void
		ActivateStringFindElement ( StringFindElement sfe )
		{
			if ( State.ActivateFindElementGuard == true ) return ;
			//PushNewState () ;
			try
            {
                //State.calculateAndDisplayListBox = false ;
                ConcorDancerTabPage ctp = sfe.ConcorDancerTabPage;

                //ConcorDancerTabPage currentCtp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
                ctp.ListBox.Item_FullRaw_StringArrayList = sfe.ItemFullRawStringArrayList;
                ctp.ListBox.MatchCollection = sfe.MatchCollection;
                //ctp.CurrentTextBox = sfe.ConcorDancerTextBox;
                ctp.ListBox.SortFlag = sfe.SortedFlag;

                ctp.CheckToMakeCurrent(); // after we reinit with above three statements
                if (sfe.BookmarkFlag == true)
                {
                    ctp.ListBox.SelectedIndex = -1; // -1 : no item is selected
                }
                //else
                sfe.DisplayListBoxItemStringArray (ctp.ListBox);// check SortFlag
                ctp.DoSelectedListIndexOnTextBoxArrayList(sfe);
                {
                    bool saved = ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard;
                    ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard = true;
                    ctp.ListBox.CheckUpdateListBoxFromSelectedListIndex(sfe.ListBoxSelectedIndex);
                    ConcorDancer.Cdm.State.ListBox_SelectedIndexChangedGuard = saved;
                }
                //State.activateFindElementGuard = true ; // prevent looping being caused by findHistoryComboBox.ReplaceAddStringFindElementToList 
                ConcorDancer.Cdwf.ComboBoxForHistory.ReplaceAddStringFindElementToList(sfe);
                //State.activateFindElementGuard = false ; // prevent looping being caused by findHistoryComboBox.ReplaceAddStringFindElementToList
                ConcorDancer.Cdwf.SetCounterLabel();
                ctp.Focus();
            }
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
			ResetState () ;
		}

		public void
		Back ()
		{
			//PushNewState () ;
			try
			{
				TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
				if (  tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) 
				{
					((FileManagerTabPage)tp).Activate () ;
					((FileManagerTabPage)tp).BackOrUpADirectory () ;
				}
				else
				{
					//StringFindElement sfe = (StringFindElement) findList.CircularBefore () ;
					DLLNode<StringFindElement> sfe = FindList.Before ();
					State.AddStringFindElementToListGuard = true ;
					State.FindHistoryComboBoxSelectedIndexChangedGuard = true ;
                    if (sfe != (object)null)
					{
						ActivateStringFindElement ( sfe.Value );
					}
                    //State.FindHistoryComboBoxSelectedIndexChangedGuard = false;
                }
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
			ResetState () ;
		}

		public void
		Forward ( )
		{
			//PushNewState () ;
			try
			{
				TabPage tp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
				if ( tp.GetType ().FullName == "ConcorDancer.FileManagerTabPage" )
				{
					( (FileManagerTabPage)tp ).Activate ();
					( (FileManagerTabPage)tp ).ForwardADirectory ();
				}
				else
				{
					//StringFindElement sfe = (StringFindElement) findList.CircularAfter () ;
					DLLNode<StringFindElement> sfe = FindList.After ();
					State.AddStringFindElementToListGuard = true;
					State.FindHistoryComboBoxSelectedIndexChangedGuard = true;
					if ( sfe != (object)null )
					{
						ActivateStringFindElement ( sfe.Value );
					}
                    //State.FindHistoryComboBoxSelectedIndexChangedGuard = false;
                }
			}
			catch ( Exception ex )
			{
				HandleException ( ex, true ) ;
			}
			ResetState () ;
		}

        public void
        RemoveTabPageListItems(ConcorDancerTabPage ctp)
        {
            try
            {
                foreach (DLLNode<StringFindElement> sfe in FindList)
                //for (DLLNode<StringFindElement> sfe = ConcorDancer.Cdm.FindList.First; sfe != ConcorDancer.Cdm.FindList.Tail; 
                //    sfe = (DLLNode<StringFindElement>)sfe.AfterReference)
                {
                    if (sfe.Value.Filename == ctp._CurrentTextBox.FullPathFilename)
                    {
                        sfe.Remove();
                    }
                }
                // remove all references from this tabpage's file from the history in findHistoryComboBox
                foreach (DLLNode<DLLNode<StringFindElement>> sfeN in ConcorDancer.Cdwf.ComboBoxForHistory.ComboBoxHistoryFindElementList)
                {
                    //StringFindElement sfe = sfeN.Sfe.Value;
                    //if (sfe.ConcorDancerTabPage == ctp)
                    if (sfeN.Value.Value.Filename == ctp._CurrentTextBox.FullPathFilename)
                    {
                        sfeN.Remove();
                    }
                }
                // clear the Items list and put the remaining elements from  sfel Back into the Items list
                ConcorDancer.Cdwf.ComboBoxForHistory.Items.Clear();
                //foreach (StringFindElement sfe in FindList )
                foreach (DLLNode<StringFindElement> sfe in ConcorDancer.Cdm.FindList)
                {
                    ConcorDancer.Cdwf.ComboBoxForHistory.Items.Insert(0,
                        sfe.Value.ListBoxItemStringArray.StringArray[sfe.Value.ListBoxSelectedIndex]);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex, true);
            }
        }
    }
}
