namespace NorthwindWPF;

public partial class MainWindow
{
    public MainWindow() => InitializeComponent();

    private void MainWindow_OnInitialized(object? sender, EventArgs e)
    {
        var db = new NorthwindContext();
        DataContext = new MainWindowViewModel().Init(db);
    }
}