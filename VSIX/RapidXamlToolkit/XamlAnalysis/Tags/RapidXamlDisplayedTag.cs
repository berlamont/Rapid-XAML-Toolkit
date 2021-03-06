﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Newtonsoft.Json;
using RapidXamlToolkit.ErrorList;

namespace RapidXamlToolkit.XamlAnalysis.Tags
{
    public abstract class RapidXamlDisplayedTag : RapidXamlAdornmentTag, IRapidXamlErrorListTag
    {
        protected RapidXamlDisplayedTag(Span span, ITextSnapshot snapshot, string fileName, string errorCode, int line, int column, TagErrorType defaultErrorType)
            : base(span, snapshot, fileName)
        {
            this.ErrorCode = errorCode;
            this.Line = line;
            this.Column = column;
            this.DefaultErrorType = defaultErrorType;
        }

        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the message shown when the error row is expanded.
        /// </summary>
        public string ExtendedMessage { get; set; }

        public int Line { get; }

        public int Column { get; }

        public TagErrorType DefaultErrorType { get; }

        /// <summary>
        /// Gets the code shown in the error list. Also used as the file name in the help link.
        /// </summary>
        public string ErrorCode { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the tag is for something that should show in the Errors tab of the error list.
        /// This should never need setting.
        /// </summary>
        public bool IsInternalError { get; protected set; }

        public TagErrorType ConfiguredErrorType
        {
            get
            {
                if (this.TryGetConfiguredErrorType(this.ErrorCode, out TagErrorType configuredType))
                {
                    return configuredType;
                }
                else
                {
                    return this.DefaultErrorType;
                }
            }
        }

        private static Dictionary<string, (DateTime timeStamp, Dictionary<string, string> settings)> SettingsCache { get; }
            = new Dictionary<string, (DateTime timeStamp, Dictionary<string, string> settings)>();

        public bool TryGetConfiguredErrorType(string errorCode, out TagErrorType tagErrorType)
        {
            var proj = ProjectHelpers.Dte.Solution.GetProjectContainingFile(this.FileName);

            var settingsFile = Path.Combine(Path.GetDirectoryName(proj.FullName), "settings.xamlAnalysis");

            if (File.Exists(settingsFile))
            {
                Dictionary<string, string> settings = null;
                var fileTime = File.GetLastWriteTimeUtc(settingsFile);

                if (SettingsCache.ContainsKey(settingsFile))
                {
                    if (SettingsCache[settingsFile].timeStamp == fileTime)
                    {
                        settings = SettingsCache[settingsFile].settings;
                    }
                }

                if (settings == null)
                {
                    var json = File.ReadAllText(settingsFile);
                    settings = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }

                SettingsCache[settingsFile] = (fileTime, settings);

                if (settings.ContainsKey(errorCode))
                {
                    if (TagErrorTypeParser.TryParse(settings[errorCode], out tagErrorType))
                    {
                        return true;
                    }
                }
            }

            // Set to default if no override in file
            tagErrorType = this.DefaultErrorType;

            return false;
        }

        public override ITagSpan<IErrorTag> AsErrorTag()
        {
            var span = new SnapshotSpan(this.Snapshot, this.Span);
            return new TagSpan<IErrorTag>(span, new RapidXamlWarningAdornmentTag(this.ToolTip));
        }

        public ErrorRow AsErrorRow()
        {
            return new ErrorRow
            {
                ExtendedMessage = this.ExtendedMessage,
                Span = new SnapshotSpan(this.Snapshot, this.Span),
                Message = this.Description,
                ErrorCode = this.ErrorCode,
                IsInternalError = this.IsInternalError,
                ErrorType = this.ConfiguredErrorType,
            };
        }
    }
}
