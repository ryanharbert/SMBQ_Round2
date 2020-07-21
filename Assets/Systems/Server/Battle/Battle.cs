namespace SMBQ.Data
{
    public class Battle
    {
        public WorldBattle world = new WorldBattle();
        public RaidBattle raid = new RaidBattle();
        public LivePvPBattle livePvP = new LivePvPBattle();
        public AsyncPvPBattle asyncPvP = new AsyncPvPBattle();
    }
}
