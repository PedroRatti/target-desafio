namespace TargetDesafio.Core.Estoque;

public sealed record MovimentacaoEstoque(
    long Identificador,
    int CodigoProduto,
    string DescricaoProduto,
    TipoMovimentacao Tipo,
    string Descricao,
    int Quantidade,
    int EstoqueAnterior,
    int EstoqueFinal,
    DateTimeOffset RegistradaEm
);