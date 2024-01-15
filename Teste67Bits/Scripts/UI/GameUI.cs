using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] TextMeshProUGUI _playerLevel;

    private void Start() {
        _goldText.text = "0";
        if (TesteGameManager.Instance) {
            TesteGameManager.Instance.OnGoldChanged += TesteGameManager_OnGoldChanged;
        }
        if (Player.Instance) {
            Player.Instance.OnLevelChanged += Player_OnLevelChanged;
        }

    }

    private void Player_OnLevelChanged(object sender, Player.OnLevelChangedEventArgs e) {
        _playerLevel.text = e.level.ToString();
    }

    private void TesteGameManager_OnGoldChanged(object sender, TesteGameManager.OnGoldChangedEventArgs e) {
        _goldText.text = e.gold.ToString();
    }
}
