using UnityEngine;

namespace DevTools.Humankind.GUITools.UI
{
    public class SmoothScroller
    {
        public float Duration { get; set; } = 0.6f;

        public Vector2 Value { get; set; } = Vector2.zero;

        public bool IsDoneAnimating => t >= 1;

        private float t = 2f;
        private float _targetX;
        private float _initialX;

        public void ScrollTo(float x)
        {
            _targetX = x;
            _initialX = Value.x;
            t = 0;
        }

        public SmoothScroller() { }

        public void Run()
        {
            if (t < 1)
            {
                t += Time.deltaTime / Duration;
                Value = new Vector2(Mathf.Lerp(_initialX, _targetX, t), Value.y);
            }
        }
    }
}
