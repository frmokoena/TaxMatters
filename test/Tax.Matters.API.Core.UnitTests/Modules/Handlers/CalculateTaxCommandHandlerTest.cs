using Azure.Core;
using Moq;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;
using Tax.Matters.API.Core.Modules.TaxCalculations.Models;
using Tax.Matters.API.Core.UnitTests;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Domain.Enums;
using Tax.Matters.Infrastructure.Data.Repositories;

namespace Tax.Matters.API.Core.Modules.Handlers;

[TestFixture]
public class CalculateTaxCommandHandlerTest
{
    [Test]
    public void HandleThrowsArgumentNullExceptionIfPostalCodeIsNull()
    {
        // Arrange
        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(new TaxCalculationRequestModel
        {
            AnnualIncome = 100000
        });

        // Act
        // Assert
        Assert.That(async () => await handler.Handle(command, default), Throws.TypeOf<ArgumentNullException>());
    }

    [Test]
    public void HandleThrowsInvalidOperationExceptionIfIncomeIsLessThanZero()
    {

        // Arrange
        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(new TaxCalculationRequestModel
        {
            AnnualIncome = -100000,
            PostalCode = "0043"
        });

        // Act
        // Assert
        Assert.That(async () => await handler.Handle(command, default), Throws.TypeOf<InvalidOperationException>());
    }

    [Test]
    public async Task HandleReturnsNotFoundIfPostalCodeNotFound()
    {
        // Arrange
        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 100000,
            PostalCode = "0043"
        };

        var command = new CalculateTaxCommand(requestModel);

