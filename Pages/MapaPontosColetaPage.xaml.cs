using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class MapaPontosColetaPage : ContentPage
{
    private SQLiteDatabaseHelper db;
    private Location userLocation;
    private string apikey = "Chave_API";

    public MapaPontosColetaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        db = new SQLiteDatabaseHelper(dbPath);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Carrega pontos e tipos dinâmicos ao aparecer
        await CarregarTiposDinamicosAsync();
        CarregarPontos();
    }

    private async Task CarregarTiposDinamicosAsync()
    {
        var todos = await db.GetAll();

        var tipos = todos
            .Select(p => p.TipoLixo?.Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t)
            .ToList();

        // Garante "Todos" como primeira opção
        tipos.Insert(0, "Todos");

        // Atualiza Picker
        TipoPicker.Items.Clear();
        foreach (var t in tipos)
            TipoPicker.Items.Add(t);

        // Seleciona "Todos" por padrão, se existente
        var idxTodos = tipos.FindIndex(t => t.Equals("Todos", StringComparison.OrdinalIgnoreCase));
        if (idxTodos >= 0)
            TipoPicker.SelectedIndex = idxTodos;
    }

    private async void CarregarPontos(List<PontoColeta>? pontos = null)
    {
        try
        {
            if (pontos == null)
                pontos = await db.GetAll();

            mapa.Pins.Clear();

            foreach (var ponto in pontos)
            {
                if (!string.IsNullOrWhiteSpace(ponto.Endereco))
                {
                    var positions = await Geocoding.GetLocationsAsync(ponto.Endereco);
                    var location = positions?.FirstOrDefault();
                    if (location != null)
                    {
                        var pin = new Pin
                        {
                            Label = ponto.Nome,
                            Address = ponto.Endereco,
                            Location = new Location(location.Latitude, location.Longitude),
                            Type = PinType.Place,
                            BindingContext = ponto
                        };

                        pin.InfoWindowClicked += Pin_InfoWindowClicked;
                        mapa.Pins.Add(pin);
                    }
                }
            }

            if (mapa.Pins.Count > 0)
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(mapa.Pins.First().Location, Distance.FromKilometers(5)));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os pontos: {ex.Message}", "OK");
        }
    }

    private async void TipoPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (TipoPicker.SelectedItem == null)
            return;

        var tipoSelecionado = TipoPicker.SelectedItem.ToString();
        var todos = await db.GetAll();

        if (string.Equals(tipoSelecionado, "Todos", StringComparison.OrdinalIgnoreCase))
        {
            CarregarPontos(todos);
            return;
        }

        var filtrados = todos
            .Where(p => string.Equals(p.TipoLixo?.Trim(), tipoSelecionado, StringComparison.OrdinalIgnoreCase))
            .ToList();

        CarregarPontos(filtrados);
    }

    private async void Pin_InfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        var pin = sender as Pin;
        if (pin?.BindingContext is PontoColeta ponto)
        {
            await DisplayAlert(ponto.Nome,
                $"Endereço: {ponto.Endereco}\n" +
                $"Tipo: {ponto.TipoLixo}\n" +
                $"Contato: {ponto.Contato}\n" +
                $"Horário: {ponto.Horario}",
                "Fechar");
        }
    }

    private async void Button_MinhaLocalizacao_Clicked(object sender, EventArgs e)
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permissão negada", "Não foi possível acessar a localização. Verifique se o aplicativo tem permissão para usar o GPS.", "OK");
                return;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

            try
            {
                userLocation = await Geolocation.GetLocationAsync(request);
            }
            catch (FeatureNotEnabledException)
            {
                await DisplayAlert("Localização desativada",
                    "O GPS do seu dispositivo está desligado. Ative a localização nas configurações para usar esta função.",
                    "OK");
                return;
            }

            if (userLocation != null)
            {
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(userLocation, Distance.FromKilometers(5)));
            }
            else
            {
                await DisplayAlert("Aviso", "Não foi possível determinar sua localização atual.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao tentar obter a localização:\n{ex.Message}", "OK");
        }
    }

    private async void Button_PontosProximos_Clicked(object sender, EventArgs e)
    {
        if (userLocation == null)
        {
            await DisplayAlert("Aviso", "Primeiro obtenha sua localização.", "OK");
            return;
        }

        var todos = await db.GetAll();

        var proximos = todos
            .Where(p => p.Latitude != 0 && p.Longitude != 0)
            .Select(p => new
            {
                Ponto = p,
                Distancia = Location.CalculateDistance(userLocation, new Location(p.Latitude, p.Longitude), DistanceUnits.Kilometers)
            })
            .OrderBy(x => x.Distancia)
            .Take(2)
            .Select(x => x.Ponto)
            .ToList();

        if (proximos.Count == 0)
        {
            await DisplayAlert("Aviso", "Nenhum ponto de coleta encontrado próximo da sua localização.", "OK");
            return;
        }

        CarregarPontos(proximos);
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
