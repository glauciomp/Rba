using Microsoft.Maui.Controls;
using Rba.Models;
using Rba.Helpers;
using System.Linq;

namespace Rba.Pages;

public partial class UsuariosPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _db;
    public UsuariosPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "rba_database.db3");

        _db = new SQLiteDatabaseHelper(dbPath);

    }

    private async void OnConsultarClicked(object sender, EventArgs e)
    {
        var usuarios = await _db.GetUsuariosAsync();

        var lista = usuarios.Select(u => new
        {
            u.ID,
            u.NomeUsuario,
            u.Email,
            u.TipoUsuario
        }).ToList();

        UsuariosListView.ItemsSource = lista;
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}