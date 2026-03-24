using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayfabsManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Login(string nameEmail, string password)
    {
        if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) || password.Length < 6) return;
        var request = new LoginWithEmailAddressRequest
        {
            Email = nameEmail,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }

    public void Register(string nameEmail, string password)
    {
        if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) || password.Length < 6) return;

        var request = new RegisterPlayFabUserRequest {
            Email = nameEmail,
            Password = password,

            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginFailure);
    }

  

    private void OnLoginSuccess(LoginResult result)
    {

        if (result != null)
        {
            //statusText.text = "login thành công ";
            //statusText.color = Color.green;
            Debug.Log($"đăng nhập thành công{result}");
        }
    }

    private void OnLoginFailure(PlayFabError error)
    {
        //statusText.text = "thất bại ";
        //statusText.color = Color.red;
        Debug.Log("hien thi ra loi" + error.GenerateErrorReport());
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        if (result != null)
        {
            //statusText.text = "regietr thành công ";
            //statusText.color = Color.yellow;
            Debug.Log($"đăng kí thành công{result}");
        }
    }


   public void ForgotPassword(string nameEmail)
    {
        if (string.IsNullOrEmpty(nameEmail))
        {
            return;
        }

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = nameEmail,
            TitleId = PlayFabSettings.staticSettings.TitleId
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnLoginFailure);
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        //statusText.text = "email khôi phục bạn hãy kiểm tra hòm thư " + result;
        //statusText.color = Color.yellow;
    }

}
