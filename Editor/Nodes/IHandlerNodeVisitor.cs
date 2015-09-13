using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    public interface IHandlerNodeVisitor
    {
        void Visit(IDiagramNodeItem item);
        
    }
}