using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Moq;
using NUnit.Framework.Legacy;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;
using Tax.Matters.API.Core.Modules.TaxCalculations.Models;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.Handlers;

[TestFixture]
public class CalculateTaxCommandHandlerTest
{
    private readonly IHttpContextAccessor _httpContext;
    private readonly DbContextOptions<AppDbContext> _contextOptions;

    public CalculateTaxCommandHandlerTest()
    {
        var httpContext = new Mock<IHttpContextAccessor>();
        _httpContext = httpContext.Object;

        _contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("CalculateTaxCommandHandlerTest")
            .ConfigureWarnings(b => b.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
    }

    [Test]
    public void HandleThrowsArgumentNullExceptionIfPostalCodeIsNull()
    {

        // Arrange
        using var context = new AppDbContext(_contextOptions, _httpContext);

        var command = new CalculateTaxCommand(new TaxCalculationRequestModel
        {
            AnnualIncome = 100000
        });

        var handler = new CalculateTaxCommandHandler(context);

        // Act
        // Assert
        Assert.That(async () => await handler.Handle(command, default), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void HandleThrowsInvalidOperationExceptionIfIncomeIsLessThanZero()
    {

        // Arrange
        using var context = new AppDbContext(_contextOptions, _httpContext);

        var command = new CalculateTaxCommand(new TaxCalculationRequestModel
        {
            AnnualIncome = -100000,
            PostalCode = "0043"
        });

        var handler = new CalculateTaxCommandHandler(context);

        // Act
        // Assert
        Assert.That(async () => await handler.Handle(command, default), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task HandleExistingCalculationIfExists()
    {

        // Arrange
        using var context = new AppDbContext(_contextOptions, _httpContext);

        var tax = new IncomeTax
        {
            FlatRate = 17.5m,
        };
        var code = "0043";
        decimal income = 10;
        var postalCode = new PostalCode
        {
            Code = code,
            IncomeTax = tax
        };

        var calculation = new TaxCalculation
        {
            AnnualIncome = income,
            TaxAmount = 1.75m,
            PostalCode = postalCode
        };

        context.Add(calculation);

        context.SaveChanges();

        var command = new CalculateTaxCommand(new TaxCalculationRequestModel
        {
            AnnualIncome = income,
            PostalCode = code
        });

        var handler = new CalculateTaxCommandHandler(context);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.That(result.Content!.Id, Is.EqualTo(calculation.Id));
    }
}
