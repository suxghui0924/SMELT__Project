using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// 게임 세이브/로드를 전담하는 싱글톤 매니저.
///
/// [사용법]
///   저장: SaveManager.Instance.Save();
///   불러오기: SaveManager.Instance.Load();
///   데이터 읽기: SaveManager.Instance.CurrentData.gold
///
/// ※ 이 파일은 세이브 담당자 외 건드리지 마세요.
/// </summary>
public class SaveManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // 싱글톤
    // ─────────────────────────────────────────
    public static SaveManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _sessionStartTime = Time.time;
    }

    // ─────────────────────────────────────────
    // 설정
    // ─────────────────────────────────────────
    private const string SAVE_FILE_NAME  = "smelt_save.json";
    private const string CURRENT_VERSION = "1.0.0";

    private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // ISaveable을 구현한 모든 매니저가 여기에 등록됨
    private readonly List<ISaveable> _saveables = new List<ISaveable>();

    // 현재 메모리에 올라와 있는 세이브 데이터
    public SaveData CurrentData { get; private set; } = new SaveData();

    // 플레이 타임 측정용
    private float _sessionStartTime;

    // 저장/불러오기 결과를 UI에 알려주는 이벤트
    public event Action<bool, string> OnSaveResult;  // (성공여부, 메시지)
    public event Action<bool, string> OnLoadResult;

    // ─────────────────────────────────────────
    // 시스템 등록 / 해제
    // 각 매니저의 Start()에서 Register(this) 호출
    // ─────────────────────────────────────────
    public void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    public void Unregister(ISaveable saveable) => _saveables.Remove(saveable);

    // ─────────────────────────────────────────
    // 저장 (버튼에서 호출)
    // ─────────────────────────────────────────
    public void Save()
    {
        try
        {
            // 플레이 시간 누적
            CurrentData.playTime  += Time.time - _sessionStartTime;
            _sessionStartTime      = Time.time;
            CurrentData.saveTime   = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            CurrentData.version    = CURRENT_VERSION;

            // 등록된 모든 매니저에서 데이터 수집
            foreach (var s in _saveables)
                s.OnSave(CurrentData);

            string json = JsonUtility.ToJson(CurrentData, prettyPrint: true);
            File.WriteAllText(SavePath, json);

            OnSaveResult?.Invoke(true, $"Save complete\n{CurrentData.saveTime}"); // 저장 완료
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Save failed: {e.Message}"); // 저장 실패
            OnSaveResult?.Invoke(false, "Save failed."); // 저장에 실패했습니다.
        }
    }

    // ─────────────────────────────────────────
    // 불러오기 (버튼에서 호출)
    // ─────────────────────────────────────────
    public bool Load()
    {
        if (!File.Exists(SavePath))
        {
            OnLoadResult?.Invoke(false, "No saved data found."); // 저장된 데이터가 없습니다.
            return false;
        }

        try
        {
            string json     = File.ReadAllText(SavePath);
            SaveData loaded = JsonUtility.FromJson<SaveData>(json);

            if (loaded.version != CURRENT_VERSION)
                Debug.LogWarning($"[SaveManager] Version mismatch: {loaded.version} -> {CURRENT_VERSION}"); // 버전 불일치

            CurrentData = loaded;

            // 등록된 모든 매니저에 데이터 배포
            foreach (var s in _saveables)
                s.OnLoad(CurrentData);

            _sessionStartTime = Time.time;

            OnLoadResult?.Invoke(true, $"Load complete\n{CurrentData.saveTime}"); // 불러오기 완료
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveManager] Load failed: {e.Message}"); // 불러오기 실패
            OnLoadResult?.Invoke(false, "Load failed."); // 불러오기에 실패했습니다.
            return false;
        }
    }

    // ─────────────────────────────────────────
    // 유틸리티
    // ─────────────────────────────────────────
    public bool HasSaveData() => File.Exists(SavePath);
    public void DeleteSave()  { if (File.Exists(SavePath)) File.Delete(SavePath); }
}
