﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Text;
using RapidXamlToolkit.Resources;
using RapidXamlToolkit.XamlAnalysis.Actions;
using RapidXamlToolkit.XamlAnalysis.Tags;

namespace RapidXamlToolkit.XamlAnalysis.Processors
{
    public class TextBoxProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            var elementGuid = Guid.NewGuid();

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBox,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxHeaderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.TextBox,
                Attributes.PlaceholderText,
                AttributeType.Inline | AttributeType.Element,
                StringRes.Info_XamlAnalysisHardcodedStringTextboxPlaceholderMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);

            if (!this.TryGetAttribute(xamlElement, Attributes.InputScope, AttributeType.Inline | AttributeType.Element, out _, out _, out _, out _))
            {
                var line = snapshot.GetLineFromPosition(offset);
                var col = offset - line.Start.Position;

                tags.TryAdd(
                    new AddTextBoxInputScopeTag(new Span(offset, xamlElement.Length), snapshot, fileName, line.LineNumber, col)
                    {
                        InsertPosition = offset,
                    },
                    xamlElement,
                    suppressions);
            }
        }
    }
}
