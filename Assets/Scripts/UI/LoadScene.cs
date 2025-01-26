using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.EffectPlayStr("8");
        Invoke("loadScene", 3f);
    }
    public void loadScene()
    {
        SceneManager.LoadScene("Main Scene");
    }
}
