using SQLite;

namespace Pokedex.Models;

public class Pokemon
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Nome { get; set; } = "";

    public string Tipo { get; set; } = "";

    public string Imagem { get; set; } = "";

    [Ignore]
    public List<Poder> Poderes { get; set; } = new();
}
