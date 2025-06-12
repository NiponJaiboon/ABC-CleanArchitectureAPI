# NCBCleanArchitectureAPI

โปรเจกต์นี้เป็นตัวอย่าง Clean Architecture สำหรับ .NET 8

- ใช้ Entity Framework Core, Dapper, MediatR, AutoMapper
- รองรับหลาย DbContext
- มีตัวอย่างการใช้งาน Repository และ Swagger

## คำสั่ง EF Core ที่ใช้บ่อย

```bash
dotnet ef migrations add InitApplication --context ApplicationDbContext -o Data/Migrations/Application/ApplicationDb --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef migrations add InitSecondContext --context FirstDbContext -o Data/Migrations/Application/FirstDb --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef migrations add InitSecondDatabaseDb --context SecondDbContext -o Data/Migrations/Application/SecondDb --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef migrations add IdentityServerInitC -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb --project ../Infrastructure/Infrastructure.csproj --startup-project ./ABC.API.csproj
dotnet ef migrations add PersistedGrantInitsC -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb --project ../Infrastructure/Infrastructure.csproj --startup-project ./ABC.API.csproj

dotnet ef database update --context ApplicationDbContext --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef database update --context FirstDbContext --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef database update --context SecondDbContext --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef database update -c ConfigurationDbContext --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj
dotnet ef database update -c PersistedGrantDbContext --project ../Infrastructure/Infrastructure.csproj --startup-project ../API/ABC.API.csproj

```

sudo dotnet ef database update --context ApplicationDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj
sudo dotnet ef database update --context FirstDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj
sudo dotnet ef database update --context SecondDbContext --startup-project /Users/nevelopdevper/iDev/ABC-CleanArchitecture/API/ABC.API.csproj

## การรันโปรเจกต์

dotnet run --project API/API.csproj

<!-- โครงสร้างโปรเจกต์ (Project Structure) ที่เหมาะสมสำหรับ Clean Architecture -->

## โครงสร้างโปรเจกต์ (Project Structure)

NCBCleanArchitectureAPI/
│
├── API/
│ ├── Controllers/
│ │ └── ProductsController.cs
│ └── Program.cs
│
├── Application/
│ ├── Dtos/
│ │ └── ProductDto.cs
│ ├── Mappings/
│ │ └── MappingProfile.cs
│ └── Services/
│ └── ProductService.cs
│
├── Core/
│ ├── Entities/
│ │ └── Product.cs
│ └── Interfaces/
│ ├── IGenericRepository.cs
│ ├── IProductRepository.cs
│ └── IUnitOfWork.cs
│
├── Infrastructure/
│ ├── Data/
│ │ ├── FirstDbContext.cs
│ │ ├── SecondDbContext.cs
│ │ └── UnitOfWork.cs
│ └── Repositories/
│ ├── GenericRepository.cs
│ └── ProductRepository.cs
│
├── appsettings.json
└── README.md

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

VERIFY SIGNATURE ของ JWT token ใน https://jwt.io/
https://localhost:5001/.well-known/openid-configuration/jwks
Capy All
แปลง JWK เป็น PEM (RSA Public Key) สำหรับ jwt.io
ไปที่ https://8gwifi.org/jwkconvertfunctions.jsp
วาง JWK (ทั้งก้อน) ลงในช่อง JWK
กด "Convert JWK to PEM"
จะได้ Public Key (PEM)
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAr7ugouwPAFRNPe8jYUAq
1PXndfgRwBR5cZFFKLW-3RSeM21aP4Q1IswAjiaOCefqY5fYP9G8uu5fiFbF-lPL
qEuRMRHwTgk340u95uw4dOutfDB98HCs6lDql-a5o7AcvvIwQ5c5g5S0pRxXVgUi
roKc43UOo2N0wkXp-8BxUU_Z7K7mkBzA2vdnA10hzrScqF12Q2LO1m-QDWd6RmEh
sKWOljAAXb32iTRi2Bg1tU8X0rfzDQsYnAXkzQj6m0S-6YL5IUtAUP_jichgHHdv
G6GKP-5slWhJWBkWEgFiefs4iXsIBUHD-XXT2O13G1TeNpyAQiYbBDclBYXJbzIy
QIDAQAB
-----END PUBLIC KEY-----
นำ Public Key (PEM) ไปวางในช่อง "Public Key or Certificate" ของ jwt.io
jwt.io จะทำการ verify signature ให้ทันที

แนวทางที่แนะนำ (Best Practice)

1.  Backend (ASP.NET Core)
    Login:
    -ส่ง access token (และ refresh token ถ้ามี) กลับใน response body เท่านั้น
    -ไม่ต้อง set cookie ใดๆ

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
    var tokenResponse = await \_authService.LoginAsync(loginDto);
    if (tokenResponse.IsError)
    return Unauthorized(new { message = "Authentication failed" });

        return Ok(new {
            access_token = tokenResponse.AccessToken,
            expires_in = tokenResponse.ExpiresIn,
            token_type = tokenResponse.TokenType,
            scope = tokenResponse.Scope
        });

    }

API ที่ต้องการ auth:
-ใช้ [Authorize] ตามปกติ
-.NET จะอ่าน JWT จาก Authorization header อัตโนมัติ

2. Frontend (React/Next.js/SPA)
   หลัง login:
   -รับ access token จาก response body
   -เก็บไว้ใน memory (ปลอดภัยสุด) หรือ localStorage (ถ้าเน้น UX)
   ทุก request ที่ต้องการ auth:
   ส่ง header:
   headers: { Authorization: `Bearer ${token}` }
   ตัวอย่างการเรียก API:const token = localStorage.getItem("token");
   const res = await fetch(`${API_URL}/api/Profile`, {
   method: 'GET',
   headers: {
   'Content-Type': 'application/json',
   'Authorization': `Bearer ${token}`
   }
   });

3. (Optional) Refresh Token
   ถ้าต้องการ UX ดีขึ้น (refresh หน้าไม่ต้อง login ใหม่)
   ใช้ refresh token (เก็บใน HttpOnly cookie หรือ localStorage)
   เขียน logic ให้ frontend ขอ access token ใหม่เมื่อหมดอายุ

4. ข้อดีของแนวทางนี้
   ปลอดภัย (ถ้าไม่เก็บ token ใน localStorage หรือป้องกัน XSS ดี)
   เป็นมาตรฐาน REST API
   รองรับทุก client (web, mobile, 3rd party)
   ไม่ต้อง custom middleware ฝั่ง backend

สรุปแนวทาง
   Backend: ส่ง JWT ใน response body, ไม่ set cookie
   Frontend: เก็บ token เอง, ส่งใน Authorization header ทุก request
   ไม่ต้องใช้ cookie สำหรับ access token