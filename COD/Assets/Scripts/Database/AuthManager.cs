using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class AuthManager : MonoBehaviour
{
    [HideInInspector] public DependencyStatus dependencyStatus;
    public static FirebaseAuth auth;
    public static FirebaseUser user;

    [Header("Login")]
    [SerializeField] private ModalControl loginModalControl;
    [SerializeField] private TMP_InputField loginEmailInput;
    [SerializeField] private TMP_InputField loginPasswordInput;

    [Header("Register")]
    [SerializeField] private ModalControl registerModalControl;
    [SerializeField] private TMP_InputField registerUsernameInput;
    [SerializeField] private TMP_InputField registerEmailInput;
    [SerializeField] private TMP_InputField registerPasswordInput;
    [SerializeField] private TMP_InputField registerPasswordVerifyInput;

    [Header("Sign Out")]
    [SerializeField] private GameObject signOutButton;

    public static AuthManager instance;

    public UnityEvent onLogin;
    public UnityEvent onSignout;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;

            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    private void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser == null)
        {
            MultithreadControl.RunOnMainThread(loginModalControl.Open);
        }
        else
        {
            user = auth.CurrentUser;
            MultithreadControl.RunOnMainThread(() => onLogin?.Invoke());
            MultithreadControl.RunOnMainThread(() => signOutButton.SetActive(true));
        }
    }

    public void Login()
    {
        string username = loginEmailInput.text;
        string password = loginPasswordInput.text;

        StartCoroutine(LoginCoroutine(username, password));
    }

    private IEnumerator LoginCoroutine(string email, string password)
    {
        Task<AuthResult> loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;
            HandleLoginError(authError);
            Debug.LogError($"Failed to login: {authError}");
        }
        else
        {
            ResetFields();
            user = loginTask.Result.User;
            loginModalControl.Close();
            MessageBoxControl.ShowOk("LOGGED IN", $"Successfully logged in as <b>{user.DisplayName}</b>.");
            MultithreadControl.RunOnMainThread(() => onLogin?.Invoke());
            Debug.Log($"Successfully logged in as <b>{user.DisplayName}</b>.");
            MultithreadControl.RunOnMainThread(() => signOutButton.SetActive(true));
        }
    }

    public void Register()
    {
        string username = registerUsernameInput.text;
        string email = registerEmailInput.text;
        string password = registerPasswordInput.text;
        string passwordVerify = registerPasswordVerifyInput.text;

        StartCoroutine(RegisterCoroutine(username, email, password, passwordVerify));
    }

    private IEnumerator RegisterCoroutine(string username, string email, string password, string passwordVerify)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            MessageBoxControl.ShowOk("REGISTER ERROR", "The username you entered appears to be empty.");
        }
        else if (password != passwordVerify)
        {
            MessageBoxControl.ShowOk("REGISTER ERROR", "The password and confirm password does not match.");
        }
        else
        {
            Task<AuthResult> registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

            yield return new WaitUntil(() => registerTask.IsCompleted);

            if (registerTask.Exception != null)
            {
                FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
                AuthError authError = (AuthError)firebaseException.ErrorCode;
                HandleRegisterError(authError);
                Debug.LogError($"Failed to register: {authError}");
            }
            else
            {
                ResetFields();
                user = registerTask.Result.User;

                yield return DatabaseManager.instance.InitializeNewUserData(user.UserId);
                yield return UpdateAuthUsername(username);
                yield return DatabaseManager.instance.UpdateDatabaseUsername(username);
                registerModalControl.Close();
                MessageBoxControl.ShowOk(
                    "REGISTRATION COMPLETE",
                    "You may now login your newly created account.",
                    () => loginModalControl.Open()
                );

                Debug.Log($"Successfully registered as <b>{user.DisplayName}</b>.");
            }
        }
    }

    public IEnumerator UpdateAuthUsername(string username)
    {
        UserProfile userProfile = new UserProfile()
        { DisplayName = username };

        Task profileTask = user.UpdateUserProfileAsync(userProfile);

        yield return new WaitUntil(() => profileTask.IsCompleted);

        if (profileTask.Exception != null)
        {
            FirebaseException firebaseException = profileTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;
            Debug.LogError($"Failed to register: {profileTask.Exception}");
        }
    }

    private void HandleLoginError(AuthError authError)
    {
        string title = "LOGIN ERROR";
        string message;

        switch (authError)
        {
            case AuthError.MissingEmail:
                message = "Email is missing.";
                break;

            case AuthError.MissingPassword:
                message = "Password is missing.";
                break;

            case AuthError.InvalidEmail:
                message = "Email is invalid.";
                break;

            case AuthError.WrongPassword:
                message = "Password is incorrect.";
                break;

            case AuthError.UserNotFound:
                message = "No data found from this account!";
                break;

            case AuthError.Failure:
                message = "Failed to login.";
                break;

            default:
                message = "Unknown error. Please try again!";
                break;
        }

        MessageBoxControl.ShowOk(title, message);
    }

    private void HandleRegisterError(AuthError authError)
    {
        string title = "REGISTER ERROR";
        string message;

        switch (authError)
        {
            case AuthError.MissingEmail:
                message = "Email is missing.";
                break;

            case AuthError.MissingPassword:
                message = "Password is missing.";
                break;

            case AuthError.InvalidEmail:
                message = "Email is invalid.";
                break;

            case AuthError.EmailAlreadyInUse:
                message = "Email is already used.";
                break;

            case AuthError.WeakPassword:
                message = "Password is too weak.";
                break;

            default:
                message = "Unknown error. Please try again!";
                break;
        }

        MessageBoxControl.ShowOk(title, message);
    }

    public static void SignOut()
    {
        MessageBoxControl.ShowYesNo("SIGN OUT", $"Sign out as <b>{user.DisplayName}</b>?", instance._SignOut);
    }

    private void _SignOut()
    {
        ResetFields();
        auth.SignOut();
        user = null;
        onSignout?.Invoke();
        loginModalControl.Open();
        signOutButton.SetActive(false);
    }

    private void ResetFields()
    {
        // Login
        loginEmailInput.text = "";
        loginPasswordInput.text = "";

        // Register
        registerUsernameInput.text = "";
        registerEmailInput.text = "";
        registerPasswordInput.text = "";
        registerPasswordVerifyInput.text = "";
    }
}
