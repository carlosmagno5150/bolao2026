---
name: project-creator
description: Use esta skill sempre que o usuário quiser criar uma solução .NET do zero, montar um novo projeto, estruturar uma aplicação .NET, scaffoldar uma API, Worker Service, Console App ou Class Library, ou organizar projetos dentro de uma solution (.sln). Acione também quando o usuário mencionar "criar projeto", "nova solução", "scaffolding .NET", "estrutura de pastas", "adicionar projeto à solution" ou pedir para configurar pacotes NuGet em projetos novos. Não espere o usuário dizer explicitamente "use a skill" — se o contexto envolver criação de estrutura .NET, esta skill deve ser usada.
---

# Project Creator

Criação de soluções e projetos .NET de forma padronizada via `dotnet CLI`.

## Conceitos-chave

- **Solution (.sln)**: Contêiner organizador que agrupa projetos relacionados. Facilita build simultâneo e compartilhamento de configurações.
- **Project (.csproj)**: Contém o código-fonte real. Compila em um binário (executável ou biblioteca reutilizável).

---

## ⚠️ Compatibilidade de Sistema Operacional

**Sempre detecte o SO do usuário antes de gerar comandos.** A sintaxe de caminhos e criação de pastas difere entre plataformas.

| SO | Shell | Separador de caminho | Criar pasta | Continuação de linha |
|---|---|---|---|---|
| Windows | PowerShell | `\` | `New-Item -ItemType Directory` | `` ` `` (backtick) |
| Windows | CMD | `\` | `mkdir` | `^` |
| Linux / macOS | Bash/Zsh | `/` | `mkdir -p` | `\` |

> **Regra de ouro para Windows**: use `.\` no início dos caminhos (não `./`), use `\` como separador, e nunca use `\` no fim da linha como continuação — no PowerShell use `` ` `` e no CMD use `^`.

---

## Fluxo de trabalho

### 1. Levantar requisitos

Antes de executar qualquer comando, entenda:

- **SO / shell** do usuário (Windows PowerShell, Windows CMD, Linux/macOS Bash)
- **Nome da solution** (ex: `MinhaEmpresa.Produto`)
- **Tipo(s) de projeto** desejado(s) — ver tabela abaixo
- **Arquitetura** preferida (Clean Architecture, DDD, Minimal, simples)
- **Versão do .NET** (perguntar ao usuário; sugerir a LTS mais recente se não souber)
- **Pacotes NuGet** necessários (inferir do contexto; ver seção de pacotes)

Se o usuário não especificar, pergunte de forma direta e concisa. Não assuma arquitetura ou SO sem confirmar.

---

### 2. Tipos de projeto suportados

| Tipo | Template CLI | Uso típico |
|---|---|---|
| Web API | `webapi` | APIs REST com controllers ou Minimal API |
| Worker Service | `worker` | Processamento em background, filas |
| Console App | `console` | CLIs, scripts, jobs agendados |
| Class Library | `classlib` | Código compartilhado entre projetos |

---

### 3. Comandos base por plataforma

Os comandos `dotnet` são iguais em todos os SOs. O que muda são os **caminhos** e a **criação de pastas internas**.

#### PowerShell (Windows)
```powershell
# Criar solution
dotnet new sln -n NomeDaSolution -o .\NomeDaSolution

# Criar projeto
dotnet new <template> -n NomeDoProjeto -o .\NomeDaSolution\src\NomeDoProjeto --framework net<versao>

# Adicionar projeto à solution
dotnet sln .\NomeDaSolution\NomeDaSolution.sln add .\NomeDaSolution\src\NomeDoProjeto\NomeDoProjeto.csproj

# Referenciar projeto em outro
dotnet add .\Projeto.A\Projeto.A.csproj reference .\Projeto.B\Projeto.B.csproj

# Adicionar pacote NuGet
dotnet add .\Projeto\Projeto.csproj package NomeDoPacote

# Criar pastas internas (PowerShell)
New-Item -ItemType Directory -Force -Path .\NomeDaSolution\src\NomeDoProjeto\Controllers
New-Item -ItemType Directory -Force -Path .\NomeDaSolution\src\NomeDoProjeto\Services
```

#### CMD (Windows)
```cmd
:: Criar solution
dotnet new sln -n NomeDaSolution -o .\NomeDaSolution

:: Criar projeto
dotnet new <template> -n NomeDoProjeto -o .\NomeDaSolution\src\NomeDoProjeto --framework net<versao>

:: Criar pastas internas (CMD)
mkdir .\NomeDaSolution\src\NomeDoProjeto\Controllers
mkdir .\NomeDaSolution\src\NomeDoProjeto\Services
```

#### Bash — Linux / macOS
```bash
# Criar solution
dotnet new sln -n NomeDaSolution -o ./NomeDaSolution

# Criar projeto
dotnet new <template> -n NomeDoProjeto -o ./NomeDaSolution/src/NomeDoProjeto --framework net<versao>

# Criar pastas internas (Bash)
mkdir -p ./NomeDaSolution/src/NomeDoProjeto/{Controllers,Services,Models}
```

---

### 4. Estrutura de pastas padrão por arquitetura

Leia o arquivo de referência correspondente à arquitetura escolhida:

- **Clean Architecture** → `references/clean-architecture.md`
- **DDD** → `references/ddd.md`
- **Minimal / Simples** → `references/minimal.md`

Cada arquivo de referência contém comandos separados para **PowerShell**, **CMD** e **Bash**.

Se o usuário não escolheu arquitetura, apresente as opções e aguarde a escolha antes de gerar qualquer comando.

---

### 5. Pacotes NuGet

Instale apenas os pacotes que o usuário pedir ou que sejam claramente necessários para o contexto. Não instale pacotes sem motivo.

Referência de pacotes comuns → `references/nuget-packages.md`

---

### 6. Verificação final

Após criar a estrutura, execute (igual em todos os SOs):

```
dotnet build .\NomeDaSolution\NomeDaSolution.sln     # Windows
dotnet build ./NomeDaSolution/NomeDaSolution.sln     # Linux/macOS
```

Se houver erros, corrija antes de apresentar o resultado ao usuário.

---

## Regras importantes

- **Sempre use o dotnet CLI** — não crie arquivos `.csproj` ou `.sln` manualmente.
- **Detecte o SO** e gere comandos com a sintaxe correta de caminhos (`\` no Windows, `/` no Linux/macOS).
- **No Windows, nunca use `./`** — use `.\` para caminhos relativos.
- **No PowerShell, nunca use `\` para continuar linha** — use `` ` `` (backtick). No CMD use `^`.
- **Confirme a versão do .NET** antes de criar projetos; use `--framework netX.X` explicitamente.
- **Nomes de projeto** devem seguir PascalCase e refletir a responsabilidade do projeto (ex: `MinhaApp.Application`, `MinhaApp.Infrastructure`).
- **Nunca instale pacotes desnecessários** — pergunte ou infira do contexto.
- **Crie as pastas internas** dos projetos (Controllers/, Services/, etc.) conforme o arquivo de referência da arquitetura escolhida.
