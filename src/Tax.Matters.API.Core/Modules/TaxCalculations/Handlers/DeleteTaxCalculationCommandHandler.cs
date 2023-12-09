using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Tax.Matters.API.Core.Modules.TaxCalculations.Commands;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Infrastructure.Data;

namespace Tax.Matters.API.Core.Modules.TaxCalculations.Handlers;

public class DeleteTaxCalculationCommandHandler(AppDbContext context) : IRequestHandler<DeleteTaxCalculationCommand, IResponse<TaxCalculation>>
{
    private readonly AppDbContext _context = context;

    public async Task<IResponse<TaxCalculation>> Handle(
        DeleteTaxCalculationCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
        {
            throw new ArgumentNullException(nameof(request), "Request id can not be null");
        }

        var taxCalculation = await _context.TaxCalculation.FirstOrDefaultAsync(m => m.Id == request.Id);

        if (taxCalculation == null)
        {
            return new Response<TaxCalculation>(
                raw: null,
                reason: "entity not found",
                statusCode: HttpStatusCode.NotFound);            
        }

        try
        {
            _context.TaxCalculation.Remove(taxCalculation);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            return new Response<TaxCalculation>(ex);
        }

        var result = new Response<TaxCalculation>(
            taxCalculation,
            raw: string.Empty,
            HttpStatusCode.OK);

        return result;
    }
}
