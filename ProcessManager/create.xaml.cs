using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ProcessManager
{
    public partial class MainWindow : Window
    {
        // 创建一个绑定的数据集合
        public ObservableCollection<ProcessModel> Processes { get; set; } = new ObservableCollection<ProcessModel>();

        public MainWindow()
        {
            InitializeComponent();
            // 将数据集合绑定到DataGrid
            ProcessDataGrid.ItemsSource = Processes;
        }

        // 创建进程
        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 从输入框获取数据
                var process = new ProcessModel
                {
                    ProcessID = int.Parse(ProcessIDTextBox.Text),
                    ProcessName = ProcessNameTextBox.Text,
                    ProcessDescription = ProcessDescriptionTextBox.Text,
                    ArrivalTime = double.Parse(ArrivalTimeTextBox.Text),
                    ServiceTime = double.Parse(ServiceTimeTextBox.Text)
                };

                // 添加到集合
                Processes.Add(process);

                // 清空输入框
                ClearInputFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}");
            }
        }

        // 删除选中的进程
        private void DeleteProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessDataGrid.SelectedItem is ProcessModel selectedProcess)
            {
                Processes.Remove(selectedProcess);
            }
            else
            {
                MessageBox.Show("请先选择要删除的进程！");
            }
        }

        // 删除所有进程
        private void ClearProcessesButton_Click(object sender, RoutedEventArgs e)
        {
            Processes.Clear();
        }

        // 清空输入框
        private void ClearInputFields()
        {
            ProcessIDTextBox.Text = "请输入进程ID";
            ProcessNameTextBox.Text = "请输入进程映像名称";
            ProcessDescriptionTextBox.Text = "请输入进程描述";
            ArrivalTimeTextBox.Text = "请输入到达时间";
            ServiceTimeTextBox.Text = "请输入服务时间";
        }

        // 处理输入框的占位符功能
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "请输入进程ID" ||
                                    textBox.Text == "请输入进程映像名称" ||
                                    textBox.Text == "请输入进程描述" ||
                                    textBox.Text == "请输入到达时间" ||
                                    textBox.Text == "请输入服务时间"))
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox.Name == "ProcessIDTextBox")
                    textBox.Text = "请输入进程ID";
                else if (textBox.Name == "ProcessNameTextBox")
                    textBox.Text = "请输入进程映像名称";
                else if (textBox.Name == "ProcessDescriptionTextBox")
                    textBox.Text = "请输入进程描述";
                else if (textBox.Name == "ArrivalTimeTextBox")
                    textBox.Text = "请输入到达时间";
                else if (textBox.Name == "ServiceTimeTextBox")
                    textBox.Text = "请输入服务时间";
            }
        }
    }
}
