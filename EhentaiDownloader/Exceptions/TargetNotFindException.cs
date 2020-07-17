using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Exceptions
{
    public class TargetNotFindException:Exception
    {
        public TargetNotFindException(string message):base(message)
        {

        }
    }
}
