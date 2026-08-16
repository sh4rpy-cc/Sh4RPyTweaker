using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sh4RPyTweaker.Models;

namespace Sh4RPyTweaker.Services
{
    public static class ProcessService
    {
        private static readonly HashSet<string> ProtectedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "system", "system idle process", "registry", "memory compression", "secure system",
            "smss", "csrss", "wininit", "winlogon", "services", "lsass", "svchost",
            "dwm", "audiodg", "fontdrvhost"
        };

        private static T Safe<T>(Func<T> f, T fallback)
        {
            try { return f(); }
            catch { return fallback; }
        }

        private static bool IsProtectedProcess(Process p)
        {
            try
            {
                string name = Safe(() => p.ProcessName, "");
                if (ProtectedNames.Contains(name)) return true;
                if (p.Id == Process.GetCurrentProcess().Id) return true;
            }
            catch
            {
                return true;
            }

            try
            {
                byte[] sid;
                if (NativeMethods.TryGetAccountSid(p.Handle, out sid))
                {
                    uint nameLen = 0, domainLen = 0;
                    int sidType;
                    if (!NativeMethods.LookupAccountSid(null, sid, null, ref nameLen, null, ref domainLen, out sidType))
                        return false;
                    if (nameLen == 0 || domainLen == 0) return false;

                    var account = new System.Text.StringBuilder((int)nameLen);
                    var domain = new System.Text.StringBuilder((int)domainLen);
                    if (NativeMethods.LookupAccountSid(null, sid, account, ref nameLen, domain, ref domainLen, out sidType))
                    {
                        string name = account.ToString().ToUpperInvariant();
                        if (name == "SYSTEM" || name == "LOCAL SERVICE" || name == "NETWORK SERVICE")
                            return true;
                    }
                }
            }
            catch
            {
            }
            return false;
        }

        public static List<ProcessItem> GetProcesses()
        {
            var before = new Dictionary<int, TimeSpan>();
            foreach (Process p in Process.GetProcesses())
            {
                try { before[p.Id] = p.TotalProcessorTime; }
                catch { }
            }

            DateTime start = DateTime.UtcNow;
            Thread.Sleep(700);
            double elapsedMs = (DateTime.UtcNow - start).TotalMilliseconds;
            int cores = Environment.ProcessorCount;

            var list = new List<ProcessItem>();
            foreach (Process p in Process.GetProcesses())
            {
                var item = new ProcessItem
                {
                    Id = p.Id,
                    Name = Safe(() => p.ProcessName, ""),
                    Threads = Safe(() => p.Threads.Count, 0),
                    MemoryMb = Safe(() => p.WorkingSet64 / 1024 / 1024, 0L),
                    StartTime = Safe(() => p.StartTime.ToString("dd.MM.yyyy HH:mm"), ""),
                    FileName = Safe(() => p.MainModule?.FileName, null),
                    IsProtected = IsProtectedProcess(p),
                    Process = p
                };

                TimeSpan prev;
                if (before.TryGetValue(p.Id, out prev))
                {
                    TimeSpan now = Safe(() => p.TotalProcessorTime, TimeSpan.Zero);
                    double delta = (now - prev).TotalMilliseconds;
                    if (delta >= 0 && elapsedMs > 0)
                    {
                        item.CpuPercent = delta / elapsedMs / cores * 100.0;
                    }
                }

                list.Add(item);
            }

            return list
                .OrderByDescending(x => x.CpuPercent)
                .ThenByDescending(x => x.MemoryMb)
                .ToList();
        }

        public static void Kill(ProcessItem item)
        {
            if (item == null || item.IsProtected || item.Process == null) return;
            try { item.Process.Kill(); } catch { }
        }

        public static void OpenFileLocation(ProcessItem item)
        {
            if (string.IsNullOrEmpty(item.FileName)) return;
            try
            {
                Process.Start("explorer.exe", "/select, \"" + item.FileName + "\"");
            }
            catch { }
        }
    }
}

