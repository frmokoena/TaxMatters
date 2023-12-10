using MediatR;
using Tax.Matters.API.Core.Modules.PostalCodes.Models;
using Tax.Matters.Client;
using Tax.Matters.Domain.Entities;

namespace Tax.Matters.API.Core.Modules.PostalCodes.Commands;

public class CreatePostalCodeCommand(CreatePostalCodeRequestModel model) : IRequest<IResponse<PostalCode>>
{
    public CreatePostalCodeRequestModel Model { get; } = model;
}
