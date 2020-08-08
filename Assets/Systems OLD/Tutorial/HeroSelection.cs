using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroSelection : MonoBehaviour
{
    public string strongholdItemId;
    public string itemId;
    public string displayName;
	[TextArea]
	public string description;
	[TextArea]
	public string startingUnits;
	public string stratDesc;
    public Faction faction;
    public Collider coll;
    public Animator heroAnim;
	public Animator[] transitionAnims;
	public string animTrigger;
    public Transform arrowPos;
    public Transform camTransform;
	public string selectedFaction;
}
