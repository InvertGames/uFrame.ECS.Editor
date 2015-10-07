using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invert.Core;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    public interface IActionMetaInfo : ITypeInfo
    {
        IEnumerable<string> CategoryPath { get; }
        bool IsEditorClass { get; set; }
        uFrameCategory Category { get; set; }
        ActionDescription DescriptionAttribute {get;set;}
        bool IsAsync { get; }
    }

    public class ActionMetaInfo : SystemTypeInfo, IItem, IActionMetaInfo
    {
        private ActionDescription _description;
        private ActionTitle _title;
        private List<ActionFieldInfo> _actionFields;
        private uFrameCategory _category;

   
        public ActionTitle TitleAttribute
        {
            get { return _title ?? (_title = MetaAttributes.OfType<ActionTitle>().FirstOrDefault()); }
            set { _title = value; }
        }


        //public virtual string TitleText 
        //{
        //    get
        //    {
        //        if (TitleAttribute == null)
        //            return SystemType.Name;

        //        return TitleAttribute.Title;
        //    }
        //}

        //public string DescriptionText
        //{
        //    get
        //    {
        //        if (Description == null)
        //        {
        //            return "No Description Specified";
        //        }
        //        return Description.Description;
        //    }
        //}
        
        public override string Title
        {
            get
            {
                if (TitleAttribute == null)
                    return SystemType.Name;

                return TitleAttribute.Title;
            }
        }


        public ActionDescription DescriptionAttribute
        {
            get { return _description ?? (_description = MetaAttributes.OfType<ActionDescription>().FirstOrDefault()); }
            set { _description = value; }
        }
        public uFrameCategory Category
        {
            get { return _category ?? (_category = SystemType.GetCustomAttributes(typeof(uFrameCategory), true).OfType<uFrameCategory>().FirstOrDefault()); }
            set { _category = value; }
        }

        public override string Description
        {
            get { return DescriptionAttribute != null ? DescriptionAttribute.Description : null; }
        }

        public bool IsAsync
        {
            get
            {
                if (MetaAttributes == null) return false;
                return MetaAttributes.OfType<AsyncAction>().Any();
            }
        }

        public IEnumerable<string> CategoryPath
        {
            get
            {
                if (Category == null)
                {
                    yield break;
                }
                foreach (var item in Category.Title)
                {
                    yield return item;
                }
            }
        }
        public List<ActionFieldInfo> ActionFields
        {
            get { return _actionFields ?? (_actionFields = new List<ActionFieldInfo>()); }
            set { _actionFields = value; }
        }

        public ActionMetaAttribute[] MetaAttributes { get; set; }
        public bool IsEditorClass { get; set; }

        public ActionMetaInfo(Type systemType) : base(systemType)
        {
        }

        public ActionMetaInfo(Type systemType, ITypeInfo other) : base(systemType, other)
        {
        }

        public override IEnumerable<IMemberInfo> GetMembers()
        {
            return ActionFields.OfType<IMemberInfo>();
        }
    }

    public class ActionMethodMetaInfo : ActionMetaInfo
    {
        private string _title1;
        public MethodInfo Method { get; set; }

        public override string Title
        {
            get { 
                return TitleAttribute == null ? Method.Name : TitleAttribute.Title ;
            }
        }

        public override string FullName
        {
            get { return base.FullName + "." + Method.Name; }
        }
        
        //public MethodInfo Method { get; set; }
        public ActionMethodMetaInfo(Type systemType) : base(systemType)
        {
        }

        public ActionMethodMetaInfo(Type systemType, ITypeInfo other) : base(systemType, other)
        {
        }
    }
}