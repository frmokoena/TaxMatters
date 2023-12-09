using Microsoft.Extensions.Options;
using Moq;
using System.Net;
using Tax.Matters.Client;
using Tax.Matters.Client.Extensions;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Web.Core.Modules.TaxCalculations.Models;

namespace Tax.Matters.Web.Core.Modules.TaxCalculations.Handlers;

public class CalculateTaxCommandHandlerTest
{
    [Test]
    public async Task HandleReturnsTaxCalculationWhenCalculationSucceeds()
    {
        // Arrange      
        string api = "https://localhost:5443", key = "1-2-3", name = "web", endpoint = "services/taxcalculations/calculate";

        var requestModel = new TaxCalculationInputModel
        {
            AnnualIncome = 100,
            PostalCode = "0043",
        };

        var resultModel = new TaxCalculation
        {
            AnnualIncome = 100,
            TaxAmount = 10,
            PostalCode = new PostalCode
            {
                Code = "0043"
            }            
        };

        var clientOptionsAccessor = new Mock<IOptions<ClientOptions>>();

        clientOptionsAccessor.Setup(m => m.Value).Returns(new ClientOptions
        {
            API = api,
            Key = key,
            Name = name
        });

        var client = new Mock<IAPIClient>();

        var response = new Response<TaxCalculation>(
            raw: resultModel.ToJsonString(),
            HttpStatusCode.OK);

        client.Setup(m => 
            m.CreateAsync<TaxCalculation, TaxCalculationInputModel>(
                It.IsAny<TaxCalculationInputModel>(),
                endpoint,
                api,
                name,
                key, 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        var handler = new CalculateTaxCommandHandler(client.Object, clientOptionsAccessor.Object);

        var command = new CalculateTaxCommand(requestModel);

        // Act
        var result = await handler.Handle(command, default);

        Assert.Multiple(() =>
        {
            // Assert
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(resultModel.TaxAmount));
        });
    }
}
