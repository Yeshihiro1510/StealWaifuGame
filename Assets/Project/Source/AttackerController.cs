using UnityEngine;

namespace Project.Source
{
    public class AttackerController : MonoBehaviour
    {
        [SerializeField] private float _damage;
        [SerializeField] private float _countdown;

        private float _countdownTimer;

        private void Update()
        {
            if (_countdownTimer > 0f)
            {
                _countdownTimer -= Time.deltaTime;
            }
            else if (_countdownTimer < 0f)
            {
                _countdownTimer = 0f;
            }
        }

        public void Attack()
        {
            // animation
            // raycast
            // deal damage
            
            _countdownTimer = _countdown;
        }
    }

    public interface IDamageable
    {
        public void TakeDamage(int damage);
    }
}