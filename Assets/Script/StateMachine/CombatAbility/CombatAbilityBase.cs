using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatAbilityBase : ScriptableObject
{
    [SerializeField] protected string abilityName;
    [SerializeField] protected int abilityID;
    [SerializeField] protected float abilityCDTime;
    [SerializeField] protected float abilityUseDistance;
    [SerializeField] protected bool abilityIsDone;


    protected Animator animator;
    protected EnemyCombat combat;
    protected CharacterMovementBase movement;


    protected int verticalID = Animator.StringToHash("Vertical");
    protected int horizontalID = Animator.StringToHash("Horizontal");
    protected int runID = Animator.StringToHash("Run");

    //�ⲿ���ü���
    public abstract void InvokeAbility();

    protected void UseAbility()
    {
        animator.Play(abilityName, 0, 0);
        abilityIsDone = false;

        ResetAbility();
    }

    public void ResetAbility()
    {
        //����CD
        //���ȣ�����ȥ�����������һ����ʱ��������Ȼ��ͨ���ó����ļ�ʱ��ȥ��ȡ�����ϵļ�ʱ�ű��еĴ�����ʱ���ĺ���
        //�������ʱ��ݼ�Ϊ0��ʱ���ڲ�ִ��һ��ί�У����ί�о��ǽ���abilityIsDone���true
        GameObjectPoolSystem.Instance.TakeGameObject("Timer")
            .GetComponent<Timer>().CreateTime(abilityCDTime, () => abilityIsDone = true, false);
    }

    #region Interface

    public void InitAbility(Animator _animator, EnemyCombat _combat, CharacterMovementBase _movement)
    {
        this.animator = _animator;
        this.combat = _combat;
        this.movement = _movement;
    }

    public string GetAbilityName() => abilityName;
    public int GetAbilityID() => abilityID;
    public bool GetAbilityIsDone() => abilityIsDone;
    public void SetAbilityDone(bool done) => abilityIsDone = done;
    #endregion

}
