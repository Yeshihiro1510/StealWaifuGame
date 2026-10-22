using System;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Project.Source
{
    [Serializable]
    public enum GameState
    {
        MainMenu,
        Game,
    }

    public class EntryPoint : MonoBehaviour
    {
        private GameState _state;

        [Header("Main menu")]
        [SerializeField] private MainMenuCameraController _mainMenuCamera;
        [SerializeField] private Canvas _mainMenuGUI;
        [SerializeField] private InputActionReference _startAction;

        [Header("Game")]
        [SerializeField] private Terrain _terrain;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _shopPoint;
        [SerializeField] private Transform _sellPoint;
        [SerializeField] private Transform _gatherPoint;
        [SerializeField] private GUIController _gameGUI;
        [SerializeField] private Button _backToMainMenuButton;
        private PlayerController _player;

        [Header("Coins")]
        [SerializeField, Min(0)] private int _coinsToSpawn = 100;
        [SerializeField, Min(0)] private Vector4 _spawnBounds;
        private CoinsFactory _coinsFactory;

        private void OnDrawGizmos()
        {
            var scaleX = _spawnBounds.y - _spawnBounds.x;
            var scaleZ = _spawnBounds.w - _spawnBounds.z;
            var scaleY = Mathf.Min(scaleX, scaleZ);
            Gizmos.DrawWireCube(
                new Vector3(_spawnBounds.x + scaleX / 2, scaleY / 2, _spawnBounds.z + scaleZ / 2),
                new Vector3(scaleX, scaleY, scaleZ));
        }

        private void Awake()
        {
            _coinsFactory = new CoinsFactory();
            for (var i = 0; i < _coinsToSpawn; i++)
            {
                var x = Random.Range(_spawnBounds.x, _spawnBounds.y);
                var z = Random.Range(_spawnBounds.z, _spawnBounds.w);
                var height = _terrain.SampleHeight(new Vector3(x, 0, z));
                var coin = _coinsFactory.Pool.Get();
                coin.transform.position = new Vector3(x, height + 1, z);
            }

            _startAction.action.performed += OnEnterPressed;
            _backToMainMenuButton.onClick.AddListener(OnBackToMainMenuButtonPressed);

            ForceSetState(_state);
        }

        private void OnDestroy()
        {
            _startAction.action.performed -= OnEnterPressed;
            _backToMainMenuButton.onClick.RemoveListener(OnBackToMainMenuButtonPressed);
            PlayerProgressStorage.Save();
        }

        private void OnEnterPressed(InputAction.CallbackContext _)
        {
            ForceSetState(GameState.Game);
        }

        private void OnBackToMainMenuButtonPressed()
        {
            ForceSetState(GameState.MainMenu);
        }

        private void ForceSetState(GameState state)
        {
            _state = state;

            switch (state)
            {
                case GameState.MainMenu:
                    DestroyGame();
                    InitMainMenu();
                    break;
                case GameState.Game:
                    DestroyMainMenu();
                    InitGame();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private void InitGame()
        {
            _gameGUI.gameObject.SetActive(true);
            _gameGUI.ShopButton.onClick.AddListener(OnShopButtonClick);
            _gameGUI.SellButton.onClick.AddListener(OnSellButtonClick);
            _gameGUI.GatherButton.onClick.AddListener(OnGatherButtonClick);

            _player = Instantiate(Resources.Load<PlayerController>("Player"));
            _player.GetComponentInChildren<KinematicCharacterMotor>().SetPosition(_spawnPoint.position);
            _player.Init(_gameGUI);
        }

        private void OnShopButtonClick()
        {
            _player.Motor.SetPositionAndRotation(_shopPoint.position, _shopPoint.rotation);
            _player.Camera.PlanarDirection = _shopPoint.forward;
        }

        private void OnSellButtonClick()
        {
            _player.Motor.SetPositionAndRotation(_sellPoint.position, _sellPoint.rotation);
            _player.Camera.PlanarDirection = _sellPoint.forward;
        }
        
        private void OnGatherButtonClick()
        {
            _player.Motor.SetPositionAndRotation(_gatherPoint.position, _gatherPoint.rotation);
            _player.Camera.PlanarDirection = _gatherPoint.forward;
        }
        
        private void DestroyGame()
        {
            if (_player != null)
            {
                Destroy(_player.gameObject);
                _player = null;
            }

            _gameGUI.HideAll();
            _gameGUI.gameObject.SetActive(false);
            _gameGUI.ShopButton.onClick.RemoveListener(OnShopButtonClick);
            _gameGUI.SellButton.onClick.RemoveListener(OnSellButtonClick);
            _gameGUI.GatherButton.onClick.RemoveListener(OnGatherButtonClick);
        }

        private void InitMainMenu()
        {
            _mainMenuGUI.gameObject.SetActive(true);
            _mainMenuCamera.gameObject.SetActive(true);
            _startAction.action.Enable();
        }

        private void DestroyMainMenu()
        {
            _mainMenuGUI.gameObject.SetActive(false);
            _mainMenuCamera.gameObject.SetActive(false);
            _startAction.action.Disable();
        }
    }
}