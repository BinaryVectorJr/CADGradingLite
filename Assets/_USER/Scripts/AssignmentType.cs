using System.Collections.Generic;
using UnityEngine;

public class AssignmentType : MonoBehaviour
{
    [SerializeField]
    public int associatedAssignment = 0;

    [SerializeField]
    public int associatedProject = 0;

    [SerializeField]
    public List<AssignmentDataElement> localAssignmentWithRubric = new List<AssignmentDataElement>();

    [SerializeField]
    public List<ProjectBaseElement> localProjectWithRubricData = new List<ProjectBaseElement>();
}
