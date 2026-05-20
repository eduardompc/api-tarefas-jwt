# API de Tarefas com Autenticação JWT

API RESTful minimalista para gerenciamento de tarefas com autenticação JWT, documentação Swagger e testes automatizados.

## 📋 Índice

- [Características](#-características)
- [Tecnologias](#-tecnologias)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação](#-instalação)
- [Configuração](#-configuração)
- [Executando a API](#-executando-a-api)
- [Executando os Testes](#-executando-os-testes)
- [Documentação da API](#-documentação-da-api)
- [Endpoints](#-endpoints)
- [Exemplos de Uso](#-exemplos-de-uso)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Melhorias Futuras](#-melhorias-futuras)
- [Solução de Problemas](#-solução-de-problemas)
- [Licença](#-licença)

## ✨ Características

- ✅ Autenticação JWT (JSON Web Tokens)
- ✅ Documentação automática com Swagger UI e ReDoc
- ✅ Testes automatizados com Pytest
- ✅ Validação de dados com Pydantic
- ✅ Isolamento de dados por usuário
- ✅ Criptografia de senhas com bcrypt
- ✅ Código tipado com type hints
- ✅ CORS pronto para produção

## 🛠 Tecnologias

- **FastAPI** - Framework web moderno e rápido
- **JWT** - Autenticação stateless
- **Pytest** - Framework de testes
- **Pydantic** - Validação de dados
- **Uvicorn** - Servidor ASGI

## 📦 Pré-requisitos

- Python 3.8 ou superior
- pip (gerenciador de pacotes Python)
- Git (opcional)

## 🔧 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/seu-usuario/api-tarefas-jwt.git
cd api-tarefas-jwt

⚙ Configuração
Crie um arquivo .env na raiz do projeto:

env
SECRET_KEY=sua-chave-secreta-super-segura-aqui
# Gere uma chave forte com: openssl rand -hex 32
⚠️ Importante: Em produção, use uma chave forte e mantenha em segredo!

🚀 Executando a API
Modo desenvolvimento (com reload automático)
bash
python main.py
Modo produção
bash
uvicorn main:app --host 0.0.0.0 --port 8000 --workers 4
A API estará disponível em: http://localhost:8000

🧪 Executando os Testes
Executar todos os testes
bash
pytest tests/ -v
Executar com relatório detalhado
bash
pytest tests/ -v --tb=short
Executar testes específicos
bash
# Apenas autenticação
pytest tests/test_auth.py -v

# Apenas tarefas
pytest tests/test_tarefas.py -v
Com cobertura de código
bash
# Instalar pytest-cov
pip install pytest-cov

# Executar com cobertura
pytest tests/ --cov=. --cov-report=html

# Abrir relatório (Linux/Mac)
open htmlcov/index.html

# Windows
start htmlcov/index.html
📚 Documentação da API
Após iniciar a API, acesse:

Swagger UI (interativo): http://localhost:8000/docs

ReDoc (alternativo): http://localhost:8000/redoc

🔗 Endpoints
Autenticação Pública
Método	Endpoint	Descrição	Autenticação
POST	/registrar	Registrar novo usuário	❌
POST	/login	Login e obtenção de token	❌
GET	/health	Health check da API	❌
Tarefas (Protegidos)
Método	Endpoint	Descrição	Autenticação
POST	/tarefas	Criar nova tarefa	✅
GET	/tarefas	Listar minhas tarefas	✅
GET	/tarefas/{id}	Buscar tarefa por ID	✅
PUT	/tarefas/{id}	Atualizar tarefa	✅
DELETE	/tarefas/{id}	Deletar tarefa	✅
💻 Exemplos de Uso
1. Registrar um usuário
bash
curl -X POST http://localhost:8000/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "username": "joao.silva",
    "password": "minhasenha123"
  }'
Resposta:

json
{
  "mensagem": "Usuário criado com sucesso"
}
2. Fazer login
bash
curl -X POST http://localhost:8000/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "joao.silva",
    "password": "minhasenha123"
  }'
Resposta:

json
{
  "access_token": "eyJhbGciOiJIUzI1NiIs...",
  "token_type": "bearer",
  "expires_in": 1800
}
3. Criar uma tarefa
bash
curl -X POST http://localhost:8000/tarefas \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Estudar FastAPI",
    "descricao": "Aprender sobre autenticação JWT"
  }'
Resposta:

json
{
  "id": 1,
  "titulo": "Estudar FastAPI",
  "descricao": "Aprender sobre autenticação JWT",
  "concluida": false,
  "usuario": "joao.silva"
}
4. Listar tarefas
bash
curl -X GET http://localhost:8000/tarefas \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
5. Filtrar tarefas por status
bash
# Apenas tarefas concluídas
curl -X GET "http://localhost:8000/tarefas?concluida=true" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"

# Apenas tarefas pendentes
curl -X GET "http://localhost:8000/tarefas?concluida=false" \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
6. Atualizar uma tarefa
bash
curl -X PUT http://localhost:8000/tarefas/1 \
  -H "Authorization: Bearer SEU_TOKEN_AQUI" \
  -H "Content-Type: application/json" \
  -d '{
    "titulo": "Estudar FastAPI Avançado",
    "concluida": true
  }'
7. Deletar uma tarefa
bash
curl -X DELETE http://localhost:8000/tarefas/1 \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
📁 Estrutura do Projeto
text
api-tarefas-jwt/
│
├── main.py                 # Aplicação principal
├── requirements.txt        # Dependências do projeto
├── .env                   # Variáveis de ambiente
├── .gitignore             # Arquivos ignorados pelo Git
│
├── tests/                 # Testes automatizados
│   ├── __init__.py
│   ├── test_auth.py      # Testes de autenticação
│   └── test_tarefas.py    # Testes de tarefas
│
└── README.md              # Documentação
🚀 Melhorias Futuras
Imediatas (Curto Prazo)
Adicionar refresh token

Implementar paginação na listagem de tarefas

Adicionar logging estruturado

Criar middleware para logging de requisições

Adicionar validação de força de senha

Intermediárias (Médio Prazo)
Substituir banco em memória por PostgreSQL ou SQLite

Adicionar ORM (SQLAlchemy ou Tortoise-ORM)

Implementar rate limiting

Adicionar cache com Redis

Criar sistema de roles (admin, user)

Adicionar versionamento da API (/v1, /v2)

Avançadas (Longo Prazo)
Containerização com Docker e docker-compose

CI/CD com GitHub Actions

Monitoramento com Prometheus + Grafana

Implementar websockets para notificações

Criar cliente frontend (React/Vue.js)

Documentação em múltiplos idiomas

Implementar filas com Celery + RabbitMQ

🔒 Segurança
Implementado
✅ Hash de senhas com bcrypt

✅ Tokens JWT com expiração

✅ Isolamento de dados por usuário

✅ Validação de inputs

Recomendações para produção
⚠️ Use HTTPS sempre

⚠️ Mantenha SECRET_KEY segura (use AWS Secrets Manager, HashiCorp Vault)

⚠️ Implemente rate limiting

⚠️ Adicione headers de segurança (CORS, CSP, etc.)

⚠️ Faça auditoria regular de dependências

🐛 Solução de Problemas
Erro: "Module not found"
bash
pip install -r requirements.txt
Erro: "Porta já está em uso"
bash
# Mudar a porta
uvicorn main:app --port 8001

# Ou matar o processo (Linux/Mac)
lsof -i :8000
kill -9 [PID]
Erro: "Invalid token"
Verifique se o token não expirou (30 minutos)

Confirme se está usando "Bearer " antes do token

Refaça o login para obter um novo token

Erro nos testes: "ImportError"
bash
# Execute os testes a partir da raiz do projeto
cd api-tarefas-jwt
python -m pytest tests/ -v
📊 Métricas e Monitoramento
Adicione em main.py para monitoramento básico:

python
from fastapi import Request
import time

@app.middleware("http")
async def add_process_time_header(request: Request, call_next):
    start_time = time.time()
    response = await call_next(request)
    process_time = time.time() - start_time
    response.headers["X-Process-Time"] = str(process_time)
    return response
🤝 Contribuindo
Faça um Fork do projeto

Crie uma branch para sua feature (git checkout -b feature/nova-feature)

Commit suas mudanças (git commit -m 'Adiciona nova feature')

Push para a branch (git push origin feature/nova-feature)

Abra um Pull Request

📄 Licença
Este projeto está sob a licença MIT. Veja o arquivo LICENSE para mais detalhes.

📞 Suporte
📧 Email: eduardomarciopc@gmail.com

🐛 Issues: GitHub Issues do projeto

💬 Discussões: GitHub Discussions

⭐ Agradecimentos
FastAPI team pela excelente documentação

Comunidade open source por todas as ferramentas