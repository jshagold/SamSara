# SamSara
삼사라몬 게임플젝



### 데이터 및 로직 흐름도 (Flow Summary)

sequenceDiagram
    participant User
    participant View
    participant Presenter
    participant UseCase
    participant Repository
    participant Entity

    Note over User, View: [Input Flow]
    User->>View: 1. 훈련 버튼 클릭
    View->>Presenter: 2. OnClick 이벤트 전달
    Presenter->>UseCase: 3. ExecuteTraining() 호출 (Fire & Forget or Await)
    
    Note over UseCase, Entity: [Logic Flow]
    UseCase->>Repository: 4. 데이터 로드 (UniTask)
    Repository-->>UseCase: Entity 반환
    UseCase->>Entity: 5. exp += 10 (순수 로직)
    UseCase->>Repository: 6. 데이터 저장 (UniTask)
    
    Note over Entity, View: [Output Flow (Observer)]
    Entity->>Presenter: 7. OnExpChanged 이벤트 발생 (Observer)
    Presenter->>View: 8. UpdateUI(newExp) 호출
    View-->>User: 9. 화면 갱신 (텍스트 변경)
