using UnityEngine;

[RequireComponent(typeof(PlayerManager))]
public class PlayerActorAdapter : MonoBehaviour, IActor, IMotor, IDamageable
{
    private PlayerManager p;

    void Awake() { p = GetComponent<PlayerManager>(); }

    // IActor
    public Transform Transform => p.transform;
    public Animator Animator => p.animator;
    public bool IsPerformingAction { get => p.isPerformingAction; set => p.isPerformingAction = value; }
    public bool CanMove           { get => p.canMove;            set => p.canMove = value; }
    public bool CanRotate         { get => p.canRotate;          set => p.canRotate = value; }

    // IMotor
    public bool IsGrounded => p.characterController ? p.characterController.isGrounded : true;
    public void Move(Vector3 delta)
    {
        if (p.characterController) p.characterController.Move(delta);
        else                       Transform.position += delta; // fallback
    }

    // IDamageable (hook to your health once you add it)
    public bool IsInvincible => false; // or p.isInvincible if you add that flag
    public void ApplyDamage(int amount, Vector3 hitPoint)
    {
        if (IsInvincible) return;
        // p.ChangeHealthValue(-amount); // when ready
    }
}
