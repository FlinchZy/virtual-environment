﻿// LIZENZBEDINGUNGEN - Seanox Software Solutions ist ein Open-Source-Projekt, im
// Folgenden Seanox Software Solutions oder kurz Seanox genannt.
// Diese Software unterliegt der Version 2 der Apache License.
//
// Virtual Environment Platform
// Creates, starts and controls a virtual environment.
// Copyright (C) 2022 Seanox Software Solutions
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not
// use this file except in compliance with the License. You may obtain a copy of
// the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
// License for the specific language governing permissions and limitations under
// the License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Platform
{
    internal static class Notification
    {
        internal enum Type
        {
            Error,
            Trace,
            Batch,
            Abort
        }

        internal readonly struct Message {
            internal Type   Type { get; }
            internal string Text { get; }

            internal Message(Type type, string text)
            {
                this.Type = type;
                this.Text = text;
            }
        }

        private static readonly List<INotification> _subscriptions;

        static Notification()
        {
            _subscriptions = new List<INotification>();

            var applicationPath = Assembly.GetExecutingAssembly().Location;
            var loggingFile = Path.Combine(Path.GetDirectoryName(applicationPath),
                Path.GetFileNameWithoutExtension(applicationPath) + ".log");
            if (File.Exists(loggingFile)
                    && new FileInfo(loggingFile).Length > 0)
                using (var streamWriter = File.AppendText(loggingFile))
                    streamWriter.WriteLine();
        }

        internal static void Push(Type type, params string[] messages)
        {
            var publication = String.Join("\r\n", messages.Where(message =>
                    !message.Trim().StartsWith("@")));
            if (publication.Length > 0)
                _subscriptions.ForEach(recipient =>
                        recipient.Receive(new Message(type, publication)));

            var applicationPath = Assembly.GetExecutingAssembly().Location;
            var loggingFile = Path.Combine(Path.GetDirectoryName(applicationPath),
                    Path.GetFileNameWithoutExtension(applicationPath) + ".log");

            messages.Select(message =>
                    Messages.DiskpartUnexpectedErrorOccurred == message
                            || Messages.WorkerUnexpectedErrorOccurred == message
                        ? String.Format(message, Path.GetFileName(loggingFile)) : message).ToArray();

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var output = String.Join("\r\n", messages.Select(message => Regex.Replace(message, "^@+", "")).ToArray()); 
            output = String.Format("{0} {1} ", timestamp, type.ToString().ToUpper()) + output;
            var prefix = String.Format("{0} {1} ", timestamp, " ... ");
            output = Regex.Replace(output, "(\r\n)|(\n\r)|[\r\n]", "\r\n" + prefix);
            using (var streamWriter = File.AppendText(loggingFile))
                streamWriter.WriteLine(output);
        }

        internal static void Subscribe(INotification recipient)
        {
            if (recipient == null)
                throw new ArgumentNullException();
            if (!_subscriptions.Contains(recipient))
                _subscriptions.Add(recipient);
        }

        internal static void UnSubscribe(INotification recipient)
        {
            if (recipient == null)
                throw new ArgumentNullException();
            if (_subscriptions.Contains(recipient))
                _subscriptions.Remove(recipient);
        }

        internal interface INotification
        {
            void Receive(Message message);
        }
    }
}