using UnityEngine;
using System.Collections;

public class RecordData {
    public static AudioClip[] crackles;
    private ArrayList leftClips = new ArrayList();
	private ArrayList rightClips = new ArrayList();
	private ArrayList clipLengths = new ArrayList();

    private AudioClip GetCrackle() {
        return crackles[Random.Range(0, crackles.Length)];
    }

	public void UnloadSongs() {
		for (int i = 0; i < leftClips.Count; i++) {
			MonoBehaviour.Destroy(((AudioClip) leftClips[i]));
			MonoBehaviour.Destroy(((AudioClip) rightClips[i]));
		}
		leftClips.Clear();
		rightClips.Clear();
	}

    public void AddSong(AudioClip leftClip, AudioClip rightClip) {
		leftClips.Add(leftClip);
        rightClips.Add(rightClip);
		clipLengths.Add(leftClip.length);
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

    /* This returns the amount of seconds it should take to play the whole record, regardless of content. (ie. "empty space" is not ignored) */
    public float GetLength() {
        /* This would be a constant value if this was a realistic representation of vinyls, obviously.
         * But, in the interest of usability and my lazyness, this is not the case. 
         * Result: this method is realistic only when albums are less than 20 minutes long. */
        return 20 * 60 < GetLoadedLength() ? GetLoadedLength() : 20 * 60;
    }

    public float GetLengthOfClip(int index) {
		return (float) clipLengths[index];
    }

    public float GetLengthUntil(int index) {
        float total = 0;
        for (int i = 0; i < index; i++) {
            total += GetLengthOfClip(i);
        }
        return total;
    }

    public float GetLoadedLength() {
		return GetLengthUntil(clipLengths.Count);
    }
}
