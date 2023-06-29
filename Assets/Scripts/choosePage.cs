using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class choosePage : MonoBehaviour
{
    public void OpenScene(int sceneid)
    {
        SceneManager.LoadScene(sceneid); 
    }
}
