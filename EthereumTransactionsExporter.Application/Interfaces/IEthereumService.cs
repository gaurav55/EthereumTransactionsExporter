using EthereumTransactionsExporter.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTransactionsExporter.Application.Interfaces
{
    public interface IEthereumService
    {
        Task<IEnumerable<Transaction>> GetTransactionsAsync(string address);
    }
}