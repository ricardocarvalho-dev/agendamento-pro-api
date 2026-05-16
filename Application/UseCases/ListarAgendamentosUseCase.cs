using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases;

public class ListarAgendamentosUseCase
{
    private readonly IAgendamentoRepository _repo;

    public ListarAgendamentosUseCase(IAgendamentoRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Agendamento>> Executar()
    {
        return await _repo.ObterTodos();
    }
}