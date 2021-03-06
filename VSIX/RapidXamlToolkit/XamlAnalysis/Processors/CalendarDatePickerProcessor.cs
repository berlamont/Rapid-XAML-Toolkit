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
    public class CalendarDatePickerProcessor : XamlElementProcessor
    {
        public override void Process(string fileName, int offset, string xamlElement, string linePadding, ITextSnapshot snapshot, TagList tags, List<TagSuppression> suppressions = null)
        {
            var (uidExists, uid) = this.GetOrGenerateUid(xamlElement, Attributes.Header);

            var elementGuid = Guid.NewGuid();

            this.CheckForHardCodedAttribute(
                fileName,
                Elements.CalendarDatePicker,
                Attributes.Header,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringCalendarDatePickerHeaderMessage,
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
                Elements.CalendarDatePicker,
                Attributes.Description,
                AttributeType.InlineOrElement,
                StringRes.Info_XamlAnalysisHardcodedStringCalendarDatePickerDescriptionMessage,
                xamlElement,
                snapshot,
                offset,
                uidExists,
                uid,
                elementGuid,
                tags,
                suppressions);
        }
    }
}
