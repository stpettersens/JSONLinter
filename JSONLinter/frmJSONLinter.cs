using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualBasic;
using System.Windows.Forms;
using ScintillaNET;

namespace JSONLinter
{

    public partial class frmJSONLinter : Form
    {
        private string[] arguments;
        private string inputFile;
        private string outputFile;
        private bool saveInPlace;
        private bool compactError;
        private bool fileChanged;
        private string indentChar;
        private frmAbout frmAbout;
        private frmLogWindow frmLogWindow;
        private const string version = "1.0";

        public frmJSONLinter()
        {
            InitializeComponent();
            arguments = new string[] { "file.json", "", "", "", "", "" };
            scintilla.Margins[0].Width = 30; // Show line numbers.
            scintilla.Lexer = Lexer.Cpp;
            scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Properties.Settings.Default.CommentLine;
            scintilla.Styles[Style.Cpp.String].ForeColor = Properties.Settings.Default.String;
            scintilla.Styles[Style.Cpp.Character].ForeColor = Properties.Settings.Default.String;
            scintilla.Styles[Style.Cpp.Number].ForeColor = Properties.Settings.Default.Number;
            scintilla.Styles[Style.Cpp.Operator].ForeColor = Properties.Settings.Default.Operator;
            scintilla.Styles[Style.Cpp.Identifier].ForeColor = Properties.Settings.Default.Boolean;
            inputFile = "_jsonlinter.json";
            outputFile = "";
            saveInPlace = false;
            compactError = false;
            fileChanged = false;
            indentChar = Properties.Settings.Default.DefaultIntent;
            arguments[1] = "-t " + indentChar;
            frmAbout = new frmAbout();
            frmLogWindow = new frmLogWindow();
        }

        private void toggleSaveOption()
        {
            if (scintilla.Text.Length > 0)
            {
                saveToolStripMenuItem.Enabled = true;
                fileChanged = true;
            }
            else
            {
                saveToolStripMenuItem.Enabled = false;
                saveAsToolStripMenuItem.Enabled = false;
                fileChanged = false;
            }
        }

        private void reset()
        {
            inputFile = "_jsonlinter.json";
            scintilla.Text = "";
            this.Text = "JSONLinter";
            grpConsole.Text = "JSON:";
            btnProcess.Text = "Process &JSON";
            lblStatus.Text = "";
            toolStripStatLbl.Text = "Created new file";
        }

        private void saveFile(string file)
        {
            if(file != "_jsonlinter.json")
            {
                this.Text = "JSONLinter - " + file;
                toolStripStatLbl.Text = "Saved " + file;
            }
            StreamWriter sw = new StreamWriter(file);
            sw.Write(scintilla.Text);
            sw.Close();
            fileChanged = false;
        }

        private void convertCSONtoJSON(string file)
        {
            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "node.exe",
                    Arguments = "node_modules/cson-cli/bin/cson2json dummy.cson > dummy.json",
                    UseShellExecute = false,
                    CreateNoWindow = false
                }
            };
            MessageBox.Show(proc.StartInfo.Arguments);
            proc.Start();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            /*if(inputFile.EndsWith(".cson"))
            {
                convertCSONtoJSON(inputFile);
            }*/

            arguments[0] = inputFile;
            if(inputFile == "_jsonlinter.json" || saveInPlace)
            {
                saveFile(inputFile);
                if (saveInPlace) toolStripStatLbl.Text = "Overwrote " + inputFile;
            }

            Process proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "node_modules/.bin/jsonlint.cmd",
                    Arguments = String.Join(" ", arguments),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            exitToolStripMenuItem.Enabled = false;
            proc.Start();
            lblStatus.Text = "";
            lblStatus.BackColor = Properties.Settings.Default.validBackground;
            lblStatus.ForeColor = Properties.Settings.Default.validForeground;
            lblStatus.Text = "Valid JSON!";
            bool cleared = false;
            string output = "";
            string error = "";
            while (!proc.StandardOutput.EndOfStream)
            {
                if (!cleared)
                {
                    scintilla.Text = "";
                    cleared = true;
                }
                output = proc.StandardOutput.ReadLine();
                scintilla.AppendText(output + "\n");
            }
            while (!proc.StandardError.EndOfStream)
            {
                lblStatus.BackColor = Properties.Settings.Default.invalidBackground;
                lblStatus.ForeColor = Properties.Settings.Default.invalidForeground;

                error = proc.StandardError.ReadLine();
                if (compactError)
                {
                    error = error.Replace("found:", "found:\n");
                }
                lblStatus.Text = "Invalid JSON:\n" + error;    
            }

            frmLogWindow.logEvent("jsonlint " + proc.StartInfo.Arguments, error);

