using Rba.Helpers;
using Rba.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Rba.Pages;

public partial class ConsultaPontosColetaPage : ContentPage
{
    private ObservableCollection<PontoColeta> lista = new();
    private List<PontoColeta> todosPontos = new();
    private SQLiteDatabaseHelper db;
   
    public bool IsMaster { get; set; }// Propriedade para Master

    public ConsultaPontosColetaPage()
	{
		InitializeComponent();

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        db = new SQLiteDatabaseHelper(dbPath);

        listaView.ItemsSource = lista;

        // Define se é Master
        IsMaster = SQLiteDatabaseHelper.Sessao.IsMaster;
        BindingContext = this;
    }

    private async void Button_Consultar(object sender, EventArgs e)
    {
        try
        {
            todosPontos = await db.GetAll();
            AtualizarLista(todosPontos);
            BuscarEntry.Text = ""; // limpa busca
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro ao consultar", ex.Message, "Ok");
        }
    }

    private void OnBuscarTextChanged(object sender, TextChangedEventArgs e)
    {
        string termo = e.NewTextValue?.ToLower() ?? "";

        var filtrados = todosPontos.Where(p =>
            (p.Nome?.ToLower().Contains(termo) ?? false) ||
            (p.Endereco?.ToLower().Contains(termo) ?? false) ||
            (p.TipoLixo?.ToLower().Contains(termo) ?? false) ||
            (p.Contato?.ToLower().Contains(termo) ?? false) ||
            (p.Horario?.ToLower().Contains(termo) ?? false)
            ).ToList();

        AtualizarLista(filtrados);
    }

    private void AtualizarLista(List<PontoColeta> dados)
    {
        lista.Clear();
        foreach (var item in dados)
            lista.Add(item);
        listaView.ItemsSource = lista;
    }

    private async void Button_Registrar(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PontosColetaPage());
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    }

    /* Esta é a versão antiga, apagar depois de validar
    protected async override void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            lista.Clear();
            var dados = await db.GetAll();
            foreach (var item in dados)
                lista.Add(item);
            
        }
        catch (Exception ex)
        {
            await DisplayAlert("Errro ao iniciar lista", ex.Message, "OK");
        }
      
    }

    private async void Button_Registrar(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PontosColetaPage());
    }

    private async void Button_Voltar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}*/