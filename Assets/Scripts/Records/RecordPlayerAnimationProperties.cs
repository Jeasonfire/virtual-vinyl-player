using UnityEngine;
using System;

[Serializable]
public class RecordPlayerAnimationProperties {
    [Range(0.01f, 2f)]
    public float interactionTransitionLength = 1;
    [Range(0.01f, 2f)]
    public float handVerticalTransitionLength = 1;
}