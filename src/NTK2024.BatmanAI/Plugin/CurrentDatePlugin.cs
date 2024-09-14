using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTK2024.BatmanAI.Plugin
{
    public sealed class CurrentDatePlugin
    {
        [KernelFunction, Description("Determines the current date and time")]
        public DateTime GetCurrentDate()
        {
            return DateTime.Now;
        }

        [KernelFunction, Description("Determines the current year")]
        public int GetCurrentYear()
        {
            return DateTime.Now.Year;
        }
    }
}
