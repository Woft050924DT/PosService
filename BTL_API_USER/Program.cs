using BLL;
using DAL;
using DTO;
using DTO_Models;
using HandleLogChangeDB;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

//dependency inject
builder.Services.AddTransient<DTO_I_DBHelper, DAL_DBHelper>();
builder.Services.AddTransient<DAL_KhachHang, DAL_KhachHang>();
builder.Services.AddTransient<BLL_KhachHang, BLL_KhachHang>();
builder.Services.AddTransient<DAL_Dashboard, DAL_Dashboard>();
builder.Services.AddTransient<BLL_Dashboard, BLL_Dashboard>();
//dependency for track hoaDon
builder.Services.AddTransient<MarkInvoiceDirty, MarkInvoiceDirty>();
builder.Services.AddTransient<NotifyAdmin, NotifyAdmin>();
builder.Services.AddTransient<HandleLog, HandleLog>();

//allow frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5500")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Đăng ký cấu hình kiểu mạnh
builder.Services.Configure<DefaultConnect>(
    builder.Configuration.GetSection("DefaultConnect"));

//for track hoaDon
builder.Services.AddDbContext<HdvContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetSection("DefaultConnect")
               .Get<DefaultConnect>()
               .connectionString
    );
});

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//listener the db changing
builder.Services.AddHostedService<InvoiceChangeLogListener>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();
