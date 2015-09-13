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
            foreach (var item in uFrameECS.Events.Select(p =>
            {
                return new SelectionMenuItem(p.Value, () =>
                {
                    command.Node.EventType = p.Value.Type.FullName;
                });
            })) selectionMenu.AddItem(item);

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
                    Command =
                     new ChangeHandlerEventCommand()
                     {
                         Node = handlerVM.HandlerNode
                     }
                });

            }
        }
    }
}