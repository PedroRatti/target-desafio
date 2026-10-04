using System.Text.Json;

namespace TargetDesafio.Web.Infrastructure;

public static class CarregadorJson
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static T Carregar<T>(string nomeArquivo)
        where T : class
    {
        var caminho = Path.Combine(AppContext.BaseDirectory, "data", nomeArquivo);

        if (!File.Exists(caminho))
        {
            throw new FileNotFoundException($"O arquivo {nomeArquivo} não foi encontrado.", caminho);
        }

        var json = File.ReadAllText(caminho);

        return JsonSerializer.Deserialize<T>(json, Opcoes) ?? throw new InvalidOperationException($"Não foi possível interpretar {nomeArquivo}.");
    }
}