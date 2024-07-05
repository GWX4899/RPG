using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NormalAbility", menuName = "Ability/NormalAbility")]
public class NormalAbility : CombatAbilityBase
{
    public override void InvokeAbility()
    {
        //�����߼�
        if (animator.CheckAnimationTag("Motion", 0) && abilityIsDone)
        {
            if (combat.GetCurrentTargetDistance() > abilityUseDistance + 0.1f)
            {
                //�����ܱ�����ʱ����û�н��뼼�ܵĹ�����Χ��AI�߹�ȥ

                movement.CharacterMoveInterface(combat.GetDirectionForTarget(), 1.4f, true);
                animator.SetFloat(verticalID, 1f, 0.25f, Time.deltaTime);
                animator.SetFloat(horizontalID, 0f, 0.25f, Time.deltaTime);
                //animator.SetFloat(runID, 1f, 0.25f, Time.deltaTime);


            }
            else
            {
                UseAbility();
            }
        }
        
    }

}
