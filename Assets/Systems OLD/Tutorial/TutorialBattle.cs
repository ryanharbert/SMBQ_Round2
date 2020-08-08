using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialBattle : MonoBehaviour
{
    public Animator anim;

    private void Start()
    {
        StartCoroutine("Sequence");
    }

    void Update ()
    {
        if(Battle.instance == null)
            return;

        if(Battle.state.gameOver)
        {
            SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("TutorialBattle"));
        }
    }

    public IEnumerator Sequence()
    {
        while(Battle.state == null || !Battle.state.playCard)
        {
            yield return null;
		}
		
		anim.SetTrigger("NextStep");

        while(Battle.state.hero[0] == null && Battle.state.hero[1] == null)
        {
            yield return null; ;
        }
		
		anim.SetTrigger("NextStep");
        
		while ((Battle.state.hero[0] != null && Battle.state.hero[0].abilities[0].CDTimer == 0) || (Battle.state.hero[1] != null && Battle.state.hero[1].abilities[0].CDTimer == 0))
		{
			yield return null;
		}
		
		anim.SetTrigger("NextStep");
	}
}
