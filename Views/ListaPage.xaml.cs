using Pokedex.Models;

namespace Pokedex.Views;

public partial class ListaPage : ContentPage
{
    public ListaPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarLista();
    }

    private async Task CarregarLista()
    {
        listaPokemons.ItemsSource = await App.Repo.ListarAsync(barraBusca.Text);
    }

    private async void barraBusca_TextChanged(object sender, TextChangedEventArgs e)
    {
        await CarregarLista();
    }

    private async void btnNovo_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("cadastro");
    }

    private async void listaPokemons_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Pokemon pokemon)
        {
            listaPokemons.SelectedItem = null;
            await Shell.Current.GoToAsync($"detalhe?id={pokemon.Id}");
        }
    }
}
