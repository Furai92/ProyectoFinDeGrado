using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;

    private void OnEnable()
    {
        EventManager.PlayerDashStartedEvent += OnDashStarted;
        EventManager.PlayerAttackStartedEvent += OnAttackStarted;
        EventManager.PlayerDamageTakenEvent += OnDamageTaken;
        EventManager.PlayerDefeatedEvent += OnDefeated;
    }

    private void OnDisable()
    {
        EventManager.PlayerDashStartedEvent -= OnDashStarted;
        EventManager.PlayerAttackStartedEvent -= OnAttackStarted;
        EventManager.PlayerDamageTakenEvent -= OnDamageTaken;
        EventManager.PlayerDefeatedEvent -= OnDefeated;
    }

    private void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        animator.SetFloat("MoveX", localVelocity.x);
        animator.SetFloat("MoveY", localVelocity.z);

        float speed = new Vector2(localVelocity.x, localVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);
    }

    private void OnDashStarted(Vector3 position, float direction)
    {
        animator.SetTrigger("Dash");
    }

    private void OnAttackStarted(Vector3 position, WeaponSO.WeaponSlot slot, float direction)
    {
        // TEMP DEACTIVATED
        /*

        if (slot == WeaponSO.WeaponSlot.Melee)
            animator.SetTrigger("MeleeAttack");
        else if (slot == WeaponSO.WeaponSlot.Ranged)
            animator.SetTrigger("RangedAttack");
        */
    }

    private void OnDamageTaken(float amount)
    {
        animator.SetTrigger("Hit");
    }

    private void OnDefeated()
    {
        animator.SetTrigger("Defeated");
    }
}
