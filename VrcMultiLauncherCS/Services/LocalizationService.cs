using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VrcMultiLauncherCS.Services
{
    public class LocalizationService : INotifyPropertyChanged
    {
        public static LocalizationService Instance { get; } = new();
        private LocalizationService() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public static event Action LanguageChanged;

        private string _lang = "en";
        public string Language
        {
            get => _lang;
            set
            {
                if (_lang == value) return;
                _lang = value;
                // Raise for all properties (empty string = all)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
                LanguageChanged?.Invoke();
            }
        }

        private bool IsJa => _lang == "ja";

        // ── MainWindow ──────────────────────────────────────────────────────

        public string ExeLabel       => IsJa ? "実行ファイル:" : "Executable:";
        public string ExePlaceholder => IsJa ? "launch.exe のパス" : "Path to launch.exe";
        public string BrowseBtn      => IsJa ? "変更" : "Browse";

        public string ColName    => IsJa ? "名前" : "Name";
        public string ColStatus  => IsJa ? "状態" : "Status";

        public string ControlsLabel => IsJa ? "コントロール" : "Controls";
        public string LaunchBtn     => IsJa ? "起動する" : "Launch";
        public string KillBtn       => IsJa ? "停止 / 強制終了" : "Kill Process";

        public string ProfilesLabel => IsJa ? "プロファイル" : "Profiles";
        public string NewBtn        => IsJa ? "新規作成" : "New";
        public string EditBtn       => IsJa ? "編集" : "Edit";
        public string DeleteBtn     => IsJa ? "削除" : "Delete";

        public string LangToggleBtn => IsJa ? "English" : "日本語";

        // ── Profile status ───────────────────────────────────────────────────

        public string Running => IsJa ? "実行中" : "Running";
        public string Stopped => IsJa ? "停止中" : "Stopped";

        // ── ProfileDialog ────────────────────────────────────────────────────

        public string SaveBtn   => IsJa ? "保存" : "Save";
        public string CancelBtn => IsJa ? "キャンセル" : "Cancel";

        public string BasicInfoLabel  => IsJa ? "基本情報" : "Basic Info";
        public string NameHeader      => IsJa ? "名前" : "Name";
        public string NamePlaceholder => IsJa ? "例: アバター確認用" : "e.g. Avatar Test";
        public string NoVrLabel       => IsJa ? "VRなし（デスクトップモード）" : "No VR (Desktop Mode)";

        public string DisplayLabel    => IsJa ? "画面設定" : "Display";
        public string FullscreenLabel => IsJa ? "フルスクリーンで起動" : "Launch Fullscreen";
        public string WidthHeader     => IsJa ? "幅" : "Width";
        public string HeightHeader    => IsJa ? "高さ" : "Height";

        public string OscLabel      => IsJa ? "OSC 通信" : "OSC";
        public string EnableOscLabel => IsJa ? "OSCを有効化" : "Enable OSC";
        public string OscInHeader   => IsJa ? "受信（In）" : "Receive (In)";
        public string OscOutHeader  => IsJa ? "送信（Out）" : "Send (Out)";

        public string ArgsLabel       => IsJa ? "追加起動引数" : "Launch Arguments";
        public string ColOn           => IsJa ? "有効" : "On";
        public string ColArgument     => IsJa ? "引数" : "Argument";
        public string ColNote         => IsJa ? "メモ" : "Note";
        public string NotePlaceholder => IsJa ? "メモ" : "note";
        public string AddArgBtn       => IsJa ? "＋ 追加" : "+ Add";
        public string RemoveArgBtn    => IsJa ? "－ 削除" : "- Remove";

        // ── Dialog titles / code-behind strings ──────────────────────────────

        public string NewProfileTitle    => IsJa ? "新規プロファイル" : "New Profile";
        public string EditProfileTitle   => IsJa ? "プロファイルを編集" : "Edit Profile";
        public string ConfirmDeleteTitle => IsJa ? "削除の確認" : "Confirm Delete";
        public string DeleteConfirmMsg(string name)
            => IsJa ? $"「{name}」を削除しますか？" : $"Delete \"{name}\"?";
        public string DeleteDialogBtn => IsJa ? "削除" : "Delete";

        public string ExeNotFound => IsJa ? "実行ファイルが見つかりません。" : "Executable not found.";
        public string LaunchingMsg(string name)
            => IsJa ? $"{name} を起動中... (VRChat.exe を待機)" : $"Launching {name}... (waiting for VRChat.exe)";
        public string LaunchedMsg(string name, int pid)
            => IsJa ? $"{name} を起動しました (VRChat.exe PID: {pid})" : $"{name} launched (VRChat.exe PID: {pid})";
        public string VrcNotFoundMsg(string name)
            => IsJa ? $"{name}: VRChat.exe が見つかりませんでした。" : $"{name}: VRChat.exe not found.";
        public string KilledMsg(int? pid)
            => IsJa ? $"PID {pid} を終了しました。" : $"Killed PID {pid}.";
        public string AlreadyTerminatedMsg
            => IsJa ? "プロセスはすでに終了しています。" : "Process already terminated.";
        public string ErrorMsg(string msg)
            => IsJa ? $"エラー: {msg}" : $"Error: {msg}";
    }
}
