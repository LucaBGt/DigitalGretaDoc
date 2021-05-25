using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StressTest : MonoBehaviour
{

    public TextMeshProUGUI fpsCounter;
    public TextMeshProUGUI PlayerNo;

    float deltaTime = 0.0f;
    float msec;
    float fps;
    float fpsPercent;

    public GameObject PlayerPrefab;
    public List<GameObject> PlayerList = new List<GameObject>();
    public Transform parent;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void FixedUpdate()
    {
        foreach (GameObject g in PlayerList)
        {
            g.transform.Translate(new Vector3(Random.RandomRange(0.1f, -0.1f), 0, Random.RandomRange(0.1f, -0.1f)));
            g.transform.Rotate(new Vector3(0, Random.Range(-2f, 2f), 0));
        }
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        fpsCounter.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        fpsPercent = fps / 30;
        fpsCounter.color = Color.Lerp(Color.red, Color.green, fpsPercent);
    }

    public void AddPlayer()
    {
        GameObject newPlayer = Instantiate(PlayerPrefab, parent);
        PlayerList.Add(newPlayer);
        newPlayer.transform.localPosition = new Vector3(Random.RandomRange(5f, -5f), 0, Random.RandomRange(5f, -5f));
        newPlayer.transform.Rotate(new Vector3(0, Random.Range(0f, 360f), 0));
        UpdatePlayerNo();
    }

    public void RemovePlayer()
    {
        try
        {
            GameObject g = PlayerList[PlayerList.Count - 1];
            PlayerList.Remove(g);
            Destroy(g);
            UpdatePlayerNo();
        }
        catch (System.Exception e)
        {

        }
    }

    void UpdatePlayerNo()
    {
        PlayerNo.text = "Current Players: " + PlayerList.Count;
    }

    public void Good()
    {
        QualitySettings.SetQualityLevel(1, true);
    }

    public void Fast()
    {
        QualitySettings.SetQualityLevel(0, true);
    }
}
