using System.Linq;
using System.Net.WebSockets;
using TMPro;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.UI;

public class CurrentRubricPanel : MonoBehaviour
{
    public int panelID = 0;

    public int localAssociatedAssignment = 0;

    public GameObject associatedAssignmentButton;

    [SerializeField]
    public TMP_Text ErrorDesc;

    [SerializeField]
    public TMP_Text ErrorTotal;

    [SerializeField]
    public TMP_Text ErrorAchieved;

    [SerializeField]
    private Button[] PanelButtons;
    
    [SerializeField]
    public Button lastPressedButtonInPanel;

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
        PanelButtons[3] = this.gameObject.transform.Find("Group/IncreaseDecrease").GetChild(2).GetComponent<Button>();
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

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            AssmManager.assmMgrInstance.RecalculateCurrentTotal(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource);
        }
    }

    public void UpdateValues()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            if(localAssociatedAssignment == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment)
            {
                //VALIDATED WORKING: Debug.Log("HERE");
                ErrorDesc.text = associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_desc;
                ErrorAchieved.text = int.Parse(associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_achieved_points.ToString()).ToString();
                ErrorTotal.text = int.Parse(associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_total_points.ToString()).ToString();
            }
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            // LINQ to search for the index of assignment rubric (L3) associated to a specific assignment (L2) in project (L1) and compare to the assignment ID stored in the button that was clicked
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(localAssociatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                ErrorDesc.text = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_desc;
                ErrorAchieved.text = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString();
                ErrorTotal.text = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString();
            }
        }
    }

    public void WriteBackToSource()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            if(localAssociatedAssignment == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment)
            {
                associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_achieved_points = int.Parse(ErrorAchieved.text);
            }
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id == localAssociatedAssignment)
            {
                //VERIFIED WORKING: Debug.Log(tempIndex.index);
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = int.Parse(ErrorAchieved.text);
            }
            else
            {
                return;
            }
        }
    }

    public void ResetScoresToOriginalTotal()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            if(localAssociatedAssignment == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment)
            {
                associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_achieved_points = associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_total_points;
            }
        }
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id == localAssociatedAssignment)
            {
                //VERIFIED WORKING: Debug.Log(tempIndex.index);
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points;
            }
            else
            {
                return;
            }
        }
    }

    public void DisableOtherButtons(Button _pressedButton)
    {
        foreach(var item in PanelButtons)
        {
            if(item.transform.name == _pressedButton.transform.name || item.transform.name == "BTN_Reset")
            {
                // Do nothing
            }
            else
            {
                item.GetComponent<Button>().interactable = false;

            }
        }
    }

    public void EnableOtherButtons()
    {
        foreach(var item in PanelButtons)
        {
            if(item.GetComponent<Button>().interactable == false)
            {
                item.GetComponent<Button>().interactable = true;
            }
        }
    }

    public void SetZero()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_Zero");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;
            _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorTotal.text)-int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text));

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }

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

            WriteBackToSource();
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_Zero");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString())-int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString()),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
                if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Equals(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)) && RubricManager.rbmInstance.currentAssignmentButton != null)
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

            WriteBackToSource();
        }

    }

    public void SetHalf()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_Half");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2)),0,Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }

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

            WriteBackToSource();
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_Half");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString())/2,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            
            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
                if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Equals(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)) && RubricManager.rbmInstance.currentAssignmentButton != null)
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

            WriteBackToSource();
        }
    }

    public void IncreaseByOne(int val)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_AddOne");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorAchieved.text)+val),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }

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
                        RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")";
                    }
                }
            }

            WriteBackToSource();
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_AddOne");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString())+val,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
                if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Equals(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)) && RubricManager.rbmInstance.currentAssignmentButton != null)
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

            WriteBackToSource();
        }
        
    }

    public void DecreaseByOne(int val)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_RemoveOne");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorAchieved.text)-val),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }

            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
                if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Equals(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
                {
                    RubricManager.rbmInstance.currentFeedback.Add(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text);
                }
                else
                {
                    var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;

                    // WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

                    if(indexOfFeedbackItem.HasValue)
                    {
                        RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString()  + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")";
                    }
                }
            }

            WriteBackToSource();
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            // Deactivate other buttons so that they cannot be pressed until Reset is hit
            lastPressedButtonInPanel = PanelButtons.FirstOrDefault(btnName => btnName.gameObject.name == "BTN_RemoveOne");
            DisableOtherButtons(lastPressedButtonInPanel);

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString())-val,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

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

            WriteBackToSource();
        }
        
    }

    public void ResetScore()
    {
        EnableOtherButtons();

        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
                int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }

            if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)))
            {
                RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(ErrorDesc.text));
            }
            else
            {
                var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;

                // WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

                if(indexOfFeedbackItem.HasValue)
                {
                    RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(ErrorDesc.text));
                }
            }

            WriteBackToSource();
        }
        
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString()),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            if (!RubricManager.rbmInstance.currentFeedback.Any(s => s.Contains(ErrorDesc.text, System.StringComparison.OrdinalIgnoreCase)) && RubricManager.rbmInstance.currentAssignmentButton != null)
            {
                RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(ErrorDesc.text));
            }
            else
            {
                var indexOfFeedbackItem = RubricManager.rbmInstance.currentFeedback.Select((value,idx) => new {value,idx}).FirstOrDefault(x => x.value.Contains(ErrorDesc.text))?.idx;
                
                //WORKING VALIDATED: Debug.Log(indexOfFeedbackItem.Value.ToString());

                if(indexOfFeedbackItem.HasValue)
                {
                    RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(RubricManager.rbmInstance.currentAssignmentButton.name.ToString() + " - DEDUCTION: " + ErrorDesc.text));
                }
            }

            RubricManager.rbmInstance.MasterActiveProjectSubsetFromSource.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = _tempErrAchieved;

            WriteBackToSource();
        }
    }
}
