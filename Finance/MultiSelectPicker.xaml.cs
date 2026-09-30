using Microsoft.Maui.Layouts;

namespace SmartCubeMobile;

public partial class MultiSelectPicker : ContentView
{
    public Layout FloatingLayer { get; set; }

    private DropdownView dropdownView;
    private Grid backdropBoxview;
    private bool isOpen;

    public event EventHandler Closed;

    public MultiSelectPicker()
    {
        InitializeComponent();

        SelectedText.Text = Prompt;
        return;
    }

    private void HeaderTapped(object sender, TappedEventArgs e)
    {
        if (isOpen)
        {
            CloseDropdown();
            return;
        }
        OpenDropdown();
        return;
    }

    private void OpenDropdown()
    {
        if (FloatingLayer == null)
        {
            return;
        }

        FloatingLayer.IsVisible = true;
        FloatingLayer.Children.Clear();

        // BACKDROP (captures outside taps)
        // Replaced BoxView because it was coming up BLACK!!!
        backdropBoxview = new Grid
        {
            BackgroundColor = Colors.Transparent
        };

        backdropBoxview.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(CloseDropdown)
        });

        AbsoluteLayout.SetLayoutFlags(backdropBoxview, AbsoluteLayoutFlags.All);
        AbsoluteLayout.SetLayoutBounds(backdropBoxview, new Rect(0, 0, 1, 1));

        FloatingLayer.Children.Add(backdropBoxview);

        // DROPDOWN
        var items = ItemsSource?
            .Cast<IMultiSelectItem>()
            .ToList();

        if (items == null || items.Count == 0)
        {
            return;
        }

        dropdownView = new DropdownView(items, UpdateHeader);

        var headerPos = GetAbsolutePosition(HeaderBorder);
        var overlayPos = GetAbsolutePosition(FloatingLayer);

        var x = headerPos.X - overlayPos.X;
        var y = headerPos.Y - overlayPos.Y + HeaderBorder.Height;

        AbsoluteLayout.SetLayoutFlags(
            dropdownView,
            AbsoluteLayoutFlags.None);

        AbsoluteLayout.SetLayoutBounds(
            dropdownView,
            new Rect(
                x,
                y,
                HeaderBorder.Width,
                250));

        FloatingLayer.Children.Add(dropdownView);

        //Arrow.Text = "▴";
        ArrowImage.Source = "expander_close.png";

        isOpen = true;
        return;
    }

    private void CloseDropdown()
    {
        if (FloatingLayer == null)
        {
            return;
        }

        FloatingLayer.Children.Clear();
        FloatingLayer.IsVisible = false;

        ArrowImage.Source = "expander_open.png";

        isOpen = false;

        var selected = ItemsSource
            .Where(x => x.IsChecked)
            .Select(x => x.Content)
            .ToList();

        string message =
            selected.Count == 0
                ? "No items selected"
                : string.Join(Environment.NewLine, selected);

        Closed?.Invoke(this, EventArgs.Empty);
        return;
    }

    private void UpdateHeader(List<IMultiSelectItem> selected)
    {
        RefreshHeader();
        return;
    }

    public static readonly BindableProperty PromptProperty =
    BindableProperty.Create(nameof(Prompt),
                            typeof(string),
                            typeof(MultiSelectPicker),
                            "Select items",
                            propertyChanged: OnPromptChanged);

    public string Prompt
    {
        get => (string)GetValue(PromptProperty);
        set => SetValue(PromptProperty, value);
    }
    private static void OnPromptChanged(BindableObject bindable,
                                        object oldValue,
                                        object newValue)
    {
        if (bindable is MultiSelectPicker msp)
        {
            msp.RefreshHeader();
        }
        return;
    }

    public static readonly BindableProperty FontSizeProperty =
    BindableProperty.Create(
        nameof(FontSize),
        typeof(double),
        typeof(MultiSelectPicker),
        14.0);

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(
            nameof(FontFamily),
            typeof(string),
            typeof(MultiSelectPicker),
            default(string));

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public static readonly BindableProperty ItemsSourceProperty =
                        BindableProperty.Create(
                            nameof(ItemsSource),
                            typeof(IEnumerable<IMultiSelectItem>),
                            typeof(MultiSelectPicker),
                            default(IEnumerable<IMultiSelectItem>),
                            propertyChanged: OnItemsSourceChanged);

    public IEnumerable<IMultiSelectItem> ItemsSource
    {
        get => (IEnumerable<IMultiSelectItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(BindableObject bindable,
                                            object oldValue,
                                            object newValue)
    {
        if (bindable is not MultiSelectPicker msp ||
            newValue is not IEnumerable<IMultiSelectItem> items)
        {
            return;
        }

        foreach (var item in items)
        {
            item.PropertyChanged += (_, __) => msp.RefreshHeader();
        }

        msp.RefreshHeader();
        return;
    }
    private void RefreshHeader()
    {
        if (ItemsSource == null)
        {
            SelectedText.Text = Prompt;
            return;
        }

        var selected = ItemsSource?
            .Where(x => x.IsChecked)
            .ToList()
            ?? new List<IMultiSelectItem>();

        SelectedText.Text =
            selected.Count == 0
                ? Prompt
                : string.Join(", ", selected.Select(x => x.Content));
        return;
    }
    public static Point GetAbsolutePosition(VisualElement view)
    {
        double x = view.X;
        double y = view.Y;

        Element current = view.Parent;

        while (current is VisualElement ve)
        {
            x += ve.X + ve.TranslationX;
            y += ve.Y + ve.TranslationY;
            current = current.Parent;
        }
        return new Point(x, y);
    }
}