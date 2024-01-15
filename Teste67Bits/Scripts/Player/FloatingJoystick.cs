using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]

public class FloatingJoystick : MonoBehaviour
{
    [HideInInspector]
    public RectTransform _recTransform;
    public RectTransform _knob;

    private void Awake() {
        _recTransform = GetComponent<RectTransform>();
    }
}
