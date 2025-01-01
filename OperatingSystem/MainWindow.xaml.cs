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
            MainFrame.Navigate(new DefaultPage());

        }


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

        private void FileManage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new FileManage());
        }


        private void MenuItem_equipment_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DiskSchedulerPage());
        }
    }
}