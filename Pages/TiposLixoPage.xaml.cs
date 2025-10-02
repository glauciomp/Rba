using Microsoft.Maui.Controls;
using System;
using Rba.Models; 
using Rba.Helpers;
using System.Threading.Tasks;

namespace Rba.Pages
{
    public partial class TiposLixoPage : ContentPage
    {
        private readonly SQLiteDatabaseHelper _dbHelper;

        public TiposLixoPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "rba_database.db3");
            _dbHelper = new SQLiteDatabaseHelper(dbPath);
        }
        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            var tipoLixo = new TipoLixo
            {
                Cor = CorEntry.Text,
                Material = MaterialEntry.Text,
                Origem = OrigemEntry.Text,
                OrigemDescricao = OrigemDescrEntry.Text,
                DestinoAmbiental = DestinoEntry.Text,
                Exemplos = ExemplosEntry.Text
            };
            
            await _dbHelper.InserirTipoLixoAsync(tipoLixo);
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

        private async void OnConsultarTiposClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TiposLixoConsultaPage());
        }

    }
}
