using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DVRSDK.Plugins.Input
{
    public class ButtonManager : MonoBehaviour
    {
        public static ButtonManager Instance;

        public event EventHandler<KeyEventArgs> KeyDownEvent;
        public event EventHandler<KeyEventArgs> KeyUpEvent;
        public event EventHandler<AxisEventArgs> AxisChangedEvent;

        private List<KeyEventArgs> KeyDownList = new List<KeyEventArgs>();
        private List<KeyEventArgs> KeyUpList = new List<KeyEventArgs>();

        private Dictionary<(bool isLeft, KeyNames keyName), Vector2> AxisDictionary = new Dictionary<(bool isLeft, KeyNames keyName), Vector2>();
        private Dictionary<(bool isLeft, KeyNames keyName), bool> KeyStateDictionary = new Dictionary<(bool isLeft, KeyNames keyName), bool>();

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            Globals.buttonInputInterface.KeyDownEvent += ButtonInputInterface_KeyDownEvent;
            Globals.buttonInputInterface.KeyUpEvent += ButtonInputInterface_KeyUpEvent;
            Globals.buttonInputInterface.AxisChangedEvent += ButtonInputInterface_AxisChangedEvent;
        }

        private void ButtonInputInterface_AxisChangedEvent(object sender, AxisEventArgs e)
        {
            AxisDictionary[(e.IsLeft, e.KeyName)] = e.Value;
            AxisChangedEvent?.Invoke(sender, e);
        }

        private void ButtonInputInterface_KeyDownEvent(object sender, KeyEventArgs e)
        {
            KeyDownList.Add(e);
            KeyStateDictionary[(e.IsLeft, e.KeyName)] = true;
            KeyDownEvent?.Invoke(sender, e);
        }

        private void ButtonInputInterface_KeyUpEvent(object sender, KeyEventArgs e)
        {
            KeyUpList.Add(e);
            KeyStateDictionary[(e.IsLeft, e.KeyName)] = false;
            KeyUpEvent?.Invoke(sender, e);
        }

        // Update is called once per frame
        void Update()
        {
            KeyDownList.Clear();
            KeyUpList.Clear();
            Globals.buttonInputInterface.CheckUpdate();
        }

        public bool GetKeyDown(bool isLeft, KeyNames keyName)
        {
            return KeyDownList.Any(d => d.IsLeft == isLeft && d.KeyName == keyName);
        }

        public bool GetKeyUp(bool isLeft, KeyNames keyName)
        {
            return KeyUpList.Any(d => d.IsLeft == isLeft && d.KeyName == keyName);
        }

        public Vector2 GetAxis(bool isLeft, KeyNames keyName)
        {
            var t = (isLeft, keyName);
            if (AxisDictionary.ContainsKey(t))
            {
                return AxisDictionary[t];
            }
            return Vector2.zero;
        }

        public bool GetKeyState(bool isLeft, KeyNames keyName)
        {
            var t = (isLeft, keyName);
            if (KeyStateDictionary.ContainsKey(t))
            {
                return KeyStateDictionary[t];
            }
            return false;
        }
    }
}
