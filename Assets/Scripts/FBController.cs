using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Database;
using Firebase.Unity;

public class FBController : MonoBehaviour
{
    [Header("UI Objects")]
    [SerializeField] InputField txtID;
    [SerializeField] InputField txtNome;
    [SerializeField] InputField txtEmail;

    [HideInInspector] public string recebeNome = string.Empty;
    [HideInInspector] public string recebeEmail = string.Empty;

    private bool isMostra = false;

    DatabaseReference mDatabaseRef;

    private void Start() {
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = 
            new Uri("https://cybercracia-elix-default-rtdb.firebaseio.com/");
        
        InicializarBD();
    }

    private void Update() {
        if(isMostra){
            MostraDados();
            isMostra = false;
        }
    }

    private void MostraDados(){
        txtNome.text = recebeNome;
        txtEmail.text = recebeEmail;
    }

    private void InicializarBD(){
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
        var dependencyStatus = task.Result;
        if (dependencyStatus == Firebase.DependencyStatus.Available) {
            //app = Firebase.FirebaseApp.DefaultInstance;

            mDatabaseRef = FirebaseDatabase.DefaultInstance.RootReference;
        } else {
            UnityEngine.Debug.LogError(System.String.Format(
            "Não foi possível resolver todas as dependências do Firebase: {0}", dependencyStatus));
            // Firebase Unity SDK is not safe to use here.
        }
        });
    }

    public void Gravar(){
        if(txtNome.text.Equals("") && txtEmail.text.Equals("")){
            Debug.Log("Digite o Nome do usuário e o email");
            return;
        }

        GravarDados(txtID.text.Trim(), txtNome.text.Trim(), txtEmail.text.ToLower().Trim());

    }

    private void GravarDados(string userId, string nome, string email){
        Usuario usuario = new Usuario(nome, email);
        string json = JsonUtility.ToJson(usuario);

        //Grava
        mDatabaseRef.Child("usuarios").Child(userId).SetRawJsonValueAsync(json);

        Debug.Log("Dados Gravados!");
    }

    public void Excluir(){
        ExcluirRegistro(txtID.text.Trim());
    }

    private void ExcluirRegistro(string userId){
        //Chamada para exclusão
        mDatabaseRef.Child("usuarios").Child(userId).RemoveValueAsync();
        txtID.text = "";
        txtNome.text = "";
        txtEmail.text = "";

        Debug.Log("Registro do ID " + userId + " excluido!");
    }

    public void ConsultaRegistro(){        
        txtNome.text = "";
        txtEmail.text = "";   
        MostraRegistro(txtID.text.Trim());     
    }

    private void MostraRegistro(string userId){
        
        FirebaseDatabase.DefaultInstance
        .GetReference("usuarios")
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...
                string json = snapshot.Child(userId.ToString()).GetRawJsonValue();

                Usuario extrairDados = JsonUtility.FromJson<Usuario>(json);

                recebeNome = extrairDados.username;
                recebeEmail = extrairDados.email;

                isMostra = true;

                Debug.Log("Nome: " + extrairDados.username);
            }
        });
    }

    public void MostraTodos(){
        
        FirebaseDatabase.DefaultInstance
        .GetReference("usuarios")
        .GetValueAsync().ContinueWith(task => {
            if (task.IsFaulted) {
                // Handle the error...
            }
            else if (task.IsCompleted) {
                DataSnapshot snapshot = task.Result;
                // Do something with snapshot...

                foreach (var filhos in snapshot.Children){

                    string f = filhos.GetRawJsonValue();

                    Usuario extrairDados = JsonUtility.FromJson<Usuario>(f);
                
                    Debug.Log("Nome do usuário: " + extrairDados.username);
                    Debug.Log("Email do usuário: " + extrairDados.email);
                }
            }
        });
    }
}

public class Usuario
{
    public string username;
    public string email;

    public Usuario(string username, string email){
        this.username = username;
        this.email = email;
    }
}