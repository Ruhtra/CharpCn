namespace Shadcn.Components.Priv.Cn;

using Shadcn.Components.Priv.Clsx;
using Shadcn.TwMerge;


public static class CnHelper {
    public static string Cn(string[] classes) {
        return new TwsMerge().Merge(ClsxHelper.Clsx(classes)) ?? "";
    }
}