        var calculation = new TaxCalculation
        {
            Id = "abc-def",
            AnnualIncome = requestModel.AnnualIncome,
            TaxAmount = 10,
            PostalCode = new PostalCode
            {
                Code = requestModel.PostalCode,
            }
        };

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        });
    }

    [Test]
    public async Task CanHandleFlatRateTaxCalculation()
    {
        // Arrange
        string code = "0043";
        decimal taxAmount = 22225m;

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 127000,
            PostalCode = code
        };

        var flatRate = new IncomeTax
        {
            Id = "flat-rate",
            FlatRate = 17.5m,
            TypeName = TaxCalculationType.FlatRate,
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = flatRate
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(taxAmount));
        });
    }

    [Test]
    public async Task CanHandleFlatValueTaxCalculationForIncomeLessThanThreshold()
    {
        // Arrange
        string code = "0043";
        decimal taxAmount = 6350m;

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 127000,
            PostalCode = code
        };

        // Flat value
        var flatValue = new IncomeTax
        {
            TypeName = TaxCalculationType.FlatValue,
            FlatValue = new FlatValueIncomeTax
            {
                Amount = 10000m,
                Threshold = 200000,
                ThresholdRate = 5m
            }
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = flatValue
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(taxAmount));
        });
    }

    [Test]
    public async Task CanHandleFlatValueTaxCalculationForIncomeOverThreshold()
    {
        // Arrange
        string code = "0043";

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 543000,
            PostalCode = code
        };

        // Flat value
        var flatValue = new IncomeTax
        {
            TypeName = TaxCalculationType.FlatValue,
            FlatValue = new FlatValueIncomeTax
            {
                Amount = 10000m,
                Threshold = 200000,
                ThresholdRate = 5m
            }
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = flatValue
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(flatValue.FlatValue.Amount));
        });
    }

    [Test]
    public async Task HandleFailsForProgressiveIfThereIsNoProgressiveTable()
    {
        // Arrange
        string code = "0043";

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 543000,
            PostalCode = code
        };

        // Progressive value
        var progressiveTax = new IncomeTax
        {
            Id = "progressive-tax",
            TypeName = TaxCalculationType.Progressive
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ProgressiveIncomeTax>());

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task HandleFailsForProgressiveIfRequestedIncomeIsOutsideTaxTable()
    {
        // Arrange
        string code = "0043";

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 543000,
            PostalCode = code
        };

        // Progressive
        var progressiveTaxTable = new List<ProgressiveIncomeTax>
        {
            new()
            {
                MinimumIncome = 600000m,
                Rate = 16m
            }
        };
        var progressiveTax = new IncomeTax
        {
            Id = "progressive-tax",
            TypeName = TaxCalculationType.Progressive,
            ProgressiveTaxTable = progressiveTaxTable
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progressiveTaxTable);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task HandleFailsForProgressiveIfTaxBracketMinimumIncomeIsGreaterThanBracketMaximumIncome()
    {
        // Arrange
        string code = "0043";

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 543000,
            PostalCode = code
        };

        // Progressive
        var progressiveTaxTable = new List<ProgressiveIncomeTax>
        {
            new()
            {
                MinimumIncome = 600m,
                MaximumIncome = 500m,
                Rate = 16m
            }
        };
        var progressiveTax = new IncomeTax
        {
            Id = "progressive-tax",
            TypeName = TaxCalculationType.Progressive,
            ProgressiveTaxTable = progressiveTaxTable
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progressiveTaxTable);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task HandleFailsForProgressiveIfProgressiveTableRangeOverlap()
    {
        // Arrange
        string code = "0043";

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 300,
            PostalCode = code
        };

        // Progressive
        var progressiveTaxTable = new List<ProgressiveIncomeTax>
        {
            new()
            {
                MinimumIncome = 0,
                MaximumIncome = 100,
                Rate = 16m
            },
            new()
            {
                MinimumIncome = 40m,
                MaximumIncome = 175m,
                Rate = 27m
            }
        };
        var progressiveTax = new IncomeTax
        {
            Id = "progressive-tax",
            TypeName = TaxCalculationType.Progressive,
            ProgressiveTaxTable = progressiveTaxTable
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progressiveTaxTable);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.True);
            Assert.That(result.HttpStatusCode, Is.EqualTo(HttpStatusCode.Conflict));
        });
    }

    [Test]
    public async Task CanHandleAFewBracketedIncome()
    {
        // Arrange
        string code = "0043", taxId = "progressive-tax";
        var taxAmount = 1082.50m;

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 10000,
            PostalCode = code
        };

        // Progressive
        var progressiveTaxTable = ProgressiveTaxTableProvider.GetProgressiveTable(taxId, requestModel.AnnualIncome);

        var progressiveTax = new IncomeTax
        {
            Id = taxId,
            TypeName = TaxCalculationType.Progressive,
            ProgressiveTaxTable = progressiveTaxTable
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progressiveTaxTable);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(taxAmount));
        });
    }

    [Test]
    public async Task CanHandleAWholyBracketedIncome()
    {
        // Arrange
        string code = "0043", taxId = "progressive-tax";
        var taxAmount = 208683.50m;

        var requestModel = new TaxCalculationRequestModel
        {
            AnnualIncome = 660000,
            PostalCode = code
        };

        // Progressive
        var progressiveTaxTable = ProgressiveTaxTableProvider.GetProgressiveTable(taxId, requestModel.AnnualIncome);
        var progressiveTax = new IncomeTax
        {
            Id = taxId,
            TypeName = TaxCalculationType.Progressive,
            ProgressiveTaxTable = progressiveTaxTable
        };

        var repository = new Mock<ICalculationRepository>();

        var handler = new CalculateTaxCommandHandler(repository.Object);

        var command = new CalculateTaxCommand(requestModel);

        var postalCode = new PostalCode
        {
            Id = "postal-0043",
            Code = code,
            IncomeTax = progressiveTax
        };

        repository.Setup(m =>
            m.GetPostalCodeAsync(requestModel.PostalCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(postalCode);

        repository.Setup(m =>
        m.GetProgressiveIncomeTaxTableAsync(postalCode.IncomeTax.Id, requestModel.AnnualIncome, It.IsAny<CancellationToken>()))
            .ReturnsAsync(progressiveTaxTable);

        // Act
        var result = await handler.Handle(command, default);

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(result.IsError, Is.False);
            Assert.That(result.Content!.TaxAmount, Is.EqualTo(taxAmount));
        });
    }
}
