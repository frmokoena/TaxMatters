using MediatR;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.API.Core.Wrappers;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Queries;

public class GetPostalCodesQuery(PostalCodesFilteringModel model) : IRequest<IResponse<PageList<PostalCode>>>
{
    public PostalCodesFilteringModel Model { get; } = model;
}
