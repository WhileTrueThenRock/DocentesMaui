namespace EFDocenteMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            App.Current.UserAppTheme = AppTheme.Light;
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF5cWWJCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH1ceXVURGNYU0JyWkY=");

        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 1250;
            const int newHeight = 750;

            window.Width = newWidth;
            window.Height = newHeight;

            return window;
        }
    }
}
