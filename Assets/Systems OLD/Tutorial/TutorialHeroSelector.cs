using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

public class TutorialHeroSelector : MonoBehaviour {

    public HeroSelection[] heroes;

    public HeroSelectInfoDisplay heroInfo;

    public RectTransform arrowRect;

    public GameObject arrow;
    public GameObject chooseYourHero;

    public float arrowSpeed;

    HeroSelection selectedHero;
    HeroSelection hoverHero;

    Vector3 camStartPos;
    Quaternion camStartRot;

    Vector3 arrowTargetPos;

    private void Start()
    {
        camStartPos = Camera.main.transform.position;
        camStartRot = Camera.main.transform.rotation;

        arrow.SetActive(false);
    }

    private void Update()
    {
        if (selectedHero != null)
            return;

        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit heroHit;

        if (Physics.Raycast(camRay, out heroHit, 50f))
        {
            foreach(HeroSelection h in heroes)
            {
                if(h.coll == heroHit.collider)
                {
                    arrowTargetPos = Camera.main.WorldToScreenPoint(h.arrowPos.position);

                    if (h.heroAnim != null && hoverHero != h)
                    {
                        hoverHero = h;
                        h.heroAnim.SetTrigger(h.animTrigger);

                        if(!arrow.activeSelf)
                        {
                            arrow.SetActive(true);
                            arrowRect.position = arrowTargetPos;
                        }
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
						foreach(Animator a in h.transitionAnims)
						{
							a.SetBool("Selected", true);
						}

                        chooseYourHero.SetActive(false);
                        arrow.SetActive(false);
                        selectedHero = h;

                        StartCoroutine("FocusOnHero");
                        return;
                    }
                }
            }
        }

        arrowRect.position = Vector3.Lerp(arrowRect.position, arrowTargetPos, Time.deltaTime * arrowSpeed);
    }

    IEnumerator FocusOnHero()
    {
        while (Camera.main.transform.position != selectedHero.camTransform.position || Camera.main.transform.rotation != selectedHero.camTransform.rotation)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, selectedHero.camTransform.position, Time.smoothDeltaTime * 10f);
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, selectedHero.camTransform.rotation, Time.smoothDeltaTime * 100f);

            yield return null;
        }

        heroInfo.Open(selectedHero);
    }

    public void Back()
    {
        heroInfo.displayObject.SetActive(false);
		foreach (Animator a in selectedHero.transitionAnims)
		{
			a.SetBool("Selected", false);
		}

		StartCoroutine("BackToHeroSelect");
    }

    IEnumerator BackToHeroSelect()
    {
        while (Camera.main.transform.position != camStartPos || Camera.main.transform.rotation != camStartRot)
        {
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, camStartPos, Time.smoothDeltaTime * 10f);
            Camera.main.transform.rotation = Quaternion.RotateTowards(Camera.main.transform.rotation, camStartRot, Time.smoothDeltaTime * 100f);

            yield return null;
        }

        selectedHero = null;
        chooseYourHero.SetActive(true);
        arrow.SetActive(true);
    }

    public void ConfirmHero()
    {
        heroInfo.displayObject.SetActive(false);

        Data.instance.tutorial.ConfirmHero(selectedHero.selectedFaction, HeroConfirmationSuccess);
    }

    void HeroConfirmationSuccess()
    {
        SceneLoader.ChangeScenes("NewTutorial");
    }
}
