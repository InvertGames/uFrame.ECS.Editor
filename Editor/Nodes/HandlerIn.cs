using System.Collections.Generic;
using System.Linq;
using Invert.Core.GraphDesigner;
using Invert.Data;

namespace Invert.uFrame.ECS
{
    public class EntityGroupIn : SelectionFor<IMappingsConnectable, HandlerInValue>, IFilterInput
    {
        public IMappingsConnectable FilterNode
        {
            get { return this.Item; }
        }

        public virtual string MappingId
        {
            get { return "EntityId"; }
        }

        public override string Name
        {
            get { return "Group"; }
            set { base.Name = value; }
        }

        public override string Title
        {
            get { return "Group"; }
        }

        public virtual string HandlerPropertyName
        {
            get { return Name; }
        }

        public override IEnumerable<IValueItem> GetAllowed()
        {
            return Repository.AllOf<IMappingsConnectable>().OfType<IValueItem>();
        }
        public override bool AllowInputs
        {
            get { return false; }
        }

    }

    public class HandlerIn : EntityGroupIn, IFilterInput
    {
    
        public override string MappingId
        {
            get { return EventFieldInfo.MemberName; }
        }

        public IMemberInfo EventFieldInfo { get; set; }
        public override string Title
        {
            get
            {
                return EventFieldInfo.MemberName;
            }
        }
        public override string Name
        {
            get { return EventFieldInfo.MemberName; }
            set { base.Name = value; }
        }

        public override string HandlerPropertyName
        {
            get { return Name; }
        }

    }

    public class HandlerInValue : InputSelectionValue
    {
        
    }
}