            if(saveInPlace)
            {
                string[] lines = File.ReadAllLines(inputFile);
                scintilla.Text = "";
                foreach(string line in lines)
                {
                    scintilla.AppendText(line + "\n");
                }
            }
            exitToolStripMenuItem.Enabled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(scintilla.Text.Length > 0)
            {
                DialogResult result = MessageBox.Show(
                "Do you want to create a new file?\nExisting buffer will be cleared.",
                "New file", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(result == DialogResult.Yes)
                {
                    reset();
                }
            }
            else
            {
                reset();
            }
        }

        private void inputFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            { 
                reset();
                inputFile = openFileDialog.FileName;
                string[] lines = File.ReadAllLines(inputFile);
                foreach(string line in lines) 
                {
                    scintilla.AppendText(line + "\n");
                }

                overwriteFileInplaceToolStripMenuItem.Enabled = true;
                saveToolStripMenuItem.Enabled = true;
                saveAsToolStripMenuItem.Enabled = true;

                if(inputFile != "_jsonlinter.json")
                {
                    this.Text = "JSONLinter - " + inputFile;
                }

                if(inputFile.EndsWith(".cson")) 
                {
                    grpConsole.Text = "CSON:";
                    btnProcess.Text = "Process as &JSON";
                }
                toolStripStatLbl.Text = "Opened " + inputFile;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (inputFile == "_jsonlinter.json")
            {
                saveAsToolStripMenuItem_Click(sender, e);
            }
            else
            {
                saveFile(inputFile);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = saveFileDialog.ShowDialog();
            if(result == DialogResult.OK)
            {
                outputFile = saveFileDialog.FileName;
                saveFile(outputFile);
                inputFile = outputFile;
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (scintilla.Text.Length > 0)
            {
                Clipboard.SetText(scintilla.Text);
                toolStripStatLbl.Text = "Copied buffer text";
            }
        }

        private void pasteFromClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
            scintilla.Text = Clipboard.GetText();
            saveFile(inputFile);
            overwriteFileInplaceToolStripMenuItem.Enabled = false;
            toolStripStatLbl.Text = "Pasted text to buffer";
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout.ShowDialog();
        }

        private void logWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmLogWindow.ShowDialog();
        }

        private void setCharactersForindentationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            indentChar = Interaction.InputBox("Set character(s) for indentation:", 
            "Set indentation character(s)", indentChar);
            if(indentChar.Length > 0)
            {
                arguments[1] = "-t " + indentChar;
            }
            else
            {
                arguments[1] = "";
            }
        }

        private void sortObjectKeyssToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sortObjectKeyssToolStripMenuItem.Checked)
            {
                sortObjectKeyssToolStripMenuItem.Checked = false;
                arguments[2] = "";
            }
            else
            {
                sortObjectKeyssToolStripMenuItem.Checked = true;
                arguments[2] = "-s";
            }
        }

        private void overwriteFileInplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (overwriteFileInplaceToolStripMenuItem.Checked)
            {
                overwriteFileInplaceToolStripMenuItem.Checked = false;
                saveInPlace = false;
                arguments[3] = "";
            }
            else
            {
                overwriteFileInplaceToolStripMenuItem.Checked = true;
                inputFile = openFileDialog.FileName;
                saveInPlace = true;
                arguments[3] = "-i";
            }
        }

        private void compactErrorDisplayccompactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (compactErrorDisplayccompactToolStripMenuItem.Checked)
            {
                compactErrorDisplayccompactToolStripMenuItem.Checked = false;
                compactError = false;
                arguments[4] = "";
            }
            else
            {
                compactErrorDisplayccompactToolStripMenuItem.Checked = true;
                compactError = true;
                arguments[4] = "-c";
            }
        }

        private void prettyPrintpprettyprintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (prettyPrintpprettyprintToolStripMenuItem.Checked)
            {
                prettyPrintpprettyprintToolStripMenuItem.Checked = false;
                arguments[5] = "";
            }
            else
            {
                prettyPrintpprettyprintToolStripMenuItem.Checked = true;
                arguments[5] = "-p";
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            reset();
        }

        private void scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            toggleSaveOption();
        }

        private void scintilla_Delete(object sender, ModificationEventArgs e)
        {
            toggleSaveOption();
        }

        private void scintilla_Insert(object sender, ModificationEventArgs e)
        {
            toggleSaveOption();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if((scintilla.Text.Length > 0) && outputFile == "" && fileChanged)
            {
                DialogResult result = MessageBox.Show(
                "Do you really want to exit?\nCurrent buffer will be lost.",
                "Exit?", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if(result == DialogResult.OK)
                {
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void frmJSONLinter_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (File.Exists("_jsonlinter.json"))
            {
                File.Delete("_jsonlinter.json");
            }
        }
    }
}
