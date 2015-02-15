using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenTibiaCommons
{
    public static class OtConverter
    {
        public static byte TibiaFluidToOtFluid(byte fluid)
        {
            if (fluid == 11)
                return 3;
            else if (fluid < OtConstants.ReverseFluidMap.Length)
                return OtConstants.ReverseFluidMap[fluid];

            return 0;
        }

    }
}
