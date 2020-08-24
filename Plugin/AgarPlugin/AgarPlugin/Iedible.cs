using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEdible
{
    ushort ID { get; }
    float X { get; }
    float Y { get; }
    float Radius { get; }
}