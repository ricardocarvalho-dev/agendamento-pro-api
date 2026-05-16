namespace Domain.Entities;

public class Agendamento
{
    public Guid Id { get; set; }
    public Guid ClienteId { get; set; }
    public DateTime DataHora { get; set; }
}