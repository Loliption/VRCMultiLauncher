using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Linq;
using VrcMultiLauncherCS.Models;
using VrcMultiLauncherCS.Services;

namespace VrcMultiLauncherCS
{
    public sealed partial class ProfileDialog : ContentDialog
    {
        private readonly ObservableCollection<CustomOption> _args = new();

        // Exposed for {x:Bind} in XAML
        public LocalizationService Loc => LocalizationService.Instance;

        public ProfileDialog(Profile profile, string title, XamlRoot xamlRoot)
        {
            this.InitializeComponent();
            this.Title = title;
            this.XamlRoot = xamlRoot;

            LoadFrom(profile);
            this.PrimaryButtonClick += (_, _) => ApplyTo(profile);
        }

        private void LoadFrom(Profile p)
        {
            NameBox.Text = p.Name;
            IdBox.Value = p.ProfileId;
            NoVrCheck.IsChecked = p.NoVr;

            // Python版に合わせ: フルスクリーンチェック = Windowed の逆
            FullscreenCheck.IsChecked = !p.Windowed;
            WidthBox.Value = p.Width;
            HeightBox.Value = p.Height;
            FpsBox.Value = p.Fps;

            OscCheck.IsChecked = p.OscEnable;
            OscInBox.Value = p.OscIn;
            OscOutBox.Value = p.OscOut;
            OscPortPanel.Visibility = p.OscEnable ? Visibility.Visible : Visibility.Collapsed;

            _args.Clear();
            foreach (var opt in p.CustomOptions)
                _args.Add(new CustomOption { Enabled = opt.Enabled, Arg = opt.Arg, Desc = opt.Desc });

            ArgsList.ItemsSource = _args;
        }

        private void ApplyTo(Profile p)
        {
            p.Name = NameBox.Text;
            p.ProfileId = (int)IdBox.Value;
            p.NoVr = NoVrCheck.IsChecked ?? false;

            p.Windowed = !(FullscreenCheck.IsChecked ?? false);
            p.Width = (int)WidthBox.Value;
            p.Height = (int)HeightBox.Value;
            p.Fps = (int)FpsBox.Value;

            p.OscEnable = OscCheck.IsChecked ?? false;
            p.OscIn = (int)OscInBox.Value;
            p.OscOut = (int)OscOutBox.Value;

            p.CustomOptions = _args.ToList();
        }

        private void OscCheck_Changed(object sender, RoutedEventArgs e)
        {
            OscPortPanel.Visibility = OscCheck.IsChecked == true
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void OnAddArg_Click(object sender, RoutedEventArgs e)
        {
            _args.Add(new CustomOption { Enabled = true });
        }

        private void OnRemoveArg_Click(object sender, RoutedEventArgs e)
        {
            if (ArgsList.SelectedItem is CustomOption selected)
                _args.Remove(selected);
        }
    }
}
