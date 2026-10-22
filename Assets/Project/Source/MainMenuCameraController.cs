using UnityEngine;

namespace Project.Source
{
    public class MainMenuCameraController : MonoBehaviour
    {
        [SerializeField] private Transform _centralRotationPoint;
        [SerializeField] private Camera _camera;

        [SerializeField] private float _rotationSpeed;
        [SerializeField] private float _height;
        [SerializeField] private float _distance;
        
        private void Update()
        {
            var time = Time.time * _rotationSpeed;
            var position = new Vector3(Mathf.Sin(time), 0, -Mathf.Cos(time)) * _distance;
            position.y = _height;
            _camera.transform.position = _centralRotationPoint.position + position;
            _camera.transform.LookAt(_centralRotationPoint);
        }
    }
}