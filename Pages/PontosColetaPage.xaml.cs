using Rba.Helpers;
using Rba.Models;
using System.Collections.ObjectModel;

namespace Rba.Pages;

public partial class PontosColetaPage : ContentPage
{
    private readonly SQLiteDatabaseHelper db;
    private ObservableCollection<PontoColeta> lista = new ObservableCollection<PontoColeta>();

    public PontosColetaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        db = new SQLiteDatabaseHelper(dbPath);

        listaView.ItemsSource = lista;
    }

    // Atualiza lista sempre que a tela volta a aparecer
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarDados();
    }

    private async Task CarregarDados()
    {
        var dados = await db.GetAll();
        lista.Clear();
        foreach (var p in dados)
            lista.Add(p);
    }

    // CONSULTAR: carrega todos os pontos
    private async void OnConsultarClicked(object sender, EventArgs e)
    {
        await CarregarDados();
        listaView.IsVisible = true;
    }

    // BUSCAR: filtra conforme texto digitado
    private void OnBuscarTextChanged(object sender, TextChangedEventArgs e)
    {
        string termo = e.NewTextValue?.ToLower() ?? "";
        if (string.IsNullOrWhiteSpace(termo))
        {
            listaView.ItemsSource = lista;
            return;
        }

        var filtrados = lista.Where(p =>
            (p.Nome?.ToLower().Contains(termo) ?? false) ||
            (p.Endereco?.ToLower().Contains(termo) ?? false) ||
            (p.TipoLixo?.ToLower().Contains(termo) ?? false) ||
            (p.Contato?.ToLower().Contains(termo) ?? false) ||
            (p.Horario?.ToLower().Contains(termo) ?? false)
        ).ToList();

        listaView.ItemsSource = filtrados;
    }

    // LIMPAR: limpa campos do formulário
    private void OnLimparClicked(object sender, EventArgs e)
    {
        nomeEntry.Text = enderecoEntry.Text = tipoEntry.Text = contatoEntry.Text = "";
        inicioPicker.Time = new TimeSpan(8, 0, 0);
        fimPicker.Time = new TimeSpan(18, 0, 0);
        BuscarEntry.Text = "";
        lista.Clear();
        listaView.ItemsSource = lista;
        listaView.IsVisible = false;
    }

    // SALVAR novo ponto
    private async void Button_Salvar(object sender, EventArgs e)
    {
        string nome = nomeEntry.Text?.Trim() ?? "";
        string endereco = enderecoEntry.Text?.Trim() ?? "";
        string tipo = tipoEntry.Text?.Trim() ?? "";
        string contato = contatoEntry.Text?.Trim() ?? "";
        string horario = $"{inicioPicker.Time:hh\\:mm} - {fimPicker.Time:hh\\:mm}";

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(endereco))
        {
            await DisplayAlert("Erro", "Preencha pelo menos nome e endereço.", "OK");
            return;
        }

        double latitude = 0;
        double longitude = 0;

        try
        {
            var locations = await Geocoding.GetLocationsAsync(endereco);
            var location = locations?.FirstOrDefault();
            if (location != null)
            {
                latitude = location.Latitude;
                longitude = location.Longitude;
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Geocodificação", ex.Message, "OK");
        }

        var ponto = new PontoColeta
        {
            Nome = nome,
            Endereco = endereco,
            TipoLixo = tipo,
            Contato = contato,
            Horario = horario,
            Latitude = latitude,
            Longitude = longitude
        };

        await db.Insert(ponto);

        await DisplayAlert("Cadastro realizado!", $"Ponto \"{ponto.Nome}\" cadastrado com sucesso!", "OK");
        OnLimparClicked(sender, EventArgs.Empty);

        // força atualização da lista
        await CarregarDados();
    }

    // EXCLUIR
    private async void Button_Excluir(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is PontoColeta ponto)
        {
            bool confirmar = await DisplayAlert("Excluir", $"Deseja excluir {ponto.Nome}?", "Sim", "Não");
            if (confirmar)
            {
                await db.Delete(ponto.Id);
                lista.Remove(ponto);
            }
        }
    }

    // EDITAR
    private async void Button_Editar(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.BindingContext is PontoColeta ponto)
        {
            await Navigation.PushAsync(new EditarPontoColeta(ponto));
        }
    }

    // VOLTAR
    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
