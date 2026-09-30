namespace SmartCubeMobile.Dashboard
{
    public static class AnimationHelper
    {
        public static async Task StaggerIn(Layout container, uint staggerDelay = 70, uint duration = 400)
        {
            var children = container.Children.OfType<View>().ToList();
            foreach (var child in children)
            {
                child.Opacity = 0;
                child.TranslationY = 24;
            }

            foreach (var child in children)
            {
                _ = child.FadeTo(1, duration, Easing.CubicOut);
                _ = child.TranslateTo(0, 0, duration, Easing.CubicOut);
                await Task.Delay((int)staggerDelay);
            }
        }

        public static async Task SlideInFromRight(View view, uint duration = 400)
        {
            view.Opacity = 0;
            view.TranslationX = 60;
            await Task.WhenAll(
                view.FadeTo(1, duration, Easing.CubicOut),
                view.TranslateTo(0, 0, duration, Easing.CubicOut)
            );
        }

        public static async Task FadeIn(View view, uint delay = 0, uint duration = 350)
        {
            view.Opacity = 0;
            if (delay > 0) await Task.Delay((int)delay);
            await view.FadeTo(1, duration, Easing.CubicOut);
        }

        public static async Task ScaleIn(View view, uint delay = 0, uint duration = 350)
        {
            view.Opacity = 0;
            view.Scale = 0.85;
            if (delay > 0) await Task.Delay((int)delay);
            await Task.WhenAll(
                view.FadeTo(1, duration, Easing.CubicOut),
                view.ScaleTo(1.0, duration, Easing.SpringOut)
            );
        }

        public static async Task PulseValue(Label label)
        {
            await label.ScaleTo(1.08, 250, Easing.CubicOut);
            await label.ScaleTo(1.0, 250, Easing.CubicIn);
        }

        public static void PrepareForEntry(params View[] views)
        {
            foreach (var v in views)
            {
                v.Opacity = 0;
                v.TranslationY = 20;
            }
        }

        public static async Task AnimateEntry(View view, uint delay = 0, uint duration = 400)
        {
            if (delay > 0) await Task.Delay((int)delay);
            await Task.WhenAll(
                view.FadeTo(1, duration, Easing.CubicOut),
                view.TranslateTo(0, 0, duration, Easing.CubicOut)
            );
        }
    }
}
