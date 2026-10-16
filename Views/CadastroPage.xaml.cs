using Pokedex.Models;

namespace Pokedex.Views;

[QueryProperty(nameof(PokemonId), "id")]
public partial class CadastroPage : ContentPage
{
    private int _id = 0;
    private string _imagem = "";
    private List<string> _poderes = new List<string>();

    public string PokemonId
    {
        set
        {
            if (int.TryParse(value, out var id))
            {
                _id = id;
                _ = CarregarPokemon();
            }
        }
    }

    public CadastroPage()
    {
        InitializeComponent();
    }

    private async Task CarregarPokemon()
    {
        var pokemon = await App.Repo.BuscarAsync(_id);
        if (pokemon == null) return;

        Title = "Editar Pokémon";
        txtNome.Text = pokemon.Nome;
        txtTipo.Text = pokemon.Tipo;
        _imagem = pokemon.Imagem;
        imgPokemon.Source = _imagem;

        _poderes = pokemon.Poderes.Select(p => p.Nome).ToList();
        AtualizarPoderes();
    }

    private void AtualizarPoderes()
    {
        listaPoderes.Children.Clear();

        foreach (var poder in _poderes)
        {
            var linha = new HorizontalStackLayout { Spacing = 10 };
            linha.Children.Add(new Label { Text = poder, VerticalOptions = LayoutOptions.Center });

            var botao = new Button { Text = "Remover", FontSize = 12 };
            botao.Clicked += (s, e) =>
            {
                _poderes.Remove(poder);
                AtualizarPoderes();
            };
            linha.Children.Add(botao);

            listaPoderes.Children.Add(linha);
        }
    }

    private async void btnImagem_Clicked(object sender, EventArgs e)
    {
        try
        {
            var foto = await MediaPicker.Default.PickPhotoAsync();
            if (foto == null) return;

            var destino = Path.Combine(FileSystem.AppDataDirectory,
                Guid.NewGuid() + Path.GetExtension(foto.FileName));

            using var origem = await foto.OpenReadAsync();
            using var saida = File.OpenWrite(destino);
            await origem.CopyToAsync(saida);

            _imagem = destino;
            imgPokemon.Source = destino;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private void btnAdicionarPoder_Clicked(object sender, EventArgs e)
    {
        var texto = txtPoder.Text?.Trim() ?? "";
        if (texto.Length == 0) return;

        _poderes.Add(texto);
        txtPoder.Text = "";
        AtualizarPoderes();
    }

    private async void btnSalvar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtTipo.Text))
        {
            await DisplayAlert("Atenção", "Informe o nome e o tipo.", "OK");
            return;
        }

        var pokemon = new Pokemon
        {
            Id = _id,
            Nome = txtNome.Text.Trim(),
            Tipo = txtTipo.Text.Trim(),
            Imagem = _imagem,
            Poderes = _poderes.Select(p => new Poder { Nome = p }).ToList()
        };

        await App.Repo.SalvarAsync(pokemon);
        await Shell.Current.GoToAsync("..");
    }
}
