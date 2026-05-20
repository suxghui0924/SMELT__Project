namespace _01_Scripts._Core._States
{
    public class LobbyState : IGameState
    {
        public void Enter()
        {
            VolumeManager.instance.VolumeChange(VolumeType.Start, 1.0f);
            SceneLoader.LoadScene("Lobby");
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}