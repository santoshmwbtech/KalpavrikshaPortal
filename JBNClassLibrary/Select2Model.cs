using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JBNClassLibrary
{
    public class Select2Model
    {

    }
    public class Select2OptionModel
    {
        public string id { get; set; }
        public string text { get; set; }
    }

    public class Select2PagedResult
    {
        public int Total { get; set; }
        public List<Select2OptionModel> Results { get; set; }
    }
}
