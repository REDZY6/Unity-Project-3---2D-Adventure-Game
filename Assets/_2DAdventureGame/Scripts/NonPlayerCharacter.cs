using UnityEngine;

public class NonPlayerCharacter : MonoBehaviour
{
    public GameObject dialogueBubble;
    public string npcMessage = "Press C to shoot cogs and help me fix these broken robots! Come speak to me when you're finished!";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dialogueBubble.SetActive(false);
    }
}
