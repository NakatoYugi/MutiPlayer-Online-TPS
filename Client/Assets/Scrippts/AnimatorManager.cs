using System;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    public Animator anim;

    public void PlayTargetAnimation(string targetAnim, bool isInteracting)
    {
        anim.applyRootMotion = isInteracting;
        anim.SetBool("isInteracting", isInteracting);
        //ʹ�ñ�׼��ʱ�䴴���ӵ�ǰ״̬���κ�����״̬�ĵ��뵭��Ч����
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