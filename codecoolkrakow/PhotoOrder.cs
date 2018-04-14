using System;
using System.Collections.Generic;
using System.Text;

namespace codecoolkrakow
{
    public class PhotoOrder
    {
        public string CustomerEmail {get; set; }
        public string FileName{ get; set; }
        public int RequiredHeight { get; set; }
        public int RequiredWidth { get; set; }
    }
}
