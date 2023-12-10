using MediatR;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Commands;

/// <summary>
/// Initializes a new instance of the <see cref="EditPostalCodeCommand"/> command class
/// </summary>
/// <param name="id"></param>
/// <param name="model"></param>
public class EditPostalCodeCommand(string id, EditPostalCodeRequestModel model) : IRequest<IResponse<PostalCode>>
{
    public string Id { get; } = id;
    public EditPostalCodeRequestModel Model { get; } = model;
}