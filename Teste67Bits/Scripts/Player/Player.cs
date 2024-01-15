using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class Player : MonoBehaviour {
    [SerializeField] private Vector2 _joystickSize = new Vector2(300, 300);
    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private Transform _player;
    [SerializeField] private LayerMask _enemiesLayerMask;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    [SerializeField] private Material[] _materials;

    private Finger _movementFinger;
    private Vector2 _movementAmount;
    private bool _isRunning;
    private int _bodiesAmount = 0;
    private int _playerLevel = 1;
    private CarryBodyHips _firstBody;
    private float _movementSpeed = 10f;
    private PlayerAnimator _playerAnimator;

    public static Player Instance { get; private set; }

    public event EventHandler<OnLevelChangedEventArgs> OnLevelChanged;
    public class OnLevelChangedEventArgs : EventArgs {
        public float level;
    }

    private void OnEnable() {
        EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += Touch_onFingerDown;
        ETouch.Touch.onFingerUp += Touch_onFingerUp;
        ETouch.Touch.onFingerMove += Touch_onFingerMove;
    }

    private void OnDisable() {
        ETouch.Touch.onFingerDown -= Touch_onFingerDown;
        ETouch.Touch.onFingerUp -= Touch_onFingerUp;
        ETouch.Touch.onFingerMove -= Touch_onFingerMove;
        EnhancedTouchSupport.Disable();
    }
    private void Awake() {
        Instance = this;
        _playerAnimator = _player.GetComponentInChildren<PlayerAnimator>();
        OnLevelChanged?.Invoke(this, new OnLevelChangedEventArgs {
            level = _playerLevel
        });
    }
    private void Update() {
        if (!_playerAnimator.IsPunching()) {
            HandleMovement();
            HandleInteractions();
        }
    }
    private void Touch_onFingerDown(Finger touchedFinger) {
        if (_movementFinger == null && touchedFinger.screenPosition.x <= Screen.width) {
            _movementFinger = touchedFinger;
            _movementAmount = Vector2.zero;

            _joystick.gameObject.SetActive(true);
            _joystick._recTransform.sizeDelta = _joystickSize;
            _joystick._recTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
        }
    }
    private void Touch_onFingerMove(Finger movedFinger) {
        if (movedFinger == _movementFinger) {
            Vector2 knobPosition;
            float maxMovement = _joystickSize.x / 2f;
            ETouch.Touch currentTouch = movedFinger.currentTouch;

            if (Vector2.Distance(currentTouch.screenPosition, _joystick._recTransform.anchoredPosition) > maxMovement) {
                knobPosition = (currentTouch.screenPosition - _joystick._recTransform.anchoredPosition).normalized * maxMovement;
            } else {
                knobPosition = currentTouch.screenPosition - _joystick._recTransform.anchoredPosition;
            }

            _joystick._knob.anchoredPosition = knobPosition;
            _movementAmount = knobPosition / maxMovement;
        }
    }
    private void Touch_onFingerUp(Finger lostFinger) {
        if (lostFinger == _movementFinger) {
            _movementFinger = null;
            _joystick._knob.anchoredPosition = Vector2.zero;
            _joystick.gameObject.SetActive(false);
            _movementAmount = Vector2.zero;
        }
    }


    private void HandleInteractions() {
        PunchInteraction();
        if (_bodiesAmount < _playerLevel) {
            GrabBodiesInteraction();
        }
    }

    private void PunchInteraction() {
        Vector3 moveDir = new Vector3(_movementAmount.x, 0, _movementAmount.y);

        float interactDistance = .5f;

        if (Physics.Raycast(transform.position, moveDir, out RaycastHit raycastHit, interactDistance, _enemiesLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out Enemy enemy)) {
                if (!enemy.IsDead()) {
                    enemy.Interact();
                    _playerAnimator.PunchAnimation();
                }
            }
        }
    }

    private void GrabBodiesInteraction() {
        float interactRange = 1f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, _enemiesLayerMask);
        CarryBodyHips lastBody;
        foreach (var collider in colliderArray) {
            if (collider.TryGetComponent(out CarryBodyHips body)) {
                if (!body.IsCarrying()) {
                    _bodiesAmount++;
                    // If first body
                    if (_bodiesAmount <= 1) {
                        body.SetParentTransform(transform);
                        _firstBody = body;
                    } else {
                        // If Second body
                        if (_firstBody.GetChildrenBody() == null) {
                            _firstBody.SetChildrenBody(body);
                            body.SetParentTransform(_firstBody.GetTransform());
                        } else {
                            lastBody = _firstBody.GetChildrenBody();
                            while (lastBody.GetChildrenBody() != null) {
                                lastBody = lastBody.GetChildrenBody();
                            }
                            lastBody.SetChildrenBody(body);
                            body.SetParentTransform(lastBody.GetTransform());
                        }
                    }
                }

            }
        }
    }

    private void HandleMovement() {
        Vector3 moveDir = new Vector3(_movementAmount.x, 0, _movementAmount.y);
        //_movementSpeed = 10f + (_playerLevel / 2) - (_bodiesAmount / 2);
        float moveDistance = _movementSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance, _obstaclesLayerMask);

        if (!canMove) {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance, _obstaclesLayerMask);

            if (canMove) {
                moveDir = moveDirX;
            } else {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance, _obstaclesLayerMask);

                if (canMove) {
                    moveDir = moveDirZ;
                }
            }
        }

        Vector3 scaledMovement = moveDistance * moveDir;

        if (canMove) {
            _player.transform.position += scaledMovement;
            _isRunning = moveDir != Vector3.zero;
        }

        //_player.transform.forward = moveDir;
        float rotateSpeed = 20f;
        _player.transform.forward = Vector3.Slerp(_player.transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }

    private Vector2 ClampStartPosition(Vector2 startPosition) {
        if (startPosition.x > Screen.width - _joystickSize.x) {
            startPosition.x = Screen.width - _joystickSize.x;
        }

        if (startPosition.y > Screen.height - _joystickSize.y) {
            startPosition.y = Screen.height - _joystickSize.y;
        }

        return startPosition;
    }

    public bool IsRunning() {
        return _isRunning;
    }

    public void RemoveLastBody() {
        float bodyPrice = 10f;
        if (_bodiesAmount == 1) {
            _bodiesAmount--;
            Destroy(_firstBody.GetComponentInParent<Enemy>().gameObject);
            if (TesteGameManager.Instance) {
                TesteGameManager.Instance.IncreaseGoldCallEvent(bodyPrice);
            }
        } else {
            CarryBodyHips lastBody = _firstBody.GetChildrenBody();
            while (lastBody.GetChildrenBody() != null) {
                lastBody = lastBody.GetChildrenBody();
            }
            _bodiesAmount--;
            lastBody.GetParentTransform().GetComponent<CarryBodyHips>().SetChildrenBody(null);
            Destroy(lastBody.GetComponentInParent<Enemy>().gameObject);
            if (TesteGameManager.Instance) {
                TesteGameManager.Instance.IncreaseGoldCallEvent(bodyPrice);
            }
        }
    }

    public int GetBodiesAmount() {
        return _bodiesAmount;
    }

    public void IncreasePlayerLevel() {
        _playerLevel++;
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = _materials[UnityEngine.Random.Range(0, _materials.Length)];
        OnLevelChanged?.Invoke(this, new OnLevelChangedEventArgs {
            level = _playerLevel
        });
    }

}
