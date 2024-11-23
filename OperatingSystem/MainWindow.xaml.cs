using OperatingSystem.Progress;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace OperatingSystem
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void OpenCreateProcessPage(object sender, RoutedEventArgs e)
        {
            // 创建 create.xaml 窗口实例
            create createWindow = new create();
            createWindow.Show(); // 显示 create.xaml 窗口
            this.Close(); // 关闭当前窗口（如果不想关闭主窗口，可以删除这一行）
        }
    }
}