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
        try
        {
            // Tenta executar a lógica de criação do agendamento
            await _useCase.Executar(dto);
            return Ok(new { mensagem = "Agendamento criado com sucesso" });
        }
        catch (Exception ex)
        {
            // CAPTURA DO VILÃO: Retorna o erro exato textualmente para o Swagger em vez de dar Erro 500 em branco
            return StatusCode(500, new { 
                mensagem = "A API barrou um erro interno no Use Case!",
                erroReal = ex.Message, 
                ondeQuebrou = ex.StackTrace,
                erroInterno = ex.InnerException?.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> Listar() // ✅ AGORA É ASYNC
    {
        try
        {
            var lista = await _listarUseCase.Executar();
            return Ok(lista);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { 
                mensagem = "Erro ao tentar listar agendamentos!",
                erroReal = ex.Message,
                ondeQuebrou = ex.StackTrace
            });
        }
    }
}