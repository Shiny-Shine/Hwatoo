using System.Collections.Generic;

public class GameEngine
{
    public List<int> Deck = new List<int>(); // 카드 ID 만 관리

    // 델리게이트: 화면 쪽에 "뭔가 변했다"고 알려주는 신호
    public System.Action<int> OnCardDistributed;

    public void StartGame()
    {
        // 1. 덱 초기화 및 셔플 로직
        for (int i = 0; i < 48; i++) Deck.Add(i);

        // 2. 카드 나눠주기 (데이터만 이동)
        int cardId = Deck[0];
        Deck.RemoveAt(0);

        // 3. 화면 쪽에 알림
        if (OnCardDistributed != null)
            OnCardDistributed(cardId);
    }
}
