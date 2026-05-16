using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases;

public class CriarAgendamentoUseCase
{
    private readonly IAgendamentoRepository _repo;

    public CriarAgendamentoUseCase(IAgendamentoRepository repo)
    {
        _repo = repo;
    }

    public async Task Executar(CriarAgendamentoDto dto)
    {
        var agendamento = new Agendamento
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId,
            DataHora = dto.DataHora
        };

        // Corrigido de _repositorio para _repo:
        await _repo.Adicionar(agendamento);
    }
}