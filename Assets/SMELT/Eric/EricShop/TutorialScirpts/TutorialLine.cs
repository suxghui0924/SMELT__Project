using System;
using UnityEngine;

public class TutorialLine : MonoBehaviour
{
    public static TutorialLine Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public string[] enterDungeon1 =
    {
        "우선 무기 제작에 필요한 재료를 모아보자!",
        "WASD를 눌러서 오른쪽에 있는 빨간 카펫으로 이동해 보자!",
    };

    public string[] enterDungeon2 =
    {
        "E키를 눌러서 입장해보자.",
    };
    public string[] dungeon1 =
    {
        "오른쪽 아래에 있는 게이지는 기력 게이지야!",
        "기력은 시간에 따라 소진되기도 하지만 적과 닿아도 소진돼!",
        "기력이 모두 소진되면 그 날은 던전에 들어올 수 없으니 명심해!.",
    };

    public string[] dungeon2 =
    {
        "앗! 저기 사과석 한 마리가 나타났어!.",
    };    
    public string[] dungeon3 =
    {
        "D  키를 눌러 공격해보자!.",
    };   
    public string[] dungeon4 =
    {
        "4마리만 더 공격해보자!",
    };
    public string[] dungeon5 =
    {
        "이 버튼을 누르면 중간에 가게로 돌아갈 수 있어.",
    };
    public string[] getOrder1 =
    {
        "앗, 손님이 방문하셨어!",
    };

    public string[] getOrder2 =
    {
        "주문 게시판을 봐보자!",
        "이건 주문에 대한 요청이야 .",
        "이 게이지는 손님의 인내심을 뜻해!",
        "인내심이 모두 달면 손님이 떠나버리니 조심해!",
        "사과석 검을 만들어 달라네 한번 재료도 있으니 만들어 보자",
    };
    public string[] makeWeapon1 =
    {
        "모루가 있는 곳으로 이동해보자!",
    };

    public string[] makeWeapon2 =
    {
        "E키를 눌러보자",
        "사과석 검을 만들어 달라고 했고 멜론석도 섞어달라고 했으니 만들어 보자.",
    };
    public string[] sellWeapon1 =
    {
        "무기를 만들었으니 손님께 이동해 팔아보자!",
    };

    public string[] sellWeapon2 =
    {
        "드디어 첫판매구나!",
        "이제 업그레이드를 해볼까?",
    };
    public string[] store1 =
    {
        "빛나는 곳으로 이동해보자!",
    };

    public string[] store2 =
    {
        "업그레이드를 해보자!",
    };
    public string[] store3 =
    {
        "좋았어! 이제 곡괭이 업그레이드도 알려줄게.",
        "이 버튼을 눌러보자.",
        "재료가 모이면 여기서 곡괭이를 업그레이드 할 수 있어!",
    };
    public string[] minMoney =
    {
        "이건 우리가 하루 동안 채워야할 할당량이야",
        "채우지 못하면 폐업하게 되니 할당량 지킬 수 있게 잘 해봐!",
        "그리고 7일차까지 끝내게 되면 보스에 도전할 수 있어!",
        "이제 난 가볼게 화이팅!",
    };
    public string[] changeBgm =
    {
        "이곳으로 다가가 E키를 누르면 노래를 바꿀 수 있습니다.",
    };
}
