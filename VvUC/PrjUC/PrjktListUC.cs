using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using System.Collections.Generic;

public class PrjktListUC : KupdobListUC
{

   public PrjktListUC(Control parent, Prjkt _prjkt, VvForm.VvSubModul vvSubModul) : base(parent, _prjkt, vvSubModul)
   {
      this.MasterSubModulEnum = ZXC.VvSubModulEnum.PRJ;
      this.TheSubModul        = vvSubModul;
      this.Parent.Text        = this.TheSubModul.subModul_name;
   }

   protected override void InitializeFindFormSpecifics()
   {
      recordSorter = Prjkt.sorterNaziv;

      this.ds_prjkt = new Vektor.DataLayer.DS_FindRecord.DS_findPrjkt();

      this.Name = "PrjktListUC";
      this.Text = "PrjktListUC";
   }

   private Vektor.DataLayer.DS_FindRecord.DS_findPrjkt ds_prjkt;

   protected override DataSet VirtualUntypedDataSet { get { return ds_prjkt; } }
  
}