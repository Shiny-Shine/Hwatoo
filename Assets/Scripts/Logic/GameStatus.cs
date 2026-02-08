using System.Collections.Generic;

public sealed class GameState
{
    public Queue<Card> Deck { get; } = new Queue<Card>();
    public List<Card> FloorCards { get; } = new List<Card>();
    public List<Card> PlayerHand { get; } = new List<Card>();
    public List<Card> EnemyHand { get; } = new List<Card>();

    public FloorManager FloorManager { get; }

    public GameState(FloorManager floorManager)
    {
        FloorManager = floorManager;
    }

    public void Reset()
    {
        Deck.Clear();
        FloorCards.Clear();
        PlayerHand.Clear();
        EnemyHand.Clear();
        FloorManager.reset();
    }

    public void SetDeck(Queue<Card> deck)
    {
        Deck.Clear();
        while (deck.Count > 0)
            Deck.Enqueue(deck.Dequeue());
    }
}