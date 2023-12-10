using MediatR;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Queries;

/// <summary>
/// Initializes a new instance of the <see cref="GetPostalCodesQuery"/> query class
/// </summary>
/// <param name="model"></param>
public class GetPostalCodesQuery(PostalCodesFilteringModel model) : IRequest<IResponse<PageList<PostalCode>>>
{
    public PostalCodesFilteringModel Model { get; } = model;
}
