using System;
using System.Collections;
using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using JsonObject = PlayFab.Json.JsonObject;

public class PlayFabController : MonoBehaviour
{
    public static PlayFabController PFC;
    
    [Header("GameObjects & ID")]
    public GameObject loginPanel;
    public GameObject socialLoginPanel;
    public GameObject loggingInStatusView;
    public GameObject addLoginPanel;
    public GameObject recoverButton;
    public GameObject loggedInViewPanel;
    public TMP_Text welcomeUsernameText;
    public string playFabTitleID = "B2C52";

    [Header("LoginData")]
    [SerializeField] private string username;
    [SerializeField] private string displayname;
    [SerializeField] private string userEmail;
    [SerializeField] private string userPassword;
    [SerializeField] private string myID;
    [SerializeField] private bool rememberMeActive = true;
    
    private void OnEnable()
    {
        if (PlayFabController.PFC == null)
        {
            PlayFabController.PFC = this;
        }
        else
        {
            if (PlayFabController.PFC != this)
            {
                Destroy(this.gameObject);
            }
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void Start()
    {
        //Set PlayFab TitleID if not set
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
        {
            PlayFabSettings.staticSettings.TitleId = playFabTitleID;
        }

        
        
        #region Auto-Login---------------
        //Login if there is any login data
        //Uncomment to delete Login-Settings
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            loginPanel.SetActive(false);
            socialLoginPanel.SetActive(false);
            loggingInStatusView.SetActive(true);
            var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
#if UNITY_ANDROID
            loginPanel.SetActive(false);
            socialLoginPanel.SetActive(false);
            loggingInStatusView.SetActive(true);
            var requestAndroid = new LoginWithAndroidDeviceIDRequest {AndroidDeviceId = ReturnMobileID(), CreateAccount = true};
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
#if UNITY_IOS
            loginPanel.SetActive(false);
            socialLoginPanel.SetActive(false);
            loggingInStatusView.SetActive(true);
            var requestIOS = new LoginWithIOSDeviceIDRequest {DeviceId = ReturnMobileID(), CreateAccount = true};
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnLoginMobileFailure);
#endif
            
        }
        #endregion AutoLogin---------------
    }
    
    

    #region Login/Logout---------------

