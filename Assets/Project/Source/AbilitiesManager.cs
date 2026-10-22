using UnityEngine;

namespace Project.Source
{
    public class AbilitiesManager : MonoBehaviour
    {
        [SerializeField] private MagnetController _magnet;

        private void Awake()
        {
            var magnetAbilityData = PlayerProgressStorage.Data.Magnet;

            UpdateState(magnetAbilityData);
        }

        private void OnEnable()
        {
            PlayerProgressStorage.Data.Magnet.changed += UpdateState;
        }

        private void OnDisable()
        {
            PlayerProgressStorage.Data.Magnet.changed -= UpdateState;
        }

        private void UpdateState(MagnetModel model)
        {
            _magnet.gameObject.SetActive(model.Unlocked);
            _magnet.SetRadius(model.Radius);
        }
    }
}