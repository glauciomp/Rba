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
        private ObservableCollection<TipoLixo> tipoLixo = new ();
        private List<TipoLixo> todosTipos = new(); // Lista completa para busca

        public TiposLixoPage()
        {
            InitializeComponent();

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
            _dbHelper = new SQLiteDatabaseHelper(dbPath);
            TiposLixoListView.ItemsSource = tipoLixo;

        }
        /* // pr� carrega o que j� tem cadastrado 
         protected override async void OnAppearing()
         {
             base.OnAppearing();

             var dados = await _dbHelper.GetTiposLixosAsync();
             tipoLixo.Clear();

             foreach (var item in dados)
                 tipoLixo.Add(item);
         }*/

        private async void OnConsultarClicked(object sender, EventArgs e)
        {
            todosTipos = await _dbHelper.GetTiposLixosAsync();
            AtualizarLista(todosTipos);
            BuscarEntry.Text = ""; // limpa busca
        }

        private void OnBuscarTextChanged(object sender, TextChangedEventArgs e)
        {
            string termo = e.NewTextValue?.ToLower() ?? "";

            var filtrados = todosTipos.Where(t =>
                t.ID.ToString().Contains(termo) ||
                (t.Cor?.ToLower().Contains(termo) ?? false) ||
                (t.Material?.ToLower().Contains(termo) ?? false) ||
                (t.Origem?.ToLower().Contains(termo) ?? false) ||
                (t.OrigemDescricao?.ToLower().Contains(termo) ?? false) ||
                (t.DestinoAmbiental?.ToLower().Contains(termo) ?? false) ||
                (t.Exemplos?.ToLower().Contains(termo) ?? false)
            ).ToList();

            AtualizarLista(filtrados);
        }

        private void AtualizarLista(List<TipoLixo> lista)
        {
            tipoLixo.Clear();
            foreach (var item in lista)
                tipoLixo.Add(item);
        }
        private async void OnRegistrarClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CorEntry.Text) ||
                string.IsNullOrWhiteSpace(MaterialEntry.Text) ||
                string.IsNullOrWhiteSpace(OrigemEntry.Text) ||
                string.IsNullOrWhiteSpace(OrigemDescrEntry.Text) ||
                string.IsNullOrWhiteSpace(DestinoEntry.Text) ||
                string.IsNullOrWhiteSpace(ExemplosEntry.Text))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }
            var novoTipo = new TipoLixo
            {
                Cor = CorEntry.Text,
                Material = MaterialEntry.Text,
                Origem = OrigemEntry.Text,
                OrigemDescricao = OrigemDescrEntry.Text,
                DestinoAmbiental = DestinoEntry.Text,
                Exemplos = ExemplosEntry.Text
            };
            
            await _dbHelper.InserirTipoLixoAsync(novoTipo);
            await DisplayAlert("Registro", "Tipo de lixo registrado.", "OK");

            // Limpar campos
            CorEntry.Text = string.Empty;
            MaterialEntry.Text = string.Empty;
            OrigemEntry.Text = string.Empty;
            OrigemDescrEntry.Text = string.Empty;
            DestinoEntry.Text = string.Empty;
            ExemplosEntry.Text = string.Empty;

            // Atualizar lista
            var dados = await _dbHelper.GetTiposLixosAsync();
            tipoLixo.Clear();
            foreach (var item in dados)
                tipoLixo.Add(item);

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
            if (sender is Button btn && btn.BindingContext is TipoLixo itemSelecionado)
            {
                bool confirmar = await DisplayAlert("Tem certeza?", $"Remover{itemSelecionado.ID}?", "Sim", "N�o");
                if (confirmar)
                {
                    await _dbHelper.DeleteTipoLixo(itemSelecionado.ID);

                    // Atualizar lista ap�s exclus�o
                    var dados = await _dbHelper.GetTiposLixosAsync();
                    tipoLixo.Clear();
                    foreach (var item in dados)
                        tipoLixo.Add(item);

                }
            }
        }
    }
}
