using System;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.ProjectSystem.VS;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ollon.VisualStudio.Extensibility.Utilities
{
    public static class IVsHierarchyExtensions
    {
        //public static T GetProperty<T>(this IVsHierarchy hierarchy, VsHierarchyPropID property, T defaultValue = default(T))
        //{
        //    return hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, property, defaultValue);
        //}

        ///// <summary>
        /////     Gets the value of the specified property if the hierarchy supports it.
        ///// </summary>
        //public static T GetProperty<T>(this IVsHierarchy hierarchy, uint item, VsHierarchyPropID property, T defaultValue = default(T))
        //{
        //    Microsoft.Requires.NotNull(hierarchy, nameof(hierarchy));

        //    if (item == VSConstants.VSITEMID_NIL || item == VSConstants.VSITEMID_SELECTION)
        //        throw new ArgumentException(null, nameof(item));
        //    HResult hr = hierarchy.GetProperty(item, (int)property, out object resultObject);
        //    if (hr == VSConstants.DISP_E_MEMBERNOTFOUND)
        //        return defaultValue;

        //    if (hr.Failed)
        //        throw hr.Exception;

        //    return (T)resultObject;
        //}

        public static bool IsCpsProject(this IVsHierarchy hierarchy)
        {
            return hierarchy.IsCapabilityMatch("CPS");
        }

        public static object GetProjectProperty(this IVsHierarchy hierarchy, HierarchyProperty property)
        {
            hierarchy.GetProperty(VSConstants.VSITEMID_ROOT, (int) property, out object rawValue);
            
            return rawValue;
        }
    }
}