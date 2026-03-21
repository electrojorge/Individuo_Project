using UnityEngine;

public class UnitsAnimator : MonoBehaviour
{
    Animator animator;
    BattleSystem BS;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        BS = BattleSystem.instance;

    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isDead", true);
        animator.SetBool("isAttacking", false);
        animator.SetBool("isHit", false);
        animator.SetBool("isIdle", false);
    }
}
