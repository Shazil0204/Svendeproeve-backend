# These are the commands to make the structure

## Folder structure and references

```
dotnet new sln -n LMSBackend

mkdir src

dotnet new classlib -n LMSBackend.Domain -o src/LMSBackend.Domain
dotnet new classlib -n LMSBackend.Application -o src/LMSBackend.Application
dotnet new classlib -n LMSBackend.Infrastructure -o src/LMSBackend.Infrastructure
dotnet new webapi -n LMSBackend.API -o src/LMSBackend.API

dotnet sln LMSBackend.slnx add src/LMSBackend.Domain
dotnet sln LMSBackend.slnx add src/LMSBackend.Application
dotnet sln LMSBackend.slnx add src/LMSBackend.Infrastructure
dotnet sln LMSBackend.slnx add src/LMSBackend.API

dotnet add src/LMSBackend.Application reference src/LMSBackend.Domain

dotnet add src/LMSBackend.Infrastructure reference src/LMSBackend.Application
dotnet add src/LMSBackend.Infrastructure reference src/LMSBackend.Domain

dotnet add src/LMSBackend.API reference src/LMSBackend.Application
dotnet add src/LMSBackend.API reference src/LMSBackend.Infrastructure
```

## Packages

```
dotnet add src/LMSBackend.API package DotNetEnv
dotnet add src/LMSBackend.API package Microsoft.EntityFrameworkCore.Design
dotnet add src/LMSBackend.API package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add src/LMSBackend.API package Microsoft.AspNetCore.OpenApi --version 10.0.11

dotnet add src/LMSBackend.Infrastructure package MailKit
dotnet add src/LMSBackend.Infrastructure package BCrypt.Net-Next
dotnet add src/LMSBackend.Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/LMSBackend.Infrastructure package System.IdentityModel.Tokens.Jwt
dotnet add src/LMSBackend.Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL

```

## EF DB

```

dotnet ef migrations add InitialCreate `
  --project src/LMSBackend.Infrastructure `
  --startup-project src/LMSBackend.API `
  --output-dir Data/Migrations

dotnet ef database update `
  --project src/LMSBackend.Infrastructure `
  --startup-project src/LMSBackend.API

```

## Run PostgreSQL on Docker

```
docker compose --env-file .env -f Docker/docker-compose.yaml up -d --build database
```

## Runner access to Docker

```
sudo usermod -aG docker gitlab-runner
sudo systemctl restart gitlab-runner
sudo -u gitlab-runner -H docker ps
```

## Different Endpoint Access Control
Let Student & Teacher
```
[Authorize(Roles = $"{nameof(UserRole.Student)},{nameof(UserRole.Teacher)}")]
```

Let Administrator
```
[Authorize(Roles = nameof(UserRole.Administrator))]
```

Let Teacher
```
[Authorize(Roles = nameof(UserRole.Teacher))]
```

Let Anyone
```
[AllowAnonymous]
```

Let Only Authorized one
```
[Authorize]
```