using Microsoft.IdentityModel.Logging;
using Twileloop.EntraID;
using Twileloop.EntraID.DemoApi.EntraID;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<MyConfigResolver>();
builder.Services.AddSingleton<MyLogger>();
var serviceProvider = builder.Services.BuildServiceProvider();

builder.Services.AddEntraID(opt =>
{
    opt.Enable = true;
    opt.EnableEventLogging = true;
    opt.ConfigurationResolver = serviceProvider.GetService<MyConfigResolver>(); 
    opt.SecurityEventLogger = serviceProvider.GetService<MyLogger>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
