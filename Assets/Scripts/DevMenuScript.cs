using UnityEngine;

public class DevMenuScript : MonoBehaviour
{
    public GameObject background;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyUp(KeyCode.Delete))
        {
            Time.timeScale = background.activeSelf ? 1 : 0;
            background.SetActive(!background.activeSelf);
        }
#endif
    }
}
