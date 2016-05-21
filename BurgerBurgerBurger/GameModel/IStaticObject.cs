using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public interface IStaticObject
    {
        void InteractWithMovingObject(MovableObject movingObject);
    }
}
