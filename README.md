# AuthAPI

Secure your backend and manage user context.

```bash
cd .\AuthAPI.Api
dotnet build
dotnet run
```

## To run migrations:

```bash
dotnet ef migrations add <Migration Name> -p .\AuthAPI.Infrastructure -s .\AuthAPI.Api
dotnet ef database update -p .\AuthAPI.Infrastructure -s .\AuthAPI.Api
```