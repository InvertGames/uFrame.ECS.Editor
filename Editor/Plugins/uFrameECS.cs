using System.Reflection;
using Invert.Core.GraphDesigner.Unity;
using Invert.Data;
using Invert.IOC;
using Invert.Windows;
using uFrame.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;

    public class uFrameECS : uFrameECSBase,
        IPrefabNodeProvider,
        IContextMenuQuery, IQuickAccessEvents, IOnMouseDoubleClickEvent,
        IExecuteCommand<AddSlotInputNodeCommand>,
        IExecuteCommand<NewModuleWorkspace>,
        IQueryPossibleConnections,
        IExecuteCommand<GroupActionNodes>,
        IQueryTypes,
        IDocumentationProvider

    {
        public override decimal LoadPriority
        {
            get { return 500; }
        }

        private static Dictionary<string, ActionMetaInfo> _actions;
        private static Dictionary<string, EventMetaInfo> _events;
        private readonly static HashSet<Type> _types = new HashSet<Type>();

        static uFrameECS()
        {
            
            InvertApplication.CachedAssembly(typeof(Button).Assembly);
            InvertApplication.CachedTypeAssembly(typeof(uFrameECS).Assembly);
            InvertApplication.TypeAssemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies().Where(p => p.FullName.StartsWith("Assembly")));
        }
        public static Type EcsComponentType
        {
            get;
            set;
        }
        public static Type EntityComponentType
        {
            get;
            set;
        }

       

        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);
            InvertGraphEditor.TypesContainer.RegisterInstance(new GraphTypeInfo()
            {
                Type=typeof(IDisposable),
                IsPrimitive =  false,
                Label = "Disposable",
                Name = "Disposable"
            },"IDisposable");
            container.RegisterInstance<IDocumentationProvider>(this, "ECS");
            container.RegisterGraphItem<HandlerNode, HandlerNodeViewModel, HandlerNodeDrawer>();
            container.RegisterGraphItem<CustomAction, CustomActionViewModel, SequenceItemNodeDrawer>();
            Handler.AllowAddingInMenu = false;
            Library.HasSubNode<EnumNode>();
            //            ComponentGroup.AllowAddingInMenu = false;
            PropertyChanged.Name = "Property Changed Handler";
            UserMethod.AllowAddingInMenu = false;
            Action.AllowAddingInMenu = false;
            SequenceItem.AllowAddingInMenu = false;
            //            VariableReference.AllowAddingInMenu = false;
            CustomAction.Name = "Custom Action";
            System.Name = "System";
            Handler.Name = "Handler";
            ComponentCreated.Name = "Component Created Handler";
            ComponentDestroyed.Name = "Component Destroyed Handler";
            Action.NodeColor.Literal = NodeColor.Green;
            //System.HasSubNode<TypeReferenceNode>();
            Module.HasSubNode<TypeReferenceNode>();
            Module.HasSubNode<EnumNode>();
            Group.HasSubNode<EnumValueNode>();
            //System.HasSubNode<ComponentNode>();
            // System.HasSubNode<ContextNode>(); 

            Library.HasSubNode<TypeReferenceNode>();
            Module.HasSubNode<ComponentNode>();
           // System.HasSubNode<ComponentNode>();
            container.RegisterDrawer<ItemViewModel<IContextVariable>, ItemDrawer>();
            //container.AddItemFlag<ComponentsReference>("Multiple", UnityEngine.Color.blue);
            container.AddItemFlag<PropertiesChildItem>("Mapping", UnityEngine.Color.blue);
            container.AddNodeFlag<EventNode>("Dispatcher");
            //System.HasSubNode<EnumNode>();
            container.Connectable<IContextVariable, IActionIn>();
            container.Connectable<IActionOut, IContextVariable>();
            //container.RegisterInstance<RegisteredConnection>(new RegisteredConnection()
            //{
            //    TInputType = typeof(IContextVariable),
            //    TOutputType = typeof(IActionIn)
            //}, "Context Variables");
            //container.RegisterInstance<RegisteredConnection>(new RegisteredConnection()
            //{
            //    TInputType = typeof(IActionOut),
            //    TOutputType = typeof(IContextVariable)
            //}, "Context Variables2");
            // container.Connectable<ActionOut, ActionIn>(UnityEngine.Color.blue);
            container.Connectable<ActionBranch, SequenceItemNode>();
            container.Connectable<IMappingsConnectable, HandlerIn>();
            //container.AddWorkspaceConfig<LibraryWorkspace>("Library").WithGraph<LibraryGraph>("Library Graph");
            container.AddWorkspaceConfig<EcsWorkspace>("ECS")
                .WithGraph<LibraryGraph>("Library","Create components, groups, events, custom actions, and more.")
                .WithGraph<SystemGraph>("System","System defines behaviour for items defines inside a your libraries.")
                .WithGraph<ModuleGraph>("Module","A module graph allows you to comprise library items and system as a single graph.")
                ;
            //container.AddWorkspaceConfig<BehaviourWorkspace>("Behaviour").WithGraph<SystemGraph>("System Graph");
            EnumValue.Name = "Enum Value";
            //            VariableReference.Name = "Var";

            StaticLibraries.Add(typeof(Input));
            StaticLibraries.Add(typeof(Math));
            StaticLibraries.Add(typeof(Mathf));
            SystemTypes.Add(typeof (UnityEngine.UI.Button));
            SystemTypes.Add(typeof (UnityEngine.UI.LayoutElement));
            SystemTypes.Add(typeof (UnityEngine.UI.LayoutGroup));
            SystemTypes.Add(typeof (UnityEngine.UI.GridLayoutGroup));
            SystemTypes.Add(typeof (UnityEngine.UI.Text));
            SystemTypes.Add(typeof (UnityEngine.UI.InputField));
            //SystemTypes.Add(typeof (UnityEngine.UI.Dropdown));
            SystemTypes.Add(typeof (UnityEngine.UI.ScrollRect));
            SystemTypes.Add(typeof (UnityEngine.UI.Scrollbar));
            SystemTypes.Add(typeof (UnityEngine.UI.Outline));
            SystemTypes.Add(typeof (UnityEngine.UI.Toggle));
            SystemTypes.Add(typeof (UnityEngine.UI.ToggleGroup));
            SystemTypes.Add(typeof (UnityEngine.UI.Slider));
            SystemTypes.Add(typeof (UnityEngine.Transform));
            //StaticLibraries.Add(typeof(Vector2));
            //StaticLibraries.Add(typeof(Vector3));
            StaticLibraries.Add(typeof(Physics));
            StaticLibraries.Add(typeof(Physics2D));

            LoadActions();
            LoadEvents();

            AddHandlerType(typeof(PropertyChangedNode));
            AddHandlerType(typeof(ComponentDestroyedNode));
            AddHandlerType(typeof(ComponentCreatedNode));
            AddHandlerType(typeof(ActionGroupNode));


        }

        private static void AddHandlerType(Type type)
        {
            var propertyTypes = FilterExtensions.AllowedFilterNodes[type] = new List<Type>();
            foreach (var item in FilterExtensions.AllowedFilterNodes[typeof(HandlerNode)])
            {
                propertyTypes.Add(item);
            }
        }


        private void LoadActions()
        {


            LoadActionTypes();

            LoadActionLibrary();
        }

        private void LoadActionTypes()
        {
            Actions.Clear();

            //// Query for the available actions
            //ActionTypes = InvertApplication.GetDerivedTypes<UFAction>(false, false).ToArray();

            foreach (var actionType in ActionTypes)
            {

                if (Actions.ContainsKey(actionType.FullName)) continue;
                var actionInfo = new ActionMetaInfo(actionType);
                actionInfo.MetaAttributes =
                    actionType.GetCustomAttributes(typeof(ActionMetaAttribute), true).OfType<ActionMetaAttribute>().ToArray();
                var fields = actionType.GetFields(BindingFlags.Instance | BindingFlags.Public);
                if (!typeof(SequenceItemNode).IsAssignableFrom(actionType))
                {
                    foreach (var field in fields)
                    {
                        var fieldMetaInfo = new ActionFieldInfo()
                        {
                            MemberType = new SystemTypeInfo(field.FieldType),
                            Name = field.Name,
                            IsBranch = field.FieldType == typeof(Action),
                            MemberName = field.Name
                        };
                        if (!SystemTypes.Contains(field.FieldType))
                            SystemTypes.Add(field.FieldType);
                        fieldMetaInfo.MetaAttributes =
                            field.GetCustomAttributes(typeof(ActionAttribute), true)
                                .OfType<ActionAttribute>()
                                .ToArray();
                        if (fieldMetaInfo.DisplayType == null)
                            continue;

                        actionInfo.ActionFields.Add(fieldMetaInfo);
                    }
                }
                else
                {
                    Container.RegisterRelation(actionType, typeof(ViewModel), typeof(CustomActionViewModel));
                    Container.GetNodeConfig(actionType);
                    actionInfo.IsEditorClass = true;
                }

                Actions.Add(actionType.FullName, actionInfo);
            }
        }

        public static HashSet<Type> StaticLibraries
        {
            get { return _staticLibraries ?? (_staticLibraries = new HashSet<Type>()); }
            set { _staticLibraries = value; }
        }

        private static void LoadActionLibrary()
        {
            foreach (var assembly in InvertApplication.CachedAssemblies.Concat(InvertApplication.TypeAssemblies))
            {
                foreach (
                    var type in
                        assembly.GetTypes() 
                            .Where(p => p.IsSealed && p.IsDefined(typeof(ActionLibrary), true) || StaticLibraries.Contains(p)))
                {

                    var category = type.GetCustomAttributes(typeof(uFrameCategory), true).OfType<uFrameCategory>().FirstOrDefault();
                    var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        if (method.Name.StartsWith("get_")) continue;
                        if (method.Name.StartsWith("set_")) continue;
                        var actionInfo = new ActionMethodMetaInfo(type)
                        {
                            Category = category,
                            Method = method,
                        };

                        actionInfo.MetaAttributes =
                            method.GetCustomAttributes(typeof(ActionMetaAttribute), true)
                                .OfType<ActionMetaAttribute>()
                                .ToArray();

                        if (actionInfo.Category == null)
                        {
                            actionInfo.Category = new uFrameCategory(type.Name);
                        }
                        //var genericArguments = method.GetGenericArguments();
                        var vars = method.GetParameters();

                        //foreach (var item in genericArguments)
                        //{
                        //    var fieldMetaInfo = new ActionFieldInfo()
                        //    {
                        //        MemberType = new SystemTypeInfo(item.GetGenericParameterConstraints()[0]),
                        //        Name = item.Name,
                        //        DisplayType = new In(item.Name, item.Name),
                        //        IsGenericArgument = true,
                             
                        //    };
                        //    actionInfo.ActionFields.Add(fieldMetaInfo);
                        //}

                        foreach (var parameter in vars)
                        {
                            var fieldMetaInfo = new ActionFieldInfo()
                            {
                                MemberType = new SystemTypeInfo(parameter.ParameterType),
                                Name = parameter.Name,
                                IsBranch = parameter.ParameterType == typeof(Action),
                                MemberName = parameter.Name,
                                IsByRef = parameter.ParameterType.IsByRef
                            };
                            if (!SystemTypes.Contains(parameter.ParameterType))
                                SystemTypes.Add(parameter.ParameterType);
                            fieldMetaInfo.MetaAttributes =
                                method.GetCustomAttributes(typeof(FieldDisplayTypeAttribute), true)
                                    .Cast<FieldDisplayTypeAttribute>()
                                    .Where(p => p.ParameterName == parameter.Name).ToArray();
                            if (!fieldMetaInfo.MetaAttributes.Any())
                            {
                                if (parameter.IsOut || parameter.ParameterType == typeof(Action))
                                {
                                    fieldMetaInfo.DisplayType = new Out(parameter.Name, parameter.Name);
                                }
                                else
                                {
                                    fieldMetaInfo.DisplayType = new In(parameter.Name, parameter.Name);
                                }
                            }
                            actionInfo.ActionFields.Add(fieldMetaInfo);
                        }
                        if (method.ReturnType != typeof(void))
                        {
                            var result = new ActionFieldInfo()
                            {
                                MemberType = new SystemTypeInfo(method.ReturnType),
                                IsReturn = true,
                                Name = "Result",
                                MemberName = "Result"
                            };
                            if (!SystemTypes.Contains(method.ReturnType))
                                SystemTypes.Add(method.ReturnType);
                            result.MetaAttributes =
                                method.GetCustomAttributes(typeof(FieldDisplayTypeAttribute), true)
                                    .OfType<FieldDisplayTypeAttribute>()
                                    .Where(p => p.ParameterName == "Result").ToArray();

                            result.DisplayType = new Out("Result", "Result");
                            actionInfo.ActionFields.Add(result);
                        }
                        if (Actions.ContainsKey(actionInfo.FullName))
                            continue;
                        Actions.Add(actionInfo.FullName, actionInfo);
                    }
                }
            }
        }
        public IEnumerable<Type> ActionTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.TypeAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsDefined(typeof(ActionTitle), true))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
        public IEnumerable<Type> EventTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.TypeAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {

                        if (type.IsDefined(typeof(uFrameEvent), true))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }

        private IHandlerCodeWriter[] _codeWriters;
        private static HashSet<Type> _staticLibraries;

        public IHandlerCodeWriter[] CodeWriters
        {
            get
            {
                return _codeWriters ??
                       (_codeWriters = EventCodeWriterTypes.Select(p => Activator.CreateInstance(p)).Cast<IHandlerCodeWriter>().ToArray());
            }
        }
        public IEnumerable<Type> EventCodeWriterTypes
        {
            get
            {
                foreach (var assembly in InvertApplication.CachedAssemblies)
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.IsClass && !type.IsAbstract && typeof(IHandlerCodeWriter).IsAssignableFrom(type))
                        {
                            yield return type;
                        }
                    }
                }
            }
        }
        private void LoadEvents()
        {

            Events.Clear();
            foreach (var eventType in EventTypes)
            {
                if (Events.ContainsKey(eventType.FullName)) continue;
                var eventInfo = new EventMetaInfo(eventType)
                {                    
                 
                };

                eventInfo.Attribute =
                    eventType.GetCustomAttributes(typeof(uFrameEvent), true).OfType<uFrameEvent>().FirstOrDefault();

                //var fields = eventType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                //var properties = eventType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);


                //foreach (var field in fields)
                //{
                //    var fieldMetaInfo = new EventFieldInfo()
                //    {
                //        Type = field.FieldType,
                //        Attribute = eventType.GetCustomAttributes(typeof(uFrameEventMapping), true).OfType<uFrameEventMapping>().FirstOrDefault(),
                //        Name = field.Name
                //    };
                //    if (!SystemTypes.Contains(field.FieldType))
                //        SystemTypes.Add(field.FieldType);

                //    eventInfo.Members.Add(fieldMetaInfo);
                //}
                //foreach (var field in properties)
                //{
                //    var fieldMetaInfo = new EventFieldInfo()
                //    {
                //        Type = field.PropertyType,
                //        Name = field.Name,
                //        Attribute = eventType.GetCustomAttributes(typeof(uFrameEventMapping), true).OfType<uFrameEventMapping>().FirstOrDefault(),
                //        IsProperty = true
                //    };

                //    if (!SystemTypes.Contains(field.PropertyType))
                //        SystemTypes.Add(field.PropertyType);

                //    eventInfo.Members.Add(fieldMetaInfo);
                //}
                Events.Add(eventType.FullName, eventInfo);
            }
        }

        public IEnumerable<QuickAddItem> PrefabNodes(INodeRepository nodeRepository)
        {
            foreach (var item in Events)
            {
                var item1 = item;
                var qa = new QuickAddItem(item.Value.Namespace, item.Value.Attribute.Title, _ =>
                {
                    var eventNode = new HandlerNode()
                    {
                        Meta = item1.Value

                    };
                    _.Diagram.AddNode(eventNode, _.MousePosition);
                })
                {

                };
                yield return qa;
            }
            yield break;
        }

        public static Dictionary<string, EventMetaInfo> Events
        {
            get { return _events ?? (_events = new Dictionary<string, EventMetaInfo>()); }
            set { _events = value; }
        }
        public static Dictionary<string, ActionMetaInfo> Actions
        {
            get { return _actions ?? (_actions = new Dictionary<string, ActionMetaInfo>()); }
            set { _actions = value; }
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, object obj)
        {

            if (obj is InputOutputViewModel)
            {
            
                QuerySlotMenu(ui, (InputOutputViewModel)obj);
            }
            var handlerVM = obj as HandlerNodeViewModel;
            if (handlerVM != null)
            {
                var handler = handlerVM.Handler;
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Code Handler",
                    Checked = handler.CodeHandler,
                    Command = new LambdaCommand(
                        "Toggle Code Handler", 
                        () => {
                            handler.CodeHandler = !handler.CodeHandler;
                        })
                });
                foreach (var handlerIn in handler.HandlerInputs)
                {
                    if (handlerIn.Item != null)
                    {
                        ui.AddCommand(new ContextMenuItem()
                        {
                            Title = "Navigate To " + handlerIn.Item.Name,
                            Group = "Navigation",
                            Command = new NavigateToNodeCommand()
                            {
                                Node = handlerIn.Item as IDiagramNode
                            }
                        });
                    }
                }
            }

            //var nodeViewModel = obj as SequenceItemNodeViewModel;
            //if (nodeViewModel != null)
            //{
            //    var node = nodeViewModel.SequenceNode;
            //    ui.AddCommand(new ContextMenuItem()
            //    {
            //        Title = "Move To Group",
            //        Command = new GroupActionNodes()
            //        {
            //            Node = node
            //        }
            //    });
            //}
            var diagramViewModel = obj as DiagramViewModel;

            if (diagramViewModel != null)
            {
                var contextVar = diagramViewModel.GraphData.CurrentFilter as IVariableContextProvider;
                if (contextVar != null)
                {


                    foreach (var item in contextVar.GetAllContextVariables())
                    {
                        var item1 = item;
                        foreach (var child in item.GetPropertyDescriptions())
                        {
                            var child1 = child;
                            ui.AddCommand(new ContextMenuItem()
                            {
                                Title = item1.ShortName + "/" + child1.ShortName,
                                Group = "Variables",
                                Command = new LambdaCommand("Add Variable",
                                    () =>
                                    {
                                        var node = new PropertyNode()
                                        {
                                            Graph = diagramViewModel.GraphData,
                                        };
                                        diagramViewModel.AddNode(node, evt.LastMousePosition).Collapsed = true;
                                        node.Object.SetInput(item1);
                                        node.PropertySelection.SetInput(child1);
                                        node.IsSelected = true;
                                    })
                            });
                        }
                    }
                  
                }
            }


        }

        private void QuerySlotMenu(ContextMenuUI ui, InputOutputViewModel slot)
        {

            //ui.AddCommand(new ContextMenuItem()
            //{
            //    Title = "Test",
            //    Command = new LambdaCommand("test", () =>
            //    {

            //        var variableIn = slot.DataObject as VariableIn;
            //        foreach (var item in variableIn.Inputs)
            //        {
            //            InvertApplication.Log(item.Input.Title);
            //            InvertApplication.Log(item.Output.Title);
            //        }
            //    })
            //});

        }



        public void QuickAccessItemsEvents(QuickAccessContext context, List<IItem> items)
        {

            //            if (context.ContextType == typeof (IInsertQuickAccessContext))
            //            {
            //                items.Clear();
            //                items.AddRange(QueryInsert(context));
            //            }
            //            if (context.ContextType == typeof (IConnectionQuickAccessContext))
            //            {
            //                if (InvertApplication.Container.Resolve<WorkspaceService>().CurrentWorkspace.CurrentGraph.CurrentFilter is HandlerNode)
            //                {
            //             
            //                    items.Clear();
            //                    items.AddRange(QueryConntectionActions(context));
            //                }
            //                
            //            }
        }

        private void QueryInsert(SelectionMenu menu)
        {
            var mousePosition = UnityEngine.Event.current.mousePosition;
            var currentGraph = InvertApplication.Container.Resolve<WorkspaceService>().CurrentWorkspace.CurrentGraph;
            var systemNode = currentGraph.CurrentFilter as SystemNode;
            if (systemNode != null)
            {
                var category = new SelectionMenuCategory()
                {
                    Title = "Events",
                    Expanded = true,
                    Description = "This category includes events exposed by ECS as well as any custom events."
                };

                menu.AddItem(category);
                foreach (var item in currentGraph.Repository.All<EventNode>())
                {
                    var item1 = item;
                    var qa = new SelectionMenuItem(item, () =>
                    {
                        
                        var eventNode = new HandlerNode()
                        {
                            MetaType = item1.Identifier,
                            Name = systemNode.Name + item1.Name
                        };
                        InvertGraphEditor.CurrentDiagramViewModel.AddNode(eventNode, LastMouseEvent != null ? LastMouseEvent.MousePosition : new Vector2(0, 0));
                    }) {Group = "Handlers"};
                    menu.AddItem(qa, category);
                }
                foreach (var item in Events)
                {
                    var item1 = item;
                    var qa = new SelectionMenuItem(item.Value, () =>
                    {
                        var eventNode = new HandlerNode()
                        {
                            Meta = item1.Value,
                            Name = systemNode.Name + item1.Value.Title
                        };
                        InvertGraphEditor.CurrentDiagramViewModel.AddNode(eventNode, LastMouseEvent != null ? LastMouseEvent.MousePosition : new Vector2(0, 0));
                    });
                    menu.AddItem(qa, category);
                }
            }
            if (currentGraph.CurrentFilter is SequenceItemNode)
            {
                var vm = InvertGraphEditor.CurrentDiagramViewModel;

                var category = new SelectionMenuCategory()
                {
                    Title = "ECS Variables"
                };

                menu.AddItem(category);

                menu.AddItem(new SelectionMenuItem("Set", "Set Variable", () => { vm.AddNode(new SetVariableNode(), vm.LastMouseEvent.LastMousePosition); }), category);

                menu.AddItem(new SelectionMenuItem("Create", "Bool Variable", () =>
                {
                    Execute(new CreateNodeCommand() { GraphData = vm.GraphData, Position = vm.LastMouseEvent.MouseDownPosition, NodeType = typeof(BoolNode) });

                }), category);
                menu.AddItem(new SelectionMenuItem("Create", "Vector2 Variable", () => { vm.AddNode(new Vector2Node(), vm.LastMouseEvent.LastMousePosition); }), category);
                menu.AddItem(new SelectionMenuItem("Create", "Vector3 Variable", () => { vm.AddNode(new Vector3Node(), vm.LastMouseEvent.LastMousePosition); }), category);
                menu.AddItem(new SelectionMenuItem("Create", "String Variable", () => { vm.AddNode(new StringNode(), vm.LastMouseEvent.LastMousePosition); }), category);
                menu.AddItem(new SelectionMenuItem("Create", "Float Variable", () => { vm.AddNode(new FloatNode(), vm.LastMouseEvent.LastMousePosition); }), category);
                menu.AddItem(new SelectionMenuItem("Create", "Integer Variable", () => { vm.AddNode(new IntNode(), vm.LastMouseEvent.LastMousePosition); }), category);
                menu.AddItem(new SelectionMenuItem("Create", "Literal", () => { vm.AddNode(new LiteralNode(), vm.LastMouseEvent.LastMousePosition); }), category);


                //var currentFilter = currentGraph.CurrentFilter as HandlerNode;
                //foreach (var item in currentFilter.GetAllContextVariables())
                //{
                //    var item1 = item;
                //    var qa = new QuickAccessItem("Variables", item.VariableName ?? "Unknown", _ =>
                //    {
                //        var command = new AddVariableReferenceCommand()
                //        {
                //            Variable = _ as IContextVariable,
                //            Handler = currentFilter,
                //            Position = mousePosition
                //        };
                //        // TODO 2.0 Add Variable Reference COmmand
                //        //InvertGraphEditor.ExecuteCommand(command);
                //    })
                //    {
                //        Item = item1
                //    };
                //    yield return qa;
                //}
                QueryActions(menu);
            }


        }
        private void QueryActions(SelectionMenu menu)
        {
            var mousePosition = UnityEngine.Event.current.mousePosition;
            var diagramViewModel = InvertGraphEditor.CurrentDiagramViewModel;
            GetActionsMenu(menu, _ =>
            {
                SequenceItemNode node = null;
                var type = _ as ActionMetaInfo;
                if (type != null && type.IsEditorClass)
                {
                    node = Activator.CreateInstance(type.SystemType) as SequenceItemNode;
                }
                else
                { 
                    node = new ActionNode
                    {
                        Meta = _,
                    };
                    //node.Name = "";
                }
                node.Graph = diagramViewModel.GraphData;
                diagramViewModel.AddNode(node, mousePosition);
                node.IsSelected = true;
                    
            });
        }

        private void GetActionsMenu(SelectionMenu menu, Action<IActionMetaInfo> onSelect)
        {

            foreach (var item in uFrameECS.Actions)
            {
                var item1 = item;
                if (item.Value.Category == null) continue;

                var category = menu.CreateCategoryIfNotExist(item.Value.CategoryPath.ToArray());
                category.Add(new SelectionMenuItem(item.Value, () =>
                {
                    onSelect(item1.Value);
                }));


            }

            foreach (var action in this.Container.Resolve<IRepository>().All<CustomActionNode>())
            {
                var action1 = action;
                menu.AddItem(new SelectionMenuItem(action, () =>
                {
                    onSelect(action1);
                }));
            }
            //var _categoryTitles = uFrameECS.Actions
            //    .Where(_ => _.Value.Category != null)
            //    .SelectMany(_ => _.Value.Category.Title)
            //    .Distinct();

            //foreach (var categoryTitle in _categoryTitles)
            //{
            //    var category = new SelectionMenuCategory()
            //    {
            //        Title = categoryTitle
            //    };
            //    menu.AddItem(category);
            //    var title = categoryTitle;

            //    foreach (
            //        var action in uFrameECS.Actions.Values.Where(_ => _.Category != null && _.Category.Title.Contains(title)))
            //    {
            //        var action1 = action;
            //        menu.AddItem(new SelectionMenuItem(action, () =>
            //        {
            //            onSelect(action1);
            //        }), category);
            //    }
            //}
            foreach (
                var action in uFrameECS.Actions.Values.Where(_ => _.Category == null))
            {
                var action1 = action;
                menu.AddItem(new SelectionMenuItem(action, () =>
                {
                    onSelect(action1);
                }));
            }
        }

        public void QueryPossibleConnections(SelectionMenu menu, DiagramViewModel diagramViewModel,
            ConnectorViewModel startConnector,
            Vector2 mousePosition)
        {
            var contextVar = startConnector.ConnectorFor.DataObject as IContextVariable;
            if (contextVar != null)
            {
                menu.Items.Clear();
                foreach (var item in contextVar.GetPropertyDescriptions())
                {
                    var item1 = item;
                    menu.AddItem(new SelectionMenuItem(contextVar.ShortName, item.ShortName, () =>
                    {
                        var node = new PropertyNode()
                        {
                            Graph = diagramViewModel.GraphData,
                        };
                        diagramViewModel.AddNode(node, mousePosition).Collapsed = true;
                        diagramViewModel.GraphData.AddConnection(startConnector.ConnectorFor.DataObject as IConnectable, node.Object);
                        node.PropertySelection.SetInput(item1);
                        node.IsSelected = true;
                    }));
                }
            }

            if (startConnector.ConnectorFor.DataObject is IVariableContextProvider)
            {
                menu.Items.Clear();
                GetActionsMenu(menu, _ =>
                {
                    SequenceItemNode node = null;
                    var type = _ as ActionMetaInfo;
                    if (type != null && type.IsEditorClass)
                    {
                        node = Activator.CreateInstance(type.SystemType) as SequenceItemNode;
                    }
                    else
                    {
                       
                        node = new ActionNode
                        {
                            Meta = _,
                        };
                        //node.Name = "";
                    }
                    node.Graph = diagramViewModel.GraphData;
                    diagramViewModel.AddNode(node, mousePosition);
                    diagramViewModel.GraphData.AddConnection(startConnector.ConnectorFor.DataObject as IConnectable, node);
                    node.IsSelected = true;
                    node.Name = "";
                });
            }

        }
        private IEnumerable<IItem> QueryConntectionActions(QuickAccessContext context)
        {
            var connectionHandler = context.Data as ConnectionHandler;
            var diagramViewModel = connectionHandler.DiagramViewModel;

            var category = new QuickAccessCategory()
            {
                Title = "Connections"
            };

            foreach (var item in Actions)
            {

                var qaItem = new QuickAccessItem(item.Value.CategoryPath.FirstOrDefault() ?? string.Empty, item.Value.Title, item.Value.Title, _ =>
                {
                    var actionInfo = _ as ActionMetaInfo;
                    var node = new ActionNode()
                    {
                        Meta = actionInfo
                    };
                    node.Graph = diagramViewModel.GraphData;


                    diagramViewModel.AddNode(node, context.MouseData.MouseUpPosition);
                    diagramViewModel.GraphData.AddConnection(connectionHandler.StartConnector.ConnectorFor.DataObject as IConnectable, node);
                    node.IsSelected = true;
                    node.Name = "";
                })
                {
                    Item = item.Value
                };
                category.Add(qaItem);
            }
            yield return category;
        }

        public void OnMouseDoubleClick(Drawer drawer, MouseEvent mouseEvent)
        {
            var d = drawer as DiagramDrawer;
            if (d != null)
            {
                // When we've clicked nothing
                if (d.DrawersAtMouse.Length < 1)
                {
                    LastMouseEvent = mouseEvent;
                    //                    InvertApplication.SignalEvent<IWindowsEvents>(_ =>
                    //                    {
                    //                        _.ShowWindow("QuickAccessWindowFactory", "Add Node", null, mouseEvent.LastMousePosition,
                    //                            new Vector2(500, 600));
                    //                    });

                    ShowQuickAccess(mouseEvent);

                }
                else
                {

                }
                var firstOrDefault = d.DrawersAtMouse.FirstOrDefault();
                if (firstOrDefault != null)
                {
                    var handlerVM = firstOrDefault.ViewModelObject as HandlerNodeViewModel;
                    if (handlerVM != null)
                    {
                        if (handlerVM.Handler.CodeHandler)
                        {
                            var config = InvertGraphEditor.Container.Resolve<IGraphConfiguration>();
                            var fileGenerators = InvertGraphEditor.GetAllFileGenerators(config, new[] { handlerVM.DataObject as IDataRecord }).ToArray();
                            var editableGenerator = fileGenerators.FirstOrDefault(p => p.Generators.Any(x => !x.AlwaysRegenerate));
                            if (editableGenerator != null)
                                InvertGraphEditor.Platform.OpenScriptFile(editableGenerator.AssetPath);
                        }
                    }
                }
            }


        }

        private void ShowQuickAccess(MouseEvent mouseEvent)
        {

            var menu = new SelectionMenu();

            QueryInsert(menu);

            InvertApplication.SignalEvent<IShowSelectionMenu>(_ => _.ShowSelectionMenu(menu, mouseEvent.LastMousePosition - mouseEvent.ContextScroll));

            //            InvertApplication.SignalEvent<IShowSelectionMenu>(_ => _.ShowSelectionMenu(new QuickAccessContext()
            //            {
            //                ContextType = typeof(IInsertQuickAccessContext),
            //                MouseData = mouseEvent
            //            }, mouseEvent.LastMousePosition));

        }

        public MouseEvent LastMouseEvent { get; set; }

        public static HashSet<Type> SystemTypes
        {
            get { return _types; }
        }

        public void Execute(AddSlotInputNodeCommand command)
        {
            //var referenceNode = new VariableReferenceNode()
            //{

            //    VariableId = command.Variable.Identifier,
            //    HandlerId = command.Handler.Identifier
            //};

            //command.DiagramViewModel.AddNode(referenceNode, command.Position);
            //var connectionData = command.DiagramViewModel.CurrentRepository.Create<ConnectionData>();
            //connectionData.InputIdentifier = command.Input.Identifier;
            //connectionData.OutputIdentifier = referenceNode.Identifier;
            //referenceNode.Name = command.Variable.VariableName;
        }

        public void Execute(NewModuleWorkspace command)
        {
            var repository = InvertApplication.Container.Resolve<IRepository>();
            var createWorkspaceCommand = new CreateWorkspaceCommand() { Name = command.Name, Title = command.Name };

            Execute(createWorkspaceCommand);


            var dataGraph = new DataGraph();
            var systemGraph = new SystemGraph();
            dataGraph.Name = command.Name + "Data";
            systemGraph.Name = command.Name + "System";

            repository.Add(dataGraph);
            repository.Add(systemGraph);

            createWorkspaceCommand.Result.AddGraph(dataGraph);
            createWorkspaceCommand.Result.AddGraph(systemGraph);
            createWorkspaceCommand.Result.CurrentGraphId = dataGraph.Identifier;

            Execute(new OpenWorkspaceCommand()
            {
                Workspace = createWorkspaceCommand.Result
            });



        }

        public void Execute(GroupActionNodes command)
        {

            List<IDiagramNodeItem> list = new List<IDiagramNodeItem>();
            GrabDependencies(list, command.Node);

            list.Add(command.Node);

            var groupNode = new ActionGroupNode();
            InvertGraphEditor.CurrentDiagramViewModel.AddNode(groupNode, command.Node.FilterLocation.Position);
            foreach (var item in list.OfType<GenericNode>())
            {
                item.FilterLocation.FilterId = groupNode.Identifier;
            }
            groupNode.IsSelected = true;
            groupNode.IsEditing = true;
            Container.Resolve<IRepository>().Add(groupNode);
        }

        public void GrabDependencies(List<IDiagramNodeItem> items, GraphNode node)
        {
            foreach (var item in node.GraphItems.OfType<IConnectable>())
            {

                foreach (var dependent in item.InputsFrom<IDiagramNodeItem>().Concat(item.OutputsTo<IDiagramNodeItem>()))
                {
                    if (items.Contains(dependent)) continue;
                    if (items.Contains(dependent.Node)) continue;

                    items.Add(dependent);
                    items.Add(dependent.Node);

                    GrabDependencies(items, dependent.Node);

                }

            }
        }

        public void QueryTypes(List<ITypeInfo> typeInfo)
        {
            foreach (var item in SystemTypes)
            {
                typeInfo.Add(new SystemTypeInfo(item));
            }
            foreach (var item in Events)
            {
                typeInfo.Add(item.Value);
            }
        }

        public void GetDocumentation(IDocumentationBuilder node)
        {
            
        }

        public void GetPages(List<DocumentationPage> rootPages)
        {
            foreach (var item in Actions)
            {
                rootPages.Add(new ActionPage() {MetaInfo = item.Value});
            }
        }
    }

    public class uFrameECSPage : DocumentationPage
    {
        
    }
    public class ActionPage : DocumentationPage
    {
        public IActionMetaInfo MetaInfo { get; set; }

        public override string Name
        {
            get { return MetaInfo.Title; }
        }

        public override void GetContent(IDocumentationBuilder _)
        {
            base.GetContent(_);
           
            _.Title3(string.Join(",",MetaInfo.CategoryPath.ToArray()));
            
            _.Break();
            _.Title2("Inputs");
            foreach (var item in MetaInfo.GetMembers().OfType<IActionFieldInfo>().Where(p=>p.DisplayType is In))
            {
                _.Title3(item.Name);
                PrintDescription(_, item);
            }
            _.Break();
            _.Title2("Outputs");
            foreach (var item in MetaInfo.GetMembers().OfType<IActionFieldInfo>().Where(p => p.DisplayType is Out && !p.MemberType.IsAssignableTo(new SystemTypeInfo(typeof(Action)))))
            {
                _.Title3(item.Name);
                PrintDescription(_, item);
            }
            _.Break();
            _.Title2("Branches");
            foreach (var item in MetaInfo.GetMembers().OfType<IActionFieldInfo>().Where(p => p.DisplayType is Out && p.MemberType.IsAssignableTo(new SystemTypeInfo(typeof(Action)))))
            {
                _.Title3(item.Name);
                PrintDescription(_, item);
            }
        }

        private static void PrintDescription(IDocumentationBuilder _, IActionFieldInfo item)
        {
            var actionDescription = item.GetAttribute<ActionDescription>();
            if (actionDescription != null)
            {
                _.Paragraph(actionDescription.Description);
            }
        }
    }
}
