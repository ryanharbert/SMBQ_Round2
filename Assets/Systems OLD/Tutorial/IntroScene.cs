using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : MonoBehaviour {

    public Animator anim;
	public Animator skeletonAnim;

    public void Idle()
    {
        anim.SetTrigger("Idle");
    }

    public void Moving()
    {
        anim.SetTrigger("Moving");
    }

    public void Attacking()
    {
        anim.SetTrigger("Attacking");
    }

	public void SkeletonJump()
	{
		skeletonAnim.SetTrigger("Jump");
	}
}
