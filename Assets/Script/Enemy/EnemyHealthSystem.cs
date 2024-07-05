using UnityEngine;
public class EnemyHealthSystem : CharacterHealthSystemBase
{

    private void LateUpdate()
    {
        OnHitLockTarget(currentAttacker);
    }

    public override void Hurted(float damage, string HitAnimation, Transform attacker)
    {
        SetAttacker(attacker);
        _animator.Play(HitAnimation, 0, 0f);
        GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.Hurt);
    }

    private void OnHitLockTarget(Transform attacker)
    {
        if (_animator.CheckAnimationTag("Hit", 0) || _animator.CheckAnimationTag("GSHit", 0))
        {
            transform.rotation = transform.LockOnTarget(attacker, transform, 50f);
        }
    }
}
