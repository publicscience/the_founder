/*
 * New Product Flow
 * ================
 *
 * Selecting a Product Type, Industry, and Market
 * to start development of a new product.
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    private enum Aspect {
        PRODUCTTYPE,
        INDUSTRY,
        MARKET,
        COMPLETE
    }
    // Begin on Product Type selection.
    private Aspect aspect = Aspect.PRODUCTTYPE;

    // Lots of UI elements we will be managing...
    public UILabel aspectLabel;
    public UITexture background;
    public UICenterOnChild gridCenter;
    public UIGrid grid;
    public UIScrollView scrollView;
    public UIButton selectButton;
    public UILabel selectLabel;
    public UIWidget completedScreen;

    // The prefab for all product aspects.
    public GameObject productAspectPrefab;

    // Keep track of the selected product aspects.
    private ProductType productType;
    private Industry industry;
    private Market market;

    // These labels are used to display
    // the user's current selections.
    public UILabel productTypeLabel;
    public UILabel industryLabel;
    public UILabel marketLabel;

    // These display the user's selections
    // on the final confirmation screen.
    public UILabel finalProductTypeLabel;
    public UILabel finalIndustryLabel;
    public UILabel finalMarketLabel;

    void OnEnable() {
        gm = GameManager.Instance;
        background.color = new Color(0.61f,0.067f,0.57f,1f);

        gridCenter.onFinished = OnCenter;

        LoadProductTypes();
    }

    private void OnCenter() {
        // Re-enable the select button
        // when centering the item after a swipe is complete.
        selectButton.isEnabled = true;
    }

    void Update() {
        if (scrollView.isDragging) {
            // Disable the select button
            // while dragging.
            selectButton.isEnabled = false;
        }
    }

    public void Select() {
        switch (aspect) {
            case Aspect.PRODUCTTYPE:
                aspect = Aspect.INDUSTRY;
                aspectLabel.text = "INDUSTRY";
                background.color = new Color(1f,1f,1f,1f);

                productType = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as ProductType;
                productTypeLabel.text = productType.ToString();
                productTypeLabel.gameObject.SetActive(true);

                LoadIndustries();
                break;

            case Aspect.INDUSTRY:
                aspect = Aspect.MARKET;
                aspectLabel.text = "MARKET";
                background.color = new Color(1f,0.69f,1f,1f);

                industry = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as Industry;
                industryLabel.text = industry.ToString();
                industryLabel.gameObject.SetActive(true);

                LoadMarkets();
                break;

            case Aspect.MARKET:
                aspect = Aspect.COMPLETE;
                background.color = new Color(0.2f,0.69f,0.7f,1f);

                // Hide the aspect label.
                aspectLabel.gameObject.SetActive(false);

                market = gridCenter.centeredObject.GetComponent<UIProductAspect>().aspect as Market;
                marketLabel.text = market.ToString();
                marketLabel.gameObject.SetActive(true);

                scrollView.gameObject.SetActive(false);
                completedScreen.gameObject.SetActive(true);
                finalProductTypeLabel.text = productType.ToString();
                finalIndustryLabel.text = industry.ToString();
                finalMarketLabel.text = market.ToString();

                selectLabel.text = "Start";
                break;

            case Aspect.COMPLETE:
                gm.playerCompany.StartNewProduct(productType, industry, market);

                // TEMPORARY, this has bad performance.
                UIRoot.Broadcast("UpdateDevelopingProducts");

                UIRoot.Broadcast("ClosePopup");
                break;
        }
    }

    private void LoadProductTypes() {
        ClearGrid();
        foreach (ProductType a in gm.unlocked.productTypes) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void LoadIndustries() {
        ClearGrid();
        foreach (Industry a in gm.unlocked.industries) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void LoadMarkets() {
        ClearGrid();
        foreach (Market a in gm.unlocked.markets) {
            GameObject productAspect = NGUITools.AddChild(grid.gameObject, productAspectPrefab);
            productAspect.GetComponent<UIProductAspect>().aspect = a;
        }
        grid.Reposition();
        WrapGrid();
        gridCenter.Recenter();
    }

    private void ClearGrid() {
        while (grid.transform.childCount > 0)
            NGUITools.DestroyImmediate(grid.transform.GetChild(0).gameObject);
    }

    // The grid has to be re-wrapped whenever
    // its contents change, since UIWrapContent
    // caches the grid's contents on its start.
    private void WrapGrid() {
        NGUITools.DestroyImmediate(grid.gameObject.GetComponent<UIWrapContent>());
        UIWrapContent wrapper = grid.gameObject.AddComponent<UIWrapContent>();

        // Disable culling since it screws up the grid's layout.
        wrapper.cullContent = false;

        // The wrapper's item width is the same as the grid's cell width, duh
        wrapper.itemSize = (int)grid.cellWidth;
    }
}


