using BLL;
using DAL;
using DTO;
var builder = WebApplication.CreateBuilder(args);

//dependency inject
builder.Services.AddTransient<DTO_I_DBHelper, DAL_DBHelper>();
builder.Services.AddTransient<DAL_KhachHang, DAL_KhachHang>();
builder.Services.AddTransient<BLL_KhachHang, BLL_KhachHang>();


// Đăng ký cấu hình kiểu mạnh
builder.Services.Configure<DefaultConnect>(
    builder.Configuration.GetSection("DefaultConnect"));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
