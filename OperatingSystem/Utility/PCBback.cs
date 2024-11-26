using System.ComponentModel;

public class PCBack : INotifyPropertyChanged
{
    private int _processID;
    public int ProcessID
    {
        get => _processID;
        set
        {
            _processID = value;
            OnPropertyChanged(nameof(ProcessID));
        }
    }

    private string _processName;
    public string ProcessName
    {
        get => _processName;
        set
        {
            _processName = value;
            OnPropertyChanged(nameof(ProcessName));
        }
    }

    public string ProcessDescription { get; set; }
    public double ArrivalTime { get; set; }
    public double ServiceTime { get; set; }
    public double RemainingTime { get; set; }

    // 完成时间默认为 null
    private double? _completionTime;
    public double? CompletionTime
    {
        get => _completionTime;
        set
        {
            _completionTime = value;
            OnPropertyChanged(nameof(CompletionTime));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
