using TargetDesafio.Core.Comissoes;
using Xunit;

namespace TargetDesafio.Tests.Comissoes;

public sealed class ComissaoServiceTests
{
    private readonly ComissaoService _service = new();

    public static TheoryData<decimal, decimal> FaixasDeComissao =>
        new()
        {
            { 99.99m, 0m },
            { 100m, 1m },
            { 499.99m, 5m },
            { 500m, 25m }
        };

    [Theory]
    [MemberData(nameof(FaixasDeComissao))]
    public void CalcularComissao_DeveRespeitarAsFaixas(decimal valorVenda, decimal comissaoEsperada)
    {
        var comissao = _service.CalcularComissao(valorVenda);

        Assert.Equal(comissaoEsperada, comissao);
    }

    [Fact]
    public void CalcularComissao_DeveArredondarParaDuasCasas()
    {
        var comissao = _service.CalcularComissao(1200.50m);

        Assert.Equal(60.03m, comissao);
    }

    [Fact]
    public void CalcularPorVendedor_DeveAgruparAsVendas()
    {
        var vendas = new[]
        {
            new Venda("Ana", 50m),
            new Venda("Ana", 100m),
            new Venda("Bruno", 500m)
        };

        var resultados = _service.CalcularPorVendedor(vendas);

        var resultadoAna = Assert.Single(resultados, resultado => resultado.Vendedor == "Ana");

        Assert.Equal(150m, resultadoAna.TotalVendido);
        Assert.Equal(1m, resultadoAna.TotalComissao);
    }

    [Fact]
    public void CalcularComissao_DeveRejeitarValorNegativo()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => _service.CalcularComissao(-1m));
    }
}