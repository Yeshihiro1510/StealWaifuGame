using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Source
{
    public class ShopWindow : GUIWindow
    {
        [SerializeField] private Image _image;

        private Sequence _activeSequence;

        private void OnDestroy()
        {
            _activeSequence.Kill();
        }

        public override void Open()
        {
            base.Open();
            ClearActiveSequence();
            _activeSequence = DOTween.Sequence()
                .Join(_image.rectTransform.DOLocalMoveY(0f, 0.2f).From(-500f))
                .Join(_image.DOFade(1f, 0.2f).From(0f))
                .OnStart(() => gameObject.SetActive(true))
                .OnComplete(OnOpen);
        }

        public override void Close()
        {
            base.Close();
            ClearActiveSequence();
            _activeSequence = DOTween.Sequence()
                .Join(_image.rectTransform.DOLocalMoveY(-500f, 0.2f))
                .Join(_image.DOFade(0f, 0.2f))
                .OnComplete(() =>
                {
                    gameObject.SetActive(false);
                    OnClose();
                });
        }

        private void ClearActiveSequence()
        {
            if (_activeSequence == null) return;
            _activeSequence.Kill(true);
            _activeSequence = null;
        }
    }
}