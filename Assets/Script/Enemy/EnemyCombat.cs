using System.Collections.Generic;
using UnityEngine;

public class EnemyCombat : CombatBase
{
    [SerializeField, Header("检测中心")] private Transform detectionCenter;
    [SerializeField, Header("检测范围")] private float detectionRange;
    [SerializeField, Header("检测敌人")] LayerMask whatisEnemy;
    [SerializeField, Header("检测障碍物")] private LayerMask whatisBos;

    [SerializeField, Header("目标")] Collider[] colliderTarget = new Collider[1];

    [SerializeField, Header("当前目标")] private Transform currentTarget;

    private CharacterMovementBase _characterMovementBase;


    [SerializeField, Header("动画播放倍率")] private float AnimationMoveMult;

    private int lockID = Animator.StringToHash("LockOn");
    private int animationMoveID = Animator.StringToHash("AnimationMove");

    [SerializeField, Header("技能搭配")] private List<CombatAbilityBase> abilities = new List<CombatAbilityBase>();

    private void Start()
    {
        _characterMovementBase = GetComponent<CharacterMovementBase>();
        InitAbility();
    }
    private void Update()
    {
        //Debug.Log(GetCurrentTargetDistance());
        AIView();
        LockOnTarget();
        UpdateAniamtionMove();
    }

    private void FixUpdate()
    {
        
    }

    private void AIView()
    {
        int targetcount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRange, colliderTarget, whatisEnemy);

        if (targetcount > 0)
        {
            if (!Physics.Raycast((transform.root.position + transform.root.up * .5f), (colliderTarget[0].transform.root.position - transform.root.position).normalized, out var hit, detectionRange, whatisBos))
            {
                if (Vector3.Dot((colliderTarget[0].transform.root.position - transform.root.position).normalized, transform.root.forward) > 0.45f)
                {
                    currentTarget = colliderTarget[0].transform;
                }
                //Debug.Log(Time.time + "  1" + ((hit.collider == null) ? "空的" : hit.transform.name));
            }
            else
            {
                //Debug.Log(Time.time + "  2" + ((hit.collider == null) ? "空的" : hit.transform.name));
                currentTarget = null;
            }
        }
    }

    private void LockOnTarget()
    {
        if (_animator.CheckAnimationTag("Motion", 0) && currentTarget != null)
        {
            _animator.SetFloat(lockID, 1f);
            //Debug.Log(transform.name);
            transform.root.rotation = transform.LockOnTarget(currentTarget, transform.root, 50);
        }
        else if (_animator.CheckAnimationTag("Attack", 0))
        {
            transform.root.rotation = transform.LockOnTarget(currentTarget, transform.root, 50);
        }
        else
        {
            _animator.SetFloat(lockID, 0f);
        }
    }


    public Transform GetCurrentTarget()
    {
        if (currentTarget == null) return null;

        return currentTarget;
    }

    private void UpdateAniamtionMove()
    {
        if (_animator.CheckAnimationTag("Roll", 0))
        {
            _characterMovementBase.CharacterMoveInterface(transform.root.forward, _animator.GetFloat(animationMoveID) * AnimationMoveMult, true);
        }
        if (_animator.CheckAnimationTag("Attack", 0))
        {
            _characterMovementBase.CharacterMoveInterface(transform.root.forward, _animator.GetFloat(animationMoveID) * AnimationMoveMult, true);
        }
    }

    public Vector3 GetDirectionForTarget()
    {
        if (currentTarget == null) return Vector3.zero;
        return (currentTarget.position - transform.root.position).normalized;

    }

    public float GetCurrentTargetDistance()
    {
        if (currentTarget == null) return 100;
        return Vector3.Distance(currentTarget.position, transform.root.position);
    }


    #region 技能

    private void InitAbility()
    {
        if (abilities.Count == 0) return;

        for (int i = 0; i < abilities.Count; i++)
        {
            abilities[i].InitAbility(_animator, this, _characterMovementBase);

            if (!abilities[i].GetAbilityIsDone())
            {
                abilities[i].ResetAbility();
            }
        }
    }

    public CombatAbilityBase GetAnDoneAbility()
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].GetAbilityIsDone())
            {
                return abilities[i];
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    public CombatAbilityBase GetAbilityUseName(string name)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].GetAbilityName().Equals(name))
            {
                return abilities[i];
            }
            else
            {
                continue;
            }
        }
        return null;
    }

    public CombatAbilityBase GetAbilityUseID(int ID)
    {
        for (int i = 0; i < abilities.Count; i++)
        {
            if (abilities[i].GetAbilityID() == ID)
            {
                return abilities[i];
            }
            else
            {
                continue;
            }
        }
        return null;
    }
    #endregion

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(detectionCenter.position, detectionRange);
        //Gizmos.DrawLine((transform.root.position + transform.root.up * .5f), colliderTarget[0].transform.root.position);
    }
}
