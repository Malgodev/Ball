using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirebaseLoginManager : MonoBehaviour
{
    [Header("Register")]
    public InputField ipRegisterEmail;
    public InputField ipRegisterPassword;
    public InputField ipRegisterUsername; 
    public Button btnRegister;
    public Button btnToLogin;

    [Header("Sign in")]
    public InputField ipLoginEmail;
    public InputField ipLoginPassword;
    public Button btnLogin;
    public Button btnToRegister;

    private FirebaseAuth auth;
    private DatabaseReference dbReference;
    public GameObject registerForm;
    public GameObject loginForm;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbReference = FirebaseDatabase.DefaultInstance.GetReference("Users");

        btnRegister.onClick.AddListener(RegisterAccount);
        btnLogin.onClick.AddListener(LoginAccount);

        btnToRegister.onClick.AddListener(() =>
        {
            registerForm.SetActive(true);
            loginForm.SetActive(false);
        });

        btnToLogin.onClick.AddListener(() =>
        {
            registerForm.SetActive(false);
            loginForm.SetActive(true);
        });
    }

    private async void RegisterAccount()
    {
        string email = ipRegisterEmail.text;
        string password = ipRegisterPassword.text;
        string username = ipRegisterUsername.text; 

        string result = await CreateUser(email, password, username);
        Debug.Log(result);
    }

    private async void LoginAccount()
    {
        string email = ipLoginEmail.text;
        string password = ipLoginPassword.text;

        string result = await SignInUser(email, password);
        Debug.Log(result);
    }

    public async Task<string> CreateUser(string email, string password, string username)
    {
        try
        {
            AuthResult authResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            FirebaseUser newUser = authResult.User;

            var userData = new Dictionary<string, object>
            {
                { "userId", newUser.UserId },
                { "email", email },
                { "username", username }
            };

            await dbReference.Child(newUser.UserId).SetValueAsync(userData);

            return $"User created successfully with ID: {newUser.UserId}";
        }
        catch (FirebaseException e)
        {
            var errorCode = (AuthError)e.ErrorCode;
            string errorMessage;

            switch (errorCode)
            {
                case AuthError.EmailAlreadyInUse:
                    errorMessage = "The email is already in use. Please try another email.";
                    break;
                case AuthError.InvalidEmail:
                    errorMessage = "The email is invalid.";
                    break;
                case AuthError.WeakPassword:
                    errorMessage = "The password is too weak.";
                    break;
                default:
                    errorMessage = "Registration failed: " + e.Message;
                    break;
            }
            return errorMessage;
        }
    }

    public async Task<string> SignInUser(string email, string password)
    {
        try
        {
            AuthResult authResult = await auth.SignInWithEmailAndPasswordAsync(email, password);
            FirebaseUser user = authResult.User;

            DataSnapshot snapshot = await dbReference.Child(user.UserId).GetValueAsync();
            if (snapshot.Exists)
            {
                string username = snapshot.Child("username").Value.ToString();

                SceneManager.LoadScene("SampleScene");
                return $"Login successful! Welcome, {username}";
            }
            else
            {
                return "User data not found.";
            }
        }
        catch (FirebaseException e)
        {
            var errorCode = (AuthError)e.ErrorCode;
            string errorMessage;

            switch (errorCode)
            {
                case AuthError.InvalidEmail:
                    errorMessage = "The email is invalid.";
                    break;
                case AuthError.WrongPassword:
                    errorMessage = "The password is incorrect.";
                    break;
                case AuthError.UserNotFound:
                    errorMessage = "No user found with this email.";
                    break;
                default:
                    errorMessage = "Login failed: " + e.Message;
                    break;
            }
            return errorMessage;
        }
    }
}
