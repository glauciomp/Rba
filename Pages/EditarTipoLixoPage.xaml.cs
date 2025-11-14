using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class EditarTipoLixoPage : ContentPage
{
    private TipoLixo _tipoLixo;
    private SQLiteDatabaseHelper _dbHelper;

    public EditarTipoLixoPage(TipoLixo tipoLixo)
    {
        InitializeComponent();
        _tipoLixo = tipoLixo;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
        _dbHelper = new SQLiteDatabaseHelper(dbPath);

        // Preencher campos com dados do objeto
        CorEntry.Text = _tipoLixo.Cor;
        OrigemEntry.Text = _tipoLixo.Origem;
        OrigemDescrEntry.Text = _tipoLixo.OrigemDescricao;
        ExemplosEntry.Text = _tipoLixo.Exemplos;

        // Seleciona valores atuais nos Pickers
        MaterialPicker.SelectedItem = _tipoLixo.Material;
        DestinoPicker.SelectedItem = _tipoLixo.DestinoAmbiental;
    }

    private async void Button_Salvar(object sender, EventArgs e)
    {
        // Atualizar objeto com novos valores
        _tipoLixo.Cor = CorEntry.Text;
        _tipoLixo.Material = MaterialPicker.SelectedItem?.ToString() ?? string.Empty;
        _tipoLixo.Origem = OrigemEntry.Text;
        _tipoLixo.OrigemDescricao = OrigemDescrEntry.Text;
        _tipoLixo.DestinoAmbiental = DestinoPicker.SelectedItem?.ToString() ?? string.Empty;
        _tipoLixo.Exemplos = ExemplosEntry.Text;

        // Atualiza imagem se material foi alterado
        _tipoLixo.Imagem = ObterImagemPorMaterial(_tipoLixo.Material);

        // Atualizar no banco
        await _dbHelper.UpdateTl(_tipoLixo);

        await DisplayAlert("Sucesso!", "Tipo de lixo atualizado.", "OK");
        await Navigation.PopAsync(); // Volta para a página anterior
    }

    private async void Button_Cancelar(object sender, EventArgs e)
    {
        bool cancelar = await DisplayAlert("Cancelar edições?", "Deseja sair sem salvar?", "Sim", "Não");
        if (cancelar)
            await Navigation.PopAsync();
    }

    // Reutiliza o mesmo método de imagem
    private string ObterImagemPorMaterial(string material)
    {
        return material?.ToLower() switch
        {
            "plástico" => "plastico.png",
            "papel" => "papel.png",
            "vidro" => "vidro.png",
            "metal" => "metal.png",
            "orgânico" => "organico.png",
            "não recicláveis" => "nao_reciclavel.png",
            "madeira" => "madeira.png",
            "resíduos perigosos" => "residuos_perigosos.png",
            "hospitalar" => "hospitalar.png",
            "radioativos" => "radioativos.png",
            _ => "padrao.png"
        };
    }
}
