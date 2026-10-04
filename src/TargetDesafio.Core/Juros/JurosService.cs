namespace TargetDesafio.Core.Juros;

public sealed class JurosService
{
    public const decimal TaxaDiaria = 0.025m;

    public ResultadoJuros Calcular(decimal valorOriginal, DateOnly dataVencimento, DateOnly dataCalculo)
    {
        if (valorOriginal <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(valorOriginal), "O valor original deve ser maior que zero.");
        }

        var diasEmAtraso = dataCalculo > dataVencimento
            ? dataCalculo.DayNumber - dataVencimento.DayNumber
            : 0;

        var valorJuros = decimal.Round(
            valorOriginal * TaxaDiaria * diasEmAtraso,
            2,
            MidpointRounding.AwayFromZero
        );

        var valorTotal = valorOriginal + valorJuros;

        return new ResultadoJuros(
            ValorOriginal: valorOriginal,
            DataVencimento: dataVencimento,
            DataCalculo: dataCalculo,
            DiasEmAtraso: diasEmAtraso,
            TaxaDiaria: TaxaDiaria,
            ValorJuros: valorJuros,
            ValorTotal: valorTotal
        );
    }
}