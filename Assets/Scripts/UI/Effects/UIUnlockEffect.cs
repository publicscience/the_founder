using UnityEngine;

public class UIUnlockEffect : MonoBehaviour {
    public UILabel label;

    public void Set(string name) {
        label.text = "Unlocks " + name;
    }

    public void SetSpecial(EffectSet.Special effect) {
        string text = "";
        switch (effect) {
            case EffectSet.Special.Immortal:
                text = "Grants you immortality";
                break;
            case EffectSet.Special.Cloneable:
                text = "Allows cloning of your workers";
                break;
            case EffectSet.Special.Prescient:
                text = "65% chance of correctly predicting economic fluctations";
                break;
            case EffectSet.Special.WorkerInsight:
                text = "Provides detailed metrics of potential hires";
                break;
        }
        label.text = text;
    }
}


