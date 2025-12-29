using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineCourseSellingPlatform.Data;
using OnlineCourseSellingPlatform.Interfaces;
using OnlineCourseSellingPlatform.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#region Add Services Here
// sử dụng restful api controllers
builder.Services.AddControllers();
// đăng ký dịch vụ swagger generator để tạo tài liệu api (OpenApi)
builder.Services.AddSwaggerGen();
// đăng ký dịch vụ api endpoints cho project giúp swagger (OpenApi) tự động đọc được các endpoint của dự án
builder.Services.AddEndpointsApiExplorer();
// đăng ký dịch vụ truy cập HttpContext hiện tại trong các lớp dịch vụ
builder.Services.AddHttpContextAccessor();
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
        Description = "RESTful API for Online Course Platform with Categories, Reviews, Wishlist, and Admin Dashboard" // Mô tả API
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
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("OnlineCourseSellingPlatform")));
#endregion

#region Jwt Authentication Configuration here
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new Exception("Missing Jwt Key!");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
#endregion

#region Dependency Injection Configuration here
// Core Services
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

// New Services
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IAdminService, AdminService>();

#endregion

#region CORS Configuration here
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
#endregion

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Course Platform Api v1");
        c.RoutePrefix = string.Empty; // Swagger at root
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();