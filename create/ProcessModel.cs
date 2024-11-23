namespace ProcessManager
{
    public class ProcessModel
    {
        public int ProcessID { get; set; }
        public string ProcessName { get; set; }
        public string ProcessDescription { get; set; }
        public double ArrivalTime { get; set; }
        public double ServiceTime { get; set; }
        public double RemainingTime => ServiceTime; // 初始剩余时间等于服务时间
    }
}
