using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;
using System;

public class ProductManager : MonoBehaviour
{
    //[SerializeField] private string apiUrl = "https://dummyjson.com/products";
    //private string apiUrl = "https://dummyjson.com/products?limit=12&skip=12";
    private string apiUrl = "https://dummyjson.com/products?limit=12&skip=";
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform contentParent;
    private int pageInteger;
    private int elementsInPage = 12;
    

    void Start()
    {
        pageInteger = 1;
        string skipElements = (pageInteger * elementsInPage).ToString();
        StartCoroutine(FetchProducts(skipElements));
        ButtonRight.Instance.OnRightButtonClicked += ButtonNextPrev_OnRightButtonClicked;
        ButtonLeft.Instance.OnLeftButtonClicked += ButtonNextPrev_nLeftButtonClicked;
    }

    private void ButtonNextPrev_OnRightButtonClicked(object sender, EventArgs e)
    {
        pageInteger++;
        string skipElements = (pageInteger * elementsInPage).ToString();
        StartCoroutine(FetchProducts(skipElements));
    }

    private void ButtonNextPrev_nLeftButtonClicked(object sender, EventArgs e)
    {
        if (pageInteger != 1)
        {
            pageInteger--;
            string skipElements = (pageInteger * elementsInPage).ToString();
            StartCoroutine(FetchProducts(skipElements));
        }
    }

    IEnumerator FetchProducts(string skipElements)
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl + skipElements);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            var jsonResponse = request.downloadHandler.text;
            ProductList productList = JsonUtility.FromJson<ProductList>(jsonResponse);
            DisplayProducts(productList.products);
        }
    }

    void DisplayProducts(List<Product> products)
    {
        // Clear previous products
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var product in products)
        {
            GameObject productGO = Instantiate(productPrefab, contentParent);

            TextMeshProUGUI nameText = productGO.transform.Find("ProductName").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI priceText = productGO.transform.Find("ProductPrice").GetComponent<TextMeshProUGUI>();
            Image productImage = productGO.transform.Find("ProductImage").GetComponent<Image>();

            if (nameText != null && priceText != null && productImage != null)
            {
                nameText.text = product.title;
                priceText.text = "$" + product.price.ToString("F2");
                StartCoroutine(LoadImage(product.thumbnail, productImage));
            }
            else
            {
                Debug.LogError("Components not found on the product prefab.");
            }
        }
    }

    IEnumerator LoadImage(string url, Image image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}

[System.Serializable]
public class Product
{
    public int id;
    public string title;
    public double price;
    public string thumbnail;  // URL to the product image
    
}

[System.Serializable]
public class ProductList
{
    public List<Product> products;
}
