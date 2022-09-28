using System;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Ink.UnityIntegration {
    [Serializable]
    public class InkCompilerLog {
        private static Regex _errorRegex =
            new(
                @"(?<errorType>ERROR|WARNING|TODO):(?:\s(?:'(?<filename>[^']*)'\s)?line (?<lineNo>\d+):)?(?<message>.*)");

        public ErrorType type;
        public string content;
        public string fileName;
        public int lineNumber;

        public InkCompilerLog(ErrorType type, string content, string fileName, int lineNumber = -1) {
            this.type = type;
            this.content = content;
            this.fileName = fileName;
            this.lineNumber = lineNumber;
        }

        public static bool TryParse(string rawLog, out InkCompilerLog log) {
            var match = _errorRegex.Match(rawLog);
            if (match.Success) {
                var errorType = ErrorType.Author;
                string filename = null;
                var lineNo = -1;
                string message = null;

                var errorTypeCapture = match.Groups["errorType"];
                if (errorTypeCapture != null) {
                    var errorTypeStr = errorTypeCapture.Value;
                    if (errorTypeStr == "AUTHOR" || errorTypeStr == "TODO") errorType = ErrorType.Author;
                    else if (errorTypeStr == "WARNING") errorType = ErrorType.Warning;
                    else if (errorTypeStr == "ERROR") errorType = ErrorType.Error;
                    else Debug.LogWarning("Could not parse error type from " + errorTypeStr);
                }

                var filenameCapture = match.Groups["filename"];
                if (filenameCapture != null)
                    filename = filenameCapture.Value;

                var lineNoCapture = match.Groups["lineNo"];
                if (lineNoCapture != null)
                    lineNo = int.Parse(lineNoCapture.Value);

                var messageCapture = match.Groups["message"];
                if (messageCapture != null)
                    message = messageCapture.Value.Trim();
                log = new InkCompilerLog(errorType, message, filename, lineNo);
                return true;
            }

            Debug.LogWarning("Could not parse InkFileLog from log: " + rawLog);
            log = null;
            return false;
        }
    }
}