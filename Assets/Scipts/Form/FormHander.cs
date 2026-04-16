using Photon.Pun;
using Photon.Realtime;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class FormHander : MonoBehaviour
{
    [SerializeField] private EmailInput forgotEmailName; //panel quên mât khẩu

    [SerializeField] private EmailInput emailName;
    [SerializeField] private PasswordInput currentPassword;
    [SerializeField] private NameDisplayInput currentName;
    [SerializeField] private ConfirmInput confirmPass;

    private string playFabUserId;

    private void OnEnable()
    {
        emailName = GetComponentInChildren<EmailInput>();
        currentPassword = GetComponentInChildren<PasswordInput>();

        confirmPass = GetComponentInChildren<ConfirmInput>(true);// true là để lấy compoment khi chưa được bật
        UIManager.Instance.TitlleFormGame = StringManager.titlleLogin; ;

    }

    private void Start()
    {
        forgotEmailName = UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(1).GetComponentInChildren<EmailInput>(true);
    }

    public void Login()
    {
        string nameEmail = this.emailName.EmailName;
        string password = this.currentPassword.Password;

        //if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) || password.Length < 6)
        if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password))
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail;
            Debug.Log(UIManager.Instance.KeyNotificationTxt);
            StartCoroutine(DisplayNotiCouroutine(1));
            return;
        }

        var request = new LoginWithEmailAddressRequest
        {

            Email = nameEmail,
            Password = password
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLFailureAll);

    }

    public void Register()
    {
        string nameEmail = this.emailName.EmailName;
        string password = this.currentPassword.Password;
        string confirmPass = this.confirmPass.PasswordConfirm;


        // kiểm tra null, độ dài pass
        if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) || password.Length < 6 || password != confirmPass)
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail;
            StartCoroutine(DisplayNotiCouroutine(1));
            return;
        }

        var request = new RegisterPlayFabUserRequest
        {
            Email = nameEmail,
            Password = password,

            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLFailureAll); // thực hiên đăng kí và tải lên palyfabs
    }


    // action login thành công
    private void OnLoginSuccess(LoginResult result)
    {
        if (result != null)
        {


            playFabUserId = result.PlayFabId;
            PlayFabDataManager.Instance.LoadPlayerData();
            UIManager.Instance.KeyNotificationTxt = StringManager.notiLoginSuccess;

            StartCoroutine(DisplayNotiCouroutine(1));

            CheckHasNameDisplay();

        }
    }

    private void OnLFailureAll(PlayFabError error)
    {
        if (error.Error == PlayFabErrorCode.EmailAddressNotAvailable)
        {
            Debug.Log("Email đã tồn tại!");
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail;
        }
        else
        {
            Debug.Log("Lỗi khác: " + error.ErrorMessage);
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail;
        }
        StartCoroutine(DisplayNotiCouroutine(2));

    }

    void CheckHasNameDisplay() // hàm kiemer tra xem tài khoản có tên hiển thi trong game chưa ?
    {

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), result =>
        {
            // playFabUserId = result.PlayerProfile.PlayerId;
            string displayName = result.PlayerProfile.DisplayName;
            if (string.IsNullOrEmpty(displayName))
            {
                // Debug.Log("tên " + displayName);
                UIManager.Instance.uiFormCanvas.transform.GetChild(0).gameObject.SetActive(false); //ui form
                UIManager.Instance.uiFormCanvas.transform.GetChild(3).gameObject.SetActive(true); // ui displayname
                //  SubmitDisplayName(currentName.name);
                currentName = GetComponentInChildren<NameDisplayInput>();
            }
            else
            {

                Debug.Log("bạn đã tạo tên thanh công bạn có thể chuyến scnene");
                PlayFabDataManager.Instance.playerData.playerName = displayName;
                UIManager.Instance.DisplayNameUI = displayName;

                UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);
                Debug.Log(UIManager.Instance.DisplayNameUI);
            }

            PhotonManager.Instance.GetPhotonToken(playFabUserId, displayName);
        }, OnLFailureAll);


        Debug.Log("play user id " + playFabUserId);

    }

    // nhập tên lần khi vào
    public void SubmitDisplayName()
    {

        string nameDisplay = this.currentName.NameDisplay;
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameDisplay
        };

        Debug.Log("ten display la" + nameDisplay);
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplaySucccess, OnLFailureAll); // cập nahatj tên user
    }


    // sử lí hiển thị tên nên màn hình
    private void OnUpdateDisplaySucccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("tên của bạn hợp lệ");
        PlayFabDataManager.Instance.playerData.playerName = result.DisplayName;

        // Ẩn UI nhập tên và chuyển tiếp
        UIManager.Instance.DisplayNameUI = result.DisplayName;
        UIManager.Instance.uiFormCanvas.transform.GetChild(0).gameObject.SetActive(true); //ui form
        UIManager.Instance.uiFormCanvas.transform.GetChild(2).gameObject.SetActive(false); // ui displayname
                                                                                           //  SubmitDisplayName(currentName.name);
        UIManager.Instance.ChangeScene(UIManager.SceneType.ONLINEMAINMENU);

    }


    //action reigister thành công
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        if (result != null)
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notiRegisterSuccess;
            PlayFabDataManager.Instance.LoadPlayerData();

            UIManager.Instance.IsLogin = true; // dăng kí thành công trở về màn hình login
            UIManager.Instance.uiFormCanvas.transform.GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);

            StartCoroutine(DisplayNotiCouroutine(1));


            Debug.Log($"đăng kí thành công{result}");
        }
    }

    // quên mật khẩu
    public void ForgotPassword()
    {
        string nameEmail = this.forgotEmailName.EmailName;

        Debug.Log("hiên thi thực hiện" + nameEmail);
        if (string.IsNullOrEmpty(nameEmail))
        {
            return;
        }

        var request = new SendAccountRecoveryEmailRequest
        {
            Email = nameEmail,
            TitleId = PlayFabSettings.TitleId,
            //  EmailTemplateId = "20C0D1B4D718FCD4",
        };

        PlayFabClientAPI.SendAccountRecoveryEmail(request, OnRecoverySuccess, OnLFailureAll);
    }

    // hiển thị thông báo
    IEnumerator DisplayNotiCouroutine(int time)
    {
        Debug.Log("thực hiên notti");
        //  Debug.Log("thuc hien bat");
        // Debug.Log(UIManager.Instance.uiFormCanvas.transform.GetChild(1).name);
        UIManager.Instance.uiFormCanvas.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(time);
        // Debug.Log("thuwc hien tawt");
        UIManager.Instance.uiFormCanvas.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
    }


    //hàm thực hiện forgot mật khẩu
    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("thuc hien forgot thanh conog");
        UIManager.Instance.KeyNotificationTxt = StringManager.notiForgotSuccess;
    }



}
