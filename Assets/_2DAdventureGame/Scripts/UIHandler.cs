using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
    public static UIHandler instance { get; private set; }
    public float displayTime = 4.0f;

    private VisualElement m_Healthbar;
    private VisualElement m_NonPlayerDialogue;
    private VisualElement m_WinScreen;
    private VisualElement m_LoseScreen;
    private Label m_DialogueLabel;
    private Label m_RobotCounter;
    private float m_TimerDisplay;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        // The root of the UI Hierarchy is accessed through rootVisualElement
        // Q function is the Query which used to find a particular Visual Element in bracket "HealthBar"
        m_Healthbar = uiDocument.rootVisualElement.Q<VisualElement>("HealthBar");
        SetHealthValue(1.0f);

        m_NonPlayerDialogue = uiDocument.rootVisualElement.Q<VisualElement>("NPCDialogue");
        m_DialogueLabel = m_NonPlayerDialogue.Q<Label>("DialogueLabel");
        m_NonPlayerDialogue.style.display = DisplayStyle.None;

        m_RobotCounter = uiDocument.rootVisualElement.Q<Label>("CounterLabel");

        m_WinScreen = uiDocument.rootVisualElement.Q<VisualElement>("WinScreenContainer");
        m_LoseScreen = uiDocument.rootVisualElement.Q<VisualElement>("LoseScreenContainer");
        // Sets countdown to -1, which is just below 0, and van be used as a default for not displaying the UI
        m_TimerDisplay = -1.0f;
    }

    private void Update()
    {
        if (m_TimerDisplay > 0)
        {
            m_TimerDisplay -= Time.deltaTime;
            if (m_TimerDisplay < 0)
            {
                m_NonPlayerDialogue.style.display = DisplayStyle.None;
            }
        }
    }

    public void SetHealthValue(float percentage)
    {
        m_Healthbar.style.width = Length.Percent(100 * percentage);
    }

    public void DisplayDialogue(string content)
    {
        m_DialogueLabel.text = content;
        m_NonPlayerDialogue.style.display = DisplayStyle.Flex;
        m_TimerDisplay = displayTime;
    }

    public void DisplayWinScreen()
    {
        m_WinScreen.style.opacity = 1.0f;
    }

    public void DisplayLoseScreen()
    {
        m_LoseScreen.style.opacity = 1.0f;
    }

    public void SetCounter(int current, int enemies)
    {
        m_RobotCounter.text = $"{current} / {enemies}";
    }
}
