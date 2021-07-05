using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MsgBox : MonoBehaviour
{
    public GameObject msgPanel;
    public TMP_Text rMessage;
    public float timeMess = 3.0f;

    public void ShowMsg(string message){
        StartCoroutine(ShowMessage(message, timeMess));
    }

    IEnumerator ShowMessage(string m, float t){
        msgPanel.SetActive(true);
        rMessage.text = m;
        yield return new WaitForSeconds(t);
        msgPanel.SetActive(false);
    }
}
