using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.PostalCodes.Models;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="CreatePostalCodeCommand"/> command class
/// </summary>
/// <param name="model"></param>
public class CreatePostalCodeCommand(PostalCodeInputModel model) : IRequest<IResponse<PostalCode>>
{
    public PostalCodeInputModel Model { get; } = model;
}
