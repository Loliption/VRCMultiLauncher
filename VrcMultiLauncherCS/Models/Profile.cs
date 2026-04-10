using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using VrcMultiLauncherCS.Services;

namespace VrcMultiLauncherCS.Models
{
    public class CustomOption : INotifyPropertyChanged
    {
        private bool _enabled = true;
        private string _arg = "";
        private string _desc = "";

        [JsonProperty("enabled")]
        public bool Enabled
        {
            get => _enabled;
            set { _enabled = value; OnPropertyChanged(); }
        }

        [JsonProperty("arg")]
        public string Arg
        {
            get => _arg;
            set { _arg = value; OnPropertyChanged(); }
        }

        [JsonProperty("desc")]
        public string Desc
        {
            get => _desc;
            set { _desc = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class Profile : INotifyPropertyChanged
    {
        private string _name = "Instance";
        private int _profileId;
        private bool _noVr = true;
        private bool _oscEnable = true;
        private int _oscIn = 9000;
        private int _oscOut = 9001;
        private bool _windowed = true;
        private int _width = 1280;
        private int _height = 720;
        private int _fps = 60;
        private List<CustomOption> _customOptions = new();
        private int? _lastPid;

        [JsonProperty("name")]
        public string Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        [JsonProperty("profile_id")]
        public int ProfileId
        {
            get => _profileId;
            set { _profileId = value; OnPropertyChanged(); }
        }

        [JsonProperty("no_vr")]
        public bool NoVr
        {
            get => _noVr;
            set { _noVr = value; OnPropertyChanged(); }
        }

        [JsonProperty("osc_enable")]
        public bool OscEnable
        {
            get => _oscEnable;
            set { _oscEnable = value; OnPropertyChanged(); OnPropertyChanged(nameof(OscDisplay)); }
        }

        [JsonProperty("osc_in")]
        public int OscIn
        {
            get => _oscIn;
            set { _oscIn = value; OnPropertyChanged(); OnPropertyChanged(nameof(OscDisplay)); }
        }

        [JsonProperty("osc_out")]
        public int OscOut
        {
            get => _oscOut;
            set { _oscOut = value; OnPropertyChanged(); OnPropertyChanged(nameof(OscDisplay)); }
        }

        [JsonProperty("windowed")]
        public bool Windowed
        {
            get => _windowed;
            set { _windowed = value; OnPropertyChanged(); }
        }

        [JsonProperty("width")]
        public int Width
        {
            get => _width;
            set { _width = value; OnPropertyChanged(); }
        }

        [JsonProperty("height")]
        public int Height
        {
            get => _height;
            set { _height = value; OnPropertyChanged(); }
        }

        [JsonProperty("fps")]
        public int Fps
        {
            get => _fps;
            set { _fps = value; OnPropertyChanged(); }
        }

        [JsonProperty("custom_options")]
        public List<CustomOption> CustomOptions
        {
            get => _customOptions;
            set { _customOptions = value; OnPropertyChanged(); }
        }

        [JsonIgnore]
        public int? LastPid
        {
            get => _lastPid;
            set
            {
                _lastPid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsRunning));
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(PidDisplay));
            }
        }

        [JsonIgnore]
        public bool IsRunning => _lastPid.HasValue;

        [JsonIgnore]
        public string Status => _lastPid.HasValue
            ? LocalizationService.Instance.Running
            : LocalizationService.Instance.Stopped;

        public void NotifyStatusChanged() => OnPropertyChanged(nameof(Status));

        [JsonIgnore]
        public string PidDisplay => _lastPid.HasValue ? _lastPid.ToString() : "-";

        [JsonIgnore]
        public string OscDisplay => OscEnable ? $"{OscIn}/{OscOut}" : "OFF";

        /// <summary>別プロファイルの値をすべてコピーします（LastPid は除く）。</summary>
        public void ApplyFrom(Profile source)
        {
            Name = source.Name;
            ProfileId = source.ProfileId;
            NoVr = source.NoVr;
            OscEnable = source.OscEnable;
            OscIn = source.OscIn;
            OscOut = source.OscOut;
            Windowed = source.Windowed;
            Width = source.Width;
            Height = source.Height;
            Fps = source.Fps;
            CustomOptions = source.CustomOptions;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class AppSettings
    {
        [JsonProperty("vrchat_path")]
        public string VrcPath { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe";

        [JsonProperty("profiles")]
        public List<Profile> Profiles { get; set; } = new List<Profile>();

        [JsonProperty("language")]
        public string Language { get; set; } = "en";
    }
}
