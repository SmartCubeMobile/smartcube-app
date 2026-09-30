#if XAMARIN
using Xamarin.Forms;
#endif

namespace SmartCubeMobile
{
#if XAMARIN
    public class ExtendedCarousel : CarouselView
    {
        public ExtendedCarousel()
        {
            HorizontalScrollBarVisibility = ScrollBarVisibility.Never;
            VerticalScrollBarVisibility = ScrollBarVisibility.Never;

            IsScrollAnimated = false;

            this.ItemsLayout = new LinearItemsLayout(ItemsLayoutOrientation.Horizontal)
            {
                SnapPointsAlignment = SnapPointsAlignment.Center,
                SnapPointsType = SnapPointsType.MandatorySingle
            };

            this.PositionChanged += ExtendedCarousel_PositionChanged;
        }

        private void ExtendedCarousel_PositionChanged(object sender, PositionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
#endif
}