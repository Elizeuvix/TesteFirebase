using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using Firebase;
using Firebase.Database;
using Firebase.Unity;
using Firebase.Auth;

public class FBLogin : MonoBehaviour
{
    [Header("Login UI")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] TMP_InputField lEmail;
    [SerializeField] TMP_InputField lPassword;
    [Header("Register UI")]
    [SerializeField] GameObject regPanel;
    [SerializeField] TMP_InputField rUsername;
    [SerializeField] TMP_InputField rEmail;
    [SerializeField] TMP_InputField rPassword; 
    [SerializeField] TMP_Text rMessage;
    [Header("Message UI")]
    [SerializeField] GameObject msgPanel;
    [SerializeField] TMP_Text eMessage;

    private string username, email, password, errorMessage; 
    
    private string[] registerMessage = new string[4]{
        "Você digitou alguma coisa errada!",
        "Email inválido! Digite o email corretamente.",
        "Este email já está em uso! Digite outro email.",
        "Senha inválida! Digite uma senha mais forte."
    };
    private int errorCode = 0;

    private FirebaseAuth autenticacao;
    private FBUser usuario;
    private ShowExceptionMessage showExceptionMessage;    
    private string userID;
    private bool falhou = false;
    MsgBox msgBox;

    private void Start() {
        //Define o banco de dados
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = 
            new Uri("https://cybercracia-elix-default-rtdb.firebaseio.com/");    

        msgBox = GetComponent<MsgBox>(); 
    }

    public void BtnLogin(){

    }

    public void BtnRegister(){

        username = rUsername.text;
        email = rEmail.text;
        password = rPassword.text;
        rMessage.text = string.Empty;        

        if(username != string.Empty && username.Length > 5){
            if(email != string.Empty && email.Length > 5){
                if (password != string.Empty && password.Length > 5){
                    //Tudo preenchido!!!
                    usuario = new FBUser();
                    usuario.SetNome(username);
                    usuario.SetEmail(email);
                    usuario.SetSenha(password);

                    CadastrarUsuario();
                    LerMessage();
                }else{
                    msgBox.ShowMsg("O campo Password deve conter 6 ou mais caracteres");
                    return;
                }
            }else{
                msgBox.ShowMsg("O campo Email deve conter 6 ou mais caracteres");
                return;
            }
        }else{
            msgBox.ShowMsg("O campo Username deve conter 6 ou mais caracteres");
            return;
        }       
    }

    private void CadastrarUsuario(){

        autenticacao = FBConfiguration.getFirebaseAutenticacao();

        autenticacao.CreateUserWithEmailAndPasswordAsync(
            usuario.GetEmail(), usuario.GetSenha()
        ).ContinueWith(task => {
        if (task.IsCanceled) {
            Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
            return;
        }
        if (task.IsFaulted) {

            showExceptionMessage = new ShowExceptionMessage();

            errorMessage = task.Exception.ToString();
            
            string errorString = errorMessage;

            if(errorString.Contains("The email address is badly formatted")){
                errorCode =1;
            }else if(errorString.Contains("The email address is already in use by another account")){
                errorCode =2;
            }else if(errorString.Contains("The given password is invalid")){
                errorCode =3;
            }else{
                errorCode =0;
            }  

            string[] messError = new string[4]{
                "Você digitou alguma coisa errada!",
                "Email inválido! Digite o email corretamente.",
                "Este email já está em uso! Digite outro email.",
                "Senha inválida! Digite uma senha mais forte."
            };
            showExceptionMessage.SetExceptionMessage(messError[errorCode]);
            Debug.Log("ERROR: " + showExceptionMessage.GetExceptionMessage()); //Este funciona
            
            return;
        }

        // Firebase user has been created.
        FirebaseUser newUser = task.Result;
        falhou = false;
        Debug.LogFormat("Usuario do Firebase criado com sucesso: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
            userID = newUser.UserId; 
            showExceptionMessage.SetExceptionMessage("Usuario do Firebase criado com sucesso");

            Debug.Log("ERROR232: " + showExceptionMessage.GetExceptionMessage()); //Este funciona
        });
        
    }

    private void LerMessage(){
        ShowExceptionMessage sEM;
        sEM = new ShowExceptionMessage();
        msgBox.ShowMsg(sEM.GetExceptionMessage());

    }

    public class FBUser{
        private string nome;
        private string email;
        private string senha;

        public FBUser(){

        }

        public string GetNome(){
            return nome;
        }

        public void SetNome(string nome){
            this.nome = nome;
        }

        public string GetEmail(){
            return email;
        }

        public void SetEmail(string email){
            this.email = email;
        }

        public string GetSenha(){
            return senha;
        }

        public void SetSenha(string senha){
            this.senha = senha;
        }
    }

    public class ShowExceptionMessage{
        private string exceptionMessage;

        public ShowExceptionMessage(){

        }

        public string GetExceptionMessage(){
            return exceptionMessage;
        }

        public void SetExceptionMessage(string exceptionMessage){
            this.exceptionMessage = exceptionMessage;
        }
    }
}