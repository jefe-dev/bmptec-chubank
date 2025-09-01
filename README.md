# BMPTEC ChuBank API

API bancária desenvolvida em .NET 6 com autenticação JWT, transferências entre contas e consulta de extratos.

## 1. Clonar o Repositório

```bash
git clone https://github.com/jefe-dev/bmptec-chubank.git
cd bmptec-chubank
```

## 2. Como Rodar

### Pré-requisitos

- Docker e Docker Compose
- .NET 6 SDK (para rodar testes)

### Docker

```bash
# Rodar em background
docker-compose up --build -d
```

### Testes

```bash
dotnet test --verbosity normal
```

## 3. Credenciais JWT

- **Usuário**: `admin`
- **Senha**: `admin123`

## 4. Acessos

- **Swagger**: http://localhost:5000/swagger
- **API**: http://localhost:5000
- **Health Check**: http://localhost:5000/health
- **PostgreSQL**: localhost:5432

## 5. Exemplo de Uso

```bash
# 1. Fazer login para obter token
curl -X POST http://localhost:5000/api/v1.0/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username": "admin", "password": "admin123"}'

# 2. Usar o token nas requisições
curl -X GET http://localhost:5000/api/v1.0/accounts \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

## Endpoints Principais

- `POST /api/v1.0/auth/login` - Autenticação
- `POST /api/v1.0/accounts` - Criar conta
- `GET /api/v1.0/accounts` - Listar contas
- `POST /api/v1.0/transfers` - Realizar transferência
- `GET /api/v1.0/statement` - Extrato de transferências
