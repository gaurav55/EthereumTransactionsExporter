namespace EthereumTransactionsExporter.Domain.Entities
{
    public class Transaction
    {
        public string TransactionHash { get; set; }
        public string DateTime { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string TransactionType { get; set; }
        public string AssetContractAddress { get; set; }
        public string AssetSymbolOrName { get; set; }
        public string TokenId { get; set; }
        public decimal Value { get; set; }
        public string GasFee { get; set; }
    }
}
