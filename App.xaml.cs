using Pokedex.Services;

namespace Pokedex;

public partial class App : Application
{
    public static PokemonRepository Repo { get; } = new PokemonRepository();

    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
