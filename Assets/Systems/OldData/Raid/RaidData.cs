using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RaidData
{
    public int UnlockHours;
    public ChestData Chest;
    public Dictionary<string, string> Difficulty;
    public Dictionary<string, RaidTemplateData> Templates;
}
