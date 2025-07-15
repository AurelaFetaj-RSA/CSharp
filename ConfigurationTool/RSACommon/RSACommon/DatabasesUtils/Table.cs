using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSACommon.DatabasesUtils
{
    public interface IDBTable
    {
        string Name { get; set; }
        ILog Logger { get; set; }

    }
}
