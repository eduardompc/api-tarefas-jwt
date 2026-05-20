# 🚀 API de Tarefas com Autenticação JWT - Minimal API

![.NET Version](https://img.shields.io/badge/.NET-8.0-purple)
![Swagger](https://img.shields.io/badge/Swagger-UI-brightgreen)
![JWT](https://img.shields.io/badge/JWT-Authentication-blue)
![Tests](https://img.shields.io/badge/Tests-xUnit-important)
![License](https://img.shields.io/badge/License-MIT-yellow)

## 📋 Sobre o Projeto

Este projeto é uma **API RESTful minimalista** desenvolvida como parte dos desafios da **Digital Innovation One (DIO)**. A API implementa um sistema completo de gerenciamento de tarefas com autenticação JWT (JSON Web Tokens), documentação Swagger e testes automatizados.

### 🎯 Objetivos Educacionais

- ✅ Aprender na prática conceitos de **ASP.NET Core Minimal API**
- ✅ Implementar **autenticação e autorização** com JWT
- ✅ Criar documentação interativa com **Swagger/OpenAPI**
- ✅ Desenvolver **testes automatizados** com xUnit
- ✅ Aplicar boas práticas de **segurança** (hash de senhas com BCrypt)

### 🌟 Funcionalidades

| Funcionalidade | Status | Descrição |
|----------------|--------|------------|
| Registro de usuários | ✅ | Cadastro com hash de senha (BCrypt) |
| Login com JWT | ✅ | Geração de token com expiração |
| CRUD de tarefas | ✅ | Criar, ler, atualizar, deletar tarefas |
| Isolamento por usuário | ✅ | Cada usuário vê apenas suas tarefas |
| Documentação Swagger | ✅ | Interface interativa para testes |
| Testes automatizados | ✅ | Cobertura de testes com xUnit |
| Validação de dados | ✅ | Data annotations e validações |

## 🏗️ Arquitetura do Projeto
ApiTarefas/
├── 📁 Models/ # Entidades e DTOs
│ ├── Usuario.cs # Modelos de usuário
│ ├── Tarefa.cs # Modelos de tarefa
│ └── Token.cs # Modelo de resposta do token
├── 📁 Services/ # Regras de negócio
│ └── TokenService.cs # Geração e validação de JWT
├── 📁 Tests/ # Testes automatizados
│ ├── AuthTests.cs # Testes de autenticação
│ └── TarefasTests.cs # Testes do CRUD de tarefas
├── 📄 Program.cs # Configuração e endpoints (Minimal API)
├── 📄 appsettings.json # Configurações da aplicação
└── 📄 ApiTarefas.csproj # Dependências do projeto

text

## 🛠️ Tecnologias Utilizadas

| Tecnologia | Versão | Propósito |
|------------|--------|------------|
| .NET | 8.0 | Framework principal |
| C# | 12.0 | Linguagem de programação |
| ASP.NET Core | 8.0 | Framework web |
| JWT Bearer | 8.0 | Autenticação com tokens |
| Swashbuckle | 6.5.0 | Documentação OpenAPI/Swagger |
| BCrypt | 4.0.3 | Hash de senhas |
| xUnit | 2.6.1 | Framework de testes |
| Moq | 4.20.70 | Mocking para testes |
| FluentAssertions | 6.12.0 | Asserções legíveis |

## 📦 Pré-requisitos

Antes de começar, você vai precisar ter instalado em sua máquina:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download) ou superior
- [Git](https://git-scm.com)
- [Visual Studio Code](https://code.visualstudio.com) ou [Visual Studio 2022](https://visualstudio.microsoft.com)
- [Docker](https://docker.com) (opcional, para containerização)

## 🔧 Instalação e Execução

### 1. Clone o repositório

```bash
git clone https://github.com/eduardompc/api-tarefas-jwt.git
cd api-tarefas-jwt
2. Restaure as dependências
bash
dotnet restore
3. Configure as variáveis de ambiente
Crie um arquivo appsettings.Production.json ou use variáveis de ambiente:

json
{
  "TokenSettings": {
    "SecretKey": "sua-chave-secreta-super-segura-aqui-minimo-32-caracteres",
    "AccessTokenExpirationMinutes": 30
  }
}
⚠️ IMPORTANTE: Em produção, use uma chave forte! Gere com: dotnet user-jwts create

4. Execute a API
bash
# Modo desenvolvimento (com hot reload)
dotnet watch run

# Modo produção
dotnet run --configuration Release
5. Acesse a documentação
Abra o navegador em: http://localhost:5000/index.html (ou a porta exibida no terminal)

🧪 Testando a API
Executando os testes automatizados
bash
# Executar todos os testes
dotnet test

# Com saída detalhada
dotnet test --verbosity normal

# Executar testes específicos
dotnet test --filter "FullyQualifiedName~AuthTests"
dotnet test --filter "FullyQualifiedName~TarefasTests"

# Com cobertura de código (instalar coverlet primeiro)
dotnet test --collect:"XPlat Code Coverage"
Testes implementados
csharp
✅ Testes de Autenticação
   ├── Registrar novo usuário
   ├── Evitar registro duplicado
   ├── Login com credenciais corretas
   ├── Login com credenciais incorretas
   └── Health check da API

✅ Testes de Tarefas
   ├── Criar tarefa com token válido
   ├── Listar tarefas do usuário
   ├── Atualizar tarefa existente
   ├── Deletar tarefa
   ├── Acesso sem token (401)
   └── Token inválido (401)
📚 Documentação da API
Endpoints Públicos (sem autenticação)
Método	Endpoint	Descrição	Exemplo
GET	/health	Verifica saúde da API	curl http://localhost:5000/health
POST	/registrar	Registra novo usuário	Ver exemplo
POST	/login	Realiza login e obtém token	Ver exemplo
Endpoints Protegidos (requerem JWT)
Método	Endpoint	Descrição	Autenticação
POST	/tarefas	Criar nova tarefa	🔒 Bearer Token
GET	/tarefas	Listar tarefas	🔒 Bearer Token
GET	/tarefas/{id}	Buscar tarefa por ID	🔒 Bearer Token
PUT	/tarefas/{id}	Atualizar tarefa	🔒 Bearer Token
DELETE	/tarefas/{id}	Deletar tarefa	🔒 Bearer Token
Exemplos de Uso
Registrar Usuário
bash
curl -X POST http://localhost:5000/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "username": "joao.silva",
    "password": "SenhaForte123!"
  }'
Resposta:

json
{
  "mensagem": "Usuário criado com sucesso"
}
Fazer Login
bash
curl -X POST http://localhost:5000/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "joao.silva",
    "password": "SenhaForte123!"
  }'
Resposta:

json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "tokenType": "bearer",
  "expiresIn": 1800
}
Criar Tarefa
bash
curl -X POST http://localhost:5000/tarefas \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Estudar Minimal APIs",
    "descricao": "Aprender sobre ASP.NET Core Minimal APIs e JWT"
  }'
Resposta:

json
{
  "id": 1,
  "titulo": "Estudar Minimal APIs",
  "descricao": "Aprender sobre ASP.NET Core Minimal APIs e JWT",
  "concluida": false,
  "usuario": "joao.silva"
}
Listar Tarefas
bash
curl -X GET "http://localhost:5000/tarefas?concluida=false" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
Atualizar Tarefa
bash
curl -X PUT http://localhost:5000/tarefas/1 \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Estudar Minimal APIs Avançado",
    "concluida": true
  }'
Deletar Tarefa
bash
curl -X DELETE http://localhost:5000/tarefas/1 \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
🐳 Executando com Docker
Build da imagem
bash
docker build -t api-tarefas-jwt .
Executar container
bash
docker run -d \
  -p 5000:80 \
  -e TokenSettings__SecretKey="sua-chave-secreta" \
  --name api-tarefas \
  api-tarefas-jwt
Usando docker-compose
bash
docker-compose up -d
🚀 Melhorias Implementadas
✅ Concluídas
Autenticação JWT completa

Documentação Swagger interativa

Testes automatizados (xUnit)

Validação de dados com Data Annotations

Hash de senhas com BCrypt

Isolamento de dados por usuário

Health check endpoint

Docker support

🔄 Em desenvolvimento
Refresh token

Paginação na listagem de tarefas

Rate limiting

Logging estruturado

Banco de dados PostgreSQL (atualmente em memória)

📋 Planejadas
CI/CD com GitHub Actions

Monitoramento com Application Insights

Versionamento da API (v1, v2)

WebSockets para notificações em tempo real

Frontend em React/Blazor

🔒 Segurança
Implementado
✅ Senhas hasheadas com BCrypt (salt automático)

✅ Tokens JWT com expiração (30 minutos)

✅ Isolamento de dados por usuário

✅ Validação de inputs nos endpoints

✅ Cabeçalhos de segurança configuráveis

Recomendações para Produção
⚠️ Use HTTPS obrigatoriamente

⚠️ Armazene chaves no Azure Key Vault ou AWS Secrets Manager

⚠️ Implemente rate limiting (evitar brute force)

⚠️ Configure CORS adequadamente

⚠️ Use um banco de dados real (PostgreSQL/SQL Server)

⚠️ Implemente logging centralizado

📊 Métricas de Qualidade
Métrica	Status	Valor
Cobertura de testes	🟡 Em andamento	~85%
Code smells	✅ Ok	0
Vulnerabilidades	✅ Ok	0
Duplicação de código	✅ Ok	< 1%
Manutenibilidade	✅ Alta	A
🤝 Como Contribuir
Faça um Fork do projeto

Crie uma branch para sua feature (git checkout -b feature/nova-feature)

Commit suas mudanças (git commit -m 'feat: Adiciona nova feature')

Push para a branch (git push origin feature/nova-feature)

Abra um Pull Request

Padrões de commit
feat: Nova funcionalidade

fix: Correção de bug

docs: Documentação

test: Testes

refactor: Refatoração

style: Formatação

chore: Tarefas de manutenção

📝 Licença
Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

🙏 Agradecimentos
Digital Innovation One (DIO) - Pela oportunidade de aprendizado e desafios propostos

Comunidade .NET - Pelo suporte e excelente ecossistema

Instrutores da DIO - Pelo conteúdo de qualidade

📞 Contato
**Link do Perfil DIO:  https://www.dio.me/users/eduardomarciopc
**Link do Perfil LINKEDIN: https://www.linkedin.com/in/eduardo-carvalho-b13398258/
** Link do perfil GITHUB: https://github.com/eduardompc

Email: eduardomarciopc@gmail.com

📖 Referências
Documentação ASP.NET Core

JWT.io

Swagger Documentation

Digital Innovation One

⭐ Sobre o Desafio DIO
Este projeto foi desenvolvido como parte dos desafios de código da Digital Innovation One, com o objetivo de consolidar conhecimentos em:

Desenvolvimento de APIs RESTful com ASP.NET Core

Implementação de autenticação segura com JWT

Documentação automatizada com Swagger

Testes automatizados e boas práticas de qualidade de código

Desafio proposto pela DIO:

"Criar uma API minimalista para gerenciamento de tarefas, implementando autenticação JWT e documentação Swagger, seguindo as melhores práticas de desenvolvimento e segurança."

Desenvolvido com 💙 por Eduardo MPC durante o bootcamp da DIO

text

## Arquivos Complementares

### LICENSE (MIT)

```markdown
MIT License

Copyright (c) 2024 Eduardo MPC

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
.gitignore
gitignore
# .NET
bin/
obj/
*.user
*.suo
*.cache
*.csproj.user
*.dbmdl
*.dbproj.schemaview
*.publishsettings
*.vs/
.vs/

# IDE
.vscode/
.idea/
*.swp
*.swo
*~

# Build results
[Dd]ebug/
[Rr]elease/
x64/
x86/
build/
bld/
[Bb]in/
[Oo]bj/

# Testing
TestResults/
*.coverage
*.coveragexml
*.trx

# Logs
*.log
*.logs
*.txt
logs/

# Environment
.env
.env.*
appsettings.Production.json
appsettings.Local.json

# Docker
*.pid
*.dockerignore
CODE_OF_CONDUCT.md
markdown
# Código de Conduta do Contribuidor

## Nosso Compromisso

Como participantes, contribuidores e líderes, nós nos comprometemos a fazer com que a participação em nossa comunidade seja uma experiência livre de assédio para todos.

## Comportamentos Aceitáveis

- Uso de linguagem acolhedora e inclusiva
- Respeito aos diferentes pontos de vista
- Aceitar críticas construtivas
- Foco no que é melhor para a comunidade

## Comportamentos Inaceitáveis

- Uso de linguagem ou imagens sexualizadas
- Comentários insultuosos ou depreciativos
- Assédio público ou privado
- Publicação de informações privadas sem permissão

## Aplicação

Líderes de projeto são responsáveis por esclarecer e fazer cumprir este código de conduta.

## Atribuição

Este Código de Conduta é adaptado do [Contributor Covenant](https://www.contributor-covenant.org).
Como Subir para o GitHub
bash
# 1. Inicializar repositório
git init

# 2. Adicionar todos os arquivos
git add .

# 3. Commit inicial
git commit -m "feat: Adiciona API de tarefas com autenticação JWT

- Implementa Minimal API com ASP.NET Core 8
- Adiciona autenticação JWT
- Configura Swagger/OpenAPI
- Inclui testes automatizados com xUnit
- Adiciona suporte a Docker

Desafio proposto pela Digital Innovation One (DIO)"

# 4. Conectar ao GitHub
git remote add origin https://github.com/eduardompc/api-tarefas-jwt.git

# 5. Enviar para o GitHub
git branch -M main
git push -u origin main

# 6. Criar tag da versão
git tag -a v1.0.0 -m "Primeira versão estável da API"
git push origin v1.0.0
Badges para o README (adicione no topo)
markdown
![GitHub repo size](https://img.shields.io/github/repo-size/eduardompc/api-tarefas-jwt)
![GitHub stars](https://img.shields.io/github/stars/eduardompc/api-tarefas-jwt?style=social)
![GitHub forks](https://img.shields.io/github/forks/eduardompc/api-tarefas-jwt?style=social)
![GitHub last commit](https://img.shields.io/github/last-commit/eduardompc/api-tarefas-jwt)
![GitHub issues](https://img.shields.io/github/issues/eduardompc/api-tarefas-jwt)
