using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Add Services Here
// sử dụng restful api controllers
builder.Services.AddControllers();
// đăng ký dịch vụ swagger generator để tạo tài liệu api (OpenApi)
builder.Services.AddSwaggerGen();
// đăng ký dịch vụ api endpoints cho project giúp swagger (OpenApi) tự động đọc được các endpoint của dự án
builder.Services.AddEndpointsApiExplorer();
#endregion


#region Swagger Configuration here 
// swagger configuration with jwt support   
// Cấu hình Swagger với hỗ trợ JWT Bearer Authentication
builder.Services.AddSwaggerGen(c =>
{
    // Tạo một tài liệu Swagger với version và thông tin cơ bản
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Course Platform API",       // Tên API hiển thị trên Swagger UI
        Version = "v1",                      // Phiên bản API
        Description = "RESTful API for Online Course Platform" // Mô tả API
    });

    // định nghĩa thêm version khi mở rộng

    // Thêm định nghĩa bảo mật kiểu Bearer (JWT)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "Enter 'Bearer' [space] and then your token",
        Name = "Authorization",             // Header sẽ dùng tên "Authorization"
        In = ParameterLocation.Header,      // Token sẽ được gửi trong Header
        Type = SecuritySchemeType.ApiKey,   // Loại bảo mật là ApiKey (dùng cho token)
        Scheme = "Bearer"                   // Scheme dùng là Bearer
    });

    // Yêu cầu bảo mật toàn bộ API dùng Bearer token
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, // Tham chiếu đến SecurityDefinition
                    Id = "Bearer"                        // ID phải trùng với "Bearer" đã định nghĩa
                }
            },
            Array.Empty<string>()  // Không yêu cầu role đặc biệt, chỉ cần token
        }
    });
});
#endregion

#region Database configuration here

#endregion

#region Jwt Authentication Configuration here

#endregion

#region Dependency Injection Configuration here

#endregion

#region CORS Configuration here

#endregion

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
