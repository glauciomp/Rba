using Rba.Helpers;
using Rba.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Rba.Pages;

public partial class PontosColetaPage : ContentPage
{
    private readonly SQLiteDatabaseHelper db;
    ObservableCollection<PontoColeta> lista = new ObservableCollection<PontoColeta>();
    public PontosColetaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        db = new SQLiteDatabaseHelper(dbPath);

        listaView.ItemsSource = lista;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        var dados = await db.GetAll();
        lista.Clear();
        foreach (var p in dados)
            lista.Add(p);
    }

    // Salvar novo ponto (apenas admin)
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

        var ponto = new PontoColeta
        {
            Nome = nome,
            Endereco = endereco,
            TipoLixo = tipo,
            Contato = contato,
            Horario = horario
        };

        await db.Insert(ponto);
        lista.Add(ponto);

        await DisplayAlert("Cadastro realizado!", $"Ponto de coleta \"{ponto.Nome}\" cadastrado com sucesso!", "OK");

        // Limpar campos
        nomeEntry.Text = enderecoEntry.Text = tipoEntry.Text = contatoEntry.Text = "";
        inicioPicker.Time = new TimeSpan(8, 0, 0);
        fimPicker.Time = new TimeSpan(18, 0, 0);
    }

    // Excluir ponto
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

    private async void Button_Editar(object sender, EventArgs e)
    {
        try
        {
            Button btn = sender as Button;
            if (btn?.BindingContext is PontoColeta ponto)
            {
                await Navigation.PushAsync(new EditarPontoColeta(ponto));
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao Editar", ex.Message, "OK");
        }
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}