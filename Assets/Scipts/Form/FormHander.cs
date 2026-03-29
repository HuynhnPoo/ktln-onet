using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FormHander : MonoBehaviour
{
    [SerializeField] private EmailInput emailName;
    [SerializeField] private PasswordInput currentPassword;
    [SerializeField] private NameDisplayInput currentName;
    [SerializeField] private ConfirmInput confirmPass;

   

    private void Awake()
    {

    }
    private void OnEnable()
    {
        emailName = GetComponentInChildren<EmailInput>();
        currentPassword = GetComponentInChildren<PasswordInput>();

        confirmPass = GetComponentInChildren<ConfirmInput>(true);
        //   Debug.Log(confirmPass);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    Debug.Log(emailName.EmailName + " " + currentPassword.Password);
        //}
    }

    public void Login()
    {
        string nameEmail = this.emailName.EmailName;
        string password = this.currentPassword.Password;

        //if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) || password.Length < 6)
        if (string.IsNullOrEmpty(nameEmail) || string.IsNullOrEmpty(password) )
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
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

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

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginFailure); // thực hiên đăng kí và tải lên palyfabs
    }


    // action login thành công
    private void OnLoginSuccess(LoginResult result)
    {
        if (result != null)
        {
            PlayFabDataManager.Instance.LoadPlayerData();
            UIManager.Instance.KeyNotificationTxt = StringManager.notiLoginSuccess;

            StartCoroutine(DisplayNotiCouroutine(1));
           
            CheckHasNameDisplay();

           // Debug.Log($"đăng nhập thành công{result}");
        }
    }

    void CheckHasNameDisplay()
    {

        PlayFabClientAPI.GetPlayerProfile(new GetPlayerProfileRequest(), result =>
        {
            string displayName=result.PlayerProfile.DisplayName;
            if (string.IsNullOrEmpty(displayName))
            {
               // Debug.Log("tên " + displayName);
                UIManager.Instance.uiFormCanvas.transform.GetChild(0).gameObject.SetActive(false);
                UIManager.Instance.uiFormCanvas.transform.GetChild(2).gameObject.SetActive(true);
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

        }, OnLoginFailure);

    }

    // nhập tên lần khi vào
    public void SubmitDisplayName()
    {

        Debug.Log(this.currentName.NameDisplay);
        string nameDisplay = this.currentName.NameDisplay;
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = nameDisplay
        };

        Debug.Log("ten display la" + nameDisplay);
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnUpdateDisplaySucccess, OnLoginFailure); // cập nahatj tên user
    }

    private void OnUpdateDisplaySucccess(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("tên của bạn hợp lệ");
        PlayFabDataManager.Instance.playerData.playerName = result.DisplayName;

        // Ẩn UI nhập tên và chuyển tiếp
        UIManager.Instance.DisplayNameUI = result.DisplayName;
    }


    //action reigister thành công
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        if (result != null)
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notiRegisterSuccess;
            PlayFabDataManager.Instance.LoadPlayerData();
            StartCoroutine(DisplayNotiCouroutine(1));


            Debug.Log($"đăng kí thành công{result}");
        }
    }

    // hàm  cho các action thất bại
    private void OnLoginFailure(PlayFabError error)
    {
        if (UIManager.Instance.IsLogin)
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail; // gắn string khi login fail
            StartCoroutine(DisplayNotiCouroutine(1));
            Debug.LogWarning("hien thi ra loi" + error.GenerateErrorReport());


        }
        else
        {
            UIManager.Instance.KeyNotificationTxt = StringManager.notifail;
            StartCoroutine(DisplayNotiCouroutine(1));
            Debug.LogWarning("hien thi ra loi" + error.GenerateErrorReport());

        }
    }

    // quên mật khẩu
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

   public  void DisplayNoti()
    {
      

    }

   IEnumerator DisplayNotiCouroutine(int time)
    {
        Debug.Log("thuc hien bat");
        UIManager.Instance.uiFormCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);

        yield return new WaitForSeconds(time);
        Debug.Log("thuwc hien tawt");
        UIManager.Instance.uiFormCanvas.transform.GetChild(1).GetChild(0).gameObject.SetActive(false);
    }

    private void OnRecoverySuccess(SendAccountRecoveryEmailResult result)
    {
        //statusText.text = "email khôi phục bạn hãy kiểm tra hòm thư " + result;
        //statusText.color = Color.yellow;
    }

}
