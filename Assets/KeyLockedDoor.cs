using System.Reflection;
using Gamekit3D;
using Gamekit3D.GameCommands;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class KeyLockedDoor : MonoBehaviour
{
    public GameCommandReceiver door;

    bool opened;

    static readonly FieldInfo TimeField =
        typeof(SimpleTransformer).GetField("time", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo PositionField =
        typeof(SimpleTransformer).GetField("position", BindingFlags.Instance | BindingFlags.NonPublic);
    static readonly FieldInfo DirectionField =
        typeof(SimpleTransformer).GetField("direction", BindingFlags.Instance | BindingFlags.NonPublic);

    void OnTriggerEnter(Collider other)
    {
        if (opened)
            return;

        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        if (KeyHUD.Instance == null || !KeyHUD.Instance.HasKey)
            return;

        ArmOpen();
        door.Receive(GameCommandType.Open);
        opened = true;
        KeyHUD.Instance.Consume();
    }

    void ArmOpen()
    {
        if (door == null)
            return;

        foreach (var translator in door.GetComponents<SimpleTranslator>())
        {
            if (translator.interactionType != GameCommandType.Open)
                continue;

            TimeField.SetValue(translator, 0f);
            PositionField.SetValue(translator, 0f);
            DirectionField.SetValue(translator, 1f);
            translator.enabled = true;
        }
    }
}