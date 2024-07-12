using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class ProductManager : MonoBehaviour
{
    [SerializeField] private string apiUrl = "https://dummyjson.com/products";
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform contentParent;
    private Vector3 pivot = new Vector3(30,0,0);

    void Start()
    {
        StartCoroutine(FetchProducts());
    }

    IEnumerator FetchProducts()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
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
        foreach (var product in products)
        {
            GameObject productGO = Instantiate(productPrefab, contentParent);

            // Locate the TextMeshPro and Image components inside the nested Canvas structure
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
