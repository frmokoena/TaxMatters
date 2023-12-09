using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Controllers;

public class TaxCalculationsControllerTest
{
    [Test]
    public async Task ListCalculationsReturnsOkObjectResultWhenQueryIsSuccess()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxCalculationsController(mediator.Object);

        var list = new PageList<TaxCalculation>(new List<TaxCalculation>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<TaxCalculation>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        mediator.Setup(m => m.Send(It.IsAny<GetTaxCalculationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListCalculations();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ListCalculationsReturnsObjectResultWhenQueryFailsWithHttpReason()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxCalculationsController(mediator.Object);

        var list = new PageList<TaxCalculation>(new List<TaxCalculation>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<TaxCalculation>>(
            raw: null,
            reason: "entity not found",
            statusCode: HttpStatusCode.NotFound);

        mediator.Setup(m => m.Send(It.IsAny<GetTaxCalculationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListCalculations();


        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(404));
        });
    }

    [Test]
    public async Task ListCalculationsReturnsStatusResultWhenQueryFailsWithHttpStatus()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxCalculationsController(mediator.Object);

        var list = new PageList<TaxCalculation>(new List<TaxCalculation>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<TaxCalculation>>(
            raw: null,
            reason: null,
            statusCode: HttpStatusCode.Conflict);

        mediator.Setup(m => m.Send(It.IsAny<GetTaxCalculationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListCalculations();

        // Assert
        Assert.That(result, Is.TypeOf<StatusCodeResult>());
    }

    [Test]
    public async Task ListCalculationsReturnsInternalServerErrorWhenQueryFailsWithNonHttpError()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxCalculationsController(mediator.Object);

        var list = new PageList<TaxCalculation>(new List<TaxCalculation>(), count: 0, pageIndex: 1);

        var ex = new ArgumentNullException("param");

        var response = new Response<PageList<TaxCalculation>>(ex);

        mediator.Setup(m => m.Send(It.IsAny<GetTaxCalculationsQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListCalculations();

        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(500));
        });
    }
}
