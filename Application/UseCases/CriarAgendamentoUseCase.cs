using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases;

public class CriarAgendamentoUseCase
{
    private readonly IAgendamentoRepository _repo;
    // Forçando o caminho completo do namespace para o compilador não se perder:
    private readonly Application.Interfaces.IMessagingService _messagingService;

    // Construtor atualizado com o caminho completo
    public CriarAgendamentoUseCase(IAgendamentoRepository repo, Application.Interfaces.IMessagingService messagingService)
    {
        _repo = repo;
        _messagingService = messagingService;
    }

    /*
    public async Task Executar(CriarAgendamentoDto dto)
    {
        var agendamento = new Agendamento
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId,
            DataHora = dto.DataHora
        };

        // 1. Salva no banco de dados
        await _repo.Adicionar(agendamento);

        // 2. Dispara para o RabbitMQ na nuvem
        try
        {
            _messagingService.EnviarNotificacaoAgendamento(new 
            {
                AgendamentoId = agendamento.Id,
                ClienteId = agendamento.ClienteId,
                DataHora = agendamento.DataHora,
                Status = "Criado"
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Mensageria] Erro ao publicar evento: {ex.Message}");
        }
    }
    */
    public async Task Executar(CriarAgendamentoDto dto)
    {
        var agendamento = new Agendamento
        {
            Id = Guid.NewGuid(),
            ClienteId = dto.ClienteId,
            DataHora = dto.DataHora
        };

        try
        {
            await _repo.Adicionar(agendamento);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERRO REPO: " + ex.ToString());
            throw;
        }
    }
}