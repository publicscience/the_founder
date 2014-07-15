using UnityEngine;
using System.Collections;

public abstract class ProductAspect : ScriptableObject {
    public string description;

    public override string ToString() {
        return name;
    }
}
