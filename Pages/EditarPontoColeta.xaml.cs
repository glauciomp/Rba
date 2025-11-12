using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class EditarPontoColeta : ContentPage
{
    private SQLiteDatabaseHelper db;
    private PontoColeta ponto;

    public EditarPontoColeta(PontoColeta pontoSelecionado)
    {
        InitializeComponent();

        ponto = pontoSelecionado;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");

        db = new SQLiteDatabaseHelper(dbPath);

        nomeEntry.Text = ponto.Nome;
        enderecoEntry.Text = ponto.Endereco;
        tipoEntry.Text = ponto.TipoLixo;
        contatoEntry.Text = ponto.Contato;

        if (!string.IsNullOrEmpty(ponto.Horario))
        {
            var partes = ponto.Horario.Split(" - ");
            if (partes.Length == 2)
            {
                inicioPicker.Time = TimeSpan.Parse(partes[0]);
                fimPicker.Time = TimeSpan.Parse(partes[1]);
            }
        }
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        try
        {
            var inicio = inicioPicker.Time;
            var fim = fimPicker.Time;

            if (fim <= inicio)
            {
                await DisplayAlert("Erro", "O horário de término deve ser maior que o de início.", "OK");
                return;
            }

            ponto.Nome = nomeEntry.Text;
            ponto.Endereco = enderecoEntry.Text;
            ponto.TipoLixo = tipoEntry.Text;
            ponto.Contato = contatoEntry.Text;
            ponto.Horario = $"{inicio:hh\\:mm} - {fim:hh\\:mm}";

            // Se o endereço mudou, atualiza as coordenadas
            await AtualizarCoordenadasSeNecessario();


            await db.Update(ponto);

            await DisplayAlert("Sucesso", "Ponto atualizado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha ao salvar alterações", "OK");
        }
    }
    // Atualiza latitude e longitude se o endereço foi alterado
    private async Task AtualizarCoordenadasSeNecessario()
    {
        try
        {
            var positions = await Geocoding.GetLocationsAsync(ponto.Endereco);
            var location = positions?.FirstOrDefault();

            if (location != null)
            {
                ponto.Latitude = location.Latitude;
                ponto.Longitude = location.Longitude;
            }
            else
            {
                await DisplayAlert("Aviso", "Não foi possível atualizar as coordenadas com base no endereço informado.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Geolocalização", $"Não foi possível obter coordenadas: {ex.Message}", "OK");
        }
    }
    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool cancelar = await DisplayAlert("Cancelar edi��o", "Deseja sair sem salvar?", "Sim", "Não");
        if (cancelar)
            await Navigation.PopAsync();
    }
}
