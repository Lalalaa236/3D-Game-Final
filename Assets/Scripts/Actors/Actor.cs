using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActor
{
    Transform Transform { get; }
    Animator Animator { get; }
    bool IsPerformingAction { get; set; }
    bool CanMove { get; set; }
    bool CanRotate { get; set; }
}

public interface IMotor
{
    bool IsGrounded { get; }
    void Move(Vector3 delta); // delta is already dt-scaled
}

public interface IDamageable
{
    bool IsInvincible { get; }
    void ApplyDamage(int amount, Vector3 hitPoint);
}
