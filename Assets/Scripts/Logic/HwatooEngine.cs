using System.Collections;
using System.Collections.Generic;
using System;

public class HwatooEngine
{
    private CardManager cManager;
    private Queue<Card> cardQ;

    public FloorManager FManager { get; private set; }
    public Player[] players { get; private set; }

    public HwatooEngine()
    {
        cManager = new CardManager();
        FManager = new FloorManager();
        cardQ = new Queue<Card>();
        players = new Player[2];
        players[0] = new Player(0);   // user
        players[1] = new Player(1);   // enemy
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
        // 2번 반복
        for (int t = 0; t < 2; t++)
        {
            // 바닥 패 4장
            for (int i = 0; i < 4; i++)
            {
                FManager.AddStartCard(PopCard());
            }

            // 플레이어 5장.
            for (int i = 0; i < 5; ++i)
            {
                players[0].AddCard(PopCard());
            }

            // 상대 5장.
            for (int i = 0; i < 5; ++i)
            {
                players[1].AddCard(PopCard());
            }
        }
        
        FManager.RefreshFloor();
    }
    
    public void ProcessTurn(Card selectedHandCard, int playerIdx)
    {
        // 1. 플레이어가 선택한 카드를 바닥에 놓는다.
        CardSlot firstSlot = FManager.PutCard(selectedHandCard);
        
        Card drawnCard = PopCard();
        // 2. 뽑은 카드가 같은 번호면 뻑, 세 장 모두 바닥에 놓는다.
        if (firstSlot.IsSame(drawnCard.number))
        {
            firstSlot = FManager.PutCard(drawnCard);
            
            if (firstSlot.cards.Count == 2)   // 이 경우엔 뻑이 아니라 쪽
            {
                players[playerIdx].AddCardFloor(firstSlot.cards[0]);
                players[playerIdx].AddCardFloor(firstSlot.cards[1]);
                // TODO: 상대방의 피를 한 장 가져오는 로직 구현 필요
                firstSlot.Reset();
                return;
            }
            
            players[playerIdx].getfuck();
        }
        else
        {
            CardSlot secondSlot = FManager.PutCard(drawnCard);
            if (secondSlot.cards.Count >= 2)
            {
                for(int i = 0; i < secondSlot.cards.Count; i++)
                    players[playerIdx].AddCardFloor(secondSlot.cards[i]);
                secondSlot.Reset();
            }
        }
    }
}