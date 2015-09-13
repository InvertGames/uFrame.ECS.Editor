using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Invert.Core;
using uFrame.Attributes;

namespace Invert.uFrame.ECS
{
    public class ActionMetaInfo : IItem
    {
        private ActionDescription _description;
        private ActionTitle _title;
        private List<ActionFieldInfo> _actionFields;
        private uFrameCategory _category;
        private string _searchTag;
        public Type Type { get; set; }
        public MethodInfo Method { get; set; }
        public ActionTitle Title
        {
            get { return _title ?? (_title = MetaAttributes.OfType<ActionTitle>().FirstOrDefault()); }
            set { _title = value; }
        }

        public string Group { get; private set; }

        public string SearchTag
        {
            get { return _searchTag ?? (_searchTag = TitleText + string.Join(" ", CategoryPath.ToArray())); }
        }

        string IItem.Description { get; set; }

        public string FullName
        {
            get
            {
                if (Method == null)
                    return Type.FullName;
                return Type.FullName + "." + Method.Name;
            }
        }
        public string TitleText
        {
            get
            {
                if (Title == null && Method != null)
                    return Type.Name + " " + Method.Name;

                if (Title == null)
                    return Type.Name;

                return Title.Title;
            }
        }

        public string DescriptionText
        {
            get
            {
                if (Description == null)
                {
                    return "No Description Specified";
                }
                return Description.Description;
            }
        }

        string IItem.Title
        {
            get { return TitleText; }
        }

        public ActionDescription Description
        {
            get { return _description ?? (_description = MetaAttributes.OfType<ActionDescription>().FirstOrDefault()); }
            set { _description = value; }
        }
        public uFrameCategory Category
        {
            get { return _category ?? (_category = Type.GetCustomAttributes(typeof(uFrameCategory), true).OfType<uFrameCategory>().FirstOrDefault()); }
            set { _category = value; }
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
    }
}