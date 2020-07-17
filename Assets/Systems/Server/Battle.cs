using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;

namespace SMBQ.Server
{
    public class Battle
    {
        
        #region World
        
        #endregion
        
        #region Raid
        
        #endregion
        
        #region Live PVP
        
        #endregion
        
        #region Async PVP
        public void AsyncWin()
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "endPvPFightv3", GeneratePlayStreamEvent = true, FunctionParameter = new { opponent = Data.instance.pvpBattle.enemyName, win = true } }, PvPAsyncRewards, ChestSlotFilledFailure);
        }

        public void AsyncLose()
        {
            PlayFab.CloudScriptModels.EntityKey e = new PlayFab.CloudScriptModels.EntityKey() { Id = Data.instance.entityID, Type = Data.instance.entityType };
            PlayFabCloudScriptAPI.ExecuteEntityCloudScript(new PlayFab.CloudScriptModels.ExecuteEntityCloudScriptRequest() { Entity = e, FunctionName = "endPvPFightv3", FunctionParameter = new { opponent = Data.instance.pvpBattle.enemyName, win = false } }, PvPAsyncRewards, ChestSlotFilledFailure);
        }
        
        #endregion
    }
}
