using Invert.Core.GraphDesigner;

namespace Invert.uFrame.ECS.Templates
{
    public partial class GroupItemTemplate
    {
        [ForEach("SelectComponents"),GenerateProperty, WithField]
        public _ITEMTYPE_ _Name_ { get; set; }

       
        [GenerateProperty]
        public int ComponentID
        {
            get
            {
                Ctx._("return {0}", ComponentTemplate._ComponentIds++);
                return 0;
            }
        }
    }
}