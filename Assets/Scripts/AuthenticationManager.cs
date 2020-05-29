using System.Collections;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class AuthenticationManager : Singleton<AuthenticationManager>
{
    [SerializeField,Header("Ur title id")] private string titleId; //Ur title id

    private string userEmail;
    private string userPassword;
    private string userName;

    private GameManager m_GameManager;
    private GameManager GM => m_GameManager != null ?
    m_GameManager : m_GameManager = GameManager.Instance;

    private void Start()
    {
        //Check titleId and set it from code
        if (string.IsNullOrEmpty(PlayFabSettings.TitleId))
        {
            PlayFabSettings.TitleId = titleId;
        }

        SignIn();

    }

    public void SignIn()
    {
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            LoadData();
            LoginEmailReq();
        }
        else
        {
#if UNITY_ANDROID
            var requestAndroid = new LoginWithAndroidDeviceIDRequest { AndroidDeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithAndroidDeviceID(requestAndroid, OnLoginMobileSuccess, OnPlayFabError);
#elif UNITY_IOS
            var requestIOS = new LoginWithIOSDeviceIDRequest { DeviceId = ReturnMobileID(), CreateAccount = true };
            PlayFabClientAPI.LoginWithIOSDeviceID(requestIOS, OnLoginMobileSuccess, OnPlayFabError);
#else
            LoginEmailReq();
#endif
        }
    }

    private void LoginEmailReq()
    {
        var req = new LoginWithEmailAddressRequest { Email = userEmail, Password = userPassword };
        PlayFabClientAPI.LoginWithEmailAddress(req, OnLoginSuccess, OnPlayFabError);
    }

    private string ReturnMobileID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    private IEnumerator UpdatePlayerStats()
    {
        PlayFabStatsDataManager.Instance.GetPlayerScoreFromCloud();
        yield return new WaitForSecondsRealtime(3);
        Debug.Log("Login Success");
    }

    private void OnLoginSuccess(LoginResult loginResult)
    {
        GM.ShowLoadingText(true);
        StartCoroutine(UpdatePlayerStats()); //Sometimes we need ~1000ms for update

        //Account info request for get user name from server
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetUsernameResult, OnPlayFabError);

        SaveData();

    }

    private void OnGetUsernameResult(GetAccountInfoResult result)
    {
        userName = result.AccountInfo.Username;
        GM.OnSignInClick(userName);
    }

    private void OnLoginMobileSuccess(LoginResult result)
    {
        GM.ShowLoadingText(true);
        StartCoroutine(UpdatePlayerStats());
        //In this implementaion we usen't ios and google services,so without registration,we need user name.
        //It will be work fine without it,but name need for leaderboard info.
        var names = new string[] { "Tom", "Nick", "BigNick", "LittleNick", "MediumNick" };
        var userName = names[Random.Range(0, 5)];
        SaveData();
        Debug.Log($"Mobile Login Success,generated name: {userName}");
        GM.OnSignInClick(userName);

    }

    private void OnDisplayName(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log(result.DisplayName + " is your new display name");
    }

    private void OnPlayFabError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    public void SignUp()
    {
        var regRequest = new RegisterPlayFabUserRequest
        { Email = userEmail, Password = userPassword, Username = userName };
        PlayFabClientAPI.RegisterPlayFabUser(regRequest, OnRegistrationSuccess, OnPlayFabError);
    }

    private void OnRegistrationSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log($"{userEmail}, successfully registered");
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest
        { DisplayName = userName }, OnDisplayName, OnPlayFabError);
        SaveData();
        SignIn();
    }

    private void SaveData()
    {
        //FIXME: should be encrypted
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayerPrefs.SetString("USERNAME", userName);
    }

    private void LoadData()
    {
        userEmail = PlayerPrefs.GetString("EMAIL");
        userPassword = PlayerPrefs.GetString("PASSWORD");
        userName = PlayerPrefs.GetString("USERNAME");
    }

    private void DeleteData()
    {
        PlayerPrefs.DeleteKey("EMAIL");
        PlayerPrefs.DeleteKey("PASSWORD");
        PlayerPrefs.DeleteKey("USERNAME");
    }

    public void SetUserEmail(string email)
    {
        userEmail = email;
    }

    public void SetUserPass(string pass)
    {
        userPassword = pass;
    }

    public void SetUserName(string userName)
    {
        this.userName = userName;
    }

    public string GetUserName => userName;

    public void SignOut()
    {
        GM.ShowMainPanel();
        GM.ResetScore();
        DeleteData();
    }
}
