# NCBCleanArchitectureAPI

โปรเจกต์นี้เป็นตัวอย่าง Clean Architecture สำหรับ .NET 8  
- ใช้ Entity Framework Core, Dapper, MediatR, AutoMapper  
- รองรับหลาย DbContext  
- มีตัวอย่างการใช้งาน Repository และ Swagger

## คำสั่ง EF Core ที่ใช้บ่อย

```sh
dotnet ef migrations add <MigrationName> --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --context FirstDbContext
dotnet ef database update --context FirstDbContext --startup-project API/API.csproj

dotnet ef migrations add UpdateIdentityPostgreSQL  --context ApplicationDbContext --startup-project ../API/ABC.API.csproj
dotnet ef migrations add FirstDbContextPostgreSQL  --context FirstDbContext --startup-project ../API/ABC.API.csproj
dotnet ef migrations add SecondDatabaseDb  --context SecondDbContext --startup-project ../API/ABC.API.csproj

dotnet ef database update --context ApplicationDbContext --startup-project ../API/ABC.API.csproj
dotnet ef database update --context FirstDbContext --startup-project ../API/ABC.API.csproj
dotnet ef database update --context SecondDbContext --startup-project ../API/ABC.API.csproj



## การรันโปรเจกต์

```sh
dotnet run --project API/API.csproj
```

<!-- โครงสร้างโปรเจกต์ (Project Structure) ที่เหมาะสมสำหรับ Clean Architecture -->
## โครงสร้างโปรเจกต์ (Project Structure)

```
NCBCleanArchitectureAPI/
│
├── API/
│   ├── Controllers/
│   │   └── ProductsController.cs
│   └── Program.cs
│
├── Application/
│   ├── Dtos/
│   │   └── ProductDto.cs
│   ├── Mappings/
│   │   └── MappingProfile.cs
│   └── Services/
│       └── ProductService.cs
│
├── Core/
│   ├── Entities/
│   │   └── Product.cs
│   └── Interfaces/
│       ├── IGenericRepository.cs
│       ├── IProductRepository.cs
│       └── IUnitOfWork.cs
│
├── Infrastructure/
│   ├── Data/
│   │   ├── FirstDbContext.cs
│   │   ├── SecondDbContext.cs
│   │   └── UnitOfWork.cs
│   └── Repositories/
│       ├── GenericRepository.cs
│       └── ProductRepository.cs
│
├── appsettings.json
└── README.md
```