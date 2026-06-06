
using System.Text.Json;

namespace AgendamentoPro.FrontEnd.Services
{
    public class AgendamentoService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseApiUrl = "https://agendamentopro-api-ricardo-staging-hzhzhfddb7djbmd6.centralus-01.azurewebsites.net/api/agendamentos";

        public AgendamentoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AgendamentoDto>> GetAgendamentosAsync()
        {
            var response = await _httpClient.GetAsync(_baseApiUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<AgendamentoDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task CreateAgendamentoAsync(AgendamentoCreateDto agendamento)
        {
            var jsonContent = new StringContent(JsonSerializer.Serialize(agendamento),
                System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(_baseApiUrl, jsonContent);
            response.EnsureSuccessStatusCode();
        }
    }

    public class AgendamentoDto
    {
        public Guid AgendamentoId { get; set; }
        public Guid ClienteId { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; }
    }

    public class AgendamentoCreateDto
    {
        public Guid ClienteId { get; set; }
        public DateTime DataHora { get; set; }
        public string Status { get; set; }
        public string ClienteIdString { get; set; } = string.Empty; // Added for Blazor InputText binding
    }
}
