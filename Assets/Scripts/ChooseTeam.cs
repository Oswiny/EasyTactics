using HtmlAgilityPack;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTeam : MonoBehaviour
{


    public TMP_InputField teamInputField;
    public GameObject choosePrefab;
    public GameObject choosePlayerPrefab;
    public Canvas canvas;
    public GameObject parentObject;
    public GameObject playerParentObject;
    public GameObject yearSelectMenu;
    public GameObject playerConfirmMenu;
    public string searchFor;
    public bool searchClubs;
    public bool searchPlayers;
    public GameObject searchForClubsButton;
    public GameObject searchForPlayersButton;
    public GameObject noResults;

    static public string teamHref;
    static public string playerHref;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            
            
            
            Debug.Log(teamInputField.text);
        }
    }

    List<GameObject> spawnedSearchResults = new List<GameObject>();
    void search(string searchName)
    {
        string baseURL = "https://www.transfermarkt.com/schnellsuche/ergebnis/schnellsuche?query=";

        string searchableName = searchName.Replace(" ", "+");

        string searchURL = baseURL + searchableName;

        var web = new HtmlWeb();
        var document = web.Load(searchURL);
        var nodes = document.DocumentNode.SelectNodes(".//div[@class='large-12 columns']");
        if (nodes == null)
        {
            noResults.SetActive(true);
            return;
        }
        int index = -1;
        for (int i = 0; i < nodes.Count; i++)
        {
            if (nodes[i].SelectSingleNode(".//h2[@class='content-box-headline']").InnerText.Trim().Contains(searchFor))
            {
                index = i;
            }
        }

        var trueNodes = nodes[index].SelectNodes("div/div/div/table/tbody/tr[position()+1>1]");

        /*
        
        
        (trueNodes[0].SelectSingleNode(".//td[@class='hauptlink']").InnerText);
        Debug.Log(trueNodes[0].SelectSingleNode(".//td[@class='hauptlink']/a").GetAttributeValue("href", string.Empty).Trim());
        */

        for (int i = 0; i < trueNodes.Count; i++)
        {
            if (searchClubs)
            {
                GameObject spawnedChoosePrefab = Instantiate(choosePrefab, new Vector3(-400f, 70f - (i * 60f), 0f), choosePrefab.transform.rotation);
                spawnedChoosePrefab.transform.SetParent(parentObject.transform, false);
                spawnedSearchResults.Add(spawnedChoosePrefab);
                TMP_Text teamName = spawnedChoosePrefab.GetComponentInChildren<TMP_Text>();
                teamName.text = trueNodes[i].SelectSingleNode(".//td[@class='hauptlink']").InnerText;
                string href = trueNodes[i].SelectSingleNode(".//td[@class='hauptlink']/a").GetAttributeValue("href", string.Empty).Trim();
                spawnedChoosePrefab.GetComponentInChildren<Button>().onClick.AddListener(delegate { selectTeam(href); });
            }
            else if (searchPlayers)
            {
                GameObject spawnedChoosePlayerPrefab = Instantiate(choosePlayerPrefab);
                spawnedChoosePlayerPrefab.transform.SetParent(playerParentObject.transform, false);
                spawnedSearchResults.Add(spawnedChoosePlayerPrefab);
                TMP_Text[] texts = spawnedChoosePlayerPrefab.GetComponentsInChildren<TMP_Text>();
                texts[0].text = trueNodes[i].SelectSingleNode(".//td[@class='hauptlink']/a").InnerText;
                texts[1].text = trueNodes[i].SelectSingleNode("td[2]").InnerText;
                texts[2].text = trueNodes[i].SelectSingleNode("td[@class='rechts hauptlink']").InnerText;
                string href = trueNodes[i].SelectSingleNode(".//td[@class='hauptlink']/a").GetAttributeValue("href", string.Empty);
                spawnedChoosePlayerPrefab.GetComponentInChildren<Button>().onClick.AddListener(delegate { selectPlayer(href); });
                if (i >= 4)
                {
                    break;
                }
            }

        }

    }


    //possible optimization by grouping common lines
    public void onSearchButtonDown()
    {
        noResults.SetActive(false);
        if (spawnedSearchResults.Count == 0)
        {
            teamInputField.interactable = false;
            search(teamInputField.text);
            teamInputField.interactable = true;
        }
        else if (spawnedSearchResults.Count > 0)
        {
            spawnedSearchResults.ForEach(item => Destroy(item));
            teamInputField.interactable = false;
            search(teamInputField.text);
            teamInputField.interactable = true;
        }

        //playerConfirmMenu.SetActive(false);
        yearSelectMenu.SetActive(false);
        if(searchClubs)
        {
            parentObject.SetActive(true);
        }
        else if (searchPlayers)
        {
            playerParentObject.SetActive(true);
        }

    }

    static public bool requiredTeamScraping = false;
    static public bool requiredPlayerScraping = false;
    static public int teamSelectAmount = 0;
    static public GameObject oldTeam;
    public void selectTeam(string href)
    {
        teamHref = href;

        if (teamSelectAmount > 0)
        {
            oldTeam = Scraper.team1.attachedGameObject;
        }
        teamSelectAmount++;
        requiredTeamScraping = true;
    }

    public void selectPlayer(string href)
    {
        playerHref = href;
        Debug.Log(playerHref);
        requiredPlayerScraping = true;
    }

    public void chooseSearchGroup(string group)
    {
        searchFor = group;
        searchClubs = false;
        searchPlayers = false;
        if (group == "players")
        {
            searchPlayers = true;
        }
        else if (group == "Clubs")
        {
            searchClubs = true;
        }
    }

}
