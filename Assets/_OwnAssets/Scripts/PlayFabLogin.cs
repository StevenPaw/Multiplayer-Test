using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField]
    private string username;
    private string userEmail;
    private string userPassword;
    private bool rememberMeActive = true;
    public GameObject loginPanel;
    public GameObject loggingInStatusView;
    public string playFabTitleID = "B2C52";
    
    public void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            PlayFabSettings.staticSettings.TitleId = playFabTitleID;
        }
        
        PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey("EMAIL") && PlayerPrefs.HasKey("PASSWORD"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            loginPanel.SetActive(false);
            loggingInStatusView.SetActive(true);
            var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess,OnLoginFailure);
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("LOGIN WAS SUCCESSFUL!!");
        FinishLogin();
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("REGISTRATION WAS SUCCESSFUL! Going to login");
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
        loggingInStatusView.SetActive(false);
    }
    
    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest {Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
        loggingInStatusView.SetActive(false);
        loginPanel.SetActive(true);
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
        loggingInStatusView.SetActive(false);
        loginPanel.SetActive(true);
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
        loggingInStatusView.SetActive(true);
        var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess,OnLoginFailure);
    }
}
