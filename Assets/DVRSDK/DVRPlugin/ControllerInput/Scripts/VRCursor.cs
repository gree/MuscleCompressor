using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DVRSDK.Plugins.Input
{
    [RequireComponent(typeof(LineRenderer))]
    public class VRCursor : MonoBehaviour
    {
        public Transform gazeIcon;

        [Tooltip("Angular scale of pointer")]
        public float depthScaleMultiplier = 0.03f;

        public Transform rayTransform;

        public bool hidden { get; private set; }

        private float depth;

        private int positionSetsThisFrame = 0;

        private float lastShowRequestTime;

        private LineRenderer lineRenderer;

        public float visibilityStrength
        {
            get
            {
                return Mathf.Clamp01(1 - (Time.time - lastShowRequestTime) / 1); // 1sec
            }
        }

        public void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        void Update()
        {
            lineRenderer.SetPosition(0, rayTransform.position);
            lineRenderer.SetPosition(1, gazeIcon.position);

            if (rayTransform == null && Camera.main != null)
                rayTransform = Camera.main.transform;

            transform.position = rayTransform.position + rayTransform.forward * depth;

            if (visibilityStrength == 0 && !hidden)
            {
                Hide();
            }
            else if (visibilityStrength > 0 && hidden)
            {
                Show();
            }
        }

        public void SetCursorStartDest(Vector3 _, Vector3 pos, Vector3 normal)
        {
            var vec = (rayTransform.position - pos).normalized;

            var nearPos = pos + vec * 0.01f;
            transform.position = nearPos;

            Quaternion newRot = transform.rotation;
            newRot.SetLookRotation(normal, rayTransform.up);
            transform.rotation = newRot;

            depth = (rayTransform.position - nearPos).magnitude;

            var currentScale = depth * depthScaleMultiplier;
            transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            positionSetsThisFrame++;
            RequestShow();
        }

        void LateUpdate()
        {
            if (positionSetsThisFrame == 0)
            {
                Quaternion newRot = transform.rotation;
                newRot.SetLookRotation(rayTransform.forward, rayTransform.up);
                transform.rotation = newRot;
            }

            Quaternion iconRotation = gazeIcon.rotation;
            iconRotation.SetLookRotation(transform.rotation * new Vector3(0, 0, 1));
            gazeIcon.rotation = iconRotation;

            positionSetsThisFrame = 0;
        }

        public void RequestShow()
        {
            Show();
            lastShowRequestTime = Time.time;
        }

        void Hide()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            var renderer = GetComponent<Renderer>();
            if (renderer) renderer.enabled = false;
            lineRenderer.enabled = false;
            hidden = true;
        }

        void Show()
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            var renderer = GetComponent<Renderer>();
            if (renderer) renderer.enabled = true;
            lineRenderer.enabled = true;
            hidden = false;
        }
    }
}
