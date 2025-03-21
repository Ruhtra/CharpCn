using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwMerge.Models {
    internal readonly record struct ClassModifiersInfo(
        ICollection<string> Modifiers,
        bool HasImportantModifier,
        string BaseClassName,
        int? PostfixModifierPosition,
        bool? IsExternal = null
    );
}
