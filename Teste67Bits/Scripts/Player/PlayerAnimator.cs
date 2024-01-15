using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    private const string IS_RUNNING = "IsRunning";
    private const string IS_PUNCHING = "IsPunching";

    [SerializeField] private Player _player;

    private Animator _animator;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void Update() {
        if (_animator.GetBool(IS_PUNCHING) == false) {
            _animator.SetBool(IS_RUNNING, _player.IsRunning());
        }
    }

    public void PunchAnimation() {
        _animator.SetBool(IS_PUNCHING, true);
        StartCoroutine(StopPunchAnimation());
    }

    public bool IsPunching() {
        return _animator.GetBool(IS_PUNCHING);
    }

    IEnumerator StopPunchAnimation() {
        yield return new WaitForSeconds(1.5f);
        _animator.SetBool(IS_PUNCHING, false);
    }
}
