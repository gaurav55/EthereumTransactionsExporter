namespace EthereumTransactionsExporter.Infrastructure.Models
{
    public class AlchemyTransaction
    {
        public string BlockNum { get; set; }
        public string UniqueId { get; set; }
        public string Hash { get; set; }
        public string From { get; set; }
        public string? To { get; set; }
        public decimal? Value { get; set; }
        public string? Erc721TokenId { get; set; }
        public List<Erc1155MetadataItem>? Erc1155Metadata { get; set; }
        public string? TokenId { get; set; }
        public string? Asset { get; set; }
        public string? Category { get; set; }
        public RawContract? RawContract { get; set; }
        public Metadata? Metadata { get; set; }
    }

    public class Erc1155MetadataItem
    {
        public string TokenId { get; set; }

        public string Value { get; set; }
    }
}