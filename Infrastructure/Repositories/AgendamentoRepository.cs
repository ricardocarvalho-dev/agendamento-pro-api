using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AgendamentoRepository : IAgendamentoRepository
{
    private readonly AppDbContext _context;

    public AgendamentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task Adicionar(Agendamento agendamento)
    {
        await _context.Agendamentos.AddAsync(agendamento);
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Agendamento>> ObterTodos()
    {
        return await _context.Agendamentos.ToListAsync();
    }

    public async Task<Agendamento?> ObterPorId(Guid id)
    {
        return await _context.Agendamentos.FindAsync(id);
    }
}