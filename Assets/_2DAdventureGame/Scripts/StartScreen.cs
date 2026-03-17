using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
public class StartScreen : MonoBehaviour
{
    private UIDocument m_UIDocument;

    private VisualElement m_DuckoSelector;

    private void OnEnable()
    {
        m_UIDocument = GetComponent<UIDocument>();

        m_DuckoSelector = m_UIDocument.rootVisualElement.Q<VisualElement>("DuckoSelector");

        m_DuckoSelector.AddManipulator(new Clickable(() => { SceneManager.LoadScene(1); }));
    }
}
