const moeda = new Intl.NumberFormat("pt-BR", {
  style: "currency",
  currency: "BRL"
});

const numero = new Intl.NumberFormat("pt-BR");

function elemento(id) {
  return document.getElementById(id);
}

function mostrarStatus(id, mensagem, estado = "") {
  const alvo = elemento(id);
  alvo.textContent = mensagem;
  alvo.dataset.state = estado;
}

function celula(valor, classe = "") {
  const td = document.createElement("td");
  td.textContent = String(valor);
  if (classe) td.className = classe;
  return td;
}

async function requisitar(caminho, opcoes) {
  const resposta = await fetch(caminho, opcoes);

  if (!resposta.ok) {
    let detalhe = "Não foi possível concluir a operação.";

    try {
      const erro = await resposta.json();
      detalhe = erro.detail || erro.title || detalhe;
    } catch {
      // A resposta pode não conter JSON.
    }

    throw new Error(detalhe);
  }

  return resposta.json();
}

async function carregarComissoes() {
  const botao = elemento("atualizar-comissoes");
  botao.disabled = true;
  mostrarStatus("status-comissoes", "Carregando comissões...");

  try {
    const comissoes = await requisitar("/api/comissoes");
    const linhas = comissoes.map((item) => {
      const tr = document.createElement("tr");
      tr.append(
        celula(item.vendedor),
        celula(moeda.format(item.totalVendido), "numeric"),
        celula(moeda.format(item.totalComissao), "numeric")
      );
      return tr;
    });

    elemento("lista-comissoes").replaceChildren(...linhas);
    elemento("quadro-comissoes").hidden = linhas.length === 0;
    mostrarStatus("status-comissoes", linhas.length ? "" : "Nenhuma venda cadastrada.");
  } catch (erro) {
    mostrarStatus("status-comissoes", erro.message, "error");
  } finally {
    botao.disabled = false;
  }
}

async function carregarEstoque() {
  const botao = elemento("atualizar-estoque");
  const seletor = elemento("produto");
  const codigoSelecionado = seletor.value;
  botao.disabled = true;
  mostrarStatus("status-estoque", "Carregando produtos...");

  try {
    const produtos = await requisitar("/api/estoque");
    const linhas = produtos.map((produto) => {
      const tr = document.createElement("tr");
      tr.append(
        celula(produto.codigoProduto),
        celula(produto.descricaoProduto),
        celula(numero.format(produto.estoque), "numeric")
      );
      return tr;
    });

    const placeholder = new Option("Selecione um produto", "");
    const opcoes = produtos.map((produto) =>
      new Option(`${produto.codigoProduto} · ${produto.descricaoProduto}`, produto.codigoProduto)
    );

    elemento("lista-estoque").replaceChildren(...linhas);
    seletor.replaceChildren(placeholder, ...opcoes);
    seletor.value = produtos.some((produto) => String(produto.codigoProduto) === codigoSelecionado)
      ? codigoSelecionado
      : "";
    seletor.disabled = produtos.length === 0;
    elemento("enviar-movimentacao").disabled = produtos.length === 0;
    elemento("quadro-estoque").hidden = linhas.length === 0;
    mostrarStatus("status-estoque", linhas.length ? "" : "Nenhum produto cadastrado.");
  } catch (erro) {
    mostrarStatus("status-estoque", erro.message, "error");
  } finally {
    botao.disabled = false;
  }
}

async function carregarHistorico() {
  mostrarStatus("status-historico", "Carregando histórico...");

  try {
    const movimentacoes = await requisitar("/api/estoque/movimentacoes");
    const linhas = [...movimentacoes].reverse().map((item) => {
      const tr = document.createElement("tr");
      tr.append(
        celula(new Date(item.registradaEm).toLocaleString("pt-BR")),
        celula(item.descricaoProduto),
        celula(item.tipo === "entrada" ? "Entrada" : "Saída"),
        celula(numero.format(item.quantidade), "numeric"),
        celula(numero.format(item.estoqueFinal), "numeric")
      );
      return tr;
    });

    elemento("lista-historico").replaceChildren(...linhas);
    elemento("quadro-historico").hidden = linhas.length === 0;
    mostrarStatus("status-historico", linhas.length ? "" : "Ainda não há movimentações.");
  } catch (erro) {
    mostrarStatus("status-historico", erro.message, "error");
  }
}

elemento("atualizar-comissoes").addEventListener("click", carregarComissoes);
elemento("atualizar-estoque").addEventListener("click", async () => {
  await Promise.all([carregarEstoque(), carregarHistorico()]);
});

elemento("form-movimentacao").addEventListener("submit", async (evento) => {
  evento.preventDefault();
  const form = evento.currentTarget;
  const botao = elemento("enviar-movimentacao");
  const dados = new FormData(form);
  botao.disabled = true;
  mostrarStatus("status-movimentacao", "Registrando movimentação...");

  try {
    await requisitar("/api/estoque/movimentacoes", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        codigoProduto: Number(dados.get("codigoProduto")),
        tipo: dados.get("tipo"),
        quantidade: Number(dados.get("quantidade")),
        descricao: String(dados.get("descricao")).trim()
      })
    });

    form.elements.quantidade.value = "";
    form.elements.descricao.value = "";
    mostrarStatus("status-movimentacao", "Movimentação registrada com sucesso.", "success");
    await Promise.all([carregarEstoque(), carregarHistorico()]);
  } catch (erro) {
    mostrarStatus("status-movimentacao", erro.message, "error");
  } finally {
    botao.disabled = elemento("produto").options.length <= 1;
  }
});

elemento("form-juros").addEventListener("submit", async (evento) => {
  evento.preventDefault();
  const botao = elemento("enviar-juros");
  const dados = new FormData(evento.currentTarget);
  botao.disabled = true;
  elemento("resultado-juros").hidden = true;
  mostrarStatus("status-juros", "Calculando...");

  try {
    const resultado = await requisitar("/api/juros/calcular", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        valorOriginal: Number(dados.get("valorOriginal")),
        dataVencimento: dados.get("dataVencimento")
      })
    });

    elemento("juros-dias").textContent = numero.format(resultado.diasEmAtraso);
    elemento("juros-valor").textContent = moeda.format(resultado.valorJuros);
    elemento("juros-total").textContent = moeda.format(resultado.valorTotal);
    elemento("resultado-juros").hidden = false;
    mostrarStatus("status-juros", "");
  } catch (erro) {
    mostrarStatus("status-juros", erro.message, "error");
  } finally {
    botao.disabled = false;
  }
});

void Promise.all([carregarComissoes(), carregarEstoque(), carregarHistorico()]);
