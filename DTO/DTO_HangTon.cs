using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace DTO
    {
        public class DTO_HangTon
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public string CategoryName { get; set; }
            public DateTime CreatedAt { get; set; }
            public string ProductName { get; set; }
        }
    }
