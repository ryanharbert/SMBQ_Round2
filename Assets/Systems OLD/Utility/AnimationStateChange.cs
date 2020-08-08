using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationStateChange : MonoBehaviour {

	public Animator anim;

	public void Fire()
	{
		anim.SetTrigger("Arrow Attack");
	}

	public void Cast()
	{
		anim.SetTrigger("Cast1");
	}

	public void Move()
	{
		anim.SetTrigger("Moving");
	}

	public void Idle()
	{
		anim.SetTrigger("Idle");
	}
}
