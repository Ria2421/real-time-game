//---------------------------------------------------------------
//
// 繧｢繧ｹ繝壹け繝域ｯ泌ｯｾ蠢弑I繧ｹ繧ｱ繝ｼ繝ｩ繝ｼ [ RectScalerWithViewport.cs ]
// Author:Kenta Nakamoto
// Data:2024/07/17
// Update:2024/07/17
//
//---------------------------------------------------------------
using UnityEngine;

namespace TedLab
{
    [ExecuteAlways] // 蜀咲函譎ゆｻ･螟悶〒繧ょ虚菴懊☆繧�
    public class RectScalerWithViewport : MonoBehaviour
    {
        private const float LogBase = 2;

        [SerializeField] private Camera refCamera = null;
        [SerializeField] private RectTransform refRect = null;
        [SerializeField] private Vector2 referenceResolution = new Vector2(1920, 1080);
        [Range(0, 1)][SerializeField] private float matchWidthOrHeight = 0;

        private float _width = -1;
        private float _height = -1;
        private float _matchWidthOrHeight = 0f;

        private void Awake()
        {
            if (refRect == null)
            {
                refRect = GetComponent<RectTransform>();
            }
            UpdateRect();
        }

        private void Update()
        {
            UpdateRectWithCheck();
        }

        public void UpdateRectWithCheck()
        {
            if (refRect == null)
                return;

            var targetCamera = GetTargetCamera();
            if (targetCamera == null)
                return;

            var rect = targetCamera.rect;
            var width = rect.width * Screen.width;
            var height = rect.height * Screen.height;

            if (Mathf.Approximately(_width, width)
                && Mathf.Approximately(_height, height)
                && Mathf.Approximately(_matchWidthOrHeight, matchWidthOrHeight))
            {
                return;
            }

            UpdateRect();
        }

        private Camera GetTargetCamera()
        {
            // 險ｭ螳壹′縺ゅｌ縺ｰ縺昴■繧峨ｒ蜆ｪ蜈�
            return refCamera != null ? refCamera : Camera.main;
        }

        private void UpdateRect()
        {
            if (refRect == null)
                return;

            if (Mathf.Approximately(referenceResolution.x, 0f) || Mathf.Approximately(referenceResolution.y, 0f))
                return;

            var targetCamera = GetTargetCamera();
            if (targetCamera == null)
                return;

            var rect = targetCamera.rect;
            var width = rect.width * Screen.width;
            var height = rect.height * Screen.height;
            if (width == 0f || height == 0f)
            {
                return;
            }

            // canvas scaler縺九ｉ蠑慕畑
            var logWidth = Mathf.Log(width / referenceResolution.x, LogBase);
            var logHeight = Mathf.Log(height / referenceResolution.y, LogBase);
            var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight);
            var scale = Mathf.Pow(LogBase, logWeightedAverage);

            if (float.IsNaN(scale) || scale <= 0f)
            {
                return;
            }

            refRect.localScale = new Vector3(scale, scale, scale);

            // 繧ｹ繧ｱ繝ｼ繝ｫ縺ｧ邵ｮ縺ｾ繧九��縺ｧ鬆伜沺縺�縺大ｺ��￡繧�
            var revScale = 1f / scale;
            refRect.sizeDelta = new Vector2(width * revScale, height * revScale);

            _width = width;
            _height = height;
            _matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}