using System.Collections.Generic;

public class MatchResult
{
    public bool IsMatched { get; }
    public Card PlayedCard { get; }
    public List<Card> MatchedCards { get; }
    public bool IsBomb { get; }

    public MatchResult(bool isMatched, Card playedCard, List<Card> matchedCards, bool isBomb = false)
    {
        IsMatched = isMatched;
        PlayedCard = playedCard;
        MatchedCards = matchedCards ?? new List<Card>();
        IsBomb = isBomb;
    }
}