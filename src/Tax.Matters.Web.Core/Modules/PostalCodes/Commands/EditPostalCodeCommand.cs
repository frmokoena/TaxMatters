using MediatR;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;
using Tax.Matters.Web.Core.Modules.PostalCodes.Models;

namespace Tax.Matters.Web.Core.Modules.PostalCodes.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="EditPostalCodeCommand"/> command class
/// </summary>
/// <param name="id"></param>
/// <param name="model"></param>
public class EditPostalCodeCommand(string id, PostalCodeEditModel model) : IRequest<IResponse<PostalCode>>
{
    public string Id { get; } = id;
    public PostalCodeEditModel Model { get; } = model;
}

