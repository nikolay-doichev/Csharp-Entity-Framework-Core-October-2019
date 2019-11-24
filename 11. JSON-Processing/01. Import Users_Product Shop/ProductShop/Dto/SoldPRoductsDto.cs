using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Dto
{
    public class SoldProductsDto
    {
        public int Count { get; set; }
        public SoldProductsDto[] Products { get; set; }
    }
}
