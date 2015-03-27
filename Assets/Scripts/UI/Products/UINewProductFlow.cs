/*
 * New Product Flow
 * ================
 *
 * Select one or more Product Types
 * to start development of a new product.
 */

using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UINewProductFlow : MonoBehaviour {
    private GameManager gm;

    public GameObject productTypePrefab;
    public GameObject selectedProductTypePrefab;
    public GameObject productTypeSelectionView;
    public List<GameObject> productTypeItems;
    public UIGrid selectedGrid;
    public UIGrid grid;
    public UIButton confirmSelectionButton;

    // Keep track of the selected product aspects.
    private List<ProductType> productTypes;
    private Product product;

    void OnEnable() {
        gm = GameManager.Instance;

        if (productTypes == null) {
            productTypes = new List<ProductType>();
            productTypeItems = new List<GameObject>();
        }

        UpdateConfirmButton();
        LoadProductTypes();
    }

    // Load product types into the grid.
    private void LoadProductTypes() {
        productTypeItems.Clear();
        foreach (ProductType pt in gm.unlocked.productTypes.Where(p => p.isAvailable(gm.playerCompany))) {
            GameObject productType = NGUITools.AddChild(grid.gameObject, productTypePrefab);
            UIEventListener.Get(productType).onClick += SelectProductType;
            productType.GetComponent<UIProductType>().productType = pt;
            productTypeItems.Add(productType);
            ToggleProductType(pt, productType);
        }
        grid.Reposition();
    }

    private void ToggleProductType(ProductType pt, GameObject obj) {
        if (productTypes.Contains(pt)) {
            obj.transform.Find("Overlay").gameObject.SetActive(true);
            return;
        }
    }

    // Select a product type for the new product.
    private void SelectProductType(GameObject obj) {
        // Required product types is 2.
        if (selectedGrid.transform.childCount < 2) {
            ProductType pt = obj.GetComponent<UIProductType>().productType;

            // Can't have two of the same product type.
            if (productTypes.Contains(pt))
                return;

            GameObject productType = NGUITools.AddChild(selectedGrid.gameObject, selectedProductTypePrefab);

            productType.transform.Find("Label").GetComponent<UILabel>().text = pt.name;
            Transform po = productType.transform.Find("Product Object");
            po.GetComponent<MeshFilter>().mesh = pt.mesh;

            // Deselect on click.
            UIEventListener.Get(productType.gameObject).onClick += delegate(GameObject go) {
                NGUITools.Destroy(productType);
                productTypes.Remove(pt);
                selectedGrid.Reposition();

                UpdateConfirmButton();
                UpdateProductTypeItems();
            };

            productTypes.Add(pt);
            selectedGrid.Reposition();

            UpdateConfirmButton();
            UpdateProductTypeItems();
        }
    }

    // Update which product types can be selected.
    private void UpdateProductTypeItems() {
        foreach (GameObject item in productTypeItems) {
            ProductType pt = item.GetComponent<UIProductType>().productType;
            ToggleProductType(pt, item);
        }
    }

    public void BeginProductDevelopment() {
        UIManager.Instance.Confirm("Are you happy with this product configuration?", BeginProductDevelopment_, null);
    }

    private void BeginProductDevelopment_() {
        gm.playerCompany.StartNewProduct(productTypes, 0, 0, 0);
        SendMessageUpwards("Close");
    }

    private void UpdateConfirmButton() {
        if (productTypes.Count == 2) {
            confirmSelectionButton.isEnabled = true;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = "Ok, let's get started";
        } else if (productTypes.Count == 0) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select two product types");
        } else if (productTypes.Count == 1) {
            confirmSelectionButton.isEnabled = false;
            confirmSelectionButton.transform.Find("Label").GetComponent<UILabel>().text = string.Format("Select one more product type");
        }
    }
}


