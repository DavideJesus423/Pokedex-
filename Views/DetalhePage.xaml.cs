namespace Pokedex.Views;

[QueryProperty(nameof(PokemonId), "id")]
public partial class DetalhePage : ContentPage
{
    private int _id = 0;

    public string PokemonId
    {
        set
        {
            int.TryParse(value, out _id);
        }
    }

    public DetalhePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarPokemon();
    }

    private async Task CarregarPokemon()
    {
        var pokemon = await App.Repo.BuscarAsync(_id);
        if (pokemon == null) return;

        Title = pokemon.Nome;
        lblNome.Text = pokemon.Nome;
        lblTipo.Text = "Tipo: " + pokemon.Tipo;
        imgPokemon.Source = pokemon.Imagem;

        listaPoderes.Children.Clear();
        foreach (var poder in pokemon.Poderes)
            listaPoderes.Children.Add(new Label { Text = "• " + poder.Nome });
    }

    private async void btnEditar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync($"cadastro?id={_id}");
    }

    private async void btnExcluir_Clicked(object sender, EventArgs e)
    {
        var confirmar = await DisplayAlert("Excluir", "Excluir este Pokémon?", "Excluir", "Cancelar");
        if (!confirmar) return;

        await App.Repo.ExcluirAsync(_id);
        await Shell.Current.GoToAsync("..");
    }
}
