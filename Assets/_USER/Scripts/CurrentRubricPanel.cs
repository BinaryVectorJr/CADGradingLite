using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
        ErrorDesc = this.gameObject.transform.Find("Group/TXT_ErrDesc").GetComponent<TMP_Text>();
        ErrorTotal = this.gameObject.transform.Find("Group/TXT_ErrorTotal").GetComponent<TMP_Text>();
        ErrorAchieved = this.gameObject.transform.Find("Group/IncreaseDecrease/TXT_ErrorAchieved").GetComponent<TMP_Text>();

        PanelButtons[0] = this.gameObject.transform.Find("Group/CalcControlGroup").GetChild(0).GetComponent<Button>();
        PanelButtons[1] = this.gameObject.transform.Find("Group/CalcControlGroup").GetChild(1).GetComponent<Button>();
        PanelButtons[2] = this.gameObject.transform.Find("Group/IncreaseDecrease").GetChild(0).GetComponent<Button>();
        PanelButtons[3] = this.gameObject.transform.Find("Group/IncreaseDecrease").GetChild(1).GetComponent<Button>();
        PanelButtons[4] = this.gameObject.transform.Find("Group/BTN_Reset").GetComponent<Button>();
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
        int _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorTotal.text)-int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text));
        ErrorAchieved.text = _tempErrAchieved.ToString();
        int _tempErrTotal = int.Parse(ErrorTotal.text);
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
        if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)) && RubricManager.rbmInstance.currentAssignmentButton != null)
        {
            RubricManager.rbmInstance.currentFeedback.Add(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")");
        }
        else
        {
            var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;
            
            //WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

            if(indexOfFeedbackItem.HasValue)
            {
                RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")";
            }
        }
    }

    public void SetHalf()
    {
        int _tempErrAchieved = Mathf.Clamp((Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2)),0,Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2));
        ErrorAchieved.text = _tempErrAchieved.ToString();
        int _tempErrTotal = int.Parse(ErrorTotal.text);
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
        if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
        {
            RubricManager.rbmInstance.currentFeedback.Add(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")");
        }
        else
        {
            var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;

            // WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

            if(indexOfFeedbackItem.HasValue)
            {
                RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")";
            }
        }
    }

    public void IncreaseByOne(int val)
    {
        ErrorAchieved.text = Mathf.Clamp((int.Parse(ErrorAchieved.text)+val),0,int.Parse(ErrorTotal.text)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
        if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
        {
            ResetScore();
        }
        else
        {
            if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
            {
                RubricManager.rbmInstance.currentFeedback.Add(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text);
            }
            else
            {
                var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;

                // WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

                if(indexOfFeedbackItem.HasValue)
                {
                    RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text;
                }
            }
        }
    }

    public void DecreaseByOne(int val)
    {
        ErrorAchieved.text = Mathf.Clamp((int.Parse(ErrorAchieved.text)-val),0,int.Parse(ErrorTotal.text)).ToString();
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
        if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
        {
            ResetScore();
        }
        else
        {
            if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
            {
                RubricManager.rbmInstance.currentFeedback.Add(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text);
            }
            else
            {
                var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;

                // WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

                if(indexOfFeedbackItem.HasValue)
                {
                    RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text;
                }
            }
        }
    }

    public void ResetScore()
    {
        ErrorAchieved.text = ErrorTotal.text;
        RubricManager.rbmInstance.UpdateScore();
        RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
        if (RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
        {
            RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(ErrorDesc.text));
        }
    }
}
