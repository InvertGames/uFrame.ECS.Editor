using System.Collections.Generic;
using System.Linq;
using Invert.Core;
using Invert.Core.GraphDesigner;
using Invert.IOC;

namespace Invert.uFrame.ECS
{

    public class ChangeHandlerEventCommand : Command
    {
        public HandlerNode Node;
    }
    public class ActionNodeSystem
        : DiagramPlugin
        , IExecuteCommand<ChangeHandlerEventCommand>
        , IContextMenuQuery
    {
        public override void Initialize(UFrameContainer container)
        {
            base.Initialize(container);

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

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, object obj)
        {
            var handlerVM = obj as HandlerNodeViewModel;
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

        public void QueryContextMenu(ContextMenuUI ui, MouseEvent evt, object obj)
        {
            var diagram = obj as DiagramViewModel;
            var node = obj as DiagramNodeViewModel;
            if (node != null)
            {
                ui.AddCommand(new ContextMenuItem()
                {
                    Title = "Pickup",
                    Group="CopyPaste",
                    Command = new PickupCommand(),
         
                });

            }
            if (diagram != null)
            {
                if (CopiedNodes.Count > 0)
                {
                    ui.AddCommand(new ContextMenuItem()
                    {
                        Title = "Drop",
                        Group = "CopyPaste",
                        Command = new DropCommand()
                    });
                }
            }
        }
    }
}