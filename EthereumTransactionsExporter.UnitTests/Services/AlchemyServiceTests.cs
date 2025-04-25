using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EthereumTransactionsExporter.UnitTests.Services
{
    internal class AlchemyServiceTests
    {
        public void GivenValidAddress_WhenFetchTransactionAndReceipt_ThenReturnAllTransactionWithGasFee()
        {
            //Arrange

            //Assert

            //Act
        }


        public void GivenInvalidAddress_WhenFetchTransactions_ThenThrowException()
        {
            //Arrange

            //Assert

            //Act
        }


        public void GivenValidAddress_WhenExceptionFetchTransactions_ThenPropogateException()
        {
            //Arrange

            //Assert

            //Act
        }

        public void GivenValidAddress_WhenZeroTransactionsFetched_ThenGetTransactionReceiptIsNotCalled()
        {
            //Arrange

            //Assert

            //Act
        }

        public void GivenValidAddress_WhenExceptionGetTotalTransactionCost_ThenLogAndPropogateException()
        {
            //Arrange

            //Assert

            //Act
        }


        public void GivenValidAddress_WhenExceptionInAlchemyAPI_ThenLogAndPropogateException()
        {
            //Arrange

            //Assert

            //Act
        }



        public void GivenValidCategory_WhenCategorizeTransaction_ThenReturnsCorrectLabel()
        {

        }

        public void GivenValidInput_WhenIsInvalidAddress_ThenReturnsExpectedValidationResult()
        {

        }

    }
}
