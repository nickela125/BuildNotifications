using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Model.Helpers
{
    public static class TreeHelper
    {
        public static bool? GetChildrenSelectedState(IList<TreeItem> children)
        {
            bool allTrue = true;
            bool allFalse = true;

            if (children == null || !children.Any())
            {
                return false;
            }

            foreach (TreeItem child in children)
            {
                if (child.IsSelected == null)
                {
                    return null;
                }
                if (child.IsSelected.Value)
                {
                    allFalse = false;
                    if (allTrue == false)
                    {
                        return null;
                    }
                }
                else if (child.IsSelected.Value == false)
                {
                    allTrue = false;
                    if (allFalse == false)
                    {
                        return null;
                    }
                }
            }
            return allTrue;
        }
    }
}
