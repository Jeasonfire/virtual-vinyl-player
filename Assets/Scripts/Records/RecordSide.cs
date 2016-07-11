using UnityEngine;
using System.Collections;

public class RecordSide {
    private AudioClip[] crackles;
    private ArrayList leftClips = new ArrayList();
    private ArrayList rightClips = new ArrayList();

    public RecordSide(AudioClip mainCrackle, AudioClip[] crackles) {
        this.crackles = crackles;
        AddSong(mainCrackle, mainCrackle);
    }

    private AudioClip GetCrackle() {
        return crackles[Random.Range(0, crackles.Length)];
    }

    public void AddSong(AudioClip leftClip, AudioClip rightClip) {
        leftClips.Add(leftClip);
        rightClips.Add(rightClip);
    }

    /* Clip getters */

    public AudioClip GetClipLeft(int index) {
        return (AudioClip)(index < leftClips.Count ? leftClips[index] : GetCrackle());
    }

    public AudioClip GetClipRight(int index) {
        return (AudioClip)(index < rightClips.Count ?  rightClips[index] : GetCrackle());
    }

    public int GetClipIndex(float position) {
        float lengthSoFar = 0;
        for (int i = 0; i < leftClips.Count; i++) {
            lengthSoFar += GetClipLeft(i).length;
            if (position <= lengthSoFar) {
                return i;
            }
        }
        return -1;
    }

    /* Length getters */

    public float GetLength() {
        // (20 min) * (60 s/min)
        return 20 * 60;
    }

    public float GetLengthOfClip(int index) {
        return GetClipLeft(index).length;
    }

    public float GetLengthUntil(int index) {
        float total = 0;
        for (int i = 0; i < index; i++) {
            total += GetLengthOfClip(i);
        }
        return total;
    }

    public float GetLoadedLength() {
        return GetLengthUntil(leftClips.Count);
    }
}
