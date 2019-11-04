using AsyncAwaitBestPractices;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamConcentricOnboarding.Helpers;

namespace XamConcentricOnboarding.Views.Controls
{
    public sealed class ConcentricOnboardView : SKCanvasView
    {
        #region Bindable Properties

        public static readonly BindableProperty ButtonDiameterProperty =
           BindableProperty.Create(nameof(ButtonDiameter), typeof(int), typeof(ConcentricOnboardView),
               defaultValue: 200, propertyChanged: OnButtonSizeChanged);

        public static readonly BindableProperty CommandProperty =
           BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(ConcentricOnboardView),
               defaultValue: default(ICommand));

        #endregion

        #region Fields

        private SKRect buttonRect = new SKRect();
        private SKColor backgroundColor;
        private SKColor buttonColor;
        private SKSurface surface;
        private SKImageInfo info;
        private SKCanvas canvas;

        private double limit = 15;
        private double timerStep = 0.02;
        private double duration = 1;
        private double CurrentProgress = 1;

        private double step
        {
            get => 2 * limit / (duration / timerStep);
        }

        #endregion

        #region Properties

        public event EventHandler<EventArgs> OnAnimationStart;

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public int ButtonDiameter
        {
            get => (int)GetValue(ButtonDiameterProperty);
            set => SetValue(ButtonDiameterProperty, value);
        }

        public bool IsBusy
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        public ConcentricOnboardView()
        {
            buttonColor = SKColorHelper.GetRandomColor(RandomMode.LightOnly);
            backgroundColor = SKColorHelper.GetRandomColor(RandomMode.LightOnly);
            EnableTouchEvents = true;

        }

        #endregion

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            info = e.Info;
            surface = e.Surface;
            canvas = surface.Canvas;

            canvas.Clear();

            var left = (info.Width - ButtonDiameter) / 2;
            var right = left + ButtonDiameter;
            var top = (info.Height - ButtonDiameter - 100);
            var bottom = info.Height - (100 / ButtonDiameter);
            buttonRect = new SKRect(left, top, right, bottom);

            DrawBackground();
            DrawButton();
        }

        protected override void OnTouch(SKTouchEventArgs args)
        {
            var point = args.Location;

            switch (args.ActionType)
            {
                case SKTouchAction.Pressed:
                    if (buttonRect.Contains(point))
                        ActionButtonClicked();
                    break;

                case SKTouchAction.Moved:
                case SKTouchAction.Released:
                case SKTouchAction.Cancelled:
                    break;
            }

            base.OnTouch(args);
        }

        private void DrawButton()
        {
            var spacing = 100;
            var radius = ButtonDiameter / 2;

            var grow = CurrentProgress < limit;
            float r = Convert.ToSingle(radius + Math.Pow(2, ((grow ? CurrentProgress : limit - (CurrentProgress - limit)))));
            float delta = Convert.ToSingle((1 - CurrentProgress / limit) * radius);
            float d = r * 2;

            var left = info.Width / 2 - delta;
            var right = grow ? left + d : left - d;
            var bottom = info.Height - (spacing / ButtonDiameter);

            var yPoint = bottom - radius - spacing;
            var direction = grow ?
                SKPathDirection.Clockwise : SKPathDirection.CounterClockwise;

            using (var paint = GetPaint(grow ? buttonColor : backgroundColor))
            {
                using (SKPath skPath = new SKPath())
                {
                    skPath.MoveTo(left, yPoint);
                    skPath.ArcTo(r, r, 0f, SKPathArcSize.Large, direction, right, yPoint);
                    skPath.ArcTo(r, r, 0f, SKPathArcSize.Large, direction, left, yPoint);
                    skPath.Close();

                    canvas.DrawPath(skPath, paint);
                }
            }
        }

        private void DrawBackground()
        {
            var isGrowing = CurrentProgress < limit;
            using (var paint = GetPaint(isGrowing ? backgroundColor : buttonColor))
            {
                using (SKPath skPath = new SKPath())
                {
                    skPath.MoveTo(0, 0);
                    skPath.LineTo(info.Width, 0);
                    skPath.LineTo(info.Width, info.Height);
                    skPath.LineTo(0, info.Height);
                    skPath.Close();

                    canvas.DrawPath(skPath, paint);
                }
            }
        }

        private SKPaint GetPaint(SKColor colorToFill)
        {
            return new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = colorToFill,
                StrokeWidth = 5,
                IsAntialias = true
            };
        }

        private static void OnButtonSizeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is ConcentricOnboardView view)
            {
                view.InvalidateSurface();
            }
        }

        private void ActionButtonClicked()
        {
            if (IsBusy) return;
            IsBusy = true;

            buttonColor = SKColorHelper.GetRandomColor(RandomMode.LightOnly);
            InvalidateSurface();

            ExecuteAnimationAsync()
                .SafeFireAndForget(onException: ex => Console.WriteLine(ex));

            if (Command?.CanExecute(this) ?? false)
                Command?.Execute(this);
        }

        private async Task ExecuteAnimationAsync()
        {
            OnAnimationStart?.Invoke(this, EventArgs.Empty);

            while (CurrentProgress < 2 * limit)
            {
                CurrentProgress += step;
                await Task.Delay((int)(timerStep * 1000));
                InvalidateSurface();
            }

            var color = buttonColor;
            var bgColor = backgroundColor;

            backgroundColor = color;
            buttonColor = bgColor;
            CurrentProgress = 0;
            IsBusy = false;
        }
    }
}
