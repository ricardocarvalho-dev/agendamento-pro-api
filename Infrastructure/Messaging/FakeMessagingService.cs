using Application.Interfaces;

namespace Infrastructure.Messaging
{
    public class FakeMessagingService : IMessagingService
    {
        public void EnviarNotificacaoAgendamento(object dadosAgendamento)
        {
            // Mock: não faz nada, apenas finge que enviou
        }
    }
}
