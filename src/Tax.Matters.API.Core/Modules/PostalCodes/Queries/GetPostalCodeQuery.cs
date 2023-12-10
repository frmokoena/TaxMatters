using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Queries;

/// <summary>
/// Initializes a new instance of the <see cref="GetPostalCodeQuery"/> query clss
/// </summary>
/// <param name="model"></param>
public class GetPostalCodeQuery(string id) : IRequest<IResponse<PostalCode>>
{
    public string Id { get; } = id;
}