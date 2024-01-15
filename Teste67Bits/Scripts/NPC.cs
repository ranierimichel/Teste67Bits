using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour {
    private void Start() {
        StartCoroutine(TryBuyBodies());
    }

    IEnumerator TryBuyBodies() {
        yield return new WaitForSeconds(.5f);
        float interactRange = 5f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange);
        foreach (var collider in colliderArray) {
            if (collider.TryGetComponent(out Player player)) {
                if (player.GetBodiesAmount() >= 1) {
                    player.RemoveLastBody();
                }
            }
        }
        StartCoroutine(TryBuyBodies());
    }
}
