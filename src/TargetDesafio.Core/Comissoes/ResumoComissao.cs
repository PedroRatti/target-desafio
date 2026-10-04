namespace TargetDesafio.Core.Comissoes;

public sealed record ResumoComissao(
    string Vendedor,
    decimal TotalVendido,
    decimal TotalComissao
);