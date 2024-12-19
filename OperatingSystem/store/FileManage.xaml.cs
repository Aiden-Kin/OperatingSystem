using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace OperatingSystem.store
{
    /// <summary>
    /// FileManage.xaml 的交互逻辑
    /// </summary>
    public partial class FileManage : Page
    {
        private List<MFD> MFDList = new List<MFD>(); // 主文件目录
        private List<UFD> CurrentUserUFD = new List<UFD>(); // 当前用户的用户文件目录
        private List<UOF> UOFList = new List<UOF>(); // 已打开文件表
        private DiskBlock[] Disk = new DiskBlock[100]; // 模拟磁盘

        private string CurrentUsername = string.Empty; // 当前登录用户

        public FileManage()
        {
            InitializeComponent();
            InitializeDisk();
            UpdateGrids();
        }

        // 初始化磁盘
        private void InitializeDisk()
        {
            for (int i = 0; i < Disk.Length; i++)
            {
                Disk[i] = new DiskBlock { BlockId = i, IsOccupied = false, Data = string.Empty };
            }
        }

        // 更新表格显示
        private void UpdateGrids()
        {
            MFDGrid.ItemsSource = null;
            MFDGrid.ItemsSource = MFDList;

            UFDGrid.ItemsSource = null;
            UFDGrid.ItemsSource = CurrentUserUFD;

            UOFGrid.ItemsSource = null;
            UOFGrid.ItemsSource = UOFList;

            DiskGrid.ItemsSource = null;
            DiskGrid.ItemsSource = Disk;
        }

        // 查找空闲块
        private DiskBlock GetFreeBlock()
        {
            return Disk.FirstOrDefault(block => !block.IsOccupied);
        }

        // 注册按钮点击事件
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("用户名和密码不能为空！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MFDList.Any(u => u.Username == username))
            {
                MessageBox.Show("用户名已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var freeBlock = GetFreeBlock();
            if (freeBlock == null)
            {
                MessageBox.Show("磁盘已满，无法分配！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            freeBlock.IsOccupied = true;
            MFDList.Add(new MFD { Username = username, Password = password, UFDAddress = freeBlock.BlockId });

            MessageBox.Show("注册成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateGrids();
        }


        // 登录按钮点击事件
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            var user = MFDList.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                MessageBox.Show("用户名或密码错误！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentUsername = username;
            CurrentUserUFD = LoadUFD(user.UFDAddress);

            MessageBox.Show($"欢迎 {username}！", "登录成功", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateGrids();
        }

        // 退出按钮点击事件
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            CurrentUsername = string.Empty;
            CurrentUserUFD.Clear();
            UOFList.Clear();

            MessageBox.Show("已退出登录。", "退出", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateGrids();
        }

        // 加载用户文件目录
        private List<UFD> LoadUFD(int ufdAddress)
        {
            // 模拟加载 UFD 数据
            var block = Disk[ufdAddress];
            return string.IsNullOrEmpty(block.Data) ? new List<UFD>() : Newtonsoft.Json.JsonConvert.DeserializeObject<List<UFD>>(block.Data);
        }

        // 保存用户文件目录
        private void SaveUFD()
        {
            var user = MFDList.FirstOrDefault(u => u.Username == CurrentUsername);
            if (user == null) return;

            var block = Disk[user.UFDAddress];
            block.Data = Newtonsoft.Json.JsonConvert.SerializeObject(CurrentUserUFD);
        }

        // 创建文件按钮点击事件
        private void CreateFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentUsername))
            {
                MessageBox.Show("请先登录！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string fileName = "NewFile.txt"; // 示例文件名
            if (CurrentUserUFD.Any(f => f.FileName == fileName))
            {
                MessageBox.Show("文件名已存在！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var freeBlock = GetFreeBlock();
            if (freeBlock == null)
            {
                MessageBox.Show("磁盘已满，无法分配！", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            freeBlock.IsOccupied = true;
            CurrentUserUFD.Add(new UFD { FileName = fileName, BlockAddress = freeBlock.BlockId, LastModified = DateTime.Now });

            SaveUFD();
            UpdateGrids();
            MessageBox.Show("文件创建成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DeleteFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentUsername))
            {
                MessageBox.Show("请先登录！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrentUserUFD.Count == 0)
            {
                MessageBox.Show("当前目录中无文件！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 假设删除选中的文件
            var selectedFile = UFDGrid.SelectedItem as UFD;
            if (selectedFile == null)
            {
                MessageBox.Show("请选择要删除的文件！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            CurrentUserUFD.Remove(selectedFile);
            Disk[selectedFile.BlockAddress].IsOccupied = false;

            SaveUFD();
            UpdateGrids();
            MessageBox.Show("文件删除成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentUsername))
            {
                MessageBox.Show("请先登录！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedFile = UFDGrid.SelectedItem as UFD;
            if (selectedFile == null)
            {
                MessageBox.Show("请选择要打开的文件！", "错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (UOFList.Any(uof => uof.FileName == selectedFile.FileName))
            {
                MessageBox.Show("文件已被打开！", "警告", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            UOFList.Add(new UOF { FileName = selectedFile.FileName, Mode = 1 }); // 模式 1 代表只读
            UpdateGrids();
            MessageBox.Show("文件打开成功！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        // 数据结构定义
        public class MFD
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public int UFDAddress { get; set; }
        }

        public class UFD
        {
            public string FileName { get; set; }
            public int BlockAddress { get; set; }
            public DateTime LastModified { get; set; }
        }

        public class UOF
        {
            public string FileName { get; set; }
            public int Mode { get; set; }
        }

        public class DiskBlock
        {
            public int BlockId { get; set; }
            public bool IsOccupied { get; set; }
            public string Data { get; set; }
        }



        // 添加以下方法模板到 FileManage.xaml.cs 文件中
        private void CloseFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("关闭文件按钮被点击！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }



        private void ModifyFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("修改文件按钮被点击！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void WriteFileButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("写文件按钮被点击！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
