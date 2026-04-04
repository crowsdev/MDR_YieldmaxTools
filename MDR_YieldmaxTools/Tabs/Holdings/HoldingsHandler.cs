using MDR_YieldmaxTools.Models;

namespace MDR_YieldmaxTools.Tabs.Holdings
{
    public class HoldingsHandler
    {
        private static HoldingsHandler _instance;

        public static HoldingsHandler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HoldingsHandler();
                }

                return _instance;
            }
        }

        public HoldingsProfile CurrentProfile { get; set; }
        
        public HoldingsHandler()
        {
            CurrentProfile = HoldingsProfile.Load("local");
        }
    }
}