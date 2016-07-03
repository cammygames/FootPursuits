using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FootPursuits.Util
{
    public class Enums
    {
        public enum CalloutState
        {
            Created,
            Cancelled,
            Responding,
            OnScene,
            Complete
        }

        public enum ResponseType
        {
            Code2 = 2,
            Code3 = 3
        }
    }
}
