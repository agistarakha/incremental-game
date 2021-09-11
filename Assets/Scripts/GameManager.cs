using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    [Range(0f, 1f)] public float AutoCollectPercentage = 0.1f;
    public ResourceConfig[] ResourceConfigs;
    public Sprite[] ResourcesSprites;


    public Transform ResourcesParent;
    public ResourceController ResourcePrefab;
    public TapText TapTextPrefab;

    public Transform CoinIcon;
    public Text GoldInfo;
    public Text AutoCollectInfo;
    public Text TapCountInfo;
    public AudioSource CoinTapSound;
    private List<ResourceController> _activeResources = new List<ResourceController>();
    private List<TapText> _tapTextPool = new List<TapText>();
    private float _collectSecond;
    private double _totalGold;
    private int _totalTap = 0;

    // Start is called before the first frame update
    void Start()
    {
        AddAllResources();
    }

    // Update is called once per frame
    void Update()
    {
        _collectSecond += Time.unscaledDeltaTime;

        if (_collectSecond >= 1f)
        {

            CollectPerSecond();

            _collectSecond = 0f;

        }
        CheckResourceCost();
        CoinIcon.transform.localScale = Vector3.LerpUnclamped(CoinIcon.transform.localScale, Vector3.one * 2f, 0.15f);
        CoinIcon.Rotate(0f, 0f, Time.deltaTime * -1000f);
    }

    private void CheckResourceCost()
    {
        foreach (ResourceController resource in _activeResources)
        {
            bool isBuyable = false;
            if (resource.IsUnlocked)
            {
                isBuyable = TotalGold >= resource.GetUpgradeCost();

            }
            else
            {
                isBuyable = TotalGold >= resource.GetUnlockCost();
            }
            resource.ResourceImage.sprite = ResourcesSprites[isBuyable ? 1 : 0];
        }
    }
    public void CollectByTap(Vector3 tapPosition, Transform parent)
    {
        CoinTapSound.Play();
        HitController.Instance.IncrementHit();
        double output = 0;
        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput() * HitController.Instance.RankBonus();
            }
        }
        TapText tapText = GetOrCreateTapText();
        tapText.transform.SetParent(parent, false);
        tapText.transform.position = tapPosition;

        tapText.Text.text = $"+{output.ToString("0")}";
        tapText.gameObject.SetActive(true);
        CoinIcon.transform.localScale = Vector3.one * 1.75f;
        AddGold(output);
    }

    private TapText GetOrCreateTapText()
    {
        TapText tapText = _tapTextPool.Find(t => t.gameObject.activeSelf);
        if (tapText == null)
        {
            tapText = Instantiate(TapTextPrefab).GetComponent<TapText>();
            _tapTextPool.Add(tapText);
        }
        return tapText;
    }
    private void AddAllResources()
    {
        bool ShowResources = true;
        foreach (ResourceConfig config in ResourceConfigs)
        {
            GameObject obj = Instantiate(ResourcePrefab.gameObject, ResourcesParent, false);
            ResourceController resource = obj.GetComponent<ResourceController>();
            resource.SetConfig(config);
            obj.gameObject.SetActive(ShowResources);
            if (ShowResources && !resource.IsUnlocked)
            {
                ShowResources = false;
            }
            _activeResources.Add(resource);

        }
    }

    public void ShowNextResource()
    {
        foreach (ResourceController resource in _activeResources)
        {
            if (!resource.gameObject.activeSelf)
            {
                resource.gameObject.SetActive(true);
                break;

            }

        }

    }
    private void CollectPerSecond()
    {
        double output = 0;

        foreach (ResourceController resource in _activeResources)
        {
            if (resource.IsUnlocked)
            {
                output += resource.GetOutput();
            }


        }
        output *= AutoCollectPercentage;

        AutoCollectInfo.text = $"Auto Collect: { output.ToString("F1") } / second";
        AddGold(output);

    }

    public void AddGold(double value)
    {

        _totalGold += value;

        GoldInfo.text = $"Gold: { _totalGold.ToString("0") }";

    }

    public double TotalGold
    {
        get
        {
            return _totalGold;
        }
    }
}


[System.Serializable]
public struct ResourceConfig
{
    public string Name;
    public double UnlockCost;
    public double UpgradeCost;
    public double Output;

}

