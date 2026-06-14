namespace finansee_api.Models;

public class Projeto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? NumeroEdital { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataFim { get; set; }
    public decimal ValorTotalOrcamento { get; set; }
    public int UsuarioResponsavelId { get; set; }
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public Usuario? UsuarioResponsavel { get; set; }
    public ICollection<ItemOrcado> ItensOrcados { get; set; } = new List<ItemOrcado>();
    public ICollection<ItemRealizado> ItensRealizados { get; set; } = new List<ItemRealizado>();
}
