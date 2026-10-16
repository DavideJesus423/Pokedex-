using SQLite;

namespace Pokedex.Models;

public class Poder
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    [Indexed]
    public int PokemonId { get; set; }

    public string Nome { get; set; } = "";
}
