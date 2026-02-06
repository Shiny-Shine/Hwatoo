using System.Collections.Generic;
using static UnityEngine.InputSystem.InputControlScheme;

public class MatchingSystem
{
    public MatchResult TryMatch(Card playedCard, FloorManager floorManager)
    {
        var matchingCards = floorManager.GetMatchingCards(playedCard.number);

        if (matchingCards.Count == 0)
        {
            // 매칭되는 카드가 없음 - 바닥에 놓기
            return new MatchResult(false, playedCard, null);
        }
        else if (matchingCards.Count == 1)
        {
            // 1장 매칭 - 둘 다 가져가기
            return new MatchResult(true, playedCard, matchingCards);
        }
        else if (matchingCards.Count == 2)
        {
            // 2장 매칭 - 플레이어가 선택해야 함 (또는 모두 가져가기)
            return new MatchResult(true, playedCard, matchingCards);
        }
        else if (matchingCards.Count == 3)
        {
            // 3장 매칭 - 폭탄! 모든 카드 가져가기
            return new MatchResult(true, playedCard, matchingCards, true);
        }

        return new MatchResult(false, playedCard, null);
    }
}
