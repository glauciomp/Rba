using Microsoft.Maui.Controls;

namespace Rba;

public partial class App : Application
{
    [Obsolete]
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new Pages.LoginPage());
    }

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
