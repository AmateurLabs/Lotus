using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lotus.ECS {
    [Flags]
    public enum RenderLayers {
        None=0,
        Layer0=1 << 0,
        Layer1=1 << 1,
        Layer2=1 << 2,
        Layer3=1 << 3,
        Layer4=1 << 4,
        Layer5=1 << 5,
        Layer6=1 << 6,
        Layer7=1 << 7,
        Layer8=1 << 8,
        Layer9=1 << 9,
        Layer10=1 << 10,
        Layer11=1 << 11,
        Layer12=1 << 12,
        Layer13=1 << 13,
        Layer14=1 << 14,
        Layer15=1 << 15,
        All=Layer0|Layer1|Layer2|Layer3|Layer4|Layer5|Layer6|Layer7|Layer8|Layer9|Layer10|Layer11|Layer12|Layer13|Layer14|Layer15,

        Default=Layer0,
        World=Layer0,
        GUI=Layer1
    }
}
