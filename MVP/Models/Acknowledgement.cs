using System;
using MVP.Resources;

namespace MVP.Models
{
    public class Acknowledgement
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class AcknowledgementTypes
    {
        public static string Translator = Translations.acknowledgementtype_translator;
        public static string Tester = Translations.acknowledgementtype_tester;
    }
}
