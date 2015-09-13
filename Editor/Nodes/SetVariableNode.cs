namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class SetVariableNode : SetVariableNodeBase {
        private string _VariableInputSlotId;

        private string _ValueInputSlotId;

        private VariableIn _Variable;

        private ValueIn _Value;

        public override string Name
        {
            get { return "Set Variable"; }
            set { base.Name = value; }
        }

        [Invert.Json.JsonProperty()]
        public virtual string VariableInputSlotId
        {
            get
            {
                if (_VariableInputSlotId == null)
                {
                    _VariableInputSlotId = Guid.NewGuid().ToString();
                }
                return _VariableInputSlotId;
            }
            set
            {
                _VariableInputSlotId = value;
            }
        }

        [Invert.Json.JsonProperty()]
        public virtual string ValueInputSlotId
        {
            get
            {
                if (_ValueInputSlotId == null)
                {
                    _ValueInputSlotId = Guid.NewGuid().ToString();
                }
                return _ValueInputSlotId;
            }
            set
            {
                _ValueInputSlotId = value;
            }
        }


        public virtual VariableIn VariableInputSlot
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                if (_Variable != null)
                {
                    return _Variable;
                }
                return _Variable ?? (_Variable = new VariableIn()
                {
                    Repository = Repository, Node = this, Identifier = VariableInputSlotId,
                    Name="Variable",
                    DoesAllowInputs = true
                });
            }
        }

        public virtual ValueIn ValueInputSlot
        {
            get
            {
                if (Repository == null)
                {
                    return null;
                }
                if (_Value != null)
                {
                    return _Value;
                }
                return _Value ?? (_Value = new ValueIn()
                {
                    DoesAllowInputs = true, 
                    Node = this, 
                    Identifier = ValueInputSlotId, 
                    Name = "Value",
                    Variable = VariableInputSlot,
                    Repository = Repository
                });
            }
        }

        public override void Validate(List<ErrorInfo> errors)
        {
            base.Validate(errors);
            if (VariableInputSlot.Item == null || ValueInputSlot.Item == null)
            {
                errors.AddError("Variable and Value must be set.", this.Node);
                return;
            }
            if (!ValueInputSlot.Item.VariableType.IsAssignableTo(VariableInputSlot.Item.VariableType))
            {
                errors.AddError(string.Format("Variable types {0} and {1} do not match.", VariableInputSlot.Item.VariableType.FullName, ValueInputSlot.Item.VariableType.FullName),this.Node);
            }
        }

        public override IEnumerable<IGraphItem> GraphItems
        {
            get
            {
                
                yield return VariableInputSlot;
                if (VariableInputSlot.Item != null)
                {
                    yield return ValueInputSlot;
                }
                
            }
        }

        public override void WriteCode(IHandlerNodeVisitor visitor, TemplateContext ctx)
        {
            var ctxVariable = VariableInputSlot.Item;
            if (ctxVariable == null) return;

            ctx._("{0} = ({1}){2}", ctxVariable.VariableName, ctxVariable.VariableType.FullName,
                ValueInputSlot.VariableName);
        }
    }

    public class ValueIn : VariableIn
    {
        public VariableIn Variable { get; set; }

        public override ITypeInfo VariableType
        {
            get
            {
                return Variable.VariableType;
                return base.VariableType;
            }
        }
    }
    public partial interface ISetVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
