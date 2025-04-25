using EthereumTransactionsExporter.Application.Interfaces;
using EthereumTransactionsExporter.Domain.Entities;
using EthereumTransactionsExporter.Infrastructure.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace EthereumTransactionsExporter.Infrastructure.Services
{
    public class AlchemyService : IEthereumService
    {
        private readonly HttpClient _httpClient;
        private ILogger<AlchemyService> _logger;
        private readonly IConfiguration _configuration;

        private readonly string ApiKey;
        private readonly string BaseUrl;

        public AlchemyService(HttpClient httpClient, ILogger<AlchemyService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            ApiKey = _configuration["apikey"] ?? throw new ArgumentNullException("api key is missing.");
            BaseUrl = $"https://eth-mainnet.g.alchemy.com/v2/{ApiKey}";
        }

        public async Task<IEnumerable<Transaction>> GetTransactionsAsync(string address)
        {
            _logger.LogInformation($"Fetching transaction for addess {address}");

            if (!IsInvalidAddress(address))
            {
                _logger.LogWarning($"The address received in invalid: {address}");
                throw new ArgumentException("The address received in invalid.");
            }

            var transactions = new List<Transaction>();
            string pageKey = string.Empty;

            while (true)
            {
                var (currentTransactions, nextPageKey) = await FetchTransactions(address, pageKey);

                var tasks = new List<Task>();

                int batchSize = 100;
                int total = currentTransactions.Count;
                int totalChunks = (int)Math.Ceiling((double)total / batchSize);



                for (int i = 0; i < totalChunks; i++)
                {
                    var transactionBatch = currentTransactions.Skip(i * batchSize).Take(batchSize).ToList();
                    var trnsactionId = transactionBatch.Select(t => t.TransactionHash).ToList();

                    var task = Task.Run(async () =>
                    {
                        var GasFeesByTransactionId = await GetTotalTransactionCost(trnsactionId);
                        foreach (var ctrn in GasFeesByTransactionId)
                        {
                            var transaction = currentTransactions.FirstOrDefault(x => x.TransactionHash == ctrn.Key);
                            if (transaction != null)
                            {
                                transaction.GasFee = ctrn.Value;
                            }
                        }
                    });

                    tasks.Add(task);
                }

                await Task.WhenAll(tasks);

                transactions.AddRange(currentTransactions);

                if (string.IsNullOrEmpty(nextPageKey))
                {
                    break;
                }

                pageKey = nextPageKey;
            }

            _logger.LogInformation("Fetched total of {Count} transactions.", transactions.Count);
            return transactions;
        }

        private async Task<(List<Transaction>, string)> FetchTransactions(string address, string pageKey)
        {
            List<Transaction> transactions = new List<Transaction>();
            var @param = new Dictionary<string, object>
                        {
                            {"fromBlock", "0x0"},
                            {"toBlock", "latest"},
                            {"toAddress", address},
                            {"category", new[] { "external","internal","erc20","erc721","erc1155","specialnft"  }},
                            {"order", "asc"},
                            {"withMetadata", true},
                            {"excludeZeroValue", false},
                            {"maxCount", "0x3e8"},
                        };

            if (!string.IsNullOrEmpty(pageKey))
            {
                param.Add("pageKey", pageKey);
            }

            var payload = new
            {
                id = 1,
                jsonrpc = "2.0",
                method = "alchemy_getAssetTransfers",
                @params = new[] { @param }
            };

            try
            {
                var requestContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var jsonResponse = await _httpClient.PostAsync(BaseUrl, requestContent);
                jsonResponse.EnsureSuccessStatusCode();

                var jsonTransactionContent = await jsonResponse.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var alchemyTransactionList = System.Text.Json.JsonSerializer.Deserialize<AlchemyResponse>(jsonTransactionContent, options)?.Result;

                if (alchemyTransactionList == null)
                {
                    return (transactions, string.Empty);
                }

                foreach (var tx in alchemyTransactionList.Transfers)
                {
                    transactions.Add(new Transaction
                    {
                        TransactionHash = tx.Hash,
                        DateTime = tx.Metadata?.BlockTimestamp,
                        FromAddress = tx.From,
                        ToAddress = tx.To,
                        TransactionType = CategorizeTransaction(tx.Category),
                        AssetContractAddress = tx.RawContract?.Address,
                        AssetSymbolOrName = tx.Asset,
                        TokenId = tx.TokenId,
                        Value = tx.Value ?? 0
                    });
                }

                pageKey = alchemyTransactionList.PageKey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Ethereum transactions");
                throw;
            }

            return (transactions, pageKey);
        }

        public async Task<Dictionary<string, string>> GetTotalTransactionCost(IEnumerable<string> transactionIds)
        {
            var batchRequests = new List<object>();
            int id = 1;

            foreach (var trnId in transactionIds)
            {
                batchRequests.Add(new
                {
                    jsonrpc = "2.0",
                    id = id++,
                    method = "eth_getTransactionReceipt",
                    @params = new[] { trnId }
                });
            }

            try
            {
                var jsonContent = JsonConvert.SerializeObject(batchRequests);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(BaseUrl, content);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var receipts = JsonConvert.DeserializeObject<List<JObject>>(responseBody);
                var gasPriceAndTransactions = new Dictionary<string, string>();

                if (receipts?.Count == 0)
                {
                    return gasPriceAndTransactions;
                }

                foreach (var receipt in receipts)
                {
                    var gasUsed = (string)receipt["result"]["gasUsed"];
                    var gasPrice = (string)receipt["result"]["effectiveGasPrice"];
                    var transactionId = (string)receipt["result"]["transactionHash"]; ;

                    var totalGasPrice = CalculateGasFeeInEth(gasUsed, gasPrice);
                    gasPriceAndTransactions.Add(transactionId, totalGasPrice);
                }

                return gasPriceAndTransactions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch Ethereum transaction receipt.");
                throw;
            }
        }

        private static string CategorizeTransaction(string category)
        {
            if (category == "external")
            {
                return "External (Normal) Transfer";
            }
            else if (category == "internal")
            {
                return "Internal Transfer";
            }
            else if (category == "erc20")
            {
                return "ERC-20";
            }
            else if (category == "erc721")
            {
                return "ERC-721";
            }
            else if (category == "erc1155")
            {
                return "ERC-1155";
            }
            else if (category == "specialnft")
            {
                return "Special NFT";
            }

            return string.Empty;
        }

        public static string CalculateGasFeeInEth(string gasUsedHex, string gasPriceHex)
        {
            var gasUsed = Convert.ToInt64(gasUsedHex, 16);
            var gasPrice = Convert.ToInt64(gasPriceHex, 16);
            var totalWei = gasUsed * (decimal)gasPrice;
            var eth = totalWei / 1_000_000_000_000_000_000m;
            return eth.ToString();
        }

        private static bool IsInvalidAddress(string address)
        {
            string regexPattern = @"^(0x)?[0-9a-fA-F]{40}$";
            Regex regex = new Regex(regexPattern);
            return regex.IsMatch(address);
        }
    }
}