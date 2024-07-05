using UnityEngine;
public class PlayerHealthSystem : CharacterHealthSystemBase
{
    
    public Transform GSParryTransform;
    private Transform GSTransform;
    private void LateUpdate()
    {
        ResetGSTransform();
        OnHitLockTarget(currentAttacker);
    }

    public override void Hurted(float damage, string HitAnimation, Transform attacker)
    {
        SetAttacker(attacker);
        if (CanParry())
        {
            Parry(HitAnimation);
        }
        else
        {
            if (_playerMove.GetCurrentWeapon().name == "Sword")
                _animator.Play(HitAnimation, 0, 0f);
            else if (_playerMove.GetCurrentWeapon().name == "GS")
                _animator.Play(("GS" + HitAnimation), 0, 0f);
            GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.Hurt);
        }
        transform.rotation = transform.LockOnTarget(currentAttacker, transform, 50f);

    }

    private void OnHitLockTarget(Transform attacker)
    {
        if (_animator.CheckAnimationTag("Hit", 0) || _animator.CheckAnimationTag("GSHit", 0) || _animator.CheckAnimationTag("ParryHit", 0))
        {
            transform.rotation = transform.LockOnTarget(attacker, transform, 50f);
        }
    }


    #region 格挡

    private bool CanParry()
    {
        //if (_animator.CheckCurrentTagAnimationTimeIsLow("Hit", .1f)) return false;

        return true;
    }

    private void Parry(string hitname)
    {
        if (!CanParry()) return;
        if (_playerMove.GetCurrentWeapon().name == "Sword")
        {
            switch (hitname)
            {

                case "Hit_LD_Up":
                    _animator.Play("ParryLF", 0, 0);
                    break;
                case "Hit_RD_Up":
                    _animator.Play("ParryRF", 0, 0);
                    break;
                case "Hit_Up_Right":
                    _animator.Play("ParryUp", 0, 0);
                    break;
                case "Hit_H_Right":
                    _animator.Play("ParryR", 0, 0);
                    break;
                default:
                    break;
            }
            GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.SwordDefend);
        }
        else if (_playerMove.GetCurrentWeapon().name == "GS")
        {
            SetGSParryTransform();
            _animator.Play("GSParry", 0, 0);
            GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.GSwordDefend);
        }

    }
    private void SetGSParryTransform()
    {
        GSTransform = _playerMove.GS.transform;
        _playerMove.GS.transform.position = GSParryTransform.position;
        _playerMove.GS.transform.rotation = GSParryTransform.rotation;
    }

    private void ResetGSTransform()
    {
        if (_animator.CheckCurrentTagAnimationTimeIsExceed("GSParry", .9f))
        {
            _playerMove.GS.transform.position = GSTransform.position;
            _playerMove.GS.transform.rotation = GSTransform.rotation;
        }
    }
    #endregion

}
