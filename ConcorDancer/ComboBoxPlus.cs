
using System;
using System.Collections;
using System.Windows.Forms;

namespace ConcorDancer
{
	public partial class
	ComboBoxPlus : ComboBox
	{
		public void
		RemoveStringFromItems ( string text )
		{
			try 
			{
				int i, j ;
				int count = Items.Count ;
				string [] listBoxItemStringArray = new string [ count ] ; //= new Array () ;
				for ( i = 0, j = 0 ; i < count ; i++ )
				{
					// retain only the items which are not the same as current Text
					// string dbg = (string) Items [ i ]  ;
					if ( text.CompareTo ( (string) Items [ i ] ) != 0 )
					// keep only the unequal strings to prevent duplicates
					{
						listBoxItemStringArray.SetValue ( Items [ i ], j++ ) ;
					}
				}
				// if ( j > 0 && j < count )
				// ? don't reset if no matching items :-> j == Item.Count
				{
					Items.Clear () ;
					for ( i = 0; i < j ; i++  ) Items.Add ( listBoxItemStringArray [ i ] ) ;
				}
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		ReplaceAddStringToItemsList ( string text )
		// item added to top of list deleting any other equal occurence in the list 
		{
			try
			{
				RemoveStringFromItems ( text ) ;
				Items.Insert ( 0, text ) ;
				//SelectedItem = (string) Items [ 0 ] ;
				//Text = (string) Items [ 0 ] ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		MoveCurrentItemToTopOfItemsList ()
		{
			ReplaceAddStringToItemsList ( Text ) ;
		}

		virtual public void 
		ClearTextBox ()
		{
			//Text = new string ( ' ', 0 ) ; // as opposed to "" which color selects the whole line
			Text = new string ( ' ', 0 ) ; // as opposed to "" which color selects the whole line
		}

		/*
		public
		ComboBoxPlus ()
		{
			ClearTextBox () ;
		}
		*/
	}

	public class
	ComboBoxPlusWithFindElementList : ComboBoxPlus
	{
		//int LastCharacterWidth ; //, lastSelectedIndex ;
        public DLList<DLLNode<StringFindElement>> ComboBoxHistoryFindElementList = new DLList<DLLNode<StringFindElement>>();
        //StringFindElement SelectedItemStringFindElement;
        //public int BoxWidth_inCharacters;
        public int PixelWidthPerCharacter;
        //public static int BoxWidthCorrection_inCharacters = 0 ;
        //public int WidthInCharacters ;


        public ComboBoxPlusWithFindElementList()
        {
            Init();
        }

        public void 
        Init ()
        {
            //Font = ConcorDancer.ComboBoxFont;
            System.Drawing.Size size = TextRenderer.MeasureText(ConcorDancer.MeasuredText, Font);
            PixelWidthPerCharacter = (int) (size.Width / ConcorDancer.MeasuredText.Length);// (384.0 / 53.0);	// fraction derived by experiment with fixed font size 9, same for 8 and 10
            //BoxWidth_inCharacters = Width / pixelWidthPerCharacter;
            //WidthInCharacters = (int)(Width / PixelWidthPerCharacter) + 1;
        }

        public StringFindElement
		GetSelectedItemStringFindElement ()
		{
            foreach (DLLNode<DLLNode<StringFindElement>> sfeN in ComboBoxHistoryFindElementList)
			{
                StringFindElement sfe = sfeN.Value.Value;
                //string debug = sfe.SelectMatchTextAndFitIntoWidthOfBox( Width / PixelWidthPerCharacter );
                if ((string)SelectedItem == sfe.SelectMatchTextAndFitIntoWidthOfBox(sfe.ListBoxSelectedIndex, Width / PixelWidthPerCharacter))
                //    if ((string)SelectedItem == sfe.Ctp.ListBox.SelectMatchTextAndFitIntoWidthOfBox(sfe.Ctp.ListBox.ListBoxSelectedIndex,
                // Width / PixelWidthPerCharacter)) //(string) sfe.ListBoxItemStringArray.StringArray[sfe.ListBoxSelectedIndex] )
                    //sfe.ConcorDancerTabPage.SelectTextAndFitIntoListBoxWidth ( sfe.listBoxSelectedIndex ) )
				{
					return sfe ;
				}
			}
			return ( StringFindElement ) null ;
		}

		public void
		SetText ( string text )
		{
			ConcorDancer.Cdm.State.FindHistoryComboBoxSelectedIndexChangedGuard = true ; // prevent calling SelectedIndexChangedPlus
			Text = text ;
			ConcorDancer.Cdm.State.FindHistoryComboBoxSelectedIndexChangedGuard = false ;
		}

		public void
		ReplaceAddStringFindElementToList ( StringFindElement sfe )
		{
			try
			{
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
                DLLNode<DLLNode<StringFindElement>> sfeNN = new DLLNode<DLLNode<StringFindElement>>(new DLLNode<StringFindElement>(sfe));
                //ReplaceAddStringToItemsList((string) sfe.ListBoxItemStringArray.StringArray[sfe.ListBoxSelectedIndex]);
                ReplaceAddStringToItemsList(sfe.SelectMatchTextAndFitIntoWidthOfBox(sfe.ListBoxSelectedIndex, Width / PixelWidthPerCharacter));
                ComboBoxHistoryFindElementList.AddIfNotAlreadyPresent(sfeNN);
                //SelectedItemStringFindElement = sfe;
				SetText ( (string)Items [ 0 ] ); 
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true );
			}
		}

		public void 
		ClearList () 
		{
            ComboBoxHistoryFindElementList = new DLList<DLLNode<StringFindElement>>();
			Items.Clear () ;
			ClearTextBox () ;
		}

		public void
		AdjustDisplay ()
		{
			try
			{
                /*
				//adjust any possibly selected item if it exists
				foreach ( ExtraNode en in comboBoxHistoryFindElementList )
				{
                    StringFindElement sfe = (StringFindElement)en.ContainingObject;
                    if ((string)SelectedItem ==
                        ConcorDancer.Cdm.CurrentConcorDancerTabPage.FitTextToComboBox(sfe.listBoxSelectedIndex))
					{
                        //SelectedItem = ConcorDancer.Cdm.CurrentConcorDancerTabPage.FitTextToComboBox(sfe.listBoxSelectedIndex);
						break ;
					}
				}
                */
				Items.Clear () ;
				// ... and the rest
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage;
                foreach (DLLNode<DLLNode<StringFindElement>> sfeN in ComboBoxHistoryFindElementList)
				{
                    StringFindElement sfe = sfeN.Value.Value;
                    //ReplaceAddStringToItemsList((string)sfe.ListBoxItemStringArray.StringArray[sfe.ListBoxSelectedIndex]);
                    ReplaceAddStringToItemsList(sfe.SelectMatchTextAndFitIntoWidthOfBox(sfe.ListBoxSelectedIndex, Width / PixelWidthPerCharacter));
                }
				//SelectedItem = (string) Items [ 0 ] ;
				//ConcorDancer.Cdm.State.findHistoryComboBoxSelectedIndexChangedGuard = true ;
				if ( Items.Count != 0 ) SetText ( (string) Items [ 0 ] ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}	
	}
}
