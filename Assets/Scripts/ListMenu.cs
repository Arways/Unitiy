using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ListMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public void Broadcast1()
    {
        Debug.Log("broadcasting1");
        SceneManager.LoadScene("BroadcastAR");
    }

    public void goBack()
    {
        Debug.Log("go gack to list");
        SceneManager.LoadScene("MainList");
    }
}
