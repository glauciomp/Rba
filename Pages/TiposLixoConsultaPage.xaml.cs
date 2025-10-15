using Microsoft.Maui.Controls;
using Rba.Models;
using Rba.Helpers;
using System.Linq;
using System.IO;

namespace Rba.Pages;

public partial class TiposLixoConsultaPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _db;

    public bool IsMaster { get; set; } // Propriedade para Master

    public TiposLixoConsultaPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "rba.db3");

        _db = new SQLiteDatabaseHelper(dbPath);
        // Define se é Master
        IsMaster = SQLiteDatabaseHelper.Sessao.IsMaster;
        BindingContext = this;
    }

    protected async override void OnAppearing()
    {
        var tipos = await _db.GetTiposLixosAsync();

        var lista = tipos.Select(t => new
        {
            t.ID,
            t.Cor,
            t.Material,
            t.Origem,
            t.OrigemDescricao,
            t.DestinoAmbiental,
            t.Exemplos
        }).ToList();

        TiposLixoListView.ItemsSource = lista;
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
