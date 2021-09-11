using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HitController : MonoBehaviour
{
    public static HitController _instance = null;
    public static HitController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HitController>();
            }
            return _instance;
        }
    }

    public Text HitRankText;
    public SpriteRenderer Coin;
    private float TimeLimit = 0.5f;
    private Text HitCountText;
    private float timer;
    private float originalAlpha;
    private float totalHit;
    private enum Rank
    {
        NONE,
        LEMBUR,
        RODI,
        ROMUSHA
    }
    private Rank currentRank;

    // Start is called before the first frame update
    void Start()
    {
        // Coin.color = Color.red;
        // Coin.color = new Color(224, 159, 5, 255);
        // Coin.color = Color.Lerp(new Color(243f, 173f, 0f, 255f), new Color(243f, 0f, 54f, 255f), 0.01f);
        HitCountText = GetComponent<Text>();
        originalAlpha = HitCountText.color.a;
        ResetHitCountInfo();
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.unscaledDeltaTime;
        HitCountText.CrossFadeAlpha(0f, TimeLimit, false);
        // Coin.color = Color.Lerp(Color.white, new Color(255f, 0f, 54f, 255f), TimeLimit);
        if (timer <= 0f)
        {
            ResetHitCountInfo();
        }
    }

    public void IncrementHit()
    {
        totalHit++;
        RankUpdate();
        timer = TimeLimit;
        HitCountText.CrossFadeAlpha(originalAlpha, 0f, false);
        HitCountText.text = $"{totalHit} Hits";
    }
    public void ResetHitCountInfo()
    {
        Coin.color = Color.white;
        currentRank = Rank.NONE;
        totalHit = 0;
        HitCountText.CrossFadeAlpha(0f, 0f, false);
        HitCountText.text = $"{totalHit} Hits";
        HitRankText.text = "";
    }
    private void RankUpdate()
    {
        if (totalHit >= 25 && totalHit < 50)
        {
            currentRank = Rank.LEMBUR;
        }
        else if (totalHit >= 50 && totalHit < 100)
        {
            currentRank = Rank.RODI;
        }
        else if (totalHit >= 100)
        {
            currentRank = Rank.ROMUSHA;
        }
        else
        {
            currentRank = Rank.NONE;
        }
    }

    public float RankBonus()
    {
        switch (currentRank)
        {
            case Rank.LEMBUR:
                // Coin.color = Color.blue;
                HitRankText.text = "Lembur";
                return 2.0f;
            case Rank.RODI:
                Coin.color = Color.green;
                HitRankText.text = "Rodi";
                return 3.0f;
            case Rank.ROMUSHA:
                Coin.color = Color.red;
                HitRankText.text = "Romusha";
                return 4.0f;
            default:
                HitRankText.text = "";
                return 1.0f;
        }
    }
}