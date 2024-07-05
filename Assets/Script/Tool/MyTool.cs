using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MyTool
{

    // Start is called before the first frame update
    public static bool CheckAnimationTag(this Animator animator, string tagName, int animationLayer)
    {
        return animator.GetCurrentAnimatorStateInfo(animationLayer).IsTag(tagName);
    }

    public static void AnimationMovement(CharacterController controller, Vector3 moveDirection, float moveSpeed)
    {
        //Debug.Log(Time.time + "方向:" + moveDirection + "速度:" + moveSpeed);
        controller.Move((moveSpeed * Time.deltaTime) * moveDirection.normalized);

    }

    public static bool CheckCurrentTagAnimationTimeIsExceed(this Animator animator,string tagname,float time)
    {
        if (animator.CheckAnimationTag(tagname, 0))
        {
            return (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > time) ? true : false;
        }
        return false;
    }

    public static bool CheckCurrentTagAnimationTimeIsLow(this Animator animator, string tagname, float time)
    {
        if (animator.CheckAnimationTag(tagname, 0))
        {
            return (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < time) ? true : false;
        }
        return false;
    }

    public static Quaternion LockOnTarget(this Transform transform, Transform target, Transform self, float lerpTime)
    {
        if (target == null) return self.rotation;
        //Debug.Log(111);

        Vector3 targetDirection = (target.position - self.position).normalized;
        targetDirection.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(targetDirection);

        //Debug.Log(targetDirection);

        return Quaternion.Lerp(self.rotation, newRotation, lerpTime * Time.deltaTime);
    }
}
