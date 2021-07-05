using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Firebase;
using Firebase.Database;
using Firebase.Unity;
using Firebase.Auth;

public class FBConfiguration : MonoBehaviour
{
    private static FirebaseAuth autenticacao;

    public static FirebaseAuth getFirebaseAutenticacao(){
        if (autenticacao == null){
            autenticacao = FirebaseAuth.DefaultInstance;
        }
        return autenticacao;
    }
}
