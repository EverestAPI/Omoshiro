using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Omoshiro {
    public class GhostData {

        public readonly static string Magic = "everest-ghost\r\n";
        public readonly static char[] MagicChars = Magic.ToCharArray();

        public readonly static int Version = 0;

        public readonly static Regex PathVerifyRegex = new Regex("[\"`?* #" + Regex.Escape(new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars())) + "]", RegexOptions.Compiled);

        public static string GetGhostFilePrefix(string sid, AreaMode mode, string level)
            => PathVerifyRegex.Replace($"{sid}-{(char) ('A' + (int) mode)}-{level}-", "-");

        public static string GetGhostFileName(string sid, AreaMode mode, string level, string name, DateTime date)
            => GetGhostFilePrefix(sid, mode, level) + PathVerifyRegex.Replace($"{name}-{date.ToString("yyyy-MM-dd-HH-mm-ss-fff", CultureInfo.InvariantCulture)}", "-") + ".oshiro";

        public string SID;
        public AreaMode Mode;
        public string Level;
        public string Target;

        public string Name;
        public DateTime Date;

        public bool Dead;

        public float? Opacity;

        public string FilePath;

        public List<GhostFrame> Frames = new List<GhostFrame>();

        public GhostFrame this[int i] {
            get {
                if (i < 0 || i >= Frames.Count)
                    return default(GhostFrame);
                return Frames[i];
            }
        }

        public GhostData() {
            Date = DateTime.UtcNow;
        }
        public GhostData(string filePath)
            : this() {
            FilePath = filePath;
        }

        public GhostData Read() {
            if (FilePath == null)
                // Keep existing frames in-tact.
                return null;

            if (!File.Exists(FilePath)) {
                // File doesn't exist - load nothing.
                Frames = new List<GhostFrame>();
                return null;
            }

            using (Stream stream = File.OpenRead(FilePath))
            using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8))
                return Read(reader);
        }
        public GhostData Read(BinaryReader reader) {
            if (reader.ReadInt16() != 0x0ade)
                return null; // Endianness mismatch.

            char[] magic = reader.ReadChars(MagicChars.Length);
            if (magic.Length != MagicChars.Length)
                return null; // Didn't read as much as we wanted to read.
            for (int i = 0; i < MagicChars.Length; i++) {
                if (magic[i] != MagicChars[i])
                    return null; // Magic mismatch.
            }

            int version = reader.ReadInt32();
            // Don't read data from the future, but try to read data from the past.
            if (version > Version)
                return null;

            int compression = reader.ReadInt32();

            if (compression != 0)
                return null; // Compression not supported yet.

            SID = reader.ReadNullTerminatedString();
            Mode = (AreaMode) reader.ReadInt32();
            Level = reader.ReadNullTerminatedString();
            Target = reader.ReadNullTerminatedString();

            Name = reader.ReadNullTerminatedString();
            long dateBin = reader.ReadInt64();
            try {
                Date = DateTime.FromBinary(dateBin);
            } catch {
                // The date was invalid. Let's ignore it.
                Date = DateTime.UtcNow;
            }

            Dead = reader.ReadBoolean();

            Opacity = reader.ReadBoolean() ? (float?) reader.ReadSingle() : null;

            int count = reader.ReadInt32();
            reader.ReadChar(); // \r
            reader.ReadChar(); // \n
            Frames = new List<GhostFrame>(count);
            for (int i = 0; i < count; i++) {
                GhostFrame frame = new GhostFrame();
                frame.Read(reader);
                Frames.Add(frame);
            }

            return this;
        }

        public void Write() {
            if (FilePath == null)
                return;
            if (FilePath != null && File.Exists(FilePath)) {
                // Force ourselves onto the set filepath.
                File.Delete(FilePath);
            }

            if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

            using (Stream stream = File.OpenWrite(FilePath))
            using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8))
                Write(writer);
        }
        public void Write(BinaryWriter writer) {
            writer.Write((short) 0x0ade);
            writer.Write(MagicChars);
            writer.Write(Version);

            writer.Write(0); // Uncompressed

            writer.WriteNullTerminatedString(SID);
            writer.Write((int) Mode);
            writer.WriteNullTerminatedString(Level);
            writer.WriteNullTerminatedString(Target);

            writer.WriteNullTerminatedString(Name);
            writer.Write(Date.ToBinary());

            writer.Write(Dead);

            if (Opacity != null) {
                writer.Write(true);
                writer.Write(Opacity.Value);
            } else {
                writer.Write(false);
            }

            writer.Write(Frames.Count);
            writer.Write('\r');
            writer.Write('\n');
            for (int i = 0; i < Frames.Count; i++) {
                GhostFrame frame = Frames[i];
                frame.Write(writer);
            }
        }

    }
}
