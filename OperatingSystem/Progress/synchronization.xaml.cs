using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OperatingSystem.Progress
{
    public partial class Synchronization : Page
    {
        // 队列集合
        public ObservableCollection<PCB> ReadyQueue { get; set; } = new ObservableCollection<PCB>();
        public ObservableCollection<PCB> RunningQueue { get; set; } = new ObservableCollection<PCB>();
        public ObservableCollection<PCB> BlockedQueue { get; set; } = new ObservableCollection<PCB>();

        // 信号量
        private int Mutex = 1; // 互斥锁
        private int Empty = 10; // 空缓冲区
        private int Full = 0; // 满缓冲区

        // 执行状态
        private bool isRunning = false;

        public Synchronization()
        {
            InitializeComponent();

            // 数据绑定
            ReadyQueueDataGrid.ItemsSource = ReadyQueue;
            RunningQueueDataGrid.ItemsSource = RunningQueue;
            BlockedQueueDataGrid.ItemsSource = BlockedQueue;

            // 初始化信号量显示
            UpdateSemaphoreDisplay();
        }

        // 创建进程
        private void CreateProcess_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(ProducerCountTextBox.Text, out int producerCount) ||
                !int.TryParse(ConsumerCountTextBox.Text, out int consumerCount))
            {
                MessageBox.Show("请输入有效的生产者和消费者数量！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 创建生产者进程
            for (int i = 0; i < producerCount; i++)
            {
                ReadyQueue.Add(new PCB(i + 1, $"Producer{i + 1}", "生产者任务", 10, 0));
            }

            // 创建消费者进程
            for (int i = 0; i < consumerCount; i++)
            {
                ReadyQueue.Add(new PCB(producerCount + i + 1, $"Consumer{i + 1}", "消费者任务", 10, 1));
            }

            RefreshQueueDisplay();
        }

        // 删除进程
        private void DeleteProcess_Click(object sender, RoutedEventArgs e)
        {
            if (ReadyQueue.Count > 0)
            {
                ReadyQueue.RemoveAt(ReadyQueue.Count - 1);
                RefreshQueueDisplay();
            }
        }

        // 开始执行
        private async void StartExecution_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                MessageBox.Show("进程已经在运行中！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            isRunning = true;

            while (isRunning && (ReadyQueue.Count > 0 || RunningQueue.Count > 0 || BlockedQueue.Count > 0))
            {
                CPUScheduling();
                await ExecuteAndSwitchProcess();
                UnblockProcesses();
                await Task.Delay(300); // 模拟时间片
            }

            isRunning = false;
        }

        // 退出程序
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            isRunning = false;
            Application.Current.Shutdown();
        }

        // CPU 调度逻辑
        private void CPUScheduling()
        {
            if (RunningQueue.Count == 0 && ReadyQueue.Count > 0)
            {
                // 从就绪队列中获取一个进程
                PCB process = ReadyQueue[0];
                ReadyQueue.RemoveAt(0);

                // 设置进程状态为运行中
                process.Status = "RUNNING";
                RunningQueue.Add(process);

                Console.WriteLine($"调度进程: PID={process.PID}, Name={process.ImageName}, Status={process.Status}");
                RefreshQueueDisplay();
            }
            else
            {
                Console.WriteLine("无法调度进程：RunningQueue 已有进程或 ReadyQueue 为空");
            }
        }

        // 执行进程逻辑
        private async Task ExecuteAndSwitchProcess()
        {
            if (RunningQueue.Count == 0)
            {
                Console.WriteLine("运行队列为空");
                return;
            }

            // 获取当前运行的进程
            PCB currentProcess = RunningQueue[0];

            // 模拟运行逻辑
            if (currentProcess.Description.Contains("生产者"))
            {
                if (Empty > 0 && Mutex > 0)
                {
                    Mutex--;
                    Empty--;
                    Full++;
                    currentProcess.Runtime++;
                    Console.WriteLine($"生产者运行: PID={currentProcess.PID}, Name={currentProcess.ImageName}, Runtime={currentProcess.Runtime}");

                    // 检查是否完成
                    if (currentProcess.Runtime >= currentProcess.TotalTime)
                    {
                        currentProcess.Status = "COMPLETED";
                        Console.WriteLine($"进程完成: PID={currentProcess.PID}, Name={currentProcess.ImageName}");
                        RunningQueue.Remove(currentProcess);
                    }
                    else
                    {
                        currentProcess.Status = "RUNNING"; // 保持运行状态
                        // 继续保留在 RunningQueue 中
                    }

                    Mutex++;
                }
                else
                {
                    currentProcess.Status = "BLOCKED";
                    currentProcess.BlockReason = "缓冲区已满";
                    BlockedQueue.Add(currentProcess);
                    RunningQueue.Remove(currentProcess);
                    Console.WriteLine($"进程阻塞: PID={currentProcess.PID}, Name={currentProcess.ImageName}, 原因={currentProcess.BlockReason}");
                }
            }
            else if (currentProcess.Description.Contains("消费者"))
            {
                if (Full > 0 && Mutex > 0)
                {
                    Mutex--;
                    Full--;
                    Empty++;
                    currentProcess.Runtime++;
                    Console.WriteLine($"消费者运行: PID={currentProcess.PID}, Name={currentProcess.ImageName}, Runtime={currentProcess.Runtime}");

                    // 检查是否完成
                    if (currentProcess.Runtime >= currentProcess.TotalTime)
                    {
                        currentProcess.Status = "COMPLETED";
                        Console.WriteLine($"进程完成: PID={currentProcess.PID}, Name={currentProcess.ImageName}");
                        RunningQueue.Remove(currentProcess);
                    }
                    else
                    {
                        currentProcess.Status = "RUNNING"; // 保持运行状态
                        // 继续保留在 RunningQueue 中
                    }

                    Mutex++;
                }
                else
                {
                    currentProcess.Status = "BLOCKED";
                    currentProcess.BlockReason = "缓冲区为空";
                    BlockedQueue.Add(currentProcess);
                    RunningQueue.Remove(currentProcess);
                    Console.WriteLine($"进程阻塞: PID={currentProcess.PID}, Name={currentProcess.ImageName}, 原因={currentProcess.BlockReason}");
                }
            }

            RefreshQueueDisplay();
            await Task.Delay(500); // 模拟运行时间
        }

        // 尝试解除阻塞的进程
        private void UnblockProcesses()
        {
            // 创建一个临时列表来存储可以解除阻塞的进程
            var processesToUnblock = new ObservableCollection<PCB>();

            foreach (var process in BlockedQueue)
            {
                if (process.Description.Contains("生产者"))
                {
                    if (Empty > 0 && Mutex > 0)
                    {
                        processesToUnblock.Add(process);
                    }
                }
                else if (process.Description.Contains("消费者"))
                {
                    if (Full > 0 && Mutex > 0)
                    {
                        processesToUnblock.Add(process);
                    }
                }
            }

            // 解除阻塞
            foreach (var process in processesToUnblock)
            {
                if (process.Description.Contains("生产者"))
                {
                    Mutex--;
                    Empty--;
                    Full++;
                    process.Runtime++;
                    Console.WriteLine($"解除阻塞并运行生产者: PID={process.PID}, Name={process.ImageName}, Runtime={process.Runtime}");

                    // 检查是否完成
                    if (process.Runtime >= process.TotalTime)
                    {
                        process.Status = "COMPLETED";
                        Console.WriteLine($"进程完成: PID={process.PID}, Name={process.ImageName}");
                    }
                    else
                    {
                        process.Status = "READY"; // 返回就绪状态
                    }

                    Mutex++;

                    // 移除并添加回 ReadyQueue 或直接运行
                    BlockedQueue.Remove(process);

                    if (process.Status == "COMPLETED")
                    {
                        // 直接移除，无需添加回队列
                    }
                    else
                    {
                        ReadyQueue.Add(process);
                    }
                }
                else if (process.Description.Contains("消费者"))
                {
                    Mutex--;
                    Full--;
                    Empty++;
                    process.Runtime++;
                    Console.WriteLine($"解除阻塞并运行消费者: PID={process.PID}, Name={process.ImageName}, Runtime={process.Runtime}");

                    // 检查是否完成
                    if (process.Runtime >= process.TotalTime)
                    {
                        process.Status = "COMPLETED";
                        Console.WriteLine($"进程完成: PID={process.PID}, Name={process.ImageName}");
                    }
                    else
                    {
                        process.Status = "READY"; // 返回就绪状态
                    }

                    Mutex++;

                    // 移除并添加回 ReadyQueue 或直接运行
                    BlockedQueue.Remove(process);

                    if (process.Status == "COMPLETED")
                    {
                        // 直接移除，无需添加回队列
                    }
                    else
                    {
                        ReadyQueue.Add(process);
                    }
                }
            }

            if (processesToUnblock.Count > 0)
            {
                RefreshQueueDisplay();
            }
        }

        // 刷新界面显示
        private void RefreshQueueDisplay()
        {
            ReadyQueueDataGrid.Items.Refresh();
            RunningQueueDataGrid.Items.Refresh();
            BlockedQueueDataGrid.Items.Refresh();
            UpdateBufferDisplay();
            UpdateSemaphoreDisplay();

            // 调试输出
            Console.WriteLine($"ReadyQueue Count: {ReadyQueue.Count}");
            Console.WriteLine($"RunningQueue Count: {RunningQueue.Count}");
            Console.WriteLine($"BlockedQueue Count: {BlockedQueue.Count}");
        }

        // 更新缓冲池显示
        private void UpdateBufferDisplay()
        {
            for (int i = 0; i < 10; i++)
            {
                var bufferTextBox = (TextBox)FindName($"Buffer{i}");
                if (bufferTextBox != null)
                {
                    if (i < Full)
                    {
                        bufferTextBox.Text = "占用";
                    }
                    else
                    {
                        bufferTextBox.Text = "空闲";
                    }
                }
            }
        }

        // 更新信号量显示
        private void UpdateSemaphoreDisplay()
        {
            MutexTextBox.Text = Mutex.ToString();
            EmptyTextBox.Text = Empty.ToString();
            FullTextBox.Text = Full.ToString();
        }
    }
}
