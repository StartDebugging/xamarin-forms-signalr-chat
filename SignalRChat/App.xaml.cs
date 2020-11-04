using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;
using Xamarin.Forms;

namespace SignalRChat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<MainPageViewModel>();
            serviceCollection.AddScoped<MainPage>();

            Stream resourceStream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SignalRChat.appsettings.json");

            var configuration = new ConfigurationBuilder()
                .AddJsonStream(resourceStream)
                .Build();

            serviceCollection.AddSingleton<IConfiguration>(configuration);

            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            MainPage = serviceProvider.GetService<MainPage>();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
