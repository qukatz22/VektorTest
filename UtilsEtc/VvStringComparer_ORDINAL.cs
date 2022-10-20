/// <summary>
/// Po ASCII npr '5' manje od '<', a u LINQ OrderBy JE OBRNUTO
/// Pa ako oces da ti i LINQ radi po ASCII-ju moras imati Posebnu Comparer Klasu!?
/// Ovo 'ordinal' znaci da i de po UNOCODE-u
/// 
/// The .NET Framework uses three distinct ways of sorting: word sort, string sort, and ordinal sort. Word sort performs a culture-sensitive comparison of strings. 
/// Certain nonalphanumeric characters might have special weights assigned to them. For example, the hyphen ("-") might have a very small weight 
/// assigned to it so that "coop" and "co-op" appear next to each other in a sorted list. String sort is similar to word sort, except that there are no special cases. 
/// Therefore, all nonalphanumeric symbols come before all alphanumeric characters. Ordinal sort compares strings based on the Unicode values of each element of the string.
/// </summary>
using System.Collections.Generic;
public class VvCompareStringsOrdinal : IComparer<string>
{
   // Because the class implements IComparer, it must define a 

   // Compare method. The method returns a signed integer that indicates 

   // whether s1 > s2 (return is greater than 0), s1 < s2 (return is negative),

   // or s1 equals s2 (return value is 0). This Compare method compares strings. 

   public int Compare(string s1, string s2)
   {
      return string.CompareOrdinal(s1, s2);
   }
}
