using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reenbit.BlobTriggerFunc.Models
{
    internal class TriggerParams
    {
        public string Name { get; set; }
        public IDictionary<string, string> Metadata { get; set; }
    }
}
