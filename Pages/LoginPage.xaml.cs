using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;

namespace Rba.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            // Aqui vai a lógica de login
            string user = UserEntry.Text;
            string pass = PasswordEntry.Text;

            // Exemplo: se admin
            if (user == "admin" && pass == "123")
            {
                await Navigation.PushAsync(new HomePage("Administrador", "Master"));
                return;
            }

            // Para usuários normais, procurar no banco
            // await LoginDoUsuario(user, pass);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }

        private void OnExitClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
        }
    }
}
