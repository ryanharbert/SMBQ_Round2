using UnityEngine;
using UnityEngine.UI;

public class ManaBar : MonoBehaviour
{
	public Slider manaProgress;
	public Slider currentMana;
	public Text manaText;
	public Animator manaAnim;
	
	bool full = false;

	private void Update()
	{
		if(!Battle.instance.setup)
			return;
		
		manaText.text = Battle.state.currentMana.ToString();
        currentMana.value = Battle.state.currentMana;
        manaProgress.value = Battle.state.currentMana + Battle.state.manaProgress;

		if(Battle.state.currentMana == Battle.state.maxMana && !full)
		{
			manaAnim.SetBool("ManaFull", true);
			full = true;
		}
		else if (Battle.state.currentMana != Battle.state.maxMana && full)
		{
			manaAnim.SetBool("ManaFull", false);
			full = false;
		}
	}
}
