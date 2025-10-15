using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class EditarTipoLixoPage : ContentPage
{
    private TipoLixo _tipoLixo;
    private SQLiteDatabaseHelper _dbHelper;
	public EditarTipoLixoPage( TipoLixo tipoLixo)
	{
		InitializeComponent();
        _tipoLixo = tipoLixo;

        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "rba.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        // Preencher campos com dados do objeto
        CorEntry.Text = _tipoLixo.Cor;
        MaterialEntry.Text = _tipoLixo.Material;
        OrigemEntry.Text = _tipoLixo.Origem;
        OrigemDescrEntry.Text = _tipoLixo.OrigemDescricao;
        DestinoEntry.Text = _tipoLixo.DestinoAmbiental;
        ExemplosEntry.Text = _tipoLixo.Exemplos;

    }

    private async void Button_Salvar(object sender, EventArgs e)
    {
        // Atualizar objeto com novos valores
        _tipoLixo.Cor = CorEntry.Text;
        _tipoLixo.Material = MaterialEntry.Text;
        _tipoLixo.Origem = OrigemEntry.Text;
        _tipoLixo.OrigemDescricao = OrigemDescrEntry.Text;
        _tipoLixo.DestinoAmbiental = DestinoEntry.Text;
        _tipoLixo.Exemplos = ExemplosEntry.Text;

        // Atualizar
        await _dbHelper.UpdateTl(_tipoLixo);

        await DisplayAlert("Sucesso", "Tipo de lixo atualizado.", "OK");
        await Navigation.PopAsync(); // Volta para a página anterior
    }

    private async void Button_Cancelar(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}