using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omoshiro {
    public class GhostFrame {

        // Omoshiro helpers.
        internal GhostData Data;

        internal string IndexString {
            get {
                return Data?.Frames?.IndexOf(this).ToString("000000") ?? "------";
            }
            set {

            }
        }

        public void Read(BinaryReader reader) {
            string chunk;
            // The last "chunk" type, \r\n (Windows linebreak), doesn't contain a length.
            while ((chunk = reader.ReadNullTerminatedString()) != "\r\n") {
                uint length = reader.ReadUInt32();
                switch (chunk) {
                    case "input":
                        ReadChunkInput(reader);
                        break;
                    default:
                        // Skip any unknown chunks.
                        reader.BaseStream.Seek(length, SeekOrigin.Current);
                        break;
                }
            }
        }

        public void Write(BinaryWriter writer) {
            WriteChunkInput(writer);

            writer.WriteNullTerminatedString("\r\n");
        }

        public long WriteChunkStart(BinaryWriter writer, string name) {
            writer.WriteNullTerminatedString(name);
            writer.Write(0U); // Filled in later.
            long start = writer.BaseStream.Position;
            return start;
        }

        public void WriteChunkEnd(BinaryWriter writer, long start) {
            long pos = writer.BaseStream.Position;
            long length = pos - start;

            // Update the chunk length, which consists of the 4 bytes before the chunk Data?.
            writer.Flush();
            writer.BaseStream.Seek(start - 4, SeekOrigin.Begin);
            writer.Write((int) length);

            writer.Flush();
            writer.BaseStream.Seek(pos, SeekOrigin.Begin);
        }

        public bool HasInput;

        public int MoveX;
        public int MoveY;

        public Vector2 Aim;
        public Vector2 MountainAim;

        // Omoshiro helpers.

        internal bool? MoveLeft {
            get {
                return MoveX < 0;
            }
            set {
                if (value == null)
                    return;
                if (value.Value)
                    MoveX = -1;
                else
                    MoveX = 0;
                Data?.Change(this);
            }
        }
        internal bool? MoveRight {
            get {
                return MoveX > 0;
            }
            set {
                if (value == null)
                    return;
                if (value.Value)
                    MoveX = +1;
                else
                    MoveX = 0;
                Data?.Change(this);
            }
        }
        internal bool? MoveUp {
            get {
                return MoveY < 0;
            }
            set {
                if (value == null)
                    return;
                if (value.Value)
                    MoveY = -1;
                else
                    MoveY = 0;
                Data?.Change(this);
            }
        }
        internal bool? MoveDown {
            get {
                return MoveY > 0;
            }
            set {
                if (value == null)
                    return;
                if (value.Value)
                    MoveY = +1;
                else
                    MoveY = 0;
                Data?.Change(this);
            }
        }

        internal string AimString {
            get {
                return Aim.ToString();
            }
            set {
                Aim.Parse(value);
            }
        }

        internal string MountainAimString {
            get {
                return MountainAim.ToString();
            }
            set {
                MountainAim.Parse(value);
            }
        }

        public int Buttons;

        public bool ESC {
            get {
                return (Buttons & (int) ButtonMask.ESC) == (int) ButtonMask.ESC;
            }
            set {
                Buttons &= (int) ~ButtonMask.ESC;
                if (value)
                    Buttons |= (int) ButtonMask.ESC;
                Data?.Change(this);
            }
        }
        public bool Pause {
            get {
                return (Buttons & (int) ButtonMask.Pause) == (int) ButtonMask.Pause;
            }
            set {
                Buttons &= (int) ~ButtonMask.Pause;
                if (value)
                    Buttons |= (int) ButtonMask.Pause;
                Data?.Change(this);
            }
        }
        public bool MenuLeft {
            get {
                return (Buttons & (int) ButtonMask.MenuLeft) == (int) ButtonMask.MenuLeft;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuLeft;
                if (value)
                    Buttons |= (int) ButtonMask.MenuLeft;
                Data?.Change(this);
            }
        }
        public bool MenuRight {
            get {
                return (Buttons & (int) ButtonMask.MenuRight) == (int) ButtonMask.MenuRight;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuRight;
                if (value)
                    Buttons |= (int) ButtonMask.MenuRight;
                Data?.Change(this);
            }
        }
        public bool MenuUp {
            get {
                return (Buttons & (int) ButtonMask.MenuUp) == (int) ButtonMask.MenuUp;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuUp;
                if (value)
                    Buttons |= (int) ButtonMask.MenuUp;
                Data?.Change(this);
            }
        }
        public bool MenuDown {
            get {
                return (Buttons & (int) ButtonMask.MenuDown) == (int) ButtonMask.MenuDown;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuDown;
                if (value)
                    Buttons |= (int) ButtonMask.MenuDown;
                Data?.Change(this);
            }
        }
        public bool MenuConfirm {
            get {
                return (Buttons & (int) ButtonMask.MenuConfirm) == (int) ButtonMask.MenuConfirm;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuConfirm;
                if (value)
                    Buttons |= (int) ButtonMask.MenuConfirm;
                Data?.Change(this);
            }
        }
        public bool MenuCancel {
            get {
                return (Buttons & (int) ButtonMask.MenuCancel) == (int) ButtonMask.MenuCancel;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuCancel;
                if (value)
                    Buttons |= (int) ButtonMask.MenuCancel;
                Data?.Change(this);
            }
        }
        public bool MenuJournal {
            get {
                return (Buttons & (int) ButtonMask.MenuJournal) == (int) ButtonMask.MenuJournal;
            }
            set {
                Buttons &= (int) ~ButtonMask.MenuJournal;
                if (value)
                    Buttons |= (int) ButtonMask.MenuJournal;
                Data?.Change(this);
            }
        }
        public bool QuickRestart {
            get {
                return (Buttons & (int) ButtonMask.QuickRestart) == (int) ButtonMask.QuickRestart;
            }
            set {
                Buttons &= (int) ~ButtonMask.QuickRestart;
                if (value)
                    Buttons |= (int) ButtonMask.QuickRestart;
                Data?.Change(this);
            }
        }
        public bool Jump {
            get {
                return (Buttons & (int) ButtonMask.Jump) == (int) ButtonMask.Jump;
            }
            set {
                Buttons &= (int) ~ButtonMask.Jump;
                if (value)
                    Buttons |= (int) ButtonMask.Jump;
                Data?.Change(this);
            }
        }
        public bool Dash {
            get {
                return (Buttons & (int) ButtonMask.Dash) == (int) ButtonMask.Dash;
            }
            set {
                Buttons &= (int) ~ButtonMask.Dash;
                if (value)
                    Buttons |= (int) ButtonMask.Dash;
                Data?.Change(this);
            }
        }
        public bool Grab {
            get {
                return (Buttons & (int) ButtonMask.Grab) == (int) ButtonMask.Grab;
            }
            set {
                Buttons &= (int) ~ButtonMask.Grab;
                if (value)
                    Buttons |= (int) ButtonMask.Grab;
                Data?.Change(this);
            }
        }
        public bool Talk {
            get {
                return (Buttons & (int) ButtonMask.Talk) == (int) ButtonMask.Talk;
            }
            set {
                Buttons &= (int) ~ButtonMask.Talk;
                if (value)
                    Buttons |= (int) ButtonMask.Talk;
                Data?.Change(this);
            }
        }

        public void ReadChunkInput(BinaryReader reader) {
            HasInput = true;

            MoveX = reader.ReadInt32();
            MoveY = reader.ReadInt32();

            Aim = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            MountainAim = new Vector2(reader.ReadSingle(), reader.ReadSingle());

            Buttons = reader.ReadInt32();
        }

        public void WriteChunkInput(BinaryWriter writer) {
            if (!HasInput)
                return;
            long start = WriteChunkStart(writer, "input");

            writer.Write(MoveX);
            writer.Write(MoveY);

            writer.Write(Aim.X);
            writer.Write(Aim.Y);

            writer.Write(MountainAim.X);
            writer.Write(MountainAim.Y);

            writer.Write(Buttons);

            WriteChunkEnd(writer, start);
        }

        [Flags]
        public enum ButtonMask : int {
            ESC = 1 << 0,
            Pause = 1 << 1,
            MenuLeft = 1 << 2,
            MenuRight = 1 << 3,
            MenuUp = 1 << 4,
            MenuDown = 1 << 5,
            MenuConfirm = 1 << 6,
            MenuCancel = 1 << 7,
            MenuJournal = 1 << 8,
            QuickRestart = 1 << 9,
            Jump = 1 << 10,
            Dash = 1 << 11,
            Grab = 1 << 12,
            Talk = 1 << 13
        }

    }
}
