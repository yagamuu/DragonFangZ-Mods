# DFZ Enabled State Serializer Mod
ドラゴンファングZにてステートセーブ/ロード機能を追加するModです。
**このMODはあくまで検証/練習用にのみ使用してください。RTA中などに使用するのは禁止されています。**
**また、動画などで当Modを使用する際は使用している事を明記することを強く推奨します。**

## 使い方
1. MelonLoaderのインストーラーを[ダウンロードする](https://github.com/LavaGang/MelonLoader.Installer/releases/latest/download/MelonLoader.Installer.exe)

2. ダウンロードしたインストーラーを起動し、`Unity Game:`の横の`SELECT`ボタンを押し、ドラゴンファングZのexeファイルを選択する(インストール先はSteam内でプロパティ→ローカルファイルを閲覧などで確認できる)

3. Version横の`Latest`チェックマークを外し、`v0.5.7`を選択する

4. 一番下の`INSTALL`ボタンを押す

5. ゲームフォルダ内のModsフォルダに[`DFZEnabledStateSerializerMod.dll`](https://github.com/yagamuu/speedrun/blob/master/DragonFangZ/Mod/DFZEnabledStateSerializerMod/DFZEnabledStateSerializerMod.dll)を入れる

6. ゲーム中にF1~F10キーを押すとステートロード、Shiftキーを同時に押すとステートセーブを行う

7. 希望に応じてオプションを変更してください
    1. ゲームフォルダ内の`UserData\MelonPreferences.cfg`をテキストエディタで開き書き換えることで変更可能です。
    2. よくわからない人は[`MelonPrefManager.Mono.dll`](https://github.com/yagamuu/DragonFangZ-Mods/blob/master/DFZForceFangEquipMod/MelonPrefManager.Mono.dll)と[`UniverseLib.Mono.dll`](https://github.com/yagamuu/DragonFangZ-Mods/blob/master/DFZForceFangEquipMod/UniverseLib.Mono.dll)をModsフォルダに入れてF5キーを押して設定してください(From [MelonPreferencesManager](https://github.com/kafeijao/MelonPreferencesManager))。

### オプションの指定例
```
[DFZEnabledStateSerializerMod]
# 自動で階段を降りるたびにステートセーブを裏で行う(false:行わない、true:行う)
enableAutoStateSaveOnGoNextFloor = false