using System;
using System.ComponentModel;

namespace OperatingSystem.Progress
{
    public class PCB : INotifyPropertyChanged
    {
        // 原有字段和功能完全保留
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

        private int _pc;
        public int PC
        {
            get => _pc;
            set { _pc = value; OnPropertyChanged(nameof(PC)); }
        }

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
            set
            {
                _runtime = value;
                OnPropertyChanged(nameof(Runtime));
                OnPropertyChanged(nameof(RemainingTime)); // 动态更新剩余时间
            }
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

        private int _requiredMemory;
        public int RequiredMemory
        {
            get => _requiredMemory;
            set { _requiredMemory = value; OnPropertyChanged(nameof(RequiredMemory)); }
        }

        private int _requiredCPU;
        public int RequiredCPU
        {
            get => _requiredCPU;
            set { _requiredCPU = value; OnPropertyChanged(nameof(RequiredCPU)); }
        }

        private int _ioOperations;
        public int IOOperations
        {
            get => _ioOperations;
            set { _ioOperations = value; OnPropertyChanged(nameof(IOOperations)); }
        }

        private int _pauseTime;
        public int PauseTime
        {
            get => _pauseTime;
            set { _pauseTime = value; OnPropertyChanged(nameof(PauseTime)); }
        }

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

        private double _serviceTime;
        public double ServiceTime
        {
            get => _serviceTime;
            set { _serviceTime = value; OnPropertyChanged(nameof(ServiceTime)); }
        }

        private string _blockReason;
        public string BlockReason
        {
            get => _blockReason;
            set { _blockReason = value; OnPropertyChanged(nameof(BlockReason)); }
        }

        private DateTime? _completionTime;
        public DateTime? CompletionTime
        {
            get => _completionTime;
            set { _completionTime = value; OnPropertyChanged(nameof(CompletionTime)); }
        }

        public PCB Next { get; set; } // 链表结构指针

        // 动态计算周转时间
        public int TurnaroundTime
        {
            get => EndTime - ArrivalTime;
        }

        // 动态计算带权周转时间
        public double WeightedTurnaroundTime
        {
            get => TotalTime > 0 ? (double)TurnaroundTime / TotalTime : 0;
        }

        // === 新增方法 ===

        // 更新状态和PC值
        public void UpdateStatus(string newStatus, int newPC)
        {
            Status = newStatus;
            OnPropertyChanged(nameof(Status)); // 添加状态变化通知
            PC = newPC;
            OnPropertyChanged(nameof(PC)); // 添加PC变化通知
        }


        // 增加运行时间
        public void IncrementRuntime()
        {
            Runtime++;
            RemainingTime = TotalTime - Runtime; // 自动更新剩余时间
            OnPropertyChanged(nameof(Runtime)); // 添加运行时间变化通知
            OnPropertyChanged(nameof(RemainingTime)); // 添加剩余时间变化通知

            if (Runtime >= TotalTime)
            {
                Status = "COMPLETED"; // 完成状态
                OnPropertyChanged(nameof(Status)); // 添加状态变化通知
                CompletionTime = DateTime.Now;
                OnPropertyChanged(nameof(CompletionTime)); // 通知完成时间变化
            }
            else
            {
                Status = "RUNNING"; // 运行中状态
                OnPropertyChanged(nameof(Status)); // 添加状态变化通知
            }
        }


        // 构造函数
        public PCB(int pid, string imageName, string description, int totalTime, int pc)
        {
            PID = pid;
            ImageName = imageName;
            Description = description;
            TotalTime = totalTime;
            Runtime = 0;
            PC = pc; // 初始化 PC
            Status = "READY"; // 初始状态
        }

        // 实现 INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 调试输出
        public override string ToString()
        {
            return $"PID: {PID}, Name: {ImageName}, Status: {Status}, PC: {PC}, Runtime: {Runtime}/{TotalTime}";
        }
    }
}
