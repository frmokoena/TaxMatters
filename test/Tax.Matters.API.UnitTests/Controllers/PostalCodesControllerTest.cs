using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Net;
using Tax.Matters.API.Core.Modules.PostalCodes.Queries;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Controllers;

public class PostalCodesControllerTest
{
    [Test]
    public async Task ListReturnsOkObjectResultWhenQueryIsSuccess()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new PostalCodesController(mediator.Object);

        var list = new PageList<PostalCode>(new List<PostalCode>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<PostalCode>>(
            list,
            raw: string.Empty,
            HttpStatusCode.OK);

        mediator.Setup(m => m.Send(It.IsAny<GetPostalCodesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.List();

        // Assert
        Assert.That(result, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ListReturnsObjectResultWhenQueryFailsWithHttpReason()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new PostalCodesController(mediator.Object);

        var list = new PageList<PostalCode>(new List<PostalCode>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<PostalCode>>(
            raw: null,
            reason: "entity not found",
            statusCode: HttpStatusCode.NotFound);

        mediator.Setup(m => m.Send(It.IsAny<GetPostalCodesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.List();


        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(404));
        });
    }

    [Test]
    public async Task ListReturnsStatusResultWhenQueryFailsWithHttpStatus()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new PostalCodesController(mediator.Object);

        var list = new PageList<PostalCode>(new List<PostalCode>(), count: 0, pageIndex: 1);

        var response = new Response<PageList<PostalCode>>(
            raw: null,
            reason: null,
            statusCode: HttpStatusCode.Conflict);

        mediator.Setup(m => m.Send(It.IsAny<GetPostalCodesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.List();

        // Assert
        Assert.That(result, Is.TypeOf<StatusCodeResult>());
    }

    [Test]
    public async Task ListReturnsInternalServerErrorWhenQueryFailsWithNonHttpError()
    {
        // Arrange
        var mediator = new Mock<IMediator>();

        var controller = new PostalCodesController(mediator.Object);

        var list = new PageList<PostalCode>(new List<PostalCode>(), count: 0, pageIndex: 1);

        var ex = new ArgumentNullException("param");

        var response = new Response<PageList<PostalCode>>(ex);

        mediator.Setup(m => m.Send(It.IsAny<GetPostalCodesQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(response);

        // Act
        var result = await controller.List();

        // Assert       
        ObjectResult? objectResult = result as ObjectResult;
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ObjectResult>());
            Assert.That(objectResult!.StatusCode!, Is.EqualTo(500));
        });
    }
}
