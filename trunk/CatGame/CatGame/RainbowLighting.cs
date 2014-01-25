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
        private const float CYCLE_TIME = 1f;
        private const float MOVEMENT_TIME = .4f;

        public RainbowLighting()
        {
        }

        public Vector3 GetDirection()
        {
            float progress = (elapsed % CYCLE_TIME) / CYCLE_TIME;
            Vector3 lightVector = new Vector3(0, 0, 1);
            float rotation = MathHelper.Lerp(0, MathHelper.Pi, MathHelper.Clamp((progress - (1-MOVEMENT_TIME) / 2) / MOVEMENT_TIME, 0, 1));
            lightVector = Vector3.Transform(lightVector, Matrix.CreateRotationX(rotation));
            return lightVector;
        }

        public Vector3 GetColor()
        {
            float progress = (elapsed % CYCLE_TIME) / CYCLE_TIME;
            float heatUpTime = (1 - MOVEMENT_TIME) / 2;
            float intensity = 1;
            if (progress < heatUpTime)
            {
                intensity = MathHelper.Lerp(0.2f, 1, progress / heatUpTime);
            }
            else if (progress >= (1 - heatUpTime))
            {
                intensity = MathHelper.Lerp(0.2f, 1, (1 - progress) / heatUpTime);
            }
            return (RAINBOW_COLORS[(int) elapsed % 7].ToVector3() * 1f + Vector3.One * 0f) * intensity;
        }

        internal void Update(float delta)
        {
            elapsed += delta;
        }
    }
}
