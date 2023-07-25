using UnityEngine;
using System.Collections.Generic;

namespace DVRSDK.Plugins
{
    [ExecuteInEditMode] // シーンビューでも動かす
    public class Mirror : MonoBehaviour
    {
        public bool OverrideMirrorLayer = false;
        public LayerMask MirrorLayerMask = ~0;

        protected class MirrorSetting
        {
            public RenderTexture texture;
            public MaterialPropertyBlock propertyBlock;
        }
        private Dictionary<Camera, MirrorSetting> mirrorSettings = new Dictionary<Camera, MirrorSetting>();

        private Camera mirrorCamera;
        private Renderer mirrorRenderer;

        private static bool nowRendering = false;
        private static int shaderReflectionTexPropertyID;

        protected static readonly Rect leftEyeRect = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
        protected static readonly Rect rightEyeRect = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
        protected static readonly Rect defaultRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        private void Awake()
        {
            shaderReflectionTexPropertyID = Shader.PropertyToID("_ReflectionTex");
        }

        // このオブジェクトが何かのカメラ(シーンビューも)に描画されるときに呼ばれる
        private void OnWillRenderObject()
        {
            if (mirrorRenderer == null)
            {
                mirrorRenderer = GetComponent<Renderer>();
            }
            if (enabled == false || mirrorRenderer == null || mirrorRenderer.enabled == false)
            {
                return;
            }

            Camera currentCamera = Camera.current;
            if (currentCamera == null || currentCamera == mirrorCamera)
            {
                return;
            }

            // レンダー中のレンダーで再帰しないように
            if (nowRendering)
            {
                return;
            }
            nowRendering = true;

            MirrorSetting mirrorSetting = GetMirrorSetting(currentCamera);

            CopyCameraSettingsToMirrorCamera(currentCamera);

            Render(mirrorSetting, currentCamera);

            mirrorRenderer.SetPropertyBlock(mirrorSetting.propertyBlock);

            nowRendering = false;
        }

        // SteamVRやOculusの実装を分けるためにvirtual
        protected virtual void Render(MirrorSetting mirrorSetting, Camera currentCamera)
        {
            RenderMirror(mirrorSetting.texture, currentCamera.transform.position, currentCamera.transform.rotation, currentCamera.projectionMatrix, defaultRect);
        }

        private void OnDisable() => DestroyObjects();

        private void DestroyObjects()
        {
            if (mirrorCamera)
            {
                DestroyImmediate(mirrorCamera.gameObject);
                mirrorCamera = null;
            }

            foreach (MirrorSetting reflectionData in mirrorSettings.Values)
            {
                DestroyImmediate(reflectionData.texture);
            }
            mirrorSettings.Clear();
        }

        protected void RenderMirror(RenderTexture targetTexture, Vector3 camPosition, Quaternion camRotation, Matrix4x4 camProjectionMatrix, Rect camViewport)
        {
            mirrorCamera.ResetWorldToCameraMatrix();
            mirrorCamera.transform.position = camPosition;
            mirrorCamera.transform.rotation = camRotation;
            mirrorCamera.projectionMatrix = camProjectionMatrix;
            mirrorCamera.targetTexture = targetTexture;
            mirrorCamera.rect = camViewport;

            // ミラー表面
            Vector3 pos = transform.position;
            Vector3 normal = transform.up;

            // ミラー表面のReflectionMatrixを設定
            Vector4 worldSpaceClipPlane = CalculateClipPlane(pos, normal);
            mirrorCamera.worldToCameraMatrix *= CalculateReflectionMatrix(worldSpaceClipPlane);

            // ミラーの平面でクリッピングする(ミラーより後ろのオブジェクトが写らないようにする)
            Vector4 cameraSpaceClipPlane = CameraSpaceClipPlane(mirrorCamera, pos, normal);
            mirrorCamera.projectionMatrix = mirrorCamera.CalculateObliqueMatrix(cameraSpaceClipPlane);

            // カメラ位置を使うエフェクト用にカメラ位置設定(worldToCameraMatrixを設定した時点で位置に関係なくレンダリングはされます)
            mirrorCamera.transform.position = MatrixGetPosition(mirrorCamera.cameraToWorldMatrix);
            mirrorCamera.transform.rotation = MatrixGetRotation(mirrorCamera.cameraToWorldMatrix);

            bool oldInvertCulling = GL.invertCulling;
            GL.invertCulling = !oldInvertCulling;
            mirrorCamera.Render();
            GL.invertCulling = oldInvertCulling;
        }

