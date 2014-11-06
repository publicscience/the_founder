/*
 * This is a set of points which are distributed amongst
 * a FeatureSet.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class FeaturePoints {
    public int charisma = 0;
    public int cleverness = 0;
    public int creativity = 0;

    public FeaturePoints(int ch, int cl, int cr) {
        charisma = ch;
        cleverness = cl;
        creativity = cr;
    }

    public static FeaturePoints operator +(FeaturePoints left, FeaturePoints right) {
        return new FeaturePoints(
                    left.charisma + right.charisma,
                    left.cleverness + right.cleverness,
                    left.creativity + right.creativity);
    }

    public static FeaturePoints operator -(FeaturePoints left, FeaturePoints right) {
        return new FeaturePoints(
                    left.charisma - right.charisma,
                    left.cleverness - right.cleverness,
                    left.creativity - right.creativity);
    }
}
