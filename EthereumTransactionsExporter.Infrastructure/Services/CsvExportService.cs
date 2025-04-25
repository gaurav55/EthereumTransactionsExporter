using CsvHelper;
using EthereumTransactionsExporter.Application.Interfaces;
using EthereumTransactionsExporter.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTransactionsExporter.Infrastructure.Services
{
    public class CsvExportService : ICsvExportService
    {
        private readonly ILogger<CsvExportService> _logger;

        public CsvExportService(ILogger<CsvExportService> logger)
        {
            _logger = logger;
        }

        public byte[] ExportTransactionsToCsv(IEnumerable<Transaction> transactions)
        {
            _logger.LogInformation("Exporting transactions to CSV");
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(transactions);
            writer.Flush();
            _logger.LogInformation("CSV export completed successfully.");
            return memoryStream.ToArray();
        }
    }
}