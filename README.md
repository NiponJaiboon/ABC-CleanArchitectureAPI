# NCBCleanArchitectureAPI

โปรเจกต์นี้เป็นตัวอย่าง Clean Architecture สำหรับ .NET 8  
- ใช้ Entity Framework Core, Dapper, MediatR, AutoMapper  
- รองรับหลาย DbContext  
- มีตัวอย่างการใช้งาน Repository และ Swagger

## คำสั่ง EF Core ที่ใช้บ่อย

```sh
dotnet ef migrations add <MigrationName> --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --context FirstDbContext
dotnet ef database update --context FirstDbContext --startup-project API/API.csproj
```

## การรันโปรเจกต์

```sh
dotnet run --project API/API.csproj
```