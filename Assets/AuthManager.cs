using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;


public class AuthManager : MonoBehaviour
{
    public InputField idInput;
    public InputField pwInput;

    string userID;
    string userPW;

    async void Start()
    {
        await UnityServices.InitializeAsync();
    }

    async Task SignUp(string id, string pw)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, pw);
            Debug.Log($"Sign up successful! Player ID : {AuthenticationService.Instance.PlayerId}");
        }
        catch(AuthenticationException ex)
        {
            Debug.LogError($"Sign up failed : {ex.Message}");
        }
    }

    public void OnClickSignUpBtn()
    {
        userID = idInput.text;
        userPW = pwInput.text;
        SignUp(userID, userPW);
    }

    async Task LogIn(string id, string pw)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(id, pw);
            Debug.Log($"Log in successful! Player ID : {AuthenticationService.Instance.PlayerId}");
        }
        catch (AuthenticationException ex)
        {
            Debug.LogError($"Log in failed : {ex.Message}");
        }
    }

    public void OnClickLogInBtn()
    {
        userID = idInput.text;
        userPW = pwInput.text;
        LogIn(userID, userPW);
    }
}
