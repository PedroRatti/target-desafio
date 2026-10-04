using System.Net;
using System.Net.Http.Json;
using TargetDesafio.Core.Juros;

namespace TargetDesafio.Tests.Api;

public sealed class JurosApiTests
{
    [Fact]
    public async Task PostCalcularJuros_DeveRetornarCalculo()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var vencimento =
            DateOnly.FromDateTime(DateTime.Today).AddDays(-3);

        var response = await client.PostAsJsonAsync(
            "/api/juros/calcular",
            new
            {
                valorOriginal = 100m,
                dataVencimento = vencimento
            }
        );

        var resultado =
            await response.Content.ReadFromJsonAsync<ResultadoJuros>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(resultado);
        Assert.Equal(3, resultado.DiasEmAtraso);
        Assert.Equal(7.50m, resultado.ValorJuros);
        Assert.Equal(107.50m, resultado.ValorTotal);
    }

    [Fact]
    public async Task PostCalcularJurosComValorZero_DeveRetornarBadRequest()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/juros/calcular",
            new
            {
                valorOriginal = 0m,
                dataVencimento = DateOnly.FromDateTime(DateTime.Today)
            }
        );

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
