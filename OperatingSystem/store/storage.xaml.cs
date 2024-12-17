using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace OperatingSystem.store
{
    public partial class Storage : Page
    {
      

        public Storage()
        {
            InitializeComponent();
        }

        private void StartSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 校验输入数据
                if (string.IsNullOrWhiteSpace(PageSequenceTextBox.Text))
                {
                    throw new Exception("页面访问序列不能为空！");
                }

                var pageSequence = PageSequenceTextBox.Text
                    .Trim()
                    .Replace('，', ',')
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s =>
                    {
                        if (int.TryParse(s.Trim(), out int result))
                            return result;
                        throw new Exception($"页面序列中包含无效值：{s.Trim()}");
                    })
                    .ToArray();

                if (!int.TryParse(MemoryBlockCountTextBox.Text.Trim(), out var memoryBlockCount) || memoryBlockCount <= 0)
                {
                    throw new Exception("内存块数必须是一个正整数！");
                }

                // 检查选中的算法
                string algorithm = OptAlgorithm.IsChecked == true ? "OPT" :
                                   FifoAlgorithm.IsChecked == true ? "FIFO" : "LRU";

                // 执行算法并获取结果
                var result = RunPageReplacementAlgorithm(pageSequence, memoryBlockCount, algorithm);

                // 显示结果
                ResultTextBox.Text = $"缺页次数: {result.MissCount}\n缺页率: {result.MissRate:P}\n页面置换次数: {result.ReplacementCount}";

                // 动态生成横向表格
                GenerateStepsGrid(result.StepDetails, memoryBlockCount, pageSequence.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"输入错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private (int MissCount, double MissRate, int ReplacementCount, List<StepDetail> StepDetails) RunPageReplacementAlgorithm(int[] pageSequence, int memoryBlockCount, string algorithm)
        {
            int missCount = 0, replacementCount = 0;
            var memory = new List<int>(); // 当前内存状态
            var steps = new List<StepDetail>(); // 每一步的详细信息
            var missPages = new List<int>(); // 缺页的页号
            var swapPages = new List<int>(); // 被置换的页号
            var memoryUsageOrder = new List<int>(); // 用于记录页面访问顺序（LRU）

            for (int i = 0; i < pageSequence.Length; i++)
            {
                int page = pageSequence[i]; // 当前访问的页面
                string memoryState = string.Join(" ", memory.Select(x => x.ToString()).ToArray());
                string missPage = "-", replacePage = "-", swapArea = "-";

                // 检查页面是否在内存中
                if (!memory.Contains(page))
                {
                    missCount++; // 缺页计数
                    missPage = page.ToString();
                    missPages.Add(page); // 记录缺页页号

                    // 如果内存已满，需要进行页面置换
                    if (memory.Count >= memoryBlockCount)
                    {
                        replacementCount++; // 页面置换计数
                        if (algorithm == "OPT")
                        {
                            // 最佳页面置换算法（OPT）
                            var farthestPage = memory.OrderByDescending(p =>
                            {
                                int nextUse = Array.IndexOf(pageSequence, p, i + 1);
                                return nextUse == -1 ? int.MaxValue : nextUse;
                            }).First();
                            memory.Remove(farthestPage);
                            replacePage = farthestPage.ToString();
                            swapArea = farthestPage.ToString();
                            swapPages.Add(farthestPage); // 记录被置换页
                        }
                        else if (algorithm == "FIFO")
                        {
                            // 先进先出算法（FIFO）
                            replacePage = memory[0].ToString();
                            swapArea = memory[0].ToString();
                            memory.RemoveAt(0); // 移除队列最前面的页面
                            swapPages.Add(int.Parse(replacePage)); // 记录被置换页
                        }
                        else if (algorithm == "LRU")
                        {
                            // 最近最久未使用算法（LRU）
                            if (memoryUsageOrder.Count > 0)
                            {
                                int leastRecentlyUsedPage = memoryUsageOrder[0]; // 获取最久未使用的页面
                                memory.Remove(leastRecentlyUsedPage); // 从内存中移除
                                memoryUsageOrder.Remove(leastRecentlyUsedPage); // 从使用顺序中移除

                                replacePage = leastRecentlyUsedPage.ToString();
                                swapArea = leastRecentlyUsedPage.ToString();
                                swapPages.Add(int.Parse(replacePage)); // 记录被置换页
                            }
                        }
                    }

                    // 添加新页面到内存
                    memory.Add(page);
                }

                // 更新 LRU 的使用顺序
                if (algorithm == "LRU")
                {
                    if (memoryUsageOrder.Contains(page))
                    {
                        memoryUsageOrder.Remove(page); // 如果页面已存在，移除旧位置
                    }
                    memoryUsageOrder.Add(page); // 添加到队尾（最新使用）
                }

                // 更新内存状态
                memoryState = string.Join(" ", memory.Select(x => x.ToString()).ToArray());

                // 记录当前步骤
                steps.Add(new StepDetail
                {
                    Step = i + 1,
                    MemoryState = memoryState,
                    MissPage = missPage,
                    ReplacePage = replacePage,
                    SwapArea = swapArea
                });
            }

            // 计算缺页率
            double missRate = (double)missCount / pageSequence.Length;

            // 返回最终结果
            return (missCount, missRate, replacementCount, steps);
        }



        private void GenerateStepsGrid(List<StepDetail> stepDetails, int memoryBlockCount, int sequenceLength)
        {
            StepsGrid.Children.Clear();
            StepsGrid.RowDefinitions.Clear();
            StepsGrid.ColumnDefinitions.Clear();

            // 添加列标题（步骤）
            StepsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            for (int col = 0; col <= sequenceLength; col++) // 多加两列：缺页页号和磁盘对换区
            {
                StepsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                var headerText = new TextBlock
                {
                    Text = col == 0 ? "物理块" :
                           col <= sequenceLength ? $"步骤{col}" :
                           col == sequenceLength + 1 ? "缺页页号" : "磁盘对换区",
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };
                Grid.SetRow(headerText, 0);
                Grid.SetColumn(headerText, col);
                StepsGrid.Children.Add(headerText);
            }

            // 填充物理块数据
            for (int row = 1; row <= memoryBlockCount + 2; row++) // 多加两行：缺页页号和磁盘对换区
            {
                StepsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                // 物理块标题
                var blockText = new TextBlock
                {
                    Text = row <= memoryBlockCount ? $"物理块{row}" :
                           row == memoryBlockCount + 1 ? "缺页页号" : "磁盘对换区",
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(5)
                };
                Grid.SetRow(blockText, row);
                Grid.SetColumn(blockText, 0);
                StepsGrid.Children.Add(blockText);

                // 每个步骤的数据
                for (int col = 1; col <= sequenceLength + 2; col++) // 遍历每一列
                {
                    string cellValue = "";

                    if (col <= sequenceLength) // 仅在有效步骤内显示数据
                    {
                        if (row <= memoryBlockCount)
                        {
                            var memoryParts = stepDetails[col - 1].MemoryState.Split(' ');
                            cellValue = row - 1 < memoryParts.Length ? memoryParts[row - 1] : "";
                        }
                        else if (row == memoryBlockCount + 1)
                        {
                            cellValue = stepDetails[col - 1].MissPage; // 显示缺页页号
                        }
                        else if (row == memoryBlockCount + 2)
                        {
                            cellValue = stepDetails[col - 1].SwapArea; // 显示磁盘对换区
                        }
                    }

                    // 创建单元格
                    var cellText = new TextBlock
                    {
                        Text = cellValue,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Margin = new Thickness(5)
                    };
                    Grid.SetRow(cellText, row);
                    Grid.SetColumn(cellText, col);
                    StepsGrid.Children.Add(cellText);
                }
            }

        }

        private void StopSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            // 清除模拟结果
            StepsGrid.Children.Clear();
            StepsGrid.RowDefinitions.Clear();
            StepsGrid.ColumnDefinitions.Clear();

            // 清空结果文本框
            ResultTextBox.Text = string.Empty;

            // 清空输入框（如果需要）
            PageSequenceTextBox.Text = string.Empty;
            MemoryBlockCountTextBox.Text = string.Empty;
            // 重置算法选择
            OptAlgorithm.IsChecked = false;
            FifoAlgorithm.IsChecked = false;
            LruAlgorithm.IsChecked = false;

            MessageBox.Show("模拟已停止，数据已清除！", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ReturnToMainPageButton_Click(object sender, RoutedEventArgs e)
        {
            
        }


        public class StepDetail
        {
            public int Step { get; set; }
            public string MemoryState { get; set; }
            public string MissPage { get; set; }
            public string ReplacePage { get; set; }
            public string SwapArea { get; set; }
        }
    }
}
