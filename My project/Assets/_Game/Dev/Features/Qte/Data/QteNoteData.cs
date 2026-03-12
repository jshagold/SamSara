using System;
using UnityEngine;

[Serializable]
public class QteNoteData
{
    [Tooltip("화면 좌표 (0,0 ~ 1,1 비율) - (0.5, 0.5)가 중앙")]
    public Vector2 positionRatio = new Vector2(0.5f, 0.5f);

    [Tooltip("이 노트가 시작된 후, 다음 노트가 나올 때까지의 대기 시간")]
    public float nextDelay;

    [Tooltip("노트 판정 지속 시간")]
    public float inputDuration = 1.0f;

    public Sprite noteVisual;
}