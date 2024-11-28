using System;
using System.ComponentModel;

namespace OperatingSystem.Progress
{
    public class PCB : INotifyPropertyChanged
    {
        // 进程标识符
        private int _pid;
        public int PID
        {
            get => _pid;
            set { _pid = value; OnPropertyChanged(nameof(PID)); }
        }

        private string _imageName;
        public string ImageName
        {
            get => _imageName;
            set { _imageName = value; OnPropertyChanged(nameof(ImageName)); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        // 处理机状态
        private int _pc;
        public int PC
        {
            get => _pc;
            set { _pc = value; OnPropertyChanged(nameof(PC)); }
        }

        // 进程调度信息
        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        private int _arrivalTime;
        public int ArrivalTime
        {
            get => _arrivalTime;
            set { _arrivalTime = value; OnPropertyChanged(nameof(ArrivalTime)); }
        }

        private int _totalTime;
        public int TotalTime
        {
            get => _totalTime;
            set { _totalTime = value; OnPropertyChanged(nameof(TotalTime)); }
        }

        private int _remainingTime;
        public int RemainingTime
        {
            get => _remainingTime;
            set { _remainingTime = value; OnPropertyChanged(nameof(RemainingTime)); }
        }

        private int _runtime;
        public int Runtime
        {
            get => _runtime;
            set { _runtime = value; OnPropertyChanged(nameof(Runtime)); }
        }

        private int _endTime;
        public int EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        private int _priority;
        public int Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(nameof(Priority)); }
        }

        private int _policy;
        public int Policy
        {
            get => _policy;
            set { _policy = value; OnPropertyChanged(nameof(Policy)); }
        }

        private string _event;
        public string Event
        {
            get => _event;
            set { _event = value; OnPropertyChanged(nameof(Event)); }
        }

        // 进程控制信息
        private int _mutex;
        public int Mutex
        {
            get => _mutex;
            set { _mutex = value; OnPropertyChanged(nameof(Mutex)); }
        }

        private int _empty;
        public int Empty
        {
            get => _empty;
            set { _empty = value; OnPropertyChanged(nameof(Empty)); }
        }

        private int _full;
        public int Full
        {
            get => _full;
            set { _full = value; OnPropertyChanged(nameof(Full)); }
        }

        // 新增属性：服务时间
        private double _serviceTime;
        public double ServiceTime
        {
            get => _serviceTime;
            set { _serviceTime = value; OnPropertyChanged(nameof(ServiceTime)); }
        }


        // 动态计算周转时间
        public int TurnaroundTime
        {
            get => EndTime - ArrivalTime; // 周转时间 = 完成时间 - 到达时间
        }

        // 动态计算带权周转时间
        public double WeightedTurnaroundTime
        {
            get => TotalTime > 0 ? (double)TurnaroundTime / TotalTime : 0; // 带权周转时间 = 周转时间 / 服务时间
        }

        private DateTime? _completionTime;
        public DateTime? CompletionTime
        {
            get => _completionTime;
            set { _completionTime = value; OnPropertyChanged(nameof(CompletionTime)); }
        }

        public PCB Next { get; set; } // 链表结构指针

        // 构造函数
        public PCB(int pid, string imageName, string description, int totalTime, int arriveTime)
        {
            PID = pid;
            ImageName = imageName;
            Description = description;
            TotalTime = totalTime;
            RemainingTime = totalTime;
            ArrivalTime = arriveTime;
            Status = "READY"; // 初始状态
        }

        // INotifyPropertyChanged 实现
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 重写 ToString 方法方便调试
        public override string ToString()
        {
            return $"PID: {PID}, Name: {ImageName}, Status: {Status}, ArrivalTime: {ArrivalTime}, TotalTime: {TotalTime}";
        }
    }
}
