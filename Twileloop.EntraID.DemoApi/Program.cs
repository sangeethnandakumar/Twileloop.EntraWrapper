using Microsoft.IdentityModel.Logging;
using Twileloop.EntraID;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntraID(opt =>
{
    opt.Enable = true;
    opt.EnableEventLogging = true;
    opt.EntraConfig = builder.Configuration.GetSection("EntraConfig");
    opt.OnSecurityEventLog = msg =>
    {
        Console.WriteLine(msg);
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
