using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core.GraphDesigner;
    using Invert.Data;

    public class InputsChildItem : InputsChildItemBase, IActionFieldInfo
    {
        public bool IsGenericArgument { get { return false; } }
        public bool IsReturn { get { return false; } }
        public bool IsByRef { get { return false; } }
        public FieldDisplayTypeAttribute DisplayType { get { return new In(MemberName); } }
        public bool IsBranch { get { return false; } }

        private bool _isOptional;

        public bool IsOptional
        {
            get { return _isOptional; }
            set { this.Changed("IsOptional", ref _isOptional, value); }
        }

    }

    public partial interface IInputsConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable
    {
    }
}
