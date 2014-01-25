using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace CatGame
{
    class RainbowLighting
    {
        private static Color[] RAINBOW_COLORS = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };
        private float elapsed = 0;

        public RainbowLighting()
        {
        }

        public Vector3 GetDirection()
        {
            Vector3 initialVector = Vector3.Forward;
            // TODO Sweeping lighting
            return new Vector3(0, -1, (float)Math.Cos(elapsed));
        }

        public Vector3 GetColor()
        {
            return RAINBOW_COLORS[(int) elapsed % 7].ToVector3() * 0.2f + Vector3.One * .8f;
        }

        internal void Update(float delta)
        {
            elapsed += delta;
        }
    }
}
