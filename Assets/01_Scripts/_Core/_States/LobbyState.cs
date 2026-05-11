namespace _01_Scripts._Core._States
{
    public class LobbyState : IGameState
    {
        public void Enter()
        {
            SceneLoader.LoadScene("Lobby");
        }

        public void Execute()
        {
            throw new System.NotImplementedException();
        }

        public void Exit()
        {
        }
    }
}