using UnityEngine.SceneManagement;

namespace _01_Scripts._Core._States
{
    public class LobbyState : IGameState
    {
        public void Enter()
        {
            VolumeManager.instance.VolumeChange(VolumeType.Start, 1.0f);
            SceneManager.sceneLoaded += OnLobbyLoaded;
            SceneLoader.LoadScene("Lobby");
        }

        private void OnLobbyLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "Lobby") return;
            SceneManager.sceneLoaded -= OnLobbyLoaded;
            if (UICanvasManager.instance != null)
                UICanvasManager.instance.SetCanvasActive(CanvasType.Title, true);
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}