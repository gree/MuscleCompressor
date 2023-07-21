using System;
using UnityEngine;

namespace DVRSDK.Plugins.Input
{
    public class OculusButtonInput : MonoBehaviour, ButtonInputInterface
    {
        public event EventHandler<KeyEventArgs> KeyDownEvent;
        public event EventHandler<KeyEventArgs> KeyUpEvent;
        public event EventHandler<AxisEventArgs> AxisChangedEvent;

        private Vector2 lastLeftStickAxis = Vector2.zero;
        private Vector2 lastRightStickAxis = Vector2.zero;

        public void CheckUpdate()
        {
            if (OVRInput.GetDown(OVRInput.RawButton.A)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Select, false));
            if (OVRInput.GetDown(OVRInput.RawButton.B)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Cancel, false));
            if (OVRInput.GetDown(OVRInput.RawButton.RThumbstick)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Stick, false));
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Trigger, false));
            if (OVRInput.GetDown(OVRInput.RawButton.RHandTrigger)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Grip, false));

            if (OVRInput.GetDown(OVRInput.RawButton.X)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Select, true));
            if (OVRInput.GetDown(OVRInput.RawButton.Y)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Cancel, true));
            if (OVRInput.GetDown(OVRInput.RawButton.LThumbstick)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Stick, true));
            if (OVRInput.GetDown(OVRInput.RawButton.LIndexTrigger)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Trigger, true));
            if (OVRInput.GetDown(OVRInput.RawButton.LHandTrigger)) KeyDownEvent?.Invoke(this, new KeyEventArgs(KeyNames.Grip, true));

            if (OVRInput.GetUp(OVRInput.RawButton.A)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Select, false));
            if (OVRInput.GetUp(OVRInput.RawButton.B)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Cancel, false));
            if (OVRInput.GetUp(OVRInput.RawButton.RThumbstick)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Stick, false));

            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Trigger, false));
            if (OVRInput.GetUp(OVRInput.RawButton.RHandTrigger)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Grip, false));

            if (OVRInput.GetUp(OVRInput.RawButton.X)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Select, true));
            if (OVRInput.GetUp(OVRInput.RawButton.Y)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Cancel, true));
            if (OVRInput.GetUp(OVRInput.RawButton.LThumbstick)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Stick, true));
            if (OVRInput.GetUp(OVRInput.RawButton.LIndexTrigger)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Trigger, true));
            if (OVRInput.GetUp(OVRInput.RawButton.LHandTrigger)) KeyUpEvent?.Invoke(this, new KeyEventArgs(KeyNames.Grip, true));

            var leftStickAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick); // 左スティック
            var rightStickAxis = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick); // 右スティック
            if (leftStickAxis != lastLeftStickAxis)
            {
                lastLeftStickAxis = leftStickAxis;
                AxisChangedEvent?.Invoke(this, new AxisEventArgs(KeyNames.Stick, true, leftStickAxis));
            }
            if (rightStickAxis != lastRightStickAxis)
            {
                lastRightStickAxis = rightStickAxis;
                AxisChangedEvent?.Invoke(this, new AxisEventArgs(KeyNames.Stick, false, rightStickAxis));
            }
        }
    }
}
