using System.Collections.Generic;

namespace BugHelper.Models
{
    public class SoruCevapModel
    {
        public SorularContext SoruCevapContext { get; set; }
        public SorularModel[] SorularModelListe{ get; set; }
        public CevaplarModel[] CevaplarModelListe { get; set; }
        public List<CevaplarModel> CevaplarModelForSoru { get; set; }
        public int SoruId { get; set; }
        public bool FavorideMi = false;
    }
}