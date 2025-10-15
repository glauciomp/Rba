using Rba.Helpers;
using Rba.Models;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;

namespace Rba.Pages
{
    public partial class TiposLixoPage : ContentPage
    {
        private readonly SQLiteDatabaseHelper _dbHelper;
        private ObservableCollection<TipoLixo> tipoLixo = new ObservableCollection<TipoLixo>();

        public TiposLixoPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "rba.db3");
            _dbHelper = new SQLiteDatabaseHelper(dbPath);
            TiposLixoListView.ItemsSource = tipoLixo;

        }
       protected override async void OnAppearing()
        {
            base.OnAppearing();

            var dados = await _dbHelper.GetTiposLixosAsync();
            tipoLixo.Clear();

            foreach (var item in dados)
                tipoLixo.Add(item);
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

        private async void Button_Editar(object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                if (btn?.BindingContext is TipoLixo tipoLixo )
                {
                    await Navigation.PushAsync(new EditarTipoLixoPage(tipoLixo));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro ao Editar", ex.Message, "OK");
            }
        }

        private async void Button_Excluir(object sender, EventArgs e)
        {
            if (sender is Button btn && btn.BindingContext is TipoLixo tipoLixo)
            {
                bool confirmar = await DisplayAlert("Tem certeza?", $"Remover{tipoLixo.ID}?", "Sim", "Não");
                if (confirmar)
                {
                    await _dbHelper.DeleteTipoLixo(tipoLixo.ID);
                   
                    
                }
            }
        }
    }
}
