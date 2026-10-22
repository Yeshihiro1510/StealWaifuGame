using System.Collections.Generic;
using UnityEngine;

namespace Project.Source
{
    public class MagnetController : MonoBehaviour
    {
        [SerializeField] private Transform _root;
        [SerializeField] private float _radiusPlaneHeight = 0.01f;
        [SerializeField] private SphereCollider _collider;
        [SerializeField] private Transform _radiusView;

        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _attractionSpeed;

        private readonly List<Collider> _coinsToPull = new();

        private void OnDrawGizmos()
        {
            foreach (var c in _coinsToPull)
            {
                Gizmos.DrawLine(c.transform.position, _root.position);
            }
        }

        private void Update()
        {
            foreach (var c in _coinsToPull)
            {
                var direction = (_root.position - c.transform.position).normalized;
                c.transform.Translate(direction * (Time.deltaTime * _attractionSpeed), Space.World);
            }
        }

        private void LateUpdate()
        {
            _radiusView.Rotate(Vector3.forward, Time.deltaTime * 360 * _rotationSpeed, Space.Self);
            
            var rootPosition = _root.position;
            transform.position = rootPosition;
            _radiusView.position = new Vector3(rootPosition.x, _radiusPlaneHeight, rootPosition.z);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Tags.Coin))
                _coinsToPull.Add(other);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Tags.Coin))
                _coinsToPull.Remove(other);
        }
        
        public void SetRadius(float value)
        {
            transform.localScale = new Vector3(value, value, value);
        }
    }
}