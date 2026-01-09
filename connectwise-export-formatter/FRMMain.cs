using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace connectwise_export_formatter
{
    public partial class FRMMain : Form
    {
        public FRMMain()
        {
            InitializeComponent();
            InitializeDefaults();
        }

        private void InitializeDefaults()
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var defaultInput = Path.Combine(baseDir, "export.json");
            txtInput.Text = defaultInput;
            txtOutput.Text = Path.Combine(baseDir, "output_pdfs");
        }

        private void btnBrowseInput_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                ofd.Title = "Select export.json";
                if (File.Exists(txtInput.Text))
                {
                    ofd.InitialDirectory = Path.GetDirectoryName(txtInput.Text);
                    ofd.FileName = Path.GetFileName(txtInput.Text);
                }

                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    txtInput.Text = ofd.FileName;
                    var inputDir = Path.GetDirectoryName(ofd.FileName) ?? AppDomain.CurrentDomain.BaseDirectory;
                    if (!string.IsNullOrWhiteSpace(txtOutput.Text))
                    {
                        txtOutput.Text = Path.Combine(inputDir, "output_pdfs");
                    }
                }
            }
        }

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select output folder";
                if (Directory.Exists(txtOutput.Text))
                {
                    fbd.SelectedPath = txtOutput.Text;
                }

                if (fbd.ShowDialog(this) == DialogResult.OK)
                {
                    txtOutput.Text = fbd.SelectedPath;
                }
            }
        }

        private async void btnRun_Click(object sender, EventArgs e)
        {
            var inputPath = txtInput.Text.Trim();
            var outputDir = txtOutput.Text.Trim();

            if (!File.Exists(inputPath))
            {
                MessageBox.Show(this, "Input file not found.", "Missing file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(outputDir))
            {
                MessageBox.Show(this, "Choose an output folder.", "Output required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Directory.CreateDirectory(outputDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Unable to create output folder: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ToggleUi(false);
            txtLog.Clear();
            AppendLog("Writing embedded script...");
            var scriptPath = Program.WriteScriptToTemp();
            AppendLog("Script ready: " + scriptPath);

            var workingDir = Path.GetDirectoryName(inputPath) ?? AppDomain.CurrentDomain.BaseDirectory;

            int exitCode = -1;
            bool started = false;
            string pythonUsed = null;

            try
            {
                await Task.Run(() =>
                {
                    started = Program.RunWithAnyPython(
                        scriptPath,
                        workingDir,
                        inputPath,
                        outputDir,
                        AppendLog,
                        AppendLog,
                        out exitCode,
                        out pythonUsed);
                }).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                AppendError("Unexpected error: " + ex.Message);
            }
            finally
            {
                ToggleUi(true);
            }

            if (!started)
            {
                const string help = "Python 3 with reportlab is required.\nGet Python: https://www.python.org/downloads/\nThen run: pip install reportlab";
                AppendError(help);
                MessageBox.Show(this, help, "Python/reportlab missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (exitCode == 0)
            {
                if (!string.IsNullOrWhiteSpace(pythonUsed))
                {
                    AppendLog("Ran with: " + pythonUsed);
                }
                AppendLog("Done. PDFs are in: " + outputDir);
                MessageBox.Show(this, "Finished generating PDFs.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                AppendError("Script exited with code " + exitCode);
                MessageBox.Show(this, "Script exited with code " + exitCode + ". See log for details.", "Script error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOpenOutput_Click(object sender, EventArgs e)
        {
            var outputDir = txtOutput.Text.Trim();
            if (Directory.Exists(outputDir))
            {
                Process.Start("explorer.exe", outputDir);
            }
            else
            {
                MessageBox.Show(this, "Output folder does not exist yet.", "Not found", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ToggleUi(bool enabled)
        {
            btnBrowseInput.Enabled = enabled;
            btnBrowseOutput.Enabled = enabled;
            btnRun.Enabled = enabled;
            btnOpenOutput.Enabled = enabled;
        }

        private void AppendLog(string message)
        {
            AppendToLog(message);
        }

        private void AppendError(string message)
        {
            AppendToLog("ERROR: " + message);
        }

        private void AppendToLog(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<string>(AppendToLog), message);
                return;
            }

            var line = "[" + DateTime.Now.ToString("HH:mm:ss") + "] " + message + Environment.NewLine;
            txtLog.AppendText(line);
        }

        private void FRMMain_Load(object sender, EventArgs e)
        {

        }

        private void txtInput_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void txtInput_DragDrop(object sender, DragEventArgs e)
        {
            var paths = e.Data?.GetData(DataFormats.FileDrop) as string[];
            if (paths == null || paths.Length == 0)
            {
                return;
            }

            var first = paths[0];
            if (!File.Exists(first))
            {
                return;
            }

            txtInput.Text = first;

            var inputDir = Path.GetDirectoryName(first) ?? AppDomain.CurrentDomain.BaseDirectory;
            if (!string.IsNullOrWhiteSpace(inputDir))
            {
                txtOutput.Text = Path.Combine(inputDir, "output_pdfs");
            }
        }

        private void LBLLicense_Click(object sender, EventArgs e)
        {
            try
            {
                var assembly = typeof(Program).Assembly;
                using (var stream = assembly.GetManifestResourceStream("connectwise_export_formatter.LICENSE.txt"))
                {
                    if (stream == null)
                    {
                        MessageBox.Show(this, "License file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var path = Path.GetTempFileName() + ".txt";
                    using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                    {
                        stream.CopyTo(fileStream);
                    }

                    var psi = new ProcessStartInfo
                    {
                        FileName = "notepad.exe",
                        Arguments = "\"" + path + "\"",
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Error opening license: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
