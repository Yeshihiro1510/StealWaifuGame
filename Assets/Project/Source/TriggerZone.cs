using System;
using UnityEngine;

namespace Project.Source
{
    public class TriggerZone : MonoBehaviour
    {
        public event Action<Collider> onTriggerEnter;
        public event Action<Collider> onTriggerExit;

        private void OnTriggerEnter(Collider other) => onTriggerEnter?.Invoke(other);
        private void OnTriggerExit(Collider other) => onTriggerExit?.Invoke(other);
    }
}