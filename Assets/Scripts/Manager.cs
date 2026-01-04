using UnityEngine;

public class Manager : MonoBehaviour
{
    private GameEngine _engine; // 로직 객체 소유
    public GameObject cardPrefab; // 프리팹 연결

    void Start()
    {
        _engine = new GameEngine();

        // 로직에서 신호가 오면 실행할 함수 연결
        _engine.OnCardDistributed += HandleCardDistributed;

        // 게임 시작 (로직에게 명령)
        _engine.StartGame();
    }

    // 화면에 그리는 함수
    void HandleCardDistributed(int cardId)
    {
        GameObject newCard = Instantiate(cardPrefab);
        // cardId에 맞는 이미지를 찾아서 스프라이트 교체
        // newCard.GetComponent<SpriteRenderer>().sprite = ...
        Debug.Log($"카드 {cardId}가 화면에 생성되었습니다.");
    }
}