using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct BayData
{
    public string SampleDate;
    public float Oxygen;
    public float Temperature;
};

public class DataContainer
{
    public static BayData[] Data =
    {
       new BayData() { SampleDate = "06/01/20", Oxygen = 126.0f, Temperature = 20.42f  },
       new BayData() { SampleDate = "06/02/20", Oxygen =  73.4f, Temperature = 18.65f  },
       new BayData() { SampleDate = "06/03/20", Oxygen =  79.7f, Temperature = 19.95f  },
       new BayData() { SampleDate = "06/04/20", Oxygen =  65.0f, Temperature = 21.13f  },
       new BayData() { SampleDate = "06/05/20", Oxygen =  68.7f, Temperature = 21.68f  },
       new BayData() { SampleDate = "06/06/20", Oxygen =  56.4f, Temperature = 22.31f  },
       new BayData() { SampleDate = "06/07/20", Oxygen =  86.5f, Temperature = 22.97f  },
       new BayData() { SampleDate = "06/08/20", Oxygen = 100.0f, Temperature = 22.82f  },
       new BayData() { SampleDate = "06/09/20", Oxygen =  80.0f, Temperature = 22.96f  },
       new BayData() { SampleDate = "06/10/20", Oxygen =  69.2f, Temperature = 22.76f  },
       new BayData() { SampleDate = "06/11/20", Oxygen =  75.2f, Temperature = 24.47f  },
       new BayData() { SampleDate = "06/12/20", Oxygen =  68.8f, Temperature = 23.88f  },
       new BayData() { SampleDate = "06/13/20", Oxygen =  72.2f, Temperature = 23.4f   },
       new BayData() { SampleDate = "06/14/20", Oxygen =  81.3f, Temperature = 22.63f  },
       new BayData() { SampleDate = "06/15/20", Oxygen = 111.3f, Temperature = 22.72f  },
    };
}
