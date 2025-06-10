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

sudo dotnet ef database update --context ApplicationDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj
sudo dotnet ef database update --context FirstDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj
sudo dotnet ef database update --context SecondDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj



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

ระบบจัดการ User (User Management) สำหรับแอปพลิเคชันสมัยใหม่ ควรมีฟีเจอร์หลักดังนี้:

1. Authentication (การยืนยันตัวตน)
ลงทะเบียน (Register/Sign up)
ล็อกอิน (Login)
ล็อกเอาต์ (Logout)
ล็อกอินด้วย Social/External Provider (Google, Facebook, ฯลฯ)
Refresh Token
2. Authorization (การกำหนดสิทธิ์)
กำหนดบทบาท (Roles) เช่น Admin, User
กำหนดสิทธิ์ (Permissions) ตามบทบาท
3. Password Management
เปลี่ยนรหัสผ่าน (Change Password)
ลืมรหัสผ่าน/รีเซ็ตรหัสผ่าน (Forgot/Reset Password)
บังคับเปลี่ยนรหัสผ่านเมื่อครบกำหนด
4. User Profile
ดูข้อมูลโปรไฟล์ (Get Profile)
แก้ไขข้อมูลโปรไฟล์ (Edit Profile)
อัปโหลดรูปโปรไฟล์
5. User Administration (สำหรับแอดมิน)
ดูรายชื่อผู้ใช้ทั้งหมด
แก้ไข/ลบ/ระงับบัญชีผู้ใช้
กำหนด/เปลี่ยนบทบาทผู้ใช้
ค้นหา/กรองผู้ใช้
6. Security
2FA (Two-Factor Authentication)
ตรวจสอบประวัติการเข้าสู่ระบบ (Login History)
แจ้งเตือนกิจกรรมที่ผิดปกติ
7. อื่น ๆ
Email Verification (ยืนยันอีเมล)
Account Lockout (ล็อกบัญชีเมื่อพยายามเข้าสู่ระบบผิดหลายครั้ง)
การจัดการ session/token
หมายเหตุ:
ฟีเจอร์ที่ควรมีขึ้นกับความต้องการและความปลอดภัยของแต่ละระบบ
สำหรับระบบองค์กรหรือ production ควรมีฟีเจอร์ด้านความปลอดภัยครบถ้วน