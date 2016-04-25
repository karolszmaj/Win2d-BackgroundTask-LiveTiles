using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace TilleDrawingEngine.Logging
{
    public class MemoryInfoNotifier
    {
        public static void DisplayCurrentMemoryStatus()
        {
            var memory = MemoryManager.AppMemoryUsage;
            var memoryLimit = MemoryManager.AppMemoryUsageLimit;
            Debug.WriteLine("Memory warning: \n\tused {0} with limit {1} MB\n\tused {2} with limit {3} KB",
                ToMegaBytes(memory), ToMegaBytes(memoryLimit), ToKiloBytes(memory), ToKiloBytes(memoryLimit));
        }

        private static float ToMegaBytes(ulong memory)
        {
            return memory / 1024 / 1024;
        }

        private static float ToKiloBytes(ulong memory)
        {
            return memory / 1024f;
        }
    }
}
