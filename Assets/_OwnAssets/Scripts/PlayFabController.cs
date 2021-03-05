using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.PfEditor.Json;
using UnityEngine;
using UnityEngine.UI;
using PlayFab.Json;
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
    public string playFabTitleID = "B2C52";

    [Header("LoginData")]
    [SerializeField] private string username;
    [SerializeField] private string userEmail;
    [SerializeField] private string userPassword;
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
    
    

    #region Login---------------
    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN WAS SUCCESSFUL!!");
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
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("REGISTRATION WAS SUCCESSFUL! Going to login");
        
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest{DisplayName = username}, OnDisplayName, OnDisplayErrorName)
        
        FinishLogin();
    }

    void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + " is your new displayname");
    }
    
    void OnDisplayNameError(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + " is your new displayname");
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
        FinishLogin();
    }
    
    private void FinishLogin()
    {
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
        
        GetStats();
    }
    
    #endregion Login---------------

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
        Debug.Log(PlayFab.PluginManager.GetPlugin<ISerializerPlugin>(PluginContract.PlayFab_Serializer));
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

    public void GetLeaderboarder()
    {
        var requestLeaderboard = new GetLeaderboardRequest
            {StartPosition = 0, StatisticName = "PlayerHighScore", MaxResultsCount = 20};
        PlayFabClientAPI.GetLeaderboard(requestLeaderboard, OnGetLeaderboard, OnErrorLeaderboard);
    }

    void OnGetLeaderboard(GetLeaderboardResult result)
    {
        Debug.Log(result.Leaderboard[0].StatValue);
        foreach (PlayerLeaderboardEntry player in result.Leaderboard)
        {
            Debug.Log(player.DisplayName + ": " + player.StatValue);
        }
    }

    void OnErrorLeaderboard(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }
    
    #endregion
}