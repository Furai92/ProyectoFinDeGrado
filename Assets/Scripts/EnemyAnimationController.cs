using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody rb;
    private void Update()
    {
        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);

        animator.SetFloat("MoveX", localVelocity.x);
        animator.SetFloat("MoveY", localVelocity.z);

        float speed = new Vector2(localVelocity.x, localVelocity.z).magnitude;
        animator.SetFloat("Speed", speed);
    }
}
