using System.Collections.Generic;
using System.Net.Http;


namespace TwitterAdsAPIProject.Models
{

    public class LineItemList
    {
        public List<LineItem>? data { get; set; }
    }

    public class LineItem
    {
        public Dictionary<string, object>? Metrics { get; set; }


    }
}
