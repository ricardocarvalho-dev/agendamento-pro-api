using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Repositories;
using Application.Interfaces;
using Application.UseCases;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Captura a string de conexão correta dependendo do ambiente ativo
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Configura o Banco de Dados de forma dinâmica
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Se estiver rodando local (Development), usa SQLite
        options.UseSqlite(connectionString);
    }
    else
    {
        // Se estiver rodando no Azure (Production), usa SQL Server
        options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Infrastructure"));
    }
});

builder.Services.AddScoped<IAgendamentoRepository, AgendamentoRepository>();
builder.Services.AddScoped<CriarAgendamentoUseCase>();
builder.Services.AddScoped<ListarAgendamentosUseCase>();

var app = builder.Build();

/*
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
*/
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.MapControllers();

// Mantém o EnsureCreated apenas para o ambiente de testes local
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
    }
}

app.Run();