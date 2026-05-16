using Application.DTOs;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/agendamentos")]
public class AgendamentoController : ControllerBase
{
    private readonly CriarAgendamentoUseCase _useCase;
    private readonly ListarAgendamentosUseCase _listarUseCase;

    public AgendamentoController(
        CriarAgendamentoUseCase useCase,
        ListarAgendamentosUseCase listarUseCase)
    {
        _useCase = useCase;
        _listarUseCase = listarUseCase;
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarAgendamentoDto dto)
    {
        await _useCase.Executar(dto);

        return Ok(new { mensagem = "Agendamento criado com sucesso" });
    }

    [HttpGet]
    public async Task<IActionResult> Listar() // ✅ AGORA É ASYNC
    {
        var lista = await _listarUseCase.Executar();
        
        return Ok(lista);
    }
}