using System;
using System.Collections.Generic;
using ComponentFramework.Core;
using ComponentFramework.Tools;
using MTV3D65;

namespace ComponentFramework.Components
{
    public class DebuggingBag : Component, IDebuggingBagService
    {
        static readonly TV_COLOR NewItemColor = new TV_COLOR(1, 1, 0.5f, 1);

        readonly Dictionary<string, DebuggingLine> items = new Dictionary<string, DebuggingLine>();

        public bool Shadows { get; set; }

        public DebuggingBag(ICore core)
            : base(core)
        {
            Order = int.MinValue;
        }

        public void Put(string name, object item)
        {
            var value = item == null ? "" : item.ToString();

            DebuggingLine line;
            if (items.TryGetValue(name, out line))
            {
                if (line.Value != value)
                    line.Refresh();
                line.Value = value;
            }
            else
                items.Add(name, new DebuggingLine(name, value));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (items.Count == 0) return;

            foreach (var line in items.Values)
                line.Update(elapsedTime);
        }

        public override void PostDraw()
        {
#if DEBUG
            const int LineSize = 15;
            const int x = 8;
            int y = 30;

            if (items.Count == 0) return;

            Screen2DText.Action_BeginText(true);

            foreach (var line in items.Values)
            {
                float invSqItemAge = 1 - Easing.EaseIn(line.Age, EasingType.Quadratic);
                if (MathHelper.AlmostEqual(invSqItemAge, 0))
                    continue;

                var itemColor = line.New ? NewItemColor : new TV_COLOR(1, 1, 1, invSqItemAge);

                var displayText = string.Format("{0} {1} {2}", line.Name, line.Value.Equals(string.Empty) ? string.Empty : ":", line.Value);

                if (Shadows)
                {
                    var shadowColor = new TV_COLOR(0, 0, 0, invSqItemAge * 0.5f);
                    Screen2DText.TextureFont_DrawText(displayText, x - 1, y - 1, shadowColor.GetIntColor());
                }
                Screen2DText.TextureFont_DrawText(displayText, x, y, itemColor.GetIntColor());
                y += LineSize;
            }

            Screen2DText.Action_EndText();

            Clean();
#endif
        }

        readonly List<string> toRemove = new List<string>();
        void Clean()
        {
            foreach (string name in items.Keys)
                if (items[name].Expired)
                    toRemove.Add(name);

            if (toRemove.Count > 0)
            {
                foreach (string name in toRemove)
                    items.Remove(name);
                toRemove.Clear();
            }
        }

        class DebuggingLine
        {
            static readonly TimeSpan HighlightTime = TimeSpan.FromSeconds(0.25);
            static readonly TimeSpan ExpirationTime = TimeSpan.FromSeconds(20);

            public readonly string Name;

            TimeSpan sinceSeen, sinceRefreshed;

            public DebuggingLine(string name, object value)
            {
                Name = name;
                Value = value.ToString();
                Refresh();
            }

            string value;
            public string Value
            {
                get { return value; }
                set
                {
                    this.value = value;
                    sinceSeen = TimeSpan.Zero;
                }
            }

            public void Refresh()
            {
                sinceSeen = TimeSpan.Zero;
                sinceRefreshed = TimeSpan.Zero;
            }

            public void Update(TimeSpan elapsed)
            {
                sinceSeen += elapsed;
                sinceRefreshed += elapsed;
            }

            public bool Expired
            {
                get { return sinceSeen >= ExpirationTime; }
            }

            public float Age
            {
                get { return MathHelper.Clamp((float)sinceSeen.Ticks / ExpirationTime.Ticks, 0, 1); }
            }

            public bool New
            {
                get { return sinceRefreshed < HighlightTime; }
            }
        }
    }

    public interface IDebuggingBagService : IService
    {
        void Put(string name, object item);
        bool Shadows { get; set; }
    }
}
