namespace finansee_api.Models;

public class Usuario
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string Perfil { get; set; } = "usuario";
    public bool Ativo { get; set; } = true;
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    public ICollection<Projeto> ProjetosResponsaveis { get; set; } = new List<Projeto>();
    public ICollection<ItemRealizado> ItensRegistrados { get; set; } = new List<ItemRealizado>();
}
