using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }
    IEnumerator LoadAsyncScene()
    {
        // 매니저에 저장된 다음 씬 이름을 비동기로 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.nextScene);
        op.allowSceneActivation = false; // 100% 로드되어도 바로 넘어가지 않게 방지

        yield return new WaitForSeconds(3f);
        
        UICanvasManager.instance.FadeStart();
        op.allowSceneActivation = true;
        switch(SceneLoader.nextScene.ToString())
        {
            case "House":
                UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
                break;
            case "Lobby":
                UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, false);
                break;
        }
    }
}