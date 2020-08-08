using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
using Photon.Realtime;

namespace SMBQ.Battle
{
    public abstract class BattleType<T> : MonoBehaviourPunCallbacks
    {
        protected bool setup = false;
        protected T battleData;

        public static BattleState state;

        public GameSystem gameSystem;
        public InputSystem inputSystem;
        public HeroSystem heroSystem;
        public ManaSystem manaSystem;
        public UnitSystem unitSystem;
        public ProjectileSystem projectileSystem;

        public virtual void Setup(T battleData)
        {
            this.battleData = battleData;
        }

        protected virtual void Init()
        {
            gameSystem.Init(state);
            inputSystem.Init(state);
            heroSystem.Init(state);
            manaSystem.Init(state);
            unitSystem.Init(state);
            projectileSystem.Init(state);

            setup = true;
        }

        protected virtual void Update()
        {
            if (!setup) return;

            gameSystem.Run(state);
            inputSystem.Run(state);
            heroSystem.Run(state);
            manaSystem.Run(state);
            unitSystem.Run(state);
            projectileSystem.Run(state);
        }
    }
}
