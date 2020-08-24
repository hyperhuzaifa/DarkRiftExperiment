using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DarkRift;
using DarkRift.Server;

namespace AgarPlugin
{
    class Player
    {
        public ushort ID { get; set; }
        public string name { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; }
        public byte ColorR { get; set; }
        public byte ColorG { get; set; }
        public byte ColorB { get; set; }

        public Player(ushort ID, string name, float x, float y, float radius, byte colorR, byte colorG, byte colorB)
        {
            this.ID = ID;
            this.name = name;
            this.X = x;
            this.Y = y;
            this.Radius = radius;
            this.ColorR = colorR;
            this.ColorG = colorG;
            this.ColorB = colorB;
        }
    }
}
