
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

namespace ConcorDancer
{
    public partial class
    ConcorDancerModel
    {
        public void
        HandleException (Exception ex, bool reportFlag)
        {
            ConcorDancer.HandleException (ex, reportFlag);
        }

        public string
        _ReadFileToString (string filename, Encoding textEncoding)
        {
            try
            {
                StreamReader sr = new StreamReader (filename, textEncoding); //, System.Text.Encoding.UTF7 ) ; 
                return sr.ReadToEnd ();
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
                return null;
            }
        }

        public string
        ReadFileToString (string filename)
        {
            try
            {
                string fileString;
                Encoding textEncoding;
                if (ConcorDancer.Cdm.OpenFileDialogFilterIndex == 6) // UTF-32
                {
                    textEncoding = Encoding.UTF32;
                }
                if (ConcorDancer.Cdm.OpenFileDialogFilterIndex == 5) // UTF-16 - Unicode
                {
                    textEncoding = Encoding.Unicode;
                }
                if (ConcorDancer.Cdm.OpenFileDialogFilterIndex == 4) // UTF-8
                {
                    textEncoding = Encoding.UTF8;
                }
                else if (ConcorDancer.Cdm.OpenFileDialogFilterIndex == 3) // UTF-7
                {
                    textEncoding = Encoding.UTF7;
                    fileString = _ReadFileToString (filename, Encoding.UTF7);
                    return FilterUTF7String (fileString);
                }
                else // if (  ( openFileDialog1FilterIndex == 1, 2 or 8 // ASCII, HTML and ALL -- (7 = .rtf)
                {
                    fileString = _ReadFileToString (filename, Encoding.UTF7);
                    int utf8Flag = fileString.IndexOf ("UTF-8", 0);
                    if (utf8Flag >= 0)
                    {
                        fileString = _ReadFileToString (filename, Encoding.UTF8);
                        goto checkForHtml;
                    }

                    else if (!(filename.EndsWith ("htm", true, null) ||
                        filename.EndsWith ("html", true, null)))
                    {
                        fileString = FilterUTF7String (fileString);
                        return fileString;
                    }

                    goto checkForHtml;
                }

                fileString = _ReadFileToString (filename, textEncoding);

                checkForHtml:
                if (ConcorDancer.Cdwf.RemoveHtmlTagsOptionMenuItem.Checked == true)
                {
                    if (filename.EndsWith ("htm", true, null) ||
                        filename.EndsWith ("html", true, null))
                    {
                        fileString = RemoveHtmlTags (fileString);
                    }
                }
                return fileString;
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
                return null;
            }
        }

        public void
        ReadFiles ()
        {
            //ConcorDancer.Cdm.PushNewState () ;
            try
            {
                //State.calculateAndDisplayListBox = true ;
                string[] filenames = ConcorDancer.Cdwf.OpenFileDialog.FileNames;
                foreach (string filename in filenames)
                {
                    ConcorDancerTabPage ctp = SetupANewConcorDancerTabPage (filename);
                    ctp.ReadAFile (filename);
                }
            }
            catch (Exception e)
            {
                HandleException (e, true);
            }
            ConcorDancer.Cdm.ResetState ();
        }

        public void
        GetAFile (ref StringBuilder appendString, ref int insertPoint, string filename)
        {
            try
            {
                string fileString = ReadFileToString (filename);
                if (ConcorDancer.Cdwf.insertFileNamesOnJoinOptionMenuItem.Checked == true)
                {
                    //string filename1 = ( ( filename.Length > 128 ) ? "..." : "" ) + filename.Substring (
                    //	( filename.Length > 128 ) ? ( filename.Length - 128) : 0 ) ;
                    string fileString2 = "\n\nFile - " + filename + " :\n\n" + fileString;
                    appendString.Insert (insertPoint - 1, fileString2);
                    insertPoint += fileString2.Length - 1; // insertPoint indexes a zero indexed array
                }
                else
                {
                    appendString.Insert (insertPoint - 1, fileString);
                    insertPoint += fileString.Length - 1; // insertPoint indexes a zero indexed array
                }
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
            }
        }

        public void
        GetFilesRecursively (string folderName, string genericFilename)
        {
            string dbg_f ;
            string[] filenames;
            try
            {
                filenames = Directory.GetFiles (folderName, genericFilename);

                ArrayList al = new ArrayList ();
                foreach (string filename in filenames)
                {
                    al.Add (filename);
                }
                //al.Sort(new AlphabeticalSortComparer());
                al.Sort (new CaseInsensitiveComparer ()); // so files are alphabetical in the final text
                foreach (string filename in al)
                {
                    dbg_f = filename;
                    GetAFile (ref AppendString, ref InsertPoint, filename);
                }
                string[] directories = Directory.GetDirectories (folderName);
                foreach (string directory in directories)
                {
                    if (ConcorDancer.Cdwf.RecurseFoldersOptionMenuItem.Checked == true)
                    {
                        GetFilesRecursively (directory, genericFilename);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
            }
        }

        public long
        GetFileTotalLengthRecursively (string folderName, string genericFilename)
        {
            try
            {
                long length = 0;
                string[] directories = Directory.GetDirectories (folderName);
                foreach (string directory in directories)
                {
                    string[] lFilenames = Directory.GetFiles (directory, genericFilename);
                    foreach (string filename in lFilenames)
                    {
                        FileInfo fi = new FileInfo (filename);
                        length += fi.Length + filename.Length + 16; // 16 : padding for wrapper - "\n\nFile - " " :\n\n" 
                    }
                    if (ConcorDancer.Cdwf.RecurseFoldersOptionMenuItem.Checked == true)
                    {
                        length += GetFileTotalLengthRecursively (directory, genericFilename);
                    }
                }
                return length;
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
                return (long)0;
            }
        }

        // Open Folder
        public void
        OpenFolderMenuItem_Click ()
        {
            try
            {
                ConcorDancerTabPage ctp;

                string genericFilename = ConcorDancer.Cdwf.ComboBoxForFind.Text;
                //folderBrowserDialog1.RootFolder = "." ;
                if (genericFilename == "")
                {
                    ConcorDancer.Cdwf.FolderBrowserDialog.Description = String.Format (
                        "Get files with default description : *.*htm*.  " +
                        "You can change the default by typing a description in the Find (left) box" +
                        " before selecting the menu item to Join Files.");
                    genericFilename = "*.*htm*";
                }
                else
                {
                    ConcorDancer.Cdwf.FolderBrowserDialog.Description = String.Format (
                        "Get files with description : {0}", ConcorDancer.Cdwf.ComboBoxForFind.Text);
                }
                if (ConcorDancer.Cdwf.FolderBrowserDialog.ShowDialog () == DialogResult.OK)
                {
                    Refresh ();
                    InsertPoint = 1; // 1 : adjust for first time thru GetAFile
                    string folderName = ConcorDancer.Cdwf.FolderBrowserDialog.SelectedPath;
                    if (genericFilename != "")
                    {
                        // This needs to be done here before ReadAFile which could take a long time before display is adjusted else
                        // display looks funny while ReadAFile is processing
                        ctp = SetupANewConcorDancerTabPage (folderName);
                        ctp.Text = String.Format ("... reading {0} ...", folderName);
                        Refresh ();

                        try
                        {
                            long length = GetFileTotalLengthRecursively (folderName, genericFilename);
                            AppendString = new StringBuilder ((int)length);
                            GetFilesRecursively (folderName, genericFilename);
                        }
                        catch (Exception ex)
                        {
                            HandleException (ex, false);
                        }
                    }
                    else return;
                    // TODO : we shouldn't have to do this but...
                    int asl = AppendString.Length;
                    for (int i = 0 ; i < asl ; i++)
                    {
                        if (AppendString[i] == 0)
                        {
                            AppendString[i] = ' ';
                        }
                    }
                    //
                    //ConcorDancerTabPage ctp = SetupANewConcorDancerTabPage ( folderName ) ;
                    ctp.Text = ConcorDancer.MakeFilenameFromFullPathName (folderName);
                    ctp.TextBox.Text = AppendString.ToString (0, InsertPoint - 1);
                    ctp.ListBox.TextBoxText = ctp.TextBox.Text;
                    ctp.TextBox.TextChanged += new EventHandler (
                        ctp.TextBox_TextChanged);
                    //ctp.ConcorDancerSortComparer.Text = ctp.textBox1.Text ;
                    ConcorDancer.Cdm.CurrentConcorDancerTabPage = ctp; // triggers ConcorDancer.Cdwf.TabControl1_SelectedIndexChanged
                    //AdjustDisplay () ;
                    SetWindowTitle ();
                    //ConcorDancer.Cdm.Refresh();
                }
            }
            catch (Exception ex)
            {
                HandleException (ex, true);
            }
        }

        int
        MoveIndexToEndOfNextEndTag (ref int i, ref int fileLength, ref string htmlFileData, char tagStartChar)
        {
            while (true)
            {
                i = htmlFileData.IndexOf ("</", i) + 2;
                // ?? only check to see if tag has correct first letter
                if (Char.ToLower (htmlFileData[i]) != tagStartChar) continue;
                else { i = htmlFileData.IndexOf ('>', i); break; }
            }
            return i;
        }

        void
        FilterUTF7Character (char utfChar, ref char[] sb, ref int sbIndex, int filterNewlineFlag)
        {
            switch (utfChar)
            {

                case (char)0x0d:
                case (char)0x0a:
                {
                    if (filterNewlineFlag != 0)
                    {
                        sb[sbIndex] = ' ';
                    }
                    else sb[sbIndex] = utfChar;
                    sbIndex++;
                    break;
                }
                case (char)0x97: // big dash
                {
                    sb[sbIndex] = '-';
                    sbIndex++;
                    sb[sbIndex] = '-';
                    sbIndex++;
                    break;
                }
                case (char)0x96: //
                {
                    sb[sbIndex] = ' ';
                    sbIndex++;
                    sb[sbIndex] = '-';
                    sbIndex++;
                    sb[sbIndex] = ' ';
                    sbIndex++;
                    break;
                }
                case (char)0xa2: // 
                case (char)0xa6: // 
                {
                    sb[sbIndex] = '-';
                    sbIndex++;
                    break;
                }
                case (char)0x93: // 
                {
                    sb[sbIndex] = '“';
                    sbIndex++;
                    break;
                }
                case (char)0x94: // 
                {
                    sb[sbIndex] = '”';
                    sbIndex++;
                    break;
                }
                case (char)0x80:
                case (char)0xe3:
                case (char)0xe2:
                case (char)0xc3:
                case (char)0xc2:
                case (char)0xca:
                {
                    break;
                }
                case (char)0x85: // elipsis
                {
                    sb[sbIndex] = '.';
                    sbIndex++;
                    sb[sbIndex] = '.';
                    sbIndex++;
                    sb[sbIndex] = '.';
                    sbIndex++;
                    break;
                }
                case (char)0xed: // right single quote
                case (char)0xbc: // right single quote
                case (char)0x91: // left single quote
                case (char)0x92: // right single quote
                case (char)0x98: // right single quote
                case (char)0x99: // right single quote
                case (char)0x9c: // right single quote
                case (char)0x9d: // right single quote
                {
                    //sb[sbIndex] = utf7FileData[i];
                    sb[sbIndex] = '\''; // utf7FileData[i];
                    sbIndex++;
                    break;
                }
                default:
                {
                    if (utfChar >= ' ')
                    {
                        //addChar :
                        sb[sbIndex] = utfChar;
                        sbIndex++;
                    }
                    break;
                }
            }
        }

        string
        RemoveHtmlTags (string htmlFileData)
        {
            int fileLength = htmlFileData.Length;
            char[] sb = new char[fileLength + 40000];
            int sbIndex = 0;
            try
            {
                // try to skip to "<body>"
                int i = htmlFileData.IndexOf ("<body", 0);
                if (i == -1)
                {
                    i = htmlFileData.IndexOf ("<BODY", 0);
                }
                if (i == -1) i = 0; // xml filetype
                else
                {
                    i = htmlFileData.IndexOf (">", i) + 1;
                }
                for ( ; i < fileLength ; i++)
                {
                    //string dbg = htmlFileData.Substring ( i, 100 ) ;
                    // skip most html tags : except <p>, <br>, </body>

                    //char debug = htmlFileData[i];
                    //switch ( debug )
                    switch (htmlFileData[i])
                    {
                        case '<':
                        // we're in a tag within <body>
                        {
                            switch (htmlFileData[i + 1])
                            {
                                case 'p':
                                case 'P':
                                // substitute newline tab space for <p>
                                {
                                    sb[sbIndex] = (char)0x0d;
                                    sbIndex++;
                                    sb[sbIndex] = (char)0x0a;
                                    sbIndex++;
                                    sb[sbIndex] = '\t';
                                    sbIndex++;
                                    sb[sbIndex] = ' ';
                                    sbIndex++;
                                    i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                                case 'b':
                                case 'B':
                                {
                                    if (((htmlFileData[i + 2] == 'r')
                                        || (htmlFileData[i + 2] == 'R')))
                                    // substitute newline for <br>
                                    {
                                        sb[sbIndex] = (char)0x0d;
                                        sbIndex++;
                                        sb[sbIndex] = (char)0x0a;
                                        sbIndex++;
                                    }
                                    i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                                case 's':
                                case 'S':
                                {
                                    // <script ...>
                                    if (((htmlFileData[i + 2] == 'c')
                                        || (htmlFileData[i + 2] == 'C')))
                                    {
                                        i = MoveIndexToEndOfNextEndTag (ref i, ref fileLength,
                                            ref htmlFileData, 's');
                                        break;
                                    }
                                    // <style
                                    else if (((htmlFileData[i + 2] == 't')
                                        || (htmlFileData[i + 2] == 'T')))
                                    {
                                        if (((htmlFileData[i + 3] == 'y')
                                            || (htmlFileData[i + 3] == 'Y')))
                                        {
                                            i = MoveIndexToEndOfNextEndTag (ref i, ref fileLength,
                                                ref htmlFileData, 's');
                                        }
                                        else i = htmlFileData.IndexOf ('>', i);
                                        break;
                                    }

                                    else i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                                case 'a':
                                case 'A':
                                {
                                    if (ConcorDancer.Cdwf.RemoveHtmlAnchorsOptionMenuItem.Checked == true)
                                    // remove all anchored text
                                    {
                                        MoveIndexToEndOfNextEndTag (ref i, ref fileLength, ref htmlFileData, 'a');
                                    }
                                    else i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                                case '/':
                                // end tag
                                {
                                    if ((htmlFileData[i + 2] == 'b')
                                        & (htmlFileData[i + 3] == 'o'))
                                    // </body> tag - we're done; prevent any characters at the end of file which would screw us up
                                    {
                                        return FilterBlankSpace (sb, sbIndex);
                                        //return new string ( sb, 0, sbIndex ) ;
                                    }
                                    i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                                case '!':
                                {
                                    if ((htmlFileData[i + 2] == '-')
                                        & (htmlFileData[i + 3] == '-'))
                                    // commented out sections
                                    {
                                        i = htmlFileData.IndexOf ("-->", i + 4) + 2;
                                    }
                                    break;
                                }
                                default:
                                // skip the tag
                                {
                                    i = htmlFileData.IndexOf ('>', i);
                                    break;
                                }
                            }
                            break;
                        }
                        case '&':
                        // http://www.1stsitefree.com/special_characters.htm
                        // substitute " for '&quot;' , '' for '&nbsp;', etc.
                        {
                            char type = htmlFileData[i + 1];
                            switch (type)
                            {
                                case 'q':
                                {
                                    sb[sbIndex] = '"'; // replace with "
                                    sbIndex++;
                                    break;
                                }
                                case 'n':
                                {
                                    sb[sbIndex] = ' ';
                                    sbIndex++;
                                    break;
                                }
                                case 'a':
                                {
                                    sb[sbIndex] = '&'; // ampersand
                                    sbIndex++;
                                    break;
                                }
                                case 'c':
                                {
                                    if (htmlFileData.Substring (i + 1, 4) == "copy")
                                    {
                                        sb[sbIndex] = '©';
                                        sbIndex++;
                                    }
                                    break;
                                }
                                case 't':
                                {
                                    if (htmlFileData.Substring (i + 2, 4) == "rade")
                                    {
                                        sb[sbIndex] = '™';
                                        sbIndex++;
                                    }
                                    break;
                                }
                                case 'r': // right angle quote
                                {
                                    switch (htmlFileData.Substring (i + 2, 2))
                                    {
                                        case "aq":
                                        {
                                            sb[sbIndex] = '»'; // ampersand
                                            sbIndex++;
                                            break;
                                        }
                                        case "sq":
                                        {
                                            sb[sbIndex] = '’'; // right single quote
                                            sbIndex++;
                                            break;
                                        }
                                        case "dq":
                                        {
                                            sb[sbIndex] = '”'; // right double quote
                                            sbIndex++;
                                            break;
                                        }
                                        case "eg":
                                        {
                                            sb[sbIndex] = '®'; // registered trademark
                                            sbIndex++;
                                            break;
                                        }

                                        default:
                                        {
                                            sb[sbIndex] = htmlFileData[i];
                                            sbIndex++;
                                            continue; // eg. &copy : copyright
                                        }
                                    }
                                    break;
                                }
                                case 'l': // right angle quote
                                {
                                    switch (htmlFileData.Substring (i + 2, 4))
                                    {
                                        case "aquo":
                                        {
                                            sb[sbIndex] = '«'; // ampersand
                                            sbIndex++;
                                            break;
                                        }
                                        case "squo":
                                        {
                                            sb[sbIndex] = '‘'; // ampersand
                                            sbIndex++;
                                            break;
                                        }
                                        case "dquo":
                                        {
                                            sb[sbIndex] = '“'; // ampersand
                                            sbIndex++;
                                            break;
                                        }

                                        default:
                                        {
                                            sb[sbIndex] = htmlFileData[i];
                                            sbIndex++;
                                            continue; // eg. &copy : copyright
                                        }
                                    }
                                    break;
                                }
                                case 'm':
                                {
                                    switch (htmlFileData.Substring (i + 2, 4))
                                    {
                                        case "dash": //mdash
                                        {
                                            sb[sbIndex] = '—';
                                            sbIndex++;
                                            break;
                                        }
                                        case "iddo": // middot
                                        {
                                            sb[sbIndex] = '·';
                                            sbIndex++;
                                            break;
                                        }
                                        default:
                                        {
                                            sb[sbIndex] = htmlFileData[i];
                                            sbIndex++;
                                            continue; // eg. &copy : copyright
                                        }
                                    }
                                    //i = htmlFileData.IndexOf(';', i);
                                    break;
                                }
                                case '#':
                                {
                                    switch (htmlFileData.Substring (i + 2, 3))
                                    {
                                        case "146":
                                        {
                                            sb[sbIndex] = '\'';
                                            sbIndex++;
                                            break;
                                        }
                                        case "147":
                                        {
                                            sb[sbIndex] = '\"';
                                            sbIndex++;
                                            break;
                                        }
                                        case "148":
                                        {
                                            sb[sbIndex] = '\"';
                                            sbIndex++;
                                            break;
                                        }
                                        case "151":
                                        {
                                            sb[sbIndex] = '-';
                                            sbIndex++;
                                            sb[sbIndex] = '-';
                                            sbIndex++;
                                            break;
                                        }
                                        default: break;
                                    }
                                    break;
                                }
                                //default: break;
                                default:
                                {
                                    sb[sbIndex] = htmlFileData[i];
                                    sbIndex++;
                                    continue; // eg. &copy : copyright
                                }
                            }
                            i = htmlFileData.IndexOf (';', i);
                            break;
                        }
                        case '.':
                        // oddity with urantia webpages
                        {
                            if (((i + 2) < fileLength) && (
                                (htmlFileData[i + 1] == 'c') & (htmlFileData[i + 2] == '.')))
                            // some kind of chapter heading code - skip it all
                            {
                                i = htmlFileData.IndexOf (';', i);
                            }
                            else
                            {
                                //goto addChar ;
                                sb[sbIndex] = htmlFileData[i];
                                sbIndex++;
                            }
                            break;
                        }
                        case ' ':
                        {
                            if (((i + 4) < fileLength)
                                && (((htmlFileData[i + 1] == 13) | (htmlFileData[i + 2] == 10))
                                & (htmlFileData[i + 3] == ' ')
                                & (htmlFileData[i + 4] == ' ')))
                            // this pattern occurs alot in the UB
                            {
                                i += 3; // keep the last space
                            }
                            else
                            {
                                //goto addChar;
                                sb[sbIndex] = htmlFileData[i];
                                sbIndex++;
                            }
                            break;
                        }
                        case '|':
                        case '~':
                        {
                            break; // not added 
                        }
                        default:
                        {
                            FilterUTF7Character (htmlFileData[i], ref sb, ref sbIndex, 0);
                            break;
                        }
                    }
                }
                return FilterBlankSpace (sb, sbIndex);
            }
            catch (Exception e)
            {
                HandleException (e, true);
                return (string)null;
            }
        }

        string
        FilterUTF7String (string utf7String)
        {
            int fileLength = utf7String.Length;
            char[] sb = new char[fileLength + 1000000];
            int i, sbIndex = 0;
            try
            {
                for (i = 0 ; i < fileLength ; i++)
                {
                    FilterUTF7Character (utf7String[i], ref sb, ref sbIndex, 0);
                }
                return FilterBlankSpace (sb, sbIndex);
            }
            catch (Exception e)
            {
                HandleException (e, true);
                return (string)null;
            }
        }

        public string
        FilterBlankSpace (char[] sb, int sbIndex)
        {
            int j = 0, k = 0;
            char[] sb1 = new char[sbIndex];
            string ns = null;
            try
            {
                while (k < sbIndex)
                {
                    while (sb[k] == (char)0) k++;
                    if ((sb[k] == (char)0x0d) && (sb[k + 2] == (char)0x0d) && (sb[k + 4] == (char)0x0d))
                    {
                        k += 2;
                    }
                    else if ((sb[k] == ' ') && (sb[k + 1] == ' ') && (sb[k + 2] == ' '))
                    {
                        k++;
                    }
                    else
                    {
                        sb1[j] = sb[k];
                        j++; k++;
                    }
                }
                if (j + 1 <= sbIndex) j++;
                else j = sbIndex;
                ns = new string (sb1, 0, j);
            }
            catch (Exception e)
            {
                HandleException (e, true);
            }
            return ns;
        }
        /*
                public void
                RestoreState ()
                {
                    try
                    {
                        foreach ( ConcorDancerTabPage ctp in ConcorDancerTabPageList.List )
                        {
                            ctp.RestoreState () ;
                        }
                        ConcorDancerTabPage ctp = ConcorDancer.Cdm.CurrentConcorDancerTabPage ;
                        ctp.textBox1.Text = ctp.Text () ;
                        ctp.textBox1.Modified = false ; // loading is not 'Modifying'
                        ConcorDancer.Cdm.SetWindowTitle () ;
                        ctp._CalculateAndDisplayListBoxStringArray() ;
                        //setButtonText () ;
                    }
                    catch ( Exception e )
                    {
                        HandleException ( e, true ) ;
                    }
                }
        */
    }
}
