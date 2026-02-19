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

    public void Start()
    {
        Reset();
        Shuffle();
        CardDistribution();
    }

    Card PopMainDeck()
    {
        return cardQ.Dequeue();
    }

    private void Reset()
    {
        players[0].Reset();
        players[1].Reset();
        cManager.MakeCards();
        FManager.Reset();
    }

    void Shuffle()
    {
        cardQ.Clear();
        cManager.Shuffle();
        cardQ = cManager.ExportToQ();
    }
    
    // 상대방의 피를 가져옴
    void TakeJunk(int cnt, int playerIdx)
    {
        players[playerIdx].AddCardFloor(CardType.P, 1);
        players[(playerIdx + 1) % 2].PopPlayerFloor(CardType.P, 1);
    }

    void CardDistribution()
    {
        // 2번 반복
        for (int t = 0; t < 2; t++)
        {
            // 바닥 패 4장
            for (int i = 0; i < 4; i++)
            {
                FManager.AddStartCard(PopMainDeck());
            }

            // 플레이어 5장.
            for (int i = 0; i < 5; ++i)
            {
                players[0].AddPlayerHand(PopMainDeck());
            }

            // 상대 5장.
            for (int i = 0; i < 5; ++i)
            {
                players[1].AddPlayerHand(PopMainDeck());
            }
        }
        
        FManager.RefreshFloor();
    }
    
    public void ProcessTurn(Card selectedHandCard, int playerIdx)
    {
        // 1. 플레이어가 선택한 카드를 바닥에 놓는다.
        CardSlot firstSlot = FManager.PutCard(selectedHandCard);
        
        Card drawnCard = PopMainDeck();
        // 2. 뽑은 카드가 같은 번호면 뻑, 세 장 모두 바닥에 놓는다.
        if (firstSlot.IsSame(drawnCard.number))
        {
            firstSlot = FManager.PutCard(drawnCard);
            
            if (firstSlot.cards.Count == 2)   // 이 경우엔 뻑이 아니라 쪽
            {
                players[playerIdx].AddCardFloor(firstSlot.PopTopCard().CType, 1);
                players[playerIdx].AddCardFloor(firstSlot.PopTopCard().CType, 1);
                TakeJunk(1, playerIdx);
                firstSlot.Reset();
                return;
            }
            else if (firstSlot.cards.Count == 4)
            {
                // 	따닥 : 바닥에 같은 무늬 패 두 장이 있고 그걸 먹기 위해 패를 낸 다음 뒤집은 패마저 같은 무늬일 경우 네 장 모두 먹음과 동시에 상대방의 피 한 장을 가져옵니다.
                for(int i = 0; i < firstSlot.cards.Count; i++)
                    players[playerIdx].AddCardFloor(firstSlot.PopTopCard().CType, 1);
                TakeJunk(1, playerIdx);
                firstSlot.Reset();
                return;
            }
            
            players[playerIdx].getfuck();
        }
        else // 뽑은 카드가 같은 번호가 아님.
        {
            CardSlot secondSlot = FManager.PutCard(drawnCard);
            if (secondSlot.cards.Count >= 2)
            {
                for(int i = 0; i < secondSlot.cards.Count; i++)
                    players[playerIdx].AddCardFloor(secondSlot.PopTopCard().CType, 1);
                secondSlot.Reset();
            }
        }
        
        // 판쓰리
        if(FManager.IsEmpty())
        {
            TakeJunk(1, playerIdx);
        }
    }
}