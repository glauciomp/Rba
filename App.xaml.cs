using Microsoft.Maui.Controls;
using Rba.Helpers;

namespace Rba;

public partial class App : Application
{
   
    public App()
    {
        InitializeComponent();
        // Inicializa o banco de dados
        string dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "rba.db3");
        Db = new SQLiteDatabaseHelper(dbPath);
        MainPage = new NavigationPage(new Pages.LoginPage());
    }
    
    public static SQLiteDatabaseHelper Db { get; private set; }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);

        // Simulando tamanho de celular Galaxy S21
        window.Width = 560; // largura típica do galaxy S21 é 360 - ajustado para ficar um pouco mais largo
        window.Height = 800; // altura típica do galaxy s21 é 800

        // modelo Iphone 12 Largura 390 e altura 844
        // modelo Pixel 6 Largura 412 e altura 915

        return window;
    }
}
