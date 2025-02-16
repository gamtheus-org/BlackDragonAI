using System;
using System.Collections.Generic;
using System.Text;

namespace BlackLegionBot.TwitchApi.Models
{
    public class ListResultWithPagination<T> : ListResultPure<T>
    {
        public Pagination Pagination { get; set; }
        public int Total { get; set; } = 0;
    }

    public class Pagination
    {
        public string Cursor { get; set; }
    }
}
