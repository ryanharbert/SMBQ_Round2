using PlayFab;
using PlayFab.CloudScriptModels;

[System.Serializable]
public class GuildMemberData
{
	public string displayName;
	public string rankName;
	public EntityKey entityKey;
	public long lastLogin;
}
