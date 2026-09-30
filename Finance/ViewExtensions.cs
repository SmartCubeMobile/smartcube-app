namespace SmartCubeMobile;

public static class ViewExtensions
{
    public static Rect GetBoundsRelativeTo(
    this VisualElement view,
    VisualElement ancestor)
    {
        double x = view.X;
        double y = view.Y;

        Element current = view.Parent;

        while (current != null && current != ancestor)
        {
            if (current is VisualElement ve)
            {
                x += ve.X;
                y += ve.Y;
            }

            current = current.Parent;
        }
        return new Rect(x, y, view.Width, view.Height);
    }
}