using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    [Flags]
    enum WallPositionFlags
    {
        Up = 0x1,
        Down = 0x2,
        Left = 0x4,
        Right = 0x8
    }
}
