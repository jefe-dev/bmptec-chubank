# BMPTEC Chu Bank API

## Rodar com Docker

```bash
# Subir a aplicação
docker-compose up --build

# Rodar em background
docker-compose up --build -d

```

### Acessos
- **API**: http://localhost:5000
- **Swagger**: http://localhost:5000/ (documentação)
- **Health Check**: http://localhost:5000/health
- **PostgreSQL**: localhost:5432

## Rodar os Testes

### Comandos
```bash

dotnet test --verbosity normal
```

## Credenciais de Teste JWT Token
- **Usuário**: admin
- **Senha**: 123456

## Endpoints Principais

- `POST /api/v1/auth/login` - Autenticação
- `POST /api/v1/accounts` - Criar conta
- `GET /api/v1/accounts` - Listar contas
- `POST /api/v1/transfers` - Realizar transferência
- `GET /api/v1/statement` - Extrato de transferências
