using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DayTimer : MonoBehaviour, ISaveable
{
    public static DayTimer Instance { get; private set; }

    private const float MAX_TIME = 600f; // 10분

    private float _remainingTime;
    private bool _isRunning;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _remainingTime = MAX_TIME;
    }

    private void Start()
    {
        SaveManager.Instance?.Register(this);
    }

    private void OnDestroy()
    {
        SaveManager.Instance?.Unregister(this);
    }

    public void OnSave(SaveData data)
    {
        data.remainingTime = _remainingTime;
    }

    public void OnLoad(SaveData data)
    {
        _remainingTime = data.remainingTime > 0f ? data.remainingTime : MAX_TIME;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Lobby")
            PauseTimer();
        else
        {
            if (_remainingTime <= 0f)
                _remainingTime = MAX_TIME;
            StartTimer();
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            _remainingTime = 30f;
            _isRunning = true;
        }
#endif

        if (!_isRunning) return;

        _remainingTime -= Time.deltaTime;

        if (_remainingTime <= 0f)
        {
            _remainingTime = 0f;
            _isRunning = false;
            UICanvasManager.instance?.UpdateTimerDisplay("00:00", true);
            GameManager.instance.ChangeState(new GameOverState());
            return;
        }

        int minutes = Mathf.FloorToInt(_remainingTime / 60f);
        int seconds = Mathf.FloorToInt(_remainingTime % 60f);
        bool isWarning = _remainingTime <= 60f;
        UICanvasManager.instance?.UpdateTimerDisplay($"{minutes:00}:{seconds:00}", isWarning);
    }

    public void StartTimer()
    {
        _isRunning = true;
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResetTimer()
    {
        _remainingTime = MAX_TIME;
        _isRunning = true;
        UICanvasManager.instance?.UpdateTimerDisplay("10:00", false);
    }
}
