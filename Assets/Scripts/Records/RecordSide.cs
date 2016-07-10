using UnityEngine;
using System.Collections;

public class RecordSide {
    private AudioClip[] crackles;
    private ArrayList leftClips = new ArrayList();
    private ArrayList rightClips = new ArrayList();

    public RecordSide(AudioClip mainCrackle, AudioClip[] crackles) {
        this.crackles = crackles;
        AddSong(mainCrackle, true);
        AddSong(mainCrackle, false);
    }

    private AudioClip GetCrackle() {
        return crackles[Random.Range(0, crackles.Length)];
    }

    public void AddSong(AudioClip clip, bool panRight) {
        if (panRight) {
            rightClips.Add(clip);
        } else {
            leftClips.Add(clip);
        }
    }

    public AudioClip GetClip(int index, bool panRight = false) {
        return (AudioClip)(index < leftClips.Count ? (panRight ? rightClips[index] : leftClips[index]) : GetCrackle());
    }

    public float GetLength() {
        // (20 min) * (60 s/min)
        return 20 * 60;
    }

    public float GetLengthOfClip(int index) {
        return GetClip(index).length;
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
