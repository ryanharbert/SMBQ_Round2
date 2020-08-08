using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;


[System.Serializable]
public class MailData
{
	public string Title;
	public string Desc;
	public bool New;
	public string Chest;
	public object Preset;
}
