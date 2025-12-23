using apiIventory;
using BLL;
using DAL;
using DTO;


var builder = WebApplication.CreateBuilder(args);

//Inject dependency into the project
builder.Services.AddTransient<dto_helloWorld, helloWorld>();
builder.Services.AddScoped<dal_Products>();
builder.Services.AddScoped<bll_Products>();
builder.Services.AddScoped<dal_InventoryTransaction>();
builder.Services.AddScoped<bll_InventoryTransaction>();
// Strongly typed config
builder.Services.Configure<appSetting>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.Configure<ConnectionStrings>(
    builder.Configuration.GetSection("ConnectionStrings"));


// Add services to the container.
builder.Services.AddScoped<bll_Categories>();
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
