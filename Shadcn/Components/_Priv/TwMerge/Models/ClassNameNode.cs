using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shadcn.TwMerge.Commom;

namespace Shadcn.TwMerge.Models;
internal record ClassNameNode {
    internal string? ClassGroupId { get; set; }
    internal Dictionary<string, ClassNameNode>? Next { get; set; }
    internal List<ClassValidator>? Validators { get; set; }

    internal ClassNameNode AddNextNode(string className) {
        var current = this;
        var parts = className.Split(Constants.ClassNameSeparator, StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts) {
            current.Next ??= new Dictionary<string, ClassNameNode>(); //updated

            if (!current.Next.TryGetValue(part, out var next)) {
                next = new ClassNameNode();
                current.Next[part] = next;
            }

            current = next;
        }

        return current;
    }

    internal void AddValidator(Func<string, bool> validator, string classGroupId) {
        Validators ??= new List<ClassValidator>(); //updated
        Validators.Add(new ClassValidator(classGroupId, validator));
    }
}

