using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static Scraper;

public class TeamSpawner : MonoBehaviour
{
    public TMP_Text position;
    public TMP_Text maxNumberOfPositions;
    public TMP_Text page;
    public TMP_Text maxNumberOfPages;
    public GameObject playerObject;
    public GameObject ballPrefab;

    Dictionary<string, List<Scraper.Player>> positionsAndPlayers;
    bool mMouseWheel = false;


    string currentKey;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Scraper.signalTeamSpawner)
        {
            Scraper.signalTeamSpawner = false;
            positionsAndPlayers = Scraper.team1.playerPositionDict();
            positionNo = 0;
            pageNo = 0;
            pager();
            createTeam();
            spawnAll();
            nextPosition();
            previousPosition();
            if (ChooseTeam.teamSelectAmount > 1)
            {
                Destroy(ChooseTeam.oldTeam);
            }
            JerseySelector.setJerseyDefaultColor();
        }
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            debugLogPages();
        }
    }
    static public List<Scraper.Player> playersCurrentlyOnDisplay = new List<Scraper.Player>();
    void showplayers(List<Scraper.Player> playersToDisplay)
    {
        foreach (Scraper.Player player in playersCurrentlyOnDisplay)
        {
            if (!player.isInPitch)
            {
                player.attachedGameObject.transform.position = restingPlace;
            }
        }

        playersCurrentlyOnDisplay.Clear();
        for (int i = 0; i < playersToDisplay.Count; i++)
        {
            if (playersToDisplay[i].isInPitch)
            {
                continue;
            }

            GameObject player = playersToDisplay[i].attachedGameObject;
            player.transform.position = new Vector3(7f, 3f + (i * -1.5f), 0f);

            PlayerObject playerPO = player.GetComponent<PlayerObject>();
            playerPO.playerJerseyNumber.text = playersToDisplay[i].jerseyNumber;
            playerPO.playerName.text = playersToDisplay[i].lastName;

            playersCurrentlyOnDisplay.Add(playersToDisplay[i]);
        }

    }

    Vector3 restingPlace = new Vector3(10f, 0f, 0f);

    void spawnAll()
    {
        for (int i = 0; i < Scraper.team1.players.Count; i++)
        {
            if (!Scraper.team1.players[i].isSpawned)
            {
                GameObject player = Instantiate(playerObject, restingPlace, playerObject.transform.rotation);
                Scraper.team1.players[i].attachedGameObject = player;

                PlayerObject playerPO = player.GetComponent<PlayerObject>();

                playerPO.playerJerseyNumber.text = Scraper.team1.players[i].jerseyNumber;
                playerPO.playerName.text = Scraper.team1.players[i].lastName;
                playerPO.attachedPlayer = Scraper.team1.players[i];

                player.name = Scraper.team1.players[i].lastName;

                Scraper.team1.players[i].img = playerPO.jerseyEIMG;

                Scraper.team1.players[i].isSpawned = true;

                StartCoroutine(downloadAndSetIMG(Scraper.team1.players[i].imgURL, Scraper.team1.players[i].img));
                player.transform.SetParent(team1.attachedGameObject.transform, false);
            }
        }
    }

    void createTeam()
    {
        if (team1.attachedGameObject == null)
        {
            team1.attachedGameObject = new GameObject(team1.name);
        }
    }


    IEnumerator downloadAndSetIMG(string url, RawImage setDownloadedIMGto)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D downloadedIMG = ((DownloadHandlerTexture)request.downloadHandler).texture;

            //scraped for now
            /*
            Color[] pixelArray = downloadedIMG.GetPixels();
            
            for (int i = 0; i < pixelArray.Length; i++)
            {
                if (pixelArray[i].r > 0.85f && pixelArray[i].g > 0.85f && pixelArray[i].b > 0.85f)
                {
                    
            
            ("CLEARED COLOR");
                    pixelArray[i] = new Color(0, 0, 0, 0);
                    // STOPPED HERE <-
                    // current problem: cant remove white bg from player images
                }
            }

            downloadedIMG.SetPixels(pixelArray);
            downloadedIMG.Apply();
            */
            setDownloadedIMGto.texture = downloadedIMG;
            setDownloadedIMGto.SetNativeSize();
        }

    }


    int maxDisplayAtAPage = 4;
    Dictionary<string, List<List<Scraper.Player>>> pages;
    void pager()
    {
        pages = new Dictionary<string, List<List<Scraper.Player>>>();

        foreach (string position in positionsAndPlayers.Keys.ToList())
        {
            if (positionsAndPlayers[position].Count <= maxDisplayAtAPage)
            {
                pages[position] = new List<List<Scraper.Player>>() { positionsAndPlayers[position] };
            }
            else
            {
                pages[position] = new List<List<Scraper.Player>>();
                for (int i = 0; i < positionsAndPlayers[position].Count; i = i + maxDisplayAtAPage)
                {
                    if (maxDisplayAtAPage + i <= positionsAndPlayers[position].Count)
                    {
                        pages[position].Add(positionsAndPlayers[position].GetRange(i, maxDisplayAtAPage));
                    }
                    else
                    {
                        int count = Math.Min(maxDisplayAtAPage, positionsAndPlayers[position].Count - i);
                        //maxDisplayAtAPage + i - positionsAndPlayers[position].Count - 2
                        pages[position].Add(positionsAndPlayers[position].GetRange(i, count));
                    }
                }

            }
        }
    }




    void debugLogPages()
    {
        foreach (string key in pages.Keys.ToList())
        {
            int i = 0;
            foreach (List<Scraper.Player> value in pages[key].ToList())
            {
                foreach (Scraper.Player valuevalue in value)
                {
                    
                    
                    Debug.Log("Position: " + key + " Name: " + valuevalue.lastName + " at the page : " + i);
                }
                i++;
            }
        }
    }



    List<GameObject> balls = new List<GameObject>();


    //button funcs



    int positionNo = 0;
    public void nextPosition()
    {
        if (positionNo + 1 == positionsAndPlayers.Keys.Count)
        {
            playersCurrentlyOnDisplay.ForEach(item => item.attachedGameObject.transform.position = restingPlace);

        }
        else if (positionNo + 1 < positionsAndPlayers.Keys.Count)
        {
            positionNo++;
        }
        else
        {
            positionNo = 0;
        }

        position.text = positionsAndPlayers.Keys.ToList()[positionNo];
        pageNo = 0;
        page.text = "1";
        maxNumberOfPages.text = (Mathf.Ceil((float)positionsAndPlayers[positionsAndPlayers.Keys.ToList()[positionNo]].Count / 4.0f)).ToString();
        showplayers(pages[position.text][pageNo]);
    }

    public void previousPosition()
    {
        if (positionNo - 1 >= 0)
        {
            positionNo--;
        }
        else
        {
            positionNo = positionsAndPlayers.Keys.Count - 1;
        }

        position.text = positionsAndPlayers.Keys.ToList()[positionNo];
        pageNo = 0;
        page.text = "1";
        maxNumberOfPages.text = (Mathf.Ceil((float)positionsAndPlayers[positionsAndPlayers.Keys.ToList()[positionNo]].Count / 4.0f)).ToString();
        showplayers(pages[position.text][pageNo]);
    }


    int pageNo = 0;
    public void nextPage()
    {
        if (pageNo + 1 < Convert.ToInt32(maxNumberOfPages.text))
        {
            pageNo++;
        }
        else
        {
            pageNo = 0;
        }

        page.text = (pageNo + 1).ToString();
        showplayers(pages[position.text][pageNo]);
    }


    public void previousPage()
    {
        if (pageNo - 1 >= 0)
        {
            pageNo--;
        }
        else
        {
            pageNo = Convert.ToInt32(maxNumberOfPages.text) - 1;
        }

        page.text = (pageNo + 1).ToString();
        showplayers(pages[position.text][pageNo]);
    }

}
