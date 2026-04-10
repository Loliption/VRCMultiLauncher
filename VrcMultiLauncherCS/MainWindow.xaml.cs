using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Newtonsoft.Json;
using Windows.Graphics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VrcMultiLauncherCS.Models;
using VrcMultiLauncherCS.Services;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace VrcMultiLauncherCS
{
    public sealed partial class MainWindow : Window
    {
        private AppSettings _settings;
        private ObservableCollection<Profile> _profiles;
        private DispatcherTimer _timer;

        private const string SETTINGS_FILE = "settings.json";

        // Exposed for {x:Bind} in XAML
        public LocalizationService Loc => LocalizationService.Instance;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "VRChat Multi Launcher";
            this.AppWindow.Resize(new SizeInt32(1100, 680));

            LoadSettings();

            LocalizationService.LanguageChanged += OnLanguageChanged;

            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2.5) };
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        // ── Settings ────────────────────────────────────────────────────────

        private void LoadSettings()
        {
            if (File.Exists(SETTINGS_FILE))
            {
                try
                {
                    _settings = JsonConvert.DeserializeObject<AppSettings>(
                        File.ReadAllText(SETTINGS_FILE));
                }
                catch { _settings = new AppSettings(); }
            }
            else
            {
                _settings = new AppSettings();
            }

            if (_settings.Profiles == null) _settings.Profiles = new List<Profile>();

            Loc.Language = _settings.Language ?? "en";

            _profiles = new ObservableCollection<Profile>(_settings.Profiles);
            ProfileList.ItemsSource = _profiles;
            PathBox.Text = _settings.VrcPath ?? "";
        }

        private void SaveSettings()
        {
            _settings.VrcPath = PathBox.Text;
            _settings.Profiles = _profiles.ToList();
            _settings.Language = Loc.Language;
            File.WriteAllText(SETTINGS_FILE,
                JsonConvert.SerializeObject(_settings, Formatting.Indented));
        }

        // ── Language toggle ──────────────────────────────────────────────────

        private void OnToggleLang_Click(object sender, RoutedEventArgs e)
        {
            Loc.Language = Loc.Language == "en" ? "ja" : "en";
            SaveSettings();
        }

        private void OnLanguageChanged()
        {
            // Refresh Status / Running / Stopped display for all profiles
            foreach (var p in _profiles)
                p.NotifyStatusChanged();
        }

        // ── Timer (process monitoring) ───────────────────────────────────────

        private void Timer_Tick(object sender, object e)
        {
            foreach (var p in _profiles)
            {
                if (!p.LastPid.HasValue) continue;
                try { Process.GetProcessById(p.LastPid.Value); }
                catch { p.LastPid = null; }
            }
        }

        // ── Path ─────────────────────────────────────────────────────────────

        private async void OnBrowsePath_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            picker.FileTypeFilter.Add(".exe");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                PathBox.Text = file.Path;
                SaveSettings();
            }
        }

        // ── Launch / Kill ────────────────────────────────────────────────────

        private void OnLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileList.SelectedItem is Profile p)
                LaunchProfile(p);
        }

        private void LaunchProfile(Profile p)
        {
            string exe = PathBox.Text;
            if (!File.Exists(exe))
            {
                StatusText.Text = Loc.ExeNotFound;
                return;
            }

            var args = new List<string>();
            if (p.NoVr) args.Add("--no-vr");
            args.Add($"--profile={p.ProfileId}");
            if (p.OscEnable) args.Add($"--osc={p.OscIn}:127.0.0.1:{p.OscOut}");

            args.Add(p.Windowed ? "-screen-fullscreen 0" : "-screen-fullscreen 1");
            args.Add($"-screen-width {p.Width}");
            args.Add($"-screen-height {p.Height}");
            args.Add($"--fps={p.Fps}");

            foreach (var opt in p.CustomOptions)
            {
                if (opt.Enabled && !string.IsNullOrWhiteSpace(opt.Arg))
                    args.AddRange(opt.Arg.Trim().Split(' ',
                        StringSplitOptions.RemoveEmptyEntries));
            }

            try
            {
                var psi = new ProcessStartInfo(exe, string.Join(" ", args))
                {
                    WorkingDirectory = Path.GetDirectoryName(exe)
                };
                var beforePids = Process.GetProcessesByName("VRChat")
                                        .Select(x => x.Id).ToHashSet();
                Process.Start(psi);
                StatusText.Text = Loc.LaunchingMsg(p.Name);
                _ = WaitForVrChatAsync(p, beforePids);
            }
            catch (Exception ex)
            {
                StatusText.Text = Loc.ErrorMsg(ex.Message);
            }
        }

        private async Task WaitForVrChatAsync(Profile p, HashSet<int> beforePids)
        {
            var deadline = DateTime.Now.AddSeconds(60);
            while (DateTime.Now < deadline)
            {
                await Task.Delay(1000);
                var newPid = Process.GetProcessesByName("VRChat")
                                    .Select(x => x.Id)
                                    .FirstOrDefault(id => !beforePids.Contains(id));
                if (newPid != 0)
                {
                    DispatcherQueue.TryEnqueue(() =>
                    {
                        p.LastPid = newPid;
                        StatusText.Text = Loc.LaunchedMsg(p.Name, newPid);
                    });
                    return;
                }
            }
            DispatcherQueue.TryEnqueue(() =>
                StatusText.Text = Loc.VrcNotFoundMsg(p.Name));
        }

        private void OnKill_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileList.SelectedItem is Profile p && p.LastPid.HasValue)
            {
                try
                {
                    Process.GetProcessById(p.LastPid.Value).Kill();
                    StatusText.Text = Loc.KilledMsg(p.LastPid);
                }
                catch
                {
                    StatusText.Text = Loc.AlreadyTerminatedMsg;
                }
                finally
                {
                    p.LastPid = null;
                }
            }
        }

        // ── Profile management ───────────────────────────────────────────────

        private async void OnAdd_Click(object sender, RoutedEventArgs e)
        {
            int newId = _profiles.Any() ? _profiles.Max(x => x.ProfileId) + 1 : 0;
            var p = new Profile
            {
                Name = $"Instance {newId}",
                ProfileId = newId,
                OscIn = 9000 + newId * 10,
                OscOut = 9001 + newId * 10
            };

            var dialog = new ProfileDialog(p, Loc.NewProfileTitle, this.Content.XamlRoot);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _profiles.Add(p);
                SaveSettings();
            }
        }

        private async void OnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileList.SelectedItem is not Profile original) return;

            var clone = JsonConvert.DeserializeObject<Profile>(
                JsonConvert.SerializeObject(original));
            clone.LastPid = original.LastPid;

            var dialog = new ProfileDialog(clone, Loc.EditProfileTitle, this.Content.XamlRoot);
            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                original.ApplyFrom(clone);
                SaveSettings();
            }
        }

        private void ProfileList_DoubleTapped(object sender,
            Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            OnEdit_Click(sender, null);
        }

        private async void OnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (ProfileList.SelectedItem is not Profile p) return;

            var confirm = new ContentDialog
            {
                XamlRoot = this.Content.XamlRoot,
                Title = Loc.ConfirmDeleteTitle,
                Content = Loc.DeleteConfirmMsg(p.Name),
                PrimaryButtonText = Loc.DeleteDialogBtn,
                CloseButtonText = Loc.CancelBtn,
                DefaultButton = ContentDialogButton.Close
            };

            var result = await confirm.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                _profiles.Remove(p);
                SaveSettings();
            }
        }
    }
}
