using Microsoft.Maui.Controls;
using Rba.Helpers;
using Rba.Models;
using System;
using System.Threading.Tasks;

namespace Rba.Pages
{
    public partial class LoginPage : ContentPage
    {
        private readonly SQLiteDatabaseHelper _db;
        public LoginPage()
        {
            InitializeComponent();
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
            _db = new SQLiteDatabaseHelper(dbPath);
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Aqui vai a lógica de login
            string user = UserEntry.Text?.Trim();
            string pass = PasswordEntry.Text?.Trim();

            // Exemplo: se admin
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                await DisplayAlert("Atenção", "Por favor, preencha usuário e senha.", "OK");
                return;
            }

            // Verifica se é o admin fixo
            if (user == "admin" && pass == "123")
            {
                var admin = new Usuario
                {
                    NomeUsuario = "Administrador",
                    TipoUsuario = "Master",
                    Email = "admin@rba.com"
                };

                SQLiteDatabaseHelper.Sessao.IniciarSessao(admin);
                await Navigation.PushAsync(new HomePage(admin.NomeUsuario, admin.TipoUsuario));
                return;
            }

            // Verifica no banco
            var usuario = await _db.ObterUsuarioPorNomeAsync(user);
            if (usuario == null)
            {
                await DisplayAlert("Erro", "Usuário não encontrado.", "OK");
                return;
            }

            if (usuario.Senha != pass)
            {
                await DisplayAlert("Erro", "Senha incorreta.", "OK");
                return;
            }

            // Inicia sessão e vai para a HomePage
            SQLiteDatabaseHelper.Sessao.IniciarSessao(usuario);
            await Navigation.PushAsync(new HomePage(usuario.NomeUsuario, usuario.TipoUsuario));

        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }
    }
}
