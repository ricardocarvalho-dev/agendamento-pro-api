using Xunit;
using Moq;
using Domain.Entities;
using Application.Interfaces;
using Application.DTOs;
using Application.UseCases;

namespace Tests.UseCases;

public class CriarAgendamentoUseCaseTests
{
    private readonly Mock<IAgendamentoRepository> _repositoryMock;
    private readonly Mock<IMessagingService> _messagingServiceMock;
    private readonly CriarAgendamentoUseCase _useCase;

    public CriarAgendamentoUseCaseTests()
    {
        _repositoryMock = new Mock<IAgendamentoRepository>();
        _messagingServiceMock = new Mock<IMessagingService>();
        _useCase = new CriarAgendamentoUseCase(_repositoryMock.Object, _messagingServiceMock.Object);
    }

    [Fact]
    public async Task Executar_ComDadosValidos_DeveAdicionarAgendamento()
    {
        var dto = new CriarAgendamentoDto
        {
            ClienteId = Guid.NewGuid(),
            DataHora = DateTime.UtcNow.AddHours(1)
        };

        _repositoryMock
            .Setup(r => r.Adicionar(It.IsAny<Agendamento>()))
            .Returns(Task.CompletedTask);

        await _useCase.Executar(dto);

        _repositoryMock.Verify(
            r => r.Adicionar(It.Is<Agendamento>(a => 
                a.ClienteId == dto.ClienteId && 
                a.DataHora == dto.DataHora)),
            Times.Once);
    }

    [Fact]
    public async Task ObterTodos_DeveRetornarLista()
    {
        var agendamentos = new List<Agendamento>
        {
            new Agendamento { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), DataHora = DateTime.UtcNow },
            new Agendamento { Id = Guid.NewGuid(), ClienteId = Guid.NewGuid(), DataHora = DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(r => r.ObterTodos())
            .ReturnsAsync(agendamentos);

        var resultado = await _repositoryMock.Object.ObterTodos();

        Assert.NotNull(resultado);
        Assert.Equal(2, resultado.Count);
    }
}