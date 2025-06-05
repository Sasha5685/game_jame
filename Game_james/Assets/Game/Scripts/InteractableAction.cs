
using UnityEngine;
[System.Serializable]
public class InteractableAction 
{
    public MonoBehaviour targetScript; // ������, ���������� �����
    public string methodName; // ��� ������
    public int parameter; // �������� �������� (����� �������� �� ������ ���)
    public ParameterType parameterType = ParameterType.None;

    public enum ParameterType
    {
        None,
        Int,
        Float,
        String,
        Bool
    }
}
