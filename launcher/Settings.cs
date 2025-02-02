﻿// LIZENZBEDINGUNGEN - Seanox Software Solutions ist ein Open-Source-Projekt, im
// Folgenden Seanox Software Solutions oder kurz Seanox genannt.
// Diese Software unterliegt der Version 2 der Apache License.
//
// Virtual Environment Launcher
// Program starter for the virtual environment.
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

using System.Xml.Serialization;

namespace launcher
{
    [XmlRoot("settings")]
    public struct Settings
    {
        [XmlElement("opacity")]
        public int Opacity;

        [XmlElement("hotKey")]
        public string HotKey;

        [XmlArray("tiles")]
        [XmlArrayItem("tile", typeof(Tile))]
        public Tile[] Tiles;
        
        public struct Tile
        {
            [XmlElement("index")]
            public int Index;

            [XmlElement("title")]
            public string Title;

            [XmlElement("icon")]
            public string Icon;

            [XmlElement("destination")]
            public string Destination;

            [XmlElement("arguments")]
            public string Arguments;

            [XmlElement("workingDirectory")]
            public string WorkingDirectory;
        }
    }
}