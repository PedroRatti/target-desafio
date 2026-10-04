using System.Globalization;
using System.Text.Json;
using TargetDesafio.Core.Comissoes;
using TargetDesafio.Core.Estoque;
using TargetDesafio.Core.Juros;

var opcoesJson = new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
};

var cultura = CultureInfo.GetCultureInfo("pt-BR");
EstoqueService? estoqueService = null;

while (true)
{
    Console.WriteLine("TARGET DESAFIO");
    Console.WriteLine();
    Console.WriteLine("1 - Calcular comissões");
    Console.WriteLine("2 - Movimentar estoque");
    Console.WriteLine("3 - Calcular juros");
    Console.WriteLine("0 - Sair");
    Console.WriteLine();
    Console.Write("Escolha uma opção: ");

    var opcao = Console.ReadLine();

    Console.WriteLine();

    try
    {
        switch (opcao)
        {
            case "1":
                await ExecutarComissoesAsync(opcoesJson, cultura);
                break;

            case "2":
                estoqueService ??= await CarregarEstoqueAsync(opcoesJson);

                ExecutarEstoque(estoqueService);
                break;

            case "3":
                ExecutarCalculoDeJuros(cultura);
                break;

            case "0":
                return;

            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
    catch (Exception exception)
    {
        Console.Error.WriteLine($"Erro: {exception.Message}");
    }

    Console.WriteLine();
    Console.WriteLine("Pressione Enter para continuar.");
    Console.ReadLine();
    Console.Clear();
}

static async Task ExecutarComissoesAsync(
    JsonSerializerOptions opcoesJson,
    CultureInfo cultura
)
{
    var arquivoVendas =
        await CarregarJsonAsync<ArquivoVendas>("vendas.json", opcoesJson);

    var service = new ComissaoService();
    var resultados = service.CalcularPorVendedor(arquivoVendas.Vendas);

    Console.WriteLine("COMISSÕES POR VENDEDOR");
    Console.WriteLine();

    foreach (var resultado in resultados)
    {
        Console.WriteLine($"Vendedor: {resultado.Vendedor}");
        Console.WriteLine($"Total vendido: {resultado.TotalVendido.ToString("C", cultura)}");
        Console.WriteLine($"Comissão: {resultado.TotalComissao.ToString("C", cultura)}");
        Console.WriteLine();
    }
}

static async Task<EstoqueService> CarregarEstoqueAsync(
    JsonSerializerOptions opcoesJson
)
{
    var arquivoEstoque =
        await CarregarJsonAsync<ArquivoEstoque>("estoque.json", opcoesJson);

    return new EstoqueService(arquivoEstoque.Estoque);
}

static void ExecutarEstoque(EstoqueService service)
{
    while (true)
    {
        Console.Clear();
        Console.WriteLine("MOVIMENTAÇÃO DE ESTOQUE");
        Console.WriteLine();

        foreach (var produto in service.ListarProdutos())
        {
            Console.WriteLine(
                $"{produto.CodigoProduto} - " +
                $"{produto.DescricaoProduto} | " +
                $"Estoque: {produto.Estoque}"
            );
        }

        Console.WriteLine();
        Console.WriteLine("1 - Registrar entrada");
        Console.WriteLine("2 - Registrar saída");
        Console.WriteLine("0 - Voltar");
        Console.WriteLine();
        Console.Write("Escolha uma opção: ");

        var opcao = Console.ReadLine();

        if (opcao == "0")
        {
            return;
        }

        TipoMovimentacao? tipo = opcao switch
        {
            "1" => TipoMovimentacao.Entrada,
            "2" => TipoMovimentacao.Saida,
            _ => null
        };

        if (tipo is null)
        {
            Console.WriteLine("Opção inválida.");
            Pausar();
            continue;
        }

        var codigoProduto =
            LerInteiroPositivo("Código do produto: ");

        var quantidade =
            LerInteiroPositivo("Quantidade: ");

        Console.Write("Descrição da movimentação: ");
        var descricao = Console.ReadLine() ?? string.Empty;

        try
        {
            var movimentacao =
                service.RegistrarMovimentacao(
                    codigoProduto,
                    tipo.Value,
                    quantidade,
                    descricao
                );

            Console.WriteLine();
            Console.WriteLine("MOVIMENTAÇÃO REGISTRADA");
            Console.WriteLine($"Identificador: {movimentacao.Identificador}");
            Console.WriteLine($"Produto: {movimentacao.DescricaoProduto}");
            Console.WriteLine($"Tipo: {movimentacao.Tipo}");
            Console.WriteLine($"Quantidade: {movimentacao.Quantidade}");
            Console.WriteLine($"Estoque anterior: {movimentacao.EstoqueAnterior}");
            Console.WriteLine($"Estoque final: {movimentacao.EstoqueFinal}");
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine();
            Console.Error.WriteLine($"Não foi possível registrar: {exception.Message}");
        }

        Pausar();
    }
}

static int LerInteiroPositivo(string mensagem)
{
    while (true)
    {
        Console.Write(mensagem);
        var entrada = Console.ReadLine();

        if (int.TryParse(entrada, out var valor) && valor > 0)
        {
            return valor;
        }

        Console.WriteLine("Informe um número inteiro maior que zero.");
    }
}

static async Task<T> CarregarJsonAsync<T>(string nomeArquivo, JsonSerializerOptions opcoesJson)
    where T : class
{
    var caminho = Path.Combine(
        AppContext.BaseDirectory,
        "data",
        nomeArquivo
    );

    if (!File.Exists(caminho))
    {
        throw new FileNotFoundException($"O arquivo {nomeArquivo} não foi encontrado.", caminho);
    }

    var json = await File.ReadAllTextAsync(caminho);

    return JsonSerializer.Deserialize<T>(json, opcoesJson) ?? throw new InvalidOperationException($"Não foi possível interpretar {nomeArquivo}.");
}

static void ExecutarCalculoDeJuros(CultureInfo cultura)
{
    Console.WriteLine("CÁLCULO DE JUROS");
    Console.WriteLine();

    var valorOriginal = LerValorMonetario("Valor original, por exemplo 1000,50: ", cultura);

    var dataVencimento = LerData("Data de vencimento no formato dd/MM/yyyy: ", cultura);

    var dataCalculo = DateOnly.FromDateTime(DateTime.Today);

    var service = new JurosService();
    var resultado = service.Calcular(
        valorOriginal,
        dataVencimento,
        dataCalculo
    );

    Console.WriteLine();
    Console.WriteLine("RESULTADO");
    Console.WriteLine($"Valor original: {resultado.ValorOriginal.ToString("C", cultura)}");
    Console.WriteLine($"Vencimento: {resultado.DataVencimento:dd/MM/yyyy}");
    Console.WriteLine($"Data do cálculo: {resultado.DataCalculo:dd/MM/yyyy}");
    Console.WriteLine($"Dias em atraso: {resultado.DiasEmAtraso}");
    Console.WriteLine($"Taxa diária: {resultado.TaxaDiaria.ToString("P2", cultura)}");
    Console.WriteLine($"Valor dos juros: {resultado.ValorJuros.ToString("C", cultura)}");
    Console.WriteLine($"Valor total: {resultado.ValorTotal.ToString("C", cultura)}");
}

static decimal LerValorMonetario(string mensagem, CultureInfo cultura)
{
    while (true)
    {
        Console.Write(mensagem);
        var entrada = Console.ReadLine();

        if (
            decimal.TryParse(
                entrada,
                NumberStyles.Number,
                cultura,
                out var valor
            )
            && valor > 0
        )
        {
            return valor;
        }

        Console.WriteLine("Informe um valor monetário maior que zero. Exemplo: 1000,50.");
    }
}

static DateOnly LerData(string mensagem, CultureInfo cultura)
{
    while (true)
    {
        Console.Write(mensagem);
        var entrada = Console.ReadLine();

        if (
            DateOnly.TryParseExact(
                entrada,
                "dd/MM/yyyy",
                cultura,
                DateTimeStyles.None,
                out var data
            )
        )
        {
            return data;
        }

        Console.WriteLine("Informe uma data válida no formato dd/MM/yyyy.");
    }
}

static void Pausar()
{
    Console.WriteLine();
    Console.WriteLine("Pressione Enter para continuar.");
    Console.ReadLine();
}