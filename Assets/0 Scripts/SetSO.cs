using UnityEngine;


[CreateAssetMenu(fileName = "New Set", menuName = "Data/Set")]
public class SetSO : ScriptableObject
{
    public int index;
    public int range, gold, moveSpeed;
    public string attribute;
}

