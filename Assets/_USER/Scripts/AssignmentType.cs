using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignmentType : MonoBehaviour
{
    [SerializeField]
    public int assignmentTypeCode = 0;

    [SerializeField]
    public int projectID = 0;

    [SerializeField]
    public List<ProjectWithRubric> localProjectWithRubricData = new List<ProjectWithRubric>();
}
