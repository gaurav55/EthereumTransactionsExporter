using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTransactionsExporter.Infrastructure.Models
{
    public class RawContract
    {
        public string Value { get; set; }
        public string Address { get; set; }
        public string Decimal { get; set; }
    }
}
