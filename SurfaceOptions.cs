#nullable enable
#nullable disable warnings
using System;

namespace oed {
   public class SurfaceOptions { 

       private string _form;

       public bool IncludeRegion { get; set; }
       public bool IncludeInflections { get; set; }
       public string? Form { 
           get {
               return _form;
           } 
           set {
               _form = value.Trim().ToLower();
           }
       }

       public SurfaceOptions(bool includeRegion, bool includeInflections, string form, string part_Of_Speech) {
           this.IncludeRegion = includeRegion;
           this.IncludeInflections = includeInflections;
           this.Form = form;
       }

   }
}