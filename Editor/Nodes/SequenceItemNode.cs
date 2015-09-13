using Invert.Core;
using Invert.Json;
using UnityEngine;

namespace Invert.uFrame.ECS
{
    using Invert.Core.GraphDesigner;
    using Invert.Data;
    using System.Collections.Generic;
    using System.Linq;

    public partial interface ISequenceItemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }

    public class SequenceItemNode : SequenceItemNodeBase, ICodeOutput
    {
        private string _variableName;

        public override bool AllowMultipleInputs
        {
            get { return false; }
        }

        public override bool AllowMultipleOutputs
        {
            get { return false; }
        }

        public override Color Color
        {
            get { return Color.blue; }
        }

        public IVariableContextProvider Left
        {
            get
            {
                var r = this.InputFrom<IVariableContextProvider>();
                return r;
            }
        }

        public IEnumerable<IVariableContextProvider> LeftNodes
        {
            get
            {
                var left = Left;
                while (left != null)
                {
                    yield return left;
                    left = left.Left;
                }
            }
        }

        public SequenceItemNode Right
        {
            get { return this.OutputTo<SequenceItemNode>(); }
        }

        public IEnumerable<IVariableContextProvider> RightNodes
        {
            get
            {
                var right = Right;
                while (right != null)
                {
                    yield return right;
                    right = right.Right;
                }
            }
        }

        [JsonProperty, InspectorProperty]
        public string VariableName
        {
            get {
                return _variableName ?? (_variableName = VariableNameProvider.GetNewVariableName(this.GetType().Name));
            }
            set { this.Changed("VariableName", ref _variableName, value); }
        }

        public IVariableNameProvider VariableNameProvider
        {
            get { return Graph as IVariableNameProvider; }
        }

        public IEnumerable<IContextVariable> GetAllContextVariables()
        {
            var left = Left;
            if (left != null)
            {
                foreach (var contextVar in left.GetAllContextVariables())
                {
                    yield return contextVar;
                }
            }
            foreach (var item in GetContextVariables())
            {
                yield return item;
            }
        }

        public virtual IEnumerable<IContextVariable> GetContextVariables()
        {
            yield break;
        }

        public override void RecordRemoved(IDataRecord record)
        {
            base.RecordRemoved(record);
            var container = this.Container();
            if (container == null || container.Identifier == record.Identifier)
            {
                Repository.Remove(this);
            }
        }

        public virtual void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            OutputVariables(ctx);
            ctx._comment(Name);
            foreach (var right in this.OutputsTo<SequenceItemNode>())
            {
                if (right != null)
                {
                    right.WriteCode(visitor, ctx);
                }
            }
        }

        protected void OutputVariables(TemplateContext ctx)
        {
            foreach (var item in GraphItems.OfType<IConnectable>())
            {
                var decl = item.InputFrom<VariableNode>();
                if (decl == null) continue;

                ctx.CurrentDeclaration.Members.Add(decl.GetFieldStatement());
            }
        }

    }
}