using Microsoft.Maui.Controls;
using Rba.Models;
using Rba.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace Rba.Pages;

public partial class UsuariosPage : ContentPage
{
    private readonly SQLiteDatabaseHelper _db;
    private List<Usuario> todosUsuarios = new(); // Lista Completa para buscar
    public UsuariosPage()
    {
        InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        _db = new SQLiteDatabaseHelper(dbPath);
    }

    private async void OnConsultarClicked(object? sender, EventArgs? e)
    {
        todosUsuarios = await _db.GetUsuariosAsync(); // Atualiza a lista completa
        UsuariosListView.ItemsSource = todosUsuarios;
    }

    private void OnConsultarTextChanged(object sender, TextChangedEventArgs e)
    {
        string termo = e.NewTextValue?.ToLower() ?? "";

        var filtrados = todosUsuarios
            .Where(u => u.NomeUsuario.ToLower().Contains(termo) ||
                        u.Email.ToLower().Contains(termo) ||
                        u.TipoUsuario.ToLower().Contains(termo))
            .ToList();

        UsuariosListView.ItemsSource = filtrados;
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

                ConsultarEntry.Text = ""; // Limpa filtro
            }

            UsuariosListView.SelectedItem = null; // Limpa seleção
        });


    }

    // Mostrar caminho do banco
    /*
    private async void OnVerCaminhoClicked(object sender, EventArgs e)
    {
        string caminho = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        await DisplayAlert("Caminho do banco", caminho, "OK");
    }
    */

}