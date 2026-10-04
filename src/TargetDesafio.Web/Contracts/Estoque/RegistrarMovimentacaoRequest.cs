using TargetDesafio.Core.Estoque;

namespace TargetDesafio.Web.Contracts.Estoque;

public sealed record RegistrarMovimentacaoRequest(
    int CodigoProduto,
    TipoMovimentacao Tipo,
    int Quantidade,
    string Descricao
);
