namespace TargetDesafio.Web.Contracts.Juros;

public sealed record CalcularJurosRequest(
    decimal ValorOriginal,
    DateOnly DataVencimento
);
