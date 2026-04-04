using System.Collections.Generic;
using System.Windows.Documents;
using MDR_YieldmaxTools.Enums;

namespace MDR_YieldmaxTools
{
    public class GlobalVars
    {
        public static string Version = "1.4";

        public static List<Symbols> AllSymbols = new List<Symbols>()
        {
            Symbols.ABNY,
            Symbols.AIYY,
            Symbols.AMDY,
            Symbols.AMZY,
            Symbols.APLY,
            Symbols.BABO,
            // Symbols.BIGY,
            Symbols.BRKC,
            Symbols.CHPY,
            Symbols.CONY,
            Symbols.CRCO,
            Symbols.CRSH,
            Symbols.CVNY,
            Symbols.DIPS,
            Symbols.DISO,
            Symbols.DRAY,
            Symbols.FBY,
            Symbols.FEAT,
            Symbols.FIAT,
            Symbols.FIVY,
            Symbols.GDXY,
            Symbols.GMEY,
            Symbols.GOOY,
            Symbols.GPTY,
            Symbols.HIYY,
            Symbols.HOOY,
            Symbols.JPMO,
            Symbols.LFGY,
            Symbols.MARO,
            Symbols.MRNY,
            Symbols.MSFO,
            Symbols.MSST,
            Symbols.MSTY,
            Symbols.NFLY,
            Symbols.NVDY,
            Symbols.NVIT,
            Symbols.OARK,
            Symbols.PLTY,
            Symbols.PYPY,
            Symbols.QDTY,
            Symbols.RBLY,
            Symbols.RDTY,
            Symbols.RDYY,
            // Symbols.RNTY,
            Symbols.SDTY,
            Symbols.SLTY,
            Symbols.SMCY,
            Symbols.SNOY,
            // Symbols.SOXY,
            Symbols.SQY,
            Symbols.TEST,
            Symbols.TSLY,
            Symbols.TSMY,
            Symbols.ULTY,
            Symbols.WNTR,
            Symbols.XOMO,
            Symbols.XYZY,
            Symbols.YBIT,
            Symbols.YMAG,
            Symbols.YMAX,
            Symbols.YQQQ,
        };

        public static Dictionary<string, double> ROIMinDivValues = new Dictionary<string, double>
        {
            { "6 months", 0.041666666667 },
            { "1 year", 0.020833333333 },
            { "1.5 years", 0.013888888889 },
            { "2 years", 0.010416666667 },
            { "3 years", 0.006944444444 },
            { "4 years", 0.005208333333 },
            { "5 years", 0.004166666667 }
        };
    }
}