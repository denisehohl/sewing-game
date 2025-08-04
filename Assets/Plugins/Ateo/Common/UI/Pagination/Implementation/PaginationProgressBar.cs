using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Ateo.Extensions;

namespace Ateo.UI
{
    public class PaginationProgressBar : Pagination
    {
        [Required]
        public RectTransform ProgressBar;

        public float Duration;
        public Ease Ease;

        private float _width;
        private Tween _tween;

        public override void Initialize(int count, int index = 0)
        {
            Count = count;
            _width = ProgressBar.parent.GetComponent<RectTransform>().rect.width;
            SelectElement(0, true);
        }

        public override void SelectElement(int index, bool instant = false)
        {
            var progress = Mathf.Clamp01(1f / Count * (index + 1));
            var right = _width * (1f - progress);
            var vector = new Vector4(0f, right, 0f, 0f);

            if (instant)
            {
                ProgressBar.SetLeftRightTopBottom(vector);
            }
            else
            {
                _tween = DOTween
                    .To(ProgressBar.GetLeftRightTopBottom,
                        ProgressBar.SetLeftRightTopBottom,
                        vector,
                        Duration)
                    .SetEase(Ease)
                    .Play();
            }
        }
    }
}