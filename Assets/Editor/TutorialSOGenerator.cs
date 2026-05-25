using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class TutorialSOGenerator
{
    private const string OutputFolder = "Assets/01_Scripts/_Core/Tutorial/GeneratedData";

    [MenuItem("Tools/Tutorial/SO 자동 생성")]
    public static void Generate()
    {
        if (!AssetDatabase.IsValidFolder("Assets/01_Scripts/_Core/Tutorial/GeneratedData"))
            AssetDatabase.CreateFolder("Assets/01_Scripts/_Core/Tutorial", "GeneratedData");

        var steps = new List<TutorialStepSO>();

        // ── 가게 시작 ─────────────────────────────────────────────
        steps.Add(Step("01_Start", "공주", new[]
        {
            "안녕? 난 이 섬의 공주야, 잘부탁해!",
            "넌 지금부터 이 상점을 운영하게 될 거야.",
            "우선 무기 제작에 필요한 재료를 모아보자!",
            "WASD를 눌러서 오른쪽에 있는 빨간 카펫으로 이동해 보자!",
        }));

        // ── 던전 입장 ─────────────────────────────────────────────
        steps.Add(Step("02_EnterDungeon", "공주", new[]
        {
            "E키를 눌러서 입장해보자.",
        }));

        // ── 던전 기력 설명 ────────────────────────────────────────
        steps.Add(Step("03_Dungeon_Stamina", "공주", new[]
        {
            "오른쪽 아래에 있는 게이지는 기력 게이지야!",
            "기력은 시간에 따라 소진되기도 하지만 적과 닿아도 소진돼!",
            "기력이 모두 소진되면 그 날은 던전에 들어올 수 없으니 명심해!",
        }));

        // ── 적 등장 알림 ──────────────────────────────────────────
        steps.Add(Step("04_EnemyAppear", "공주", new[]
        {
            "앗! 저기 사과석 한 마리가 나타났어!",
        }));

        // ── D키 공격 (KeyPress 조건) ──────────────────────────────
        var attackR = Step("05_AttackRight", "공주", new[]
        {
            "D키를 눌러 공격해보자!",
        });
        attackR.condition = TutorialAdvanceCondition.KeyPress;
        attackR.waitKey = KeyCode.D;
        steps.Add(attackR);

        // ── A키 공격 (KeyPress 조건) ──────────────────────────────
        var attackL = Step("06_AttackLeft", "공주", new[]
        {
            "A키를 눌러 공격해보자!",
        });
        attackL.condition = TutorialAdvanceCondition.KeyPress;
        attackL.waitKey = KeyCode.A;
        steps.Add(attackL);

        // ── 던전 복귀 ─────────────────────────────────────────────
        steps.Add(Step("07_DungeonHome", "공주", new[]
        {
            "HOME 버튼을 누르면 중간에 가게로 돌아갈 수 있어.",
        }));

        // ── 상단 UI 설명 ──────────────────────────────────────────
        steps.Add(Step("08_TopUI", "공주", new[]
        {
            "가장 상단에 있는 UI는 보유 재화와 보유 재료를 뜻해.",
            "이제 재료도 있으니 문으로 가서 E를 눌러 가게를 열고 손님을 맞이해봐.",
        }));

        // ── 손님 주문 설명 ────────────────────────────────────────
        steps.Add(Step("09_CustomerOrder", "공주", new[]
        {
            "앗, 손님이 방문했어!",
            "주문 게시판을 봐보자!",
            "이건 주문에 대한 요청이야.",
            "이 게이지는 손님의 인내심을 뜻해!",
            "인내심은 시간에 따라 깎이고, 받는 돈이 떨어져.",
            "인내심이 모두 달면 손님이 떠나버리고 돈도 깎이니 조심해!",
            "재료도 있으니 만들어 보자.",
            "모루가 있는 곳으로 이동해보자!",
        }));

        // ── 모루 상호작용 ─────────────────────────────────────────
        steps.Add(Step("10_AnvilInteract", "공주", new[]
        {
            "E키를 눌러보자.",
        }));

        // ── 무기 제작 설명 ────────────────────────────────────────
        steps.Add(Step("11_WeaponCraft", "공주", new[]
        {
            "모루 위에서 E키를 누르면 무기를 제작할 수 있어!",
            "상단은 무기의 종류, 중간은 주 재료, 하단은 부 재료야.",
            "주 재료에 따라 무기의 이름과 외형이 달라지니 잘 골라봐!",
            "손님이 원하는 재료로 맞춰서 만들어줘야 해. 한번 만들어보자!",
        }));

        // ── 무기 판매 ─────────────────────────────────────────────
        steps.Add(Step("12_SellWeapon1", "공주", new[]
        {
            "무기를 만들었으니 이제 손님한테 가서 팔아보자!",
            "손님 근처로 이동하면 판매 버튼이 나타날 거야.",
        }));

        steps.Add(Step("13_SellWeapon2", "공주", new[]
        {
            "드디어 첫 판매구나! 수고했어!",
            "이제 업그레이드를 해볼까?",
            "가게 안에 나무 오브젝트가 보이지? 거기로 이동해봐!",
        }));

        // ── 업그레이드 ────────────────────────────────────────────
        steps.Add(Step("14_UpgradeOpen", "공주", new[]
        {
            "나무 오브젝트 앞에서 E키를 눌러봐.",
        }));

        steps.Add(Step("15_UpgradeExplain", "공주", new[]
        {
            "여기서 곡괭이 업그레이드와 스킬 트리를 관리할 수 있어!",
            "왼쪽 버튼으로 곡괭이 업그레이드와 스킬 트리를 전환할 수 있어.",
            "스킬 트리에선 다양한 능력을 해금할 수 있으니 재료가 모이면 꼭 써봐!",
            "재료가 충분하면 곡괭이도 업그레이드해서 더 강해질 수 있어.",
            "다 봤으면 우측 상단 X 버튼을 눌러 나가자.",
        }));

        // ── 할당량 / 일차 ─────────────────────────────────────────
        steps.Add(Step("16_Quota", "공주", new[]
        {
            "우측 하단에 뭔가 보이지?",
            "이건 우리가 하루 동안 채워야할 할당량과 일차야.",
            "시간 제한이 끝나면 폐업해야해.",
            "7일차까지 끝내게 되면 보스에 도전할 수 있어!",
            "채우지 못하면 폐업하게 되니 조심해.",
        }));

        // ── 라디오 ────────────────────────────────────────────────
        steps.Add(Step("17_Radio", "공주", new[]
        {
            "라디오는 E키를 누르고 창을 열어 노래를 바꿀 수 있어.",
        }));

        // ── 캘린더 ────────────────────────────────────────────────
        steps.Add(Step("18_Calendar", "공주", new[]
        {
            "캘린더는 E키를 누르고 창을 열면 할당량만큼 돈이 있다면 다음 날짜로 갈 수 있어.",
        }));

        // ── 마무리 ────────────────────────────────────────────────
        steps.Add(Step("19_End", "공주", new[]
        {
            "이제 내가 알려줄 건 다 알려준 거 같네, 화이팅해봐~!",
        }));

        // ── TutorialSequenceSO 생성 ───────────────────────────────
        var sequence = ScriptableObject.CreateInstance<TutorialSequenceSO>();
        sequence.steps = steps.ToArray();
        AssetDatabase.CreateAsset(sequence, OutputFolder + "/MainTutorialSequence.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"[TutorialSOGenerator] Step {steps.Count}개 + Sequence 1개 → {OutputFolder}");
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = sequence;
    }

    private static TutorialStepSO Step(string fileName, string charName, string[] lines)
    {
        var so = ScriptableObject.CreateInstance<TutorialStepSO>();
        so.characterName = charName;
        so.lines = lines;
        so.condition = TutorialAdvanceCondition.Click;
        so.freezeTime = false;
        AssetDatabase.CreateAsset(so, $"{OutputFolder}/{fileName}.asset");
        return so;
    }
}
