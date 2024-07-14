using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;


public class ProductSearchManager : MonoBehaviour
{
    [SerializeField] private string searchUrl = "https://dummyjson.com/products/search?q=";
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private Transform contentParent;
    [SerializeField] private TextMeshProUGUI searchInput;
    [SerializeField] private Button searchButton;

    void Start()
    {
        searchButton.onClick.AddListener(OnSearchButtonClicked);
    }

    void OnSearchButtonClicked()
    {
        string query = searchInput.text;
        if (!string.IsNullOrEmpty(query))
        {
            StartCoroutine(FetchSearchResults(query));
        }
    }

    IEnumerator FetchSearchResults(string query)
    {
        UnityWebRequest request = UnityWebRequest.Get(searchUrl + query);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            var jsonResponse = request.downloadHandler.text;
            SearchProductList productList = JsonUtility.FromJson<SearchProductList>(jsonResponse);
            DisplayProducts(productList.products);
        }
    }

    void DisplayProducts(List<Product> products)
    {
        // Clear previous search results
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // Display new search results
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
public class SearchProduct
{
    public int id;
    public string title;
    public double price;
    public string thumbnail;  // URL to the product image
    
}


[System.Serializable]
public class SearchProductList
{
    public List<Product> products;
}
