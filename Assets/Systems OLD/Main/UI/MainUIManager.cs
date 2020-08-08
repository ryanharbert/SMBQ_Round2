using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    public NavBar navBar;
    public EnterShopUI enterShopUI;
    public TravelUI travelUI;
    public ChestLootDisplay chestLootDisplay;

    [SerializeField] private ChatWindow main;
    [SerializeField] private ChatWindow guild;


    public void Setup()
    {
        if (main != null)
        {
            ChatManager.instance.worldChat = main;
            ChatManager.instance.guildChat = guild;
        }
    }
}
