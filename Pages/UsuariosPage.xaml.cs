using Microsoft.Maui.Controls;
using Rba.Models;
using Rba.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Rba.Pages;

public partial class UsuariosPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _db;
    public UsuariosPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        _db = new SQLiteDatabaseHelper(dbPath);

    }

    private async void OnConsultarClicked(object? sender, EventArgs? e)
    {
        var usuarios = await _db.GetUsuariosAsync();

        UsuariosListView.ItemsSource = usuarios;

        
    }

    private async void OnVoltarClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    private void OnUsuarioSelecionado(object sender, SelectionChangedEventArgs e)
    {
        // CA1826: Use indexador ao invés de FirstOrDefault
        if (e.CurrentSelection.Count == 0)
            return;

        var usuarioSelecionado = e.CurrentSelection[0] as Usuario;
        if (usuarioSelecionado == null)
            return;

        int id = usuarioSelecionado.ID;

        // Confirma exclusão
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool confirmar = await DisplayAlert("Excluir Usuário",
                $"Deseja realmente excluir o usuário com ID {id}?",
                "Sim", "Cancelar");

            if (confirmar)
            {
                await _db.DeleteUsuarioAsync(id);
                await DisplayAlert("Sucesso", $"Usuário {id} excluído com sucesso.", "OK");
                OnConsultarClicked(null, null); // Atualiza a lista
            }

            UsuariosListView.SelectedItem = null; // Limpa seleção
        });
    }

}