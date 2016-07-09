using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class FloatTweener {
    public float position;

    private Queue moveStack = new Queue();
    private float lastTime = 0;

    public FloatTweener(float position) {
        this.position = position;
    }

    public void ClearMoves() {
        moveStack.Clear();
    }

    public void AddMove(float move, float length) {
        float time = GetNewMoveTime(length);
        lastTime = time;
        moveStack.Enqueue(new object[] { move, length, time });
    }

    private float GetNewMoveTime(float length) {
        return moveStack.Count == 0 ? Time.time : lastTime + length;
    }

    public float GetPositionAtTime(float time) {
        if (moveStack.Count == 0) {
            return position;
        } else {
            float move = GetLatestMove();
            float moveLength = (float)(((object[])moveStack.Peek())[1]);
            float moveTime = (float)(((object[])moveStack.Peek())[2]);
            float currentLength = time - moveTime;
            if (currentLength >= moveLength) {
                position = move;
                moveStack.Dequeue();
                return GetPositionAtTime(time);
            } else {
                float ratio = currentLength / moveLength;
                float delta = move - position;
                return ratio * delta + position;
            }
        }
    }

    public bool InProgress() {
        return moveStack.Count == 0;
    }

    private float GetLatestMove() {
        return moveStack.Count == 0 ? position : (float)(((object[])moveStack.Peek())[0]);
    }
}
