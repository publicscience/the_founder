using UnityEngine;
using System.Collections;

public class Piece : MonoBehaviour {
    public int x;
    public int y;
    public bool stacked;
    public GameObject obj;

    public enum Type {
        ProductType,
        Product,
        Influencer,
        Bug,
        Outrage,
        Happy
    }
    public Type type;

    public void Setup(Type t, int x_, int y_) {
        x = x_;
        y = y_;
        type = t;
        stacked = false;
    }

    public bool Equals(Piece p) {
        // TODO this needs to also compare that the producttype or product are equivalent
        if (p != null && type == p.type) {
        }
        return false;
    }

    void Update() {
        if (obj != null)
            UIAnimator.Rotate(obj);
    }

    public void SwapWith(Piece p) {
        int tmp = p.x;
        p.x = x;
        x = tmp;

        tmp = p.y;
        p.y = y;
        y = tmp;
    }
}
