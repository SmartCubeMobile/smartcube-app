using System.ComponentModel;

namespace SmartCubeMobile
{
    public interface IMultiSelectItem : INotifyPropertyChanged
    {
        string Content { get; }
        bool IsChecked { get; set; }
    }
}
