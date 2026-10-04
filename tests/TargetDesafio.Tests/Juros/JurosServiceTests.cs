using TargetDesafio.Core.Juros;
using Xunit;

namespace TargetDesafio.Tests.Juros;

public sealed class JurosServiceTests
{
    private readonly JurosService _service = new();

    [Fact]
    public void Calcular_DeveAplicarJurosPorDiaDeAtraso()
    {
        var vencimento = new DateOnly(2026, 10, 1);
        var dataCalculo = new DateOnly(2026, 10, 4);

        var resultado = _service.Calcular(
            valorOriginal: 100m,
            dataVencimento: vencimento,
            dataCalculo: dataCalculo
        );

        Assert.Equal(3, resultado.DiasEmAtraso);
        Assert.Equal(7.50m, resultado.ValorJuros);
        Assert.Equal(107.50m, resultado.ValorTotal);
    }

    [Fact]
    public void Calcular_ComVencimentoHoje_NaoDeveAplicarJuros()
    {
        var hoje = new DateOnly(2026, 10, 4);

        var resultado = _service.Calcular(
            valorOriginal: 100m,
            dataVencimento: hoje,
            dataCalculo: hoje
        );

        Assert.Equal(0, resultado.DiasEmAtraso);
        Assert.Equal(0m, resultado.ValorJuros);
        Assert.Equal(100m, resultado.ValorTotal);
    }

    [Fact]
    public void Calcular_ComVencimentoFuturo_NaoDeveAplicarJuros()
    {
        var resultado = _service.Calcular(
            valorOriginal: 100m,
            dataVencimento: new DateOnly(2026, 10, 10),
            dataCalculo: new DateOnly(2026, 10, 4)
        );

        Assert.Equal(0, resultado.DiasEmAtraso);
        Assert.Equal(0m, resultado.ValorJuros);
    }

    [Fact]
    public void Calcular_DeveArredondarJurosParaCentavos()
    {
        var resultado = _service.Calcular(
            valorOriginal: 100.20m,
            dataVencimento: new DateOnly(2026, 10, 3),
            dataCalculo: new DateOnly(2026, 10, 4)
        );

        Assert.Equal(2.51m, resultado.ValorJuros);
        Assert.Equal(102.71m, resultado.ValorTotal);
    }

    [Fact]
    public void Calcular_DeveRejeitarValorZero()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            _service.Calcular(
                valorOriginal: 0m,
                dataVencimento: new DateOnly(2026, 10, 1),
                dataCalculo: new DateOnly(2026, 10, 4)
            )
        );
    }
}