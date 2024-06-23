using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Data/Weapon")]
public class WeaponSO : ScriptableObject
{
    public int index;
    public string nameWeapon;
    public int cost;
    public int range, atkSpeed;
    public string attribute;
    public TypeAtk typeAtk;
}
