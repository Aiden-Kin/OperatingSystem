using System;
using System.ComponentModel;

namespace OperatingSystem.Progress
{
    // Process Control Block (PCB) 类，实现 INotifyPropertyChanged 接口，用于进程调度管理
    public class PCB : INotifyPropertyChanged
    {
        // PID (进程标识符)
        private int _pid;
        public int PID
        {
            get => _pid;
            set { _pid = value; OnPropertyChanged(nameof(PID)); }
        }

        // 映像名称 (进程的图像名称或进程的标识)
        private string _imageName;
        public string ImageName
        {
            get => _imageName;
            set { _imageName = value; OnPropertyChanged(nameof(ImageName)); }
        }

        // 描述 (进程的描述信息)
        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(nameof(Description)); }
        }

        // 程序计数器 (PC) 的值，指示程序的当前执行位置
        private int _pc;
        public int PC
        {
            get => _pc;
            set { _pc = value; OnPropertyChanged(nameof(PC)); }
        }

        // 进程状态（例如 "READY", "RUNNING", "BLOCKED"）
        private string _status;
        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        // 到达时间（进程被创建时的时间）
        private int _arrivalTime;
        public int ArrivalTime
        {
            get => _arrivalTime;
            set { _arrivalTime = value; OnPropertyChanged(nameof(ArrivalTime)); }
        }

        // 总时间（进程需要运行的总时间）
        private int _totalTime;
        public int TotalTime
        {
            get => _totalTime;
            set { _totalTime = value; OnPropertyChanged(nameof(TotalTime)); }
        }

        // 剩余时间（进程尚需运行的时间）
        private int _remainingTime;
        public int RemainingTime
        {
            get => _remainingTime;
            set { _remainingTime = value; OnPropertyChanged(nameof(RemainingTime)); }
        }

        // 运行时间（进程已经执行的时间）
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

        // 结束时间（进程的完成时间）
        private int _endTime;
        public int EndTime
        {
            get => _endTime;
            set { _endTime = value; OnPropertyChanged(nameof(EndTime)); }
        }

        // 优先级（进程的优先级，用于调度）
        private int _priority;
        public int Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(nameof(Priority)); }
        }

        // 调度策略（用于指定进程调度策略）
        private int _policy;
        public int Policy
        {
            get => _policy;
            set { _policy = value; OnPropertyChanged(nameof(Policy)); }
        }

        // 事件（记录进程的当前事件或状态）
        private string _event;
        public string Event
        {
            get => _event;
            set { _event = value; OnPropertyChanged(nameof(Event)); }
        }

        // 所需内存（进程执行所需要的内存量）
        private int _requiredMemory;
        public int RequiredMemory
        {
            get => _requiredMemory;
            set { _requiredMemory = value; OnPropertyChanged(nameof(RequiredMemory)); }
        }

        // 所需CPU（进程执行所需要的CPU资源）
        private int _requiredCPU;
        public int RequiredCPU
        {
            get => _requiredCPU;
            set { _requiredCPU = value; OnPropertyChanged(nameof(RequiredCPU)); }
        }

        // I/O 操作次数（进程的 I/O 操作次数）
        private int _ioOperations;
        public int IOOperations
        {
            get => _ioOperations;
            set { _ioOperations = value; OnPropertyChanged(nameof(IOOperations)); }
        }

        // 暂停时间（进程被暂停的时间）
        private int _pauseTime;
        public int PauseTime
        {
            get => _pauseTime;
            set { _pauseTime = value; OnPropertyChanged(nameof(PauseTime)); }
        }

        // 互斥标志（进程是否在等待共享资源）
        private int _mutex;
        public int Mutex
        {
            get => _mutex;
            set { _mutex = value; OnPropertyChanged(nameof(Mutex)); }
        }

        // 空状态（进程是否处于空闲状态）
        private int _empty;
        public int Empty
        {
            get => _empty;
            set { _empty = value; OnPropertyChanged(nameof(Empty)); }
        }

        // 满状态（进程是否处于资源满载状态）
        private int _full;
        public int Full
        {
            get => _full;
            set { _full = value; OnPropertyChanged(nameof(Full)); }
        }

        // 服务时间（进程所需的服务时间）
        private double _serviceTime;
        public double ServiceTime
        {
            get => _serviceTime;
            set { _serviceTime = value; OnPropertyChanged(nameof(ServiceTime)); }
        }

        // 阻塞原因（进程被阻塞的原因）
        private string _blockReason;
        public string BlockReason
        {
            get => _blockReason;
            set { _blockReason = value; OnPropertyChanged(nameof(BlockReason)); }
        }

        // 链表结构指针，用于管理多个进程
        public PCB Next { get; set; }

        // 动态计算周转时间（进程从到达时间到完成时间的时间差）
        public int TurnaroundTime
        {
            get => EndTime - ArrivalTime;
        }

        // 动态计算带权周转时间（周转时间与总时间的比值）
        public double WeightedTurnaroundTime
        {
            get => TotalTime > 0 ? (double)TurnaroundTime / TotalTime : 0;
        }

        // 更新状态和PC值的方法
        public void UpdateStatus(string newStatus, int newPC)
        {
            Status = newStatus;
            OnPropertyChanged(nameof(Status)); // 添加状态变化通知
            PC = newPC;
            OnPropertyChanged(nameof(PC)); // 添加PC变化通知
        }

        // 构造函数，用于初始化 PCB 实例
        public PCB(int pid, string imageName, string description, int totalTime, int arriveTime)
        {
            PID = pid;
            ImageName = imageName;
            Description = description;
            TotalTime = totalTime;
            Runtime = 0;
            ArrivalTime = arriveTime;
            Status = "READY"; // 初始状态设为 READY
        }

        // INotifyPropertyChanged 实现，通知属性值变化
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // 调试输出方法，用于输出 PCB 实例的关键信息
        public override string ToString()
        {
            return $"PID: {PID}, Name: {ImageName}, Status: {Status}, PC: {PC}, Runtime: {Runtime}/{TotalTime}";
        }
    }
}
