
using UnityEngine;
[System.Serializable]
public class InteractableAction 
{
    public MonoBehaviour targetScript; // Скрипт, содержащий метод
    public string methodName; // Имя метода
    public int parameter; // Числовой параметр (можно изменить на нужный тип)
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
