using KinematicCharacterController;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Project.Source
{
    public class PlayerController : MonoBehaviour
    {
        private static Texture2D _cursorTexture;

        [Header("Services")]
        [SerializeField] private ExampleCharacterController _character;
        [SerializeField] private ExampleCharacterCamera _cameraController;
        [SerializeField] private TriggerZone _triggerZone;
        private GUIController _GUI;

        [Header("Input")]
        [SerializeField] private InputActionReference _moveAction;
        [SerializeField] private InputActionReference _jumpAction;

        [SerializeField] private InputActionReference _lookAction;
        [SerializeField] private InputActionReference _zoomAction;
        [SerializeField] private InputActionReference _shiftLockAction;
        [SerializeField] private InputActionReference _pointerAction;

        [Header("Cursor")]
        [SerializeField] private float _shiftLockXOffset;

        private bool _cursorLocked;
        private bool _cursorWasLockedBeforeWindow;

        private Quaternion _cameraRotationBuffer;
        
        public KinematicCharacterMotor Motor => _character.Motor;
        public ExampleCharacterCamera Camera => _cameraController;

        public void Init(GUIController GUI)
        {
            _cursorTexture = Resources.Load<Texture2D>("Cursor_texture");

            _GUI = GUI;
            _GUI.windowOpened += OnWindowOpened;
            _GUI.windowClosed += OnWindowClosed;

            _shiftLockAction.action.started += OnShiftLockStarted;

            _triggerZone.onTriggerEnter += ZoneTriggerEnter;
            _triggerZone.onTriggerExit += ZoneTriggerExit;
        }

        private void Start()
        {
            _cameraController.SetFollowTransform(_character.CameraFollowPoint);
            _cameraController.IgnoredColliders.Clear();
            _cameraController.IgnoredColliders.AddRange(_character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (_cameraController.RotateWithPhysicsMover && _character.Motor.AttachedRigidbody)
            {
                // _cameraController.PlanarDirection =
                //     _physicsMover.RotationDeltaFromInterpolation * _cameraController.PlanarDirection;
                _cameraController.PlanarDirection = Vector3
                    .ProjectOnPlane(_cameraController.PlanarDirection, _character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void OnDestroy()
        {
            _GUI.windowOpened -= OnWindowOpened;
            _GUI.windowClosed -= OnWindowClosed;
            _shiftLockAction.action.started -= OnShiftLockStarted;
            _triggerZone.onTriggerEnter -= ZoneTriggerEnter;
            _triggerZone.onTriggerExit -= ZoneTriggerExit;
        }

        private void ZoneTriggerEnter(Collider other) => ValidateTriggerEntered(other, true);
        private void ZoneTriggerExit(Collider other) => ValidateTriggerEntered(other, false);

        private void OnWindowOpened(GUIWindow _)
        {
            if (_cursorLocked)
            {
                UnlockCursor();
                _cursorWasLockedBeforeWindow = true;
            }
            else
            {
                _cursorWasLockedBeforeWindow = false;
            }
        }

        private void OnWindowClosed(GUIWindow _)
        {
            if (_cursorWasLockedBeforeWindow)
            {
                LockCursor();
            }
        }

        private void OnShiftLockStarted(InputAction.CallbackContext _)
        {
            if (_GUI.AnyWindowOpened) return;
            if (_cursorLocked)
                UnlockCursor();
            else
                LockCursor();
        }

        private void ValidateTriggerEntered(Collider other, bool entered)
        {
            switch (other.tag)
            {
                case Tags.ShopZone:
                {
                    if (entered)
                        _GUI.ShopWindow.Open();
                    else
                        _GUI.ShopWindow.Close();
                    break;
                }
                case Tags.Coin:
                {
                    if (other.TryGetComponent<CoinController>(out var coin))
                        PlayerProgressStorage.Data.Coins.Collect(coin);

                    break;
                }
                default:
                    Debug.LogWarning($"{other.gameObject.name} has no suitable tag.");
                    break;
            }
        }

        private void HandleCameraInput()
        {
            var lookInputVector = Vector3.zero;
            var scrollInput = -_zoomAction.action.ReadValue<float>();

            if (_cursorLocked)
            {
                lookInputVector = _lookAction.action.ReadValue<Vector2>();
                BufferCameraRotation();
            }
            else if (_pointerAction.action.inProgress)
            {
                lookInputVector = _lookAction.action.ReadValue<Vector2>();
            }

            _cameraController.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
        }


        private void HandleCharacterInput()
        {
            var moveInput = _moveAction.action.ReadValue<Vector2>();
            if (moveInput.magnitude >= 1) BufferCameraRotation();

            var characterInputs = new PlayerCharacterInputs
            {
                MoveAxisForward = moveInput.y,
                MoveAxisRight = moveInput.x,
                JumpDown = _jumpAction.action.inProgress,
                CameraRotation = _cameraRotationBuffer,
            };

            _character.SetInputs(ref characterInputs);
        }

        private void BufferCameraRotation()
        {
            _cameraRotationBuffer = _cameraController.Transform.rotation;
        }

        private void LockCursor()
        {
            _cursorLocked = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.SetCursor(_cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
            _cameraController.FollowPointFraming.x = _shiftLockXOffset;
            _character.OrientationMethod = OrientationMethod.TowardsCamera;
        }

        private void UnlockCursor()
        {
            _cursorLocked = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            _cameraController.FollowPointFraming.x = 0;
            _character.OrientationMethod = OrientationMethod.TowardsMovement;
        }
    }
}