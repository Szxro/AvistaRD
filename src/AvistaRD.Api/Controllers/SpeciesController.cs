using AvistaRD.Api.Common;
using AvistaRD.Application.Features.SpeciesCatalog.Commands.RegisterSpecies;
using AvistaRD.SharedKernel;
using AvistaRD.SharedKernel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AvistaRD.Api.Controllers;

[Route("api/species")]
public class SpeciesController : BaseController
{
    public SpeciesController(ISender sender) : base(sender) { }

    [HttpPost]
    public async Task<IResult> Register(
        [FromBody] RegisterSpeciesCommand command,
        CancellationToken cancellationToken)
    {
        Result<int> result = await Sender.Send(command, cancellationToken);

        return result.Match(_ => Success(result), Problem);
    }
}
