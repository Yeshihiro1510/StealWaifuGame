using DG.Tweening;
using UnityEngine;
using UnityEngine.Pool;

namespace Project.Source
{
    public class CoinController : MonoBehaviour
    {
        [SerializeField] private Renderer _renderer;

        [Header("Animation")]
        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _height;
        [SerializeField] private float _angle;
        [SerializeField] private float _duration;
        [SerializeField] private Ease _ease;

        public ObjectPool<CoinController> Pool { get; set; }

        public bool Collected { get; private set; }

        private void Awake()
        {
            _renderer.material = Instantiate(_renderer.material);
        }

        private void OnDestroy()
        {
            Destroy(_renderer.material);
        }

        private void Update()
        {
            if (Collected) return;
            var time = Mathf.Repeat(Time.time * _rotationSpeed, 1f);
            var progress = Mathf.Lerp(time * time, Mathf.Sqrt(time), 0.5f);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, progress * 360f, transform.eulerAngles.z);
        }

        public void Collect()
        {
            if (Collected) return;
            Collected = true;
            var color = _renderer.material.color;
            color.a = 0f;
            DOTween.Sequence()
                .Join(transform.DOMoveY(transform.position.y + _height, _duration))
                .Join(transform.DORotate(Vector3.up * _angle, _duration, RotateMode.WorldAxisAdd))
                .Join(_renderer.material.DOColor(color, _duration))
                .OnComplete(Release)
                .SetEase(_ease);
        }

        public void Init()
        {
            Collected = false;
            var color = _renderer.material.color;
            color.a = 1f;
            _renderer.material.color = color;
        }

        public void Release() => Pool.Release(this);
    }
}