namespace TargetDesafio.Core.Juros;

public sealed record ResultadoJuros(
    decimal ValorOriginal,
    DateOnly DataVencimento,
    DateOnly DataCalculo,
    int DiasEmAtraso,
    decimal TaxaDiaria,
    decimal ValorJuros,
    decimal ValorTotal
);