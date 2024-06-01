using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager
{
    private List<Card> cards;

    public CardManager()
    {
        this.cards = new List<Card>();
    }

    public Queue<Card> exportToQ()
    {
        Queue<Card> tempq = new Queue<Card>();
        for (int i = 0; i < cards.Count; i++)
        {
            tempq.Enqueue(cards[i]);
        }

        return tempq;
    }

    void swap(int x, int y)
    {
        Card temp = cards[x];
        cards[x] = cards[y];
        cards[y] = temp;
    }

    public void shuffle()
    {
        for (int i = 0; i < 200; i++)
        {
            int first = Random.Range(1, cards.Count);
            int second = Random.Range(1, cards.Count);
            swap(first, second);
        }
    }

    public void makeCards()
    {
        this.cards.Clear();
        // 1
        this.cards.Add(new Card(1, 1, CardType.K));
        this.cards.Add(new Card(1, 2, CardType.T, CardStat.HONGDAN));
        this.cards.Add(new Card(1, 3, CardType.P));
        this.cards.Add(new Card(1, 4, CardType.P));
        // 2
        this.cards.Add(new Card(2, 1, CardType.Y, CardStat.GODORI));
        this.cards.Add(new Card(2, 2, CardType.T, CardStat.HONGDAN));
        this.cards.Add(new Card(2, 3, CardType.P));
        this.cards.Add(new Card(2, 4, CardType.P));
        // 3
        this.cards.Add(new Card(3, 1, CardType.K));
        this.cards.Add(new Card(3, 2, CardType.T, CardStat.HONGDAN));
        this.cards.Add(new Card(3, 3, CardType.P));
        this.cards.Add(new Card(3, 4, CardType.P));
        // 4
        this.cards.Add(new Card(4, 1, CardType.Y, CardStat.GODORI));
        this.cards.Add(new Card(4, 2, CardType.T, CardStat.CHODAN));
        this.cards.Add(new Card(4, 3, CardType.P));
        this.cards.Add(new Card(4, 4, CardType.P));
        // 5
        this.cards.Add(new Card(5, 1, CardType.Y));
        this.cards.Add(new Card(5, 2, CardType.T, CardStat.CHODAN));
        this.cards.Add(new Card(5, 3, CardType.P));
        this.cards.Add(new Card(5, 4, CardType.P));
        // 6
        this.cards.Add(new Card(6, 1, CardType.Y));
        this.cards.Add(new Card(6, 2, CardType.T, CardStat.CHEONGDAN));
        this.cards.Add(new Card(6, 3, CardType.P));
        this.cards.Add(new Card(6, 4, CardType.P));
        // 7
        this.cards.Add(new Card(7, 1, CardType.Y));
        this.cards.Add(new Card(7, 2, CardType.T, CardStat.CHODAN));
        this.cards.Add(new Card(7, 3, CardType.P));
        this.cards.Add(new Card(7, 4, CardType.P));
        // 8
        this.cards.Add(new Card(8, 1, CardType.K));
        this.cards.Add(new Card(8, 2, CardType.Y, CardStat.GODORI));
        this.cards.Add(new Card(8, 3, CardType.P));
        this.cards.Add(new Card(8, 4, CardType.P));
        // 9
        this.cards.Add(new Card(9, 1, CardType.Y, CardStat.SP));
        this.cards.Add(new Card(9, 2, CardType.T, CardStat.CHEONGDAN));
        this.cards.Add(new Card(9, 3, CardType.P));
        this.cards.Add(new Card(9, 4, CardType.P));
        // 10
        this.cards.Add(new Card(10, 1, CardType.Y));
        this.cards.Add(new Card(10, 2, CardType.T, CardStat.CHEONGDAN));
        this.cards.Add(new Card(10, 3, CardType.P));
        this.cards.Add(new Card(10, 4, CardType.P));
        // 11
        this.cards.Add(new Card(11, 1, CardType.K));
        this.cards.Add(new Card(11, 2, CardType.P, CardStat.SP));
        this.cards.Add(new Card(11, 3, CardType.P));
        this.cards.Add(new Card(11, 4, CardType.P));
        // 12
        this.cards.Add(new Card(12, 1, CardType.K));
        this.cards.Add(new Card(12, 2, CardType.Y));
        this.cards.Add(new Card(12, 3, CardType.T));
        this.cards.Add(new Card(12, 4, CardType.P, CardStat.SP));
    }
}