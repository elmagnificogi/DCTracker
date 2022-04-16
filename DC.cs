using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCProgress
{

    class DC
    {
        public String region;
        public String sc;
        public String exp;
        public String ladder;
        public String progress;

        public DC(String r, String sc, String exp, String ladder, String pro)
        {
            this.region = r;
            this.sc = sc;
            this.exp = exp;
            this.ladder = ladder;
            this.progress = pro;
        }
    }
}
