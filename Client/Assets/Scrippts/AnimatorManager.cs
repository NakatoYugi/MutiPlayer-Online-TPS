using System;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator anim;

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        //使用标准化时间创建从当前状态到任何其他状态的淡入淡出效果。
        anim.CrossFade(targetAnim, 0.2f);
    }


    #region Anim Events
    public virtual void OnAnimEnter() { }

    public virtual void OnAnimExit() 
    {
        anim.SetBool("isInteracting", false);
    }
    #endregion
}