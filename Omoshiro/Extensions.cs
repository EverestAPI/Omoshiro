using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omoshiro {
    public static class Extensions {

        public static string ReadNullTerminatedString(this BinaryReader stream) {
            string text = "";
            char c;
            while ((c = stream.ReadChar()) > '\0') {
                text += c.ToString();
            }
            return text;
        }

        public static void WriteNullTerminatedString(this BinaryWriter stream, string text) {
            if (text != null) {
                for (int i = 0; i < text.Length; i++) {
                    char c = text[i];
                    stream.Write(c);
                }
            }
            stream.Write('\0');
        }

    }
}
