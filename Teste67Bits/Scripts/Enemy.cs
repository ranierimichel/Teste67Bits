using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour {

    private Animator _enemyAnimator;
    private bool _isDead;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Rigidbody[] _hipsSpineRigidBodies;
    [SerializeField] private ParticleSystem _particleSystem;

    private void Awake() {
        _enemyAnimator = GetComponentInChildren<Animator>();
        //StartCoroutine(WaitAnimationToDie());// Debug
    }
    public void Interact() {
        StartCoroutine(WaitPlayerPunchAnimationToDie());
    }

    IEnumerator WaitPlayerPunchAnimationToDie() {
        yield return new WaitForSeconds(1f);
        float force = Random.Range(150, 200);
        float randomDirection = Random.Range(0, 3);
        Vector3 direction;
        if (randomDirection < 1) {
            direction = Vector3.forward;
        } else if (randomDirection < 2) {
            direction = Vector3.right;
        } else {
            direction = Vector3.left;
        }

        _rigidbody.AddForce(Vector3.up * force, ForceMode.Impulse);
        _rigidbody.AddForce(direction * force, ForceMode.Impulse);
        _particleSystem.gameObject.SetActive(true);
        _enemyAnimator.enabled = false;
        _isDead = true;

        float timeToLand = 5f;

        yield return new WaitForSeconds(timeToLand);
        if (gameObject.activeInHierarchy) {
            foreach (var rigidBody in _hipsSpineRigidBodies) {
                rigidBody.isKinematic = true;
            }
        }
    }

    public bool IsDead() {
        return _isDead;
    }
}
