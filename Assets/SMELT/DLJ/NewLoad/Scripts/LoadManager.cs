using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class LoadManager : MonoBehaviour
{
    private float _minLoadTime = 1f;

    private void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    IEnumerator LoadAsyncScene()
    {
        _minLoadTime = 0;
        yield return null;
        // 매니저에 저장된 다음 씬 이름을 비동기로 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(SceneLoader.nextScene);
        op.allowSceneActivation = false; // 100% 로드되어도 바로 넘어가지 않게 방지
        float timer = 0.0f;
        while (!op.isDone)
        {
            yield return null;
            timer += Time.unscaledDeltaTime;
            if (op.progress >= 0.9f && timer >= _minLoadTime)
            {
                UICanvasManager.instance.FadeStart();
                op.allowSceneActivation = true;
                switch (SceneLoader.nextScene)
                {
                    case "House":
                        Debug.Log("asd");
                        UICanvasManager.instance.SetCanvasActive(CanvasType.Hud, true);
                        break;
                    case "Lobby":
                        SoundManager.instance.PlayBGM("Lobby");
                        UICanvasManager.instance.SetCanvasActive(CanvasType.Title, true);
                        break;
                }
            }
        }


    }
}