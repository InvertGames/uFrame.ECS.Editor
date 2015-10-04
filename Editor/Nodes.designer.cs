// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 2.0.50727.1433
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------

namespace Invert.uFrame.ECS {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Invert.Core;
    using Invert.Core.GraphDesigner;
    
    
    public class CustomActionNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Outputs", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<OutputsChildItem> Outputs {
            get {
                return PersistedItems.OfType<OutputsChildItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Inputs", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<InputsChildItem> Inputs {
            get {
                return PersistedItems.OfType<InputsChildItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Branches", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<BranchesChildItem> Branches {
            get {
                return PersistedItems.OfType<BranchesChildItem>();
            }
        }
    }
    
    public partial interface ICustomActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class DataNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IDataConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class LibraryNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ILibraryConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class StringLiteralNodeBase : VariableNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IStringLiteralConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class PropertyNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IPropertyConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class StringNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IStringConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ActionGroupNodeBase : SequenceItemNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IActionGroupConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class BoolNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IBoolConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ModuleNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IModuleConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class StopTimerNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        private string _TimerInputSlotId;
        
        private Timer _Timer;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string TimerInputSlotId {
            get {
                if (_TimerInputSlotId == null) {
                    _TimerInputSlotId = Guid.NewGuid().ToString();
                }
                return _TimerInputSlotId;
            }
            set {
                _TimerInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Timer", false, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Timer TimerInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Timer != null) {
                    return _Timer;
                }
                return _Timer ?? (_Timer = new Timer() { Repository = Repository, Node = this, Identifier = TimerInputSlotId });
            }
        }
    }
    
    public partial interface IStopTimerConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class AllFalseNodeBase : BoolExpressionNode {
        
        private string _ExpressionsInputSlotId;
        
        private Expressions _Expressions;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string ExpressionsInputSlotId {
            get {
                if (_ExpressionsInputSlotId == null) {
                    _ExpressionsInputSlotId = Guid.NewGuid().ToString();
                }
                return _ExpressionsInputSlotId;
            }
            set {
                _ExpressionsInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Expressions", true, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Expressions ExpressionsInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Expressions != null) {
                    return _Expressions;
                }
                return _Expressions ?? (_Expressions = new Expressions() { Repository = Repository, Node = this, Identifier = ExpressionsInputSlotId });
            }
        }
    }
    
    public partial interface IAllFalseConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class CodeActionNodeBase : ActionNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ICodeActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class BoolExpressionNodeBase : Invert.Core.GraphDesigner.GenericNode, IExpressionsConnectable, IGroupConnectable {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IBoolExpressionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class FloatNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IFloatConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class UserMethodNodeBase : SequenceItemNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IUserMethodConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class LoopCollectionNodeBase : SequenceItemNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ILoopCollectionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class AnyFalseNodeBase : BoolExpressionNode {
        
        private string _ExpressionsInputSlotId;
        
        private Expressions _Expressions;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string ExpressionsInputSlotId {
            get {
                if (_ExpressionsInputSlotId == null) {
                    _ExpressionsInputSlotId = Guid.NewGuid().ToString();
                }
                return _ExpressionsInputSlotId;
            }
            set {
                _ExpressionsInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Expressions", true, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Expressions ExpressionsInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Expressions != null) {
                    return _Expressions;
                }
                return _Expressions ?? (_Expressions = new Expressions() { Repository = Repository, Node = this, Identifier = ExpressionsInputSlotId });
            }
        }
    }
    
    public partial interface IAnyFalseConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ComponentCreatedNodeBase : HandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IComponentCreatedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class SetVariableNodeBase : SequenceItemNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ISetVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class CollectionItemRemovedNodeBase : CollectionModifiedHandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ICollectionItemRemovedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class CollectionModifiedHandlerNodeBase : HandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ICollectionModifiedHandlerConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class PropertyChangedNodeBase : HandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IPropertyChangedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class VariableNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IVariableConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class GroupNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        public virtual System.Collections.Generic.IEnumerable<Invert.Core.IItem> PossibleRequire {
            get {
                return this.Repository.AllOf<IRequireConnectable>().Cast<IItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.ReferenceSection("Require", SectionVisibility.Always, false, false, typeof(IRequireConnectable), false, OrderIndex=0, HasPredefinedOptions=false, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<RequireReference> Require {
            get {
                return PersistedItems.OfType<RequireReference>();
            }
        }
    }
    
    public partial interface IGroupConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class Vector3NodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IVector3Connectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class EventNodeBase : Invert.Core.GraphDesigner.GenericInheritableNode, Invert.Core.GraphDesigner.IClassTypeNode {
        
        public virtual string ClassName {
            get {
                return this.Name;
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Properties", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<PropertiesChildItem> Properties {
            get {
                return PersistedItems.OfType<PropertiesChildItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Collections", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<CollectionsChildItem> Collections {
            get {
                return PersistedItems.OfType<CollectionsChildItem>();
            }
        }
    }
    
    public partial interface IEventConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class LiteralNodeBase : VariableNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ILiteralConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ComponentNodeBase : Invert.Core.GraphDesigner.GenericNode, Invert.Core.GraphDesigner.IClassTypeNode, IRequireConnectable, IComponentsConnectable {
        
        public virtual string ClassName {
            get {
                return this.Name;
            }
        }
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Properties", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<PropertiesChildItem> Properties {
            get {
                return PersistedItems.OfType<PropertiesChildItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.Section("Collections", SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual System.Collections.Generic.IEnumerable<CollectionsChildItem> Collections {
            get {
                return PersistedItems.OfType<CollectionsChildItem>();
            }
        }
    }
    
    public partial interface IComponentConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class IntNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IIntConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class CollectionItemAddedNodeBase : CollectionModifiedHandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ICollectionItemAddedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ComponentDestroyedNodeBase : HandlerNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IComponentDestroyedConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class Vector2NodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IVector2Connectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class AllTrueNodeBase : BoolExpressionNode {
        
        private string _ExpressionsInputSlotId;
        
        private Expressions _Expressions;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string ExpressionsInputSlotId {
            get {
                if (_ExpressionsInputSlotId == null) {
                    _ExpressionsInputSlotId = Guid.NewGuid().ToString();
                }
                return _ExpressionsInputSlotId;
            }
            set {
                _ExpressionsInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Expressions", true, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Expressions ExpressionsInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Expressions != null) {
                    return _Expressions;
                }
                return _Expressions ?? (_Expressions = new Expressions() { Repository = Repository, Node = this, Identifier = ExpressionsInputSlotId });
            }
        }
    }
    
    public partial interface IAllTrueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class StartTimerNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        private string _TimerInputSlotId;
        
        private Timer _Timer;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string TimerInputSlotId {
            get {
                if (_TimerInputSlotId == null) {
                    _TimerInputSlotId = Guid.NewGuid().ToString();
                }
                return _TimerInputSlotId;
            }
            set {
                _TimerInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Timer", false, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Timer TimerInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Timer != null) {
                    return _Timer;
                }
                return _Timer ?? (_Timer = new Timer() { Repository = Repository, Node = this, Identifier = TimerInputSlotId });
            }
        }
    }
    
    public partial interface IStartTimerConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ConditionNodeBase : BoolExpressionNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IConditionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ActionNodeBase : SequenceItemNode, IActionConnectable {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IActionConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class HandlerNodeBase : SequenceItemNode, ISequenceItemConnectable {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IHandlerConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class SystemNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        public virtual System.Collections.Generic.IEnumerable<Invert.Core.IItem> PossibleComponents {
            get {
                return this.Repository.AllOf<IComponentsConnectable>().Cast<IItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.ReferenceSection("Components", SectionVisibility.Always, false, false, typeof(IComponentsConnectable), false, OrderIndex=0, HasPredefinedOptions=false, IsNewRow=false)]
        public virtual System.Collections.Generic.IEnumerable<ComponentsReference> Components {
            get {
                return PersistedItems.OfType<ComponentsReference>();
            }
        }
    }
    
    public partial interface ISystemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class EntityNodeBase : Invert.Core.GraphDesigner.GenericNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        public virtual System.Collections.Generic.IEnumerable<Invert.Core.IItem> PossibleComponents {
            get {
                return this.Repository.AllOf<IComponentsConnectable>().Cast<IItem>();
            }
        }
        
        [Invert.Core.GraphDesigner.ReferenceSection("Components", SectionVisibility.Always, false, false, typeof(IComponentsConnectable), false, OrderIndex=0, HasPredefinedOptions=false, IsNewRow=false)]
        public virtual System.Collections.Generic.IEnumerable<ComponentsReference> Components {
            get {
                return PersistedItems.OfType<ComponentsReference>();
            }
        }
    }
    
    public partial interface IEntityConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class ColorNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IColorConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class EnumValueNodeBase : LiteralNode {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface IEnumValueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class AnyTrueNodeBase : BoolExpressionNode {
        
        private string _ExpressionsInputSlotId;
        
        private Expressions _Expressions;
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
        
        [Invert.Json.JsonProperty()]
        public virtual string ExpressionsInputSlotId {
            get {
                if (_ExpressionsInputSlotId == null) {
                    _ExpressionsInputSlotId = Guid.NewGuid().ToString();
                }
                return _ExpressionsInputSlotId;
            }
            set {
                _ExpressionsInputSlotId = value;
            }
        }
        
        [Invert.Core.GraphDesigner.InputSlot("Expressions", true, SectionVisibility.Always, OrderIndex=0, IsNewRow=true)]
        public virtual Expressions ExpressionsInputSlot {
            get {
                if (Repository == null) {
                    return null;
                }
                if (_Expressions != null) {
                    return _Expressions;
                }
                return _Expressions ?? (_Expressions = new Expressions() { Repository = Repository, Node = this, Identifier = ExpressionsInputSlotId });
            }
        }
    }
    
    public partial interface IAnyTrueConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
    
    public class SequenceItemNodeBase : Invert.Core.GraphDesigner.GenericNode, ISequenceItemConnectable {
        
        public override bool AllowMultipleInputs {
            get {
                return true;
            }
        }
        
        public override bool AllowMultipleOutputs {
            get {
                return true;
            }
        }
    }
    
    public partial interface ISequenceItemConnectable : Invert.Core.GraphDesigner.IDiagramNodeItem, Invert.Core.GraphDesigner.IConnectable {
    }
}
