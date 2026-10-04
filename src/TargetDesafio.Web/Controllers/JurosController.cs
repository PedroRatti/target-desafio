using Microsoft.AspNetCore.Mvc;
using TargetDesafio.Core.Juros;
using TargetDesafio.Web.Contracts.Juros;

namespace TargetDesafio.Web.Controllers;

[ApiController]
[Route("api/juros")]
public sealed class JurosController : ControllerBase
{
    private readonly JurosService _service;

    public JurosController(JurosService service)
    {
        _service = service;
    }

    [HttpPost("calcular")]
    [ProducesResponseType(
        typeof(ResultadoJuros),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ResultadoJuros> Calcular(
        CalcularJurosRequest request
    )
    {
        try
        {
            var dataCalculo = DateOnly.FromDateTime(DateTime.Today);

            var resultado = _service.Calcular(
                request.ValorOriginal,
                request.DataVencimento,
                dataCalculo
            );

            return Ok(resultado);
        }
        catch (ArgumentException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Dados inválidos para o cálculo",
                detail: exception.Message
            );
        }
    }
}
