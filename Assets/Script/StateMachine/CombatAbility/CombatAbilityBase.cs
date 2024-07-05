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

    //外部调用技能
    public abstract void InvokeAbility();

    protected void UseAbility()
    {
        animator.Play(abilityName, 0, 0);
        abilityIsDone = false;

        ResetAbility();
    }

    public void ResetAbility()
    {
        //技能CD
        //首先，我们去对象池里面拿一个计时器出来，然后通过拿出来的计时器去获取它身上的计时脚本中的创建计时器的函数
        //当传入的时间递减为0的时候，内部执行一个委托，这个委托就是将将abilityIsDone变成true
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
