using System;
using System.Collections;
using System.Collections.Generic;
using Michsky.MUIP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HwatooGamePresenter : MonoBehaviour
{
    [Serializable]
    public struct CardSpriteBinding
    {
        public int month;      // 1~12
        public int position;   // 1~4
        public Sprite sprite;
    }

    [Header("Card")]
    [SerializeField] private CardView cardViewPrefab;
    [SerializeField] private Sprite backSprite;
    [SerializeField] private CardSpriteBinding[] faceBindings;

    [Header("Roots")]
    [SerializeField] private Transform myHandRoot;
    [SerializeField] private Transform enemyHandRoot;
    [SerializeField] private Transform[] floorMonthRoots = new Transform[12];
    [SerializeField] private Transform[] myFieldRoots = new Transform[4];
    [SerializeField] private Transform[] enemyFieldRoots = new Transform[4];

    [Header("UI")]
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text myScoreText;
    [SerializeField] private TMP_Text enemyScoreText;
    [SerializeField] private ButtonManager startRoundButton;

    [SerializeField] private GameObject nineYeolPanel;
    [SerializeField] private ButtonManager nineYeolAsYeolButton;
    [SerializeField] private ButtonManager nineYeolAsSsangPiButton;

    [SerializeField] private GameObject goStopPanel;
    [SerializeField] private ButtonManager goButton;
    [SerializeField] private ButtonManager stopButton;

    [Header("Layout")]
    [SerializeField] private float handSpacing = 1.2f;
    [SerializeField] private float floorStackOffsetY = 0.18f;
    [SerializeField] private float aiDelay = 0.35f;

    private readonly MatchFlowController flow = new MatchFlowController();
    private readonly List<CardView> spawnedViews = new List<CardView>();
    private readonly Dictionary<int, Sprite> faceSpriteMap = new Dictionary<int, Sprite>();
    private Coroutine enemyRoutine;

    private void Awake()
    {
        BuildSpriteMap();

        startRoundButton.onClick.AddListener(OnClickStartRound);
        nineYeolAsYeolButton.onClick.AddListener(() => ApplyResult(flow.ChooseNineYeol(false)));
        nineYeolAsSsangPiButton.onClick.AddListener(() => ApplyResult(flow.ChooseNineYeol(true)));
        goButton.onClick.AddListener(() => ApplyResult(flow.ChooseGoStop(true)));
        stopButton.onClick.AddListener(() => ApplyResult(flow.ChooseGoStop(false)));
    }

    private void Start()
    {
        OnClickStartRound();
    }

    private void BuildSpriteMap()
    {
        faceSpriteMap.Clear();

        for (int i = 0; i < faceBindings.Length; i++)
        {
            CardSpriteBinding b = faceBindings[i];
            if (b.month < 1 || b.month > 12) continue;
            if (b.position < 1 || b.position > 4) continue;
            if (b.sprite == null) continue;

            faceSpriteMap[MakeKey(b.month, b.position)] = b.sprite;
        }
    }

    private static int MakeKey(int month, int position)
    {
        return month * 10 + position;
    }

    private Sprite ResolveFront(Card card)
    {
        if (card == null) return backSprite;

        int key = MakeKey(card.number, card.position);
        if (faceSpriteMap.TryGetValue(key, out Sprite s))
            return s;

        return backSprite;
    }

    private void OnClickStartRound()
    {
        bool ok = flow.StartRound();
        statusText.text = ok ? "라운드를 시작했습니다." : "라운드 시작 실패";
        RenderAll();
        TryRunEnemyTurn();
    }

    private void OnClickMyCard(Card selected)
    {
        if (flow.State != TurnFlowState.WaitingPlayCard) return;
        if (flow.CurrentPlayerIdx != 0) return;

        ApplyResult(flow.PlayCard(selected));
    }

    private void ApplyResult(TurnResult result)
    {
        statusText.text = result.EndReason;
        RenderAll();

        if (result.RoundEnded)
        {
            StopEnemyRoutine();
            return;
        }

        TryRunEnemyTurn();
    }

    // A?I
    private void TryRunEnemyTurn()
    {
        if (flow.State != TurnFlowState.WaitingPlayCard) return;
        if (flow.CurrentPlayerIdx != 1) return;
        if (enemyRoutine != null) return;

        enemyRoutine = StartCoroutine(RunEnemyTurn());
    }

    private void StopEnemyRoutine()
    {
        if (enemyRoutine == null) return;
        StopCoroutine(enemyRoutine);
        enemyRoutine = null;
    }

    // 단순히 패의 첫 번째 카드를 내도록 구현, 열/고스톱 선택도 점수 기준으로 간단히 판단.
    private IEnumerator RunEnemyTurn()
    {
        while (flow.State == TurnFlowState.WaitingPlayCard && flow.CurrentPlayerIdx == 1)
        {
            yield return new WaitForSeconds(aiDelay);

            List<Card> enemyHand = flow.GetHandSnapshot(1);
            if (enemyHand.Count == 0) break;

            ApplyResult(flow.PlayCard(enemyHand[0])); // A?I, 무조건 첫 번째 카드 내기

            while (flow.State == TurnFlowState.WaitingNineYeolChoice)
            {
                yield return new WaitForSeconds(aiDelay);
                ApplyResult(flow.ChooseNineYeol(false)); // 열 유지
            }

            while (flow.State == TurnFlowState.WaitingGoStop)
            {
                yield return new WaitForSeconds(aiDelay);
                bool go = flow.GetScore(1) >= 9;
                ApplyResult(flow.ChooseGoStop(go));
            }
        }

        enemyRoutine = null;
    }

    private void RenderAll()
    {
        ClearCards();
        RenderHands();
        RenderFloor();
        RenderHud();
    }

    // 플레이어 손패와 상대 손패를 화면에 렌더링.
    private void RenderHands()
    {
        List<Card> myHand = flow.GetHandSnapshot(0);
        List<Card> enemyHand = flow.GetHandSnapshot(1);

        bool canClickMyHand = flow.State == TurnFlowState.WaitingPlayCard && flow.CurrentPlayerIdx == 0;

        for (int i = 0; i < myHand.Count; i++)
        {
            CardView v = Spawn(myHandRoot, myHand[i], true, canClickMyHand, OnClickMyCard);
            float x = (i - (myHand.Count - 1) * 0.5f) * handSpacing;
            v.transform.localPosition = new Vector3(x, 0f, -i * 0.01f);
        }

        for (int i = 0; i < enemyHand.Count; i++)
        {
            CardView v = Spawn(enemyHandRoot, enemyHand[i], false, false, null);
            float x = (i - (enemyHand.Count - 1) * 0.5f) * handSpacing;
            v.transform.localPosition = new Vector3(x, 0f, -i * 0.01f);
        }
    }

    // 바닥에 놓인 카드들을 화면에 렌더링. 같은 번호의 카드들은 하나의 슬롯에 쌓아서 표현.
    private void RenderFloor()
    {
        List<Card>[] slots = flow.GetFloorSlotsSnapshot();
        List<Card>[] myField = flow.GetFieldSnapshot(0);
        List<Card>[] enemyField = flow.GetFieldSnapshot(1);
        int monthCount = Mathf.Min(12, floorMonthRoots.Length);

        for (int month = 1; month <= monthCount; month++)
        {
            Transform root = floorMonthRoots[month - 1];
            if (root == null) continue;

            List<Card> stack = slots[month - 1];
            for (int i = 0; i < stack.Count; i++)
            {
                CardView v = Spawn(root, stack[i], true, false, null);
                float x = (i - (stack.Count - 1) * 0.5f) * handSpacing;
                v.transform.localPosition = new Vector3(x, i * floorStackOffsetY, -i * 0.01f);
            }
        }
        
        for(int i = 0; i < myField.Length; i++)
        {
            Transform root = myFieldRoots[i];
            if (root == null) continue;

            List<Card> stack = myField[i];
            for (int j = 0; j < stack.Count; j++)
            {
                CardView v = Spawn(root, stack[j], true, false, null);
                float x = (j - (stack.Count - 1) * 1.5f) * handSpacing;
                v.transform.localPosition = new Vector3(x, j * floorStackOffsetY, -j * 0.01f);
            }
        }
        
        for(int i = 0; i < enemyField.Length; i++)
        {
            Transform root = enemyFieldRoots[i];
            if (root == null) continue;

            List<Card> stack = enemyField[i];
            for (int j = 0; j < stack.Count; j++)
            {
                CardView v = Spawn(root, stack[j], true, false, null);
                float x = (j - (stack.Count - 1) * 1.5f) * handSpacing;
                v.transform.localPosition = new Vector3(x, j * floorStackOffsetY, -j * 0.01f);
            }
         }
    }

    private void RenderHud()
    {
        myScoreText.text = $"내 점수: {flow.GetScore(0)}";
        enemyScoreText.text = $"상대 점수: {flow.GetScore(1)}";

        bool myTurn = flow.CurrentPlayerIdx == 0;
        nineYeolPanel.SetActive(myTurn && flow.State == TurnFlowState.WaitingNineYeolChoice);
        goStopPanel.SetActive(myTurn && flow.State == TurnFlowState.WaitingGoStop);
    }

    // 카드 생성
    private CardView Spawn(Transform parent, Card card, bool faceUp, bool clickable, Action<Card> onClick)
    {
        CardView v = Instantiate(cardViewPrefab, parent);
        v.Bind(card, ResolveFront(card), backSprite, faceUp, clickable, onClick);
        spawnedViews.Add(v);
        return v;
    }

    private void ClearCards()
    {
        for (int i = 0; i < spawnedViews.Count; i++)
        {
            if (spawnedViews[i] != null) Destroy(spawnedViews[i].gameObject);
        }
        spawnedViews.Clear();
    }
}