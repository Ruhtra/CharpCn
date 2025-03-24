using Shadcn.Components.Priv.Clsx;
using TwMerge;

namespace Shadcn.Components.Priv.Cn
{
    public static class CnHelper
    {
        public static string Cn(string[] classes)
        {
            return new TwsMerge().Merge(ClsxHelper.Clsx(classes)) ?? "";
        }
    }
}
