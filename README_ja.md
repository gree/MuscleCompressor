# MuscleCompressor

- UnityのHumanoidアニメーションを従来の.animではなく、[Muscle](https://docs.unity3d.com/ja/2017.4/Manual/MuscleDefinitions.html)を使って軽量に外部保存・読み出しするためのスクリプトです。
- Meta QuestなどVR機器装着時の動きを外部保存・読み出しするPCVR用サンプルシーンが含まれています。
- 60fpsで記録した1分間のHumanoidモーションのデータサイズ比較。MuscleCompressorのフォーマットは既存フォーマットより軽量です（.animと比較して96%の圧縮)

![image](https://github.com/gree/MuscleCompressor/blob/readme-imgs/imgs/MuscleCompressor.png?raw=true)


## 動作環境
- Unity 2021.3.19f1
- [Oculus Integration 53.1](https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022?locale=ja-JP) (別途インポート必要)
- [Final IK](https://assetstore.unity.com/?q=Final%20IK&orderBy=1) (別途インポート必要)
- Windows 10 x64
- Meta Quest 2

※初回起動時にはFinalIKとOculus Integrationが無いためにSafeModeでの起動を促されますが、"Ignore"して進めてください

## 使い方 (モーション読み出し)

https://github.com/gree/MuscleCompressor/assets/5110708/b8c7f5ca-2493-4c2c-a33a-3b40ab4ab0d9

### ランタイム読み出し
1. Unity 2021.3.19f1でこのプロジェクトを開いたらOculus IntegrationとFinal IKをPackage Managerからインポート
2. Assets/VRStudioLab/Scenes/LoadSceneSample.unityを開き、実行
3. Load Motionボタンを押すとファイルブラウザが立ち上がるので、モーション保存場所（Assets/StreamingAssets/Motion）へ行き、.dataを選択
4. モーションがシーンのアバターに適用される

※Assets/StreamingAssets/Motionにサンプルモーションを同梱しているので、テスト等にご活用ください。

### アニメーションクリップへの変換
1. (Unity 2021.3.19f1でこのプロジェクトを開いたらOculus IntegrationとFinal IKをPackage Managerからインポート)
2. Unity上部のメニューからVRStudioLab -> Bytes2Animを押す
3. ファイルブラウザが立ち上がるので、モーション保存場所（Assets/StreamingAssets/Motion）へ行き、.dataを選択
4. Assets/VRStudioLab/Animationsに変換されたクリップが保存される
   
## 使い方 (モーション保存)

https://github.com/gree/MuscleCompressor/assets/5110708/8520bcc3-f59d-43ad-b7a9-d236f2846591

1. (Unity 2021.3.19f1でプロジェクトを開いたらOculus IntegrationとFinal IKをPackage Managerからインポート)
2. Assets/VRStudioLab/Scenes/RecordSceneSample.unityを開き、RecordingManager内のRecording Datasに収録モーションの保存名(Name)と長さ(Duration)を入力
3. VR機器を接続した状態でPlay
4. アバター読み込み後、READYボタンを押してからDuration秒のモーションがAssets/StreamingAssets/Motionに保存される




