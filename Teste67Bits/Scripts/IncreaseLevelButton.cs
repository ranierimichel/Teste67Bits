using UnityEngine;
using UnityEngine.UI;

public class IncreaseLevelButton : MonoBehaviour {
    private Image _buttonImage;
    private float _lvlUpPrice = 10f;
    private float _gold;
    private void Awake() {
        _buttonImage = GetComponent<Image>();
        _buttonImage.color = new Color(_buttonImage.color.r, _buttonImage.color.g, _buttonImage.color.b, .5f);

        PlayerInputActions playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.LevelUp.performed += PlayerInputActions_LevelUp_performed;
    }

    private void PlayerInputActions_LevelUp_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        if (_gold >= _lvlUpPrice) {
            Player.Instance.IncreasePlayerLevel();
            TesteGameManager.Instance.DecreaseGoldCallEvent(_lvlUpPrice);
        }
    }

    private void Start() {
        if (TesteGameManager.Instance) {
            TesteGameManager.Instance.OnGoldChanged += TesteGameManager_OnGoldChanged;
        }
    }

    private void TesteGameManager_OnGoldChanged(object sender, TesteGameManager.OnGoldChangedEventArgs e) {
        _gold = e.gold;
        if (e.gold >= _lvlUpPrice) {
            _buttonImage.color = new Color(_buttonImage.color.r, _buttonImage.color.g, _buttonImage.color.b, 1f);
        } else {
            _buttonImage.color = new Color(_buttonImage.color.r, _buttonImage.color.g, _buttonImage.color.b, .5f);
        }
    }
}
