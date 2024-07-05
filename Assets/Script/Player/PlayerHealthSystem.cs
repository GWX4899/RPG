using UnityEngine;
public class PlayerHealthSystem : CharacterHealthSystemBase
{

    public Transform GSParryTransform;
    private Vector3 position;
    private Vector3 rotation;

    private void Start()
    {
        position = _playerMove.GS.transform.localPosition;
        rotation = _playerMove.GS.transform.localEulerAngles;
    }
    private void LateUpdate()
    {
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
        if (_animator.CheckAnimationTag("Parry", 0)) return true;
        if (_animator.CheckCurrentTagAnimationTimeIsLow("Hit", .11f)) return true;

        return false;
    }

    private void Parry(string hitname)
    {
        //Debug.Log("1");
        if (!CanParry()) return;
        //Debug.Log(_playerMove.GetCurrentWeapon().name);
        if (_playerMove.GetCurrentWeapon().name == "Sword")
        {
            //Debug.Log("判断成功");
            switch (hitname)
            {

                case "Hit_LD_Up":
                    _animator.Play("ParryLF", 0, 0);
                    //Debug.Log("ParryLF");
                    break;
                case "Hit_RD_Up":
                    _animator.Play("ParryRF", 0, 0);
                    //Debug.Log("ParryRF");
                    break;
                case "Hit_Up_Right":
                    _animator.Play("ParryUp", 0, 0);
                    //Debug.Log("ParryUp");
                    break;
                case "Hit_H_Right":
                    _animator.Play("ParryR", 0, 0);
                    //Debug.Log("ParryR");
                    break;
                default:
                    //Debug.Log("没有找到播放动画");
                    break;
            }
            GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.SwordDefend);
        }
        else if (_playerMove.GetCurrentWeapon().name == "GS")
        {
            SetGSParryTransform();
            _animator.Play("GSParry", 0, 0);
            GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.GSwordDefend);
            //Invoke("ResetGSTransform", 1f);
            ResetGSTransform();
        }
    }
    private void SetGSParryTransform()
    {

        Debug.Log(position + " " + rotation);
        _playerMove.GS.transform.position = GSParryTransform.position;
        _playerMove.GS.transform.rotation = GSParryTransform.rotation;
    }

    private void ResetGSTransform()
    {
        _playerMove.GS.transform.localPosition = position;
        _playerMove.GS.transform.localEulerAngles = rotation;
    }
    #endregion

}
