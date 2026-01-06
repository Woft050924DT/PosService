using apiIventory;
using BLL;
using DAL;
using DTO;

var builder = WebApplication.CreateBuilder(args);

// ===================== CORS =====================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "https://localhost:5173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// =================================================

// Inject dependency
builder.Services.AddTransient<dto_helloWorld, helloWorld>();
builder.Services.AddScoped<dal_Products>();
builder.Services.AddScoped<bll_Products>();
builder.Services.AddScoped<dal_InventoryTransaction>();
builder.Services.AddScoped<bll_InventoryTransaction>();
builder.Services.AddScoped<dal_Suppliers>();
builder.Services.AddScoped<bll_Suppliers>();

builder.Services.Configure<appSetting>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));

builder.Services.AddScoped<bll_Categories>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===================== PIPELINE =====================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();              // ✅ BẮT BUỘC
app.UseCors("AllowReact");     // ✅ DÒNG QUAN TRỌNG NHẤT
app.UseAuthorization();

app.MapControllers();
app.Run();
