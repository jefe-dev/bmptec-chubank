# BMPTec ChuBank API (.NET 6)

## Rodando em dev
```bash
docker compose up --build
# Swagger: http://localhost:5000/swagger
```

## Migrações (opcional)
```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add Initial --project src/BMPTec.ChuBank.Api
dotnet ef database update --project src/BMPTec.ChuBank.Api
```

## Autenticação
- JWT Bearer (configure `Jwt` em `appsettings.json` ou variáveis de ambiente).
