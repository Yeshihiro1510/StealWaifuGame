using TMPro;
using UnityEngine;

namespace Project.Source
{
    public class CoinBalanceView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private string _format = "{0}";

        private void Awake()
        {
            Draw(PlayerProgressStorage.Data.Coins.Value);
        }

        private void OnEnable()
        {
            PlayerProgressStorage.Data.Coins.changed += Draw;
        }

        private void OnDisable()
        {
            PlayerProgressStorage.Data.Coins.changed -= Draw;
        }
        
        private void Draw(int value)
        {
            _text.text = string.Format(_format, value);
        }
    }
}