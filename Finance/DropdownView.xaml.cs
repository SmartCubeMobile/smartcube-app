namespace SmartCubeMobile;

public partial class DropdownView : ContentView
{
    private readonly List<IMultiSelectItem> _items;
    private readonly Action<List<IMultiSelectItem>> _onChanged;

    public DropdownView(
        List<IMultiSelectItem> items,
        Action<List<IMultiSelectItem>> onChanged)
    {
        InitializeComponent();

        ItemsView.HeightRequest = Math.Min(items.Count * 60, 250);

        _items = items;
        _onChanged = onChanged;

        ItemsView.ItemsSource = _items;

        foreach (var item in _items)
        {
            item.PropertyChanged += (_, __) =>
            {
                _onChanged(_items.Where(x => x.IsChecked).ToList());
            };
        }
        return;
    }
}