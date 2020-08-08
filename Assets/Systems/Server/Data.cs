using System;
using System.Diagnostics.CodeAnalysis;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Json;
using UnityEngine;

namespace SMBQ.Data
{
    public class Data : MonoBehaviour
    {
        private static Data _instance;

        public static Data Instance
        {
            get
            {
                if (!_instance)
                {
                    GameObject g = new GameObject();
                    
                    _instance = g.AddComponent<Data>();
                }
                return _instance;
            }
        }

        public bool offline = true;
        
        public string DisplayName { get; internal set; }
        
        internal string playfabID;
        internal string entityID;
        internal string entityType;

        internal PlayFab.CloudScriptModels.EntityKey entityKey;

        public readonly BattleDataManager Battle = new BattleDataManager();
        
        public readonly CurrencyDataManager Currency = new CurrencyDataManager();

        private bool set = false;

        private void Awake()
        {
            _instance = this;
        }

        public void Login(Action loginComplete)
        {
            
            
            entityKey = new PlayFab.CloudScriptModels.EntityKey() { Id = entityID, Type = entityType };
            
            loginComplete.Invoke();
            set = true;
        }

        private void Update()
        {
            if (!set) return;
            
            Currency.Update();
        }
    }
}
