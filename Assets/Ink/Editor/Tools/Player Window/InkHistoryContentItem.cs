using System;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;

namespace Ink.UnityIntegration.Debugging {
    [Serializable]
    public class InkHistoryContentItem {
        public enum ContentType {
            PresentedContent,
            ChooseChoice,
            PresentedChoice,
            EvaluateFunction,
            CompleteEvaluateFunction,
            ChoosePathString,
            Warning,
            Error,
            DebugNote
        }

        public string content;
        public List<string> tags;
        public ContentType contentType;

        [SerializeField] private JsonDateTime _time;

        private InkHistoryContentItem(string text, ContentType contentType) {
            content = text;
            this.contentType = contentType;
            time = DateTime.Now;
        }

        private InkHistoryContentItem(string text, List<string> tags, ContentType contentType) {
            content = text;
            this.tags = tags;
            this.contentType = contentType;
            time = DateTime.Now;
        }

        public DateTime time {
            get => _time;
            private set => _time = value;
        }

        public static InkHistoryContentItem CreateForContent(string choiceText, List<string> tags) {
            return new InkHistoryContentItem(choiceText, tags, ContentType.PresentedContent);
        }

        public static InkHistoryContentItem CreateForPresentChoice(Choice choice) {
            return new InkHistoryContentItem(choice.text.Trim(), ContentType.PresentedChoice);
        }

        public static InkHistoryContentItem CreateForMakeChoice(Choice choice) {
            return new InkHistoryContentItem(choice.text.Trim(), ContentType.ChooseChoice);
        }

        public static InkHistoryContentItem CreateForEvaluateFunction(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.EvaluateFunction);
        }

        public static InkHistoryContentItem CreateForCompleteEvaluateFunction(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.CompleteEvaluateFunction);
        }

        public static InkHistoryContentItem CreateForChoosePathString(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.ChoosePathString);
        }

        public static InkHistoryContentItem CreateForWarning(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.Warning);
        }

        public static InkHistoryContentItem CreateForError(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.Error);
        }

        public static InkHistoryContentItem CreateForDebugNote(string choiceText) {
            return new InkHistoryContentItem(choiceText, ContentType.DebugNote);
        }

        private struct JsonDateTime {
            public long value;

            public static implicit operator DateTime(JsonDateTime jdt) {
                return DateTime.FromFileTime(jdt.value);
            }

            public static implicit operator JsonDateTime(DateTime dt) {
                var jdt = new JsonDateTime();
                jdt.value = dt.ToFileTime();
                return jdt;
            }
        }
    }
}