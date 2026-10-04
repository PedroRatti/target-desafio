using System.Text.Json;
using System.Text.Json.Serialization;
using TargetDesafio.Core.Comissoes;
using TargetDesafio.Core.Estoque;
using TargetDesafio.Core.Juros;
using TargetDesafio.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(
                JsonNamingPolicy.CamelCase,
                allowIntegerValues: false
            )
        );
    });
builder.Services.AddOpenApi();

builder.Services.AddSingleton<ComissaoService>();
builder.Services.AddSingleton<JurosService>();

builder.Services.AddSingleton(
    _ => CarregadorJson.Carregar<ArquivoVendas>(
        "vendas.json"
    )
);

builder.Services.AddSingleton(
    _ =>
    {
        var arquivoEstoque =
            CarregadorJson.Carregar<ArquivoEstoque>(
                "estoque.json"
            );

        return new EstoqueService(arquivoEstoque.Estoque);
    }
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program;
