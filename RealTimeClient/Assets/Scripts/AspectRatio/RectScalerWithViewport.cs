//---------------------------------------------------------------
//
// 郢ｧ�｢郢ｧ�ｹ郢晏｣ｹ縺醍ｹ晏沺�ｯ豕鯉ｽｯ�ｾ陟｢蠑選郢ｧ�ｹ郢ｧ�ｱ郢晢ｽｼ郢晢ｽｩ郢晢ｽｼ [ RectScalerWithViewport.cs ]
// Author:Kenta Nakamoto
// Data:2024/07/17
// Update:2024/07/17
//
//---------------------------------------------------------------
using UnityEngine;

namespace TedLab
{
    [ExecuteAlways] // 陷�蜥ｲ蜃ｽ隴弱ｆ�ｻ�･陞滓じ縲堤ｹｧ繧��劒闖ｴ諛岩��郢ｧ�
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
            // 髫ｪ�ｭ陞ｳ螢ｹ窶ｲ邵ｺ繧��ｽ檎ｸｺ�ｰ邵ｺ譏ｴ笆�郢ｧ蟲ｨ�定怕�ｪ陷��
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

            // canvas scaler邵ｺ荵晢ｽ芽�第��逡�
            var logWidth = Mathf.Log(width / referenceResolution.x, LogBase);
            var logHeight = Mathf.Log(height / referenceResolution.y, LogBase);
            var logWeightedAverage = Mathf.Lerp(logWidth, logHeight, matchWidthOrHeight);
            var scale = Mathf.Pow(LogBase, logWeightedAverage);

            if (float.IsNaN(scale) || scale <= 0f)
            {
                return;
            }

            refRect.localScale = new Vector3(scale, scale, scale);

            // 郢ｧ�ｹ郢ｧ�ｱ郢晢ｽｼ郢晢ｽｫ邵ｺ�ｧ驍ｵ�ｮ邵ｺ�ｾ郢ｧ荵掾ｮ邵ｺ�ｧ鬯��ｼ懈ｲｺ邵ｺ�邵ｺ螟ｧ�ｺ���｡郢ｧ�
            var revScale = 1f / scale;
            refRect.sizeDelta = new Vector2(width * revScale, height * revScale);

            _width = width;
            _height = height;
            _matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}