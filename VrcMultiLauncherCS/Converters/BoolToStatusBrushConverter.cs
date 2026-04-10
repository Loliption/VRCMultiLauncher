using Microsoft.UI;
using Microsoft.UI.Xaml.Media;

namespace VrcMultiLauncherCS.Converters
{
    /// <summary>
    /// x:Bind の静的メソッド呼び出し用ヘルパー。
    /// Window は FrameworkElement でないため StaticResource コンバーターが使えないので
    /// {x:Bind converters:StatusBrush.Get(IsRunning), Mode=OneWay} で呼び出す。
    /// </summary>
    public static class StatusBrush
    {
        private static readonly SolidColorBrush Running =
            new(ColorHelper.FromArgb(255, 0x4C, 0xC2, 0xFF)); // #4CC2FF

        private static readonly SolidColorBrush Stopped =
            new(ColorHelper.FromArgb(255, 0x80, 0x80, 0x80)); // Gray

        public static SolidColorBrush Get(bool isRunning) =>
            isRunning ? Running : Stopped;
    }
}
