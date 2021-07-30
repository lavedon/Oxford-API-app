using System;

namespace oed {
   public class SurfaceOptions { 

       public bool IncludeRegion { get; set; }
       public bool IncludeInflections { get; set; }

       public SurfaceOptions(bool includeRegion, bool includeInflections) {
           this.IncludeRegion = includeRegion;
           this.IncludeInflections = includeInflections;
       }

   }
}