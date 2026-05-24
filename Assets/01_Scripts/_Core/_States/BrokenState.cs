namespace _01_Scripts._Core._States
{
    public class BrokenState : IGameState
    {
        public void Enter()
        {
            SoundManager.instance.StopBGM();
            SoundManager.instance.PlaySFX("BrokenStart");
            UICanvasManager.instance.ControlObject(ObjectType.Hackboom, true);
            
        }

        public void Execute()
        {
        }

        public void Exit()
        {
        }
    }
}