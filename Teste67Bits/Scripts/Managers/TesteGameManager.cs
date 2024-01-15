using System;
using UnityEngine;

public class TesteGameManager : MonoBehaviour {
    public static TesteGameManager Instance { get; private set; }

    private float _gold;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        _gold = 0f;
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs {
            gold = _gold,
        });
    }

    public event EventHandler<OnGoldChangedEventArgs> OnGoldChanged;
    public class OnGoldChangedEventArgs : EventArgs {
        public float gold;
    }

    public void IncreaseGoldCallEvent(float amount) {
        _gold += amount;
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs {
            gold = _gold,
        });
    }
    public void DecreaseGoldCallEvent(float amount) {
        _gold -= amount;
        OnGoldChanged?.Invoke(this, new OnGoldChangedEventArgs {
            gold = _gold,
        });
    }
}
