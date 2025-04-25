namespace EthereumTransactionsExporter.Infrastructure.Models
{
    public class AlchemyResult
    {
        public List<AlchemyTransaction> Transfers { get; set; }
        public string PageKey { get; set; }
    }
}