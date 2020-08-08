using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SMBQ.Battle
{
    public enum BattleWarningType
    {
        NotEnoughMana,
        NoCardSelected,
        HeroOnCooldown,
        AbilityOnCooldown,
        HeroAlreadyCasting
    }
    
    public class BattleWarning
    {
        //WARNINGS
        public static void Display(BattleWarningType warningType)
        {
            switch (warningType)
            {
                case BattleWarningType.NotEnoughMana:
                    Warning.Display("Not Enough Mana");
                    break;
                case BattleWarningType.NoCardSelected:
                    Warning.Display("No Card Selected");
                    break;
                case BattleWarningType.HeroOnCooldown:
                    Warning.Display("Hero is on Cooldown");
                    break;
                case BattleWarningType.AbilityOnCooldown:
                    Warning.Display("Hero's Ability is on Cooldown");
                    break;
                case BattleWarningType.HeroAlreadyCasting:
                    Warning.Display("Hero is Casting another Ability");
                    break;
            }
        }
    }
}
