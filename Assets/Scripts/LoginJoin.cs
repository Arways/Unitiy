using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginJoin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void login()
    {
        Debug.Log("login success");
        SceneManager.LoadScene("MainList");
    }

    public void goToJoin()
    {
        Debug.Log("go to join");
        SceneManager.LoadScene("Join");
    }

    public void goToLogin()
    {
        Debug.Log("go to login");
        SceneManager.LoadScene("Login");
    }
}
