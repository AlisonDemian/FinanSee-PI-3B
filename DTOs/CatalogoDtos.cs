namespace finansee_api.DTOs;

public class CategoriaResponse
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}

public class NaturezaResponse
{
    public int Id { get; set; }
    public int CategoriaId { get; set; }
    public string Categoria { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? UnidadeMedida { get; set; }
}
