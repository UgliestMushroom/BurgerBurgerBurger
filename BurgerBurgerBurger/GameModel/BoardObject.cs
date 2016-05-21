using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Philhuge.Projects.BurgerBurgerBurger.GameModel
{
    public abstract class BoardObject : IStaticObject
    {
        public int CellCol { get; set; }
        public int CellRow { get; set; }

        public BoardObject(int cellCol, int cellRow)
        {
            this.CellCol = cellCol;
            this.CellRow = cellRow;
        }

        public abstract void InteractWithMovingObject(ScoreObject movingObject);
    }
}
