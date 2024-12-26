using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRubricPanel : MonoBehaviour
{
    public int panelID = 0;

    [SerializeField]
    public TMP_Text ErrorDesc;

    [SerializeField]
    public TMP_Text ErrorTotal;

    [SerializeField]
    public TMP_Text ErrorAchieved;

    [SerializeField]
    private Button[] PanelButtons;

    public Color defaultColor;
    public Color changedColor;

    void OnValidate()
    {
        ErrorDesc = this.gameObject.transform.Find("TXT_ErrDesc").GetComponent<TMP_Text>();
        ErrorTotal = this.gameObject.transform.Find("TXT_ErrorTotal").GetComponent<TMP_Text>();
        ErrorAchieved = this.gameObject.transform.Find("TXT_ErrorAchieved").GetComponent<TMP_Text>();

        PanelButtons[0] = this.gameObject.transform.Find("CalcControlGroup").GetChild(0).GetComponent<Button>();
        PanelButtons[1] = this.gameObject.transform.Find("CalcControlGroup").GetChild(1).GetComponent<Button>();
        PanelButtons[2] = this.gameObject.transform.Find("IncreaseDecrease").GetChild(0).GetComponent<Button>();
        PanelButtons[3] = this.gameObject.transform.Find("IncreaseDecrease").GetChild(1).GetComponent<Button>();
    }

    void Update()
    {
        if(ErrorTotal.text != ErrorAchieved.text)
        {
            this.gameObject.GetComponent<Image>().color = changedColor;
            
        }
        else
        {
            this.gameObject.GetComponent<Image>().color = defaultColor;
        }
    }

    public void SetZero()
    {
        ErrorAchieved.text = Mathf.Clamp((int.Parse(ErrorTotal.text)-int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
    }

    public void SetHalf()
    {
        ErrorAchieved.text = Mathf.Clamp((Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2)),0,Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
    }

    public void IncreaseByOne(int val)
    {
        ErrorAchieved.text = Mathf.Clamp((int.Parse(ErrorAchieved.text)+val),0,int.Parse(ErrorTotal.text)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
    }

    public void DecreaseByOne(int val)
    {
        ErrorAchieved.text = Mathf.Clamp((int.Parse(ErrorAchieved.text)-val),0,int.Parse(ErrorTotal.text)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
    }
}
