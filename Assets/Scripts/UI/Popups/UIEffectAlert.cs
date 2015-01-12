using UnityEngine;
using System.Linq;
using System.Collections;

// A text alert popup which supports rendering effects.
public class UIEffectAlert : UIAlert {
    public UIGrid effectGrid;

    public GameObject buffEffectPrefab;
    public GameObject unlockEffectPrefab;
    public GameObject productEffectPrefab;

    public void RenderEffects(EffectSet es) {
        // Clear out existing effect elements.
        while (effectGrid.transform.childCount > 0) {
            GameObject go = effectGrid.transform.GetChild(0).gameObject;
            NGUITools.DestroyImmediate(go);
        }

        RenderUnlockEffects(es);
        RenderBuffEffects(es);
        RenderProductEffects(es);
    }

    public void AdjustEffectsHeight() {
        // -1 because by default there is space for about 1 effect.
        Extend((int)((effectGrid.GetChildList().Count - 1) * effectGrid.cellHeight));
    }

    private void RenderUnlockEffects(EffectSet es) {
        // Render the unlock effects for this event.
        // Note that event unlocks are *not* rendered because
        // those are "hidden" effects. You don't know they can happen until they do happen.
        foreach (ProductType i in es.unlocks.productTypes) {
            RenderUnlockEffect(i.name + " products");
        }
        foreach (Vertical i in es.unlocks.verticals) {
            RenderUnlockEffect("the " + i.name + " vertical");
        }
        foreach (Worker i in es.unlocks.workers) {
            RenderUnlockEffect(i.name);
        }
        foreach (Item i in es.unlocks.items) {
            RenderUnlockEffect(i.name);
        }
    }

    private void RenderBuffEffects(EffectSet es) {
        foreach (StatBuff buff in es.ofType<WorkerEffect>().Select(w => w.buff)) {
            RenderBuffEffect(buff, "workers");
        }
        // TO DO
        //foreach (StatBuff buff in es.ofType<CashEffect>().Select(c => c.cash)) {
            //RenderBuffEffect(buff, "the company");
        //}
    }

    private void RenderProductEffects(EffectSet es) {
        foreach (ProductEffect pe in es.ofType<ProductEffect>()) {
            GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, productEffectPrefab);
            effectObj.GetComponent<UIProductEffect>().Set(pe);
        }
    }

    private void RenderUnlockEffect(string name) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, unlockEffectPrefab);
        effectObj.GetComponent<UIUnlockEffect>().Set(name);
    }

    private void RenderBuffEffect(StatBuff buff, string target) {
        GameObject effectObj = NGUITools.AddChild(effectGrid.gameObject, buffEffectPrefab);
        effectObj.GetComponent<UIBuffEffect>().Set(buff, target);
    }

    public void Extend(int amount) {
        amount = (amount/2) + 8;
        int currentBottom = body.bottomAnchor.absolute;
        int currentTop = body.topAnchor.absolute;
        body.bottomAnchor.Set(window.transform, 0, currentBottom-amount);
        body.topAnchor.Set(window.transform, 0, currentTop+amount);
    }
}
