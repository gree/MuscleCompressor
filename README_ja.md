# MuscleCompressor

- UnityのHumanoidアニメーションを従来の.animではなく、Muscleを使って軽量に外部保存・読み出しするためのスクリプトです。
- Meta QuestなどのVR機器装着時の動きを実際に外部保存・読み出しするモーションキャプチャーとして動作するサンプルシーンが含まれています。
- 60fpsで1分のHumanoid録画のデータサイズ比較。MuscleCompressorのフォーマットは既存フォーマットより軽量です（.animの場合最大96%圧縮)


## 動作環境
- Unity 2021.3.19f1
- Oculus Integration 53.1 (別途インポート必要)
- Final IK (別途インポート必要)

## 使い方 (モーション保存)
https://github.com/reality-inc/MuscleCompressor/assets/79638033/ea09b26e-78f9-4207-9065-9bfe0bf4d212

1. Unity 2021.3.19f1でこのプロジェクトを開いたらOculus IntegrationとFinal IKをインポート
2. Assets/VRStudioLab/Scenes/RecordSceneSample.unityを開き、RecordingManager内のRecording Datasに収録モーションの保存名(Name)と長さ(Duration)を入力
3. VR機器を接続した状態でPlay
4. アバター読み込み後、READYボタンを押してからDuration秒のモーションがAssets/StreamingAssets/Motionに保存される


## (ToDo) 使い方 (モーション読み出し)


### ランタイム読み出し
1. (Unity 2021.3.19f1でこのプロジェクトを開いたらOculus IntegrationとFinal IKをインポートする)
2. Assets/VRStudioLab/Scenes/LoadSceneSample.unityを開き、実行
3. Load Motionボタンを押すとファイルブラウザが立ち上がるので、モーション保存場所（Assets/StreamingAssets/Motion）へ行き、.dataを選択
4. モーションがシーンのアバターに適用される

### アニメーションクリップへの変換
1. (Unity 2021.3.19f1でこのプロジェクトを開いたらOculus IntegrationとFinal IKをインポートする)
2. Unity上部のメニューからVRStudioLab>Bytes2Animを押す
3. ファイルブラウザが立ち上がるので、モーション保存場所（Assets/StreamingAssets/Motion）へ行き、.dataを選択
4. Assets/VRStudioLab/Animationsに変換されたクリップが保存される

