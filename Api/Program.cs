using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Habilita logging para console (capturado pelo Azure Log Stream)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.WebHost.CaptureStartupErrors(true).UseSetting("detailedErrors", "true");

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o Banco de Dados para usar SQLite com caminho persistente e fixo no Azure (D:\home\data)
builder.Services.AddDbContext<AppDbContext>(options =>
{
    // Usa a pasta wwwroot ou uma subfolder com permissões garantidas
    string dataPath = Path.Combine(AppContext.BaseDirectory, "data");
    
    // Fallback para D:\home\site se wwwroot não funcionar
    if (string.IsNullOrEmpty(AppContext.BaseDirectory))
    {
        dataPath = @"D:\home\site\data";
    }

    if (!Directory.Exists(dataPath))
        Directory.CreateDirectory(dataPath);

    var dbPath = Path.Combine(dataPath, "agendamento.db");
    
    SQLitePCL.Batteries.Init();
    
    options.UseSqlite($"Data Source={dbPath}");
});

// --- Registro dos Serviços e Casos de Uso ---
builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();

// AJUSTADO: Como o NotificationService foi limpo do deadlock, podemos registrá-lo com segurança!
builder.Services.AddSingleton<IMessagingService>(sp => 
    new NotificationService(sp.GetRequiredService<ILogger<NotificationService>>()));

builder.Services.AddScoped<CriarAgendamentoUseCase>();
builder.Services.AddScoped<ListarAgendamentosUseCase>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

// Cria/Migra o banco na inicialização com logging explícito
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        Console.WriteLine(">>> Iniciando EF Core Migrate...");
        logger.LogInformation("Applying migrations...");

        db.Database.Migrate();

        Console.WriteLine(">>> EF Core Migrate concluído.");
        logger.LogInformation("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($">>> Erro ao aplicar migrations: {ex.Message}");
        logger.LogError(ex, "Error applying migrations");
    }
}

app.Run();