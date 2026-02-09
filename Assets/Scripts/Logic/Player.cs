using System.Collections;
using System.Collections.Generic;

public class Player
{
    private int playerIdx;
    private List<Card> handCard;
    private Dictionary<CardType, List<Card>> floorCards;

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

    public void reset()
    {
        score = 0;
        goCnt = 0;
        fuckCnt = 0;
        shakeCnt = 0;
        bombCnt = 0;
        handCard.Clear();
    }

    public void addCard(Card card)
    {
        handCard.Add(card);
    }

    public void addCardFloor(Card card)
    {
        if(!floorCards.ContainsKey(card.CType))
            floorCards.Add(card.CType, new List<Card>());
        floorCards[card.CType].Add(card);
    }
    
    public void AddScore(int points)
    {
        score += points;
    }

    public Card popCard(int num, int pos, CardType type)
    {
        Card card = handCard.Find(o => { return o.number == num && o.CType == type && o.position == pos; });

        if (card == null)
            return null;

        handCard.Remove(card);
        return card;
    }

    public List<Card> popAllCard(int num)
    {
        List<Card> cards = handCard.FindAll(o => o.number == num);
        
        for (int i = 0; i < cards.Count; i++)
            handCard.Remove(cards[i]);
        
        return cards;
    }

    private List<Card> findCards(CardType type)
    {
        if (floorCards.ContainsKey(type))
            return floorCards[type];

        return null;
    }
}