using System;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Source
{
    public class LevelProgressView : MonoBehaviour
    {
        [SerializeField, Min(2)] private int _amount = 2;
        [SerializeField, Min(0)] private int _progress = 1;
        [SerializeField] private Vector2 _levelPointSize = new(100, 100);
        [SerializeField] private Color _emptyColor;
        [SerializeField] private Color _filledColor;
        [SerializeField] private Sprite _sprite;
        [SerializeField, Min(0.01f)] private float _pixelsPerUnitMultiplier = 1f;
        [SerializeField, HideInInspector] private Image[] _levelPoints;

        public void SetProgress(int value)
        {
            _progress = Mathf.Clamp(value, 0, _amount);
            for (var i = 0; i < _levelPoints.Length; i++)
            {
                _levelPoints[i].color = GetActualColor(i);
            }
        }

        public void SetAmount(int value)
        {
            _amount = value;
            _progress = Mathf.Clamp(value, 0, _amount);

            if (_levelPoints == null)
            {
                _levelPoints = new Image[_amount];
                for (var i = 0; i < _levelPoints.Length; i++)
                {
                    _levelPoints[i] = CreateLevelPoint(i);
                }
            }
            else
            {
                if (_levelPoints.Length > _amount)
                {
                    for (var i = _amount; i < _levelPoints.Length; i++)
                    {
                        Destroy(_levelPoints[i].gameObject);
                    }

                    Array.Resize(ref _levelPoints, _amount);
                }
                else if (_levelPoints.Length < _amount)
                {
                    var lenght = _levelPoints.Length;
                    Array.Resize(ref _levelPoints, _amount);
                    for (var i = lenght; i < _amount; i++)
                    {
                        _levelPoints[i] = CreateLevelPoint(i);
                    }
                }
            }
        }

        private Image CreateLevelPoint(int i)
        {
            var instance = new GameObject("LevelPoint").AddComponent<Image>();
            instance.transform.SetParent(transform);
            instance.type = Image.Type.Sliced;
            InitializeLevelPoint(instance, i);
            return instance;
        }

        private void InitializeLevelPoint(Image obj, int i)
        {
            obj.sprite = _sprite;
            obj.rectTransform.sizeDelta = _levelPointSize;
            obj.pixelsPerUnitMultiplier = _pixelsPerUnitMultiplier;
            obj.color = GetActualColor(i);
        }

        private Color GetActualColor(int i) => i < _progress ? _filledColor : _emptyColor;
    }
}