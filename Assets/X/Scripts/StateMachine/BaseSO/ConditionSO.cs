using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ConditionSO : ScriptableObject
{
    [SerializeField] protected int priority;//条件优先级

    [SerializeField] protected EnemyCombat _combatSystem;

    public virtual void Init(StateMachineSystem stateSystem) {
        _combatSystem = stateSystem.GetComponentInChildren<EnemyCombat>();
    }
    
    public abstract bool ConditionSetUp();//条件是否成立

    /// <summary>
    /// 获取当前条件的优先级
    /// </summary>
    /// <returns></returns>
    public int GetConditionPriority() => priority;
}
