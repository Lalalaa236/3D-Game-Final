using UnityEngine;

public class WeaponInstantiator : MonoBehaviour
{
    public Slot slot;
    [SerializeField] private GameObject currentWeaponPrefab;
    [SerializeField] private Transform parentTransform;
    [SerializeField] private Vector3 localPositionOffset;
    [SerializeField] private Vector3 localEulerOffset;
    [SerializeField] private Vector3 localScale = Vector3.one;

    // private void Start()
    // {
    //     if (!weaponPrefab || !rightHandTransform)
    //     {
    //         Debug.LogWarning("WeaponInstantiator: missing prefab or hand transform.");
    //         return;
    //     }

    //     // Instantiate using the hand's world pose, parented to the hand
    //     var go = Instantiate(weaponPrefab,
    //                          rightHandTransform.position,
    //                          rightHandTransform.rotation,
    //                          rightHandTransform);

    //     // Reset local transform (and apply your offsets)
    //     var t = go.transform;
    //     t.localPosition = localPositionOffset;     // usually (0,0,0)
    //     t.localRotation = Quaternion.Euler(localEulerOffset); // usually (0,0,0)
    //     t.localScale = localScale;              // usually (1,1,1)

    //     // Optional: prevent physics from pushing the player
    //     foreach (var rb in go.GetComponentsInChildren<Rigidbody>()) Destroy(rb);
    //     foreach (var col in go.GetComponentsInChildren<Collider>()) col.isTrigger = true;
    // }

    public enum Slot
    {
        RightHand,
        LeftHand
    }

    public void DestroyWeapon()
    {
        if (currentWeaponPrefab != null)
        {
            Destroy(currentWeaponPrefab);
        }
    }

    public void EquipWeapon(GameObject weaponPrefab)
    {
        currentWeaponPrefab = weaponPrefab;

        weaponPrefab.transform.parent = parentTransform;

        weaponPrefab.transform.localPosition = localPositionOffset;
        weaponPrefab.transform.localRotation = Quaternion.Euler(localEulerOffset);
        weaponPrefab.transform.localScale = localScale;
    }
}
