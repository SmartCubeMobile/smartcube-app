namespace SmartCubeMobile;

public partial class WelcomePage : ContentPage
{
    public WelcomePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(200);

        _ = CubeLogo.FadeTo(1, 800, Easing.CubicOut);
        _ = CubeLogo.ScaleTo(1, 900, Easing.SpringOut);
        CubeLogo.Scale = 0.6;

        await Task.Delay(300);
        _ = AppTitle.FadeTo(1, 600, Easing.CubicOut);
        _ = AppTitle.TranslateTo(0, 0, 600, Easing.CubicOut);
        AppTitle.TranslationY = 20;

        await Task.Delay(200);
        _ = Tagline.FadeTo(1, 500, Easing.CubicOut);

        await Task.Delay(300);
        _ = EnterBtn.FadeTo(1, 500, Easing.CubicOut);
        _ = EnterBtn.ScaleTo(1, 500, Easing.CubicOut);
        EnterBtn.Scale = 0.8;

        await Task.Delay(200);
        _ = CreatedBy.FadeTo(1, 500, Easing.CubicOut);
        _ = CompanyName.FadeTo(1, 600, Easing.CubicOut);
    }

    private async void OnEnterClicked(object sender, EventArgs e)
    {
        await EnterBtn.ScaleTo(0.92, 80, Easing.CubicOut);
        await EnterBtn.ScaleTo(1.0, 80, Easing.CubicOut);

        _ = CubeLogo.FadeTo(0, 300);
        _ = AppTitle.FadeTo(0, 300);
        _ = Tagline.FadeTo(0, 300);
        _ = EnterBtn.FadeTo(0, 300);
        _ = CreatedBy.FadeTo(0, 300);
        _ = CompanyName.FadeTo(0, 300);

        await Task.Delay(350);

        var dashboard = new SmartCubeDashboard();
        Navigation.InsertPageBefore(dashboard, this);
        await Navigation.PopAsync(false);
    }
}
