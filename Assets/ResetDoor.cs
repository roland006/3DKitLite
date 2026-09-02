using System.Reflection;
using Gamekit3D;
using Gamekit3D.GameCommands;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class ResetDoor : MonoBehaviour
{
    [Tooltip("Объект с Simple Translator (WorkingDoor).")]
    public GameCommandReceiver door;

    static readonly FieldInfo TimeField = typeof(SimpleTransformer).GetField("time", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo PositionField = typeof(SimpleTransformer).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo DirectionField = typeof(SimpleTransformer).GetField("direction", BindingFlags.Instance | BindingFlags.NonPublic);

    void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
            Arm(GameCommandType.Open);
    }

    void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
            Arm(GameCommandType.Close);
    }

    bool IsPlayer(Collider other)
    {
        return other.GetComponentInParent<PlayerController>() != null;
    }

    void Arm(GameCommandType type)
    {
        if (door == null)
            return;

        bool found = false;
        foreach (var translator in door.GetComponents<SimpleTranslator>())
        {
            if (translator.interactionType != type)
                continue;

            TimeField.SetValue(translator, 0f);
            PositionField.SetValue(translator, 0f);
            DirectionField.SetValue(translator, 1f);
            translator.enabled = true;
            found = true;
        }

        if (!found)
            Debug.LogWarning("ResetDoor: на " + door.name + " нет Simple Translator с типом " + type);
    }
}