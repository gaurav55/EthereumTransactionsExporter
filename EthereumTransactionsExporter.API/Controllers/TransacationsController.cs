using EthereumTransactionsExporter.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace EthereumTransactionsExporter.API.Controllers
{
    [ApiController]
    [Route("api/transacations")]
    public class TransacationsController : ControllerBase
    {
        private readonly IEthereumService _ethereumService;
        private readonly ICsvExportService _csvExporter;
        private readonly ILogger<TransacationsController> _logger;

        public TransacationsController(IEthereumService ethereumService, ICsvExportService csvExporter,
            ILogger<TransacationsController> logger)
        {
            _ethereumService = ethereumService;
            _csvExporter = csvExporter;
            _logger = logger;
        }

        [HttpGet("{address}/csv")]
        public async Task<IActionResult> GetCsv(string address)
        {
            _logger.LogInformation("Received request to fetch transactions for address: {Address}", address);

            if (!IsValidAddress(address))
            {
                _logger.LogWarning("Validation failed for address: {Address}", address);
                return BadRequest("Invalid Ethereum address format.");
            }

            try
            {
                var transactions = await _ethereumService.GetTransactionsAsync(address).ConfigureAwait(false);
                if (transactions.Any())
                {
                    var csvBytes = _csvExporter.ExportTransactionsToCsv(transactions);
                    return File(csvBytes, "text/csv", $"{address}_transactions.csv");
                }
                return Ok("No Ethereum transactions found.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while exporting CSV for address: {Address}", address);
                return BadRequest($"Error occurred while processing the request {ex.Message}");
            }
        }

        private static bool IsValidAddress(string address)
        {
            string regexPattern = @"^(0x)?[0-9a-fA-F]{40}$";
            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(address);
        }
    }
}