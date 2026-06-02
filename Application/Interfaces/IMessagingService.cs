namespace Application.Interfaces;

public interface IMessagingService
{
    void EnviarNotificacaoAgendamento(object dadosAgendamento);
}