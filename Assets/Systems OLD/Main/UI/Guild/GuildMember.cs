using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.CloudScriptModels;
using PlayFab;

public class GuildMember : MonoBehaviour
{
	public Text displayNameText;
	public Text	lastLoginText;
	public Text rankText;
	public Button button;
	public GuildMemberData data;

	public void MemberProfile()
	{
		GuildMemberProfile.instance.Set(this);
	}
}
