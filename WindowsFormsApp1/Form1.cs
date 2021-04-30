using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing.Printing;
using dotNetEdit.Properties;


namespace WindowsFormsApp1
{
    public partial class Main : Form
    {
        bool unsavedChanges = false;
        string recentFilename = "";
        string docTitle = "new";
        string verStr = "dotNetEdit 1.1.0";
        int start = 0;
        int indexOfSearchText = 0;

        public Main()
        {

            InitializeComponent();
            // this.KeyPreview = true;
            Text = verStr;            
            
            editPanel.Text = "";


        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (unsavedChanges == true)
            {
                string title = "Unsaved changes";
                string saveWarn = "Unsaved changes. Are you sure you want to exit?";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(saveWarn, title, buttons);
                if (result == DialogResult.Yes)
                {
                    this.Close();
                }
                else
                {
                   
                }
            }
            else
            {
                this.Close();
            }
        }

        //private void SettingsSaving(object sender, CancelEventArgs e)
        //{
        //    Console.WriteLine("Saving settings!");
        // }

        private void PrintDocumentOnPrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(this.editPanel.Text, this.editPanel.Font, Brushes.Black, 10, 25);
        }

        

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult printRes = printDialog1.ShowDialog();

            if (printRes == DialogResult.OK)
            {
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += PrintDocumentOnPrintPage;
                printDocument.Print();
            }
            else
            {
                

            }

            
        }




        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            saveFileDialog1.CheckFileExists = false;
            saveFileDialog1.CheckPathExists = true;
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|Text files (*.text)|*.text|Markdown files(*.md)|*.md|All files (*.*)|*.*";
            var dialogResult = saveFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string linesToWrite = editPanel.Text;
                System.IO.File.WriteAllText(saveFileDialog1.FileName, linesToWrite);
                

                var onlyFileName = System.IO.Path.GetFileName(saveFileDialog1.FileName);
                recentFilename = saveFileDialog1.FileName;
                docTitle = onlyFileName;
                this.Text = verStr + " - " + docTitle;
                unsavedChanges = false;
                toolStripLabel2.Text = "";
            }
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var dialogResult1 = openFileDialog1.ShowDialog();
            if (dialogResult1 == DialogResult.OK)
            {

                // Read the file as one string.
                editPanel.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
                unsavedChanges = false;
                var onlyFileName = System.IO.Path.GetFileName(openFileDialog1.FileName);
                docTitle = onlyFileName;
                this.Text = verStr + " - " + docTitle;
                toolStripLabel2.Text = "";

            }

        }        



        private void EditPanel_TextChanged(object sender, EventArgs e)
        {
            // this.Text = "dotNetEdit - unsaved changes";
            unsavedChanges = true;
            countCharactersToolStripMenuItem.Enabled = true;
            toolStripLabel2.Text = "Unsaved Changes";
            if (editPanel.Text != "")
            {
                Text = verStr + " - " + docTitle + " - *Unsaved Changes*";
            } 
            else
            {
                unsavedChanges = false;
                Text = verStr;
            }

        }

        private void Main_Load(object sender, EventArgs e)
        {

            //  timer 
            System.Timers.Timer autoSaveTimer = new System.Timers.Timer(5000);
            // what to do when timer elapses
            autoSaveTimer.Elapsed += OnTimedEvent;
            autoSaveTimer.AutoReset = true;
            autoSaveTimer.Enabled = true;
            // user settings from user settings
            this.editPanel.BackColor = Settings.Default.UserBackColor;
            this.editPanel.ForeColor = Settings.Default.UserForeColor;
            this.editPanel.Font = Settings.Default.UserFont;
            recentFilename = Settings.Default.UserRecentFilename;
            countCharactersToolStripMenuItem.Enabled = false;
            this.toolStripButton2.Checked = true;
            // ask if they want to work on the most recent file if it's not null
            if ( recentFilename != "null" )
            { 
            string title2 = "Resume last session?";
            string mostRecent = "Open last file?" + " : " + recentFilename;
            MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
            DialogResult result1 = MessageBox.Show(mostRecent, title2, buttons1);
            if (result1 == DialogResult.Yes)
            {
                editPanel.Text = System.IO.File.ReadAllText(recentFilename);
                unsavedChanges = false;
            }
            else
            { // do nothing }
            }
            }
        }

        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            // anything we want to do on a timer to here
        }
        private void countCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            int numChar = editPanel.Text.Length;
            int numWord = WordCounting.CountWords1(editPanel.Text);
            string numCharDisp = numChar.ToString();
            string numWordDisp = numWord.ToString();
            string charMsg = "Characters: " + numCharDisp + "\n" + "Words:" + numWordDisp;
            MessageBox.Show(charMsg,"Word Count");

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpAbout = verStr + "\n" + "Andrew Woodhouse 2020. " + "\n" + "Licensed under the GPL - use and share as you like :)";
            MessageBox.Show(helpAbout);
        }




        


        public int FindMyText(string txtToSearch, int searchStart, int searchEnd)
        {
            // Unselect the previously searched string
            if (searchStart > 0 && searchEnd > 0 && indexOfSearchText >= 0)
            {
                editPanel.Undo();
            }

            // Set the return value to -1 by default.
            int retVal = -1;

            // A valid starting index should be specified.
            // if indexOfSearchText = -1, the end of search
            if (searchStart >= 0 && indexOfSearchText >= 0)
            {
                // A valid ending index
                if (searchEnd > searchStart || searchEnd == -1)
                {
                    // Find the position of search string in RichTextBox
                    indexOfSearchText = editPanel.Find(txtToSearch, searchStart, searchEnd, RichTextBoxFinds.None);
                    // Determine whether the text was found in richTextBox1.
                    if (indexOfSearchText != -1)
                    {
                        // Return the index to the specified search text.
                        retVal = indexOfSearchText;
                    }
                }
            }
            return retVal;
        }



        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            string searchTerm = tsTxtBox.Text;
            // Debug.WriteLine("search term: " + searchTerm);

            int startindex = 0;

            if (searchTerm.Length > 0)
                startindex = FindMyText(searchTerm.Trim(), start, editPanel.Text.Length);

            // If string was found in the RichTextBox, highlight it
            if (startindex >= 0)
            {
                // Set the highlight color as red
                editPanel.SelectionColor = Color.Red;
                // Find the end index. End Index = number of characters in textbox
                int endindex = searchTerm.Length;
                // Highlight the search string
                editPanel.Select(startindex, endindex);
                // mark the start position after the position of
                // last search string
                start = startindex + endindex;
            }

        }



        private void tsTxtBox_TextChanged(object sender, EventArgs e)
        {
            start = 0;
            indexOfSearchText = 0;
        }
        

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            if (this.toolStripButton2.CheckState == CheckState.Checked)
            {                               
                editPanel.WordWrap = false;
                this.toolStripButton2.Checked = false;
                this.toolStripButton2.Text = "Word Wrap: OFF";
                Debug.WriteLine("wordwrap: OFF");
            }
            else
            if (this.toolStripButton2.CheckState == CheckState.Unchecked)
            {             
                editPanel.WordWrap = true;
                this.toolStripButton2.Checked = true;
                this.toolStripButton2.Text = "Word Wrap: ON";
                Debug.WriteLine("wordwrap : ON");
            }            
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (unsavedChanges == true)
            {
                string title = "Unsaved changes";
                string saveWarn = "Unsaved changes. Are you sure you want to start a new document?";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(saveWarn, title, buttons);
                if (result == DialogResult.Yes)
                {
                    editPanel.Clear();
                    this.Text = verStr + " - (NEW)";
                }
                else
                { // do nothing }
                }
            }
            
        }
        

        

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {            
            e.Cancel = true;            
            //change the event to avoid close form
            string title1 = "Really Quit?";
            string reallyQuit = "Are you sure you want to exit?";
            MessageBoxButtons buttons1 = MessageBoxButtons.YesNo;
            DialogResult result1 = MessageBox.Show(reallyQuit, title1, buttons1);
            if (result1 == DialogResult.Yes)
            {                
                Application.Exit();
            }
            else
            { // do nothing }
            }
            
        }

        private void fontToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FontDialog fontDlg = new FontDialog();
            fontDlg.ShowColor = false;
            fontDlg.ShowApply = true;
            fontDlg.ShowEffects = true;
            fontDlg.ShowHelp = true;
            if (fontDlg.ShowDialog() != DialogResult.Cancel)
            {
            editPanel.Font = fontDlg.Font;                
            }
        }

        private void darkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPanel.BackColor = Color.Black;
            editPanel.ForeColor = Color.Silver;
        }

        private void lightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPanel.BackColor = Color.White;
            editPanel.ForeColor = Color.Black;
        }

        private void savePreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings.Default.UserFont = editPanel.Font;
            Settings.Default.UserBackColor = editPanel.BackColor;
            Settings.Default.UserForeColor = editPanel.ForeColor;
            Settings.Default.UserRecentFilename = recentFilename;
            Settings.Default.Save();
        }

        private void defaultPreferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            editPanel.BackColor = Color.Black;
            editPanel.ForeColor = Color.Silver;
            Font epf = new Font("Consolas",12);
            editPanel.Font = epf;
            
        }

        private void monokaiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Color fgColor = (Color) System.Drawing.ColorTranslator.FromHtml("#f8f8f2");
            Color bgColor = (Color) System.Drawing.ColorTranslator.FromHtml("#272822");
            editPanel.BackColor = bgColor;
            editPanel.ForeColor = fgColor;
        }
    }

    

}


    



   
  