using UnityEngine;

public class WeaponInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject weaponPrefab;
    [SerializeField] private Transform rightHandTransform;
    [SerializeField] private Vector3 localPositionOffset;   // tweak in Inspector
    [SerializeField] private Vector3 localEulerOffset;      // tweak in Inspector
    [SerializeField] private Vector3 localScale = Vector3.one;

    private void Start()
    {
        if (!weaponPrefab || !rightHandTransform)
        {
            Debug.LogWarning("WeaponInstantiator: missing prefab or hand transform.");
            return;
        }

        // Instantiate using the hand's world pose, parented to the hand
        var go = Instantiate(weaponPrefab,
                             rightHandTransform.position,
                             rightHandTransform.rotation,
                             rightHandTransform);

        // Reset local transform (and apply your offsets)
        var t = go.transform;
        t.localPosition = localPositionOffset;     // usually (0,0,0)
        t.localRotation = Quaternion.Euler(localEulerOffset); // usually (0,0,0)
        t.localScale    = localScale;              // usually (1,1,1)

        // Optional: prevent physics from pushing the player
        foreach (var rb in go.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
        foreach (var col in go.GetComponentsInChildren<Collider>()) col.isTrigger = true;
    }
}
