using System;

namespace calctool
{
    public class CalcTool
    {
        public int price;
        public void Add(int price)
        {
            this.price -= price;
            if (this.price < 0)
                throw new Exception("no money");
        }
    }
}
