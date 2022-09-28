using System;
using System.Collections.Generic;
using Ink.Parsed;
using Ink.Runtime;
using Divert = Ink.Parsed.Divert;
using Object = Ink.Parsed.Object;
using Path = Ink.Runtime.Path;
using Story = Ink.Runtime.Story;
using VariableAssignment = Ink.Parsed.VariableAssignment;

namespace Ink {
    public class Compiler {
        private readonly List<DebugSourceRange> _debugSourceRanges = new();

        private bool _hadParseError;

        private readonly string _inputString;
        private readonly Options _options;


        private InkParser _parser;

        private readonly PluginManager _pluginManager;
        private Story _runtimeStory;

        public Compiler(string inkSource, Options options = null) {
            _inputString = inkSource;
            _options = options ?? new Options();
            if (_options.pluginNames != null)
                _pluginManager = new PluginManager(_options.pluginNames);
        }

        public Parsed.Story parsedStory { get; private set; }

        public Parsed.Story Parse() {
            _parser = new InkParser(_inputString, _options.sourceFilename, OnParseError, _options.fileHandler);
            parsedStory = _parser.Parse();
            return parsedStory;
        }

        public Story Compile() {
            Parse();

            if (_pluginManager != null)
                _pluginManager.PostParse(parsedStory);

            if (parsedStory != null && !_hadParseError) {
                parsedStory.countAllVisits = _options.countAllVisits;

                _runtimeStory = parsedStory.ExportRuntime(_options.errorHandler);

                if (_pluginManager != null)
                    _pluginManager.PostExport(parsedStory, _runtimeStory);
            }
            else {
                _runtimeStory = null;
            }

            return _runtimeStory;
        }

        public CommandLineInputResult HandleInput(CommandLineInput inputResult) {
            var result = new CommandLineInputResult();

            // Request for debug source line number
            if (inputResult.debugSource != null) {
                var offset = (int)inputResult.debugSource;
                var dm = DebugMetadataForContentAtOffset(offset);
                if (dm != null)
                    result.output = "DebugSource: " + dm;
                else
                    result.output = "DebugSource: Unknown source";
            }

            // Request for runtime path lookup (to line number)
            else if (inputResult.debugPathLookup != null) {
                var pathStr = inputResult.debugPathLookup;
                var contentResult = _runtimeStory.ContentAtPath(new Path(pathStr));
                var dm = contentResult.obj.debugMetadata;
                if (dm != null)
                    result.output = "DebugSource: " + dm;
                else
                    result.output = "DebugSource: Unknown source";
            }

            // User entered some ink
            else if (inputResult.userImmediateModeStatement != null) {
                var parsedObj = inputResult.userImmediateModeStatement as Object;
                return ExecuteImmediateStatement(parsedObj);
            }
            else {
                return null;
            }

            return result;
        }

        private CommandLineInputResult ExecuteImmediateStatement(Object parsedObj) {
            var result = new CommandLineInputResult();

            // Variable assignment: create in Parsed.Story as well as the Runtime.Story
            // so that we don't get an error message during reference resolution
            if (parsedObj is VariableAssignment) {
                var varAssign = (VariableAssignment)parsedObj;
                if (varAssign.isNewTemporaryDeclaration) parsedStory.TryAddNewVariableDeclaration(varAssign);
            }

            parsedObj.parent = parsedStory;
            var runtimeObj = parsedObj.runtimeObject;

            parsedObj.ResolveReferences(parsedStory);

            if (!parsedStory.hadError) {
                // Divert
                if (parsedObj is Divert) {
                    var parsedDivert = parsedObj as Divert;
                    result.divertedPath = parsedDivert.runtimeDivert.targetPath.ToString();
                }

                // Expression or variable assignment
                else if (parsedObj is Expression || parsedObj is VariableAssignment) {
                    var evalResult = _runtimeStory.EvaluateExpression((Container)runtimeObj);
                    if (evalResult != null) result.output = evalResult.ToString();
                }
            }
            else {
                parsedStory.ResetError();
            }

            return result;
        }

        public void RetrieveDebugSourceForLatestContent() {
            foreach (var outputObj in _runtimeStory.state.outputStream) {
                var textContent = outputObj as StringValue;
                if (textContent != null) {
                    var range = new DebugSourceRange();
                    range.length = textContent.value.Length;
                    range.debugMetadata = textContent.debugMetadata;
                    range.text = textContent.value;
                    _debugSourceRanges.Add(range);
                }
            }
        }

        private DebugMetadata DebugMetadataForContentAtOffset(int offset) {
            var currOffset = 0;

            DebugMetadata lastValidMetadata = null;
            foreach (var range in _debugSourceRanges) {
                if (range.debugMetadata != null)
                    lastValidMetadata = range.debugMetadata;

                if (offset >= currOffset && offset < currOffset + range.length)
                    return lastValidMetadata;

                currOffset += range.length;
            }

            return null;
        }

        // Need to wrap the error handler so that we know
        // when there was a critical error between parse and codegen stages
        private void OnParseError(string message, ErrorType errorType) {
            if (errorType == ErrorType.Error)
                _hadParseError = true;

            if (_options.errorHandler != null)
                _options.errorHandler(message, errorType);
            else
                throw new Exception(message);
        }

        public class Options {
            public bool countAllVisits;
            public ErrorHandler errorHandler;
            public IFileHandler fileHandler;
            public List<string> pluginNames;
            public string sourceFilename;
        }

        public class CommandLineInputResult {
            public int choiceIdx = -1;
            public string divertedPath;
            public string output;
            public bool requestsExit;
        }

        public struct DebugSourceRange {
            public int length;
            public DebugMetadata debugMetadata;
            public string text;
        }
    }
}