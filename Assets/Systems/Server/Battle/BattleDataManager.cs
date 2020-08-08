namespace SMBQ.Data
{
    public class BattleDataManager
    {
        public readonly WorldBattle world = new WorldBattle();
        public readonly RaidBattle raid = new RaidBattle();
        public readonly LivePvPBattle livePvP = new LivePvPBattle();
        public readonly AsyncPvPBattle asyncPvP = new AsyncPvPBattle();
    }
}
