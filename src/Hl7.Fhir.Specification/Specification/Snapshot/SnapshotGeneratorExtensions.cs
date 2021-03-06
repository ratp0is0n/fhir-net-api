﻿/* 
 * Copyright (c) 2016, Furore (info@furore.com) and contributors
 * See the file CONTRIBUTORS for details.
 * 
 * This file is licensed under the BSD 3-Clause license
 * available at https://raw.githubusercontent.com/ewoutkramer/fhir-net-api/master/LICENSE
 */

using Hl7.Fhir.Model;
using Hl7.Fhir.Support;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hl7.Fhir.Specification.Snapshot
{
    // [WMR 20161219] OBSOLETE - Instead, use ConstrainedByDifferentialAnnotation

    /// <summary>Helper methods for the <see cref="SnapshotGenerator"/> class to generate and inspect custom extensions.</summary>
    public static class SnapshotGeneratorExtensions
    {
        /// <summary>The canonical url of the extension definition that marks snapshot elements with associated differential constraints.</summary>
        public static readonly string CHANGED_BY_DIFF_EXT = "http://hl7.org/fhir/StructureDefinition/changedByDifferential";

        /// <summary>Mark the snapshot element as changed by the differential.</summary>
        /// <param name="element">An <see cref="IExtendable"/> instance.</param>
        /// <param name="value">An optional boolean value (default <c>true</c>).</param>
        /// <remarks>Sets the <see cref="CHANGED_BY_DIFF_EXT"/> extension to store the boolean flag.</remarks>
        public static void SetChangedByDiff(this IExtendable element, bool value = true)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.SetBoolExtension(CHANGED_BY_DIFF_EXT, value);
        }

        /// <summary>Determines wether the snapshot element was marked as changed by the differential.</summary>
        /// <param name="element">An <see cref="IExtendable"/> instance.</param>
        /// <returns>A boolean value, or <c>null</c>.</returns>
        /// <remarks>Gets the boolean flag from the <see cref="CHANGED_BY_DIFF_EXT"/> extension, if it exists.</remarks>
        public static bool? GetChangedByDiff(this IExtendable element) => element.GetBoolExtension(CHANGED_BY_DIFF_EXT);

        /// <summary>Removes the <see cref="CHANGED_BY_DIFF_EXT"/> extension from the specified element.</summary>
        /// <param name="element">An <see cref="IExtendable"/> instance.</param>
        public static void RemoveChangedByDiff(this IExtendable element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.RemoveExtension(CHANGED_BY_DIFF_EXT);
        }

        /// <summary>Removes all instances of the <see cref="CHANGED_BY_DIFF_EXT"/> extension from the specified element and it's child elements, recursively.</summary>
        public static void RemoveAllChangedByDiff(this Element element)
        {
            if (element == null) { throw Error.ArgumentNull(nameof(element)); }
            element.RemoveChangedByDiff();
            foreach (var child in element.Children.OfType<Element>())
            {
                child.RemoveAllChangedByDiff();
            }
        }

        /// <summary>Removes all instances of the <see cref="CHANGED_BY_DIFF_EXT"/> extension from all the specified elements and their children, recursively.</summary>
        public static void RemoveAllChangedByDiff<T>(this IList<T> elements) where T : Element
        {
            if (elements == null) { throw Error.ArgumentNull(nameof(elements)); }
            foreach (var elem in elements)
            {
                elem.RemoveAllChangedByDiff();
            }
        }

        // ========== For internal use only ==========

        /// <summary>Removes a specific extension from the snapshot element definition and it's descendant elements, recursively.</summary>
        /// <param name="elemDef">An <see cref="ElementDefinition"/> instance.</param>
        /// <param name="uri">The canonical url of the extension.</param>
        internal static void ClearAllExtensions(this ElementDefinition elemDef, string uri)
        {
            if (elemDef != null)
            {
                ClearExtensions(elemDef, uri);
            }
        }

        static void ClearExtensions<T>(this IEnumerable<T> elements, string uri) where T : Base
        {
            if (elements != null)
            {
                foreach (var child in elements)
                {
                    ClearExtensions(child, uri);
                }
            }
        }

        static void ClearExtensions<T>(this T element, string uri) where T : Base
        {
            if (element != null)
            {
                ClearExtension(element as IExtendable, uri);
                ClearExtensions(element.Children, uri);
            }
        }

        static void ClearExtension(this IExtendable extendable, string uri)
        {
            extendable?.RemoveExtension(uri);
        }

    }
}
