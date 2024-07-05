using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateActionSO : ScriptableObject
{
    [SerializeField] protected int statePriority;//״̬���ȼ�

    [SerializeField] protected Animator _animator;
    [SerializeField] protected EnemyCombat _combatSystem;
    [SerializeField] protected EnemyMovement _movement;

    protected int verticalID = Animator.StringToHash("Vertical");
    protected int horizontalID = Animator.StringToHash("Horizontal");
    protected int runID = Animator.StringToHash("Run");
    public virtual void OnEnter(StateMachineSystem stateMachineSystem) {
        _animator = stateMachineSystem.GetComponentInChildren<Animator>();
        _combatSystem = stateMachineSystem.GetComponentInChildren<EnemyCombat>();
        _movement = stateMachineSystem.GetComponentInChildren<EnemyMovement>();
    }

    public abstract void OnUpdate();

    public virtual void OnExit() { }

    /// <summary>
    /// ��ȡ״̬���ȼ�
    /// </summary>
    /// <returns></returns>
    public int GetStatePriority() => statePriority;
}
