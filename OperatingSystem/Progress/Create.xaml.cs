using System;
using System.Windows;
using System.Windows.Controls;

namespace OperatingSystem.Progress
{
    public partial class create : Window
    {
        public create()
        {
            InitializeComponent();

            // 绑定全局数据到 DataGrid
            ProcessDataGrid.ItemsSource = App.Processes;
        }

        // 创建进程
        private void CreateProcessButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 验证输入并添加进程
                if (int.TryParse(ProcessIDTextBox.Text, out int processID) &&
                    double.TryParse(ArrivalTimeTextBox.Text, out double arrivalTime) &&
                    double.TryParse(ServiceTimeTextBox.Text, out double serviceTime))
                {
                    var process = new ProcessModel
                    {
                        ProcessID = processID,
                        ProcessName = ProcessNameTextBox.Text,
                        ProcessDescription = ProcessDescriptionTextBox.Text,
                        ArrivalTime = arrivalTime,
                        ServiceTime = serviceTime,
                        RemainingTime = serviceTime, // 初始剩余时间等于服务时间
                        CompletionTime = null // 表示完成时间未设置
                    };

                    App.Processes.Add(process); // 添加到全局数据
                    ClearInputFields();
                }
                else
                {
                    MessageBox.Show("请输入有效的数字！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"发生错误: {ex.Message}");
            }
        }

        // 删除选中进程
        private void DeleteProcessButton_Click(object sender, RoutedEventArgs e)
        {
            if (ProcessDataGrid.SelectedItem is ProcessModel selectedProcess)
            {
                App.Processes.Remove(selectedProcess); // 从全局数据中删除
            }
            else
            {
                MessageBox.Show("请先选择要删除的进程！");
            }
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            // 返回主页面
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Hide(); // 隐藏当前窗口以保留状态
        }

        // 清空全局进程列表
        private void ClearProcessesButton_Click(object sender, RoutedEventArgs e)
        {
            App.Processes.Clear();
        }

        // 清空输入框
        private void ClearInputFields()
        {
            foreach (var control in new TextBox[] { ProcessIDTextBox, ProcessNameTextBox, ProcessDescriptionTextBox, ArrivalTimeTextBox, ServiceTimeTextBox })
            {
                control.Text = control.Tag.ToString();
                control.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
        }

        // 输入框获取焦点
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Tag != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            }
        }

        // 输入框失去焦点
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Tag != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
                textBox.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
        }
    }
}
