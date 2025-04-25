namespace EthereumTransactionsExporter.Infrastructure.Models
{
    public class AlchemyResponse
    {
        public string Jsonrpc { get; set; }
        public int Id { get; set; }
        public AlchemyResult Result { get; set; }
    }
}
 