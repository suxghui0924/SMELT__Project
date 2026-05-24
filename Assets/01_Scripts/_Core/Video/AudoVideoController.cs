using System;
using System.Collections;
using _01_Scripts._Core._States;
using UnityEngine;
using UnityEngine.Video;

namespace DefaultNamespace
{
    public class AudoVideoController : UnityEngine.MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        private WaitForSeconds wait;

        private void Awake()
        {
            videoPlayer = gameObject.GetComponent<VideoPlayer>();
            wait = new WaitForSeconds(26);
        }

        private void OnEnable()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Play();
                StartCoroutine(OnVideoCheck());
            }
        }
        
        private void OnDisable()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }
        }

        private IEnumerator OnVideoCheck()
        {
            yield return wait;
            UICanvasManager.instance.ControlObject(ObjectType.Hackboom, false);
            GameManager.instance.ChangeState(new LobbyState());
        }
    }
}