using System.Collections;
using System.Collections.Generic;

public class Player
{
    private int playerIdx;
    private List<Card> handCard;
    private Dictionary<CardType, int> typeCount;
    // TODO:Dictionary<CardType, List<Card>> 형태에서 변경함, 영향을 미치는 코드 점검 필요.

    public int score { get; private set; }
    public int goCnt { get; private set; }
    public int fuckCnt { get; private set; }
    public int shakeCnt { get; private set; }
    public int bombCnt { get; private set; }


    public Player(int idx)
    {
        playerIdx = idx;
        score = 0;
        bombCnt = 0;
        handCard = new List<Card>();
    }

    public void Reset()
    {
        score = 0;
        goCnt = 0;
        fuckCnt = 0;
        shakeCnt = 0;
        bombCnt = 0;
        handCard.Clear();
    }

    public void AddCard(Card card)
    {
        handCard.Add(card);
    }

    public void AddCardFloor(Card card)
    {
        typeCount[card.CType]++;
    }
    
    public void AddScore(int points)
    {
        score += points;
    }

    public Card PopCard(int num, int pos, CardType type)
    {
        Card card = handCard.Find(o => { return o.number == num && o.CType == type && o.position == pos; });

        if (card == null)
            return null;

        handCard.Remove(card);
        return card;
    }

    public List<Card> PopAllCard(int num)
    {
        List<Card> cards = handCard.FindAll(o => o.number == num);
        
        for (int i = 0; i < cards.Count; i++)
            handCard.Remove(cards[i]);
        
        return cards;
    }

    private bool FindCards(CardType type)
    {
        if (typeCount.ContainsKey(type))
            return true;

        return false;
    }
    
    public void getfuck()
    {
        fuckCnt++;
    }
}