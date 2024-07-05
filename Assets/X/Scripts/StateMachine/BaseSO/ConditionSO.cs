using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class ConditionSO : ScriptableObject
{
    [SerializeField] protected int priority;//�������ȼ�

    [SerializeField] protected EnemyCombat _combatSystem;

    public virtual void Init(StateMachineSystem stateSystem) {
        _combatSystem = stateSystem.GetComponentInChildren<EnemyCombat>();
    }
    
    public abstract bool ConditionSetUp();//�����Ƿ����

    /// <summary>
    /// ��ȡ��ǰ���������ȼ�
    /// </summary>
    /// <returns></returns>
    public int GetConditionPriority() => priority;
}
