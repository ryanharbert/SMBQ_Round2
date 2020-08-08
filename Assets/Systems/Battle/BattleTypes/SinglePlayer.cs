using Photon.Pun;
using Photon.Realtime;

namespace SMBQ.Battle
{
    public class SinglePlayer : BattleType<bool>
    {
        public override void Setup(bool battleData)
        {
            base.Setup(battleData);

            PhotonNetwork.OfflineMode = true;
            CreateRoom();
        }

        private const string roomName = "Room Name";

        void CreateRoom()
        {
            RoomOptions roomOptions = new RoomOptions();
            roomOptions.IsVisible = false;
            roomOptions.MaxPlayers = 1;
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }

        public override void OnCreatedRoom()
        {
            Init();
        }
    }
}
