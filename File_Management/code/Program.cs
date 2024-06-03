namespace File_Management;

// 委托方法类
public static class DelegateMethod
{
    // 委托方法定义
    public delegate void DelegateFunction();
}

// 应用程序类
internal static class Program
{
    // 主函数
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new Window.MainWindow());
    }
}