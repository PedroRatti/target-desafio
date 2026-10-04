using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TargetDesafio.Core.Estoque;

namespace TargetDesafio.Tests.Api;

public sealed class EstoqueApiTests
{
    [Fact]
    public async Task GetEstoque_DeveRetornarProdutos()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/estoque");
        var produtos =
            await response.Content.ReadFromJsonAsync<List<Produto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var produto = Assert.Single(Assert.IsType<List<Produto>>(produtos));
        Assert.Equal(101, produto.CodigoProduto);
        Assert.Equal(150, produto.Estoque);
    }

    [Fact]
    public async Task PostMovimentacao_DeveAtualizarEstoqueEmMemoria()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/estoque/movimentacoes",
            new
            {
                codigoProduto = 101,
                tipo = "saida",
                quantidade = 30,
                descricao = "Venda para cliente"
            }
        );

        var movimentacaoCriada = await response.Content
            .ReadFromJsonAsync<JsonElement>();

        var produto = await client.GetFromJsonAsync<Produto>(
            "/api/estoque/101"
        );

        var responseMovimentacoes = await client.GetAsync(
            "/api/estoque/movimentacoes"
        );

        var movimentacoes = await responseMovimentacoes.Content
            .ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal(
            JsonValueKind.Number,
            movimentacaoCriada.GetProperty("identificador").ValueKind
        );
        Assert.Equal(
            1L,
            movimentacaoCriada.GetProperty("identificador").GetInt64()
        );
        Assert.NotNull(produto);
        Assert.Equal(120, produto.Estoque);
        Assert.Equal(
            HttpStatusCode.OK,
            responseMovimentacoes.StatusCode
        );
        Assert.Equal(JsonValueKind.Array, movimentacoes.ValueKind);
        Assert.Equal(1, movimentacoes.GetArrayLength());
        Assert.Equal(
            "saida",
            movimentacoes[0].GetProperty("tipo").GetString()
        );
    }

    [Fact]
    public async Task GetProdutoInexistente_DeveRetornarNotFound()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/estoque/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PostSaidaAcimaDoSaldo_DeveRetornarConflict()
    {
        using var factory = new TargetDesafioWebFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/estoque/movimentacoes",
            new
            {
                codigoProduto = 101,
                tipo = "saida",
                quantidade = 151,
                descricao = "Venda sem saldo"
            }
        );

        var produto = await client.GetFromJsonAsync<Produto>(
            "/api/estoque/101"
        );

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(produto);
        Assert.Equal(150, produto.Estoque);
    }
}
