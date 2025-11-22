🎓 Nền Tảng Bán Khóa Học Online – RESTful API (ASP.NET Core 8)
![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-8.0-blue?logo=.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-2019-red?logo=microsoft-sql-server&logoColor=white)


📌 Tổng Quan Dự Án

Đây là một RESTful API cho nền tảng bán khóa học online, được xây dựng bằng ASP.NET Core 8, tuân thủ SOLID, Clean Code và kiến trúc tách lớp rõ ràng.

🧩 Các Nguyên Tắc SOLID Được Áp Dụng
1. Single Responsibility Principle (SRP)

Mỗi class có một trách nhiệm duy nhất.

CourseService quản lý logic khóa học

AuthService xử lý đăng ký/đăng nhập

Controllers chỉ xử lý Request/Response

2. Open/Closed Principle (OCP)

Mở rộng qua interface

Dễ thêm logic mà không sửa code cũ

3. Liskov Substitution Principle (LSP)

Các service implement interface có thể thay thế lẫn nhau

4. Interface Segregation Principle (ISP)

Chia nhỏ interface theo từng chức năng

IAuthService, ICourseService, ILessonService, ...

5. Dependency Inversion Principle (DIP)

Phụ thuộc abstraction thay vì implementation

Sử dụng DI toàn dự án

📂 Cấu trúc dự án
CoursePlatform/
├── Models/              
├── DTOs/                
├── Data/                
├── Interfaces/          
├── Services/            
├── Controllers/         
├── Program.cs           
└── appsettings.json     

🚀 Các Tính Năng Chính
🔐 Authentication

✔ Đăng ký, đăng nhập với JWT

✔ Phân quyền: Student / Instructor / Admin

📘 Course Management

✔ CRUD khóa học (Instructor)

✔ Tìm kiếm, phân trang

✔ Publish / Unpublish

🎬 Lesson Management

✔ CRUD bài học

✔ Sắp xếp bài học

✔ Quản lý nội dung video

🧑‍🎓 Enrollment

✔ Đăng ký khóa

✔ Xem khóa đã mua

✔ Theo dõi tiến độ %

🛠 Cài Đặt & Chạy Dự Án
Yêu cầu

.NET 8 SDK

SQL Server

VS 2022 hoặc VS Code

1. Tạo project hoặc clone
dotnet new webapi -n CoursePlatform
cd CoursePlatform

2. Cài packages
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore

3. Cập nhật connection string
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CoursePlatformDb;Trusted_Connection=True;TrustServerCertificate=True"
}

4. Migration & Database
dotnet ef migrations add InitialCreate
dotnet ef database update

5. Chạy ứng dụng
dotnet run


API: http://localhost:5000

Swagger: https://localhost:5001/

📡 API Endpoints
🔐 Auth
Method	Endpoint	Mô tả
POST	/api/auth/register	Đăng ký
POST	/api/auth/login	Đăng nhập
📘 Courses
Method	Endpoint	Mô tả
GET	/api/courses	Danh sách khóa học
GET	/api/courses/{id}	Chi tiết
POST	/api/courses	Tạo khóa học
PUT	/api/courses/{id}	Cập nhật
DELETE	/api/courses/{id}	Xóa khóa
GET	/api/courses/my-courses	Khóa học của Instructor
🎬 Lessons
Method	Endpoint	Mô tả
GET	/api/courses/{courseId}/lessons	Danh sách
POST	/api/courses/{courseId}/lessons	Thêm
PUT	/api/courses/{courseId}/lessons/{lessonId}	Sửa
DELETE	/api/courses/{courseId}/lessons/{lessonId}	Xóa
🧑‍🎓 Enrollments
Method	Endpoint	Mô tả
POST	/api/enrollments	Đăng ký
GET	/api/enrollments/my-enrollments	Xem đã mua
PUT	/api/enrollments/{id}/progress	Cập nhật tiến độ
📌 Ví dụ sử dụng API
1. Đăng ký
POST /api/auth/register
Content-Type: application/json

{
  "email": "student@example.com",
  "password": "Password123!",
  "fullName": "Nguyen Van A"
}

2. Đăng nhập
POST /api/auth/login
Content-Type: application/json

{
  "email": "student@example.com",
  "password": "Password123!"
}


Response:

{
  "success": true,
  "message": "Login successful",
  "data": "eyJhbGciOiJIUzI1NiIsInR5cCI..."
}

3. Tạo khóa học
POST /api/courses
Authorization: Bearer {token}
Content-Type: application/json

{
  "title": "Lập trình ASP.NET Core",
  "description": "Từ cơ bản đến nâng cao",
  "price": 499000,
  "thumbnailUrl": "https://example.com/image.jpg",
  "level": "Beginner"
}

4. Đăng ký khóa học
POST /api/enrollments
Authorization: Bearer {token}
Content-Type: application/json

{
  "courseId": 1
}

🚀 Mở Rộng Tương Lai

💳 Payment Gateway

🎥 Upload video

⭐ Review & Rating

🎓 Certificate

✉ Email notifications

⚡ Redis caching

📊 Logging với Serilog

🔍 Advanced filtering

🧱 Best Practices

✔ Repository Pattern
✔ Dependency Injection
✔ DTO Mapping
✔ JWT Authentication
✔ Validation
✔ Error Handling
✔ Swagger Documentation
✔ Clean Architecture mindset