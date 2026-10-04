using Microsoft.AspNetCore.Mvc;
using TargetDesafio.Core.Comissoes;

namespace TargetDesafio.Web.Controllers;

[ApiController]
[Route("api/comissoes")]
public sealed class ComissoesController : ControllerBase
{
    private readonly ComissaoService _service;
    private readonly ArquivoVendas _arquivoVendas;

    public ComissoesController(
        ComissaoService service,
        ArquivoVendas arquivoVendas
    )
    {
        _service = service;
        _arquivoVendas = arquivoVendas;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<ResumoComissao>),
        StatusCodes.Status200OK
    )]
    public ActionResult<IReadOnlyList<ResumoComissao>> Obter()
    {
        var resultados = _service.CalcularPorVendedor(
            _arquivoVendas.Vendas
        );

        return Ok(resultados);
    }
}