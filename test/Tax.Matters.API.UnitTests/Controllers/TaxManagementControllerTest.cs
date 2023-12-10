using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Globalization;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxManagement.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Controllers;

public class TaxManagementControllerTest
{
    [Test]
    public async Task ListTaxCalculationTypesReturnsOkObjectResultWhenQueryIsSuccess()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxManagementController(mediator.Object);

        var response = new Response<IEnumerable<IncomeTax>>(
            new List<IncomeTax>(),
            raw: string.Empty,
            HttpStatusCode.OK);

        mediator.Setup(m => m.Send(It.IsAny<ListTaxCalculationTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListTaxCalculationTypes();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ListTaxCalculationTypesReturnsObjectResultWhenQueryFailsWithHttpReason()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxManagementController(mediator.Object);

        var response = new Response<IEnumerable<IncomeTax>>(
            raw: null,
            reason: "entity not found",
            statusCode: HttpStatusCode.NotFound);

        mediator.Setup(m => m.Send(It.IsAny<ListTaxCalculationTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListTaxCalculationTypes();


        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(404));
        });
    }

    [Test]
    public async Task ListTaxCalculationTypesReturnsStatusResultWhenQueryFailsWithHttpStatus()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxManagementController(mediator.Object);

        var response = new Response<IEnumerable<IncomeTax>>(
            raw: null,
            reason: null,
            statusCode: HttpStatusCode.Conflict);

        mediator.Setup(m => m.Send(It.IsAny<ListTaxCalculationTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListTaxCalculationTypes();

        // Assert
        Assert.That(result, Is.TypeOf<StatusCodeResult>());
    }

    [Test]
    public async Task ListTaxCalculationTypesReturnsInternalServerErrorWhenQueryFailsWithNonHttpError()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new TaxManagementController(mediator.Object);

        var list = new PageList<IncomeTax>(new List<IncomeTax>(), count: 0, pageIndex: 1);

        var ex = new ArgumentNullException("param");

        var response = new Response<IEnumerable<IncomeTax>>(ex);

        mediator.Setup(m => m.Send(It.IsAny<ListTaxCalculationTypesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.ListTaxCalculationTypes();

        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(500));
        });
    }
}
