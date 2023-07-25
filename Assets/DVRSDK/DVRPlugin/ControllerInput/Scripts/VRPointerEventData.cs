using System;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace UnityEngine.EventSystems
{
    /// <summary>
    /// Extension of Unity's PointerEventData to support ray based pointing and also touchpad swiping
    /// </summary>
    public class VRPointerEventData : PointerEventData
    {
        public VRPointerEventData(EventSystem eventSystem) : base(eventSystem) { }

        public Ray worldSpaceRay;
        public Vector2 swipeStart;
    }
}
