using System.Collections;
using UnityEngine;

public class CarryBodyHips : MonoBehaviour {
    private bool _isCarrying;
    private Transform _parentTransform;
    private Vector3 _parentPositionDelayed;
    private CarryBodyHips _childrenTransform;

    private void Awake() {
        _isCarrying = false;
        _parentTransform = null;
        _childrenTransform = null;
        StartCoroutine(DelayedParentTransform());
    }

    IEnumerator DelayedParentTransform() {
        yield return new WaitForSeconds(.05f);
        if (_parentTransform != null) {
            _parentPositionDelayed = _parentTransform.position;
        }
        StartCoroutine(DelayedParentTransform());
    }

    public bool IsCarrying() {
        return _isCarrying;
    }

    private void Update() {
        if (_isCarrying) {
            float height = .5f;
            float speed = .1f;
            if (!_parentTransform.TryGetComponent(out CarryBodyHips body)) {
                height = 3f;
                transform.position = _parentTransform.position + Vector3.up * height;
                transform.forward = _parentTransform.forward + Vector3.down;
            } else {
                transform.position = Vector3.Lerp(transform.position, _parentPositionDelayed + Vector3.up * height, speed);
                transform.forward = Vector3.Lerp(transform.forward, _parentTransform.forward, speed);
            }
        }
    }

    public void SetParentTransform(Transform parent) {
        _isCarrying = true;
        _parentTransform = parent;
        _parentPositionDelayed = parent.position;
    }

    public void SetChildrenBody(CarryBodyHips children) {
        _childrenTransform = children;
    }

    public Transform GetTransform() {
        return transform;
    }

    public Transform GetParentTransform() {
        if (_parentTransform != null) {
            return _parentTransform;
        }
        return null;
    }
    public CarryBodyHips GetChildrenBody() {
        if (_childrenTransform != null) {
            return _childrenTransform;
        }
        return null;
    }
}
