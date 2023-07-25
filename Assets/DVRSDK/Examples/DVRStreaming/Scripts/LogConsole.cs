using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogConsole : MonoBehaviour
{
    // ログを何個まで保持するか
    [SerializeField] private int MaxLogCount = 29;

    [SerializeField] private Text textObject = null;

    [Header("Text Color")]
    public string colorError = "red";
    public string colorAssert = "red";
    public string colorWarning = "yellow";
    public string colorLog = "white";
    public string colorException = "red";

    // ログの文字列を入れておくためのQueue
    private Queue<string> LogMessages = new Queue<string>();

    // ログの文字列を結合するのに使う
    private StringBuilder StringBuilder = new StringBuilder();

    void OnEnable()
    {
        Application.logMessageReceived += OnLogMessageReceived;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= OnLogMessageReceived;
    }

    void OnLogMessageReceived(string text, string stackTrace, LogType type)
    {
        // https://docs.unity3d.com/Manual/StyledText.html#ColorNames
        string color = null;
        switch (type)
        {
            case LogType.Error:
                color = colorError;
                break;

            case LogType.Assert:
                color = colorAssert;
                break;

            case LogType.Exception:
                color = colorException;
                break;

            case LogType.Warning:
                color = colorWarning;
                break;

            case LogType.Log:
                color = colorLog;
                break;
        }

        // ログメッセージの整形
        string message = $"<color={color}>{text}</color>\n";

        // ログをQueueに追加
        LogMessages.Enqueue(message);

        // ログの個数が上限を超えていたら、最古のものを削除する
        while (LogMessages.Count > MaxLogCount)
            LogMessages.Dequeue();

        StringBuilder.Length = 0;
        foreach (string s in LogMessages)
            StringBuilder.Append(s);

        this.textObject.text = StringBuilder.ToString();
    }
}
