//Code by Vincent Kyne

using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
    public string sceneToLoad;
    public bool exitIsOpen = false;
    public bool isThisTutorial;

    public Item Key;

    public void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && exitIsOpen)
        {
            PlayerStats.getInstacne().inventory.Remove(Key);
            if (!isThisTutorial)
            {
                PlayerStats.getInstacne().raiseDifficulty();
            }
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    //The function the key uses in order to open the exit
    public static void openExit()
    {
        GameObject theExit = FindObjectOfType<ExitScript>().gameObject;
        theExit.GetComponent<ExitScript>().exitIsOpen = true;
        theExit.GetComponentInChildren<Light>().intensity = 0.5f;
        theExit.GetComponent<SpriteRenderer>().enabled = true;
    }
}
