using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCTracker
{

    class DC
    {
        public String region;
        public String sc;
        public String exp;
        public String ladder;
        public String progress;
        public String time;

        public DC(String r, String sc, String time, String ladder, String pro)
        {
            this.region = r;
            this.sc = sc;
            this.time = time;
            this.ladder = ladder;
            this.progress = pro;
        }
    }
}
