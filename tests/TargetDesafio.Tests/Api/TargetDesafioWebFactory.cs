using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TargetDesafio.Core.Comissoes;
using TargetDesafio.Core.Estoque;

namespace TargetDesafio.Tests.Api;

public sealed class TargetDesafioWebFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(
        IWebHostBuilder builder
    )
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ArquivoVendas>();
            services.RemoveAll<EstoqueService>();

            services.AddSingleton(
                new ArquivoVendas
                {
                    Vendas =
                    [
                        new Venda("Ana", 100m),
                        new Venda("Ana", 500m),
                        new Venda("Bruno", 50m)
                    ]
                }
            );

            services.AddSingleton(
                new EstoqueService(
                    [
                        new Produto(
                            codigoProduto: 101,
                            descricaoProduto: "Caneta Azul",
                            estoque: 150
                        )
                    ]
                )
            );
        });
    }
}
