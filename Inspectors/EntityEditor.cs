using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Invert.Common.UI;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.Data;
using Invert.IOC;
using Invert.uFrame.ECS;
using uFrame.Attributes;
using uFrame.ECS;

[UnityEditor.CustomEditor(typeof(uFrame.ECS.Entity))]
public class EntityEditor : Editor {
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        InvertApplication.SignalEvent<IDrawUnityInspector>(_=>_.DrawInspector(target));
    }
}

public interface IDrawUnityInspector
{
    void DrawInspector(Object target);
}
public class UnityInspectors : DiagramPlugin, IDrawUnityInspector
{
    private WorkspaceService _workspaceService;

    public WorkspaceService WorkspaceService
    {
        get { return _workspaceService ?? (_workspaceService = Container.Resolve<WorkspaceService>()); }
    }
    private IRepository _repository;
//    private UserSettings _currentUser;

    public IRepository Repository
    {
        get { return _repository ?? (_repository = Container.Resolve<IRepository>()); }
    }


    //public string CurrentUserId
    //{
    //    get { return EditorPrefs.GetString("UF_CurrentUserId", string.Empty); }
    //    set
    //    {
    //        EditorPrefs.SetString("UF_CurrentUserId",value);
    //    }
    //}

    //public UserSettings CurrentUser
    //{
    //    get { return _currentUser ?? (_currentUser = Repository.GetSingle<UserSettings>(CurrentUserId)); }
    //}

    public override void Loaded(UFrameContainer container)
    {
        base.Loaded(container);
        
    }

    public void DrawInspector(Object target)
    {
        GUIHelpers.IsInsepctor = true;
        var entityComponent = target as Entity;
        if (entityComponent != null)
        {
            if (Repository != null)
            {
                EditorGUILayout.HelpBox("0 = Auto Assign At Runtime",MessageType.Info);
                
            }
          
        }
        var component = target as EcsComponent;
        //if (component != null)
        //{
           
            
            if (Repository != null)
            {
                var attribute = target.GetType().GetCustomAttributes(typeof(uFrameIdentifier), true).OfType<uFrameIdentifier>().FirstOrDefault();
            
                if (attribute != null)
                {
                    var item = Repository.GetSingle<ComponentNode>(attribute.Identifier);
                    if (component != null)
                    {
                        //if (GUIHelpers.DoToolbarEx("System Handlers"))
                        //{
                        //    foreach (
                        //   var handlerNode in
                        //       Repository.All<HandlerNode>()
                        //           .Where(p => p.EntityGroup != null && p.EntityGroup.Item == item))
                        //    {
                        //        if (GUILayout.Button(handlerNode.Name))
                        //        {
                        //            Execute(new NavigateToNodeCommand()
                        //            {
                        //                Node = handlerNode,
                        //                Select = true
                        //            });
                        //        }
                              
                        //    }
                        //}
                        if (GUIHelpers.DoToolbarEx("uFrame Designer"))
                        foreach (
                           var handlerNode in
                               Repository.All<HandlerNode>()
                                   .Where(p => p.EntityGroup != null && p.EntityGroup.Item != null && p.EntityGroup.Item.SelectComponents.Contains(item)))
                        {
                            EditorGUILayout.BeginHorizontal();

                            if (GUILayout.Button(handlerNode.Name + " >"))
                            {
                                Execute(new NavigateToNodeCommand()
                                {
                                    Node = handlerNode,
                                    Select = true
                                });
                            }
                            var meta = handlerNode.Meta as EventMetaInfo;
                            if (meta != null && meta.Dispatcher && component.gameObject.GetComponent(meta.SystemType) == null)
                            {
                                if (GUILayout.Button("+ " + meta.SystemType.Name))
                                {
                                    
                                    component.gameObject.AddComponent(meta.SystemType);
                                }
                            }
                         
                            EditorGUILayout.EndHorizontal();
                            if (GUILayout.Button("Edit In Designer"))
                            {
                                Execute(new NavigateToNodeCommand()
                                {
                                    Node = item,
                                    Select = true
                                });
                            }
                        }

                     
                    }
                   
                }
            }

        //}
        
    }

    //public class UserSettings : IDataRecord
    //{
    //    [IDataRecord]
    //    public string UserName { get; set; }

    //    public int EntityId { get; set; }

    //    public int StartingId { get; }

    //    public string Identifier { get; set; }
    //    public IRepository Repository { get; set; }
    //    public bool Changed { get; set; }
    //    public IEnumerable<string> ForeignKeys { get { yield break; } }
    //}
}