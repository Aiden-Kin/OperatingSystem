using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Equipment
{
    public partial class DiskSchedulerPage : Page
    {
        public DiskSchedulerPage()
        {
            InitializeComponent();
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 输入处理和验证
                var inputTracks = TrackSequenceTextBox.Text.Split(',')
                                        .Select(x =>
                                        {
                                            if (!int.TryParse(x, out int val))
                                                throw new Exception($"非法输入: {x}");
                                            return val;
                                        }).ToList();

                if (!int.TryParse(CurrentTrackTextBox.Text, out int currentTrack))
                    throw new Exception("当前磁道号必须为整数。");

                if (!int.TryParse(DirectionTextBox.Text, out int direction) || (direction != 0 && direction != 1))
                    throw new Exception("磁头移动方向必须为 0 (减小) 或 1 (增加)。");

                // 调度算法选择
                List<ResultData> results = null;
                if (FCFSRadioButton.IsChecked == true)
                    results = FCFS(inputTracks, currentTrack);
                else if (SSTFRadioButton.IsChecked == true)
                    results = SSTF(inputTracks, currentTrack);
                else if (SCANRadioButton.IsChecked == true)
                    results = SCAN(inputTracks, currentTrack, direction);
                else if (CSCANRadioButton.IsChecked == true)
                    results = CSCAN(inputTracks, currentTrack, direction);

                // 更新UI结果
                ResultsDataGrid.ItemsSource = results;
                int totalDistance = results.Sum(r => r.Distance);
                double averageDistance = totalDistance / (double)results.Count;

                SummaryTextBlock.Text = $"磁头移动的总距离: {totalDistance}   平均寻道长度: {averageDistance:F3}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"输入错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        public class ResultData
        {
            public int Track { get; set; }
            public int Distance { get; set; }
        }

        private List<ResultData> FCFS(List<int> tracks, int start)
        {
            List<ResultData> results = new();
            int current = start;

            foreach (var track in tracks)
            {
                results.Add(new ResultData { Track = track, Distance = Math.Abs(track - current) });
                current = track;
            }
            return results;
        }

        private List<ResultData> SSTF(List<int> tracks, int start)
        {
            List<ResultData> results = new();
            List<int> remaining = new(tracks);
            int current = start;

            while (remaining.Count > 0)
            {
                var closest = remaining.OrderBy(x => Math.Abs(x - current)).First();
                results.Add(new ResultData { Track = closest, Distance = Math.Abs(closest - current) });
                current = closest;
                remaining.Remove(closest);
            }
            return results;
        }

        // SCAN 调度算法
        // SCAN 调度算法
        private List<ResultData> SCAN(List<int> tracks, int start, int direction)
        {
            List<ResultData> results = new();
            var sortedTracks = tracks.Distinct().OrderBy(x => x).ToList(); // 去重并排序
            int current = start;

            if (direction == 1) // 向右扫描
            {
                var right = sortedTracks.Where(x => x >= start).ToList();
                var left = sortedTracks.Where(x => x < start).ToList();

                foreach (var track in right.Concat(left)) // 先向右，再从最小值开始
                {
                    results.Add(new ResultData { Track = track, Distance = Math.Abs(track - current) });
                    current = track;
                }
            }
            else // 向左扫描
            {
                var left = sortedTracks.Where(x => x <= start).Reverse().ToList();
                var right = sortedTracks.Where(x => x > start).ToList();

                foreach (var track in left.Concat(right)) // 先向左，再从最大值开始
                {
                    results.Add(new ResultData { Track = track, Distance = Math.Abs(track - current) });
                    current = track;
                }
            }

            return results;
        }
        // 停止模拟并清空所有数据
        private void StopSimulation_Click(object sender, RoutedEventArgs e)
        {
            // 清空输入框数据
            TrackSequenceTextBox.Text = string.Empty;
            CurrentTrackTextBox.Text = string.Empty;
            DirectionTextBox.Text = string.Empty;

            // 清空DataGrid数据
            ResultsDataGrid.ItemsSource = null;

            // 重置统计信息
            SummaryTextBlock.Text = "磁头移动的总距离: 0    平均寻道长度: 0.000";

            // 弹出提示信息（可选）
            MessageBox.Show("模拟已停止，所有数据已清空。", "停止模拟", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    

        // CSCAN 调度算法
        private List<ResultData> CSCAN(List<int> tracks, int start, int direction)
        {
            List<ResultData> results = new();
            var sortedTracks = tracks.Distinct().OrderBy(x => x).ToList(); // 去重并排序
            int current = start;

            if (direction == 1) // 向右扫描
            {
                var right = sortedTracks.Where(x => x >= start).ToList();
                var left = sortedTracks.Where(x => x < start).ToList();

                foreach (var track in right.Concat(left)) // 向右扫描到最大值，再跳到最小值
                {
                    results.Add(new ResultData { Track = track, Distance = Math.Abs(track - current) });
                    current = track;
                }
            }
            else // 向左扫描
            {
                var left = sortedTracks.Where(x => x <= start).Reverse().ToList();
                var right = sortedTracks.Where(x => x > start).Reverse().ToList();

                foreach (var track in left.Concat(right)) // 向左扫描到最小值，再跳到最大值
                {
                    results.Add(new ResultData { Track = track, Distance = Math.Abs(track - current) });
                    current = track;
                }
            }

            return results;
        }

        private void DirectionTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
