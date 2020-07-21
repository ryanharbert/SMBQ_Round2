using System;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

namespace SMBQ.Data
{
    public class Data : MonoBehaviour
    {
        public static Data instance;
        
        public string displayName { get; internal set; }
        
        internal string playfabID;
        internal string entityID;
        internal string entityType;

        internal PlayFab.CloudScriptModels.EntityKey entityKey;

        public Battle battle = new Battle();
        
        internal Currency currency = new Currency();

        private void Awake()
        {
            instance = this;
        }

        public void Login(Action loginComplete)
        {
            
            
            entityKey = new PlayFab.CloudScriptModels.EntityKey() { Id = entityID, Type = entityType };
            
            loginComplete.Invoke();
        }
    }
}