    public void Logout()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("LoginScreen"))
        {
            PlayFabClientAPI.ForgetAllCredentials();
            loggedInViewPanel.SetActive(false);
            loginPanel.SetActive(true);
            socialLoginPanel.SetActive(true);
            loggingInStatusView.SetActive(false);
            PlayerPrefs.DeleteKey("EMAIL");
            PlayerPrefs.DeleteKey("USERNAME");
            PlayerPrefs.DeleteKey("PASSWORD");
            Debug.Log("PLAYER HAS BEEN LOGGED OUT");
            welcomeUsernameText.text = "...";
        }
    }
    
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN WAS SUCCESSFUL!!");
        myID = result.PlayFabId;
        GetPlayerData();
        FinishLogin();
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        Debug.Log("LOGIN WAS SUCCESSFUL!!");
        loginPanel.SetActive(false);
        socialLoginPanel.SetActive(false);
        loggingInStatusView.SetActive(false);
        if (!PlayerPrefs.HasKey("EMAIL"))
        {
            recoverButton.SetActive(true);
        }
        myID = result.PlayFabId;
        GetPlayerData();
        FinishLogin();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("REGISTRATION WAS SUCCESSFUL! Going to login");

        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {DisplayName = username},
            OnDisplayName, OnDisplayNameError);
        myID = result.PlayFabId;
        GetPlayerData();
        FinishLogin();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + " is your displayname");
        updateDisplayName();
    }
    
    void OnDisplayNameError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest
            {Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        loggingInStatusView.SetActive(false);
        socialLoginPanel.SetActive(true);
        loginPanel.SetActive(true);
    }

    private void OnLoginMobileFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        loggingInStatusView.SetActive(false);
        socialLoginPanel.SetActive(true);
        loginPanel.SetActive(true);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        loggingInStatusView.SetActive(false);
        loginPanel.SetActive(true);
        socialLoginPanel.SetActive(true);
    }

    public void SetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void SetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void SetUsername(string usernameIn)
    {
        username = usernameIn;
    }

    public void SwitchRememberMe()
    {
        rememberMeActive = !rememberMeActive;
    }

    public void OnClickLogin()
    {
        loginPanel.SetActive(false);
        socialLoginPanel.SetActive(false);
        loggingInStatusView.SetActive(true);
        var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    public static string ReturnMobileID()
    {
        string deviceID = SystemInfo.deviceUniqueIdentifier;
        return deviceID;
    }

    public void OpenAddLogin()
    {
        addLoginPanel.SetActive(true);
    }

    public void OnClickAddLogin()
    {
        var addLoginRequest = new AddUsernamePasswordRequest
            {Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.AddUsernamePassword(addLoginRequest, OnAddLoginSuccess, OnRegisterFailure);
        loggingInStatusView.SetActive(false);
        loginPanel.SetActive(true);
        socialLoginPanel.SetActive(true);
    }

    private void OnAddLoginSuccess(AddUsernamePasswordResult result)
    {
        Debug.Log("REGISTRATION WAS SUCCESSFUL! Going to login");
        addLoginPanel.SetActive(false);
        
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {DisplayName = username},
            OnDisplayName, OnDisplayNameError);
        GetPlayerData();
        FinishLogin();
    }
    
    private void FinishLogin()
    {
        updateDisplayName();
        
        if (rememberMeActive)
        {
            PlayerPrefs.SetString("EMAIL", userEmail);
            PlayerPrefs.SetString("PASSWORD", userPassword);
            PlayerPrefs.SetString("USERNAME", username);
        }
        else
        {
            PlayerPrefs.DeleteKey("EMAIL");
            PlayerPrefs.DeleteKey("PASSWORD");
            PlayerPrefs.DeleteKey("USERNAME");
        }

        loginPanel.SetActive(false);
        socialLoginPanel.SetActive(false);
        loggingInStatusView.SetActive(false);
        recoverButton.SetActive(false);
        loggedInViewPanel.SetActive(true);

        GetStats();
    }
    
    #endregion Login/Logout---------------

    #region PlayerStats---------------
    
    [Header("Statistics")]
    public int playerLevel;
    public int gameLevel;
    
    public int playerHealth;
    public int playerDamage;

    public int playerHighScore;
    
    public void SetStats()
    {
        PlayFabClientAPI.UpdatePlayerStatistics(new UpdatePlayerStatisticsRequest
        {
            // request.Statistics is a list, so multiple StatisticUpdate objects can be defines if required.
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate {StatisticName = "PlayerLevel", Value = playerLevel},
                new StatisticUpdate {StatisticName = "GameLevel", Value = gameLevel},
                new StatisticUpdate {StatisticName = "PlayerHealth", Value = playerHealth},
                new StatisticUpdate {StatisticName = "PlayerDamage", Value = playerDamage},
                new StatisticUpdate {StatisticName = "PlayerHighScore", Value = playerHighScore},
            }
        },
            result => {Debug.Log("User statistic updated");},
            error => {Debug.LogError(error.GenerateErrorReport()); });
    }

    void GetStats()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            OnGetStats,
            error => Debug.LogError(error.GenerateErrorReport())
            );
    }

    void OnGetStats(GetPlayerStatisticsResult result)
    {
        Debug.Log("Received the following Stats:");
        foreach (var eachStat in result.Statistics)
        {
            Debug.Log("Stat (" + eachStat.StatisticName + "): " + eachStat.Value);
            switch (eachStat.StatisticName)
            {
                case "PlayerLevel":
                    playerLevel = eachStat.Value;
                    break;
                case "GameLevel":
                    gameLevel = eachStat.Value;
                    break;
                case "PlayerHealth":
                    playerHealth = eachStat.Value;
                    break;
                case "PlayerDamage":
                    playerDamage = eachStat.Value;
                    break;
                case "PlayerHighScore":
                    playerHighScore = eachStat.Value;
                    break;
            }
        }
    }

    public void StartCloudUpdatePlayerStats()
    {
        PlayFabClientAPI.ExecuteCloudScript(new ExecuteCloudScriptRequest()
        {
            FunctionName = "UpdatePlayerStats",
            FunctionParameter = new
            {
                Level = playerLevel,
                HighScore = playerHighScore,
                Health = playerHealth,
                Damage = playerDamage,
                GLevel = gameLevel,
            },
            GeneratePlayStreamEvent = true,
        }, OnCloudUpdateStats, OnErrorShared);
    }
    
    private void OnCloudUpdateStats(ExecuteCloudScriptResult result) {
        // CloudScript returns arbitrary results, so you have to evaluate them one step and one parameter at a time
        PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer);
        JsonObject jsonResult = (JsonObject)result.FunctionResult;
        object messageValue;
        jsonResult.TryGetValue("messageValue", out messageValue); // note how "messageValue" directly corresponds to the JSON values set in CloudScript
        Debug.Log((string)messageValue);
    }

    private void OnErrorShared(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    #endregion PlayerStats---------------

    #region Leaderboard---------------
    
    [Header("Leaderboard")]
    public GameObject leaderBoardPanel;
    public GameObject listingPrefab;
    public Transform listingContainer;
    public void GetLeaderboarder()
    {
        var requestLeaderboard = new GetLeaderboardRequest
        {
            StartPosition = 0,
            StatisticName = "PlayerHighScore",
            MaxResultsCount = 20,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowStatistics = true,
                ShowDisplayName = true,
                ShowLastLogin = true,
                ShowAvatarUrl = true
            }
        };
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        leaderBoardPanel.SetActive(true);
        //Debug.Log("HIGHEST SCORE: " + result.Leaderboard[0].StatValue);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            GameObject tempListing = Instantiate(listingPrefab, listingContainer);
            LeaderboardListing ll = tempListing.GetComponent<LeaderboardListing>();
            ll.playerNameText.text = player.DisplayName;
            ll.playerScoreText.text = player.StatValue.ToString();
            string avatarUrl = player.Profile.AvatarUrl;

            ll.playerProfilePicture.sprite = placeHolderSprite;
            
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                StartCoroutine(DownloadImage(avatarUrl, ll));
            }
            else
            {
                avatarUrl = "https://www.gravatar.com/avatar/" + player.PlayFabId.GetHashCode() + "?d=retro"; //Uses PlayFabID to generate Retro Image on Gravatar
                StartCoroutine(DownloadImage(avatarUrl, ll));
            }
        }
    }

    public void CloseLeaderboardPanel()
    {
        leaderBoardPanel.SetActive(false);
        for (int i = listingContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(listingContainer.GetChild(i).gameObject);
        }
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
    
    #endregion
    
    #region PlayerData---------------

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = myID,
            Keys = null
        }, GetUserDataSuccess, OnErrorLeaderboard);
    }

    void GetUserDataSuccess(GetUserDataResult result)
    {
        if (result.Data == null || !result.Data.ContainsKey("Skins"))
        {
            Debug.Log("Skins not set");
        }
        else
        {
            PersistentData.PD.SkinsStringToData(result.Data["Skins"].Value);
        }
    }

    public void SetUserData(string SkinsData)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
            {
                {"Skins", SkinsData}
            }
        }, SetDataSuccess, OnErrorLeaderboard);
    }

    void SetDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log(result.DataVersion);
    }
    
    #endregion PlayerData---------------
    
    #region Tools---------------

    public void updateDisplayName()
    {
        var usernameRequest = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(usernameRequest,OnDisplaynameReceived,DisplayPlayFabError);
    }

    void OnDisplaynameReceived(GetAccountInfoResult result)
    {
        displayname = result.AccountInfo.TitleInfo.DisplayName;
        welcomeUsernameText.text = displayname;
        Debug.Log("Displayname is " + displayname);
    }

    void DisplayPlayFabError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    
    IEnumerator DownloadImage(string MediaUrl, FriendListing tempListing)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            tempListing.playerProfilePicture.sprite = placeHolderSprite;
        }
        else
        {
            Texture2D profilePicture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(profilePicture, new Rect(0, 0, profilePicture.width, profilePicture.height), new Vector2(profilePicture.width / 2, profilePicture.height / 2));
            tempListing.playerProfilePicture.sprite = sprite;
        }
    } 
    
    IEnumerator DownloadImage(string MediaUrl, LeaderboardListing tempListing)
    {   
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.Log(request.error);
            tempListing.playerProfilePicture.sprite = placeHolderSprite;
        }
        else
        {
            Texture2D profilePicture = ((DownloadHandlerTexture) request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(profilePicture, new Rect(0, 0, profilePicture.width, profilePicture.height), new Vector2(profilePicture.width / 2, profilePicture.height / 2));
            tempListing.playerProfilePicture.sprite = sprite;
        }
    } 
    
    #endregion Tools---------------
    
    #region Friends---------------

    
    [Header("Friends")]
    public GameObject friendPrefab;
    [SerializeField] private Transform friendListingContainer = null;
    [SerializeField] private Sprite placeHolderSprite = null;
    [SerializeField] private GameObject friendPanel = null; 
    [SerializeField] private List<FriendInfo> myFriends = null;

    
    enum FriendIdType
    {
        PlayFabId,
        Username,
        Email,
        DisplayName
    };
    
    void DisplayFriends(List<FriendInfo> friendsCache)
    {
        //Debug.Log("Friends");
        foreach (FriendInfo f in friendsCache)
        {
            bool isFound = false;
            if (myFriends != null)
            {
                foreach (FriendInfo g in myFriends)
                {
                    if (f.FriendPlayFabId == g.FriendPlayFabId)
                    {
                        isFound = true;
                    }
                }
            }

            if (!isFound)
            {
                GameObject listing = Instantiate(friendPrefab, friendListingContainer);
                FriendListing tempListing = listing.GetComponent<FriendListing>();
                tempListing.playerNameText.text = f.TitleDisplayName;
                PlayerProfileModel friend = f.Profile;
                tempListing.playerScoreText.text = "Last online: " + friend.LastLogin.Value;
                String avatarUrl = friend.AvatarUrl;

                if (!string.IsNullOrEmpty(avatarUrl))
                {
                    StartCoroutine(DownloadImage(avatarUrl, tempListing));
                }
                else
                {
                    avatarUrl = "https://www.gravatar.com/avatar/" + f.FriendPlayFabId.GetHashCode() + "?d=retro"; //Uses PlayFabID to generate Retro Image on Gravatar
                    StartCoroutine(DownloadImage(avatarUrl, tempListing));
                }
            }
        }

        myFriends = friendsCache;
    }

    IEnumerator WaitForFriend()
    {
        yield return new WaitForSeconds(2);
        GetFriends();
    }

    public void RunWaitFunction()
    {
        StartCoroutine(WaitForFriend());
    }

    private List<FriendInfo> _friends = null;

    public void GetFriends()
    {
        PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
        {
            IncludeSteamFriends = false,
            IncludeFacebookFriends = false,
            ProfileConstraints = new PlayerProfileViewConstraints()
            {
                ShowDisplayName = true,
                ShowLastLogin = true,
                ShowAvatarUrl = true
            }
        }, result =>
        {
            _friends = result.Friends;
            DisplayFriends(_friends); //triggers your UI
        }, DisplayPlayFabError);
    }

    void AddFriend(FriendIdType idType, string friendId)
    {
        var request = new AddFriendRequest();
        switch (idType)
        {
            case FriendIdType.PlayFabId:
                request.FriendPlayFabId = friendId;
                break;
            case FriendIdType.Username:
                request.FriendUsername = friendId;
                break;
            case FriendIdType.Email:
                request.FriendEmail = friendId;
                break;
            case FriendIdType.DisplayName:
                request.FriendTitleDisplayName = friendId;
                break;
        }
        PlayFabClientAPI.AddFriend(request, result =>
        {
            Debug.Log("Friend added successfully!");
        }, DisplayPlayFabError);
    }
    
    private string friendSearch;

    public void InputFriendID(string idIn)
    {
        friendSearch = idIn;
    }

    public void SubmitFriendRequest()
    {
        AddFriend(FriendIdType.PlayFabId, friendSearch);
    }

    public void OpenCloseFriends()
    {
        friendPanel.SetActive(!friendPanel.activeInHierarchy);
        for (int i = friendListingContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(friendListingContainer.GetChild(i).gameObject);
        }

        if (myFriends != null)
        {
            myFriends.Clear();
        }
    }

    
    #endregion Friends---------------
}