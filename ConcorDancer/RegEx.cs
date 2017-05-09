
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
	ConcorDancerTabPage
	{
		public int
		Factorial ( int x )
		{
			int result = x ;
			while ( x > 1 )
			{
				result *= ( --x) ;
			}
			return result ;
		}

		public ArrayList
		ParseString ( string text )
		{
            bool spaceDelimited = false, singleQuoteDelimited = false, quoteDelimited = false, withinAWord = false;
			int charCounter = 0, stringIndex = 0 ;
			ArrayList strings = new ArrayList () ;
			for ( int i = 0 ; i < text.Length ; i++ )
			{
				switch ( text [ i ] )
				{
					case ' ' :
					{
						if ( quoteDelimited )
						{
							if ( withinAWord == false )
							// count spaces as part of a quoted string
							{
								stringIndex = i ;
								withinAWord = true ;
							}
							charCounter ++ ;
						}
						else
						{
							if ( spaceDelimited )
							{
								if ( charCounter > 0 )
								{
									strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
									charCounter = 0 ;
									stringIndex = 0 ;
								}
							}
							withinAWord = false ;
							spaceDelimited = true ;
						}
						continue ;
					}
					case '\'' :
					{
						if ( spaceDelimited )
						{
							if ( charCounter > 0 )
							{
								strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
								charCounter = 0 ;
								stringIndex = 0 ;
							}
							spaceDelimited = false ;
                            singleQuoteDelimited = true;
						}
						else
						{
                            if ( singleQuoteDelimited )
							{
								if ( charCounter > 0 )
								{
									strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
									charCounter = 0 ;
									stringIndex = 0 ;
								}
                                singleQuoteDelimited = false; // second quote always turns off quoteDelimited
							}
                            else if ( !quoteDelimited ) singleQuoteDelimited = true;
						}
                        if (!quoteDelimited) withinAWord = false;
                        charCounter++;
                        continue;
					}
					case '"' :
					{
						if ( spaceDelimited )
						{
							if ( charCounter > 0 )
							{
								strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
								charCounter = 0 ;
								stringIndex = 0 ;
							}
							spaceDelimited = false ;
							quoteDelimited = true ;
						}
						else
						{
							if ( quoteDelimited )
							{
								if ( charCounter > 0 )
								{
									strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
									charCounter = 0 ;
									stringIndex = 0 ;
								}
								quoteDelimited = false ; // second quote always turns off quoteDelimited
							}
							else if ( ! singleQuoteDelimited ) quoteDelimited = true ;
						}
                        if (!singleQuoteDelimited) withinAWord = false;
						continue ;
					}
					default :
					{
						if ( !quoteDelimited )
						{
							spaceDelimited = true ; //
						}
						if ( withinAWord == false )
						{
							stringIndex = i ;
							withinAWord = true ;
						}
						charCounter ++ ;
						continue ;
					}
				}
			}
			if ( charCounter > 0 )
			{
				strings.Add ( text.Substring ( stringIndex, charCounter ) ) ;
			}
			return strings ;
		}

        /* ConstructRegEx
         * minor simplifications with version 2.5.4.2 
         */
		public Regex
		ConstructRegEx ( )
		{
			try
			{
                string spacer;
 				string anyInSentenceChar = "[^.!?]" ;
				//string anyInWordChar = "[a-z]" ;
				string whiteSpaceOrInSentencePunctuation = "[^a-zA-Z_0-9.!?]" ;
				//string endOfSentenceChar = "[.!?]" ;
				if ( ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked == true )
				{
					spacer = anyInSentenceChar + "*" ; // allow word extensions
				}
				else
				{
					spacer = whiteSpaceOrInSentencePunctuation + "+" ; // WholeWord
				}
                //char [] spacerToCharArray = spacer.ToCharArray();
				ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				ArrayList strings = ParseString ( ListBox.RequestedWord ) ;
				int length = strings.Count ;
				/*
				allocate an array of strings : size = factorial of length
				distribute the strings with all combinations in the array
				*/
				int factorial = Factorial ( length ) ;
				if ( length == 1 )
				{
                    AppendRegExpString = (string)strings[0];
				}
				else if ( length == 2 )
				{
					AppendRegExpString = "(" ;
					int factorialSize = factorial * length ;
					for ( int i = 0 ; i < factorialSize ; i++ )
					{
						if ( ( i > 0 ) && ( i % length == 0 ) )
						{
                            AppendRegExpString = AppendRegExpString.Substring ( 0, AppendRegExpString.Length - 7 );
                            AppendRegExpString += ")|(";
						}
						AppendRegExpString += strings [ ConcorDancer.TwoStringsTable [ i ] ] + spacer ;
					}
                    //AppendRegExpString += (")" + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                    AppendRegExpString = AppendRegExpString.Substring(0, AppendRegExpString.Length - 7);
                    AppendRegExpString += ")"; // + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                }
				else if ( length == 3 )
				{
					AppendRegExpString = "(" ;
					int factorialSize = factorial * length ;
					for ( int i = 0 ; i < factorialSize ; i++ )
					{
						if ( ( i > 0 ) && ( i % length == 0 ) )
						{
                            AppendRegExpString = AppendRegExpString.Substring(0, AppendRegExpString.Length - 7);
                            AppendRegExpString += ")|(";
						}
						AppendRegExpString += strings [ ConcorDancer.ThreeStringsTable [ i ] ] + spacer ;
					}
                    //AppendRegExpString += (")" + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                    AppendRegExpString = AppendRegExpString.Substring(0, AppendRegExpString.Length - 7);
                    AppendRegExpString += ")"; // + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                }
				else if ( length == 4 )
				{
					AppendRegExpString = "(" ;
					int factorialSize = factorial * length ;
					for ( int i = 0 ; i < factorialSize ; i++ )
					{
						if ( ( i > 0 ) && ( i % length == 0 ) )
						{
                            AppendRegExpString = AppendRegExpString.Substring(0, AppendRegExpString.Length - 7);
                            AppendRegExpString += ")|(";
						}
						AppendRegExpString += strings [ ConcorDancer.FourStringsTable [ i ] ] + spacer ;
					}
                    //AppendRegExpString += (")" + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                    AppendRegExpString = AppendRegExpString.Substring(0, AppendRegExpString.Length - 7);
                    AppendRegExpString += ")"; // + (ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked ? endOfSentenceChar : ""));
                }
				else
				// anything over 4 words
				{
					return (Regex) null ;
				}
                //AppendRegExpString.Trim(spacer.ToCharArray());
				Regex regExp = new Regex ( AppendRegExpString, RegularExpressionOptions  ) ;
				#if debug2
				using ( StreamWriter dbgSw = File.AppendText ( "debug.txt" ) )
				{
					dbgSw.Write ( AppendRegExpString ) ;
				}
				#endif
				return regExp ;
			}
			catch ( Exception e )
			{
				ConcorDancer.Cdm.HandleException ( e, true ) ;
				return (Regex) null ;
			}
		}

		Regex
		BuildRegExp ()
		{
			Regex regExp ;
			string anyInSentenceChar = "[^.!?]" ;
			string whiteSpaceOrInSentencePunctuation = "[^a-zA-Z_0-9.!?]" ;
			string anyInWordChar = "[a-zA-Z_0-9]" ;
			string searchString = "", spacer ;
			//ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
			ArrayList al = ParseString ( ListBox.RequestedWord ) ;
			if ( ConcorDancer.Cdwf.UseRegularExpressionsMenuItem.Checked == true )
			{
				if ( ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked == true )
				{
					spacer = anyInSentenceChar + "*" ; // allow word extensions including whitespace
				}
				else
				{
					spacer = anyInWordChar + "*" + whiteSpaceOrInSentencePunctuation + "+" ; // + : 1 or more
				}
				//searchString = "" ;
				for ( int i = 0 ; i < al.Count ; i ++ )
				{
					if ( i == 0 ) searchString = (string) ( al [ i ] ) ;
					else searchString += spacer + (string) ( al [ i ] )  ;
				}
			}
			else // if ( ConcorDancer.Cdwf.useRegularExpressionsMenuItem.Checked == false )
			// give user what he probably expects - the words delimited by anything but another character within one sentence
			{
				//searchString = "" ;
				for ( int i = 0 ; i < al.Count ; i ++ )
				{
					if ( i == 0 ) searchString = whiteSpaceOrInSentencePunctuation +
						(string) ( al [ i ] ) + whiteSpaceOrInSentencePunctuation ;
					else searchString += (string) ( al [ i ] ) + whiteSpaceOrInSentencePunctuation ;
				}
				//regExp = new Regex ( searchString, RegularExpressionOptions ) ;
			}
			regExp = new Regex ( searchString, RegularExpressionOptions ) ;
			return regExp ;
		}

		public void
		_DisplayRequestedRegExpWordList ( )
		{
			try
			{
				//ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
				Regex regExp ;
				if ( ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked == true )
				{
					regExp = ConstructRegEx () ;
					if ( regExp == null )
					{
						regExp = BuildRegExp () ;
					}
				}
				else
				{
					regExp = BuildRegExp () ;
				}
				ListBox.MatchCollection = regExp.Matches ( CurrentTextBox.Text ) ;
				ListBox.SortFlag = ConcorDancer.Cdwf.GlobalSortOptionMenuItem.Checked ;
				ListBox.WordsBack = ConcorDancer.Cdm.WordsBack ;
				ListBox.FindWordsAnyOrderWithinSentenceFlag = 
					ConcorDancer.Cdwf.FindWordsAnyOrderMenuItem.Checked ;
				ListBox.FindWordsAnyPlaceWithinSentenceFlag = 
					ConcorDancer.Cdwf.FindWordsAnyPlaceMenuItem.Checked ;
				ListBox.ListBoxSelectedIndex = -1 ;
                ListBox.BuildListBoxItem_FullRaw_StringArray();
                ListBox.BuildAndDisplayListBoxItemStringArray();
				ListBox.ClearSelected () ;
				ConcorDancer.Cdwf.SetCounterLabel () ;
			}
			catch ( Exception e )
			{
				ConcorDancer.Cdm.HandleException ( e, true ) ;
			}
		}
	}
}
