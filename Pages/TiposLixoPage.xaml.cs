using Microsoft.Maui.Controls;
using System;

namespace Rba.Pages
{
    public partial class TiposLixoPage : ContentPage
    {
        public TiposLixoPage()
        {
            InitializeComponent();
        }

        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Registro", "Tipo de lixo registrado.", "OK");
        }

        private void OnLimparClicked(object sender, EventArgs e)
        {
            CorEntry.Text = string.Empty;
            MaterialEntry.Text = string.Empty;
            OrigemEntry.Text = string.Empty;
            OrigemDescrEntry.Text = string.Empty;
            DestinoEntry.Text = string.Empty;
            ExemplosEntry.Text = string.Empty;
        }

        private async void OnVoltarClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
