using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace ConcorDancer
{
    public class ListBoxItemStringArray
    {
        public string[] StringArray;
        public ArrayList SortedIndexedMatch;
        public bool SortFlag = false;
        public int WordsBack = -1;
        //public ConcorDancerTextBox TextBox;

        public
        ListBoxItemStringArray (string[] stringArray)
        {
            StringArray = stringArray;
        }

        public
        ListBoxItemStringArray (string[] stringArray, ArrayList sortedIndexedMatchArrayList)
        {
            StringArray = stringArray;
            SortedIndexedMatch = sortedIndexedMatchArrayList;
            //SortFlag = true;
            //WordsBack = ConcorDancer.Cdm...WordsBack;
        }
    }

    public class ConcorDancerListBox : ListBox
    {
        public string TextBoxText;
        //public ConcorDancerTextBox CurrentTextBox;
        public bool SortFlag = ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked;
        public MatchCollection MatchCollection = null;
        public string[] Item_FullRaw_StringArrayList = null;// full length strings 256 char
        public ListBoxItemStringArray ListBoxItemStringArray;
        public bool FindWordsAnyOrderWithinSentence = ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked;
        public bool FindWordsAnyPlaceWithinSentence = ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked;
        public int WordsBack = ConcorDancer.Cdm.WordsBack;
        public int ListBoxSelectedIndex = 0;
        public string RequestedWord = "";
        ConcorDancerSortComparer ConcorDancerSortComparer = new ConcorDancerSortComparer ();
        public static int PixelWidthPerCharacter;
        public int BoxWidth_inCharacters;

        const int ItemWidth = 256;
        const int HalfItemWidth = ItemWidth / 2;
        //int HalfRequestedWordLength;
        //double ListBoxCharWidth;
        //double HalfListBoxCharWidth;

        public
        ConcorDancerListBox ()
        {
            Init ();
        }

        public void
        Init ()
        {
            //TextBoxText = text;
            Font = ConcorDancer.ListBoxFont;
            System.Drawing.Size size = TextRenderer.MeasureText (ConcorDancer.MeasuredText, Font);
            PixelWidthPerCharacter = (int)(size.Width / ConcorDancer.MeasuredText.Length);// (384.0 / 53.0);	// fraction derived by experiment with fixed font size 9, same for 8 and 10
            //BoxWidth_inCharacters = Width / PixelWidthPerCharacter;
            //WidthInCharacters = (int)(Width / PixelWidthPerCharacter) + 1;
        }

        public bool
        FindWordsAnyPlaceWithinSentenceFlag
        {
            get
            {
                return FindWordsAnyPlaceWithinSentence;
            }
            set
            {
                FindWordsAnyPlaceWithinSentence = value;
            }
        }

        public bool
        FindWordsAnyOrderWithinSentenceFlag
        {
            get
            {
                return FindWordsAnyOrderWithinSentence;
            }
            set
            {
                FindWordsAnyOrderWithinSentence = value;
            }
        }

        public void
        BuildListBoxItem_FullRaw_StringArray ()
        // build the array of items to be used by adjustListBoxDisplay and
        // ListBox1_SelectedIndexChanged
        {
            try
            {
                //ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
                int textLength = TextBoxText.Length;
                //int width = (int) listBox1.CreateGraphics().MeasureString(listBox1.Items[listBox1.Items.Count -1].ToString(),
                //	listBox1.Font).Width; // from ConcorDancerListBox.ColumnWidth example 
                int itemWidthPlus = ItemWidth + HalfItemWidth + 1; // be able to take a snapshot 256 wide from up to half the width
                char[] item = new char[itemWidthPlus + 1];
                Item_FullRaw_StringArrayList = new string[MatchCollection.Count];// remember ItemFullRawStringArrayList is the full 256 wide raw text
                //ArrayList imArray = new ArrayList ( mc.Count ) ;
                for (int i = 0 ; i < MatchCollection.Count ; i++)
                {
                    Match match = MatchCollection[i];
                    int textPosition = match.Index - HalfItemWidth;
                    int j;
                    for (j = 0 ; textPosition < 0 ; textPosition++, j++)
                    // pad words positioned before halfListBox1CharWidth with spaces
                    {
                        item[j] = ' ';
                    }
                    for ( ; j < itemWidthPlus ; textPosition++, j++)
                    {
                        if (textPosition < textLength)
                        {
                            if (TextBoxText[textPosition] < ' ')
                            {
                                item[j] = ' ';
                            }
                            else item[j] = TextBoxText[textPosition];
                        }
                        else
                        // clear out 'item' char array for words near the end of the text
                        {
                            item[j] = ' ';
                        }
                    }
                    item[j] = (Char)0; // null delimit the string
                    string itemString = new string (item);
                    Item_FullRaw_StringArrayList[i] = itemString;
                    //ListBoxItemStringArray.StringArray.Add(itemString);
                }
                //itemFullStringArrayList = listBoxItemStringArray ;
                //IndexedMatchArray = imArray ;
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException (ex, true);
            }
        }

        public string
        SelectMatchTextAndFitIntoWidthOfBox (int i, int boxWidth_inCharacters) //int pixelWidthPerCharacter, int width)
        {
            try
            {
                string text;
                int textPosition;
                //double halfTheBoxWidth_inCharacters = boxWidth_inCharacters/2 ;

                Match match = MatchCollection[i];
                if ((SortFlag == true) ||
                    (ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true))
                {
                    text = (string)Item_FullRaw_StringArrayList[
                        ((IndexedMatch)(ListBoxItemStringArray.SortedIndexedMatch[i])).MatchIndex];
                }
                else
                {
                    text = (string)Item_FullRaw_StringArrayList[i];
                }

                if (ConcorDancer.Cdwf.SetListBoxViewCenteredOptionMenuItem.Checked == true)
                {
                    int matchLength = match.Length;
                    textPosition = HalfItemWidth - ((matchLength < boxWidth_inCharacters) ? ((boxWidth_inCharacters - matchLength) / 2) : 0);
                }
                else
                {
                    if (FindWordsAnyOrderWithinSentenceFlag == true)
                    {
                        textPosition = HalfItemWidth;
                    }
                    else if (FindWordsAnyPlaceWithinSentenceFlag == true)
                    {
                        textPosition = HalfItemWidth;
                    }
                    else // ConcorDancer.Cdwf.setListBoxViewCenteredOptionMenuItem.Checked == false 
                    {
                        textPosition = HalfItemWidth - (match.Index -
                            ConcorDancerSortComparer.GetWordsBackIndex (match.Index));
                    }
                }
                if (ConcorDancer.Cdwf.ShowListBoxHorizontalScrollBarMenuItem.Checked == true)
                {
                    textPosition -= HalfItemWidth;
                    if (textPosition < 0) textPosition = 0;
                    HorizontalScrollbar = true;
                }
                int length = ItemWidth - textPosition;
                return text.Substring (textPosition, length); // 1 : round up double
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException (ex, true);
            }
            return null;
        }

        ListBoxItemStringArray
        BuildListBoxItemStringArray ()
        // determine what collections to display and setup to display the strings appropriately (centered,etc.) 
        // for the characteristics of the listBox such as width
        {
            try
            {
                if (MatchCollection != null)
                {
                    if ((SortFlag == true) || (ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true))
                    // build IndexedMatchArray
                    {
                        ArrayList imArray = new ArrayList (MatchCollection.Count);
                        for (int i = 0 ; i < MatchCollection.Count ; i++)
                        {
                            IndexedMatch im = new IndexedMatch ();
                            im.Match = MatchCollection[i];
                            im.MatchIndex = i;
                            imArray.Add (im);
                            //imArray [i] = im ;
                        }
                        ConcorDancerSortComparer.WordsBack = WordsBack;
                        ConcorDancerSortComparer.Text = TextBoxText;
                        imArray.Sort (ConcorDancerSortComparer);
                        ListBoxItemStringArray = new ListBoxItemStringArray (new string[MatchCollection.Count], imArray);
                        //SortedIndexedMatchArrayList = imArray;
                        ListBoxItemStringArray.SortFlag = true;
                        ListBoxItemStringArray.WordsBack = WordsBack;
                    }
                    else ListBoxItemStringArray = new ListBoxItemStringArray (new string[MatchCollection.Count]);

                    for (int i = 0 ; i < MatchCollection.Count ; i++)
                    {
                        ListBoxItemStringArray.StringArray[i] = SelectMatchTextAndFitIntoWidthOfBox (i, Width / PixelWidthPerCharacter);
                    }
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException (ex, false); // has to come thru here on startup
            }
            //CurrentTextBox.ListBoxItemStringArray = ListBoxItemStringArray;
            return ListBoxItemStringArray;
        }

        public bool
        ListBoxCollection_QuestionEqual (ObjectCollection items, string[] sa)
        {
            int count = items.Count;
            // compare lbisa with Items if equal we don't need this function
            if ( ( count != 0 ) && ( count == sa.Length ) )
            {
                if ( ((string)items[count - 1] == sa[count - 1]) &&
                ((string)items[0] == sa[0])) return true;// determine if we need to do this function 
            }
            return false;
        }

        public void
        DisplayListBoxItemStringArray (ListBoxItemStringArray lbisa)
        {
            string[] sa = lbisa.StringArray;
            //if (sa.Length != 0)// we want to Clear items on length 0
            {
                if (ListBoxCollection_QuestionEqual (Items, sa) == false) // determine if we need to do this function 
                {
                    //bool savedGuard = ConcorDancer.Cdm.State.listBox1_SelectedIndexChangedGuard;
                    try
                    {
                        //ConcorDancer.Cdm.State.listBox1_SelectedIndexChangedGuard = true;
                        if (Font != ConcorDancer.ListBoxFont) Font = ConcorDancer.ListBoxFont; // an time expensive operation
                        BackColor = ConcorDancer.ListBoxBackColor;
                        ForeColor = ConcorDancer.ListBoxForeColor;
                        Items.Clear ();
                        Items.AddRange (sa);
                        ListBoxItemStringArray = lbisa;
                        //CurrentTextBox.ListBoxItemStringArray = lbisa;
                    }
                    catch (Exception ex)
                    {
                        ConcorDancer.Cdm.HandleException (ex, true);
                    }
                }
            }
            //ConcorDancer.Cdm.State.listBox1_SelectedIndexChangedGuard = savedGuard ;
        }
/*
        public void
        DisplayListBoxItemStringArray (StringFindElement sfe)
        // determine what collections to display and display them appropriately for the characteristics of the listBox such as width
        {
            ListBoxItemStringArray lbisa;
            if (sfe.ListBoxWidth == Width) lbisa = sfe.ListBoxItemStringArray;
            else lbisa = sfe.BuildListBoxItemStringArray ();
            bool sortFlag = sfe.SortedFlag;
            SortFlag = sortFlag;
            DisplayListBoxItemStringArray (lbisa);
            //_CalculateAndDisplayListBoxStringArray() ;
        }
*/
        public void
        BuildAndDisplayListBoxItemStringArray ()
        // determine what collections to display and display them appropriately for the characteristics of the listBox such as width
        {
            try
            {
                if (MatchCollection != null) // && MatchCollection.Count != 0) 
                {
                    ListBoxItemStringArray = BuildListBoxItemStringArray ();
                    DisplayListBoxItemStringArray (ListBoxItemStringArray);
                }
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException (ex, true);
            }
        }

        public Match
        DetermineMatch ()
        {
            Match match = null;
            try
            {
                int currentIndex = SelectedIndex;
                // determine the Match collection to use
                if ((SortFlag == true) ||
                    (ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked == true))
                {
                    match = ((IndexedMatch)ListBoxItemStringArray.SortedIndexedMatch[currentIndex]).Match;
                }
                else
                {
                    match = MatchCollection[currentIndex];
                }
                //return match;
            }
            catch (Exception ex)
            {
                ConcorDancer.Cdm.HandleException (ex, true);
            }
            return match;
        }

        public void
        CheckUpdateListBoxFromSelectedListIndex (int selectedIndex)
        {
            if (SelectedIndex != selectedIndex)
            {
                SelectedIndex = selectedIndex;
            }
        }
    }
}