        private MirrorSetting GetMirrorSetting(Camera currentCamera)
        {
            MirrorSetting mirrorSetting = null;
            if (!mirrorSettings.TryGetValue(currentCamera, out mirrorSetting))
            {
                mirrorSetting = new MirrorSetting();
                mirrorSetting.propertyBlock = new MaterialPropertyBlock();
                mirrorSettings[currentCamera] = mirrorSetting;
            }

            int textureWidth = currentCamera.pixelWidth;
            int textureHeight = currentCamera.pixelHeight;

            // 両目の場合2倍の幅にする
            if (currentCamera.stereoEnabled)
            {
                textureWidth *= 2;
            }

            // QualitySettingsは0でオフだけどRenderTextureは1がオフなので
            var antiAliasing = Mathf.Max(1, QualitySettings.antiAliasing);

            if (!mirrorSetting.texture || mirrorSetting.texture.width != textureWidth || mirrorSetting.texture.height != textureHeight || mirrorSetting.texture.antiAliasing != antiAliasing)
            {
                if (mirrorSetting.texture)
                    DestroyImmediate(mirrorSetting.texture);
                mirrorSetting.texture = new RenderTexture(textureWidth, textureHeight, 24);
                mirrorSetting.texture.antiAliasing = antiAliasing;
                mirrorSetting.texture.hideFlags = HideFlags.DontSave;
                mirrorSetting.propertyBlock.SetTexture(shaderReflectionTexPropertyID, mirrorSetting.texture);
            }

            return mirrorSetting;
        }

        private void CopyCameraSettingsToMirrorCamera(Camera src)
        {
            if (!mirrorCamera)
            {
                GameObject go = new GameObject("MirrorCamera-" + gameObject.name);
                go.hideFlags = HideFlags.HideAndDontSave;
                mirrorCamera = go.AddComponent<Camera>();
                mirrorCamera.enabled = false;
            }

            // ミラーカメラに現在レンダリングしようとしているカメラの設定を全てコピー
            mirrorCamera.clearFlags = src.clearFlags;
            mirrorCamera.backgroundColor = src.backgroundColor;
            mirrorCamera.farClipPlane = src.farClipPlane;
            mirrorCamera.nearClipPlane = src.nearClipPlane;
            mirrorCamera.orthographic = src.orthographic;
            mirrorCamera.fieldOfView = src.fieldOfView;
            mirrorCamera.aspect = src.aspect;
            mirrorCamera.orthographicSize = src.orthographicSize;

            var layerMaskOverride = src.GetComponent<MirrorLayerMaskOverride>();
            if (layerMaskOverride != null)
            {
                mirrorCamera.cullingMask = layerMaskOverride.MirrorLayerMask;
            }
            else if (OverrideMirrorLayer)
            {
                mirrorCamera.cullingMask = MirrorLayerMask;
            }
            else
            {
                mirrorCamera.cullingMask = src.cullingMask;
            }
        }

        private static Vector4 CalculateClipPlane(Vector3 pos, Vector3 normal)
        {
            return new Vector4(normal.x, normal.y, normal.z, -Vector3.Dot(pos, normal));
        }

        // 与えられた平面の位置と法線をカメラ空間内の平面に変換する
        private static Vector4 CameraSpaceClipPlane(Camera cam, Vector3 pos, Vector3 normal)
        {
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(pos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized;
            return CalculateClipPlane(cpos, cnormal);
        }

        // 指定した平面の反射行列を計算する
        private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
        {
            Matrix4x4 reflectionMat = Matrix4x4.identity;

            reflectionMat[0, 0] = (1f - 2f * plane[0] * plane[0]);
            reflectionMat[0, 1] = (0f - 2f * plane[0] * plane[1]);
            reflectionMat[0, 2] = (0f - 2f * plane[0] * plane[2]);
            reflectionMat[0, 3] = (0f - 2f * plane[3] * plane[0]);

            reflectionMat[1, 0] = (0f - 2f * plane[1] * plane[0]);
            reflectionMat[1, 1] = (1f - 2f * plane[1] * plane[1]);
            reflectionMat[1, 2] = (0f - 2f * plane[1] * plane[2]);
            reflectionMat[1, 3] = (0f - 2f * plane[3] * plane[1]);

            reflectionMat[2, 0] = (0f - 2f * plane[2] * plane[0]);
            reflectionMat[2, 1] = (0f - 2f * plane[2] * plane[1]);
            reflectionMat[2, 2] = (1f - 2f * plane[2] * plane[2]);
            reflectionMat[2, 3] = (0f - 2f * plane[3] * plane[2]);

            reflectionMat[3, 0] = 0f;
            reflectionMat[3, 1] = 0f;
            reflectionMat[3, 2] = 0f;
            reflectionMat[3, 3] = 1f;

            return reflectionMat;
        }

        private static float CopySign(float sizeval, float signval)
        {
            return Mathf.Sign(signval) == 1 ? Mathf.Abs(sizeval) : -Mathf.Abs(sizeval);
        }

        private static Quaternion MatrixGetRotation(Matrix4x4 matrix)
        {
            Quaternion q = new Quaternion();
            q.w = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 + matrix.m11 + matrix.m22)) / 2;
            q.x = Mathf.Sqrt(Mathf.Max(0, 1 + matrix.m00 - matrix.m11 - matrix.m22)) / 2;
            q.y = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 + matrix.m11 - matrix.m22)) / 2;
            q.z = Mathf.Sqrt(Mathf.Max(0, 1 - matrix.m00 - matrix.m11 + matrix.m22)) / 2;
            q.x = CopySign(q.x, matrix.m21 - matrix.m12);
            q.y = CopySign(q.y, matrix.m02 - matrix.m20);
            q.z = CopySign(q.z, matrix.m10 - matrix.m01);
            return q;
        }

        private static Vector3 MatrixGetPosition(Matrix4x4 matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;

            return new Vector3(x, y, z);
        }
    }
}
