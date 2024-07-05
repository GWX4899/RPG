using UnityEngine;
using static MyTool;
public class PlayerCombat : CombatBase
{

    [SerializeField, Header("检测敌人")] private Transform detectionCenter;
    [SerializeField] private float detectionRange = 4f;
    [SerializeField] private Transform currentTarget;

    private float mouseButtonDownTime;
    private float mouseButtonUpTime;
    private Collider[] detectionedTarget = new Collider[1];

    [SerializeField] private bool applyAttackInput;
    private void Update()
    {

        DetectionTarget();
        UpdateCurrentTarget();
        PlayerAttackAction();

    }

    private void LateUpdate()
    {
        OnAttackActionLockOn();
        AttackAnimationMove();
    }
    private void AttackAnimationMove()
    {
        if (_animator.CheckAnimationTag("Attack", 0))
        {
            AnimationMovement(_playerMove.GetController(), transform.forward, _animator.GetFloat("AnimationMove") * 10f);
        }

    }

    private void PlayerAttackAction()
    {
        if (!_animator.CheckAnimationTag("Attack", 0))
        {
            SetApplyAttackInput(true);
        }

        if (Input.GetMouseButtonDown(0) && applyAttackInput)
        {
            var temp = transform.eulerAngles;
            temp.y = _playerMove.cameraTransform.eulerAngles.y;
            transform.eulerAngles = temp;
            //Debug.Log(transform.eulerAngles + " " + cameraTransform.eulerAngles);

            _animator.SetFloat("X", 0);
            _animator.SetFloat("Y", 0);
            //MoveLock = true;
            mouseButtonDownTime = Time.time;
            _animator.SetTrigger("LAttack");

            SetApplyAttackInput(false);
        }

        if (Input.GetMouseButton(0) && applyAttackInput)
        {
            mouseButtonUpTime = Time.time;
            _animator.SetFloat("MouseTime", mouseButtonUpTime - mouseButtonDownTime);
            if (mouseButtonUpTime - mouseButtonDownTime <= 0.4f)
            {
                //Debug.Log("Duration Time:" + (mouseButtonUpTime - mouseButtonDownTime));
                //anima.SetTrigger("LAttack");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _animator.SetBool("MouseUp", true);
            //Debug.Log("蓄力攻击！！！！！");
        }

    }

    private void OnAttackActionLockOn()
    {
        if (CanAttackLockOn() && currentTarget != null)
        {
            if (_animator.CheckAnimationTag("Attack", 0))
            {
                //Debug.Log(currentTarget.name);
                transform.root.rotation = transform.LockOnTarget(currentTarget.root.transform, transform, 50f);
            }
        }
    }

    private bool CanAttackLockOn()
    {
        if (_animator.CheckAnimationTag("Attack", 0))
        {
            if (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.75f)
            {
                return true;
            }
        }
        return false;
    }
    private void DetectionTarget()
    {
        int targetCount = Physics.OverlapSphereNonAlloc(detectionCenter.position, detectionRange, detectionedTarget, enemyLayer);

        if (targetCount > 0)
        {
            SetCurrentTarget(detectionedTarget[0].transform.root);
        }
    }

    private void SetCurrentTarget(Transform target)
    {
        if (currentTarget == null || currentTarget != target)
        {
            currentTarget = target;
        }
    }

    private void UpdateCurrentTarget()
    {
        if (_animator.CheckAnimationTag("Move", 0))
        {
            currentTarget = null;
        }
    }

    public bool GetApplyAttackInput() => applyAttackInput;
    public void SetApplyAttackInput(bool apply) => this.applyAttackInput = apply;
}
