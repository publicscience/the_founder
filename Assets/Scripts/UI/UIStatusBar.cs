/*
 * Status Bar
 * ================
 *
 * Displays important information
 * about the game.
 *
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIStatusBar : MonoBehaviour {
    private GameManager gm;

    public UILabel cashLabel;
    public UILabel yearLabel;
    public UILabel monthLabel;
    public UIGrid weekGrid;
    public Color activeWeekColor;
    public Color defaultWeekColor;
    private int week;

    void OnEnable() {
        gm = GameManager.Instance;
        week = gm.week;
        SetWeek();
    }

    void Update() {
        cashLabel.text = "$" + gm.playerCompany.cash;
        yearLabel.text = gm.year.ToString();
        monthLabel.text = gm.month.ToString().ToUpper();

        // If the week has changed,
        // update the UI.
        if (week != gm.week) {
            week = gm.week;
            SetWeek();
        }
    }

    private void SetWeek() {
        List<Transform> gridChildren = weekGrid.GetChildList();
        for (int i=0; i<gridChildren.Count; i++) {
            if (i == gm.week) {
                gridChildren[i].GetComponent<UITexture>().color = new Color(activeWeekColor.r, activeWeekColor.g, activeWeekColor.b, activeWeekColor.a);
            } else {
                gridChildren[i].GetComponent<UITexture>().color = new Color(defaultWeekColor.r, defaultWeekColor.g, defaultWeekColor.b, defaultWeekColor.a);
            }
        }
    }
}
