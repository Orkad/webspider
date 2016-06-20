﻿using System;

namespace WebSpiderLib.Parsing
{
    [Serializable]
    public class Field
    {
        public string Name { get; set; }
        public string Value { get; set; }

        internal Field()
        {
            
        }

        internal Field(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
