using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.PackageManager;
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
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(localAssociatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                ErrorDesc.text = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_desc;
                ErrorAchieved.text = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString();
                ErrorTotal.text = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString();

                // ErrorDesc.text = associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_desc;
                // ErrorAchieved.text = int.Parse(associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_achieved_points.ToString()).ToString();
                // ErrorTotal.text = int.Parse(associatedAssignmentButton.GetComponent<AssignmentType>().localAssignmentWithRubric[0].assm_rubrics[panelID].error_item_total_points.ToString()).ToString();
            }
        }
    }

    // TODO: The original projectbasedataset element is being modified somehow. Try to break the logic, where
    // original data is unmodified. Its copy is made to rubric manager. And when rubrics are manipulated, the copied instance
    // is worked on. Then when the rubrics are reset or loaded again, they pull in from original source (which should be UNMODIFIED)

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
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id == localAssociatedAssignment)
            {
                // VERIFIED WORKING: Debug.Log(tempIndex.index);
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = int.Parse(ErrorAchieved.text);
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
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            if(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id == localAssociatedAssignment)
            {
                //VERIFIED WORKING: Debug.Log(tempIndex.index);
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points;
            }
            else
            {
                return;
            }
        }
    }

    public void SetZero()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            int _tempErrAchieved = 0;
            _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorTotal.text)-int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text));
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     var localTotal = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_total_points;

            //     _tempErrAchieved = Mathf.Clamp(int.Parse(localTotal.ToString()) - int.Parse(localTotal.ToString()),0,int.Parse(ErrorTotal.text));
            // }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     RubricManager.rbmInstance.UpdateScoreProject(_tempErrAchieved);

            //     var localAchieved = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points;

            //     if(RubricManager.rbmInstance.currentScoresProjectDict.ContainsKey(RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()))
            //     {
            //         List<int> values = RubricManager.rbmInstance.currentScoresProjectDict[RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()];

            //         for (int i=0; i< values.Count; i++)
            //         {
            //             values[i] = int.Parse(localAchieved.ToString());
            //         }
            //     }

            //     // for(int i=0; i<RubricManager.rbmInstance.allAssignmentButtonsProject.Count; i++)
            //     // {
            //     //     RubricManager.rbmInstance.currentScoresProjectDict.Values.(int.Parse(localAchieved.ToString()));
            //     // }
                
            // }

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

            // if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     // Updating the local variable score so that it persists
            //     RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points = _tempErrAchieved;
            // }

            WriteBackToSource();
        }
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString())-int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString())),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
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

            WriteBackToSource();
        }

    }

    public void SetHalf()
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2)),0,Mathf.CeilToInt(float.Parse(ErrorTotal.text)/2));
            }

            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     var localTotal = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_total_points;

            //     _tempErrAchieved = Mathf.Clamp((Mathf.CeilToInt(float.Parse(localTotal.ToString())/2)),0,Mathf.CeilToInt(float.Parse(localTotal.ToString())/2));
            // }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     RubricManager.rbmInstance.UpdateScoreProject(_tempErrAchieved);
                
            //     var localAchieved = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points;

            //     if(RubricManager.rbmInstance.currentScoresProjectDict.ContainsKey(RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()))
            //     {
            //         List<int> values = RubricManager.rbmInstance.currentScoresProjectDict[RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()];

            //         for (int i=0; i< values.Count; i++)
            //         {
            //             values[i] = int.Parse(localAchieved.ToString());
            //         }
            //     }
            // }

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

            // if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     // Updating the local variable score so that it persists
            //     RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points = _tempErrAchieved;
            // }

            WriteBackToSource();
        }

        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString())/2,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            
            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
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

            WriteBackToSource();
        }
    }

    public void IncreaseByOne(int val)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorAchieved.text)+val),0,int.Parse(ErrorTotal.text));
            }

            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     var localTotal = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_total_points;

            //     _tempErrAchieved = Mathf.Clamp((int.Parse(localTotal.ToString())+val),0,int.Parse(localTotal.ToString()));
            // }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     RubricManager.rbmInstance.UpdateScoreProject(_tempErrAchieved);

            //     var localAchieved = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points;

            //     if(RubricManager.rbmInstance.currentScoresProjectDict.ContainsKey(RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()))
            //     {
            //         List<int> values = RubricManager.rbmInstance.currentScoresProjectDict[RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()];

            //         for (int i=0; i< values.Count; i++)
            //         {
            //             values[i] = int.Parse(localAchieved.ToString());
            //         }
            //     }
            // }

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

            // if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     // Updating the local variable score so that it persists
            //     RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points = _tempErrAchieved;
            // }

            WriteBackToSource();
        }
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString())+val,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
            }

            if(string.Equals(ErrorAchieved.text,ErrorTotal.text))
            {
                ResetScore();
            }
            else
            {
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

            WriteBackToSource();
        }
        
    }

    public void DecreaseByOne(int val)
    {
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
            int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorAchieved.text)-val),0,int.Parse(ErrorTotal.text));
            }

            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     var localTotal = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_total_points;

            //     _tempErrAchieved = Mathf.Clamp((int.Parse(localTotal.ToString())-val),0,int.Parse(localTotal.ToString()));
            // }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);
            
            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     RubricManager.rbmInstance.UpdateScoreProject(_tempErrAchieved);
                
            //     var localAchieved = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points;

            //     if(RubricManager.rbmInstance.currentScoresProjectDict.ContainsKey(RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()))
            //     {
            //         List<int> values = RubricManager.rbmInstance.currentScoresProjectDict[RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()];

            //         for (int i=0; i< values.Count; i++)
            //         {
            //             values[i] = int.Parse(localAchieved.ToString());
            //         }
            //     }
            // }

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
                        RubricManager.rbmInstance.currentFeedback[indexOfFeedbackItem.Value] = RubricManager.rbmInstance.currentAssignmentButton.name.ToString()  + " - DEDUCTION: " + ErrorDesc.text + " (-" + (_tempErrTotal-_tempErrAchieved).ToString() +")";
                    }
                }
            }

            // if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     // Updating the local variable score so that it persists
            //     RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points = _tempErrAchieved;
            // }

            WriteBackToSource();
        }
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points.ToString())-val,0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();
            int _tempErrTotal = int.Parse(ErrorTotal.text);

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
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
        if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
        {
                int _tempErrAchieved = 0;

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                _tempErrAchieved = Mathf.Clamp((int.Parse(ErrorTotal.text)),0,int.Parse(ErrorTotal.text));
            }

            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     var localTotal = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_total_points;

            //     _tempErrAchieved = Mathf.Clamp(int.Parse(localTotal.ToString()),0,int.Parse(ErrorTotal.text));
            // }

            ErrorAchieved.text = _tempErrAchieved.ToString();

            if(GameManager.gmInstance.currentState == GameManager.GameState.REGULAR_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.currentScores[panelID]=int.Parse(ErrorAchieved.text);
            }
            // else if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     RubricManager.rbmInstance.UpdateScoreProject(_tempErrAchieved);
                
            //     var localAchieved = RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points;

            //     if(RubricManager.rbmInstance.currentScoresProjectDict.ContainsKey(RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()))
            //     {
            //         List<int> values = RubricManager.rbmInstance.currentScoresProjectDict[RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().associatedProject.ToString()];

            //         for (int i=0; i< values.Count; i++)
            //         {
            //             values[i] = int.Parse(localAchieved.ToString());
            //         }
            //     }
            // }


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

            // if (GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            // {
            //     // Updating the local variable score so that it persists
            //     RubricManager.rbmInstance.currentAssignmentButton.GetComponent<AssignmentType>().localProjectWithRubricData[0].rubric_data[panelID].error_item_achieved_points = _tempErrAchieved;
            // }

            WriteBackToSource();
        }
        if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
        {
            var tempIndex = RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments.Select((assm_rubrics,index)=> new {assm_rubrics,index}).Where(x=>x.assm_rubrics.assm_id == associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment).FirstOrDefault();

            int _tempErrAchieved = 0;
            if(associatedAssignmentButton.GetComponent<AssignmentType>().associatedAssignment == RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_id)
            {
                _tempErrAchieved = Mathf.Clamp(int.Parse(RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_total_points.ToString()),0,int.Parse(ErrorTotal.text));
            }

            ErrorAchieved.text = _tempErrAchieved.ToString();

            if(GameManager.gmInstance.currentState == GameManager.GameState.PROJECT_GRADING)
            {
                RubricManager.rbmInstance.UpdateScore();
                RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_achieved_points = int.Parse(ErrorAchieved.text);
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
                    RubricManager.rbmInstance.currentFeedback.RemoveAll(item => item.Contains(ErrorDesc.text));
                }
            }

            RubricManager.rbmInstance.MasterActiveProjectComponent.project_associated_assignments[tempIndex.index].assm_rubrics[panelID].error_item_achieved_points = _tempErrAchieved;

            WriteBackToSource();
        }
    }
}
