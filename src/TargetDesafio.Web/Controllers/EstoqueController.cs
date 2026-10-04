using Microsoft.AspNetCore.Mvc;
using TargetDesafio.Core.Estoque;
using TargetDesafio.Web.Contracts.Estoque;

namespace TargetDesafio.Web.Controllers;

[ApiController]
[Route("api/estoque")]
public sealed class EstoqueController : ControllerBase
{
    private readonly EstoqueService _service;

    public EstoqueController(EstoqueService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyList<Produto>),
        StatusCodes.Status200OK
    )]
    public ActionResult<IReadOnlyList<Produto>> ListarProdutos()
    {
        return Ok(_service.ListarProdutos());
    }

    [HttpGet("{codigoProduto:int}")]
    [ProducesResponseType(typeof(Produto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Produto> BuscarProduto(int codigoProduto)
    {
        try
        {
            return Ok(_service.BuscarProduto(codigoProduto));
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Produto não encontrado",
                detail: exception.Message
            );
        }
    }

    [HttpGet("movimentacoes")]
    [ProducesResponseType(
        typeof(IReadOnlyList<MovimentacaoEstoque>),
        StatusCodes.Status200OK
    )]
    public ActionResult<IReadOnlyList<MovimentacaoEstoque>>
        ListarMovimentacoes()
    {
        return Ok(_service.ListarMovimentacoes());
    }

    [HttpPost("movimentacoes")]
    [ProducesResponseType(
        typeof(MovimentacaoEstoque),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public ActionResult<MovimentacaoEstoque> RegistrarMovimentacao(
        RegistrarMovimentacaoRequest request
    )
    {
        try
        {
            var movimentacao = _service.RegistrarMovimentacao(
                request.CodigoProduto,
                request.Tipo,
                request.Quantidade,
                request.Descricao
            );

            return CreatedAtAction(
                nameof(ListarMovimentacoes),
                movimentacao
            );
        }
        catch (KeyNotFoundException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status404NotFound,
                title: "Produto não encontrado",
                detail: exception.Message
            );
        }
        catch (InvalidOperationException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status409Conflict,
                title: "Movimentação não permitida",
                detail: exception.Message
            );
        }
        catch (ArgumentException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Movimentação inválida",
                detail: exception.Message
            );
        }
        catch (OverflowException exception)
        {
            return Problem(
                statusCode: StatusCodes.Status400BadRequest,
                title: "Movimentação inválida",
                detail: exception.Message
            );
        }
    }
}
