# 👔 Formale - AI-Powered Fashion E-commerce Platform

<div align="center">

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=Swagger&logoColor=black)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=JSON%20web%20tokens&logoColor=white)

**Formale** là nền tảng thương mại điện tử thời trang kết hợp trí tuệ nhân tạo (AI) để gợi ý trang phục phù hợp với phong cách người dùng.

[Tính năng](#-tính-năng-chính) •
[Cài đặt](#-cài-đặt) •
[API Endpoints](#-api-endpoints) •
[Kiến trúc](#-kiến-trúc-hệ-thống) •
[Đóng góp](#-đóng-góp)

</div>

---

## 📋 Mục lục

- [Tính năng chính](#-tính-năng-chính)
- [Công nghệ sử dụng](#-công-nghệ-sử-dụng)
- [Kiến trúc hệ thống](#-kiến-trúc-hệ-thống)
- [Cấu trúc dự án](#-cấu-trúc-dự-án)
- [Cơ sở dữ liệu](#-cơ-sở-dữ-liệu)
- [Cài đặt](#-cài-đặt)
- [Cấu hình](#-cấu-hình)
- [API Endpoints](#-api-endpoints)
- [Xác thực & Phân quyền](#-xác-thực--phân-quyền)
- [Tích hợp AI](#-tích-hợp-ai)
- [Thanh toán](#-thanh-toán)
- [Đóng góp](#-đóng-góp)

---

## ✨ Tính năng chính

### 🛍️ **Thương mại điện tử**
- Quản lý sản phẩm thời trang (quần áo, giày dép, phụ kiện)
- Giỏ hàng và đặt hàng trực tuyến
- Thanh toán tích hợp PayOS
- Đánh giá và phản hồi sản phẩm

### 🤖 **Gợi ý trang phục bằng AI**
- Phân tích phong cách từ mô tả ngôn ngữ tự nhiên
- Tự động gợi ý bộ trang phục phù hợp (Tops + Bottoms + Footwear + Accessories)
- Kết hợp sản phẩm từ tủ đồ cá nhân và hệ thống
- Lưu và quản lý các bộ combo trang phục

### 👤 **Quản lý người dùng**
- Đăng ký với xác thực OTP qua email
- Đăng nhập bằng tài khoản hoặc Google OAuth 2.0
- Hệ thống phân quyền (Admin, User, Manager)
- Gói Premium với các tính năng nâng cao

### 📦 **Tủ đồ cá nhân (Virtual Closet)**
- Lưu trữ sản phẩm yêu thích
- Quản lý các bộ outfit đã tạo
- Tìm kiếm và lọc sản phẩm trong tủ đồ

---

## 🛠 Công nghệ sử dụng

### Backend Framework
| Công nghệ | Phiên bản | Mô tả |
|-----------|-----------|-------|
| **.NET** | 8.0 | Framework chính |
| **Entity Framework Core** | 9.0 | ORM cho cơ sở dữ liệu |
| **SQL Server** | - | Cơ sở dữ liệu quan hệ |

### Authentication & Security
| Công nghệ | Mô tả |
|-----------|-------|
| **JWT Bearer** | Token-based authentication |
| **Google OAuth 2.0** | Social login |
| **OTP via Email** | Xác thực tài khoản |

### Tích hợp bên thứ ba
| Dịch vụ | Mô tả |
|---------|-------|
| **PayOS** | Cổng thanh toán Việt Nam |
| **Cloudinary** | Lưu trữ và quản lý hình ảnh |
| **OpenRouter AI** | Gợi ý trang phục bằng AI (DeepSeek model) |
| **SMTP (MimeKit)** | Gửi email xác thực |

### Thư viện hỗ trợ
| Thư viện | Phiên bản | Mô tả |
|----------|-----------|-------|
| **AutoMapper** | 14.0 | Object mapping |
| **FluentValidation** | 12.0 | Data validation |
| **Swashbuckle** | 6.6.2 | Swagger/OpenAPI |
| **RestSharp** | 112.1 | HTTP client |
| **Google.Apis.Auth** | 1.69 | Google authentication |

---

## 🏗 Kiến trúc hệ thống

Dự án được xây dựng theo **Clean Architecture** với 4 layers:

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                     │
│                          (API)                              │
│   Controllers │ AutoMapper │ Swagger │ Middleware          │
├─────────────────────────────────────────────────────────────┤
│                      APPLICATION LAYER                      │
│                      (Application)                          │
│   Services │ DTOs │ Interfaces │ Validation │ Settings     │
├─────────────────────────────────────────────────────────────┤
│                      INFRASTRUCTURE LAYER                   │
│                      (Infrastructure)                       │
│   DbContext │ Repositories │ Migrations │ External APIs    │
├─────────────────────────────────────────────────────────────┤
│                        DOMAIN LAYER                         │
│                         (Domain)                            │
│   Entities │ Enums │ Base Classes │ Business Rules         │
└─────────────────────────────────────────────────────────────┘
```

### Luồng dữ liệu

```
Client Request → Controller → Service → Repository → Database
                    ↓            ↓
               Validation    Business Logic
                    ↓            ↓
              DTO Mapping   Entity Mapping
```

---

## 📁 Cấu trúc dự án

```
Formale_EXE201/
├── 📂 API/                          # Presentation Layer
│   ├── Controllers/                 # REST API Controllers
│   │   ├── AuthController.cs        # Xác thực
│   │   ├── ProductController.cs     # Sản phẩm
│   │   ├── CartController.cs        # Giỏ hàng
│   │   ├── OrderController.cs       # Đơn hàng
│   │   ├── PaymentController.cs     # Thanh toán
│   │   ├── OutfitController.cs      # Gợi ý trang phục AI
│   │   ├── UserClosetController.cs  # Tủ đồ cá nhân
│   │   ├── FeedbackController.cs    # Đánh giá
│   │   └── ...
│   ├── Mapper/
│   │   └── AutoMapperProfile.cs     # Cấu hình AutoMapper
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Program.cs                   # Application entry point
│   └── API.csproj
│
├── 📂 Application/                   # Application Layer
│   ├── DTO/                         # Data Transfer Objects
│   │   ├── LoginDTO.cs
│   │   ├── RegisterDTO.cs
│   │   ├── ProductRequestDto.cs
│   │   ├── ProductResponseDto.cs
│   │   ├── OutfitSuggestionDto.cs
│   │   ├── PaymentDTO.cs
│   │   └── ...
│   ├── Interface/                   # Service Interfaces
│   │   ├── IProductService.cs
│   │   ├── IOutfitService.cs
│   │   ├── IPaymentService.cs
│   │   └── ...
│   ├── Service/                     # Service Implementations
│   │   ├── ProductService.cs
│   │   ├── OutfitService.cs
│   │   ├── OpenRouterService.cs     # AI Integration
│   │   ├── PaymentService.cs
│   │   ├── PayOsService.cs
│   │   ├── CloudinaryService.cs
│   │   ├── EmailService.cs
│   │   └── ...
│   ├── Settings/                    # Configuration classes
│   │   ├── JwtSetting.cs
│   │   ├── EmailSettings.cs
│   │   ├── PayOsSetting.cs
│   │   └── GoogleSetting.cs
│   ├── Validation/                  # FluentValidation rules
│   └── Application.csproj
│
├── 📂 Domain/                        # Domain Layer
│   ├── Entities/                    # Domain Entities
│   │   ├── UserAccount.cs
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── Payment.cs
│   │   ├── OutfitCombo.cs
│   │   ├── UserCloset.cs
│   │   └── ...
│   ├── Enum/                        # Enumerations
│   │   ├── Status.cs
│   │   ├── PaymentMethod.cs
│   │   └── PremiumPackageTier.cs
│   ├── Base/                        # Base classes
│   └── Domain.csproj
│
├── 📂 Infrastructure/                # Infrastructure Layer
│   ├── Repository/                  # Data Access
│   │   ├── UserRepository.cs
│   │   ├── ProductRepository.cs
│   │   ├── OrderRepository.cs
│   │   └── ...
│   ├── Base/                        # Generic Repository
│   ├── migrations/                  # EF Core Migrations
│   ├── AppDBContext.cs              # Database Context
│   └── Infrastructure.csproj
│
├── Formale_EXE201.sln               # Solution file
└── README.md
```

---

## 🗄 Cơ sở dữ liệu

### Entity Relationship Diagram

```
┌──────────────────┐       ┌──────────────────┐       ┌──────────────────┐
│   UserAccount    │       │     Product      │       │      Order       │
├──────────────────┤       ├──────────────────┤       ├──────────────────┤
│ UserId (PK)      │       │ ProductId (PK)   │       │ OrderId (PK)     │
│ FullName         │       │ Name             │       │ UserId (FK)      │
│ Email            │       │ Price            │       │ TotalPrice       │
│ Password         │       │ BrandId (FK)     │       │ Status           │
│ RoleId (FK)      │       │ ColorId (FK)     │       │ CreatedAt        │
│ PremiumPackageId │       │ StyleId (FK)     │       │ UpdatedAt        │
│ IsActive         │       │ CategoryId (FK)  │       └────────┬─────────┘
│ Status           │       │ MaterialId (FK)  │                │
└────────┬─────────┘       │ TypeId (FK)      │                │
         │                 │ ImageURL         │       ┌────────▼─────────┐
         │                 │ IsSystemCreated  │       │    OrderItem     │
         │                 └────────┬─────────┘       ├──────────────────┤
         │                          │                 │ OrderItemId (PK) │
         │                          │                 │ OrderId (FK)     │
┌────────▼─────────┐       ┌────────▼─────────┐       │ ProductId (FK)   │
│    UserCloset    │       │     Feedback     │       │ Quantity         │
├──────────────────┤       ├──────────────────┤       │ Price            │
│ ClosetId (PK)    │       │ FeedbackId (PK)  │       └──────────────────┘
│ UserId (FK)      │       │ UserId (FK)      │
│ ProductId (FK)   │       │ ProductId (FK)   │
│ ComboId (FK)     │       │ Rating           │
└──────────────────┘       │ Comment          │
                           └──────────────────┘

┌──────────────────┐       ┌──────────────────┐       ┌──────────────────┐
│   OutfitCombo    │       │ OutfitComboItem  │       │     Payment      │
├──────────────────┤       ├──────────────────┤       ├──────────────────┤
│ ComboId (PK)     │───┐   │ ItemId (PK)      │       │ Id (PK)          │
│ Name             │   │   │ ComboId (FK)     │───────│ UserId (FK)      │
│ Description      │   └──▶│ ProductId (FK)   │       │ OrderId (FK)     │
│ UserId (FK)      │       │ Slot             │       │ OrderCode        │
└──────────────────┘       └──────────────────┘       │ Amount           │
                                                      │ Status           │
┌──────────────────┐       ┌──────────────────┐       │ PaymentUrl       │
│  PremiumPackage  │       │      Roles       │       └──────────────────┘
├──────────────────┤       ├──────────────────┤
│ PackageId (PK)   │       │ RoleId (PK)      │
│ Name             │       │ RoleName         │
│ Price            │       │ • Admin (1)      │
│ DurationInDays   │       │ • User (2)       │
│ Tier             │       │ • Manager (3)    │
└──────────────────┘       └──────────────────┘
```

### Các Entity chính

#### 👤 **UserAccount**
```csharp
- UserId, FullName, Email, UserName, Password
- PhoneNumber, Address, DateOfBirth
- RoleId, PremiumPackageId, PremiumExpiryDate
- Token, OTP, OtpExpiry
- IsActive, Status, LoginProvider
- Image_User, Background_Image, Description
```

#### 👕 **Product**
```csharp
- ProductId, Name, Price, Description, ImageURL
- BrandId, ColorId, StyleId, CategoryId, MaterialId, TypeId
- IsSystemCreated, UserId
- TotalFeedbacks, AverageRating
- IsDeleted, CreatedAt, UpdatedAt (từ BaseEntity)
```

#### 🛒 **Order & OrderItem**
```csharp
Order:
- OrderId, UserId, TotalPrice, Status
- PaidAt, CreatedAt, UpdatedAt

OrderItem:
- OrderItemId, OrderId, ProductId
- Quantity, Price
```

#### 💳 **Payment**
```csharp
- Id, UserId, OrderId, PremiumPackageId
- OrderCode, Amount, Description
- BuyerName, BuyerEmail, BuyerPhone, BuyerAddress
- PaymentUrl, CheckoutUrl, TransactionId
- Status, Method, Signature
- CancelUrl, ReturnUrl, CancelReason
```

#### 👔 **OutfitCombo & OutfitComboItem**
```csharp
OutfitCombo:
- ComboId, Name, Description, UserId

OutfitComboItem:
- ItemId, ComboId, ProductId, Slot
```

### Enums

```csharp
// Status (Order/Payment)
enum Status { PENDING, COMPLETE, FAILED, CANCELLED }

// PaymentMethod
enum PaymentMethod { VNPAY, MOMO, PayOs }

// PremiumPackageTier
enum PremiumPackageTier { Premium, Gold }
```

---

## 🚀 Cài đặt

### Yêu cầu hệ thống
- **.NET SDK** 8.0 trở lên
- **SQL Server** 2019 trở lên
- **Visual Studio 2022** hoặc **VS Code**
- **Git**

### Bước 1: Clone repository
```bash
git clone https://github.com/your-username/Formale_EXE201.git
cd Formale_EXE201
```

### Bước 2: Restore packages
```bash
dotnet restore
```

### Bước 3: Cấu hình appsettings.json
Tạo file `appsettings.json` trong thư mục `API/` (xem phần [Cấu hình](#-cấu-hình))

### Bước 4: Áp dụng migrations
```bash
cd API
dotnet ef database update --project ../Infrastructure
```

### Bước 5: Chạy ứng dụng
```bash
dotnet run --project API
```

### Bước 6: Truy cập Swagger
Mở trình duyệt và truy cập:
```
https://localhost:5001/swagger
```

---

## ⚙ Cấu hình

Tạo file `appsettings.json` trong thư mục `API/`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=FormaleDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  
  "JwtSettings": {
    "Issuer": "FormaleAPI",
    "Audience": "FormaleClient",
    "SecretKey": "YOUR_SUPER_SECRET_KEY_AT_LEAST_32_CHARACTERS_LONG",
    "ExpiryInMinutes": 60
  },
  
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUser": "your-email@gmail.com",
    "SmtpPass": "your-app-password"
  },
  
  "PayOsConfig": {
    "BaseUrl": "https://api-merchant.payos.vn",
    "ClientId": "YOUR_PAYOS_CLIENT_ID",
    "ApiKey": "YOUR_PAYOS_API_KEY",
    "ChecksumKey": "YOUR_PAYOS_CHECKSUM_KEY"
  },
  
  "GoogleSetting": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID.apps.googleusercontent.com",
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
  },
  
  "OpenRouter": {
    "ApiKey": "YOUR_OPENROUTER_API_KEY"
  },
  
  "Cloudinary": {
    "CloudName": "YOUR_CLOUDINARY_CLOUD_NAME",
    "ApiKey": "YOUR_CLOUDINARY_API_KEY",
    "ApiSecret": "YOUR_CLOUDINARY_API_SECRET"
  }
}
```

### Hướng dẫn lấy credentials

| Dịch vụ | Hướng dẫn |
|---------|-----------|
| **PayOS** | Đăng ký tại [payos.vn](https://payos.vn) |
| **Google OAuth** | Tạo project tại [Google Cloud Console](https://console.cloud.google.com) |
| **OpenRouter** | Đăng ký tại [openrouter.ai](https://openrouter.ai) |
| **Cloudinary** | Đăng ký tại [cloudinary.com](https://cloudinary.com) |
| **Gmail SMTP** | Tạo App Password trong Google Account Settings |

---

## 📡 API Endpoints

### 🔐 Authentication (`/api/Auth`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/register` | Đăng ký tài khoản mới | ❌ |
| `POST` | `/register-manager` | Tạo tài khoản Manager | Admin |
| `POST` | `/login` | Đăng nhập | ❌ |
| `POST` | `/change-password` | Đổi mật khẩu (cần OTP) | ✅ |
| `POST` | `/logout` | Đăng xuất | ✅ |
| `POST` | `/active-account` | Kích hoạt tài khoản bằng OTP | ❌ |
| `POST` | `/resend-otp` | Gửi lại mã OTP | ❌ |
| `GET` | `/signin-google` | Đăng nhập bằng Google | ❌ |

### 👥 Users (`/api/User`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/` | Lấy danh sách users | Admin |
| `GET` | `/get-user-info` | Lấy thông tin user hiện tại | ✅ |
| `PUT` | `/update-profile` | Cập nhật profile | User |
| `DELETE` | `/{userId}` | Xóa user | Admin |
| `POST` | `/ban/{userId}` | Ban user | Admin |
| `POST` | `/unban/{userId}` | Unban user | Admin |

### 👕 Products (`/api/Product`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/` | Lấy tất cả sản phẩm | ❌ |
| `GET` | `/{id}` | Lấy sản phẩm theo ID | ❌ |
| `POST` | `/` | Tạo sản phẩm mới | ✅ |
| `PUT` | `/{id}` | Cập nhật sản phẩm | ✅ |
| `PUT` | `/update-image/{id}` | Cập nhật ảnh sản phẩm | ✅ |
| `DELETE` | `/{id}` | Xóa sản phẩm (soft delete) | Admin |
| `GET` | `/search` | Tìm kiếm với filter & phân trang | ❌ |
| `POST` | `/suggest` | Gợi ý trang phục AI | Premium |

### 🛒 Cart (`/api/Cart`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/` | Lấy giỏ hàng hiện tại | ✅ |
| `POST` | `/add-item/{productId}` | Thêm sản phẩm vào giỏ | ✅ |
| `POST` | `/reduce-item/{productId}` | Giảm số lượng sản phẩm | ✅ |
| `GET` | `/preview` | Xem trước đơn hàng | ✅ |
| `DELETE` | `/remove-item/{productId}` | Xóa sản phẩm khỏi giỏ | ✅ |

### 📦 Orders (`/api/Order`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/create-from-cart` | Tạo đơn hàng từ giỏ hàng | ✅ |
| `GET` | `/all` | Lấy tất cả đơn hàng | Admin |
| `GET` | `/{orderId}` | Lấy đơn hàng theo ID | Admin |

### 💳 Payments (`/api/Payment`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/getpayment` | Lấy tất cả payments | Admin |
| `GET` | `/getpayment-byuser` | Lấy payments của user | ✅ |
| `POST` | `/create` | Tạo payment mới | ✅ |
| `POST` | `/search-transaction` | Tìm theo transaction ID | ✅ |
| `POST` | `/cancel` | Hủy payment | ✅ |
| `PUT` | `/update-status/{paymentId}` | Cập nhật trạng thái | Admin |
| `GET` | `/check-status/{orderCode}` | Kiểm tra trạng thái PayOS | ✅ |
| `GET` | `/premium-payments` | Payments gói Premium | Admin |
| `POST` | `/confirm-premium-payment` | Xác nhận thanh toán Premium | Admin |

### 👔 Outfit - AI Suggestion (`/api/Outfit`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/suggest` | Gợi ý outfit từ closet | Premium |
| `POST` | `/save` | Lưu combo gợi ý | ✅ |
| `GET` | `/{comboId}` | Lấy chi tiết combo | ✅ |
| `PUT` | `/{comboId}/replace-item` | Thay thế item trong combo | ✅ |
| `GET` | `/user-combos` | Lấy danh sách combos của user | ✅ |
| `PUT` | `/update-combo-info` | Cập nhật tên/mô tả combo | ✅ |

### 🗄️ User Closet (`/api/UserCloset`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/` | Lấy tất cả closets | Admin |
| `GET` | `/{closetId}` | Lấy closet theo ID | User |
| `DELETE` | `/{closetId}` | Xóa item khỏi closet | User |
| `GET` | `/my-closet` | Lấy closet của user | ✅ |
| `GET` | `/my-items` | Lấy sản phẩm trong closet | ✅ |
| `GET` | `/my-combos` | Lấy outfit combos | ✅ |
| `GET` | `/search` | Tìm kiếm trong closet | ✅ |

### ⭐ Feedback (`/api/Feedback`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `POST` | `/create` | Tạo đánh giá mới | ✅ |
| `PUT` | `/update` | Cập nhật đánh giá | ✅ |
| `GET` | `/rating/{productId}` | Lấy thống kê rating | ❌ |
| `GET` | `/` | Lấy feedbacks của sản phẩm | ❌ |
| `GET` | `/user-feedbacks` | Lấy feedbacks của user | ✅ |
| `DELETE` | `/{feedbackId}` | Xóa feedback | Admin |
| `GET` | `/all` | Lấy tất cả feedbacks | Admin |

### 💎 Premium Packages (`/api/PremiunPackage`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/` | Lấy danh sách packages | ❌ |
| `POST` | `/{packageId}` | Lấy package theo ID | ❌ |
| `POST` | `/update/{packageId}` | Cập nhật package | Admin |
| `POST` | `/buy` | Mua gói Premium | ✅ |
| `POST` | `/update-premium` | Gán Premium cho user | Admin |

### 📊 Analytics (`/api/Log`)

| Method | Endpoint | Mô tả | Auth |
|--------|----------|-------|------|
| `GET` | `/all` | Lấy tất cả lượt truy cập | Admin |
| `GET` | `/today` | Lượt truy cập hôm nay | Admin |
| `GET` | `/{date}` | Lượt truy cập theo ngày | Admin |
| `GET` | `/registrations-this-month` | Số đăng ký trong tháng | Admin |

### 🏷️ Product Attributes

Mỗi attribute đều có CRUD endpoints:

| Controller | Endpoint Base |
|------------|---------------|
| ProductBrand | `/api/ProductBrand` |
| ProductCategory | `/api/ProductCategory` |
| ProductColor | `/api/ProductColor` |
| ProductMaterial | `/api/ProductMaterial` |
| ProductStyle | `/api/ProductStyle` |
| ProductType | `/api/ProductType` |

---

## 🔐 Xác thực & Phân quyền

### JWT Authentication

```http
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Roles

| RoleId | Role Name | Quyền hạn |
|--------|-----------|-----------|
| 1 | **Admin** | Toàn quyền hệ thống |
| 2 | **User** | Mua hàng, đánh giá, tủ đồ |
| 3 | **Manager** | Quản lý sản phẩm, đơn hàng |

### Luồng xác thực

```
1. Đăng ký → Gửi OTP qua email
2. Xác nhận OTP → Kích hoạt tài khoản
3. Đăng nhập → Nhận JWT token
4. Request API → Gửi Bearer token
5. Đổi mật khẩu → Xác nhận OTP mới
```

### Google OAuth Flow

```
1. Client redirect → /api/Auth/signin-google
2. Google authentication
3. Callback với ID token
4. Server xác thực token
5. Tạo/cập nhật user
6. Trả về JWT token
```

---

## 🤖 Tích hợp AI

### Kiến trúc AI Outfit Suggestion

```
┌─────────────────────────────────────────────────────────────┐
│                     User Input                              │
│     "Tôi muốn mặc đồ đi dự tiệc sang trọng"                │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                   OpenRouter Service                        │
│   Model: deepseek/deepseek-r1-0528-qwen3-8b:free           │
│   Task: Classify input → Style (Formal, Casual, etc.)      │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼ Style: "Formal"
┌─────────────────────────────────────────────────────────────┐
│                    Outfit Service                           │
│   1. Lấy products từ User Closet theo Style                 │
│   2. Fallback: Lấy từ System Products nếu thiếu             │
│   3. Tạo combo: Tops + Bottoms + Footwear + Accessories     │
└──────────────────────────┬──────────────────────────────────┘
                           │
                           ▼
┌─────────────────────────────────────────────────────────────┐
│                  OutfitSuggestionDto                        │
│   - Style: "Formal"                                         │
│   - ComboId: guid                                           │
│   - Tops: { ProductId, Name, Price, ImageURL, ... }        │
│   - Bottoms: { ... }                                        │
│   - Footwears: { ... }                                      │
│   - Accessories: { ... }                                    │
│   - MissingNotice: ["Missing Tops - Use Outfit System"]    │
└─────────────────────────────────────────────────────────────┘
```

### Các Style hỗ trợ

| Style | Mô tả |
|-------|-------|
| **Casual** | Trang phục thường ngày |
| **Formal** | Trang phục lịch sự, công sở |
| **Streetwear** | Phong cách đường phố |
| **Sporty** | Trang phục thể thao |
| **Vintage** | Phong cách cổ điển |
| **Minimalist** | Tối giản |
| **Bohemian** | Phong cách boho |

### Sử dụng API

```http
POST /api/Outfit/suggest
Authorization: Bearer {token}
Content-Type: application/json

{
  "prompt": "Tôi muốn mặc đồ đi cafe với bạn bè vào cuối tuần"
}
```

**Response:**
```json
{
  "style": "Casual",
  "comboId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "tops": {
    "productId": "...",
    "name": "T-Shirt Basic White",
    "price": 299000,
    "imageURL": "https://..."
  },
  "bottoms": { ... },
  "footwears": { ... },
  "accessories": { ... },
  "missingNotice": []
}
```

---

## 💳 Thanh toán

### PayOS Integration

```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│   Client    │ ──▶  │  Formale    │ ──▶  │   PayOS     │
│             │      │    API      │      │    API      │
└─────────────┘      └─────────────┘      └─────────────┘
       │                    │                    │
       │  1. Create Order   │                    │
       │ ──────────────────▶│                    │
       │                    │  2. Create Payment │
       │                    │ ──────────────────▶│
       │                    │                    │
       │                    │  3. Checkout URL   │
       │                    │ ◀──────────────────│
       │  4. Redirect URL   │                    │
       │ ◀──────────────────│                    │
       │                    │                    │
       │  5. Complete Payment on PayOS           │
       │ ───────────────────────────────────────▶│
       │                    │                    │
       │                    │  6. Webhook/Poll   │
       │                    │ ◀──────────────────│
       │                    │                    │
       │  7. Order Complete │                    │
       │ ◀──────────────────│                    │
```

### Payment Status Flow

```
PENDING → COMPLETE (Thanh toán thành công)
        → CANCELLED (Hủy bởi user)
        → FAILED (Lỗi thanh toán)
```

### API Usage

**Tạo Payment:**
```http
POST /api/Payment/create
{
  "orderId": 123,
  "returnUrl": "https://yourapp.com/payment/success",
  "cancelUrl": "https://yourapp.com/payment/cancel"
}
```

**Kiểm tra trạng thái:**
```http
GET /api/Payment/check-status/{orderCode}
```

---

## 📊 Services Overview

| Service | Chức năng chính |
|---------|----------------|
| `UserAccountService` | Đăng ký, đăng nhập, OTP, Google OAuth |
| `UserService` | CRUD users, profile management |
| `ProductService` | CRUD products, search, filter |
| `CartService` | Quản lý giỏ hàng |
| `OrderService` | Tạo đơn hàng từ cart |
| `PaymentService` | Quản lý thanh toán |
| `PayOsService` | Tích hợp PayOS API |
| `PremiumService` | Quản lý gói Premium |
| `OutfitService` | AI gợi ý trang phục |
| `OutfitComboItemService` | Quản lý items trong combo |
| `UserClosetService` | Tủ đồ cá nhân |
| `FeedbackService` | Đánh giá sản phẩm |
| `CloudinaryService` | Upload/delete ảnh |
| `EmailService` | Gửi email SMTP |
| `OpenRouterService` | Gọi AI model |
| `VisitLogService` | Analytics, thống kê |
| `CurrentUserService` | Lấy user từ JWT claims |

---

## 🧪 Testing

### Swagger UI

Truy cập Swagger để test API:
```
https://localhost:5001/swagger
```

### Test với JWT

1. Đăng nhập qua `/api/Auth/login`
2. Copy token từ response
3. Click "Authorize" trong Swagger
4. Nhập: `Bearer {your-token}`
5. Test các protected endpoints

---

## 📝 Quy ước code

### Naming Conventions

| Loại | Convention | Ví dụ |
|------|------------|-------|
| Class | PascalCase | `ProductService` |
| Method | PascalCase | `GetAllProducts()` |
| Variable | camelCase | `productList` |
| Property | PascalCase | `ProductId` |
| Interface | IPascalCase | `IProductService` |
| DTO | PascalCase + Dto | `ProductResponseDto` |

### Project Structure Rules

- **Controllers**: Chỉ chứa routing và validation
- **Services**: Business logic
- **Repositories**: Data access
- **DTOs**: Data transfer, không chứa logic
- **Entities**: Domain models

---

## 🤝 Đóng góp

1. Fork repository
2. Tạo feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Mở Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.

---

## 📞 Liên hệ

- **Project Link**: [https://github.com/your-username/Formale_EXE201](https://github.com/your-username/Formale_EXE201)

---

<div align="center">

**⭐ Star repo này nếu bạn thấy hữu ích! ⭐**

Made with ❤️ by Formale Team

</div>