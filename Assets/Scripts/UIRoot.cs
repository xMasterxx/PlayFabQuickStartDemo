using UnityEngine;
using UnityEngine.UI;

public class UIRoot : MonoBehaviour
{
   [SerializeField] private GameObject m_RegPanel;
   [SerializeField] private GameObject m_GamePanel;
   [SerializeField] private GameObject m_MainPanel;
   [SerializeField] private GameObject m_LogInPanel;

   [SerializeField,Space] private Text m_WelcomeText;
   [SerializeField] private Text m_GameScoreText;
   [SerializeField] private Text m_LoadingText;

    public GameObject RegPanel => m_RegPanel; 
    public GameObject MainPanel => m_MainPanel; 
    public GameObject GamePanel => m_GamePanel; 
    public GameObject LogInPanel => m_LogInPanel; 

    public Text WelcomeText => m_WelcomeText; 
    public Text GameScoreText => m_GameScoreText;
    public Text LoadingText => m_LoadingText;
}
