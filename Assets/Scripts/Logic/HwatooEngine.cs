using System.Collections;
using System.Collections.Generic;
using System;

public class HwatooEngine
{
    private CardManager cManager;
    private Queue<Card> cardQ;

    public FloorManager FManager { get; private set; }

    // 카드 정보 저장
    public List<Card> floorCards { get; private set; }
    public List<Card> PlayerHandCards { get; private set; }
    public List<Card> EnemyHandCards { get; private set; }

    public HwatooEngine()
    {
        cManager = new CardManager();
        FManager = new FloorManager();
        cardQ = new Queue<Card>();
    }

    public void ClearData()
    {
        
    }

    public void Start()
    {
        Shuffle();
        CardDistribution();
    }

    Card PopCard()
    {
        return cardQ.Dequeue();
    }

    public void Reset()
    {
        cManager.MakeCards();
        FManager.Reset();
    }

    void Shuffle()
    {
        cardQ.Clear();
        cManager.Shuffle();
        cardQ = cManager.ExportToQ();
    }

    void CardDistribution()
    {
        int flooridx = 0;

        // 2번 반복
        for (int t = 0; t < 2; t++)
        {
            // 바닥 패 4장
            for (int i = 0; i < 4; i++)
            {
                floorCards.Add(PopCard());
                flooridx++;
            }

            // 플레이어 5장.
            for (int i = 0; i < 5; ++i)
            {
                PlayerHandCards.Add(PopCard());
            }

            // 상대 5장.
            for (int i = 0; i < 5; ++i)
            {
                EnemyHandCards.Add(PopCard());
            }
        }
        
        FManager.RefreshFloor();
    }
    
    public void ProcessTurn(Card selectedHandCard)
    {
        CardSlot curSlot;
        // 1. 플레이어가 선택한 카드를 바닥에 놓는다.
        curSlot = FManager.AddCard(selectedHandCard);
        
        Card drawnCard = PopCard();
        // 2. 뽑은 카드가 같은 번호면 뻑, 세 장 모두 바닥에 놓는다.
        if (curSlot.IsSame(drawnCard.number))
        {
            FManager.AddCard(drawnCard);
            
            if (curSlot.cards.Count == 2)   // 이 경우엔 뻑이 아니라 쪽
            {
                
            }
        }
        // TODO:현재 플레이어가 선택한 카드를 바닥에 놓은 후 뻑 계산 후 피로 가져오는 과정 구현 중.
        
    }
}