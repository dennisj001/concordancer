using System;
using System.Text.RegularExpressions ;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace ConcorDancer
{
    public class
    CdmState
    {
        // all guards are turned off on creation 
        public bool ListBox_SelectedIndexChangedGuard = false;
        public bool ActivateFindElementGuard = false;
        public bool CalculateAndDisplayListBox = true;
        public bool TextBoxTextChangedGuard = false;
        public bool AddStringFindElementToListGuard = false;
        public bool FindHistoryComboBoxSelectedIndexChangedGuard = false;
        public bool CurrentFileManagerTabPageAdjustDisplayGuard = false;
        public bool TextBoxMouseEnterGuard = false;

        public CdmState
        MemberWiseClone()
        {
            return (CdmState)this.MemberwiseClone();
        }
    }

    public class
    DLLNode<T> 
    {
        public DLLNode<T> BeforeReference;
        public DLLNode<T> AfterReference;
        public T Value;

        public DLLNode(T t)
        {
            Value = t;
        }

        public DLLNode()
        {
        }

        public DLLNode<T> Next
        {
            get
            {
                return AfterReference;
            }
        }

        public DLLNode<T> Previous
        {
            get
            {
                return BeforeReference;
            }
        }

        public void
        Remove()
        {
            //if (this == null) return ;
            try
            {
                if ((BeforeReference == null) || (AfterReference == null)) return;// don't allow removing a list.Head or list.Tail 
                BeforeReference.AfterReference = AfterReference;
                AfterReference.BeforeReference = BeforeReference;
            }
            catch
            {
                return;
            }
        }

        public void
        InsertThisBeforeANode(DLLNode<T> node) // Insert this Before node - before : toward the head of the list - before
        {
            //if (this == null) return;
            try
            {
                if (node.BeforeReference == null) return; // can not Insert this Before the Head which always has no Before
                BeforeReference = node.BeforeReference;
                node.BeforeReference.AfterReference = this;
                node.BeforeReference = this;
                AfterReference = node;
            }
            catch
            {
                return;
            }
        }

        public void
        InsertThisAfterANode(DLLNode<T> node) // Insert this After node : toward the tail of the list - after
        {
            //if (this == null) return;
            try
            {
                if (node.AfterReference == null) return; // can not Insert this After the Tail which always has no After
                AfterReference = node.AfterReference;
                node.AfterReference.BeforeReference = this;
                node.AfterReference = this;
                BeforeReference = node;
            }
            catch
            {
                return;
            }
        }
    }

    public class
    DLList<T> : IEnumerable
    {
        DLLNode<T> HeadNode ;
        DLLNode<T> TailNode ;
        DLLNode<T> CurrentObject ;

        public void
        Init()
        {
            HeadNode.BeforeReference = null;
            HeadNode.AfterReference = TailNode;

            TailNode.BeforeReference = HeadNode;
            TailNode.AfterReference = null;

            CurrentObject = null;
        }

        public DLList()
        {
            HeadNode = new DLLNode <T>();
            TailNode = new DLLNode <T>();
            Init();
        }

        public void
        Reset()
        {
            //if (this == null) return;
            Init();
        }

        public DLLNode<T>
        Current
        {
            get
            {
                return CurrentObject;
            }
            set
            {
                CurrentObject = (DLLNode<T>)value;
            }
        }

        public DLLNode<T>
        Head
        {
            get
            {
                //if (this == null) return null;
                return HeadNode;
            }
        }

        public DLLNode<T>
        Tail
        {
            get
            {
                //if (this == null) return null;
                return TailNode;
            }
        }

        public DLLNode<T>
        First
        {
            get
            {
                return (DLLNode<T>)HeadNode.AfterReference;
            }
        }

        public DLLNode<T>
        Last
        {
            get
            {
                return (DLLNode<T>)TailNode.BeforeReference;
            }
        }

        public void
        AddToHead(DLLNode<T> node)
        {
            //if ((this == null) || (node == null)) return;
            try
            {
                node.InsertThisAfterANode(HeadNode);
                CurrentObject = node;
            }
            catch
            {
                return;
            }
        }

        public void
        AddToTail(DLLNode<T> node)
        {
            //if ((this == null) || (node == null)) return;
            try
            {
                node.InsertThisBeforeANode(TailNode);
                CurrentObject = node;
            }
            catch
            {
                return;
            }
        }

        public void
        Add(DLLNode<T> node)
        {
            AddToTail(node);
        }

        bool
        NodeIsAlreadyPresentInList(DLLNode<T> node)
        {
            //if ((this == null) || (node == null)) return true;
            foreach (DLLNode<T> index in this)
            {
                if (index == node)
                {
                    return true;
                }
            }
            return false;
        }

        public void
        AddIfNotAlreadyPresent(DLLNode<T> node)
        {
            try
            {
                if (!NodeIsAlreadyPresentInList(node)) AddToTail(node);
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
                return;
            }
        }

        public void
        Remove(DLLNode<T> node)
        {
            try
            {
                //if ((this == null) || (node == null)) return;
                foreach (DLLNode<T> index in this)
                {
                    if (index == node) node.Remove();
                }
            }
            catch
            {
                return;
            }
        }

        public DLLNode<T>
        CircularBefore()
        {
            if (CurrentObject == null) return CurrentObject;
            if (CurrentObject.BeforeReference == null)
            {
                CurrentObject = (DLLNode < T >) TailNode.BeforeReference;
            }
            else CurrentObject = (DLLNode<T>)CurrentObject.BeforeReference;
            return CurrentObject;
        }

        public DLLNode<T>
        CircularAfter()
        {
            if (CurrentObject == null) return CurrentObject;
            if (CurrentObject.AfterReference == null)
            {
                CurrentObject = (DLLNode<T>)HeadNode.AfterReference;
            }
            else CurrentObject = (DLLNode<T>)CurrentObject.AfterReference;
            return CurrentObject;
        }

        public DLLNode<T>
        Before()
        {
            // don't leave CurrentObject null
            try
            {
                if (CurrentObject.BeforeReference != HeadNode)
                {
                    CurrentObject = (DLLNode<T>)CurrentObject.BeforeReference;
                    return CurrentObject;
                }
                else return First;

            }
            catch //(Exception ex)
            {
                //ConcorDancer.Cdm.HandleException(ex, true);
                CurrentObject = First;
                return null;
            }
        }

        public DLLNode<T>
        After()
        {
            // don't leave CurrentObject null
            try
            {
                if (CurrentObject.AfterReference != TailNode)
                {
                    CurrentObject = (DLLNode<T>)CurrentObject.AfterReference;
                    return CurrentObject;
                }
                else return Last;
            }
            catch //(Exception ex)
            {
                //ConcorDancer.Cdm.HandleException(ex, true);
                CurrentObject = Last;
                return null;
            }
        }
        public IEnumerator GetEnumerator()
        {
            for (DLLNode<T> index = First; index != TailNode; index = index.AfterReference)
            {
                yield return index;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
/*
    public class
    StringNode : DLLNode<String>
    {
        //public string Text;
        public StringNode(string aString)
        {
            Value = aString;        
        }
    }

    public class
    StringFindElementNode : DLLNode<DLLNode<StringFindElement>>
    {
        DLLNode<StringFindElement> Node;
        public StringFindElementNode(StringFindElement sfe)
        {
            Node.Value = sfe;
            //Sfe = s;
        }
    }
*/
    public class
	StringFindElement 
	{
        public int MatchIndex;
        public Match Match;
        public string Text;
		public int ListBoxSelectedIndex ;
        public string [] ItemFullRawStringArrayList;
		public ListBoxItemStringArray ListBoxItemStringArray ;
		public ConcorDancerTabPage Ctp ;
		public int SelectionLength ;
		public int SelectionStart ;
		public bool BookmarkFlag ;
        public bool SortedFlag ;
        public MatchCollection MatchCollection;
        public string Filename;
        public int WordsBack;
        public int ListBoxWidth;
        const int ItemWidth = 256;
        const int HalfItemWidth = ItemWidth / 2;

        public ConcorDancerTabPage
		ConcorDancerTabPage
		{
			get
			{
				return Ctp ;
			}
			set
			{
				Ctp = value ;
			}
		}
/*		
		public void
		RemovePrevious ( DLList il )
		{
			try
			{
				restart : 
				foreach ( DLLNode<StringFindElement> node in il )
				{
					if ( (node.Value.ConcorDancerTabPage.FullPathFilename == 
						ConcorDancerTabPage.FullPathFilename) && 
						( (((StringFindElement) node).SelectionStart) == SelectionStart ) && 
						( (((StringFindElement) node).SelectionLength) == SelectionLength ) ) 
					{
						il.Remove ( node ) ;
						il.Reset () ;
						goto restart ;
					}
				}
			}
			catch ( Exception e )
			{
				ConcorDancer.Cdm.HandleException ( e, true ) ;
			}
		}

        //public static bool
        public new static bool
        Equals(object a, object b)
        {
            return ReferenceEquals(a,b);
        }

        // overload operator ==
        public static bool
        operator ==(StringFindElement a, StringFindElement b)
        {
            return a.Equals(b);
        }

        // overload operator !=
        public static bool
        operator !=(StringFindElement a, StringFindElement b)
        {
            return ! ( a.Equals ( b) ) ;
        }
*/

        public string
        SelectMatchTextAndFitIntoWidthOfBox (int i, int boxWidth_inCharacters) //int pixelWidthPerCharacter, int width)
        {
            try
            {
                //int i = ListBoxSelectedIndex;
                string text;
                int textPosition;
                //double halfTheBoxWidth_inCharacters = boxWidth_inCharacters/2 ;

                Match match = MatchCollection[i];
                if ((SortedFlag == true) ||
                    (ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true))
                {
                    text = (string)ItemFullRawStringArrayList[
                        ((IndexedMatch)(ListBoxItemStringArray.SortedIndexedMatch[i])).MatchIndex];
                }
                else
                {
                    text = (string)ItemFullRawStringArrayList[i];
                }

                if (ConcorDancer.Cdwf.SetListBoxViewCenteredOptionMenuItem.Checked == true)
                {
                    int matchLength = match.Length;
                    textPosition = HalfItemWidth - ((matchLength < boxWidth_inCharacters) ? ((boxWidth_inCharacters - matchLength) / 2) : 0);
                }
                else
                {
                    if (ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked == true)
                    {
                        textPosition = HalfItemWidth;
                    }
                    else if (ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked == true)
                    {
                        textPosition = HalfItemWidth;
                    }
                    else // ConcorDancer.Cdwf.setListBoxViewCenteredOptionMenuItem.Checked == false 
                    {
                        ConcorDancerSortComparer ConcorDancerSortComparer = new ConcorDancerSortComparer();
                        textPosition = HalfItemWidth - (match.Index -
                            ConcorDancerSortComparer.GetWordsBackIndex(match.Index));
                    }
                }
                if (ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked == true)
                {
                    textPosition -= HalfItemWidth;
                    if (textPosition < 0) textPosition = 0;
                    //HorizontalScrollbar = true;
                }
                int length = ItemWidth - textPosition;
                return text.Substring(textPosition, length); // 1 : round up double
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, true);
            }
            return null;
        }

        public void
        DisplayListBoxItemStringArray ( ConcorDancerListBox clb )
        // determine what collections to display and display them appropriately for the characteristics of the listBox such as width
        {
            ListBoxItemStringArray lbisa;
            if (ListBoxWidth == clb.Width) lbisa = ListBoxItemStringArray;
            else lbisa = BuildListBoxItemStringArray ();
            clb.SortFlag = SortedFlag ;
            clb.DisplayListBoxItemStringArray (lbisa);
            //_CalculateAndDisplayListBoxStringArray() ;
        }

        public ListBoxItemStringArray
        BuildListBoxItemStringArray()
        // determine what collections to display and setup to display the strings appropriately (centered,etc.) 
        // for the characteristics of the listBox such as width
        {
            try
            {
                if ((SortedFlag == true) || (ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true))
                // build IndexedMatchArray
                {
                    ArrayList imArray = new ArrayList(MatchCollection.Count);
                    for (int i = 0; i < MatchCollection.Count; i++)
                    {
                        IndexedMatch im = new IndexedMatch();
                        im.Match = MatchCollection[i];
                        im.MatchIndex = i;
                        imArray.Add(im);
                        //imArray [i] = im ;
                    }
                    ConcorDancerSortComparer concorDancerSortComparer = new ConcorDancerSortComparer();
                    concorDancerSortComparer.WordsBack = WordsBack;
                    concorDancerSortComparer.Text = Ctp.TextBox.Text;
                    imArray.Sort(concorDancerSortComparer);
                    ListBoxItemStringArray = new ListBoxItemStringArray(new string[MatchCollection.Count], imArray);
                    //SortedIndexedMatchArrayList = imArray;
                    ListBoxItemStringArray.SortFlag = true;
                    ListBoxItemStringArray.WordsBack = WordsBack;
                }
                else ListBoxItemStringArray = new ListBoxItemStringArray(new string[MatchCollection.Count]);

                for (int i = 0; i < MatchCollection.Count; i++)
                {
                    ListBoxItemStringArray.StringArray[i] = SelectMatchTextAndFitIntoWidthOfBox(i, Ctp.ListBox.Width / Ctp.TextBox.PixelWidthPerCharacter);
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException(ex, false); // has to come thru here on startup
            }
            //CurrentTextBox.ListBoxItemStringArray = ListBoxItemStringArray;
            return ListBoxItemStringArray;
        }

        public
        StringFindElement(){}

        public
        StringFindElement(Match match, string filename, string text, int listBoxSelectedIndex, int wordsBack)
		{
            Ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage; //.MemberWiseClone () ; // ?? Must or we will not keep the ItemStringArray used  by ConcorDancer.Cdm.Back/ConcorDancer.Cdm.Forward functions
            //ListBox = Ctp.ListBox;
            ListBoxWidth = Ctp.ListBox.Width;
            MatchCollection = Ctp.ListBox.MatchCollection;
            MatchIndex = listBoxSelectedIndex;
            if (match != null)
            {
                SelectionLength = match.Value.Length;
                SelectionStart = match.Index;
            }
            else
            {
                SelectionLength = Ctp.CurrentTextBox.SelectionLength;
                SelectionStart = Ctp.CurrentTextBox.SelectionStart;
            }
			ListBoxSelectedIndex = listBoxSelectedIndex ;
			Text = text ;
            SortedFlag = Ctp.ListBox.SortFlag;
            ItemFullRawStringArrayList = Ctp.ListBox.Item_FullRaw_StringArrayList;
            WordsBack = wordsBack;
            Filename = filename;
            ListBoxItemStringArray = Ctp.ListBox.ListBoxItemStringArray;
            //Ctp.TextBox.ListBoxItemStringArray = ListBoxItemStringArray; // initialize here for DoSelectedListIndexOnTextBoxArrayList
            //DLLNode <StringFindElement> node = new DLLNode <StringFindElement> (this) ;
            ConcorDancer.Cdm.FindList.AddIfNotAlreadyPresent(new DLLNode<StringFindElement>(this));
		}
	}

	public class
	IndexedMatch
	{
		Match match = null ;
		int matchIndex = 0 ;

		public Match
		Match
		{
			get
			{
				return match ;
			}
			set
			{
				match = value ;
			}
		}

		public int
		MatchIndex
		{
			get
			{
				return matchIndex ;
			}
			set
			{
				matchIndex = value ;
			}
		}
	}
	
	[Serializable]
	public class
	MenuAppendFileElement 
	{
		string fullPathFilename = null ;
		string menuItemText = null ;

		public string
		MafeMenuItemText
		{
			get
			{
				return menuItemText ;
			}
			set
			{
				menuItemText = value ;
			}
		}
		public string
		MafeFileName
		{
			get
			{
				return fullPathFilename ;
			}
			set
			{
				fullPathFilename = value ;
			}
		}
		
		public
		MenuAppendFileElement ( string fullPathFilename )
		{
		 	try
			{
				string filename1 = fullPathFilename.Substring (
					   fullPathFilename.LastIndexOf ( '\\') + 1 ) ;
				MenuItem menuItem = new MenuItem ( filename1, new System.EventHandler (
					ConcorDancer.Cdwf.AppendFileMenuFileItem_Click ) ) ;
				MenuItem appendTextMenuItem = ConcorDancer.Cdwf.FindAppendTextMenuItem ( ConcorDancer.Cdwf.contextMenuItem ) ;
				appendTextMenuItem.MenuItems.Add ( menuItem ) ;
				foreach ( TabPage tp in
						ConcorDancer.Cdwf.TabControl.TabPages )
				{
					if (  tp.GetType().FullName == "ConcorDancer.FileManagerTabPage" ) continue ;
					foreach ( ConcorDancerTextBox ctb in ((ConcorDancerTabPage)tp).TextBoxArrayList )
					{
					    if ( ctb != ((ConcorDancerTabPage)tp).CurrentTextBox ) 
					    {
    						ctb.ContextMenu.MenuItems.Add (
    							ConcorDancer.Cdwf.contextMenuItem.CloneMenu () ) ;
						}
					    ConcorDancer.Cdwf.FindAppendTextMenuItem ( ctb.ContextMenu ).
					        MenuItems.Add ( menuItem.CloneMenu () ) ;     
					}
				}
				MafeMenuItemText = filename1 ;
				MafeFileName = fullPathFilename ;
                DLLNode<MenuAppendFileElement> node = new DLLNode<MenuAppendFileElement>();
                node.Value = this;
                ConcorDancer.Cdm.AppendFileList.Add(node);
            }
			catch ( Exception e )
			{
				ConcorDancer.Cdm.HandleException ( e, true ) ;
			}
		}
	}
    /*
    public class
    AlphabeticalSortComparer : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(x, y));
        }

    }
    */

	public class
	ConcorDancerSortComparer : IComparer
	{
		string text ;
		int wordsBack = ConcorDancer.Cdm.WordsBack ;

		public string
		Text
		{
			get
			{
				return text ;
			}
			set
			{
				text = value ;
			}
		}

		public int
		WordsBack
		{
			get
			{
				return wordsBack ;
			}
			set
			{
				wordsBack = value ;
			}
		}

		public int
		GetWordsBackIndex ( int index )
		{
			if ( wordsBack > 0 )
			{
				for ( int i = 0 ; ( index >= 0 ) && ( i < wordsBack ) ; i++ )
				{
					index -- ;
					while ( ( index >= 0 ) && ! ( Char.IsLetterOrDigit ( text [ index ] ) ) ) index -- ;
					while ( ( index >= 0 ) && ! ( Char.IsWhiteSpace ( text [ index ] ) ) ) index -- ;
				}
				index ++ ; // adjust ConcorDancer.Cdm.Back to last non whitespace character
			}
			return index ;
		}

		int
		IComparer.Compare ( object x, object y )
		{
			try
			{
				CompareInfo compInfo = CultureInfo.InvariantCulture.CompareInfo;
				CompareOptions compOptions = CompareOptions.IgnoreNonSpace ; // | CompareOptions.IgnoreSymbols ;
				if ( ConcorDancer.Cdwf.CaseSensitiveFindsMenuItem.Checked == true )
				{
					compOptions |= CompareOptions.IgnoreCase ;
				}
				Match m1 = ((IndexedMatch) x).Match , m2 = ((IndexedMatch) y).Match ;
				int index = m1.Index ;
				if ( wordsBack > 0 ) index = GetWordsBackIndex ( index ) ;
				string m1s = text.Substring ( index, 100 ) ;
				index = m2.Index ;
				if ( wordsBack > 0 ) index = GetWordsBackIndex ( index ) ;
				string m2s = text.Substring ( index, 100 ) ;
				int returnValue = compInfo.Compare ( m1s, m2s, compOptions ) ;
				return returnValue ;
			}
			catch ( Exception e )
			{
				ConcorDancer.Cdm.HandleException ( e, true ) ;
				return 0 ;
			}
		}
	}
}
