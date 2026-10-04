using System.Net;

namespace TargetDesafio.Tests.Api;

public sealed class FrontendTests
{
    [Fact]
    public async Task PaginaInicial_DeveServirFrontendESeusArquivos()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var pagina = await client.GetAsync("/");
        var html = await pagina.Content.ReadAsStringAsync();
        var estilo = await client.GetAsync("/styles.css");
        var script = await client.GetAsync("/app.js");

        Assert.Equal(HttpStatusCode.OK, pagina.StatusCode);
        Assert.Contains("Painel de operações", html);
        Assert.Contains("id=\"form-movimentacao\"", html);
        Assert.Contains("id=\"form-juros\"", html);
        Assert.Equal(HttpStatusCode.OK, estilo.StatusCode);
        Assert.Equal(HttpStatusCode.OK, script.StatusCode);
    }
}
