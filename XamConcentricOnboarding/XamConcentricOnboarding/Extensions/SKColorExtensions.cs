using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using Xamarin.Forms;

namespace XamConcentricOnboarding.Helpers
{
    public static class SKColorHelper
    {
        private static readonly Random _random = new Random();

        public static SKColor GetRandomColor(RandomMode mode = RandomMode.Default) =>
            Color.FromRgb(
                RandomRgb(mode == RandomMode.LightOnly ? 200 : 0),
                RandomRgb(mode == RandomMode.LightOnly ? 150 : 0), 
                RandomRgb(mode == RandomMode.LightOnly ? 150 : 0)).ToSKColor();

        private static int RandomRgb(int minVal = 0) =>
            _random.Next(minVal, 255);
    }

    public enum RandomMode
    {
        Default = 0,
        LightOnly = 1
    }
}
