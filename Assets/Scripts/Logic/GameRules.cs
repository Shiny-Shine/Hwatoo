using System;

public sealed class GameRules
{
    public void PrepareDeck(GameState state, CardManager cardManager)
    {
        cardManager.makeCards();
        cardManager.shuffle();
        state.SetDeck(cardManager.exportToQ());
    }

    public void DealInitial(GameState state, Func<Card> draw, Action<CardOwner, Card> onDealt)
    {
        for (int t = 0; t < 2; t++)
        {
            for (int i = 0; i < 4; i++)
                DealToFloor(state, draw(), onDealt);

            for (int i = 0; i < 5; i++)
                DealToPlayer(state, draw(), onDealt);

            for (int i = 0; i < 5; i++)
                DealToEnemy(state, draw(), onDealt);
        }

        state.FloorManager.refreshFloor();
    }

    private static void DealToFloor(GameState state, Card card, Action<CardOwner, Card> onDealt)
    {
        state.FloorCards.Add(card);
        state.FloorManager.addStartCard(card);
        onDealt?.Invoke(CardOwner.Floor, card);
    }

    private static void DealToPlayer(GameState state, Card card, Action<CardOwner, Card> onDealt)
    {
        state.PlayerHand.Add(card);
        onDealt?.Invoke(CardOwner.Player, card);
    }

    private static void DealToEnemy(GameState state, Card card, Action<CardOwner, Card> onDealt)
    {
        state.EnemyHand.Add(card);
        onDealt?.Invoke(CardOwner.Enemy, card);
    }
}