using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace connectwise_export_formatter
{
    internal static class Program
    {
        internal const string ScriptFileName = "cw_export_formatter.py";
        internal const string LogoFileName = "logo.png";
        internal const string EmbeddedResourceName = "connectwise_export_formatter.original.py";
        internal const string EmbeddedLogoResourceName = "connectwise_export_formatter.logo.png";
        private static string _cachedPython;

        internal static readonly string[] PythonCandidates = { "python", "py", "python3" };

        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FRMMain());
        }

        internal static string WriteScriptToTemp()
        {
            var tempDir = Path.GetTempPath();
            var scriptPath = Path.Combine(tempDir, ScriptFileName);
            var script = ReadEmbeddedPython();
            File.WriteAllText(scriptPath, script, Encoding.UTF8);
            
            // Extract logo.png to the same temp directory
            var logoPath = Path.Combine(tempDir, LogoFileName);
            ExtractEmbeddedLogo(logoPath);
            
            return scriptPath;
        }

        internal static string ReadEmbeddedPython()
        {
            if (_cachedPython != null)
            {
                return _cachedPython;
            }

            var assembly = typeof(Program).Assembly;
            using (var stream = assembly.GetManifestResourceStream(EmbeddedResourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("Embedded Python script not found: " + EmbeddedResourceName);
                }

                using (var reader = new StreamReader(stream, Encoding.UTF8))
                {
                    _cachedPython = reader.ReadToEnd();
                    return _cachedPython;
                }
            }
        }

        internal static void ExtractEmbeddedLogo(string outputPath)
        {
            var assembly = typeof(Program).Assembly;
            using (var stream = assembly.GetManifestResourceStream(EmbeddedLogoResourceName))
            {
                if (stream == null)
                {
                    // Logo is optional, so just return without error
                    return;
                }

                using (var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
            }
        }

        internal static bool RunWithAnyPython(
            string scriptPath,
            string workingDir,
            string inputPath,
            string outputDir,
            Action<string> info,
            Action<string> error,
            out int exitCode,
            out string pythonUsed)
        {
            if (!TryFindPythonWithReportlab(info, error, out pythonUsed))
            {
                exitCode = -1;
                return false;
            }

            if (TryRunPython(pythonUsed, scriptPath, workingDir, inputPath, outputDir, info, error, out exitCode))
            {
                return true;
            }

            pythonUsed = null;
            exitCode = -1;
            return false;
        }

        private static bool TryFindPythonWithReportlab(Action<string> info, Action<string> error, out string pythonPath)
        {
            foreach (var candidate in PythonCandidates)
            {
                if (CanImportReportlab(candidate, info, error, out pythonPath))
                {
                    return true;
                }
            }

            pythonPath = null;
            return false;
        }

        private static bool CanImportReportlab(string pythonExe, Action<string> info, Action<string> error, out string resolvedPath)
        {
            var psi = new ProcessStartInfo(pythonExe, "-c \"import reportlab, sys; print(sys.executable)\"")
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        resolvedPath = null;
                        return false;
                    }

                    var stdout = new StringBuilder();
                    var stderr = new StringBuilder();

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            stdout.AppendLine(e.Data);
                        }
                    };
                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            stderr.AppendLine(e.Data);
                        }
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        resolvedPath = stdout.ToString().Trim();
                        if (string.IsNullOrWhiteSpace(resolvedPath))
                        {
                            resolvedPath = pythonExe;
                        }
                        info?.Invoke("Using Python: " + resolvedPath);
                        return true;
                    }

                    error?.Invoke($"{pythonExe} missing reportlab (exit {process.ExitCode}). {stderr.ToString().Trim()}");
                }
            }
            catch (Win32Exception)
            {
                // exe not found
            }
            catch (Exception ex)
            {
                error?.Invoke($"Failed probing {pythonExe}: {ex.Message}");
            }

            resolvedPath = null;
            return false;
        }

        internal static bool TryRunPython(
            string pythonExe,
            string scriptPath,
            string workingDir,
            string inputPath,
            string outputDir,
            Action<string> info,
            Action<string> error,
            out int exitCode)
        {
            var args = new StringBuilder();
            args.Append('"').Append(scriptPath).Append('"');
            if (!string.IsNullOrWhiteSpace(inputPath))
            {
                args.Append(' ').Append('"').Append(inputPath).Append('"');
            }
            if (!string.IsNullOrWhiteSpace(outputDir))
            {
                args.Append(' ').Append('"').Append(outputDir).Append('"');
            }

            var psi = new ProcessStartInfo(pythonExe, args.ToString())
            {
                WorkingDirectory = workingDir,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            psi.EnvironmentVariables["CW_WORKDIR"] = workingDir;
            psi.EnvironmentVariables["CW_INPUT"] = inputPath;
            psi.EnvironmentVariables["CW_OUTPUT_DIR"] = outputDir;

            try
            {
                using (var process = Process.Start(psi))
                {
                    if (process == null)
                    {
                        exitCode = -1;
                        return false;
                    }

                    process.OutputDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            info?.Invoke(e.Data);
                        }
                    };

                    process.ErrorDataReceived += (s, e) =>
                    {
                        if (e.Data != null)
                        {
                            error?.Invoke(e.Data);
                        }
                    };

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();
                    exitCode = process.ExitCode;
                    return true;
                }
            }
            catch (Win32Exception)
            {
                exitCode = -1;
                return false;
            }
            catch (Exception ex)
            {
                error?.Invoke($"Failed running {pythonExe}: {ex.Message}");
                exitCode = -1;
                return false;
            }
        }
    }
}
