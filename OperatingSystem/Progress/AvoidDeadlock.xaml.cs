using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OperatingSystem.Progress
{
    public partial class AvoidDeadlock : Page
    {
        // 资源数量
        private int[] Available = new int[4];  // 可用资源向量
        private int[,] Max = new int[5, 4];    // 最大需求矩阵
        private int[,] Allocation = new int[5, 4];  // 分配矩阵
        private int[,] Need = new int[5, 4];  // 需求矩阵

        // 模拟进程信息
        private List<Process> processes = new List<Process>
        {
            new Process { PID = "P1", Allocation = new int[4], Max = new int[4] },
            new Process { PID = "P2", Allocation = new int[4], Max = new int[4] },
            new Process { PID = "P3", Allocation = new int[4], Max = new int[4] },
            new Process { PID = "P4", Allocation = new int[4], Max = new int[4] },
            new Process { PID = "P5", Allocation = new int[4], Max = new int[4] }
        };

        public AvoidDeadlock()
        {
            InitializeComponent();
        }

        // 加载测试数据
        private void LoadTestData_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                LoadData();

                // 更新 UI 显示矩阵
                UpdateDataGrids();
                MessageBox.Show("测试数据加载成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载测试数据时发生错误: " + ex.Message);
            }
        }

        private void LoadData()
        {
            // 初始化资源
            Available[0] = int.Parse(AvailableResource1.Text);
            Available[1] = int.Parse(AvailableResource2.Text);
            Available[2] = int.Parse(AvailableResource3.Text);
            Available[3] = int.Parse(AvailableResource4.Text);

            // 初始化最大需求矩阵（Max）
            Max = new int[,]
            {
                    { 7, 5, 3, 0 },
                    { 3, 2, 2, 0 },
                    { 9, 0, 2, 0 },
                    { 2, 2, 2, 0 },
                    { 4, 3, 3, 0 }
            };

            // 初始化已分配资源矩阵（Allocation）
            Allocation = new int[,]
            {
                    { 0, 1, 0, 0 },
                    { 2, 0, 0, 0 },
                    { 3, 0, 2, 0 },
                    { 2, 1, 1, 0 },
                    { 0, 0, 2, 0 }
            };
        }


        // 计算需求矩阵 Need = Max - Allocation
        private void CalculateNeedMatrix()
        {
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Need[i, j] = Max[i, j] - Allocation[i, j];
                }
            }
        }

        // 更新 UI 显示矩阵
        private void UpdateDataGrids()
        {
            MaxMatrixDataGrid.ItemsSource = GetProcessData(Max, "Max");
            AllocationMatrixDataGrid.ItemsSource = GetProcessData(Allocation, "Allocation");
            NeedMatrixDataGrid.ItemsSource = GetProcessData(Need, "Need");
        }

        // 根据矩阵生成数据源
        private List<ProcessMatrix> GetProcessData(int[,] matrix, string matrixType)
        {
            List<ProcessMatrix> data = new List<ProcessMatrix>();
            for (int i = 0; i < 5; i++)
            {
                ProcessMatrix pm = new ProcessMatrix
                {
                    PID = "P" + (i + 1),
                    Resource1 = matrix[i, 0],
                    Resource2 = matrix[i, 1],
                    Resource3 = matrix[i, 2],
                    Resource4 = matrix[i, 3]
                };
                data.Add(pm);
            }
            return data;
        }

        // 检查是否安全
        private void CheckSafety_Click(object sender, RoutedEventArgs e)
        {
            string safetySequence = BankersAlgorithm(out bool isSafe);

            if (isSafe)
            {
                SystemStatusTextBox.Text = "系统状态：安全";
                SafeSequenceTextBox.Text = "安全序列: " + safetySequence;
            }
            else
            {
                SystemStatusTextBox.Text = "系统状态：不安全";
                SafeSequenceTextBox.Text = "无安全序列";
            }
            MessageBox.Show("此时刻安全检查完成");
        }


        // 使用银行家算法检查系统安全性
        private string BankersAlgorithm(out bool isSafe)
        {
            int[] Work = (int[])Available.Clone(); // 当前的可用资源向量
            bool[] Finish = new bool[5]; // 完成状态
            List<string> safetySequence = new List<string>();
            List<SafetyCheckStep> steps = new List<SafetyCheckStep>(); // 存储步骤数据

            while (true)
            {
                bool progressMade = false;

                for (int i = 0; i < 5; i++)
                {
                    if (!Finish[i] && CanAllocate(i, Work))
                    {
                        // 更新安全序列
                        safetySequence.Add("P" + (i + 1));

                        // 记录当前步骤
                        steps.Add(new SafetyCheckStep
                        {
                            ProcessID = "P" + (i + 1),
                            Work = string.Join(", ", Work),
                            Need = string.Join(", ", Need[i, 0], Need[i, 1], Need[i, 2], Need[i, 3]),
                            Allocation = string.Join(", ", Allocation[i, 0], Allocation[i, 1], Allocation[i, 2], Allocation[i, 3]),
                            WorkPlusAllocation = string.Join(", ", Work.Select((w, j) => w + Allocation[i, j])),
                            Finish = "True"
                        });

                        // 模拟资源分配
                        for (int j = 0; j < 4; j++)
                        {
                            Work[j] += Allocation[i, j];
                        }

                        Finish[i] = true;
                        progressMade = true;
                    }
                }

                // 如果无法再分配任何资源，退出循环
                if (!progressMade)
                {
                    break;
                }
            }

            // 判断是否安全
            isSafe = Finish.All(f => f);

            // 更新 DataGrid 数据源
            SafeSequenceDataGrid.ItemsSource = steps;

            return isSafe ? string.Join(" -> ", safetySequence) : "无安全序列";
        }


        // 判断是否可以为进程分配资源
        private bool CanAllocate(int processIndex, int[] Work)
        {
            for (int j = 0; j < 4; j++)
            {
                if (Need[processIndex, j] > Work[j])
                {
                    return false;
                }
            }
            return true;
        }

        // 判断资源请求是否有效
        private bool IsRequestValid(int processIndex, int[] request)
        {
            for (int i = 0; i < 4; i++)
            {
                if (request[i] > Need[processIndex, i] || request[i] > Available[i])
                {
                    return false;
                }
            }
            return true;
        }


        // 执行银行家算法的按钮点击事件
        private void RunBankerAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            string process = RequestProcess.Text.ToUpper();
            int[] request = new int[4];
            try
            {
                // 从用户输入读取资源请求
                request[0] = int.Parse(RequestResource1.Text);
                request[1] = int.Parse(RequestResource2.Text);
                request[2] = int.Parse(RequestResource3.Text);
                request[3] = int.Parse(RequestResource4.Text);

                LoadData();
                CalculateNeedMatrix();

                // 查找进程索引
                int processIndex = processes.FindIndex(p => p.PID == process);
                if (processIndex == -1)
                {
                    MessageBox.Show("无效的进程ID！");
                    return;
                }

                // 判断资源请求是否有效
                if (IsRequestValid(processIndex, request))
                {
                    // 更新分配矩阵 Allocation
                    for (int i = 0; i < 4; i++)
                    {
                        Allocation[processIndex, i] += request[i];
                        Available[i] -= request[i];
                    }

                    // 更新需求矩阵 Need
                    for (int i = 0; i < 4; i++)
                    {
                        Need[processIndex, i] -= request[i];
                    }

                    UpdateDataGrids(); // 更新数据网格
                    MessageBox.Show("资源分配成功！");

                    string safetySequence = BankersAlgorithm(out bool isSafe);

                    if (isSafe)
                    {
                        SystemStatusTextBox.Text = "系统状态：安全";
                        SafeSequenceTextBox.Text = "安全序列: " + safetySequence;
                    }
                    else
                    {
                        SystemStatusTextBox.Text = "系统状态：不安全";
                        SafeSequenceTextBox.Text = "无安全序列";
                    }

                }
                else
                {
                    MessageBox.Show("请求无效！");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入错误: " + ex.Message);
            }
        }

        // 计算需求矩阵按钮的事件处理程序
        private void CalculateNeed_Click(object sender, RoutedEventArgs e)
        {
            CalculateNeedMatrix(); // 重新计算需求矩阵
            UpdateDataGrids(); // 更新数据网格显示
            MessageBox.Show("需求矩阵已更新！");
        }



    }

    // 用于绑定进程矩阵的类
    public class ProcessMatrix
    {
        public string PID { get; set; }
        public int Resource1 { get; set; }
        public int Resource2 { get; set; }
        public int Resource3 { get; set; }
        public int Resource4 { get; set; }
    }

    // 用于模拟进程的数据类
    public class Process
    {
        public string PID { get; set; }
        public int[] Allocation { get; set; }
        public int[] Max { get; set; }
    }


    // 用于显示安全检查中间过程的类
    public class SafetyCheckStep
    {
        public string ProcessID { get; set; }
        public string Work { get; set; }
        public string Need { get; set; }
        public string Allocation { get; set; }
        public string WorkPlusAllocation { get; set; }
        public string Finish { get; set; }
    }


}
