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
            TipoUsuarioPicker.SelectedIndex = 0; // Normal por padrão

           string dbPath = Path.Combine(FileSystem.AppDataDirectory, "rba.db3");
           _db = new SQLiteDatabaseHelper(dbPath);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string nome = NomeEntry?.Text?.Trim() ?? string.Empty;
            string email = EmailEntry?.Text?.Trim() ?? string.Empty;
            string senha = SenhaEntry?.Text?.Trim() ?? string.Empty;
            string tipo = TipoUsuarioPicker?.SelectedItem?.ToString() ?? "Normal";

            // 🔹 Validação básica
            if (string.IsNullOrWhiteSpace(nome) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(senha))
            {
                await DisplayAlert("Erro", "Por favor, preencha todos os campos.", "OK");
                return;
            }

            // 🔹 Validação de e-mail
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("Erro", "Por favor, insira um email válido.", "OK");
                return;
            }

            // 🔹 Verifica se o usuário já existe
            var existente = await _db.ObterUsuarioPorNomeAsync(nome);
            if (existente != null)
            {
                await DisplayAlert("Erro", "Este nome de usuário já está em uso.", "OK");
                return;
            }

            // 🔹 Cria e salva o novo usuário
            var usuario = new Usuario
            {
                NomeUsuario = nome,
                Email = email,
                Senha = senha,
                TipoUsuario = tipo
            };

            await _db.InserirUsuarioAsync(usuario);
          

            await DisplayAlert("Sucesso", $"Usuário {nome} cadastrado como {tipo}.", "OK");

            // 🔹 Retorna ao login
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