using Domain.Entities;

namespace Application.Interfaces;

public interface IAgendamentoRepository
{
    Task<List<Agendamento>> ObterTodos();
    Task<Agendamento?> ObterPorId(Guid id);
    Task Adicionar(Agendamento agendamento);
}