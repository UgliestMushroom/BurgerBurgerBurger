using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    /// <summary>
    /// Exception for when MovableObjects attempt to move but would hit a Board wall instead.
    /// </summary>
    public class WouldHitWallException : Exception
    {
    }
}
