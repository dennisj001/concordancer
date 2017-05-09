
// #define mono

using System;
using System.Collections;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Text;
using System.Globalization;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Resources;

namespace ConcorDancer
{
    public partial class
    ConcorDancerWinForm : Form
    {
        public ConcorDancerTabControl TabControl ;
        public ComboBoxPlus ComboBoxForFind;
        public ComboBoxPlusWithFindElementList ComboBoxForHistory ;
        public Label CounterLabel ;
        public Button ForwardButton ;
        public Button FindButton ;
        public Button BackButton;

        //public FontDialog fontDialog1 ;
        public FolderBrowserDialog FolderBrowserDialog ;
        public OpenFileDialog OpenFileDialog ;
        public SaveFileDialog SaveFileDialog ;

        public MainMenu MenuStrip ;

        public MenuItem NewFileMenuItem ;
        public MenuItem FileMenuItem ;
        public MenuItem OpenFilesMenuItem ;
        public MenuItem SaveMenuItem ;
        public MenuItem SaveAsMenuItem ;
        public MenuItem CloseFileMenuItem ;
        public MenuItem OpenFolderMenuItem ;
        //public MenuItem appendTextMenuItem ;
        public MenuItem OpenAppendFileMenuItem ;
        public MenuItem SaveAppendFileMenuItem ;
        public MenuItem saveAsAppendToFileMenuItem ;
        public MenuItem OptionsMenuItem ;
        public MenuItem SetTextBoxFontMenuItem ;
        public MenuItem SetTreeViewFontMenuItem ;
        public MenuItem SetFontMenuItem ;
        public MenuItem SetBackgroundColorMenuItem ;
        public MenuItem SetListBoxFontMenuItem ;
        public MenuItem SetFileListFontMenuItem ;
        public MenuItem SetTextBoxBackgroundColorMenuItem ;
        public MenuItem SetDirectoryTreeBackgroundColorMenuItem ;
        public MenuItem SetListBoxBackgroundColorMenuItem ;
        public MenuItem SetFileListBackgroundColorMenuItem ;
        public MenuItem WordWrapMenuItem ;
        public MenuItem ReadOnlyMenuItem ;
        public MenuItem FindWordsAnyOrderMenuItem ;
        public MenuItem FindWordsAnyPlaceMenuItem ;
        public MenuItem CaseSensitiveFindsMenuItem ;
        public MenuItem RemoveHtmlTagsOptionMenuItem ;
        public MenuItem RemoveHtmlAnchorsOptionMenuItem ;
        public MenuItem RestoreStateMenuItem ;
        public MenuItem GlobalSortOptionMenuItem ;
        public MenuItem SortWordsBackMenuItem ;
        public MenuItem Start0WordsBackMenuItem ;
        public MenuItem Start1WordBackMenuItem ;
        public MenuItem Start2WordsBackMenuItem ;
        public MenuItem Start3WordsBackMenuItem ;
        public MenuItem Start4WordsBackMenuItem ;
        public MenuItem Start5WordsBackMenuItem ;
        public MenuItem KeepListFocusMenuItem ;
        public MenuItem ShiftFocusToTextMenuItem ;
        public MenuItem UseRegularExpressionsMenuItem ;
        public MenuItem RecurseFoldersOptionMenuItem ;
        public MenuItem SetListBoxViewCenteredOptionMenuItem ;
        public MenuItem ShowVersionMenuItem ;
        public MenuItem ShowListBoxHorizontalScrollBarMenuItem ;
        public MenuItem MultiColumnFileListMenuItem ;

        public MenuItem functionsMenuItem ;
        public MenuItem refreshDisplayMenuItem ;
        public MenuItem refreshListMenuItem ;
        public MenuItem sortFunctionMenuItem ;
        //public MenuItem deleteCurrentFindListElementMenuItem  ;
        //public MenuItem deleteComboBoxItemMenuItem ;
        public MenuItem insertFileNamesOnJoinOptionMenuItem ;
        public MenuItem contextMenuItem ;
        private IContainer components;
        private MenuItem menuItem1;
        public MenuItem newFileManagerMenuItem ;
#if ! mono
        public ResourceManager ResourceManager;
#endif

        public
        ConcorDancerWinForm ()
        {
            int minimumWidth, minimumHeight, minimumTextBoxWidth;
            int lastLeft, lastRight;

            InitializeComponent ();
            //CalculateLocationsAndHeightsForBoxes(true);
            KeyUp += new KeyEventHandler (WinForm_KeyUp);
            Resize += new EventHandler (ConcorDancerWinForm_Resize);

            minimumTextBoxWidth = FindButton.Width + CounterLabel.Width + BackButton.Width
                + ForwardButton.Width;
            minimumWidth = 2 * minimumTextBoxWidth - 100; //+ 124 ; // 100 for listBox 8 for 3 dividers
            minimumHeight = 120;
            MinimumSize = new System.Drawing.Size (minimumWidth, minimumHeight);
            lastLeft = Left;
            lastRight = Right;
            //Opacity = 0.9 ;
            StartPosition = FormStartPosition.CenterScreen;
            KeyPreview = true;
            AddContextMenuItems (contextMenuItem);
#if ! mono
            ResourceManager ResourceManager = new ResourceManager ("ConcorDancer.Properties.Resources", GetType ().Assembly);
#endif
            Text = String.Format ("{0}", ConcorDancer.Cdm.ProgramName);

        }

        void InitializeComponent ()
		{
            this.components = new System.ComponentModel.Container();
            this.CounterLabel = new System.Windows.Forms.Label();
            this.FindButton = new System.Windows.Forms.Button();
            this.FolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.OpenFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.MenuStrip = new System.Windows.Forms.MainMenu(this.components);
            this.FileMenuItem = new System.Windows.Forms.MenuItem();
            this.newFileManagerMenuItem = new System.Windows.Forms.MenuItem();
            this.NewFileMenuItem = new System.Windows.Forms.MenuItem();
            this.OpenFilesMenuItem = new System.Windows.Forms.MenuItem();
            this.OpenFolderMenuItem = new System.Windows.Forms.MenuItem();
            this.SaveMenuItem = new System.Windows.Forms.MenuItem();
            this.SaveAsMenuItem = new System.Windows.Forms.MenuItem();
            this.CloseFileMenuItem = new System.Windows.Forms.MenuItem();
            this.OpenAppendFileMenuItem = new System.Windows.Forms.MenuItem();
            this.SaveAppendFileMenuItem = new System.Windows.Forms.MenuItem();
            this.saveAsAppendToFileMenuItem = new System.Windows.Forms.MenuItem();
            this.OptionsMenuItem = new System.Windows.Forms.MenuItem();
            this.FindWordsAnyOrderMenuItem = new System.Windows.Forms.MenuItem();
            this.FindWordsAnyPlaceMenuItem = new System.Windows.Forms.MenuItem();
            this.UseRegularExpressionsMenuItem = new System.Windows.Forms.MenuItem();
            this.CaseSensitiveFindsMenuItem = new System.Windows.Forms.MenuItem();
            this.MultiColumnFileListMenuItem = new System.Windows.Forms.MenuItem();
            this.ShowListBoxHorizontalScrollBarMenuItem = new System.Windows.Forms.MenuItem();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.WordWrapMenuItem = new System.Windows.Forms.MenuItem();
            this.ReadOnlyMenuItem = new System.Windows.Forms.MenuItem();
            this.KeepListFocusMenuItem = new System.Windows.Forms.MenuItem();
            this.ShiftFocusToTextMenuItem = new System.Windows.Forms.MenuItem();
            this.RemoveHtmlTagsOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.RemoveHtmlAnchorsOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.RecurseFoldersOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.insertFileNamesOnJoinOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.SetListBoxViewCenteredOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.ShowVersionMenuItem = new System.Windows.Forms.MenuItem();
            this.contextMenuItem = new System.Windows.Forms.MenuItem();
            this.functionsMenuItem = new System.Windows.Forms.MenuItem();
            this.SetFontMenuItem = new System.Windows.Forms.MenuItem();
            this.SetTextBoxFontMenuItem = new System.Windows.Forms.MenuItem();
            this.SetListBoxFontMenuItem = new System.Windows.Forms.MenuItem();
            this.SetFileListFontMenuItem = new System.Windows.Forms.MenuItem();
            this.SetTreeViewFontMenuItem = new System.Windows.Forms.MenuItem();
            this.SetBackgroundColorMenuItem = new System.Windows.Forms.MenuItem();
            this.SetDirectoryTreeBackgroundColorMenuItem = new System.Windows.Forms.MenuItem();
            this.SetTextBoxBackgroundColorMenuItem = new System.Windows.Forms.MenuItem();
            this.SetFileListBackgroundColorMenuItem = new System.Windows.Forms.MenuItem();
            this.SetListBoxBackgroundColorMenuItem = new System.Windows.Forms.MenuItem();
            this.refreshDisplayMenuItem = new System.Windows.Forms.MenuItem();
            this.refreshListMenuItem = new System.Windows.Forms.MenuItem();
            this.SortWordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start0WordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start1WordBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start2WordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start3WordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start4WordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.Start5WordsBackMenuItem = new System.Windows.Forms.MenuItem();
            this.sortFunctionMenuItem = new System.Windows.Forms.MenuItem();
            this.GlobalSortOptionMenuItem = new System.Windows.Forms.MenuItem();
            this.RestoreStateMenuItem = new System.Windows.Forms.MenuItem();
            this.BackButton = new System.Windows.Forms.Button();
            this.ForwardButton = new System.Windows.Forms.Button();
            this.ComboBoxForFind = new ComboBoxPlus();
            this.ComboBoxForHistory = new ComboBoxPlusWithFindElementList();
            this.TabControl = new ConcorDancerTabControl();
            this.SuspendLayout();
            // 
            // CounterLabel
            // 
            this.CounterLabel.AutoSize = true;
            this.CounterLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.CounterLabel.Location = new System.Drawing.Point(277, 3);
            this.CounterLabel.Name = "CounterLabel";
            this.CounterLabel.Size = new System.Drawing.Size(45, 17);
            this.CounterLabel.TabIndex = 5;
            this.CounterLabel.Text = "Count";
            this.CounterLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // FindButton
            // 
            this.FindButton.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.FindButton.Location = new System.Drawing.Point(328, 1);
            this.FindButton.Name = "FindButton";
            this.FindButton.Size = new System.Drawing.Size(40, 23);
            this.FindButton.TabIndex = 1;
            this.FindButton.Text = "Find";
            this.FindButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.FindButton.Click += new System.EventHandler(this.FindListButton_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FileMenuItem,
            this.OptionsMenuItem,
            this.contextMenuItem,
            this.functionsMenuItem});
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.Index = 0;
            this.FileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.newFileManagerMenuItem,
            this.NewFileMenuItem,
            this.OpenFilesMenuItem,
            this.OpenFolderMenuItem,
            this.SaveMenuItem,
            this.SaveAsMenuItem,
            this.CloseFileMenuItem,
            this.OpenAppendFileMenuItem,
            this.SaveAppendFileMenuItem,
            this.saveAsAppendToFileMenuItem});
            this.FileMenuItem.Text = "&File";
            // 
            // newFileManagerMenuItem
            // 
            this.newFileManagerMenuItem.Index = 0;
            this.newFileManagerMenuItem.Text = "&File Manager                      F5";
            this.newFileManagerMenuItem.Click += new System.EventHandler(this.NewFileManagerMenuItem_Click);
            // 
            // NewFileMenuItem
            // 
            this.NewFileMenuItem.Index = 1;
            this.NewFileMenuItem.Text = "&New File";
            this.NewFileMenuItem.Click += new System.EventHandler(this.NewFileMenuItem_Click);
            // 
            // OpenFilesMenuItem
            // 
            this.OpenFilesMenuItem.Index = 2;
            this.OpenFilesMenuItem.Text = "&Open File";
            this.OpenFilesMenuItem.Click += new System.EventHandler(this.OpenFilesMenuItem_Click);
            // 
            // OpenFolderMenuItem
            // 
            this.OpenFolderMenuItem.Index = 3;
            this.OpenFolderMenuItem.Text = "O&pen - Join Files - Including SubDirectories Recursively";
            this.OpenFolderMenuItem.Click += new System.EventHandler(this.OpenFolderMenuItem_Click);
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Index = 4;
            this.SaveMenuItem.Text = "&Save";
            this.SaveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // SaveAsMenuItem
            // 
            this.SaveAsMenuItem.Index = 5;
            this.SaveAsMenuItem.Text = "&Save As";
            this.SaveAsMenuItem.Click += new System.EventHandler(this.SaveAsMenuItem_Click);
            // 
            // CloseFileMenuItem
            // 
            this.CloseFileMenuItem.Index = 6;
            this.CloseFileMenuItem.Text = "&Close";
            this.CloseFileMenuItem.Click += new System.EventHandler(this.CloseFileMenuItem_Click);
            // 
            // OpenAppendFileMenuItem
            // 
            this.OpenAppendFileMenuItem.Index = 7;
            this.OpenAppendFileMenuItem.Text = "Open &Append File";
            this.OpenAppendFileMenuItem.Click += new System.EventHandler(this.OpenAppendfileMenuFileItem_Click);
            // 
            // SaveAppendFileMenuItem
            // 
            this.SaveAppendFileMenuItem.Index = 8;
            this.SaveAppendFileMenuItem.Text = "Save App&end File";
            this.SaveAppendFileMenuItem.Click += new System.EventHandler(this.SaveAppendfileMenuFileItem_Click);
            // 
            // saveAsAppendToFileMenuItem
            // 
            this.saveAsAppendToFileMenuItem.Index = 9;
            this.saveAsAppendToFileMenuItem.Text = "Save Appe&nd File As";
            this.saveAsAppendToFileMenuItem.Click += new System.EventHandler(this.SaveAsAppendFileMenuFileItem_Click);
            // 
            // OptionsMenuItem
            // 
            this.OptionsMenuItem.Index = 1;
            this.OptionsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.FindWordsAnyOrderMenuItem,
            this.FindWordsAnyPlaceMenuItem,
            this.UseRegularExpressionsMenuItem,
            this.CaseSensitiveFindsMenuItem,
            this.MultiColumnFileListMenuItem,
            this.ShowListBoxHorizontalScrollBarMenuItem,
            this.menuItem1,
            this.WordWrapMenuItem,
            this.ReadOnlyMenuItem,
            this.KeepListFocusMenuItem,
            this.ShiftFocusToTextMenuItem,
            this.RemoveHtmlTagsOptionMenuItem,
            this.RemoveHtmlAnchorsOptionMenuItem,
            this.RecurseFoldersOptionMenuItem,
            this.insertFileNamesOnJoinOptionMenuItem,
            this.SetListBoxViewCenteredOptionMenuItem,
            this.ShowVersionMenuItem});
            this.OptionsMenuItem.Text = "&Options";
            // 
            // FindWordsAnyOrderMenuItem
            // 
            this.FindWordsAnyOrderMenuItem.Index = 0;
            this.FindWordsAnyOrderMenuItem.Checked = true;
            this.FindWordsAnyOrderMenuItem.Text = "&Find Words Any Order Within A Sentence";
            this.FindWordsAnyOrderMenuItem.Click += new System.EventHandler(this.FindWordsAnyOrderMenuItem_Click);
            // 
            // FindWordsAnyPlaceMenuItem
            // 
            this.FindWordsAnyPlaceMenuItem.Checked = true;
            this.FindWordsAnyPlaceMenuItem.Index = 1;
            this.FindWordsAnyPlaceMenuItem.Text = "Find Words Any &Place Within A Sentence";
            this.FindWordsAnyPlaceMenuItem.Click += new System.EventHandler(this.FindWordsAnyPlaceMenuItem_Click);
            // 
            // UseRegularExpressionsMenuItem
            // 
            this.UseRegularExpressionsMenuItem.Checked = true;
            this.UseRegularExpressionsMenuItem.Index = 2;
            this.UseRegularExpressionsMenuItem.Text = "&Use Regular Expressions in Finder";
            this.UseRegularExpressionsMenuItem.Click += new System.EventHandler(this.UseRegularExpressionsMenuItem_Click);
            // 
            // CaseSensitiveFindsMenuItem
            // 
            this.CaseSensitiveFindsMenuItem.Index = 3;
            this.CaseSensitiveFindsMenuItem.Text = "&Case Sensitive Finds";
            this.CaseSensitiveFindsMenuItem.Click += new System.EventHandler(this.CaseSensitiveFindsMenuItem_Click);
            // 
            // MultiColumnFileListMenuItem
            // 
            this.MultiColumnFileListMenuItem.Checked = false ;
            this.MultiColumnFileListMenuItem.Index = 4;
            this.MultiColumnFileListMenuItem.Text = "&MultiColumn File List";
            this.MultiColumnFileListMenuItem.Click += new System.EventHandler(this.MultiColumnFileListMenuItem_Click);
            // 
            // ShowListBoxHorizontalScrollBarMenuItem
            // 
            this.ShowListBoxHorizontalScrollBarMenuItem.Index = 5;
            this.ShowListBoxHorizontalScrollBarMenuItem.Text = "Show ConcorDancerListBox &Horizontal ScrollBar";
            this.ShowListBoxHorizontalScrollBarMenuItem.Click += new System.EventHandler(this.ShowListBoxHorizontalScrollBarMenuItem_Click);
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 6;
            this.menuItem1.Text = "-";
            // 
            // WordWrapMenuItem
            // 
            this.WordWrapMenuItem.Checked = true;
            this.WordWrapMenuItem.Index = 7;
            this.WordWrapMenuItem.Text = "&Word Wrap ";
            this.WordWrapMenuItem.Click += new System.EventHandler(this.WordWrapMenuItem_Click);
            // 
            // ReadOnlyMenuItem
            // 
            this.ReadOnlyMenuItem.Checked = true;
            this.ReadOnlyMenuItem.Index = 8;
            this.ReadOnlyMenuItem.Text = "Read Only Te&xt";
            this.ReadOnlyMenuItem.Click += new System.EventHandler(this.ReadOnlyMenuItem_Click);
            // 
            // KeepListFocusMenuItem
            // 
            this.KeepListFocusMenuItem.Checked = true;
            this.KeepListFocusMenuItem.Index = 9;
            this.KeepListFocusMenuItem.Text = "K&eep Focus with List Items";
            this.KeepListFocusMenuItem.Click += new System.EventHandler(this.KeepListFocusMenuItem_Click);
            // 
            // ShiftFocusToTextMenuItem
            // 
            this.ShiftFocusToTextMenuItem.Index = 10;
            this.ShiftFocusToTextMenuItem.Text = "&Shift Focus to Text Selection";
            this.ShiftFocusToTextMenuItem.Click += new System.EventHandler(this.ShiftFocusToTextMenuItem_Click);
            // 
            // RemoveHtmlTagsOptionMenuItem
            // 
            this.RemoveHtmlTagsOptionMenuItem.Checked = true;
            this.RemoveHtmlTagsOptionMenuItem.Index = 11;
            this.RemoveHtmlTagsOptionMenuItem.Text = "Remove Html &Tags When Opening Html File(s)";
            this.RemoveHtmlTagsOptionMenuItem.Click += new System.EventHandler(this.RemoveHtmlTagsOptionMenuItem_Click);
            // 
            // RemoveHtmlAnchorsOptionMenuItem
            // 
            this.RemoveHtmlAnchorsOptionMenuItem.Index = 12;
            this.RemoveHtmlAnchorsOptionMenuItem.Text = "Remove Html &Anchors From Html Files(s)";
            this.RemoveHtmlAnchorsOptionMenuItem.Click += new System.EventHandler(this.RemoveHtmlAnchorsOptionMenuItem_Click);
            // 
            // RecurseFoldersOptionMenuItem
            // 
            this.RecurseFoldersOptionMenuItem.Checked = true;
            this.RecurseFoldersOptionMenuItem.Index = 13;
            this.RecurseFoldersOptionMenuItem.Text = "&Open SubFolders Recursively when Joining Files";
            this.RecurseFoldersOptionMenuItem.Click += new System.EventHandler(this.RecurseFoldersOptionMenuItem_Click);
            // 
            // insertFileNamesOnJoinOptionMenuItem
            // 
            this.insertFileNamesOnJoinOptionMenuItem.Index = 14;
            this.insertFileNamesOnJoinOptionMenuItem.Checked = true ;
            this.insertFileNamesOnJoinOptionMenuItem.Text = "Insert File Names On &Join";
            this.insertFileNamesOnJoinOptionMenuItem.Click += new System.EventHandler(this.InsertFileNamesOnJoinOptionMenuItem_Click);
            // 
            // SetListBoxViewCenteredOptionMenuItem
            // 
            this.SetListBoxViewCenteredOptionMenuItem.Checked = true;
            this.SetListBoxViewCenteredOptionMenuItem.Index = 15;
            this.SetListBoxViewCenteredOptionMenuItem.Text = "Se&t List Window to Center at Match Point";
            this.SetListBoxViewCenteredOptionMenuItem.Click += new System.EventHandler(this.SetListBoxViewCenteredOptionMenuItem_Click);
            // 
            // ShowVersionMenuItem
            // 
            this.ShowVersionMenuItem.Index = 16;
            this.ShowVersionMenuItem.Text = "Show &Version Info";
            this.ShowVersionMenuItem.Click += new System.EventHandler(this.ShowVersionMenuItem_Click);
            // 
            // contextMenuItem
            // 
            this.contextMenuItem.Index = 2;
            this.contextMenuItem.Text = "&ConText";
            // 
            // functionsMenuItem
            // 
            this.functionsMenuItem.Index = 3;
            this.functionsMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SetFontMenuItem,
            this.SetBackgroundColorMenuItem,
            this.refreshDisplayMenuItem,
            this.refreshListMenuItem,
            this.SortWordsBackMenuItem,
            this.sortFunctionMenuItem,
            this.GlobalSortOptionMenuItem});
            this.functionsMenuItem.Text = "F&unctions";
            // 
            // SetFontMenuItem
            // 
            this.SetFontMenuItem.Index = 0;
            this.SetFontMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SetTextBoxFontMenuItem,
            this.SetListBoxFontMenuItem,
            this.SetFileListFontMenuItem,
            this.SetTreeViewFontMenuItem});
            this.SetFontMenuItem.Text = "Set &Font";
            // 
            // SetTextBoxFontMenuItem
            // 
            this.SetTextBoxFontMenuItem.Index = 0;
            this.SetTextBoxFontMenuItem.Text = "Set &Text Font";
            this.SetTextBoxFontMenuItem.Click += new System.EventHandler(this.SetTextBoxFontMenuItem_Click);
            // 
            // SetListBoxFontMenuItem
            // 
            this.SetListBoxFontMenuItem.Index = 1;
            this.SetListBoxFontMenuItem.Text = "Set &List Font";
            this.SetListBoxFontMenuItem.Click += new System.EventHandler(this.SetListBoxFontMenuItem_Click);
            // 
            // SetFileListFontMenuItem
            // 
            this.SetFileListFontMenuItem.Index = 2;
            this.SetFileListFontMenuItem.Text = "Set &FileList Font";
            this.SetFileListFontMenuItem.Click += new System.EventHandler(this.SetFileListFontMenuItem_Click);
            // 
            // SetTreeViewFontMenuItem
            // 
            this.SetTreeViewFontMenuItem.Index = 3;
            this.SetTreeViewFontMenuItem.Text = "Set &Directory Font";
            this.SetTreeViewFontMenuItem.Click += new System.EventHandler(this.SetDirectoryTreeFontMenuItem_Click);
            // 
            // SetBackgroundColorMenuItem
            // 
            this.SetBackgroundColorMenuItem.Index = 1;
            this.SetBackgroundColorMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.SetDirectoryTreeBackgroundColorMenuItem,
            this.SetTextBoxBackgroundColorMenuItem,
            this.SetFileListBackgroundColorMenuItem,
            this.SetListBoxBackgroundColorMenuItem});
            this.SetBackgroundColorMenuItem.Text = "Set &Background Color";
            // 
            // SetDirectoryTreeBackgroundColorMenuItem
            // 
            this.SetDirectoryTreeBackgroundColorMenuItem.Index = 0;
            this.SetDirectoryTreeBackgroundColorMenuItem.Text = "Set DirectoryTree &Background Color";
            this.SetDirectoryTreeBackgroundColorMenuItem.Click += new System.EventHandler(this.SetDirectoryTreeBackgroundColorMenuItem_Click);
            // 
            // SetTextBoxBackgroundColorMenuItem
            // 
            this.SetTextBoxBackgroundColorMenuItem.Index = 1;
            this.SetTextBoxBackgroundColorMenuItem.Text = "Set Text &Background Color";
            this.SetTextBoxBackgroundColorMenuItem.Click += new System.EventHandler(this.SetTextBoxBackgroundColorMenuItem_Click);
            // 
            // SetFileListBackgroundColorMenuItem
            // 
            this.SetFileListBackgroundColorMenuItem.Index = 2;
            this.SetFileListBackgroundColorMenuItem.Text = "Set FileList Background &Color";
            this.SetFileListBackgroundColorMenuItem.Click += new System.EventHandler(this.SetFileListBackgroundColorMenuItem_Click);
            // 
            // SetListBoxBackgroundColorMenuItem
            // 
            this.SetListBoxBackgroundColorMenuItem.Index = 3;
            this.SetListBoxBackgroundColorMenuItem.Text = "Set FindList Background &Color";
            this.SetListBoxBackgroundColorMenuItem.Click += new System.EventHandler(this.SetListBoxBackgroundColorMenuItem_Click);
            // 
            // refreshDisplayMenuItem
            // 
            this.refreshDisplayMenuItem.Index = 2;
            this.refreshDisplayMenuItem.Text = "Refresh &Display";
            this.refreshDisplayMenuItem.Click += new System.EventHandler(this.RefreshDisplayMenuItem_Click);
            // 
            // refreshListMenuItem
            // 
            this.refreshListMenuItem.Index = 3;
            this.refreshListMenuItem.Text = "Refresh &List";
            this.refreshListMenuItem.Click += new System.EventHandler(this.RefreshListMenuItem_Click);
            // 
            // SortWordsBackMenuItem
            // 
            this.SortWordsBackMenuItem.Index = 4;
            this.SortWordsBackMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.Start0WordsBackMenuItem,
            this.Start1WordBackMenuItem,
            this.Start2WordsBackMenuItem,
            this.Start3WordsBackMenuItem,
            this.Start4WordsBackMenuItem,
            this.Start5WordsBackMenuItem});
            this.SortWordsBackMenuItem.Text = "Set \'Words Back\' Sort O&ption";
            // 
            // Start0WordsBackMenuItem
            // 
            this.Start0WordsBackMenuItem.Index = 0;
            this.Start0WordsBackMenuItem.Text = "Start Sort &0 Words Back";
            this.Start0WordsBackMenuItem.Click += new System.EventHandler(this.Start0WordsBackMenuItem_Click);
            // 
            // Start1WordBackMenuItem
            // 
            this.Start1WordBackMenuItem.Index = 1;
            this.Start1WordBackMenuItem.Text = "Start Sort &1 Word  Back";
            this.Start1WordBackMenuItem.Click += new System.EventHandler(this.Start1WordBackMenuItem_Click);
            // 
            // Start2WordsBackMenuItem
            // 
            this.Start2WordsBackMenuItem.Index = 2;
            this.Start2WordsBackMenuItem.Text = "Start Sort &2 Words Back";
            this.Start2WordsBackMenuItem.Click += new System.EventHandler(this.Start2WordsBackMenuItem_Click);
            // 
            // Start3WordsBackMenuItem
            // 
            this.Start3WordsBackMenuItem.Index = 3;
            this.Start3WordsBackMenuItem.Text = "Start Sort &3 Words Back";
            this.Start3WordsBackMenuItem.Click += new System.EventHandler(this.Start3WordsBackMenuItem_Click);
            // 
            // Start4WordsBackMenuItem
            // 
            this.Start4WordsBackMenuItem.Index = 4;
            this.Start4WordsBackMenuItem.Text = "Start Sort &4 Words Back";
            this.Start4WordsBackMenuItem.Click += new System.EventHandler(this.Start4WordsBackMenuItem_Click);
            // 
            // Start5WordsBackMenuItem
            // 
            this.Start5WordsBackMenuItem.Index = 5;
            this.Start5WordsBackMenuItem.Text = "Start Sort &5 Words Back";
            this.Start5WordsBackMenuItem.Click += new System.EventHandler(this.Start5WordsBackMenuItem_Click);
            // 
            // sortFunctionMenuItem
            // 
            this.sortFunctionMenuItem.Index = 5;
            this.sortFunctionMenuItem.Text = "&Sort";
            this.sortFunctionMenuItem.Click += new System.EventHandler(this.SortFunctionMenuItem_Click);
            // 
            // GlobalSortOptionMenuItem
            // 
            this.GlobalSortOptionMenuItem.Index = 6;
            this.GlobalSortOptionMenuItem.Text = "Sort F&ind List Each Time";
            this.GlobalSortOptionMenuItem.Click += new System.EventHandler(this.SortOptionMenuItem_Click);
            // 
            // RestoreStateMenuItem
            // 
            this.RestoreStateMenuItem.Index = -1;
            this.RestoreStateMenuItem.Text = "";
            // 
            // BackButton
            // 
            this.BackButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));

            if ( File.Exists ("prev.gif" ) ) this.BackButton.Image = Image.FromFile( "prev.gif");
            else this.BackButton.Text = "<" ;

            //this.BackButton.Image = global::ConcorDancer.Properties.Resources.prev;
            this.BackButton.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.BackButton.Location = new System.Drawing.Point(592, 1);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(23, 23);
            this.BackButton.TabIndex = 4;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // ForwardButton
            // 
            this.ForwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            if ( File.Exists ("next.gif" ) ) this.ForwardButton.Image = Image.FromFile( "next.gif");
            else this.ForwardButton.Text = ">" ;
            //this.ForwardButton.Image = global::ConcorDancer.Properties.Resources.next;
            this.ForwardButton.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
            this.ForwardButton.Location = new System.Drawing.Point(621, 1);
            this.ForwardButton.Name = "ForwardButton";
            this.ForwardButton.Size = new System.Drawing.Size(23, 23);
            this.ForwardButton.TabIndex = 3;
            this.ForwardButton.Click += new System.EventHandler(this.ForwardButton_Click);
            // 
            // ComboBoxForFind
            // 
            this.ComboBoxForFind.Font = new System.Drawing.Font("Courier New", 9F);
            this.ComboBoxForFind.ItemHeight = 15;
            this.ComboBoxForFind.Location = new System.Drawing.Point(4, 0);
            this.ComboBoxForFind.MaxLength = 128;
            this.ComboBoxForFind.Name = "ComboBoxForFind";
            this.ComboBoxForFind.Size = new System.Drawing.Size(267, 23);
            this.ComboBoxForFind.TabIndex = 0;
            this.ComboBoxForFind.Text = "";
            this.ComboBoxForFind.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ComboBox1_KeyUp);
            // 
            // ComboBoxForHistory
            // 
            this.ComboBoxForHistory.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ComboBoxForHistory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBoxForHistory.Location = new System.Drawing.Point(374, 1);
            this.ComboBoxForHistory.MaxLength = 128;
            this.ComboBoxForHistory.Name = "ComboBoxForHistory";
            this.ComboBoxForHistory.Size = new System.Drawing.Size(212, 23);
            this.ComboBoxForHistory.TabIndex = 2;
            this.ComboBoxForHistory.SelectedIndexChanged += new System.EventHandler(this.FindHistoryComboBox_SelectedIndexChanged);
            // 
            // TabControl
            // 
            this.TabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.TabControl.Location = new System.Drawing.Point(0, 27);
            this.TabControl.Name = "TabControl";
            this.TabControl.SelectedIndex = 0;
            this.TabControl.Size = new System.Drawing.Size(646, 319);
            this.TabControl.TabIndex = 5;
            this.TabControl.DoubleClick += new System.EventHandler(this.TabControl1_DoubleClick);
            this.TabControl.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // ConcorDancerWinForm
            // 
            this.ClientSize = new System.Drawing.Size(646, 346);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.ForwardButton);
            this.Controls.Add(this.FindButton);
            this.Controls.Add(this.CounterLabel);
            this.Controls.Add(this.ComboBoxForFind);
            this.Controls.Add(this.ComboBoxForHistory);
            this.Controls.Add(this.TabControl);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(2000, 1000);
            this.Menu = this.MenuStrip;
            this.Name = "ConcorDancerWinForm";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

        private void ComboBox1_KeyUp(object sender, KeyEventArgs e)
        {
            ConcorDancer.Cdm.FinderComboBox_KeyUp(e);
        }

        private void FindHistoryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ConcorDancer.Cdm.FindHistoryComboBox_SelectedIndexChanged();
        }

        private void TabControl1_DoubleClick(object sender, EventArgs e)
        {
            ConcorDancer.Cdm.TabControl1_DoubleClick((ConcorDancerTabControl)sender);
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
			ConcorDancer.Cdm.TabControl1_SelectedIndexChanged ();
        }

    }
}