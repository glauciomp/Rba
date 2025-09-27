using Microsoft.Maui.Controls;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Rba.Models;
using Rba.Helpers;

namespace Rba.Pages
{
    public partial class RegisterPage : ContentPage
    {
        private readonly SQLiteDatabaseHelper _db;
        public RegisterPage()
        {
            InitializeComponent();
            TipoUsuarioPicker.SelectedIndex = 0; // Normal por padr�o

            string dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "rba_database.db3");

            _db = new SQLiteDatabaseHelper(dbPath);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {

            // Trim para remover espa�os extras
            //IsNullOrWhiteSpace para verificar se est� vazio ou s� com espa�os
            // Operador null-coalescing (??) para garantir que n�o seja nulo
            // ? para acessar Text com seguran�a;
            // ?? string.Empty para garantir que n�o seja nulo
            string nome = NomeEntry?.Text?.Trim() ?? string.Empty;
            string email = EmailEntry?.Text?.Trim()?? string.Empty;
            string senha = SenhaEntry?.Text?.Trim()?? string.Empty;
            string tipo = TipoUsuarioPicker?.SelectedItem?.ToString() ?? "Normal";

            // Valida��o de campos obrigat�rios
            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))

            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }
            // Regex para validar email (ex: nomeDdominio.com)
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Erro", "Por favor, insira um email v�lido.", "OK");
                return;
            }
            
            var usuario = new Usuario
            {
                NomeUsuario = nome,
                Email = email,
                Senha = senha,
                TipoUsuario = tipo
            };
            await _db.InserirUsuarioAsync(usuario);
            await DisplayAlert("Sucesso", $"Usu�rio {nome} cadastrado como {tipo}.", "OK");
            await Navigation.PopAsync();
        }

        private async void OnExitClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

                
        private async void OnUsuariosClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UsuariosPage()); 
        }
    }
}
