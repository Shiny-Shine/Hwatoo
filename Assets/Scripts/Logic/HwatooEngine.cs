using System.Collections;
using System.Collections.Generic;
using System;

public class HwatooEngine
{
    private CardManager CManager;
    private Queue<Card> cardQ;

    public FloorManager FManager { get; private set; }

    // 카드 정보 저장
    public List<Card> floorCards { get; private set; }
    public List<Card> playerCards { get; private set; }
    public List<Card> enemyCards { get; private set; }

    public HwatooEngine()
    {
        CManager = new CardManager();
        FManager = new FloorManager();
        cardQ = new Queue<Card>();
    }

    public void clearData()
    {
        
    }

    public void start()
    {
        shuffle();
        cardDistribution();
    }

    Card popCard()
    {
        return cardQ.Dequeue();
    }

    public void reset()
    {
        CManager.makeCards();
        FManager.reset();
    }

    void shuffle()
    {
        cardQ.Clear();
        CManager.shuffle();
        cardQ = CManager.exportToQ();
    }

    void cardDistribution()
    {
        int flooridx = 0;

        // 2번 반복
        for (int t = 0; t < 2; t++)
        {
            // 바닥 패 4장
            for (int i = 0; i < 4; i++)
            {
                floorCards.Add(popCard());
                flooridx++;
            }

            // 플레이어 5장.
            for (int i = 0; i < 5; ++i)
            {
                playerCards.Add(popCard());
            }

            // 상대 5장.
            for (int i = 0; i < 5; ++i)
            {
                enemyCards.Add(popCard());
            }
        }
        
        FManager.refreshFloor();
    }
    
    public void ProcessTurn(Card selectedHandCard, Card selectedFloorCard)
    {
        
        // TODO: 플레이어가 선택한 카드를 바닥에 놓기

        // TODO: 턴 종료 및 다음 플레이어로 전환
    }
}