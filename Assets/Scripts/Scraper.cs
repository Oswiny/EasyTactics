using HtmlAgilityPack;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Scraper : MonoBehaviour
{


    public RawImage teamLogoIMG;
    public RawImage playerIMGTest;
    public TMP_Dropdown yearSelectDD;
    public GameObject parentObject;
    public GameObject playerParentObject;
    public GameObject yearSelectMenu;
    public GameObject chooseTeamMenu;
    public GameObject playerConfirmMenu;
    public TMP_Text confirmName;
    public TMP_Text confirmClub;
    public RawImage confirmLogo;
    public TMP_Text confirmPositon;
    public TMP_Text confirmValue;
    public static string selectedYear;

    /*
     * 
     */

    // Start is called before the irst frame update
    public class Player
    {
        public string jerseyNumber { get; set; }
        public string imgURL { get; set; }
        public string fullName { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public bool isCaptain { get; set; }
        public bool isInjured { get; set; }
        public string injury { get; set; }
        public bool isEligbleForAll { get; set; }
        public string eligibility { get; set; }
        public string reason { get; set; }
        public string position { get; set; }
        public string birthDate { get; set; }
        public string age { get; set; }
        public List<string> nationalities { get; set; }
        public string height { get; set; }
        public string foot { get; set; }
        public string joinDate { get; set; }
        public string joinedFrom { get; set; }
        public string joinedFee { get; set; }
        public string contract { get; set; }
        public string marketValue { get; set; }

        public bool isSpawned = false;

        public bool isInPitch = false;

        public GameObject attachedGameObject;

        public RawImage img;
        public Player(string jerseyNumber, string imgURL, string fullname, string firstname, string middlename, string lastname, bool isCaptain, bool isInjured, string injury, bool isEligbleForAll, string eligibility, string reason, string position, string birthDate, string age, List<string> nationalities, string height, string foot, string joinDate, string joinedFrom, string joinedFee, string contract, string marketValue)
        {
            this.jerseyNumber = jerseyNumber;
            this.imgURL = imgURL;
            this.fullName = fullname;
            this.firstName = firstname;
            this.middleName = middlename;
            this.lastName = lastname;
            this.isCaptain = isCaptain;
            this.isInjured = isInjured;
            this.injury = injury;
            this.eligibility = eligibility;
            this.isEligbleForAll = isEligbleForAll;
            this.reason = reason;
            this.position = position;
            this.birthDate = birthDate;
            this.age = age;
            this.nationalities = nationalities;
            this.height = height;
            this.foot = foot;
            this.joinDate = joinDate;
            this.joinedFrom = joinedFrom;
            this.joinedFee = joinedFee;
            this.contract = contract;
            this.marketValue = marketValue;

        }

        public Player(string name, RawImage img, GameObject attachedGameObject)
        {
            this.fullName = name;
            this.img = img;
            this.attachedGameObject = attachedGameObject;
        }
    }

    public class Team
    {
        public string name { get; set; }

        public List<Player> players { get; set; }

        public List<Color> teamColors { get; set; }

        public GameObject attachedGameObject;
        public Team(string name, List<Player> players, List<Color> teamColors)
        {
            this.name = name;
            this.players = players;
            this.teamColors = teamColors;
        }

        public Team()
        {

        }

        public Dictionary<string, List<Player>> playerPositionDict()
        {
            Dictionary<string, List<Player>> playerPositionsDictionary = new Dictionary<string, List<Player>>();
            foreach (Player player in players)
            {
                if (playerPositionsDictionary.ContainsKey(player.position))
                {
                    playerPositionsDictionary[player.position].Add(player);
                }
                else
                {
                    playerPositionsDictionary[player.position] = new List<Player>() { player };
                }
            }
            return playerPositionsDictionary;
        }

    }

    static public Team team1 = new Team();
    void Start()
    {
        team1.players = new List<Player>();
    }

    static public bool signalTeamSpawner;
    static public bool signalJerseySelector;

    string teamURL;
    string colorURL;
    void Update()
    {
        if (ChooseTeam.requiredTeamScraping)
        {
            ChooseTeam.requiredTeamScraping = false;
            parentObject.SetActive(false);
            HtmlDocument defaultDocument = getDefaultTeamDocument(ChooseTeam.teamHref);
            logoScrape(defaultDocument);
            getAvailableYears(defaultDocument);
            yearSelectMenu.SetActive(true);
        }
        else if (ChooseTeam.requiredPlayerScraping)
        {
            ChooseTeam.requiredPlayerScraping = false;
            playerParentObject.SetActive(false);
            scrapeSinglePlayer(hrefToPlayerPage(ChooseTeam.playerHref));


            signalTeamSpawner = true;
            signalJerseySelector = true;
            //playerConfirmMenu.SetActive(true);
            Debug.Log("here");
        }
    }



    void scrape(string teamURL, string colorURL)
    {
        var web = new HtmlWeb();
        var document = web.Load(teamURL);
        var nodes = document.DocumentNode.SelectNodes("//*[@id=\"yw1\"]/table/tbody/tr[position()>0]");

        var colorWeb = new HtmlWeb();
        var colorDocument = colorWeb.Load(colorURL);
        var colorNode = colorDocument.DocumentNode.SelectSingleNode("//p[@class='vereinsfarbe']");


        List<Color> colorList = new List<Color>();

        for (int i = 0; i < colorNode.SelectNodes("span[position()+1>1]").Count; i++)
        {
            string colorData = colorNode.SelectSingleNode("span[" + (i + 1) + "]").GetAttributeValue("style", string.Empty);
            Color color = getColor(colorData);
            colorList.Add(color);
        }



        // gives out player jersey number, change zero to change player  -> nodes[0].SelectSingleNode("td[1]").InnerText
        // gives out player name, change zero to change player, for player that are injured/captain of the team it adds &nbps to end of it make sure to remove that
        // -> nodes[0].SelectSingleNode(".//td[@class='hauptlink']/a").InnerText.Trim()
        // gives out player position, change zero to change player -> nodes[0].SelectSingleNode(".//td[2]//tr[2]/td").InnerText.Trim()
        // gives out player age, change zero to change player -> nodes[0].SelectSingleNode("td[3]").InnerText
        // gives out player nationality, change zero to change player -> nodes[0].SelectSingleNode(".//img[@class='flaggenrahmen']").GetAttributeValue("title", string.Empty)
        // gives out player contract, change zero to change player -> nodes[0].SelectSingleNode("td[5]").InnerText
        // gives out player market value, change zero to change player -> nodes[0].SelectSingleNode("td[6]").InnerText

        //team info
        //var teamInfoNode = document.DocumentNode.SelectSingleNode("/html/body/div/main/header");
        //team name -> teamInfoNode.SelectSingleNode("div[@class='data-header__headline-container']").InnerText.Trim()

        var teamInfoNode = document.DocumentNode.SelectSingleNode("/html/body/div/main/header");
        string teamName = teamInfoNode.SelectSingleNode("div[@class='data-header__headline-container']").InnerText.Trim();

        List<Player> teamPlayers = new List<Player>();

        for (int i = 0; i < nodes.Count; i++)
        {

            string playerJerseyNumber = nodes[i].SelectSingleNode("td[1]").InnerText;

            string playerFullName = nodes[i].SelectSingleNode(".//td[@class='hauptlink']/a/text()").InnerText.Trim();
            (string playerFirstName, string playerMiddleName, string playerLastName) = nameSplitter(playerFullName);

            (bool playerIsCaptain, bool playerIsInjured, string playerInjury, bool playerIsEligbleForAll, string playerEligibility) = getCaptainInjuredEligibilityData(nodes[i].SelectNodes(".//td[@class='hauptlink']/a/span[position()>0]"));

            HtmlNode playerReasonNode = nodes[i].SelectSingleNode(".//td[@class='posrela']/a");
            string playerReason = string.Empty;
            if (playerReasonNode != null)
            {
                playerReason = playerReasonNode.GetAttributeValue("title", string.Empty);
            }

            string playerPosition = nodes[i].SelectSingleNode(".//td[2]//tr[2]/td").InnerText.Trim();

            string playerBirthDateAndAge = nodes[i].SelectSingleNode("td[3]").InnerText;
            (string playerBirthDate, string playerAge) = birthDateAndAgeSplitter(playerBirthDateAndAge);


            List<string> playerNationalities = getPlayerNationalitiesFromNode(nodes[i]);

            string playerHeight = nodes[i].SelectSingleNode("td[5]").InnerText;
            string playerFoot = nodes[i].SelectSingleNode("td[6]").InnerText;
            string playerJoinDate = nodes[i].SelectSingleNode("td[7]").InnerText;

            string playerJoinedClubAndFee = nodes[i].SelectSingleNode("td[8]/a").GetAttributeValue("title", string.Empty);
            (string playerJoinedFrom, string playerJoinFee) = getJoinedClubAndFee(playerJoinedClubAndFee);

            string playerContract = nodes[i].SelectSingleNode("td[9]").InnerText;

            string playerMarketValue = nodes[i].SelectSingleNode("td[10]").InnerText;

            //Debug.Log(playerFullName + " " + nodes[i].SelectSingleNode("td[@class='posrela']/table/tbody/tr[1]/td[1]/img").GetAttributeValue("data-src", string.Empty));
            var imgNodes = nodes[i].SelectNodes("td[@class='posrela']//img");
            int imgCount = imgNodes.Count;

            string playerImgURL = imgNodes[imgCount - 1].GetAttributeValue("data-src", string.Empty);


            teamPlayers.Add(new Player(playerJerseyNumber, playerImgURL, playerFullName, playerFirstName, playerMiddleName, playerLastName, playerIsCaptain, playerIsInjured, playerInjury, playerIsEligbleForAll, playerEligibility, playerReason, playerPosition, playerBirthDate, playerAge, playerNationalities, playerHeight, playerFoot, playerJoinDate, playerJoinedFrom, playerJoinFee, playerContract, playerMarketValue));
        }

        team1 = new Team(teamName, teamPlayers, colorList);
    }

    void scrapeSinglePlayer(string playerPageURL)
    { 
        //improvement is required
        Debug.Log(playerPageURL);
        var web = new HtmlWeb();
        var document = web.Load(playerPageURL);
        var infoHeader = document.DocumentNode.SelectSingleNode(".//header[@class='data-header']");
        var infoData = document.DocumentNode.SelectNodes(".//div[@class='large-6 large-pull-6 columns print spielerdatenundfakten']/div/span[position()>0]");

        string playerJerseyNumber = infoHeader.SelectSingleNode(".//span[@class='data-header__shirt-number']").InnerText.Remove(0);
        string playerImgURL = infoHeader.SelectSingleNode(".//img[@class='data-header__profile-image']").GetAttributeValue("src", string.Empty);
        string playerFullName = infoHeader.SelectSingleNode(".//img[@class='data-header__profile-image']").GetAttributeValue("title", string.Empty);
        (string playerFirstName, string playerMiddleName, string playerLastName) = nameSplitter(playerFullName);
        bool playerIsCaptain = false;
        bool playerIsInjured = false;
        string playerInjury = string.Empty;
        bool playerIsEligibleForAll = true;
        string playerEligibility = string.Empty;
        string playerReason = string.Empty;
        string playerPosition = infoHeader.SelectSingleNode("div[5]/div[1]/ul[2]/li[2]/span[1]").InnerText.Trim();
        string playerBirthAndAge = infoHeader.SelectSingleNode(".//span[@itemprop='birthDate']").InnerText.Trim();
        (string playerBirthDate, string playerAge) = birthDateAndAgeSplitter(playerBirthAndAge);

        List<string> playerNationalities = new List<string>();
        string playerFoot = "-";
        for (int i = 0; i < infoData.Count; i++)
        {
            if (infoData[i].InnerText == "Citizenship:")
            {
                HtmlNodeCollection citizenshipNodes = infoData[i + 1].SelectNodes(".//img");
                for (int j = 0; j < citizenshipNodes.Count; j++)
                {
                    playerNationalities.Add(citizenshipNodes[j].GetAttributeValue("title", string.Empty));
                }
            }
            else if (infoData[i].InnerText == "Foot:")
            {
                Debug.Log(infoData[i + 1].InnerText);
            }
        }

        string playerHeight = infoHeader.SelectSingleNode(".//span[@itemprop='height']").InnerText.Trim();
        string playerJoinDate = infoHeader.SelectSingleNode("div[3]/div[1]/span[4]/span[1]").InnerText;
        string playerJoinedFrom = string.Empty;
        string playerJoinedFee = string.Empty;
        string playerContract = infoHeader.SelectSingleNode("div[3]/div[1]/span[5]/span[1]").InnerText;
        string playerMarketValue = infoHeader.SelectSingleNode(".//a[@class='data-header__market-value-wrapper']").InnerText.Trim();
        playerMarketValue = playerMarketValue.Remove(playerMarketValue.IndexOf('L'), playerMarketValue.Length - playerMarketValue.IndexOf('L'));
        /*
        Debug.Log(playerJerseyNumber);
        Debug.Log(playerImgURL);
        Debug.Log(playerFullName);
        Debug.Log(playerFirstName);
        Debug.Log(playerMiddleName);
        Debug.Log(playerLastName);
        Debug.Log(playerIsCaptain);
        Debug.Log(playerIsInjured);
        Debug.Log(playerInjury);
        Debug.Log(playerIsEligibleForAll);
        Debug.Log(playerEligibility);
        Debug.Log(playerReason);
        Debug.Log(playerPosition);
        Debug.Log(playerBirthAndAge);
        Debug.Log(playerBirthDate);
        Debug.Log(playerAge);
        foreach(string nationality in playerNationalities)
        {
            Debug.Log(nationality);
        }
        Debug.Log(playerHeight);
        Debug.Log(playerFoot);
        Debug.Log(playerJoinDate);
        Debug.Log(playerJoinedFrom);
        Debug.Log(playerJoinedFee);
        Debug.Log(playerContract);
        Debug.Log(playerMarketValue);
        */
        Player importedPlayer = new Player(playerJerseyNumber, playerImgURL, playerFullName, playerFirstName, playerMiddleName, playerLastName, playerIsCaptain, playerIsInjured, playerInjury, playerIsEligibleForAll, playerEligibility, playerReason, playerPosition, playerBirthDate, playerAge, playerNationalities, playerHeight, playerFoot, playerJoinDate, playerJoinedFrom, playerJoinedFee, playerContract, playerMarketValue);
        team1.players.Add(importedPlayer);
    }

    string hrefToPlayerPage(string href)
    {
        return "https://www.transfermarkt.com" + href;
    }

    void downloadAllPlayerIMGs()
    {
        for (int i = 0; i < team1.players.Count; i++)
        {
            StartCoroutine(downloadAndSetIMG(team1.players[i].imgURL, team1.players[i].attachedGameObject.GetComponent<PlayerObject>().jerseyEIMG));
        }
    }

    //might update so that it lets you select year.
    Dictionary<string, string> yearValueAndDisplay = new Dictionary<string, string>();

    string hrefToTeamURL(string href, string year)
    {
        string baseURL = "https://www.transfermarkt.com";
        string baseTeamURL = baseURL + href;

        string seasonSelect = "/plus/1/galerie/0?saison_id=" + year;

        string URL = baseTeamURL.Replace("startseite", "kader") + seasonSelect;

        return URL;
    }

    HtmlDocument getDefaultTeamDocument(string href)
    {
        string baseURL = "https://www.transfermarkt.com";
        string baseTeamURL = baseURL + href;
        string defaultSeasonSelect = "/plus/0/galerie/0?saison_id=2024";
        string URL = baseTeamURL.Replace("startseite", "kader") + defaultSeasonSelect;
        var web = new HtmlWeb();
        var document = web.Load(URL);

        return document;
    }

    void getAvailableYears(HtmlDocument document)
    {
        var nodes = document.DocumentNode.SelectNodes("//select[@class='chzn-select']/option[position()>0]");

        for (int i = 0; i < nodes.Count; i++)
        {
            string key = nodes[i].GetAttributeValue("value", string.Empty);
            yearValueAndDisplay[key] = nodes[i].InnerText;
        }

        yearSelectDD.AddOptions(yearValueAndDisplay.Values.ToList());
    }

    void logoScrape(HtmlDocument document)
    {
        var node = document.DocumentNode.SelectSingleNode("//*[@id=\"tm-main\"]/header/div[4]/img");
        string imgURL = node.GetAttributeValue("src", string.Empty);
        StartCoroutine(downloadAndSetIMG(imgURL, teamLogoIMG));
    }



    string hrefToColorURL(string href)
    {
        string baseURL = "https://www.transfermarkt.com";
        string colorHref = href.Replace("startseite", "datenfakten");
        string URL = baseURL + colorHref;
        return URL;
    }


    //nodes[0].SelectSingleNode("td[@class='posrela']//img").GetAttributeValue("src", string.Empty)

    IEnumerator downloadAndSetIMG(string url, RawImage setDownloadedIMGto)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);


        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture downloadedIMG = ((DownloadHandlerTexture)request.downloadHandler).texture;
            setDownloadedIMGto.texture = downloadedIMG;
            setDownloadedIMGto.SetNativeSize();
        }

    }

    Color getColor(string data)
    {
        Color color;

        data = data.Replace("background-color:", "");
        data = data.Replace(";", "");

        ColorUtility.TryParseHtmlString(data, out color);
        return color;
    }

    List<string> getPlayerNationalitiesFromNode(HtmlNode node)
    {
        HtmlNodeCollection nationalityNodes = node.SelectNodes(".//img[@class='flaggenrahmen']");
        List<string> nationalities = new List<string>();
        for (int j = 0; j < nationalityNodes.Count; j++)
        {
            nationalities.Add(nationalityNodes[j].GetAttributeValue("title", string.Empty));
        }
        return nationalities;
    }

    (string first, string middle, string last) nameSplitter(string fullName)
    {
        List<string> names = fullName.Split(" ").ToList();
        for (int i = names.Count - 1; i > 0; i--)
        {

            if (names[i][0].ToString().ToLower() == names[i][0].ToString())
            {
                names[i] = names[i] + " " + names[i + 1];
                names.RemoveAt(i + 1);
            }
            else if (names[i] == "Junior" || names[i] == "Jr.")
            {
                names[i] = "JR";
                names[i - 1] = names[i - 1] + " " + names[i];
                names.RemoveAt(i);
            }
        }

        if (names.Count >= 3)
        {
            return (first: names[0], middle: string.Join(" ", names.GetRange(1, names.Count - 2)), last: names[names.Count - 1]);
        }
        else if (names.Count == 2)
        {
            return (first: names[0], middle: string.Empty, last: names[names.Count - 1]);
        }
        else if (names.Count == 1)
        {
            return (first: string.Empty, middle: string.Empty, last: names[names.Count - 1]);
        }
        else
        {
            return (first: string.Empty, middle: string.Empty, last: string.Empty);
        }
    }


    (string birthDate, string age) birthDateAndAgeSplitter(string birthDateandAge)
    {
        string[] strings = birthDateandAge.Split(" (");
        strings[1] = strings[1].Replace(")", string.Empty);
        return (birthDate: strings[0], age: strings[1]);
    }

    (string club, string fee) getJoinedClubAndFee(string joinedClubAndFee)
    {
        string[] joinedClubAndFeeSplitted = joinedClubAndFee.Split(":");
        joinedClubAndFeeSplitted[1] = joinedClubAndFeeSplitted[1].Replace(" Ablöse ", string.Empty);

        return (club: joinedClubAndFeeSplitted[0], fee: joinedClubAndFeeSplitted[1]);
    }

    (bool, bool, string, bool, string) getCaptainInjuredEligibilityData(HtmlNodeCollection CIEdata)
    {
        bool isCaptain = false;
        bool isInjured = false;
        string injury = string.Empty;
        bool isEligbleForAll = true;
        string eligibility = string.Empty;
        if (CIEdata == null)
        {
            return (isCaptain, isInjured, injury, isEligbleForAll, eligibility);
        }

        for (int i = 0; i < CIEdata.Count; i++)
        {
            if (CIEdata[i].GetAttributeValue("class", string.Empty) == "kapitaenicon-table icons_sprite")
            {
                isCaptain = true;
            }
            else if (CIEdata[i].GetAttributeValue("class", string.Empty) == "verletzt-table icons_sprite")
            {
                isInjured = true;
                injury = CIEdata[i].GetAttributeValue("title", string.Empty);
            }
            else if (CIEdata[i].GetAttributeValue("class", string.Empty) == "ausfall-table icons_sprite")
            {
                isEligbleForAll = false;
                eligibility = CIEdata[i].GetAttributeValue("title", string.Empty);
            }

        }
        return (isCaptain, isInjured, injury, isEligbleForAll, eligibility);
    }

    //button funcs

    public void startScraping()
    {
        selectedYear = yearValueAndDisplay.Keys.ToList()[yearSelectDD.value];
        teamURL = hrefToTeamURL(ChooseTeam.teamHref, selectedYear);
        colorURL = hrefToColorURL(ChooseTeam.teamHref);
        scrape(teamURL, colorURL);
        signalTeamSpawner = true;
        signalJerseySelector = true;
        chooseTeamMenu.SetActive(false);
    }


    //doesnt work might scrape

    /*
    no market value
    no jersey number
    not retired etc
     */

    public void chooseTeamMenuToggle()
    {
        chooseTeamMenu.SetActive(!chooseTeamMenu.activeSelf);
    }

    bool useYouthPlayers = false;
    public void useYouthPlayersButton()
    {
        useYouthPlayers = !useYouthPlayers;
    }

}
