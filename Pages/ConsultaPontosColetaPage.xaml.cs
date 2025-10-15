
using Rba.Helpers;
using Rba.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Rba.Pages;

public partial class ConsultaPontosColetaPage : ContentPage
{
	ObservableCollection<PontoColeta> lista = new ObservableCollection<PontoColeta>();
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
}