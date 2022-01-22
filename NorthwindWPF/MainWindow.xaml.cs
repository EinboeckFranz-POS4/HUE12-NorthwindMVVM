using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NorthwindWPF;

public partial class MainWindow
{
    public MainWindow() => InitializeComponent();

    private void MainWindow_OnInitialized(object? sender, EventArgs e)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<NorthwindContext>()
            .UseSqlServer(config.GetConnectionString(nameof(NorthwindDb)));
        
        var db = new NorthwindContext(optionsBuilder.Options);
        DataContext = new MainWindowViewModel().Init(db);
    }
}