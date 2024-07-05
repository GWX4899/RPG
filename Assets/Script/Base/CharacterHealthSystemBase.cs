using System;
using UnityEngine;
using static MyTool;

public abstract class CharacterHealthSystemBase : MonoBehaviour, IDamage
{
    protected Animator _animator;
    protected CharacterController _controller;
    protected AudioSource _audioSource;
    protected PlayerMovement _playerMove;

    protected Transform currentAttacker;

    //AnimationID
    protected int animationMove = Animator.StringToHash("AnimationMove");


    [SerializeField, Header("受击动画播放速率")] private float hitAnimationMult;


    public virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = GetComponent<CharacterController>();
        _audioSource = GetComponent<AudioSource>();
        _playerMove = GetComponent<PlayerMovement>();
    }




    public virtual void FixedUpdate()
    {
        HitAnimaitonMove();

        //_playerMove.MoveLocked(false);
    }

    public virtual void SetAttacker(Transform attacker)
    {
        if (currentAttacker != attacker || currentAttacker == null)
            currentAttacker = attacker;
    }

    protected virtual void HitAnimaitonMove()
    {
        if (!_animator.CheckAnimationTag("Hit", 0) && !_animator.CheckAnimationTag("GSHit", 0)) return;
        //Debug.Log((_animator.CheckAnimationTag("Hit", 0) ? 1 : 2));
        AnimationMovement(_controller, transform.forward, _animator.GetFloat(animationMove) * hitAnimationMult * (_animator.CheckAnimationTag("Hit", 0) ? 1 : 1.5f));
        
    }


    public virtual void Hurted(float damage)
    {
        throw new NotImplementedException();
    }

    public virtual void Hurted(string HitAnimation)
    {
        
    }

    public virtual void Hurted(float damage, string HitAnimation,Transform attacker)
    {
        throw new NotImplementedException();
    }
}
