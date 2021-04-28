using Mirror;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    [SerializeField] private NetworkAnimator networkAnimator;

    private Animator anim;
 
    // Booleans
    private readonly int isMoving = Animator.StringToHash("isMoving");
    private readonly int isStunned = Animator.StringToHash("isStunned");

    // Triggers
    private readonly int hit = Animator.StringToHash("hit");
    private readonly int die = Animator.StringToHash("die");
    private readonly int attackA = Animator.StringToHash("attackA");
    private readonly int attackB = Animator.StringToHash("attackB");

    public void SetAnimator(Animator anim)
    {
        this.anim = anim;
    }
    
    public void SetMoving(bool m)
    {
        if (!anim) return;
        
        anim.SetBool(isMoving, m);
    }

    public void SetStunned(bool m)
    {
        if (!anim) return;
        
        anim.SetBool(isStunned, m);
    }

    public void SetHit()
    {
        if (!anim) return;
        
        networkAnimator.ResetTrigger(hit);
        networkAnimator.SetTrigger(hit);
    }
    
    public void SetDie()
    {
        if (!anim) return;
        
        networkAnimator.ResetTrigger(die);
        networkAnimator.SetTrigger(die);
    }
    
    public void SetAttackA()
    {
        if (!anim) return;
        
        networkAnimator.ResetTrigger(attackA);
        networkAnimator.SetTrigger(attackA);
    }
    
    public void SetAttackB()
    {
        if (!anim) return;
        
        networkAnimator.ResetTrigger(attackB);
        networkAnimator.SetTrigger(attackB);
    }
}
