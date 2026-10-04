# Desafio técnico — Target Sistemas

Este repositório reúne as soluções dos três exercícios do desafio: cálculo de comissões, movimentação de estoque e cálculo de juros por atraso.

O projeto tem duas formas de uso. A versão **CLI** roda no terminal e permite testar cada exercício por um menu. A versão **Web** oferece uma API em ASP.NET Core e uma página simples em HTML, CSS e JavaScript. Optei por manter as duas porque, no dia a dia, uma mesma regra de negócio pode precisar atender interfaces diferentes. Por isso, os cálculos e as validações ficam no `TargetDesafio.Core`, compartilhado pela CLI e pela Web.

## Como rodar

É necessário ter o **SDK do .NET 10** instalado. No terminal, a partir da raiz do repositório, restaure as dependências:

```powershell
dotnet restore .\TargetDesafio.sln
```

Para usar a versão de terminal:

```powershell
dotnet run --project .\src\TargetDesafio.Cli\TargetDesafio.Cli.csproj
```

Para usar a versão Web:

```powershell
dotnet run --project .\src\TargetDesafio.Web\TargetDesafio.Web.csproj --launch-profile http
```

Depois, abra [http://localhost:5166](http://localhost:5166) no navegador. A página permite consultar as comissões, registrar entradas e saídas de estoque e calcular os juros.

Para executar os testes:

```powershell
dotnet test .\TargetDesafio.sln
```

## Dados e continuidade

Os arquivos iniciais de vendas e produtos estão em `data/`. As movimentações e o saldo atualizado do estoque ficam apenas em memória; ao reiniciar a aplicação, o estoque volta aos valores de `estoque.json`. Não é preciso instalar banco de dados nem Node.js para rodar o projeto.

Uma próxima versão poderia usar um banco de dados local em um container Docker. Assim, o saldo e o histórico de movimentações continuariam disponíveis mesmo depois de reiniciar a aplicação.
