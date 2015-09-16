using Invert.Core;
using UnityEditor;

[UnityEditor.CustomEditor(typeof(uFrame.ECS.EcsComponent),true)]
public class ComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InvertApplication.SignalEvent<IDrawUnityInspector>(_ => _.DrawInspector(target));
    }
}