using System;
using System.Collections.Generic;
using System.Linq;
using Invert.Core;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    public class EventMetaInfo : IItem
    {
        private List<EventFieldInfo> _members;
        private uFrameCategory _categoryAttribute;

        public Type Type { get; set; }
        public uFrameEvent Attribute { get; set; }

        public uFrameCategory CategoryAttribute
        {
            get { return _categoryAttribute ?? (_categoryAttribute = Type.GetCustomAttributes(typeof(uFrameCategory), true).OfType<uFrameCategory>().FirstOrDefault()); }
            set { _categoryAttribute = value; }
        }

        public string Category
        {
            get
            {
                if (CategoryAttribute != null)
                {
                    return CategoryAttribute.Title.FirstOrDefault() ?? "Listen For";
                }
                return "Listen For";
            }
        }
        public bool Dispatcher
        {
            get { return Attribute is UFrameEventDispatcher; }
        }

        public bool SystemEvent
        {
            get { return Attribute is SystemUFrameEvent; }
        }

        public List<EventFieldInfo> Members
        {
            get { return _members ?? (_members = new List<EventFieldInfo>()); }
            set { _members = value; }
        }

        public string SystemEventMethod
        {
            get { return (Attribute as SystemUFrameEvent).SystemMethodName; }
        }

        public IHandlerCodeWriter CodeWriter { get; set; }

        public string Title
        {
            get
            {
                if (SystemEvent) return (Attribute as SystemUFrameEvent).Title;
                return Type.Name;
            }
        }

        public string Group
        {
            get { return Category; }
        }

        public string SearchTag
        {
            get { return Title + Category; }
        }

        public string Description
        {
            get { return ""; }
            set { }
        }

    }
}