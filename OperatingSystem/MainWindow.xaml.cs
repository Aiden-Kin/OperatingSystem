using Equipment;
using OperatingSystem.Progress;
using OperatingSystem.store;
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
        //private void OpenCreateProcessPage(object sender, RoutedEventArgs e)
        //{
        //    // 创建 create.xaml 窗口实例
        //    create createWindow = new create();
        //    createWindow.Show(); // 显示 create.xaml 窗口
        //    this.Close(); // 关闭当前窗口（如果不想关闭主窗口，可以删除这一行）
        //}


        /// <summary>
        /// 点击"进程创建"菜单项时导航到 ProcessCreatePage 页面。
        /// </summary>
        private void MenuItem_ProcessCreate_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CreatePage());
        }

        /// <summary>
        /// 点击"进程调度"菜单项时导航到 ProcessSchedulePage 页面。
        /// </summary>
        private void MenuItem_ProcessSchedule_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Dispatch());
        }

       
        private void OpenSynchronization_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Synchronization());
        }

        /// <summary>
        /// 点击"避免死锁"菜单项时导航到 AvoidDeadlockPage 页面。
        /// </summary>
        private void MenuItem_AvoidDeadlock_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AvoidDeadlock());
        }


        private void MenuItem_StorageManagement_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Storage());
        }


       private void MenuItem_equipment_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DiskSchedulerPage());
        }

        ///// <summary>
        ///// 点击"文件管理"菜单项时导航到 FileManagementPage 页面。
        ///// </summary>
        //private void MenuItem_FileManagement_Click(object sender, RoutedEventArgs e)
        //{
        //    MainFrame.Navigate(new FileManagementPage());
        //}
    }
}