using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.IOC;
using Invert.Json;
using UnityEditor;
using UnityEngine;

namespace Invert.uFrame.ECS
{

    public class ChangeHandlerEventCommand : Command
    {
        public HandlerNode Node;
    }

    public class CreateConverterConnectionCommand : Command
    {
        public IContextVariable Output;
        public IActionIn Input;
        public ActionMethodMetaInfo ConverterAction;

    }
    public class ActionNodeSystem
        : DiagramPlugin
        , IExecuteCommand<ChangeHandlerEventCommand>
        , IContextMenuQuery
        , IExecuteCommand<CreateConverterConnectionCommand>

    {
        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);
            container.RegisterInstance<IConnectionStrategy>(new ConvertConnectionStrategy(), "ConvertConnectionStrategy");
        }

        public void Execute(ChangeHandlerEventCommand command)
        {
            var selectionMenu = new SelectionMenu();
            foreach (var item in uFrameECS.Events)
            {
                var item1 = item;
                selectionMenu.AddItem(new SelectionMenuItem(item.Value, () =>
                {
                    command.Node.MetaType = item1.Value.FullName;
                }));
            }


            Signal<IShowSelectionMenu>(_ => _.ShowSelectionMenu(selectionMenu));
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
            var handlerVM = obj.FirstOrDefault() as HandlerNodeViewModel;
            if (handlerVM != null)
            {
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Change Event",
                    Group = "Events",
                    Command =
                     new ChangeHandlerEventCommand()
                     {
                         Node = handlerVM.HandlerNode
                     }
                });

            }
        }

        public void Execute(CreateConverterConnectionCommand command)
        {
            SequenceItemNode node = null;
            var type = command.ConverterAction;
            if (type != null && type.IsEditorClass)
            {
                node = Activator.CreateInstance(type.SystemType) as SequenceItemNode;
            }
            else
            {
                node = new ActionNode
                {
                    Meta = type,
                };
                //node.Name = "";
            }
            node.Graph = command.Output.Graph;

            
            InvertGraphEditor.CurrentDiagramViewModel.AddNode(node, Event.current.mousePosition - new Vector2(250f,0f));

            var result = node.GraphItems.OfType<IActionOut>().FirstOrDefault(p => p.Name == "Result");
            var input = node.GraphItems.OfType<IActionIn>().FirstOrDefault();

            node.Graph.AddConnection(command.Output, input);
            node.Graph.AddConnection(result, command.Input);

            node.IsSelected = true;
        }
    }

    public class ConvertConnectionStrategy : DefaultConnectionStrategy<IContextVariable, IActionIn>
    {
        public override Color ConnectionColor
        {
            get { return Color.white; }
        }

        public override void Remove(ConnectorViewModel output, ConnectorViewModel input)
        {

        }

        protected override bool CanConnect(IContextVariable output, IActionIn input)
        {
            foreach (var p in uFrameECS.Converters)
            {
                if (output.VariableType.IsAssignableTo(p.ActionFields.First(x => !x.IsReturn).MemberType))
                {
                    if (p.ActionFields.First(x => x.IsReturn).MemberType.FullName == input.VariableType.FullName)
                        return true;
                }
                
                   
            }
            return false;

            return true;
        }

        //public override ConnectionViewModel Connect(DiagramViewModel diagramViewModel, ConnectorViewModel a, ConnectorViewModel b)
        //{
        //    //var converter = uFrameECS.Converters.FirstOrDefault(p =>
        //    //  p.ActionFields.First(x => !x.IsReturn).MemberType.IsAssignableTo(output.VariableType) &&
        //    //  p.ActionFields.First(x => x.IsReturn).MemberType.FullName == input.GetType().FullName
        //    //  );
        //    foreach (var p in uFrameECS.Converters)
        //    {
        //        //if (p.ActionFields.First(x => !x.IsReturn).MemberType.IsAssignableTo(output.VariableType))
        //        //{
        //        //    if (p.ActionFields.First(x => x.IsReturn).MemberType.FullName == input.GetType().FullName)
        //        //    {
        //        //        CreateConnection(diagramViewModel, a, b, Apply);
        //        //    }
        //        //}


        //    }
        //}
        public override void Apply(ConnectionViewModel connectionViewModel)
        {
            base.Apply(connectionViewModel);
        }

        protected override void ApplyConnection(IGraphData graph, IConnectable output, IConnectable input)
        {
            //base.ApplyConnection(graph, output, input);
            ApplyConnection(graph, output as IContextVariable,input as IActionIn);
        }

        protected override void ApplyConnection(IGraphData graph, IContextVariable output, IActionIn input)
        {
            base.ApplyConnection(graph, output, input);
           
            var converter = uFrameECS.Converters.FirstOrDefault(p =>
               output.VariableType.IsAssignableTo(p.ConvertFrom.MemberType) &&
               p.ConvertTo.MemberType.FullName == input.VariableType.FullName
               );
            if (converter != null)
            {
                InvertApplication.Execute(new CreateConverterConnectionCommand()
                {
                    ConverterAction = converter,
                    Input = input,
                    Output = output
                });
            }
        }
    }
    public class PickupCommand : Command
    {

    }

    public class DropCommand : Command
    {

    }
    public class CutPasteSystem : DiagramPlugin,
        IExecuteCommand<PickupCommand>,
        IExecuteCommand<DropCommand>,
        IExecuteCommand<PasteCommand>,
        IToolbarQuery,
        IContextMenuQuery
    {
        private List<IFilterItem> _copiedNodes;

        [Inject]
        public WorkspaceService WorkspaceService { get; set; }


        public List<IFilterItem> CopiedNodes
        {
            get { return _copiedNodes ?? (_copiedNodes = new List<IFilterItem>()); }
            set { _copiedNodes = value; }
        }


        public IEnumerable<IFilterItem> SelectedNodes
        {
            get
            {
                if (WorkspaceService == null) yield break;
                if (WorkspaceService.CurrentWorkspace == null) yield break;
                if (WorkspaceService.CurrentWorkspace.CurrentGraph == null) yield break;

                foreach (var item in WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter.FilterItems.Where(p => p.Node.IsSelected))
                {
                    yield return item;
                }
            }
        }

        public void Execute(PickupCommand command)
        {
            CopiedNodes.Clear();
            CopiedNodes.AddRange(SelectedNodes);
            Signal<INotify>(_ => _.Notify("Now navigate to the target graph and press paste.", NotificationIcon.Info));
        }

        public void Execute(DropCommand command)
        {
            foreach (var item in CopiedNodes)
            {
                item.FilterId = WorkspaceService.CurrentWorkspace.CurrentGraph.CurrentFilter.Identifier;
            }
        }

        public void QueryToolbarCommands(ToolbarUI ui)
        {

            //ui.AddCommand(new ToolbarItem()
            //{
            //    Title = "Pickup",
            //    Command = new PickupCommand(),
            //    Position = ToolbarPosition.Right,
            //});

            //if (CopiedNodes.Count > 0)
            //{
            //    ui.AddCommand(new ToolbarItem()
            //    {
            //        Title = "Drop",
            //        Command = new DropCommand()
            //    });
            //}
        }

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, params object[] obj)
        {
            var diagram = obj.FirstOrDefault() as DiagramViewModel;
            var node = obj.FirstOrDefault() as DiagramNodeViewModel;
            if (node != null)
            {
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Pickup",
                    Group="CopyPaste",
                    Command = new PickupCommand(),
         
                });
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Copy",
                    Group = "CopyPaste",
                    Command = new PickupCommand(),

                });
           
            }
            if (diagram != null)
            {
                if (CopiedNodes.Count > 0)
                {
                    //ui.AddCommand(new ContextMenuItem()
                    //{
                    //    Title = "Drop",
                    //    Group = "CopyPaste",
                    //    Command = new DropCommand()
                    //});
                    ui.AddCommand(new ContextMenuItem()
                    {
                        Title = "Paste",
                        Group = "CopyPaste",
                        Command = new PasteCommand() { Position =evt.MouseDownPosition }
                        
                    });
                }
              
            }
        }
        
        public void Execute(PasteCommand command)
        {
            var copiedNodes = CopiedNodes.ToArray();
            foreach (var item in copiedNodes)
            {
                var filter = item as IGraphFilter;
                if (filter != null)
                {
                    CopiedNodes.AddRange(filter.FilterItems.Where(p => p.Node != item));
                }
            } 
            var offset = command.Position - CopiedNodes.Last().Position;
            foreach (var item in CopiedNodes)
            {
                
                var node = item.Node;
                var repository = node.Repository;
                var nodeJson = InvertJsonExtensions.SerializeObject(node);
                var copiedNode = InvertJsonExtensions.DeserializeObject(node.GetType(), nodeJson) as GraphNode;
                copiedNode.Identifier = Guid.NewGuid().ToString();
                copiedNode.Name += "_Copy";
                copiedNode.Graph = InvertGraphEditor.CurrentDiagramViewModel.GraphData;
                repository.Add(copiedNode);

                foreach (var child in node.PersistedItems.ToArray())
                {
                    if (child == node) continue;
                    var childJson = InvertJsonExtensions.SerializeObject(child);
                    var copiedChild = InvertJsonExtensions.DeserializeObject(child.GetType(), childJson) as IDiagramNodeItem;
                    copiedChild.Identifier = Guid.NewGuid().ToString();
                    copiedChild.Node = copiedNode;
                    repository.Add(copiedChild);
                }

          
                InvertGraphEditor.CurrentDiagramViewModel.GraphData.CurrentFilter.ShowInFilter(copiedNode,
                    item.Position + offset);

               
            }

        }
    }


    public class CopyCommand : Command
    {
        
    }

    public class PasteCommand : Command
    {
        public Vector2 Position { get; set; }
    }
}