using System.Diagnostics;

namespace Sh4RPyTweaker.Services
{
    public static class ProcessHelper
    {
        public static string Run(string fileName, string arguments, bool waitForExit = true, int timeoutMs = 30000)
        {
            try
            {
                var psi = new ProcessStartInfo(fileName, arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                using (Process p = Process.Start(psi))
                {
                    string stdout = p.StandardOutput.ReadToEnd();
                    string stderr = p.StandardError.ReadToEnd();
                    if (waitForExit && !p.WaitForExit(timeoutMs))
                    {
                        try { p.Kill(); } catch { }
                    }
                    return stdout + (string.IsNullOrEmpty(stderr) ? "" : "\n" + stderr);
                }
            }
            catch
            {
                return "";
            }
        }
    }
}

