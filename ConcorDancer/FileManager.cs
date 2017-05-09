
using System;
using System.Drawing ;
using System.Windows.Forms;
using System.Collections;
using System.IO ;

namespace ConcorDancer
{
	public class
	DirectoryTabPage : TabPage
	{
        public DLList<String> DirectoryList = new DLList<String>();
		public DataGridView DataGridView ;
	}
	
	public class
	FileManagerTabPage : TabPage 
	{
		public MenuItem NewFileManagerTabPageMenuItem ;//= new MenuItem () ;
		public SplitContainer FileManagerSplitContainer = new SplitContainer () ;
		public TreeView TreeView = new TreeView () ;
		public ConcorDancerTabControl TabControl = new ConcorDancerTabControl () ;
		public MenuItem CloseFileManagerTabPageMenuItem ;//= new MenuItem () ;
		DataGridViewCellStyle FileCellStyle ; //= dataGridView.RowDefaultCellStyle.Clone ()  ;
		DataGridViewCellStyle DirectoryCellStyle ; //= dataGridView.RowDefaultCellStyle.Clone ()  
		DataGridViewCellStyle SizeColumnCellStyle ;
		DataGridViewCellStyle ModifiedTimeColumnCellStyle ;
		//public DataGridView currentDataGridView;
		public DirectoryTabPage CurrentDirectoryTabPage ;
		public string CurrentDirectoryName ;
		string [] Directories, Files ;
        public int SavedWidth, SavedHeight;

        public void
        AdjustDisplay()
        {
            //UseExistingDirectoryTabPage(ConcorDancer.Cdm.CurrentDirectory);
            SavedHeight = Height;
            SavedWidth = Width;
        }

		string
		ConvertCurrentDirectoryNameToTabPageName ()
		{
			//string name = String.Copy ( currentDirectoryName ) ;
			//string name = currentDirectoryName ;
			//return name.Length < 20 ? name : "..." + name.Substring (name.Length - 17 )  ;
			return CurrentDirectoryName.Length < 20 ? CurrentDirectoryName : "..." + 
				CurrentDirectoryName.Substring (CurrentDirectoryName.Length - 17 )  ;
		}

		public void
		SetDataGridViewColumnWidthDetailsMode ( )
		{
			try
			{
                DataGridView dataGridView = ConcorDancer.CurrentDataGridView;
                if ((dataGridView != null) &&( dataGridView.Columns.Count != 0 ))
                {
                    int width0 = 0, newWidth0;
                    int width1 = 0, newWidth1;
                    int width2 = 0, newWidth2, height = 0, dataGridViewWidth;
                    //if ( ConcorDancer.Cdwf.multiColumnFileListMenuItem.Checked == true )
                    {
                        dataGridView.Font = ConcorDancer.DataGridViewFileFont;
                        dataGridView.ScrollBars = ScrollBars.Both;
                        Graphics g = dataGridView.CreateGraphics();
                        foreach (DataGridViewRow row in dataGridView.Rows)
                        {
                            newWidth0 = (int)g.MeasureString(
                                ConcorDancer.MakeFilenameFromFullPathName((string)(row.Cells[0].Value)),
                                    row.Cells[0].InheritedStyle.Font).Width;
                            newWidth1 = (int)g.MeasureString((string)(row.Cells[1].Value.ToString()),
                                row.Cells[1].InheritedStyle.Font).Width;
                            newWidth2 = (int)g.MeasureString((string)(row.Cells[2].Value),
                                row.Cells[2].InheritedStyle.Font).Width;
                            // from DataGridView.ColumnWidth example 
                            if (newWidth0 > width0) width0 = newWidth0;
                            if (newWidth1 > width1) width1 = newWidth1;
                            if (newWidth2 > width2) width2 = newWidth2;
                            height += row.Height;
                        }
                        //int minimumColumnsWidth = width0 + width1 + width2  ; 
                        dataGridView.Columns[0].MinimumWidth = width0;
                        dataGridView.Columns[1].MinimumWidth = width1;
                        dataGridView.Columns[2].MinimumWidth = width2;
                        //width0 += 8 ; width1+= 12 ; width2+= 4 ; // allow for more of the wider characters in the strings not calculated by MeasureString ??
                        int columnsWidth = width0 + width1 + width2;
                        // see if the scroll bar will be showing 
                        if (height <= Height) dataGridViewWidth = dataGridView.Width;
                        else dataGridViewWidth = dataGridView.Width - ConcorDancer.ScrollBarWidth - 2;
                        if (dataGridViewWidth > columnsWidth)
                        {
                            int extraWidth = dataGridViewWidth - columnsWidth - 6;
                            {
                                width0 += extraWidth / 3;
                                //width0 += extraWidth / 2 ;
                                width1 += extraWidth / 2;
                                width2 += extraWidth / 6;
                            }
                            columnsWidth = width0 + width1 + width2;
                            if (dataGridViewWidth < columnsWidth) width0 -= columnsWidth - dataGridViewWidth;
                            if (dataGridViewWidth > columnsWidth) width0 += dataGridViewWidth - columnsWidth - 2;
                        }
                        dataGridView.Columns[0].Width = width0;
                        dataGridView.Columns[1].Width = width1;
                        dataGridView.Columns[2].Width = width2;
                    }
                }
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		AddDirectoryAndFilesToDataGridViewDetailsMode (  )
		{
			try
			{
				FileCellStyle.ForeColor = ConcorDancer.FileForeColor ;
				FileCellStyle.BackColor = ConcorDancer.FileListBackColor ;
				FileCellStyle.Font = ConcorDancer.DataGridViewFileFont ;
				DirectoryCellStyle.ForeColor = ConcorDancer.DirectoryForeColor ;
				DirectoryCellStyle.BackColor = ConcorDancer.FileListBackColor ;
				DirectoryCellStyle.Font = ConcorDancer.DataGridViewDirectoryFont ;
				//currentDirectoryTabPage.BackColor = ConcorDancer.fileListBackColor ;
				DataGridView dataGridView = ConcorDancer.CurrentDataGridView ;
				DataGridViewRow rowTemplate = new DataGridViewRow ();
				rowTemplate.Height = FileCellStyle.Font.Height + 3;
				dataGridView.RowTemplate = rowTemplate;
				//dataGridView.BackColor = ConcorDancer.fileListBackColor ;
				dataGridView.BackgroundColor = ConcorDancer.FileListBackColor ;
				dataGridView.ColumnCount = 3 ;
				dataGridView.Columns[0].Name = "Name";
				dataGridView.Columns[1].Name = "Size";
				dataGridView.Columns[2].Name = "Last Modified";
				dataGridView.Columns [1].DefaultCellStyle = SizeColumnCellStyle ;
				dataGridView.Columns [2].DefaultCellStyle = ModifiedTimeColumnCellStyle ;
				//dataGridView.RowHeadersVisible = false;
				dataGridView.ColumnHeadersVisible = true ;
				dataGridView.AllowUserToResizeRows = false;
				dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None ;
				dataGridView.Rows.Clear ( ) ;
				dataGridView.Rows.Add ( Directories.Length + Files.Length ) ;
				int row = 0 ;
				//dataGridView.Rows [ row ] .DividerHeight = 0 ; // dataGridView.Font.Height  ;
				dataGridView.Rows [ row ] .DefaultCellStyle = DirectoryCellStyle ;
				dataGridView.Rows [ row ] .Height = DirectoryCellStyle.Font.Height  + 2 ; //dataGridView.Font.Height  ;
				dataGridView.Rows [ row ] .Cells [0].Value =  ".."  ;
				dataGridView.Rows [ row ++ ].Cells [1].Value =  (object) ( (long)0 ); //"<folder>"  ;
				//dataGridView.Rows [ row ++ ].Cells [1].Value =  "0"; //"<folder>"  ;


				ArrayList directoriesAL = new ArrayList ( Directories ) ;
				directoriesAL.Sort () ;
				for ( int i = 0 ; i <  directoriesAL.Count ; i++, row++  ) 
				{
					//dataGridView.Rows [ row ] .DividerHeight = 0;//dataGridView.Font.Height  ;
					dataGridView.Rows [ row ] .DefaultCellStyle = DirectoryCellStyle ;
					//dataGridView.Rows [ row ] .Height = directoryCellStyle.Font.Height + 2  ; //dataGridView.Font.Height  ;
					dataGridView.Rows [ row ].Cells [0].Value =  
						ConcorDancer.MakeFilenameFromFullPathName ( (string) directoriesAL [ i ] )  ;
					//dataGridView.Rows [ row ] .Cells [1].Style = fileCellStyle ;
					dataGridView.Rows [ row  ].Cells [1].Value =  (object) ( (long)0 ); //"<folder>"  ;
					//dataGridView.Rows [ row ].Cells [1].Value =  "0" ; //"<folder>"  ;
					FileInfo fi = new FileInfo ( (string) directoriesAL [ i ] ) ;
					//dataGridView.Rows [ row ] .Cells [1].Value =  fi.Length ; // .ToString ("#,#")  ;
					//dataGridView.Rows [ row ] .Cells [2].Style = fileCellStyle ; // default Style
					dataGridView.Rows [ row ] .Cells [2].Value =  fi.LastWriteTime.ToString ("s")   ;
				}
				ArrayList filesAL = new ArrayList ( Files ) ;
				filesAL.Sort () ;
				for ( int i = 0; i <  filesAL.Count ; i++, row++  ) 
				{
					//dataGridView.Rows [ row ] .DividerHeight = 0;//dataGridView.Font.Height  ;
					//dataGridView.Rows [ row ] .DefaultCellStyle = fileCellStyle ;
					//dataGridView.Rows [ row ] .Height = fileCellStyle.Font.Height + 2  ; //dataGridView.Font.Height  ;
					dataGridView.Rows [ row ] .Cells [0].Value = 
						ConcorDancer.MakeFilenameFromFullPathName (  (string)filesAL [ i ] )  ;
					FileInfo fi = new FileInfo (  (string) filesAL [ i ] ) ;
					dataGridView.Rows [ row ] .Cells [1].Style.Alignment = DataGridViewContentAlignment.MiddleRight ;
					dataGridView.Rows [ row ] .Cells [1].Value =  fi.Length ; //.ToString ("#,#")  ;
					//dataGridView.Rows [ row ] .Cells [2].Style = fileCellStyle ;
					dataGridView.Rows [ row ] .Cells [2].Value =  fi.LastWriteTime.ToString ("s")   ;
				}
				SetDataGridViewColumnWidthDetailsMode ( ) ;
				CurrentDirectoryName = ConcorDancer.CurrentDataGridView.Name ;
				string dname = ConvertCurrentDirectoryNameToTabPageName () ;
				CurrentDirectoryTabPage.Text = dname ;
                Refresh();
				
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		SetDataGridViewColumnWidthListMode ( )
		{
			try
			{
				DataGridView dataGridView = ConcorDancer.CurrentDataGridView ;
                if (dataGridView == null) return; // happens when we first set up - prevent Error Handling
                dataGridView.ScrollBars = ScrollBars.Horizontal;
                ArrayList columns = new ArrayList();
				Graphics g = dataGridView.CreateGraphics() ;
				int width ;
				
				for ( int column = 0 ; column < dataGridView.ColumnCount ; column++ ) 
				{
					ArrayList rows = new ArrayList () ;
					columns.Add ( rows) ;
					for ( int row = 0 ; row < dataGridView.Rows.Count ; row++ )
					{
						string data = (string)(dataGridView.Rows [row].Cells [column].Value) ;
						if ( data == null ) break ;
						width = (int) g.MeasureString( 
							ConcorDancer.MakeFilenameFromFullPathName( data ), 
								dataGridView.Rows [row].Cells [column].InheritedStyle.Font).Width; 
						rows.Add ( width)  ;
					}
				}
				width = 0 ; 
				for ( int column = 0 ; column < columns.Count ; column++ ) 
				{
					object newWidth  ;
					ArrayList rows = (ArrayList) columns [ column ] ;
					if ( rows == null ) break ;
					for ( int row = 0 ; row < rows.Count ; row++ )
					{
						newWidth = rows [ row ] ;
						if ( newWidth == null ) break ;
						if ( width < (int) newWidth ) width = (int) newWidth ;
					}
				}
				for ( int i = 0 ; i < dataGridView.ColumnCount ; i++ ) 	
					dataGridView.Columns [i].Width = width  + 8 ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		AddDirectoryAndFilesToDataGridViewListMode ( )
		{
			try
			{
				FileCellStyle.ForeColor = ConcorDancer.FileForeColor ;
				FileCellStyle.BackColor = ConcorDancer.FileListBackColor ;
				FileCellStyle.Font = ConcorDancer.DataGridViewFileFont ;
				DirectoryCellStyle.ForeColor = ConcorDancer.DirectoryForeColor ;
				DirectoryCellStyle.BackColor = ConcorDancer.FileListBackColor ;
				DirectoryCellStyle.Font = ConcorDancer.DataGridViewDirectoryFont ;
				CurrentDirectoryTabPage.BackColor = ConcorDancer.FileListBackColor ;
				
				DataGridView dataGridView = ConcorDancer.CurrentDataGridView ;
				DataGridViewRow rowTemplate = new DataGridViewRow () ;
				rowTemplate.Height = FileCellStyle.Font.Height + 3 ;
				dataGridView.RowTemplate = rowTemplate ;
				//dataGridView.BackColor = ConcorDancer.fileListBackColor ;
				dataGridView.BackgroundColor = ConcorDancer.FileListBackColor ;
				dataGridView.ColumnCount = 1 ;
				dataGridView.RowHeadersVisible = false;
				dataGridView.AllowUserToResizeRows = false;
				dataGridView.Rows.Clear ( ) ;
				dataGridView.ColumnHeadersVisible = false ;
				//dataGridView.ColumnCount = 1 ;
				dataGridView.Rows.Add ((dataGridView.Height - ConcorDancer.ScrollBarWidth)/
					(DirectoryCellStyle.Font.Height + 2)) ;

				int column = 0, row = 0, height = 0 ;
				dataGridView.Rows [ row ] .Cells [column].Style = DirectoryCellStyle ;
				dataGridView.Rows [ row ] .Cells [column].Value =  ".."  ;
				//dataGridView.Rows [ row ] .Height = directoryCellStyle.Font.Height   + 2 ; //dataGridView.Font.Height  ;

				height += dataGridView.Rows [ row ] .Height  ; //dataGridView.Font.Height  ;
				ArrayList directoriesAL = new ArrayList ( Directories ) ;
				directoriesAL.Sort () ;
				for ( int i = 0 ; i <  directoriesAL.Count ; i++  ) 
				{
                    if ( height + dataGridView.Rows[row].Height >= dataGridView.Height -ConcorDancer.ScrollBarWidth)
                    {
						dataGridView.ColumnCount ++ ;
						column ++ ;
						row = -1 ;
						height = 0 ;
					}
					//dataGridView.Rows [ row ] .DividerHeight = 0;//dataGridView.Font.Height  ;
					dataGridView.Rows [ ++row ] .Cells [column].Style = DirectoryCellStyle ;
					dataGridView.Rows [ row ].Cells [ column ].Value =
						ConcorDancer.MakeFilenameFromFullPathName ( (string)directoriesAL [ i ] );
					//dataGridView.Rows [ row ].Height = directoryCellStyle.Font.Height + 2; //dataGridView.Font.Height  ;
					height += dataGridView.Rows [ row ] .Height ;
				}
				ArrayList filesAL = new ArrayList ( Files ) ;
				filesAL.Sort () ;
				for ( int i = 0; i <  filesAL.Count ; i++  ) 
			    {
                    if ( height + dataGridView.Rows[row].Height >= dataGridView.Height - ConcorDancer.ScrollBarWidth)
                    {
						dataGridView.ColumnCount ++ ;
						column ++ ;
						row = -1 ;
						height = 0 ;
					}
					dataGridView.Rows [ ++row ].Cells [column].Value =  
						ConcorDancer.MakeFilenameFromFullPathName ( (string) filesAL [ i ] )  ;
					dataGridView.Rows [ row ].Cells [ column ].Style = FileCellStyle;
					//dataGridView.Rows [ row ].Height = fileCellStyle.Font.Height + 2;
					height += dataGridView.Rows [ row ].Height ;
				}
				SetDataGridViewColumnWidthListMode () ;
				CurrentDirectoryName = ConcorDancer.CurrentDataGridView.Name ;
				string dname = ConvertCurrentDirectoryNameToTabPageName () ;
				CurrentDirectoryTabPage.Text = dname ;
				
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		void 
		AddNewTabPage ( string directoryName )
		{
			try
			{
				DirectoryTabPage directoryTabPage = new DirectoryTabPage () ;
				CurrentDirectoryTabPage = directoryTabPage ;
				directoryTabPage.Dock = DockStyle.Fill ;
				
				DataGridView dataGridView = new DataGridView () ;
                dataGridView.MouseEnter += new EventHandler(DataGridView_MouseEnter);
				DataGridViewRow rowTemplate = new DataGridViewRow ();
				rowTemplate.Height = FileCellStyle.Font.Height + 3;
				dataGridView.RowTemplate = rowTemplate;
				directoryTabPage.DataGridView = dataGridView;
				ConcorDancer.CurrentDataGridView = dataGridView ;
				dataGridView.BackgroundColor = ConcorDancer.FileListBackColor ;
				dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect ; 
				dataGridView.RowsDefaultCellStyle = FileCellStyle ;
				/*
				dataGridView.ContextMenu = new ContextMenu () ;
				// can't seem to get the ContextMenu to work
				dataGridView.ContextMenu.MenuItems.Add ( NewFileManagerTabPageMenuItem.CloneMenu () ) ;
				dataGridView.ContextMenu.MenuItems.Add ( closeFileManagerTabPageMenuItem.CloneMenu ()  ) ;
				*/
				dataGridView.MultiSelect = false;
				dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically ;
				dataGridView.ColumnHeadersHeightSizeMode = 
					DataGridViewColumnHeadersHeightSizeMode.DisableResizing ;
				dataGridView.ColumnHeadersHeight = dataGridView.Font.Height + 4 ;
				dataGridView.RowHeadersVisible = false ;
				dataGridView.AllowUserToResizeRows = false;
				dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None ;
				dataGridView.Dock = DockStyle.Fill ;
				dataGridView.CellClick += new DataGridViewCellEventHandler ( DataGridView_CellClick ) ;
				dataGridView.Name = directoryName ; // this is important - it is used by dataGridView_SelectionChanged
				dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Gray ;
				dataGridView.ColumnHeadersDefaultCellStyle.Alignment = 
					DataGridViewContentAlignment.MiddleCenter ;
				dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single ;
				dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None;
				dataGridView.GridColor = ConcorDancer.FileListBackColor ;
				dataGridView.RowHeadersVisible = false;
				dataGridView.AllowUserToResizeRows = false;
				//dataGridView.CellBorderStyle = DataGridViewCellBorderStyle.None ;
   
				directoryTabPage.Name = directoryName ;
				directoryTabPage.Controls.Add ( dataGridView ) ;
                DLLNode<String> node = new DLLNode<String>();
                node.Value = directoryName;
				directoryTabPage.DirectoryList.AddIfNotAlreadyPresent ( node ) ;

				TabControl.Controls.Add ( directoryTabPage ) ;
				TabControl.SelectedTab = directoryTabPage ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void 
		GetDirectoriesAndFiles ( string name )
		{
			try
			{
				Directories = Directory.GetDirectories ( name ) ; // get directories first before Clear (), Add () ; they could cause an exception
				Files = Directory.GetFiles ( name ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void 
		AddDirectoryTabPage ( string directoryName )
		{
			try
			{
                ConcorDancer.Cdm.CurrentDirectory = directoryName ;
				AddNewTabPage ( directoryName ) ;
				GetDirectoriesAndFiles ( directoryName ) ;
				//CurrentDirectoryName = directoryName ;
				//string dname = ConvertCurrentDirectoryNameToTabPageName () ;
				//CurrentDirectoryTabPage.Text = dname ;
                DLLNode<String> node = new DLLNode<String>();
                node.Value = SetTitles(directoryName); 
				CurrentDirectoryTabPage.DirectoryList.AddIfNotAlreadyPresent ( node ) ;
                
				//ConcorDancer.Cdwf.Text = directoryName ;
				//Text = dname ;
                //ConcorDancer.Cdm.SetWindowTitle();
                DataGridView dataGridView = ConcorDancer.CurrentDataGridView;
				dataGridView.Name = directoryName ;
				//dataGridView.Sorted = false ;
				if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
					AddDirectoryAndFilesToDataGridViewListMode (  ) ;
				else AddDirectoryAndFilesToDataGridViewDetailsMode (  ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
/*		
		public void 
		AddDirectoryTabPage ( string directoryName )
		{
			try
			{
				//
				foreach ( TabPage tp in tabControl.TabPages )
				{
					if ( tp.Name == directoryName ) 
					{
						//tabControl.SelectedTab = tp ;
						//currentDirectoryTabPage = (DirectoryTabPage) tp  ;
						directoryName = "." ;
						break ; // don't put on a second copy TabPage
					}
				}
				//
				AddDirectoryTabPage ( directoryName ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
*/		
		void 
		UseExistingDirectoryTabPage ( string directoryName )
		{
			try
			{
				GetDirectoriesAndFiles ( directoryName ) ;
				//CurrentDirectoryName = directoryName ;
				//string dname = ConvertCurrentDirectoryNameToTabPageName () ;
				//CurrentDirectoryTabPage.Text = dname ;
                DLLNode<String> node = new DLLNode<String>();
                node.Value = SetTitles(directoryName); 
                CurrentDirectoryTabPage.DirectoryList.AddIfNotAlreadyPresent(node);
				//currentDirectoryTabPage.Name = node.Name ;
				//ConcorDancer.Cdwf.Text = directoryName ;
				//Text = dname ;
            
                //ConcorDancer.Cdm.SetWindowTitle();
                DataGridView dataGridView = ConcorDancer.CurrentDataGridView;
				dataGridView.Name = directoryName ;
				//dataGridView.Sorted = false ;
				if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
					AddDirectoryAndFilesToDataGridViewListMode (  ) ;
				else AddDirectoryAndFilesToDataGridViewDetailsMode (  ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		NewFileManagerTabPageMenuItem_Click  ( object sender, System.EventArgs e )
		{
			try
			{
				//if ( ConcorDancer.Cdm.currentDirectory == null ) ConcorDancer.Cdm.currentDirectory = "C:\\" ;
				AddDirectoryTabPage ( ConcorDancer.Cdm.CurrentDirectory ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		CloseFileManagerTabPageMenuItem_Click ( object sender, System.EventArgs e )
		{
			try
			{
				TabControl.TabPages.Remove ( TabControl.SelectedTab ) ;
				ConcorDancer.Cdm.CurrentFileManagerTabPage.Activate () ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		void
		FileManagerSplitContainer_SplitterMoved ( object sender, SplitterEventArgs e )
		{
			if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
			{
				SetDataGridViewColumnWidthListMode () ;
			}
			else SetDataGridViewColumnWidthDetailsMode () ;
		}

		void 
		TreeView1_NodeMouseHover (object sender,  TreeNodeMouseHoverEventArgs  e)
		{
			try
			{
				((TreeView) sender).SelectedNode = e.Node ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		void 
		TreeView1_NodeMouseClick (object sender,  TreeNodeMouseClickEventArgs e)
		{
			try
			{
				TreeNode tnode = ((TreeView) sender).SelectedNode  ;
				if ( tnode.FirstNode == null ) // don't add if we have already
				{
					try
					{
						Directories = Directory.GetDirectories ( tnode.Name ) ;
						ConcorDancer.Cdm.CurrentDirectory = tnode.Name ;
                        ArrayList directoriesAL = new ArrayList(Directories);
                        directoriesAL.Sort();
                        //for (int i = 0; i < directoriesAL.Count; i++)
                        foreach (string directoryName in directoriesAL)
						{
							TreeNode treeNode = new TreeNode() ;
							treeNode.Name = directoryName ;
							treeNode.Text = 
								ConcorDancer.MakeFilenameFromFullPathName ( directoryName ) ;
							tnode.Nodes.Add ( treeNode ) ;
						}
					}
					// ?? this catch should never be possible because we only have directories in the TreeView
					catch ( Exception ex) // no directories but files maybe
					{
						ConcorDancer.Cdm.HandleException ( ex, false ) ;
					}
				}
				if ( TabControl.TabPages.Count > 0 )
					UseExistingDirectoryTabPage (  tnode.Name ) ;
				else AddDirectoryTabPage ( tnode.Name ) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}

		public void
		TabControl_DoubleClick (object sender, System.EventArgs e)
		{
			ConcorDancerTabControl tabControl = (ConcorDancerTabControl)sender ;
			try
			{
				tabControl.TabPages.Remove ( tabControl.SelectedTab) ;
			}
			catch ( Exception ex )
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
        string
        SetTitles ( string directoryName )		
        {
            CurrentDirectoryName = directoryName ;
            Text = ConvertCurrentDirectoryNameToTabPageName () ;
            //ConcorDancer.Cdwf.Text = directoryName;
            ConcorDancer.Cdm.SetWindowTextWithActiveFileManager(directoryName);
            CurrentDirectoryTabPage.Text = Text;
            return Text;
        }

		public void
		DataGridView_CellClick  (object sender, DataGridViewCellEventArgs e)
		{
			DataGridView dataGridView = (DataGridView) sender  ;
			string name = dataGridView.Name ; 
			string directoryName = null ;
			//directories = null ;
			
			if ( e.RowIndex < 0) return ; // column header 
			DataGridViewCell cell =  dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
			//if ( dataGridView_SelectionChangedGuard == true ) return ;
			try
			{
				if ( ((string)cell.Value) == ".." ) // the ".." selection
				{
					if ( name.EndsWith ( "\\" ) == false) 
					{
						directoryName = name.Substring ( 0, name.LastIndexOf ( "\\" )   ) ;
					}
					else
					{
						directoryName = name.Substring ( 0, name.Length - 2  ) ;
						directoryName = name.Substring ( 0, name.LastIndexOf ( "\\" )   ) ;
					}
					if ( directoryName.EndsWith ( ":" ) ) directoryName = directoryName + "\\" ;
				}
				else if ( name.EndsWith ("\\") )
				{
					directoryName = name + (string) cell.Value ;
					//directoryName = name + (string) dataGridView.SelectedCells [0] .Value ;
				}
				else
				{
					//directoryName = name +  "\\" + (string) dataGridView.SelectedCells [0] .Value;
					directoryName = name +  "\\" + (string) cell.Value;
				}
				Directories = Directory.GetDirectories ( directoryName ) ; // ?? todo : should check whether it is a directory or file before this
				ConcorDancer.Cdm.CurrentDirectory = directoryName ;
                DLLNode<String> node = new DLLNode<String>();
                node.Value = directoryName;
                CurrentDirectoryTabPage.DirectoryList.AddIfNotAlreadyPresent(node);
                //CurrentDirectoryTabPage.DirectoryList.AddIfNotAlreadyPresent(new StringNode(directoryName));
                SetTitles ( directoryName );			
                //ConcorDancer.Cdm.SetWindowTitle();
            }
			catch (  IOException ex ) // we got a file
			{
				string filename = directoryName, text ;
				ConcorDancer.Cdm.HandleException ( ex, false ) ;
				ConcorDancer.Cdm.OpenFileDialogFilterIndex = 1 ; // default : in case it is not a .rtf process it as ASCII
                ConcorDancer.Cdm.State.CurrentFileManagerTabPageAdjustDisplayGuard = true;
                ConcorDancerTabPage ctp = ConcorDancer.Cdm.SetupANewConcorDancerTabPage(filename);
                ConcorDancer.Cdm.State.CurrentFileManagerTabPageAdjustDisplayGuard = false;
                
                // This needs to be done here before ReadAFile which could take a long time before display is adjusted else
                // display looks funny while ReadAFile is processing
                ConcorDancerWinForm cdwf = ConcorDancer.Cdwf;
                text = ctp.Text;
                ctp.Text = String.Format ( "... reading {0} ...", filename ) ;
                cdwf.AdjustDisplay();
                cdwf.Refresh();
               
                ConcorDancer.Cdm.CurrentConcorDancerTabPage.ReadAFile(filename); // "directoryName" is actually a filename
                ctp.Text = text;
                ConcorDancer.Cdm.SetWindowTitle();
                //SetTitles(directoryName);

                //ConcorDancer.CurrentDataGridView = dataGridView;

                return;
			}
			catch ( Exception ex)
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
				return ;
			}
			try
			{
				Files = Directory.GetFiles ( directoryName ) ;
                TabControl.SelectedTab.Text = ConvertCurrentDirectoryNameToTabPageName ();
				dataGridView.Name = directoryName ;
				//dataGridView.Sorted = true ;
				if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
					AddDirectoryAndFilesToDataGridViewListMode (  ) ;
				else AddDirectoryAndFilesToDataGridViewDetailsMode (  ) ;
			}
			catch ( Exception ex)
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public string
		ParentDirectory ( string directory )
		{
			if ( directory [ directory.LastIndexOf ( "\\" ) - 1 ] == ':' ) 
				return directory.Substring ( 0, directory.LastIndexOf ( "\\" ) + 1 ) ;
			else return directory.Substring ( 0, directory.LastIndexOf ( "\\" ) ) ;
		}
		
		public void
		BackOrUpADirectory ()
		{
			try
			{
				string dname = ConcorDancer.CurrentDataGridView.Name ;
				//string parent = (string) currentDirectoryTabPage.directoryList.Before ;
				string parent = CurrentDirectoryTabPage.DirectoryList.CircularBefore ().Value ;
				if ( parent == null ) parent = ParentDirectory ( dname ) ;
				if ( dname != parent )
				{
					ConcorDancer.CurrentDataGridView.Name = parent ;
					GetDirectoriesAndFiles ( parent ) ;
					if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
						AddDirectoryAndFilesToDataGridViewListMode (  ) ;
					else AddDirectoryAndFilesToDataGridViewDetailsMode (  ) ;
				}
			}
			catch ( Exception ex)
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		ForwardADirectory ()
		{
			try
			{
				//string dname = (string) currentDirectoryTabPage.directoryList.After ;
				string dname = CurrentDirectoryTabPage.DirectoryList.CircularAfter ().Value ;
				ConcorDancer.CurrentDataGridView.Name = dname ;
				GetDirectoriesAndFiles ( dname ) ;
				if ( ConcorDancer.Cdwf.MultiColumnFileListMenuItem.Checked == true )
					AddDirectoryAndFilesToDataGridViewListMode (  ) ;
				else AddDirectoryAndFilesToDataGridViewDetailsMode (  ) ;
			}
			catch ( Exception ex)
			{
				ConcorDancer.Cdm.HandleException ( ex, true ) ;
			}
		}
		
		public void
		Activate () 
		{
			ConcorDancer.CurrentDataGridView = CurrentDirectoryTabPage.DataGridView ;
		}

        void
        DataGridView_MouseEnter(object sender, System.EventArgs e)
        {
            ((DataGridView)sender).Focus();
        }

        void
        TabControl_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ConcorDancerTabControl tabControl = (ConcorDancerTabControl)sender;
			CurrentDirectoryTabPage = ((DirectoryTabPage) tabControl.SelectedTab) ;
			ConcorDancer.CurrentDataGridView = CurrentDirectoryTabPage.DataGridView ;
			ConcorDancer.Cdwf.Text = Text = CurrentDirectoryTabPage.Text ;
            //ConcorDancer.Cdm.SetWindowTitle();
        }

		public 
		FileManagerTabPage  ()
		{
			TabControl.DoubleClick += new System.EventHandler ( TabControl_DoubleClick ) ;
			TabControl.SelectedIndexChanged += new System.EventHandler ( TabControl_SelectedIndexChanged ) ;
			TabControl.Appearance = ConcorDancer.PreferredTabAppearance ;
            TreeView.NodeMouseHover += new TreeNodeMouseHoverEventHandler(TreeView1_NodeMouseHover); 
            TreeView.NodeMouseClick += new TreeNodeMouseClickEventHandler(TreeView1_NodeMouseClick);
            TreeView.CheckBoxes = false;
			ConcorDancer.CurrentTreeView = TreeView ;
			TreeView.Font = ConcorDancer.TreeViewFont ;
			TreeView.BackColor = ConcorDancer.DirectoryTreeBackColor ;
			TreeView.ForeColor = ConcorDancer.TreeViewForeColor ;
			FileManagerSplitContainer.Size = new System.Drawing.Size ( 
				ConcorDancer.TabPageStartWidth - ConcorDancer.SplitterWidth, 
				ConcorDancer.TabPageStartHeight ) ;
			FileManagerSplitContainer.SplitterDistance = ( ( ConcorDancer.TabPageStartWidth - 
				ConcorDancer.SplitterWidth ) / 2 ) ;
			TreeView.Dock = DockStyle.Fill ;
			TabControl.Dock = DockStyle.Fill ;
			TabControl.Font = ConcorDancer.TabControlFont ;
			FileManagerSplitContainer.Panel1.Controls.Add ( TreeView ) ;
			FileManagerSplitContainer.Panel2.Controls.Add ( TabControl ) ;
			FileManagerSplitContainer.Dock = DockStyle.Fill ;
			FileManagerSplitContainer.Orientation = Orientation.Vertical ;
			FileManagerSplitContainer.SplitterMoved += new SplitterEventHandler ( 
				FileManagerSplitContainer_SplitterMoved);
			Controls.Add ( FileManagerSplitContainer ) ;
			
			NewFileManagerTabPageMenuItem = new MenuItem ( "New Directory Tab", new System.EventHandler (
				NewFileManagerTabPageMenuItem_Click ) ) ;
			CloseFileManagerTabPageMenuItem = new MenuItem ( "Close Directory Tab", new System.EventHandler (
				CloseFileManagerTabPageMenuItem_Click ) ) ;
			if ( ConcorDancer.Cdm.Fmtp == null )
			{
				ConcorDancer.Cdwf.functionsMenuItem.MenuItems.Add ( NewFileManagerTabPageMenuItem ) ;
				ConcorDancer.Cdwf.functionsMenuItem.MenuItems.Add ( CloseFileManagerTabPageMenuItem ) ;
			}
			ConcorDancer.Cdwf.newFileManagerMenuItem.Enabled = false ;
			
  			string [] volumes = Directory.GetLogicalDrives ( ) ;
			   // Suppress repainting the TreeView until all the objects have been created.
			TreeView.BeginUpdate();
			foreach (  string volume in volumes)
			{
				TreeNode treeNode = new TreeNode (volume) ;
				treeNode.Name = volume ;
				treeNode.Text = volume ;
				TreeView.Nodes.Add (treeNode ) ;
			}
			TreeView.EndUpdate();
			TreeView.ContextMenu = new ContextMenu () ;
			TreeView.ContextMenu.MenuItems.Add ( NewFileManagerTabPageMenuItem.CloneMenu () ) ;
			TreeView.ContextMenu.MenuItems.Add ( CloseFileManagerTabPageMenuItem.CloneMenu ()  ) ;

			DataGridView dataGridView = new DataGridView () ;
            dataGridView.MouseEnter += new EventHandler(DataGridView_MouseEnter);
			ConcorDancer.CurrentDataGridView = dataGridView ;
			FileCellStyle = dataGridView.RowsDefaultCellStyle.Clone ()  ;
			DirectoryCellStyle = dataGridView.RowsDefaultCellStyle.Clone ()  ;
			FileCellStyle.Padding = new Padding ( 0,0,0,0 )  ;
			FileCellStyle.ForeColor = ConcorDancer.FileForeColor ;
			FileCellStyle.BackColor = ConcorDancer.FileListBackColor ;
			FileCellStyle.Font = ConcorDancer.DataGridViewFileFont ;
			FileCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			DirectoryCellStyle.Padding = new Padding ( 0, 0, 0, 0 );
			DirectoryCellStyle.ForeColor = ConcorDancer.DirectoryForeColor;
			DirectoryCellStyle.BackColor = ConcorDancer.FileListBackColor ;
			DirectoryCellStyle.Font = ConcorDancer.DataGridViewDirectoryFont ;
			DirectoryCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
			/*
			fileCellStyle.ForeColor = ConcorDancer.fileForeColor;
			fileCellStyle.BackColor = ConcorDancer.fileListBackColor;
			fileCellStyle.Font = ConcorDancer.dataGridViewFileFont;
			directoryCellStyle.ForeColor = ConcorDancer.directoryForeColor;
			directoryCellStyle.BackColor = ConcorDancer.fileListBackColor;
			directoryCellStyle.Font = ConcorDancer.dataGridViewDirectoryFont;
			*/
			SizeColumnCellStyle = dataGridView.DefaultCellStyle.Clone ();
			SizeColumnCellStyle.Format = "#,#" ;
			SizeColumnCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight ;
			ModifiedTimeColumnCellStyle = dataGridView.DefaultCellStyle.Clone ()  ;
			ModifiedTimeColumnCellStyle.Format = "u" ;
			ModifiedTimeColumnCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft ;
			ModifiedTimeColumnCellStyle.Font = ConcorDancer.DataGridViewFileFont ;
			//AddDirectoryTabPage (  "C:\\" ) ;
            SavedWidth = Width;
            SavedHeight = Height;
		}
	}
}
