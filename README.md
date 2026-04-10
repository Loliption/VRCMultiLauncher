# VRChat Multi Launcher

A Windows desktop application for launching and managing multiple VRChat instances simultaneously.

> **日本語の説明は下部にあります / Japanese documentation below.**

## Features

- **Multiple Instances** — launch several VRChat clients in parallel, each with its own profile ID
- **Per-profile Settings** — configure resolution, FPS, fullscreen/windowed mode, and Desktop (No-VR) mode per instance
- **OSC Support** — assign dedicated OSC receive/send ports to each instance
- **Custom Launch Arguments** — add, enable/disable, and annotate arbitrary launch flags per profile
- **Process Monitoring** — tracks the VRChat.exe PID for each running instance (Running/Stopped status, 2.5-second refresh)
- **Kill Process** — terminate a running instance directly from the UI
- **Language Switching** — toggle between English and Japanese from the toolbar

## Requirements

- Windows 10 version 1809 (build 17763) or later
- [Windows App Runtime 1.7](https://aka.ms/windowsappsdk/1.7/latest/windowsappruntimeinstall-x64.exe) installed

## Installation

1. Download the latest release ZIP and extract it anywhere.
2. If not yet installed, run `windowsappruntimeinstall-x64.exe` first (link above).
3. Run `VrcMultiLauncherCS.exe`.

> The `libs/` folder must remain in the same directory as the EXE.

## Usage

1. **Set the executable path** — click **Browse** and select `launch.exe` in your VRChat installation folder.  
   Default Steam path: `C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe`

2. **Create a profile** — click **New**, fill in the settings, and click **Save**.

3. **Launch** — select a profile and click **Launch**.  
   The status column shows **Running** once VRChat.exe is detected.

4. **Kill** — select a running profile and click **Kill Process** to terminate it.

5. **Switch language** — click the **日本語 / English** button in the top-right of the toolbar.

### Profile Settings

| Field | Description |
|-------|-------------|
| Name | Display name for this profile |
| ID | VRChat `--profile=N` index (must be unique per instance) |
| No VR (Desktop Mode) | Adds `--no-vr` to launch in desktop mode |
| Launch Fullscreen | Toggles `-screen-fullscreen 1/0` |
| Width / Height | Window resolution |
| FPS | Target framerate via `--fps=N` |
| Enable OSC | Enables OSC with the specified ports |
| Receive (In) / Send (Out) | UDP ports for OSC communication |
| Launch Arguments | Extra flags appended to the command line |

### OSC Port Assignment

Default auto-assignment:
- Profile 0: In `9000`, Out `9001`
- Profile 1: In `9010`, Out `9011`
- Profile N: In `9000 + N×10`, Out `9001 + N×10`

## Settings File

Profiles, executable path, and language preference are saved to `settings.json` in the same directory as the EXE.

## License

This project is released under the [MIT License](LICENSE).

---

# VRChat Multi Launcher（日本語）

複数の VRChat インスタンスを同時に起動・管理するための Windows デスクトップアプリケーションです。

## 機能

- **複数インスタンス起動** — 各インスタンスに固有のプロファイル ID を割り当てて複数の VRChat クライアントを並行起動
- **プロファイルごとの設定** — 解像度・FPS・フルスクリーン/ウィンドウモード・デスクトップモード（No VR）をプロファイルごとに設定
- **OSC サポート** — インスタンスごとに専用の OSC 受信/送信ポートを割り当て
- **カスタム起動引数** — 任意の起動フラグを追加・有効/無効切り替え・メモ付きで管理
- **プロセス監視** — 各インスタンスの VRChat.exe PID を追跡し、実行中/停止中を 2.5 秒ごとに更新
- **強制終了** — UI から実行中のインスタンスを直接終了
- **言語切り替え** — ツールバーのボタンで日本語/英語を切り替え（設定は自動保存）

## 動作要件

- Windows 10 バージョン 1809（ビルド 17763）以降
- [Windows App Runtime 1.7](https://aka.ms/windowsappsdk/1.7/latest/windowsappruntimeinstall-x64.exe) のインストール

## インストール

1. 最新リリースの ZIP をダウンロードして任意の場所に展開します。
2. Windows App Runtime 未インストールの場合は `windowsappruntimeinstall-x64.exe` を先に実行してください（上記リンク）。
3. `VrcMultiLauncherCS.exe` を起動します。

> `libs/` フォルダは EXE と同じディレクトリに置いてください。

## 使い方

1. **実行ファイルの設定** — **変更** ボタンをクリックし、VRChat インストールフォルダ内の `launch.exe` を選択します。  
   Steam デフォルトパス: `C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe`

2. **プロファイルの作成** — **新規作成** をクリックして設定を入力し、**保存** をクリックします。

3. **起動** — プロファイルを選択して **起動する** をクリックします。  
   VRChat.exe が検出されると状態列に **実行中** と表示されます。

4. **強制終了** — 実行中のプロファイルを選択して **停止 / 強制終了** をクリックします。

5. **言語の切り替え** — ツールバー右端の **日本語 / English** ボタンをクリックします。

### プロファイル設定項目

| 項目 | 説明 |
|------|------|
| 名前 | プロファイルの表示名 |
| ID | VRChat の `--profile=N` インデックス（インスタンスごとに一意にすること） |
| VRなし（デスクトップモード） | `--no-vr` を追加してデスクトップモードで起動 |
| フルスクリーンで起動 | `-screen-fullscreen 1/0` を切り替え |
| 幅 / 高さ | ウィンドウ解像度 |
| FPS | `--fps=N` によるターゲットフレームレート |
| OSCを有効化 | 指定ポートで OSC を有効化 |
| 受信（In） / 送信（Out） | OSC 通信用 UDP ポート |
| 追加起動引数 | コマンドラインに追加する任意フラグ |

### OSC ポートの自動割り当て

- プロファイル 0: 受信 `9000`、送信 `9001`
- プロファイル 1: 受信 `9010`、送信 `9011`
- プロファイル N: 受信 `9000 + N×10`、送信 `9001 + N×10`

## 設定ファイル

プロファイル・実行ファイルパス・言語設定は EXE と同じディレクトリの `settings.json` に保存されます。

## ライセンス

このプロジェクトは [MIT ライセンス](LICENSE) のもとで公開されています。
