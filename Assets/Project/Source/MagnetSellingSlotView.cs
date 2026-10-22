using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Source
{
    public class MagnetSellingSlotView : MonoBehaviour
    {
        [SerializeField] private LevelProgressView _levelProgressView;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private TMP_Text _upgradeText;
        [SerializeField] private string _upgradeTextFormat = "Upgrade - {0} coins";

        [SerializeField] private Button _unlockButton;
        [SerializeField] private TMP_Text _unlockText;
        [SerializeField] private string _unlockTextFormat = "Unlock - {0} coins";

        private void Awake()
        {
            var magnetModel = PlayerProgressStorage.Data.Magnet;

            _levelProgressView.SetAmount(magnetModel.MaxLevel);

            _unlockText.text = string.Format(_unlockTextFormat, magnetModel.UnlockCost);

            UpdateState(magnetModel);
            
            magnetModel.changed += UpdateState;

            _unlockButton.onClick.AddListener(() => magnetModel.Unlock());
            _upgradeButton.onClick.AddListener(() => magnetModel.Upgrade());
        }
        
        private void OnDestroy()
        {
            PlayerProgressStorage.Data.Magnet.changed -= UpdateState;
        }

        // private void OnMagnetUnlocked()
        // {
        //     _unlockButton.gameObject.SetActive(false);
        //     _levelProgressView.gameObject.SetActive(true);
        //     _upgradeButton.gameObject.SetActive(true);
        //
        //     _unlockButton.onClick.RemoveListener(PlayerProgressStorage.UnlockMagnetAbility);
        //     PlayerProgressStorage.magnetUnlocked -= OnMagnetUnlocked;
        //
        //     UpdateUpgradeButtonAndText(PlayerProgressStorage.Data.MagnetAbility);
        // }

        // private void OnMagnetUpgraded()
        // {
        //     var magnetAbilityData = PlayerProgressStorage.Data.MagnetAbility;
        //     _levelProgressView.SetProgress(magnetAbilityData.Level);
        //     UpdateUpgradeButtonAndText(magnetAbilityData);
        // }

        // private void UpdateUpgradeButtonAndText(MagnetAbilityData magnetAbilityData)
        // {
        //     if (magnetAbilityData.Level < magnetAbilityData.Levels.Length - 1)
        //         _upgradeText.text = string.Format(_upgradeTextFormat, magnetAbilityData.UpgradeCost);
        //     else
        //         _upgradeButton.gameObject.SetActive(false);
        // }

        private void UpdateState(MagnetModel model)
        {
            _unlockButton.gameObject.SetActive(!model.Unlocked);
            _levelProgressView.gameObject.SetActive(model.Unlocked);
            _upgradeButton.gameObject.SetActive(model is { Unlocked: true, IsFinalLevel: false });

            _upgradeText.text = string.Format(_upgradeTextFormat, model.UpgradeCost);

            _levelProgressView.SetProgress(model.Level);
        }
    }
}