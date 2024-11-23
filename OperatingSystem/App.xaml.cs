using OperatingSystem.Progress;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Windows;

namespace OperatingSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ObservableCollection<ProcessModel> Processes { get; set; } = new ObservableCollection<ProcessModel>();
    }

}
