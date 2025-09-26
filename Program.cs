using Authenticator.API.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/authentication-api-.txt", rollingInterval: RollingInterval.Day)
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração dos serviços
builder.Services.AddControllers();

// Configuração dos bancos de dados
builder.Services.AddDatabaseServices(builder.Configuration);

// Configuração da autenticação JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configuração dos serviços de aplicação
builder.Services.AddApplicationServices();

// Configuração do Swagger
builder.Services.AddSwaggerServices();

// Configuração do CORS
builder.Services.AddCorsServices(builder.Configuration);

// Register application services
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("AccessControlDatabase")!, name: "access_control_db")
    .AddNpgSql(builder.Configuration.GetConnectionString("MultiTenantDatabase")!, name: "multi_tenant_db");

var app = builder.Build();

// Configuração do pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("DefaultPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

app.MapGet("/info", () => new
{
    Name = "Authentication API",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
});

try
{
    Log.Information($"Iniciando Authentication API no endereço ");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao iniciar Authentication API");
}
finally
{
    Log.CloseAndFlush();
}