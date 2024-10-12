using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Study_Language
{
    public class WordEntry
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Word { get; set; }
        public string Translation { get; set; }
    }

    public class PhraseEntry
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string Phrase { get; set; }
        public string Translation { get; set; }
    }

}

