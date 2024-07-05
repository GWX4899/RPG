using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AICombat", menuName = "StateMachine/State/AICombat")]
public class AICombat : StateActionSO
{

    private int randomHorizontal;




    [SerializeField] private CombatAbilityBase currentAbility;
    public override void OnUpdate()
    {

        AICombatAction();

    }

    public override void OnExit()
    {

    }

    private void AICombatAction()
    {
        if (currentAbility == null)
        {
            NoCombatMove();
            GetAbility();
        }
        else
        {
            currentAbility.InvokeAbility();
            if (!currentAbility.GetAbilityIsDone())
            {
                currentAbility = null;
            }

        }
    }

    private void GetAbility()
    {
        if (currentAbility == null)
        {
            currentAbility = _combatSystem.GetAnDoneAbility();
        }
    }

    private void NoCombatMove()
    {
        if (_animator.CheckAnimationTag("Motion", 0))
        {
            if (_combatSystem.GetCurrentTargetDistance() < 2f + .1f)
            {
                _movement.CharacterMoveInterface(-_combatSystem.GetDirectionForTarget(), 1.4f, true);
                _animator.SetFloat(verticalID, -1f, 0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, 0f, 0.25f, Time.deltaTime);

                randomHorizontal = GetRandomHorizontal();

                if (_combatSystem.GetCurrentTargetDistance() < 1.5 + .05f)
                {
                    if (!_animator.CheckAnimationTag("Hit", 0) || !_animator.CheckAnimationTag("Defen", 0))
                    {
                        _animator.Play("LAtk_0", 0, 0f);
                        randomHorizontal = GetRandomHorizontal();
                    }
                }
            }
            else if (_combatSystem.GetCurrentTargetDistance() > 2f + .1f && _combatSystem.GetCurrentTargetDistance() < 6.1 + .5f)
            {
                _movement.CharacterMoveInterface(_movement.transform.right * ((randomHorizontal == 0) ? 1 : randomHorizontal), 1.4f, true);
                _animator.SetFloat(verticalID, 0f, 0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, ((randomHorizontal == 0) ? 1 : randomHorizontal), 0.25f, Time.deltaTime);
            }
            else if (_combatSystem.GetCurrentTargetDistance() > 6.1 + .5f)
            {
                _movement.CharacterMoveInterface(_movement.transform.forward, 1.4f, true);
                _animator.SetFloat(verticalID, 1f, 0.25f, Time.deltaTime);
                _animator.SetFloat(horizontalID, 0f, 0.25f, Time.deltaTime);

            }



        }
        else
        {
            _animator.SetFloat(verticalID, 0f);
            _animator.SetFloat(horizontalID, 0f);
            _animator.SetFloat(runID, 0f);
        }
    }

    private int GetRandomHorizontal() => Random.Range(-1, 2);

}
