using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Omoshiro {
    public struct Vector2 {

        public float X;
        public float Y;

        public Vector2(float x, float y) {
            X = x;
            Y = y;
        }

        // Non-standard
        public override string ToString() {
            return $"{X.ToString(CultureInfo.InvariantCulture)} | {Y.ToString(CultureInfo.InvariantCulture)}";
        }

        public void Parse(string text) {
            string[] split = text.Split('|');
            if (split.Length != 2)
                return;
            float x, y;
            if (!float.TryParse(split[0].Trim(), out x) ||
                !float.TryParse(split[1].Trim(), out y))
                return;
            X = x;
            Y = y;
        }

    }
}
