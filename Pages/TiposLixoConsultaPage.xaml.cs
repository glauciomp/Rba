using Microsoft.Maui.Controls;
using Rba.Models;
using Rba.Helpers;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace Rba.Pages;

public partial class TiposLixoConsultaPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _db;
    private List<TipoLixo> todosTipos = new(); // Lista completa para busca

    public bool IsMaster { get; set; } // Propriedade para Master

    public TiposLixoConsultaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        _db = new SQLiteDatabaseHelper(dbPath);

        IsMaster = SQLiteDatabaseHelper.Sessao.IsMaster;
        BindingContext = this;
    }

    // Atualiza sempre que a tela volta a aparecer
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        todosTipos.Clear();
        TiposLixoListView.ItemsSource = null;
        BuscarEntry.Text = "";
    }
    private async void OnConsultarClicked(object sender, EventArgs e)
    {
        todosTipos = await _db.GetTiposLixosAsync();
        AtualizarLista(todosTipos);
        BuscarEntry.Text = ""; // limpa busca
    }

    private void OnConsultarTextChanged(object sender, TextChangedEventArgs e)
    {
        string termo = e.NewTextValue?.ToLower() ?? "";

        var filtrados = todosTipos.Where(t =>
            t.ID.ToString().Contains(termo) ||
            (t.Cor?.ToLower().Contains(termo) ?? false) ||
            (t.Material?.ToLower().Contains(termo) ?? false) ||
            (t.Origem?.ToLower().Contains(termo) ?? false) ||
            (t.OrigemDescricao?.ToLower().Contains(termo) ?? false) ||
            (t.DestinoAmbiental?.ToLower().Contains(termo) ?? false) ||
            (t.Exemplos?.ToLower().Contains(termo) ?? false)
        ).ToList();

        AtualizarLista(filtrados);
    }

    // Agora usamos TipoLixo diretamente e associamos imagem
    private void AtualizarLista(List<TipoLixo> lista)
    {
        foreach (var item in lista)
        {
            item.Imagem = ObterImagemPorMaterial(item.Material); // associa imagem
        }

        TiposLixoListView.ItemsSource = lista;
    }

    // Método para obter imagem com base no material
    private string ObterImagemPorMaterial(string material)
    {
        return material?.ToLower() switch
        {
            "plástico" => "plastico.png",
            "papel" => "papel.png",
            "vidro" => "vidro.png",
            "metal" => "metal.png",
            "orgânico" => "organico.png",
            "não reciclável" => "nao_reciclavel.png",
            "madeira" => "madeira.png",
            "resíduos perigosos" => "residuos_perigosos.png",
            "hospitalar" => "hospitalar.png",
            "radioativos" => "radioativos.png",
            _ => "padrao.png"
        };
    }
    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
    private async void OnRegistrarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new TiposLixoPage());
    }
}
