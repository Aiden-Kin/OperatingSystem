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
 
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 构造函数，初始化主窗口。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 点击"进程创建"菜单项时导航到 ProcessCreatePage 页面。
        /// </summary>
        private void MenuItem_ProcessCreate_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProcessCreatePage());
        }

        /// <summary>
        /// 点击"进程调度"菜单项时导航到 ProcessSchedulePage 页面。
        /// </summary>
        private void MenuItem_ProcessSchedule_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProcessSchedulePage());
        }

        /// <summary>
        /// 点击"进程同步"菜单项时导航到 ProcessSyncPage 页面。
        /// </summary>
        private void MenuItem_ProcessSync_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProcessSyncPage());
        }

        /// <summary>
        /// 点击"避免死锁"菜单项时导航到 AvoidDeadlockPage 页面。
        /// </summary>
        private void MenuItem_AvoidDeadlock_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AvoidDeadlockPage());
        }

        /// <summary>
        /// 点击"存储管理"菜单项时导航到 StorageManagementPage 页面。
        /// </summary>
        private void MenuItem_StorageManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new StorageManagementPage());
        }

        /// <summary>
        /// 点击"设备管理"菜单项时导航到 DeviceManagementPage 页面。
        /// </summary>
        private void MenuItem_DeviceManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DeviceManagementPage());
        }

        /// <summary>
        /// 点击"文件管理"菜单项时导航到 FileManagementPage 页面。
        /// </summary>
        private void MenuItem_FileManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FileManagementPage());
        }
    }
}
