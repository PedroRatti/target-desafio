using TargetDesafio.Core.Estoque;
using Xunit;

namespace TargetDesafio.Tests.Estoque;

public sealed class EstoqueServiceTests
{
    [Fact]
    public void RegistrarEntrada_DeveAumentarEstoque()
    {
        var service = CriarService();

        var movimentacao = service.RegistrarMovimentacao(
            codigoProduto: 101,
            tipo: TipoMovimentacao.Entrada,
            quantidade: 20,
            descricao: "Compra do fornecedor"
        );

        Assert.Equal(150, movimentacao.EstoqueAnterior);
        Assert.Equal(170, movimentacao.EstoqueFinal);
        Assert.Equal(170, service.BuscarProduto(101).Estoque);
    }

    [Fact]
    public void RegistrarSaida_DeveDiminuirEstoque()
    {
        var service = CriarService();

        var movimentacao = service.RegistrarMovimentacao(
            codigoProduto: 101,
            tipo: TipoMovimentacao.Saida,
            quantidade: 30,
            descricao: "Venda para cliente"
        );

        Assert.Equal(150, movimentacao.EstoqueAnterior);
        Assert.Equal(120, movimentacao.EstoqueFinal);
    }

    [Fact]
    public void RegistrarSaida_NaoDevePermitirEstoqueNegativo()
    {
        var service = CriarService();

        Assert.Throws<InvalidOperationException>(() =>
            service.RegistrarMovimentacao(
                codigoProduto: 101,
                tipo: TipoMovimentacao.Saida,
                quantidade: 151,
                descricao: "Venda para cliente"
            )
        );

        Assert.Equal(150, service.BuscarProduto(101).Estoque);
    }

    [Fact]
    public void RegistrarMovimentacoes_DeveGerarIdentificadoresDiferentes()
    {
        var service = CriarService();

        var primeira = service.RegistrarMovimentacao(
            101,
            TipoMovimentacao.Entrada,
            10,
            "Primeira entrada"
        );

        var segunda = service.RegistrarMovimentacao(
            101,
            TipoMovimentacao.Entrada,
            10,
            "Segunda entrada"
        );

        Assert.NotEqual(
            primeira.Identificador,
            segunda.Identificador
        );
        Assert.Equal(1L, primeira.Identificador);
        Assert.Equal(2L, segunda.Identificador);
    }

    [Fact]
    public void RegistrarMovimentacao_DeveRejeitarQuantidadeZero()
    {
        var service = CriarService();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            service.RegistrarMovimentacao(
                101,
                TipoMovimentacao.Entrada,
                0,
                "Entrada inválida"
            )
        );
    }

    private static EstoqueService CriarService()
    {
        var produtos = new[]
        {
            new Produto(
                codigoProduto: 101,
                descricaoProduto: "Caneta Azul",
                estoque: 150
            )
        };

        return new EstoqueService(produtos);
    }
}
