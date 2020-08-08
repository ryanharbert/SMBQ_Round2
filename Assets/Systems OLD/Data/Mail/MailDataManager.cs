using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

[Serializable]
public class MailDataManager
{
    public List<MailData> mailList { get; private set; }
    public int newMailCount { get; private set; }

    public delegate void NewMailReceived();
    public NewMailReceived newMailReceived;
    
    DateTime lastMailCheck;

    Action individualMailCallback;
    int individualMailIndex;

    public void Login(GetPlayerCombinedInfoResultPayload playerInfo)
    {
        UserDataRecord userDataRecord;

        newMailCount = 0;
        if (playerInfo.UserReadOnlyData.TryGetValue("Mail", out userDataRecord))
        {
            SetMailListFromData(userDataRecord.Value);
        }
        lastMailCheck = DateTime.UtcNow;
    }

    void SetMailListFromData(string json)
    {
        mailList = PlayFabSimpleJson.DeserializeObject<List<MailData>>(json);
        foreach (MailData m in mailList)
        {
            if (m.New)
            {
                newMailCount++;
            }
        }
    }

    #region Check Mail
    public void CheckForMail()
    {
        TimeSpan t = DateTime.UtcNow - lastMailCheck;
        if (t.TotalMinutes > 5)
        {
            PlayFabClientAPI.GetUserReadOnlyData(new GetUserDataRequest() { Keys = new List<string>() { "Mail" } }, MailCheckReturned, GetMailFailure);
            lastMailCheck = DateTime.UtcNow;
        }
    }

    void MailCheckReturned(GetUserDataResult result)
    {
        UserDataRecord userDataRecord;
        if (result.Data.TryGetValue("Mail", out userDataRecord))
        {
            SetMailListFromData(userDataRecord.Value);
        }
        newMailReceived.Invoke();
    }

    void GetMailFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Mark Read
    public void MarkMailRead()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "markMailRead" }, MarkMailReadReturned, MailError);
    }

    void MarkMailReadReturned(ExecuteCloudScriptResult result)
    {
    }

    void MailError(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion

    #region Individual Mail
    public void OpenMail(int index, Action callback)
    {
        individualMailIndex = index;
        individualMailCallback = callback;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "openMail", GeneratePlayStreamEvent = true, FunctionParameter = new { index = index } }, OpenReturned, IndividualMailFailure);
    }

    public void DeleteMail(int index, Action callback)
    {
        individualMailIndex = index;
        individualMailCallback = callback;
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest() { FunctionName = "deleteMail", FunctionParameter = new { index = index } }, MailDeleted, IndividualMailFailure);
    }

    public void OpenReturned(ExecuteCloudScriptResult result)
    {
        if (result.FunctionResult != null)
        {
            Data.instance.chests.OpenChest((JsonObject)result.FunctionResult);

            mailList[individualMailIndex].Preset = null;
            mailList[individualMailIndex].Chest = null;
            mailList.RemoveAt(individualMailIndex);
        }
        else
        {
            Debug.LogError("Chest does not exist.");
        }
    }

    public void MailDeleted(ExecuteCloudScriptResult result)
    {
        mailList.RemoveAt(individualMailIndex);
        individualMailCallback.Invoke();
    }

    void IndividualMailFailure(PlayFabError error)
    {
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }
    #endregion
}
