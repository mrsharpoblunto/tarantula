using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Tarantula.MVP.Model
{
    public interface IGesture
    {
        bool MadeGesture(List<Point> points);
    }
}
