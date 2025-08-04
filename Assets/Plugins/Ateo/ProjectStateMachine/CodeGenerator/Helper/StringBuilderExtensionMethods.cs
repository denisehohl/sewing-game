#if UNITY_EDITOR
using System.Text;
using System;

namespace Ateo.CodeGeneration
{
    public static class StringBuilderExtensionMethods
    {
        /// <summary>tab == 1 Tab</summary>
        public const string TabRepresentation = "	";

        public static void AppendIndent(this StringBuilder self, int indentCount, string text)
        {
            for (int i = 0; i < indentCount; i++)
            {
                self.Append(TabRepresentation);
            }
            self.Append(text);
        }

        public static void AppendIndentLine(this StringBuilder self, int indentCount, string text)
        {
            self.AppendIndent(indentCount, text);
            self.Append(Environment.NewLine);
        }

        public static void AppendIndentFormat(this StringBuilder self, int indentCount, string text, params object[] param)
        {
            for (int i = 0; i < indentCount; i++)
            {
                self.Append(TabRepresentation);
            }
            self.AppendFormat(text, param);
        }

        public static void AppendIndentFormatLine(this StringBuilder self, int indentCount, string text, params object[] param)
        {
            self.AppendIndentFormat(indentCount, text, param);
            self.Append(Environment.NewLine);
        }
    }
}
#endif