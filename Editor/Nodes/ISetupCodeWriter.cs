using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS
{
    public interface ISetupCodeWriter
    {
        void WriteSetupCode(IHandlerNodeVisitor visitor, TemplateContext ctx);
    }
}