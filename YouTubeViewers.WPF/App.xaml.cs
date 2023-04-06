using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using YouTubeViewers.Domain.Commands;
using YouTubeViewers.Domain.Queries;
using YouTubeViewers.EntityFramework;
using YouTubeViewers.EntityFramework.Commands;
using YouTubeViewers.EntityFramework.Queries;
using YouTubeViewers.WPF.Stores;
using YouTubeViewers.WPF.ViewModels;
using YouTubeViewers.WPF.HostBuilders;

namespace YouTubeViewers.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // After we've built the host below, we dont need these fields anymore

        //private readonly ModalNavigationStore _modalNavigationStore;
        //private readonly YouTubeViewersDbContextFactory _youTubeViewersDbContextFactory;
        //private readonly IGetAllYouTubeViewersQuery _getAllYouTubeViewersQuery;
        //private readonly ICreateYouTubeViewerCommand _createYouTubeViewerCommand;
        //private readonly IUpdateYouTubeViewerCommand _updateYouTubeViewerCommand;
        //private readonly IDeleteYouTubeViewerCommand _deleteYouTubeViewerCommand;
        //private readonly YouTubeViewersStore _youTubeViewersStore;
        //private readonly SelectedYouTubeViewerStore _selectedYouTubeViewerStore;

        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .AddDbContext()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton<IGetAllYouTubeViewersQuery, GetAllYouTubeViewersQuery>();
                    services.AddSingleton<ICreateYouTubeViewerCommand, CreateYouTubeViewerCommand>();
                    services.AddSingleton<IUpdateYouTubeViewerCommand, UpdateYouTubeViewerCommand>();
                    services.AddSingleton<IDeleteYouTubeViewerCommand, DeleteYouTubeViewerCommand>();

                    services.AddSingleton<ModalNavigationStore>();
                    services.AddSingleton<YouTubeViewersStore>();
                    services.AddSingleton<SelectedYouTubeViewerStore>();

                    services.AddTransient<YouTubeViewersViewModel>(CreateYouTubeViewersViewModel);
                    services.AddSingleton<MainViewModel>();

                    services.AddSingleton<MainWindow>((services) => new MainWindow()
                    {
                        DataContext = services.GetRequiredService<MainViewModel>()
                    });
                })
                .Build();

            // The code below is replaced with the code above (the code above is cleaner)

            //string connectionString = "Data Source=YouTubeViewers.db";

            //_modalNavigationStore = new ModalNavigationStore();
            //_youTubeViewersDbContextFactory = new YouTubeViewersDbContextFactory(
            //    new DbContextOptionsBuilder().UseSqlite(connectionString).Options);

            //_getAllYouTubeViewersQuery = new GetAllYouTubeViewersQuery(_youTubeViewersDbContextFactory);
            //_createYouTubeViewerCommand = new CreateYouTubeViewerCommand(_youTubeViewersDbContextFactory);
            //_updateYouTubeViewerCommand = new UpdateYouTubeViewerCommand(_youTubeViewersDbContextFactory);
            //_deleteYouTubeViewerCommand = new DeleteYouTubeViewerCommand(_youTubeViewersDbContextFactory);
            //_youTubeViewersStore = new YouTubeViewersStore(_getAllYouTubeViewersQuery, _createYouTubeViewerCommand, _updateYouTubeViewerCommand, _deleteYouTubeViewerCommand);
            //_selectedYouTubeViewerStore = new SelectedYouTubeViewerStore(_youTubeViewersStore);
        }

        // This was the startup method before adding the host

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    using (YouTubeViewersDbContext context = _youTubeViewersDbContextFactory.Create())
        //    {
        //        context.Database.Migrate();
        //    }

        //        YouTubeViewersViewModel youTubeViewersViewModel = YouTubeViewersViewModel.LoadViewModel(_youTubeViewersStore, _selectedYouTubeViewerStore, _modalNavigationStore);

        //    MainWindow = new MainWindow()
        //    {
        //        DataContext = new MainViewModel(_modalNavigationStore, youTubeViewersViewModel)
        //    };
        //    MainWindow.Show();

        //    base.OnStartup(e);
        //}

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();

            YouTubeViewersDbContextFactory youTubeViewersDbContextFactory = _host.Services.GetRequiredService<YouTubeViewersDbContextFactory>();

            using(YouTubeViewersDbContext context = youTubeViewersDbContextFactory.Create())
            {
                context.Database.Migrate();
            }
            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.StopAsync();
            _host.Dispose();

            base.OnExit(e);
        }
        private YouTubeViewersViewModel CreateYouTubeViewersViewModel(IServiceProvider services)
        {
            return YouTubeViewersViewModel.LoadViewModel(
                services.GetRequiredService<YouTubeViewersStore>(),
                services.GetRequiredService<SelectedYouTubeViewerStore>(),
                services.GetRequiredService<ModalNavigationStore>());
        }
    }
}
