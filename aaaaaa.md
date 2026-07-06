# Projeto

API para consulta de dados da Copa do Mundo 2026: grupos, seleções, partidas e classificação (ranking).

## Visão geral

A API permite cadastrar seleções e partidas, e a partir desses dados calcular e consultar
classificações por grupo (pontos, vitórias, saldo de gols, etc.).

## Tecnologias

- .NET 10
- ASP.NET Core Web API
- AWS DynamoDb
- xUnit (testes, ver seção "Testes")

## Arquitetura

- Projeto único (monolito modular), sem separação em múltiplos projetos/camadas físicas.
- **Vertical Slice Architecture**: o código é organizado por funcionalidade (feature), não por
  camada técnica. Cada slice contém tudo que precisa para resolver seu caso de uso (request,
  handler, validação, endpoint), evitando o acoplamento típico de camadas horizontais
  (Controllers / Services / Repositories genéricos).
- Evitar repositório genérico e camadas de serviço "anêmicas" que só repassam chamadas ao EF Core.
- Cada slice deve ser independente e de fácil localização: agrupar pasta por feature
  (ex: `Features/Selecoes/CriarSelecao`, `Features/Partidas/RegistrarResultado`).

## Escopo funcional

### Seleções
- Cadastro (criar, editar, listar, buscar por id)
- Atributos mínimos: nome, código/sigla (ex: BRA), grupo, ranking FIFA (opcional)

### Partidas
- Cadastro (criar, editar, listar, buscar por id)
- Atributos mínimos: seleção mandante, seleção visitante, grupo/fase, data, placar
  (quando já disputada), status (agendada / realizada)

### Grupos e Ranking (derivado)
- Consulta de seleções por grupo
- Cálculo de classificação por grupo com base nas partidas cadastradas:
  pontos, vitórias, empates, derrotas, gols pró, gols contra, saldo de gols

> Observação: cadastro de Grupos e Ranking não são entidades próprias — grupos são um atributo
> da seleção/partida, e o ranking é calculado a partir das partidas.

## Estrutura física

Todo o código-fonte deve ser criado dentro da pasta `src/`, na raiz do workspace atual.

Estrutura desejada:

```
src/
  CopaApi/
    CopaApi.slnx
    CopaApi/
      CopaApi.csproj
      Program.cs
      Features/
        Selecoes/
        Partidas/
      Data/
        CopaApiDbContext.cs
        Migrations/
    CopaApi.Tests/
      CopaApi.Tests.csproj
```

- Utilizar o formato de solução com extensão **`.slnx`** para o arquivo de solução
  (não usar o formato clássico `.sln`).
- O projeto de testes (`CopaApi.Tests`) deve ficar ao lado do projeto principal, dentro de
  `src/CopaApi/`, e ser referenciado na `.slnx`.

## Banco de dados

- SQLite como banco de dados, com arquivo local (ex: `copaapi.db`).
- Usar **EF Core Migrations** para versionar o schema — não usar `EnsureCreated()`.
- Toda alteração de modelo deve vir acompanhada de uma migration correspondente.

## Convenções de código

- Nomes de classes, propriedades e namespaces em inglês; nomes de domínio (ex: "Seleção",
  "Partida") podem manter-se em português caso já estejam refletidos no restante do projeto —
  manter consistência, sem misturar idiomas na mesma entidade.
- Usar `record` para DTOs/requests/responses.
- Validar entrada com `FluentValidation` (ou Data Annotations, se preferir simplicidade) antes de
  persistir.
- Endpoints organizados via **Minimal APIs**, agrupados por slice/feature usando
  `MapGroup` ou extensões de `IEndpointRouteBuilder`.
- Não usar Controllers do MVC clássico.

## Testes

- Cobrir cada slice com testes de unidade (regras de negócio, ex: cálculo de classificação) e
  testes de integração dos endpoints (usando `WebApplicationFactory` + SQLite em memória).

## Fora de escopo (por ora)

- Autenticação/autorização
- Integração com fontes externas de dados (ex: API oficial da FIFA)
- Front-end / interface visual
