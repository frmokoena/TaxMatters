using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Web.Core.Modules.TaxManagement.Queries;

/// <summary>
/// Models the query to list calculation types
/// </summary>
public class ListTaxCalculationTypesQuery: IRequest<IResponse<IEnumerable<IncomeTax>>>
{
}
