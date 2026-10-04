namespace TargetDesafio.Core.Comissoes;

public sealed class ComissaoService
{
    public decimal CalcularComissao(decimal valorVenda)
    {
        if (valorVenda < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorVenda), "O valor da venda não pode ser negativo.");
        }

        if (valorVenda < 100m)
        {
            return 0m;
        }

        decimal percentual = valorVenda < 500m ? 0.01m : 0.05m;
        return decimal.Round(valorVenda * percentual, 2, MidpointRounding.AwayFromZero);
    }

    public IReadOnlyList<ResumoComissao> CalcularPorVendedor(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        var listaDeVendas = vendas.ToList();

        if (listaDeVendas.Any(venda => string.IsNullOrWhiteSpace(venda.Vendedor)))
        {
            throw new ArgumentException("Todas as vendas devem possuir um vendedor.");
        }

        if (listaDeVendas.Any(venda => venda.Valor < 0))
        {
            throw new ArgumentException("As vendas não podem possuir valores negativos.");
        }

        return listaDeVendas
            .GroupBy(venda => venda.Vendedor.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(grupo => new ResumoComissao(
                Vendedor: grupo.Key,
                TotalVendido: grupo.Sum(venda => venda.Valor),
                TotalComissao: grupo.Sum(venda => CalcularComissao(venda.Valor))
            ))
            .OrderBy(resultado => resultado.Vendedor)
            .ToList();
    }
}