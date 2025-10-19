using Rba.Helpers;
using Rba.Models;

namespace Rba.Pages;

public partial class EditarPontoColeta : ContentPage
{
    private SQLiteDatabaseHelper db;
    private PontoColeta ponto;

    public EditarPontoColeta(PontoColeta pontoSelecionado)
    {
        InitializeComponent();

        ponto = pontoSelecionado;

        string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");

        db = new SQLiteDatabaseHelper(dbPath);

        nomeEntry.Text = ponto.Nome;
        enderecoEntry.Text = ponto.Endereco;
        tipoEntry.Text = ponto.TipoLixo;
        contatoEntry.Text = ponto.Contato;

        if (!string.IsNullOrEmpty(ponto.Horario))
        {
            var partes = ponto.Horario.Split(" - ");
            if (partes.Length == 2)
            {
                inicioPicker.Time = TimeSpan.Parse(partes[0]);
                fimPicker.Time = TimeSpan.Parse(partes[1]);
            }
        }
    }

    private async void OnSalvarClicked(object sender, EventArgs e)
    {
        try
        {
            var inicio = inicioPicker.Time;
            var fim = fimPicker.Time;

            if (fim <= inicio)
            {
                await DisplayAlert("Erro", "O horário de término deve ser maior que o de início.", "OK");
                return;
            }

            ponto.Nome = nomeEntry.Text;
            ponto.Endereco = enderecoEntry.Text;
            ponto.TipoLixo = tipoEntry.Text;
            ponto.Contato = contatoEntry.Text;
            ponto.Horario = $"{inicio:hh\\:mm} - {fim:hh\\:mm}";

            await db.Update(ponto);

            await DisplayAlert("Sucesso", "Ponto atualizado com sucesso!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", ex.Message, "OK");
        }
    }

    private async void OnCancelarClicked(object sender, EventArgs e)
    {
        bool cancelar = await DisplayAlert("Cancelar edição", "Deseja sair sem salvar?", "Sim", "Não");
        if (cancelar)
            await Navigation.PopAsync();
    }
}
