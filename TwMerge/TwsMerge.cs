﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using LruCacheNet;

using TwMerge.Commom;
using TwMerge.Extensions;
using TwMerge.Models;

namespace TwMerge {

    /// <summary>
    /// A utility for merging conflicting Tailwind CSS classes.
    /// </summary>
    public partial class TwsMerge {
        private readonly TwMergeConfig _config;
        private readonly TwMergeContext _context;
        private readonly LruCache<string, string> _cache;

        /// <summary>
        /// Initializes a new instance of <see cref="TwMerge" /> with optional configuration options.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public TwsMerge(IOptions<TwMergeConfig>? options = null) {
            _config = options?.Value ?? TwMergeConfig.Default();
            _context = new TwMergeContext(_config);
            _cache = new LruCache<string, string>(_config.CacheSize);
        }

        /// <summary>
        /// Merges the given <paramref name="classNames"/>, resolving any conflicts if present.
        /// </summary>
        /// <param name="classNames">The collection of CSS classes to be merged.</param>
        /// <returns>A <see langword="string"/> of merged CSS classes.</returns>
        public string? Merge(params string?[] classNames) {
            var joinedClassNames = string.Join(' ', classNames.Where(x => !string.IsNullOrWhiteSpace(x)));

            if (string.IsNullOrEmpty(joinedClassNames)) {
                return null;
            }

            if (_cache.TryGetValue(joinedClassNames, out var cachedResult)) {
                return cachedResult;
            }

            var result = Merge(joinedClassNames);
            _cache.AddOrUpdate(joinedClassNames, result);

            return result;
        }

        private string Merge(string classList) {
            var classGroupsInConflict = new HashSet<string>();
            var classNames = ClassesSeparatorRegex.Split(classList.Trim());

            var result = new StringBuilder();

            for (var i = classNames.Length - 1; i >= 0; i--) {
                var originalClassName = classNames[i];

                (
                    var modifiers,
                    var hasImportantModifier,
                    var baseClassName,
                    var postfixModifierPosition,
                    var isExternal
                ) = _context.SplitModifiers(originalClassName);

                if (isExternal is true) {
                    ConcatenateClassNames(originalClassName);
                    continue;
                }

                var hasPostfixModifier = postfixModifierPosition.HasValue;
                var classGroupId = _context.GetClassGroupId(
                    hasPostfixModifier
                        ? baseClassName[..postfixModifierPosition!.Value]
                        : baseClassName
                );

                if (string.IsNullOrEmpty(classGroupId)) {
                    if (!hasPostfixModifier) {
                        // Not a Tailwind class
                        ConcatenateClassNames(originalClassName);
                        continue;
                    }

                    classGroupId = _context.GetClassGroupId(baseClassName);

                    if (string.IsNullOrEmpty(classGroupId)) {
                        // Not a Tailwind class
                        ConcatenateClassNames(originalClassName);
                        continue;
                    }

                    hasPostfixModifier = false;
                }

                var variantModifier = string.Join(':', _context.SortModifiers(modifiers));

                var modifierId = hasImportantModifier
                    ? variantModifier + Constants.ImportantModifier
                    : variantModifier;

                var classId = modifierId + classGroupId;

                if (!classGroupsInConflict.Add(classId)) {
                    // Tailwind class omitted due to conflict
                    continue;
                }

                var conflictingClassGroups = _context.GetConflictingClassGroupIds(classGroupId, hasPostfixModifier);
                if (conflictingClassGroups is { Length: > 0 }) {
                    foreach (var classGroup in conflictingClassGroups) {
                        classGroupsInConflict.Add(modifierId + classGroup);
                    }
                }

                // Tailwind class not in conflict
                ConcatenateClassNames(originalClassName);
            }

            return result.ToString();

            void ConcatenateClassNames(string originalClassName) {
                if (result.Length > 0) {
                    result.Insert(0, ' ');
                }

                result.Insert(0, originalClassName);
            }
        }

        //[GeneratedRegex(@"\s+")]
        //private static partial Regex ClassesSeparatorRegex();
        private static readonly Regex ClassesSeparatorRegex = new Regex(@"\s+");
    }
}
