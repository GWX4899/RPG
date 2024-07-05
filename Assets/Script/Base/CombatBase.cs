using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MyTool;

public abstract class CombatBase : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    protected AudioSource _audioSource;
    [SerializeField] protected PlayerMovement _playerMove;

    [SerializeField, Header("¹¥»÷¼ì²â")] protected Transform attackDetectionCenter;
    [SerializeField] protected float attackDetectionRang;
    [SerializeField] protected LayerMask enemyLayer;



    // Start is called before the first frame update
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
        _audioSource = GetComponent<AudioSource>();
        _playerMove = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame

    protected virtual void OnAnimationAttackEvent(string hitName)
    {
        //Debug.Log(1);
        if (_animator.CheckAnimationTag("Dodge", 0)) return;
        if (!_animator.CheckAnimationTag("Attack", 0)) return;
        //Debug.Log(2);
        Collider[] attackDetectionTargets = new Collider[7];


        int counts = Physics.OverlapSphereNonAlloc(attackDetectionCenter.position, attackDetectionRang,
            attackDetectionTargets, enemyLayer);

        //Debug.Log(attackDetectionTargets[0] == null ? "¿ÕµÄ" : attackDetectionTargets[0].transform.name);
        if (counts > 0)
        {
            for (int i = 0; i < counts; i++)
            {
                //Debug.Log(attackDetectionTargets[i].TryGetComponent(out IDamage damager));
                if (attackDetectionTargets[i].TryGetComponent(out IDamage damage))
                {
                    //Debug.Log(4);
                    damage.Hurted(0, hitName, transform.root.transform);
                }
                //Debug.Log(Time.time + attackDetectionTargets[i].gameObject.name);
            }
        }
        else
        {
            //Debug.Log(_animator.name);
            if (_animator.GetFloat("Equip") == 2)
            {
                //Debug.Log("´ó½£¹¥»÷");
                GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.GSwordWave);
            }

            else if (_animator.GetFloat("Equip") == 1)
            {
                //Debug.Log("ÆÕÍ¨½£¹¥»÷");
                GameAssets.Instance.PlaySoundEffect(_audioSource, SoundType.SwordWave);
            }

            //Debug.Log("Ã»ÓÐ");
        }
        if (transform.name == "Player")
            _animator.SetBool("MouseUp", false);


    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackDetectionCenter.position, attackDetectionRang);
    }
}
