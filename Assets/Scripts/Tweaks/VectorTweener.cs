using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class VectorTweener {
    public Vector3 position;
    
    private Queue moveStack = new Queue();
    private float lastTime = 0;

    public VectorTweener(Vector3 position) {
        this.position = position;
    }

    public void ClearMoves() {
        moveStack.Clear();
    }

    public void AddMove(Vector3 move, float length) {
        float time = GetNewMoveTime(length);
        lastTime = time;
        moveStack.Enqueue(new object[] { move, length, time });
    }

    public void AddMoveXYZ(Vector3 move, float length, bool applyX, bool applyY, bool applyZ) {
        Vector3 appliedMove = GetLatestMove();
        if (applyX) 
            appliedMove.x = move.x;
        if (applyY) 
            appliedMove.y = move.y;
        if (applyZ) 
            appliedMove.z = move.z;
        AddMove(appliedMove, length);
    }

    private float GetNewMoveTime(float length) {
        return moveStack.Count == 0 ? Time.time : lastTime + length;
    }

    public Vector3 GetPositionAtTime(float time) {
        if (moveStack.Count == 0) {
            return position;
        } else {
            Vector3 move = GetLatestMove();
            float moveLength = (float)(((object[])moveStack.Peek())[1]);
            float moveTime = (float)(((object[])moveStack.Peek())[2]);
            float currentLength = time - moveTime;
            if (currentLength >= moveLength) {
                position = move;
                moveStack.Dequeue();
                return GetPositionAtTime(time);
            } else {
                float ratio = currentLength / moveLength;
                Vector3 delta = move - position;
                return ratio * delta + position;
            }
        }
    }

    public bool InProgress() {
        return moveStack.Count > 0;
    }

    private Vector3 GetLatestMove() {
        return moveStack.Count == 0 ? position : (Vector3)(((object[])moveStack.Peek())[0]);
    }
}
