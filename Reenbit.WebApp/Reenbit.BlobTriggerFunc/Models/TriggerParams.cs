using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;

namespace Reenbit.BlobTriggerFunc.Models
{
    public class TriggerParams
    {
        public string Name { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
        public Stream Blob { get; set; }
        public ILogger Log { get; set; }
    }
}
