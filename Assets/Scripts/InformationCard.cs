using NUnit.Framework;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class InformationCard : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject _infoCard;

    public TMP_Text _jerseyNumber;
    public TMP_Text _fullName;
    public TMP_Text _nameAndNo;

    public GameObject _isInjured; //bool
    public TMP_Text _injury;

    public GameObject _isEligibleForAll; //bool
    public TMP_Text _eligiblity;
    public TMP_Text _reason;

    public TMP_Text _position;

    public TMP_Text _birthDate;

    public TMP_Text _age;

    public TMP_Text _nationality;

    public TMP_Text _height;

    public TMP_Text _foot;

    public TMP_Text _joinDate;
    public TMP_Text _joinFrom;

    public TMP_Text _joinFee;
    public TMP_Text _contract;

    public TMP_Text _contractBeginEnd;

    public TMP_Text _joinInfo;

    public TMP_Text _marketValue;

    public RawImage _image;



    static public GameObject infoCard;
    
    static public TMP_Text jerseyNumber;
    static public TMP_Text fullName;
    static public TMP_Text nameAndNo;

    static public GameObject isInjured; //bool
    static public TMP_Text injury;

    static public GameObject isEligibleForAll; //bool
    static public TMP_Text eligiblity;
    static public TMP_Text reason;

    static public TMP_Text position;

    static public TMP_Text birthDate;

    static public TMP_Text age;

    static public TMP_Text nationality;

    static public TMP_Text height;

    static public TMP_Text foot;

    static public TMP_Text joinDate;
    static public TMP_Text joinFrom;

    static public TMP_Text joinFee;
    static public TMP_Text contract;

    static public TMP_Text contractBeginEnd;


    static public TMP_Text joinInfo;

    static public TMP_Text marketValue;

    static public RawImage image;

    void Start()
    {
        
    }

    void Awake()
    {
        infoCard = _infoCard;
        jerseyNumber = _jerseyNumber;
        fullName = _fullName;
        nameAndNo = _nameAndNo;
        injury = _injury;
        eligiblity = _eligiblity;
        reason = _reason;
        position = _position;
        birthDate = _birthDate; 
        age = _age; 
        nationality = _nationality;
        height = _height;
        foot = _foot;
        joinDate = _joinDate;
        joinFrom = _joinFrom;
        joinFee = _joinFee;
        contract = _contract;
        contractBeginEnd = _contractBeginEnd;
        joinInfo = _joinInfo;
        marketValue = _marketValue;
        image = _image;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void activateInfoCard(Scraper.Player player)
    {
        fullName.text = player.fullName;
        nameAndNo.text = player.lastName + "#" + player.jerseyNumber;
        injury.text = player.injury == string.Empty ? "None" : player.injury;
        eligiblity.text = player.eligibility == string.Empty ? "None" : player.eligibility;
        reason.text = player.reason;
        position.text = player.position;
        birthDate.text = player.birthDate;
        age.text = player.age;
        nationality.text = string.Join("&", player.nationalities);
        height.text = player.height;
        foot.text = player.foot;
        contractBeginEnd.text = player.joinDate + "-" + player.contract;
        joinInfo.text = "Joined from " + player.joinedFrom + " for ";
        joinInfo.text += player.joinedFee == "-" ? "free" : player.joinedFee;
        marketValue.text = player.marketValue;
        image.texture = player.img.texture;
        infoCard.SetActive(true);
    }

    public void closeInformationCard()
    {
        infoCard.SetActive(false);
    }
}
