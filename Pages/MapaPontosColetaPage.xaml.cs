using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class MapaPontosColetaPage : ContentPage
{
    private SQLiteDatabaseHelper db;

    public MapaPontosColetaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        db = new SQLiteDatabaseHelper(dbPath);

        CarregarPontos();

       
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
                            Type = PinType.Place
                        };
                        pin.BindingContext = ponto;
                        pin.MarkerClicked += Pin_MarkerClicked; // Adiciona o evento para o ícone
                        pin.InfoWindowClicked += Pin_InfoWindowClicked; // Adiciona o evento para o balão

                        
                       
                        mapa.Pins.Add(pin);
                    }
                }
            }

            // Centraliza no primeiro pin
            if (mapa.Pins.Count > 0)
            {
                var firstPin = mapa.Pins.First();
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(firstPin.Location, Distance.FromKilometers(5)));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os pontos: {ex.Message}", "OK");
        }
    }

    private async void Pin_InfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        // Obtém o Pin que foi clicado
        var pin = sender as Pin;

        // obter o objeto PontoColeta 
        if (pin?.BindingContext is PontoColeta ponto)
        {
           
            string mensagem =
                $"Endereço: {ponto.Endereco}\n" +
                $"Tipo de Lixo: {ponto.TipoLixo}" +
                $"Contato: {ponto.Contato}\n" +
                $"Horário: {ponto.Horario}";

            await DisplayAlert(ponto.Nome, mensagem, "OK");
        }

    }

    private void Pin_MarkerClicked(object? sender, PinClickedEventArgs e)
    {
        Pin_InfoWindowClicked(sender, e);
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
                await DisplayAlert("Permissão negada", "Não foi possível acessar a localização.", "OK");
                return;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);

            if (location != null)
            {
                var userLocation = new Location(location.Latitude, location.Longitude);
                mapa.MoveToRegion(MapSpan.FromCenterAndRadius(userLocation, Distance.FromKilometers(5)));
                await DisplayAlert("Localização", $"Você está aqui: {location.Latitude}, {location.Longitude}", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível obter a localização: {ex.Message}", "OK");
        }
    }

    private void Button_AtualizarPontos_Clicked(object sender, EventArgs e)
    {
        CarregarPontos();
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
