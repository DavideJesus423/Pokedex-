using Pokedex.Models;
using SQLite;

namespace Pokedex.Services;

public class PokemonRepository
{
    private SQLiteAsyncConnection? _db;

    private async Task InitAsync()
    {
        if (_db != null) return;

        var caminho = Path.Combine(FileSystem.AppDataDirectory, "pokedex.db3");
        _db = new SQLiteAsyncConnection(caminho);
        await _db.CreateTableAsync<Pokemon>();
        await _db.CreateTableAsync<Poder>();
    }

    public async Task<List<Pokemon>> ListarAsync(string? busca = null)
    {
        await InitAsync();

        List<Pokemon> lista;
        if (string.IsNullOrWhiteSpace(busca))
            lista = await _db!.QueryAsync<Pokemon>("SELECT * FROM Pokemon ORDER BY Nome");
        else
            lista = await _db!.QueryAsync<Pokemon>(
                "SELECT * FROM Pokemon WHERE Nome LIKE ? ORDER BY Nome", $"%{busca.Trim()}%");

        foreach (var p in lista)
            p.Poderes = await _db.Table<Poder>().Where(x => x.PokemonId == p.Id).ToListAsync();

        return lista;
    }

    public async Task<Pokemon?> BuscarAsync(int id)
    {
        await InitAsync();

        var p = await _db!.Table<Pokemon>().Where(x => x.Id == id).FirstOrDefaultAsync();
        if (p == null) return null;

        p.Poderes = await _db.Table<Poder>().Where(x => x.PokemonId == id).ToListAsync();
        return p;
    }

    public async Task SalvarAsync(Pokemon pokemon)
    {
        await InitAsync();

        if (pokemon.Id == 0)
            await _db!.InsertAsync(pokemon);
        else
            await _db!.UpdateAsync(pokemon);

        await _db.ExecuteAsync("DELETE FROM Poder WHERE PokemonId = ?", pokemon.Id);

        foreach (var poder in pokemon.Poderes)
        {
            poder.Id = 0;
            poder.PokemonId = pokemon.Id;
            await _db.InsertAsync(poder);
        }
    }

    public async Task ExcluirAsync(int id)
    {
        await InitAsync();

        await _db!.ExecuteAsync("DELETE FROM Poder WHERE PokemonId = ?", id);
        await _db.ExecuteAsync("DELETE FROM Pokemon WHERE Id = ?", id);
    }
}
