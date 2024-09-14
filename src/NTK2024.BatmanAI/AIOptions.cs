using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTK2024.BatmanAI
{
    public class AIOptions
    {
        public const string ElementName = "AIOptions";
        public string OpenAIApiKey { get; set; } = null!;
        public string AzureOpenAIApiKey { get; set; } = null!;
        public string BingAIApiKey { get; set; } = null!;
    }
}
