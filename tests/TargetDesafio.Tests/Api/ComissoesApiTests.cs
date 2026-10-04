using System.Net;
using System.Net.Http.Json;
using TargetDesafio.Core.Comissoes;

namespace TargetDesafio.Tests.Api;

public sealed class ComissoesApiTests
{
    [Fact]
    public async Task GetComissoes_DeveRetornarResumoPorVendedor()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/comissoes");
        var resultados =
            await response.Content
                .ReadFromJsonAsync<List<ResumoComissao>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(resultados);

        var ana = Assert.Single(
            resultados,
            resultado => resultado.Vendedor == "Ana"
        );

        Assert.Equal(600m, ana.TotalVendido);
        Assert.Equal(26m, ana.TotalComissao);
    }
}
