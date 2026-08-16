using System.Diagnostics;

namespace Sh4RPyTweaker.Models
{
    public class ProcessItem
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public double CpuPercent { get; set; }
        public long MemoryMb { get; set; }
        public int Threads { get; set; }
        public string StartTime { get; set; }
        public string FileName { get; set; }
        public bool IsProtected { get; set; }
        public Process Process { get; set; }

        public string CpuText { get { return CpuPercent.ToString("0.0") + " %"; } }
        public string MemoryText { get { return MemoryMb.ToString("N0") + " МБ"; } }
    }
}

