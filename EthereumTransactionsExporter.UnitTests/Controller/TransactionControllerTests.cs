using EthereumTransactionsExporter.API.Controllers;
using EthereumTransactionsExporter.Application.Interfaces;
using EthereumTransactionsExporter.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;
using System.Text.Json;
using Xunit;

namespace EthereumTransactionsExporter.UnitTests.Controller
{
    public class TransactionControllerTests
    {
        private readonly TransacationsController _controller;
        private readonly Mock<ILogger<TransacationsController>> _mockLogger;
        private readonly Mock<IEthereumService> _mockEthereumService;
        private readonly Mock<ICsvExportService> _mockCsvExporter;

        public TransactionControllerTests()
        {
            _mockLogger = new Mock<ILogger<TransacationsController>>();
            _mockEthereumService = new Mock<IEthereumService>();
            _mockCsvExporter = new Mock<ICsvExportService>();

            _controller = new TransacationsController(_mockEthereumService.Object, _mockCsvExporter.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GivenValidEthereumAddress_WhenFetchTransactions_ThenReturnTransactionsInCSVAsync()
        {
            var validAddress = "0xFB50526f49894B78541B776F5aaefE43e3Bd8590";
            var transactions = new List<Transaction>
            {
                new Transaction { TransactionHash = "hash", FromAddress = "0x1", ToAddress = "0x2", Value = 1.23m }
            };

            var expectedCsv = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(transactions));

            _mockEthereumService
                .Setup(s => s.GetTransactionsAsync(validAddress))
                .ReturnsAsync(transactions);

            _mockCsvExporter
                .Setup(c => c.ExportTransactionsToCsv(transactions))
                .Returns(expectedCsv);

            // When
            var result = await _controller.GetCsv(validAddress);

            // Then
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("text/csv", fileResult.ContentType);
            Assert.Equal($"{validAddress}_transactions.csv", fileResult.FileDownloadName);
            Assert.Equal(expectedCsv, fileResult.FileContents);
        }

        public void GivenInValidEthereumAddress_WhenFetchTransactions_ThenReturnBadRequestResponse()
        {
            //Arrange

            //Assert

            //Act
        }

        public void GivenValidEthereumAddress_WhenFetchNoTransactions_ThenReturnOkWithResponseMessage()
        {
            //Arrange

            //Assert

            //Act
        }

        public void GivenValidEthereumAddress_WhenExceptionInFetchTransactions_ThenReturnBadResponseWithExceptionMessage()
        {
            //Arrange

            //Assert

            //Act
        }
    }
}