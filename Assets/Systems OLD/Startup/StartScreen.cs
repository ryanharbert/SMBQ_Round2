using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Text statusText;

    [SerializeField] private GameObject[] loginObjects;

    [SerializeField] private GameObject chooseAudio;

    [SerializeField] private GameObject enterName;
    [SerializeField] private InputField enterNameIF;

    Action<string> setNameCallback;

    private void Start()
    {
        Data.instance.platform.Login(LoginSuccess, NewUser, Failure);
    }

    void LoginSuccess()
    {
        AudioManager.instance.StartMusic();
        StartGame();
    }

    void NewUser()
    {
        foreach (GameObject go in loginObjects)
        {
            go.SetActive(false);
        }

#if UNITY_WEBGL
        ChooseAudio();
#else
        EnterName();
        AudioManager.instance.StartMusic();
#endif
    }

    void Failure()
    {
        statusText.text = "Login Failed Please Reload";
    }

    #region Choose Sound
    void ChooseAudio()
    {
        chooseAudio.SetActive(true);
    }

    void PlayWithSound()
    {
        AudioManager.instance.StartMusic();
        chooseAudio.SetActive(false);
        EnterName();
    }

    void NoSound()
    {
        AudioManager.instance.Mute();
        chooseAudio.SetActive(false);
        EnterName();
    }
    #endregion

    #region Enter Name
    void EnterName()
    {
        enterName.SetActive(true);
#if UNITY_WEBGL
        enterNameIF.text = KongregateAPIBehaviour.instance.username;
#endif
    }

    public void SaveName()
    {
        if(enterNameIF.text.Length >= 3)
        {
            enterName.SetActive(false);
            foreach (GameObject go in loginObjects)
            {
                go.SetActive(true);
            }
            statusText.text = "Saving display name...";
            Data.instance.tutorial.SetNewPlayerName(enterNameIF.text, LoginSuccess, NameTakenCallback, Failure);
        }
        else
        {
            Warning.instance.Activate("Name must be 3 or more characters");
        }
    }

    private void NameTakenCallback()
    {
        Warning.instance.Activate("Name Already Taken");
        enterName.SetActive(true);
        foreach (GameObject go in loginObjects)
        {
            go.SetActive(false);
        }
    }
    #endregion

    void StartGame()
    {
        if (!Data.instance.tutorial.steps["ChooseHero"])
        {
            SceneLoader.ChangeScenes("FactionSelect");
        }
        else
        {
            SceneLoader.ChangeScenes("WorldMap");
        }
    }
}
