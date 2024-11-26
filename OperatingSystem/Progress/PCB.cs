using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperatingSystem.Progress
{
    public class PCB
    {
        // 进程标识符
        public int PID { get; set; } // 进程ID
        public string ImageName { get; set; } // 进程映像名称
        public string Description { get; set; } // 进程描述

        // 处理机状态
        public int PC { get; set; } // 指令计数器

        // 进程调度信息
        public string Status { get; set; } // READY, RUNNING, BLOCK
        public DateTime ArrivalTime { get; set; } // 到达时间
        public int TotalTime { get; set; } // 总服务时间
        public int RemainingTime { get; set; } // 剩余服务时间
        public int Runtime { get; set; } // 已运行时间
        public DateTime? EndTime { get; set; } // 完成时间
        public int Priority { get; set; } // 进程优先级
        public int Policy { get; set; } // 调度策略
        public string Event { get; set; } // 阻塞原因

        // 进程控制信息
        public int Mutex { get; set; }
        public int Empty { get; set; }
        public int Full { get; set; }

        public PCB Next { get; set; } // 链表结构指针

        // 构造函数
        public PCB(int pid, string imageName, string description, int totalTime, int policy)
        {
            PID = pid;
            ImageName = imageName;
            Description = description;
            TotalTime = totalTime;
            RemainingTime = totalTime;
            ArrivalTime = DateTime.Now;
            Policy = policy;
            Status = "READY"; // 初始状态
        }
    }
}
