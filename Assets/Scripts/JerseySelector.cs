using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class JerseySelector : MonoBehaviour
{
    public Image teamColorImage1;
    public Image teamColorImage2;
    public Image teamColorImage3;
    public List<Image> teamColorImageList;
    public GameObject styles;

    public GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        currentlySelectedStyle = 0;
    }


    // Update is called once per frame
    void Update()
    {
        if (Scraper.signalJerseySelector && Scraper.team1.teamColors != null)
        {
            for (int i = 0; i < Scraper.team1.teamColors.Count; i++)
            {
                teamColorImageList[i].color = Scraper.team1.teamColors[i];
            }
            Scraper.signalJerseySelector = false;
        }
    }

    static int currentlySelectedStyle;

    public void styleSelect(int style)
    {
        currentlySelectedStyle = style;
        for (int i = 0; i < Scraper.team1.players.Count; i++)
        {
            Transform styles = Scraper.team1.players[i].attachedGameObject.transform.Find("Styles");
            for (int j = 0; j < Scraper.team1.players[i].attachedGameObject.transform.Find("Styles").childCount; j++)
            {
                styles.GetChild(j).gameObject.SetActive(false);
            }
            styles.GetChild(style).gameObject.SetActive(true);
        }
    }

    public void colorSelect(int color)
    {
        bool alpha1 = Input.GetKey(KeyCode.Alpha1);
        bool alpha2 = Input.GetKey(KeyCode.Alpha2);
        bool alpha3 = Input.GetKey(KeyCode.Alpha3);


        for (int i = 0; i < Scraper.team1.players.Count; i++)
        {
            SpriteRenderer[] sprites = Scraper.team1.players[i].attachedGameObject.transform.Find("Styles").gameObject.GetComponentsInChildren<SpriteRenderer>();

            if (currentlySelectedStyle == 0 || alpha1)
            {
                sprites[0].color = Scraper.team1.teamColors[color];
            }
            else if (alpha2)
            {
                sprites[1].color = Scraper.team1.teamColors[color];
            }
            else if (currentlySelectedStyle == 2 && alpha3)
            {
                sprites[2].color = Scraper.team1.teamColors[color];
            }
            else if (currentlySelectedStyle == 3)
            {
                sprites[0].color = Scraper.team1.teamColors[color];
                sprites[1].color *= 0.8f;
            }
        }
    }

    static public void setJerseyDefaultColor()
    {
        for (int i = 0; i < Scraper.team1.players.Count; i++)
        {
            GameObject selectedJersey = Scraper.team1.players[i].attachedGameObject.transform.Find("Styles").GetChild(currentlySelectedStyle).gameObject;
            SpriteRenderer[] sprites = selectedJersey.GetComponentsInChildren<SpriteRenderer>();
            if (Scraper.team1.teamColors != null)
            {
                sprites[0].color = Scraper.team1.teamColors[0];
            }
        }
    }

}
