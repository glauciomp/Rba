using Microsoft.Maui.Controls;
using System;

namespace Rba.Pages
{
    public partial class HomePage : ContentPage
    {
        public HomePage(string nomeUsuario, string tipoUsuario)
        {
            InitializeComponent();
            lblBoasVindas.Text = $"Bem-vindo, {nomeUsuario}!";
            lblTipoUsuario.Text = $"Tipo de usuário: {tipoUsuario}";
        }

        private async void OnTiposLixoClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TiposLixoConsultaPage());
        }

        private async void OnMapaClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConsultaPontosColetaPage());
        }

       /* private async void OnDesafiosClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Desafios", "Funcionalidade de desafios ainda não implementada.", "OK");
        } */

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
