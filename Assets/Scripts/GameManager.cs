using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private UIRoot uIRoot;
    private int score;

    private PlayFabStatsDataManager m_PFSDM;
    private PlayFabStatsDataManager PFSDM => m_PFSDM != null ?
    m_PFSDM : m_PFSDM = PlayFabStatsDataManager.Instance;
    public int Score => score;


    private void Start()
    {
        ShowMainPanel();
    }

    public void ShowSignUpPanel()
    {
        uIRoot.RegPanel.SetActive(true);
        uIRoot.GamePanel.SetActive(false);
        uIRoot.MainPanel.SetActive(false);
        uIRoot.LogInPanel.SetActive(false);
    }

    public void ShowLogInPanel()
    {
        uIRoot.RegPanel.SetActive(false);
        uIRoot.GamePanel.SetActive(false);
        uIRoot.MainPanel.SetActive(false);
        uIRoot.LogInPanel.SetActive(true);
    }

    public void ShowLoadingText(bool active)
    {
        uIRoot.LoadingText.gameObject.SetActive(active);
    }

    public void ShowGamePanel()
    {
        uIRoot.RegPanel.SetActive(false);
        uIRoot.GamePanel.SetActive(true);
        uIRoot.MainPanel.SetActive(false);
        uIRoot.LogInPanel.SetActive(false);
    }

    public void ShowMainPanel()
    {
        uIRoot.LogInPanel.SetActive(false);
        uIRoot.MainPanel.SetActive(true);
        uIRoot.GamePanel.SetActive(false);
        uIRoot.RegPanel.SetActive(false);
        ShowLoadingText(false);
    }

    public void OnSignInClick(string userName)
    {
        ShowGamePanel();
        uIRoot.WelcomeText.gameObject.SetActive(!string.IsNullOrWhiteSpace(userName));
        if (uIRoot.WelcomeText.gameObject.activeSelf)
        {
            uIRoot.WelcomeText.text = $"Welcome, {char.ToUpper(userName[0])}{userName.Substring(1)}!";
        }
        ShowLoadingText(false);
    }

    public void SetScore(int value)
    {
        score = value;
        UpdateScore();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScore();
    }

    public void OnScoreButtonClick()
    {
        ++score;
        UpdateScore();

    }

    public void OnSaveScoreClick()
    {
        PFSDM.SetAndUpdateStats(score);
    }

    private void UpdateScore()
    {
        uIRoot.GameScoreText.text = score.ToString();
    }
}
