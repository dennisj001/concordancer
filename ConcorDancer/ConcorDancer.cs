
using System;
using System.Windows.Forms;
using System.Text ;
using System.Drawing ;

namespace ConcorDancer
{
    public class
    ExceptionReportWindow : Form
    {
        public static TextBox Etb;
        int EtbFontHeight ;
        Graphics EtbG;
        public
        ExceptionReportWindow()
        { 
            Etb = new TextBox();
            Etb.Dock = DockStyle.Fill;
            Etb.AutoSize = true;
            Etb.Multiline = true;
            Etb.Height = Height;
            //Etb.Width = Width;
            Etb.ScrollBars = ScrollBars.Both ;
            Controls.Add(Etb);
            EtbG = Etb.CreateGraphics();
            EtbFontHeight = (int) EtbG.MeasureString("H", Etb.Font).Height + 1 ;
            Height = EtbFontHeight + 1 ;
        }
        public void
        ReportException(string exceptionString)
		{
            int textWidth = (int)EtbG.MeasureString (exceptionString, Etb.Font).Width;
            int textHeight = (int)EtbG.MeasureString (exceptionString, Etb.Font).Height ;
            SuspendLayout ();
            if (Width < textWidth) Width = textWidth + 1 ;
            if (Etb.Width < textWidth) Etb.Width = textWidth + 1;
            //Height += ( textHeight + EtbFontHeight ) ;
            //Etb.Height = Height ;
            Etb.Text += exceptionString ;
            ResumeLayout(false);
            PerformLayout();
            Refresh();
        }
    }
    public class
    ConcorDancer
    {
        [STAThread]
		static void 
		Main ()
		{
            //do
            {
                try
                {
                    /*
                    //using System.Security.Permissions;
                    FileIOPermission f = new FileIOPermission(PermissionState.None);
                    f.AllFiles = FileIOPermissionAccess.Read;
                    try
                    {
                        f.Demand();
                    }
                    catch (Exception e )
                    {
                        ConcorDancer.Cdm.HandleException(e, true);
                    }
                    */

                    Cdm = new ConcorDancerModel();
                    Cdwf = new ConcorDancerWinForm();
                    //ConcorDancer.Cdwf.Startup () ;
                    Cdwf.TabControl.Font = ConcorDancer.TabControlFont;
                    Cdwf.InitializeTopRowBoxes();
                    Cdwf.Refresh();
                    Application.Run(Cdwf);
                    //ConcorDancer.Cdwf.ShutDown () ;
                }
                catch (Exception e)
                {
                    ConcorDancer.Cdm.HandleException(e, true);
                }
            }
            //while (true);
		}

        public static void
        Init ()
        {
            TextBoxPixelHeightPerCharacter = (double)ConcorDancer.TextBoxFont.Height;
            ButtonHeight = ConcorDancer.ListBoxFont.Height + ( 2 * SpaceBetweenForwardButtonAndBackButton ) ; 
		    ComboBoxHeight = ButtonHeight - 1;
		    YPositionOfBottomOfComboBox = YPositionOfComboBox + ComboBoxHeight ;
            ComboBoxFont = ListBoxFont;
	   }

        public static void
        HandleException(Exception e, bool reportFlag)
        {
            if ((reportFlag == true) && (ConcorDancer.Cdwf.ShowVersionMenuItem.Checked == true)) // for now use ShowVersionMenuItem as a flag for this exception report window
            {
                if (Erw == null)
                {
                    Erw = new ExceptionReportWindow();
                    Erw.Show();
                }
                Erw.ReportException( e.ToString () + "\n"); //"Error" ;
                //Error ( e.Message ) ; //"Error" ;
            }
        }

        public static ExceptionReportWindow Erw =  null ;
        //public static ConcorDancerCore Core = new ConcorDancerCore () ;
		public static ConcorDancerModel Cdm ; //= new ConcorDancerModel ;
		public static ConcorDancerWinForm Cdwf ; //= new ConcorDancerWinForm () ;
		// Preferences
		public static Font TextBoxFont = new Font ("Microsoft Sans Serif", 10F);
		public static Font ListBoxFont = new System.Drawing.Font ("Courier New", 9F) ; // our fitted, calculated Font
        public static Font ComboBoxFont = ListBoxFont;
		public static Font DataGridViewDirectoryFont = new System.Drawing.Font ("Microsoft Sans Serif", 8F) ; 
		public static Font DataGridViewFileFont = new System.Drawing.Font ("Microsoft Sans Serif", 8F) ; 
		public static Color FileForeColor = Color.FromName ( "Black" ) ;
		public static Color DirectoryForeColor = Color.FromName ( "Teal" ) ;
		public static Color FileListBackColor = Color.FromName ( "LightGray" ) ;
		//public static  Color dataGridViewDirectoryColor = Color.FromName ( "White" ) ;
		public static Font TabControlFont = new System.Drawing.Font ("Microsoft Sans Serif", 7F) ; 
		public static Font TreeViewFont = new Font("Microsoft Sans Serif", 9F) ; 
		public static Color TextBoxBackColor = Color.FromName ( "Ivory" ) ;
		public static Color TextBoxForeColor = Color.FromName ( "Black" ) ;
		public static Color ListBoxBackColor = Color.FromName ( "LightGray" ) ;
		public static Color ListBoxForeColor = Color.FromName ( "Black" ) ;
		public static Color DirectoryTreeBackColor = Color.FromName ( "LightGray" ) ;
		public static Color TreeViewForeColor = Color.FromName ( "Teal" ) ;
		public static TabAppearance PreferredTabAppearance = TabAppearance.Normal ;
		// ConcorDancer
		public static ConcorDancerTextBox CurrentTextBox ;
		public static DataGridView CurrentDataGridView ;
		public static ConcorDancerListBox CurrentListBox ;
		public static TreeView CurrentTreeView ;

		public static string 
		MakeFilenameFromFullPathName ( string fullPathFilename )
		{
			string filename = fullPathFilename.Substring ( fullPathFilename.LastIndexOf ( '\\') + 1 ) ;
			return filename ;
		}

        // constants
        public static String MeasuredText = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890.!? ";
        //public const double PixelWidthPerCharacter = size.Width / 67.0;// (384.0 / 53.0);	// fraction derived by experiment with fixed font size 9, same for 8 and 10
        //public const double PixelWidthPerProportionalCharacter = 10;
		//public static double TextBoxPixelWidthPerCharacter =  (double) ( ConcorDancer.TextBoxFont.Height - 2 ) ;
        public static double TextBoxPixelHeightPerCharacter = (double)ConcorDancer.TextBoxFont.Height;
        //public double TextBoxPixelHeightPerCharacter = 13.0 ;
		//public double TextBoxPixelWidthPerCharacter = 11.0 ;	

		public const int StartWidth = 800 ;
		public const int StartHeight = 480 ;
		public const int WindowTitleHeight = 7 ;
		//public const int MenuHeight = 14 ;
        public static int ButtonHeight = ConcorDancer.ListBoxFont.Height + ( 2 * SpaceBetweenForwardButtonAndBackButton ) ; 
		public const int SpaceBetweenFormAndTabControl = 4 ;
		//public const int LeftAndTopSpaceBetweenTabControlAndTabPage = 0 ;
		public const int SpaceBetweenComboBoxAndTabControl = 4 ;
		public const int SpaceBetweenTabPageAndSplitContainer = 4 ;
		public const int SplitContainerPadding = 4 ;
		public const int ScrollBarWidth = 12 ;
		public const int SpaceBetweenForwardButtonAndBackButton = 4 ;
		public const int SplitterWidth = 4 ;
		public static int ComboBoxHeight = ButtonHeight - 1;
		public const int YPositionOfComboBox = 2 ;
		
		public static int YPositionOfBottomOfComboBox = YPositionOfComboBox + 
			ComboBoxHeight ;
		public const int TabControlStartWidth = StartWidth - 
			( 2 * SpaceBetweenFormAndTabControl ) ; 
		public static int TabControlStartHeight = StartHeight 
			- YPositionOfBottomOfComboBox  
			- SpaceBetweenComboBoxAndTabControl			
			- SpaceBetweenFormAndTabControl ;
		public const int SpaceBetweenListBoxAndTextBox = SplitterWidth ; 

		public static int TabPageStartHeight = TabControlStartHeight ; 
			//- ( 2 *	LeftAndTopSpaceBetweenTabControlAndTabPage ) ;
		public const int TabPageStartWidth = TabControlStartWidth ; 
			//- 2 * ( LeftAndTopSpaceBetweenTabControlAndTabPage ) ;
		//public const int ListBoxStartHeight = TabPageStartHeight ; 
			//- 2 * ( SpaceBetweenTabPageAndSplitContainer ) ;
		public const int ListBoxStartWidth = ( TabPageStartWidth - SplitterWidth ) / 2 
			- SplitContainerPadding ;
		public const int MilliSecondDelayForLayout = 300 ;
		public const int MilliSecondDelayForResize = 300 ;
		public  static double [] 
		CharWidths = new double [ 128 ]
		{
			// 0 - 31
			1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */,
			1.0 /*  */, 0.863 /* \t */, 0.863 /* \n */, 1.0 /*  */,
				1.0 /*  */, 0.863 /* \n */, 1.0 /*  */, 1.0 /*  */,
			1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */,
			1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */,
			// 32 - 39
			0.863 /* ' '  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */,
			// 40 - 47
			1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /* , */, 1.0 /*  */, 1.0 /* . */, 1.0 /*  */,
			// 48 - 55
			1.0 /*  */, 1.81 /* 1 */, 2.09 /* 2 */, 2.09 /* 3 */,
				2.09 /* 4 */, 2.09 /* 5 */, 2.09 /* 6 */, 2.09 /* 7 */,
			// 56- 63
			2.09 /* 8 */, 2.09 /* 9 */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 2.08 /* ? */,
			// 64 - 71
			1.0 /*  */, 2.09 /* A */, 2.35 /* B */, 2.35 /* C */,
				2.61 /* D */, 2.35 /* E */, 2.08 /* F */, 2.61 /* G */,
			// 72 - 79
			2.61 /* H */, 1.0 /* I */, 1.81 /* J */, 2.35 /* K */,
				2.08 /* L */, 3.5 /* M */, 2.61 /* N */, 2.61 /* O */,
			// 80 - 87
			2.35 /* P */, 2.61 /* Q */, 2.61 /* R */, 2.35 /* S */,
				2.08 /* T */, 2.61 /* U */, 2.08 /* V */, 3.15 /* W */,
			// 88 - 95
			2.35 /* X */, 2.08 /* Y */, 2.08 /* Z */, 1.0 /*  */, 1.0 /*  */, 1.0 /* */, 1.0 /*  */, 1.0 /*  */,
			// 96 - 103
			1.0 /*  */, 2.08 /* a */, 2.08 /* b */, 1.8 /* c */,
				2.08 /* d */, 2.08 /* e */, 1.0 /* f */, 2.08 /* g */,
			// 104 - 111
			2.08 /* h */, 1.0 /* i */, 1.0 /* j */, 1.8 /* k */,
				1.0 /* l */, 3.15 /* m */, 2.08 /* n */, 2.08 /* o */,
			// 112 - 119
			2.08 /* p */, 2.08 /* q */, 1.28 /* r */, 1.81 /* s */,
				1.0 /* t */, 2.08 /* u */, 1.53 /* v */, 2.61 /* w */,
			// 120 - 127
			1.8 /* x */, 2.08 /* y */, 1.8 /* z */, 1.0 /*  */,
				1.0 /*  */, 1.0 /*  */, 1.0 /*  */, 1.0 /*  */,
		} ;

		public static int [] 
		FourStringsTable = new int [ 96 ]
		{
			0, 1, 2, 3,
			0, 1, 3, 2,
			0, 2, 1, 3,
			0, 2, 3, 1,
			0, 3, 1, 2,
			0, 3, 2, 1,
			1, 0, 2, 3,
			1, 0, 3, 2,
			1, 2, 0, 3,
			1, 2, 3, 0,
			1, 3, 0, 2,
			1, 3, 2, 0,
			2, 0, 1, 3,
			2, 0, 3, 1,
			2, 1, 0, 3,
			2, 1, 3, 0,
			2, 3, 0, 1,
			2, 3, 1, 0,
			3, 0, 1, 2,
			3, 0, 2, 1,
			3, 1, 0, 2,
			3, 1, 2, 0,
			3, 2, 1, 0,
			3, 2, 0, 1,
		} ;

		public static int [] 
		ThreeStringsTable = new int [ 18 ]
		{
			0, 1, 2,
			0, 2, 1,
			1, 0, 2,
			1, 2, 0,
			2, 0, 1,
			2, 1, 0
		} ;

		public static int [] 
		TwoStringsTable = new int [ 4 ]
		{
			0, 1,
			1, 0
		} ;
	}
}
