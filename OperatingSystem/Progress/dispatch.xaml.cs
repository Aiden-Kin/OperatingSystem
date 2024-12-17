using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OperatingSystem.Progress
{
    /// <summary>
    /// dispatch.xaml 的交互逻辑
    /// </summary>

    public partial class Dispatch : Page
    {
        public Dispatch()
        {
            InitializeComponent();
            InitializeData(); // 初始化数据
        }

        // 初始化就绪队列的测试数据
        private void InitializeData()
        {
           

            UpdateUI();
        }

        // 更新UI
        private void UpdateUI()
        {
            ReadyQueueDataGrid.ItemsSource = App.Processes.Where(p => p.Status == "READY").ToList();
            RunningQueueDataGrid.ItemsSource = App.Processes.Where(p => p.Status == "RUNNING").ToList();
            CompletedQueueDataGrid.ItemsSource = App.Processes.Where(p => p.Status == "COMPLETED").ToList();
            UpdateResults(); // 更新调度结果
        }

        // 更新调度结果
        private void UpdateResults()
        {
            var completedProcesses = App.Processes.Where(p => p.Status == "COMPLETED").ToList();
            if (completedProcesses.Count > 0)
            {
                double avgTurnaroundTime = completedProcesses
                    .Average(p => p.EndTime - p.ArrivalTime);

                double avgWeightedTurnaroundTime = completedProcesses
                    .Average(p => (p.TotalTime > 0)
                        ? ((double)(p.EndTime - p.ArrivalTime) / p.TotalTime)
                        : 0);

                AverageTurnaroundTime.Text = $"平均周转时间：{avgTurnaroundTime:F2} 秒";
                WeightedAverageTurnaroundTime.Text = $"平均带权周转时间：{avgWeightedTurnaroundTime:F2}";

                // 显示执行顺序
                ExecutionOrder.Text = "执行顺序：" + string.Join(", ", App.ExecutionSequence);

                // 按结束时间显示完成顺序
                CompletionOrder.Text = "完成顺序：" + string.Join(", ", completedProcesses.OrderBy(p => p.EndTime).Select(p => p.PID));
            }
            else
            {
                AverageTurnaroundTime.Text = "平均周转时间：-";
                WeightedAverageTurnaroundTime.Text = "平均带权周转时间：-";
                ExecutionOrder.Text = "执行顺序：-";
                CompletionOrder.Text = "完成顺序：-";
            }
        }

        // 开始模拟按钮点击事件


        // 停止模拟按钮点击事件
        private void StopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            // 将所有进程的状态重置为 READY
            foreach (var process in App.Processes)
            {
                if (process.Status == "COMPLETED") // 只重置非 COMPLETED 的进程
                {
                    process.Status = "READY";
                    process.RemainingTime = process.TotalTime; // 恢复剩余时间
                }
            }

            // 更新 UI
            UpdateUI();

            // 提示用户
            MessageBox.Show("模拟已停止，所有进程状态已重置为 READY。");
        }

        // 先来先服务调度算法 (FCFS)

        private async Task ScheduleFCFS()
        {
            // 清空执行顺序
            App.ExecutionSequence.Clear();

            int currentTime = 0; // 初始化模拟时钟
            var readyQueue = App.Processes.Where(p => p.Status == "READY").OrderBy(p => p.ArrivalTime).ToList();

            foreach (var process in readyQueue)
            {
                process.Status = "RUNNING";
                App.ExecutionSequence.Add(process.PID); // 记录执行顺序
                UpdateUI();

                // 模拟运行
                await Task.Delay(process.TotalTime * 100); // 模拟进程运行时间
                currentTime += process.TotalTime;

                process.Status = "COMPLETED";
                process.EndTime = currentTime;
            }

            UpdateUI();
        }



        // 抢占式短进程优先调度算法 (SRTF)
        private async Task ScheduleSJF()
        {
            // 清空执行顺序
            App.ExecutionSequence.Clear();

            var simulationStartTime = DateTime.Now;

            while (true)
            {
                // 获取 READY 状态的进程并按 TotalTime 排序
                var readyQueue = App.Processes.Where(p => p.Status == "READY").OrderBy(p => p.TotalTime).ToList();

                if (readyQueue.Count == 0)
                {
                    break; // 没有 READY 状态的进程时退出调度
                }

                var process = readyQueue.First(); // 选择运行时间最短的进程

                process.Status = "RUNNING";
                App.ExecutionSequence.Add(process.PID); // 记录执行顺序
                UpdateUI();

                // 模拟运行
                await Task.Delay(process.TotalTime * 1000);

                process.Status = "COMPLETED";
                process.EndTime = (int)(DateTime.Now - simulationStartTime).TotalSeconds;

                UpdateUI();
            }
        }



        // 时间片轮转调度算法 (RR)
        private async Task ScheduleRR(int timeSlice)
        {
            // 模拟开始时间
            var simulationStartTime = DateTime.Now;

            // 获取 READY 状态的进程队列
            var readyQueue = App.Processes.Where(p => p.Status == "READY").ToList();

            while (readyQueue.Count > 0)
            {
                foreach (var process in readyQueue.ToList())
                {
                    process.Status = "RUNNING"; // 设置为运行状态
                    App.ExecutionSequence.Add(process.PID); // 记录执行顺序
                    UpdateUI();

                    // 模拟时间片运行
                    int sliceTime = Math.Min(timeSlice, process.RemainingTime); // 计算实际运行时间
                    await Task.Delay(sliceTime * 1000); // 以毫秒为单位延迟

                    // 减少剩余时间
                    process.RemainingTime = Math.Max(0, process.RemainingTime - sliceTime);

                    if (process.RemainingTime <= 0)
                    {
                        process.Status = "COMPLETED"; // 设置为 COMPLETED
                        process.EndTime = (int)(DateTime.Now - simulationStartTime).TotalSeconds;
                        readyQueue.Remove(process); // 从队列中移除
                    }
                    else
                    {
                        process.Status = "READY"; // 重新设置为 READY
                    }

                    UpdateUI(); // 更新界面
                }
            }
        }
        private async void StartSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 清空执行顺序
                App.ExecutionSequence.Clear();

                if (FCFSRadioButton.IsChecked == true)
                {
                    await ScheduleFCFS(); // 调用先来先服务调度
                }
                else if (SJFRadioButton.IsChecked == true)
                {
                    await ScheduleSJF(); // 调用短进程优先调度
                }
                else if (RRRadioButton.IsChecked == true)
                {
                    await ScheduleRR(1); // 调用时间片轮转调度，时间片为1秒
                }

                UpdateUI(); // 更新UI
            }
            catch (Exception ex)
            {
                MessageBox.Show($"调度失败：{ex.Message}");
            }
        }



        private void ReadyQueueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
