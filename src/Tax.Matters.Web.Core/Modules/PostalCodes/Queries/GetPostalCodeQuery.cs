using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Queries;

/// <summary>
/// Initializes a new instance of the <see cref="GetPostalCodeQuery"/> query class
/// </summary>
/// <param name="id"></param>
public class GetPostalCodeQuery(string id) : IRequest<IResponse<PostalCode>>
{
    public string Id { get; } = id;
}