using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shadcn.TwMerge.Models;
internal readonly record struct ClassValidator(string ClassGroupId, Func<string, bool> Validator);

