using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAttackInput : StateMachineBehaviour
{


    private PlayerCombat combatSystem;

    [SerializeField] private float maxApplyAttackTime;
    private float currentApplyTime;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //获取玩家系统攻击脚本
        if (combatSystem == null)
            combatSystem = animator.GetComponent<PlayerCombat>();

        currentApplyTime = maxApplyAttackTime;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!combatSystem.GetApplyAttackInput())
        {
            if (currentApplyTime > 0)
            {
                currentApplyTime -= Time.deltaTime;

                if (currentApplyTime <= 0)
                {
                    combatSystem.SetApplyAttackInput(true);
                }
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }
}
