using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using System.ServiceModel;
using FinaInvoiceB2GENClient.FinaInvoiceWS;


#if MICROSOFT
using                  System.Data.SqlClient;
using XSqlConnection = System.Data.SqlClient.SqlConnection;
using XSqlCommand    = System.Data.SqlClient.SqlCommand;
#else
using MySql.Data.MySqlClient;
using XSqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using XSqlCommand    = MySql.Data.MySqlClient.MySqlCommand;
using System.Collections.Generic;
//tam
using Vektor.Reports.PIZ;
using System.ComponentModel;
#endif


//                         MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER MASTER 
public class LedinekUC : VvUserControl
{
   #region Fieldz

   private VvHamper hamper;

   #endregion Fieldz

   #region Constructor

   public LedinekUC(Control parent, VvSubModul vvSubModul)
   {
      
      this.TheSubModul = vvSubModul;

      SuspendLayout();

      CreateHamper();
      InitializeVvUserControl(parent);

      ResumeLayout();
   }

   #endregion Constructor

   #region  HAMPER

   private void CreateHamper()
   {
      hamper = new VvHamper(1, 1, "",  this, false, ZXC.Q3un, ZXC.Q3un, 0);

      hamper.VvColWdt      = new int[] { ZXC.Q10un * 2};
      hamper.VvSpcBefCol   = new int[] { ZXC.Qun4,    };
      hamper.VvRightMargin = hamper.VvLeftMargin;

      for(int i = 0; i < hamper.VvNumOfRows; i++)
      {
         hamper.VvRowHgt[i]    = ZXC.Q10un * 2;
         hamper.VvSpcBefRow[i] = ZXC.Qun4;
      } 
      hamper.VvBottomMargin = hamper.VvTopMargin;

      
      Label lbl = hamper.CreateVvLabel  (0, 0, "L I J E V I   444", ContentAlignment.MiddleCenter);
      
      lbl.Font      = new Font("comic sans ms", 25, ZXC.vvFont.LargeBoldFont.Style);
      lbl.ForeColor = Color.Red; 
      lbl.BackColor = Color.LightYellow;    
    }

    #endregion  HAMPER

    public override void GetFields(bool fuse)
    {
        // notin to do;
    }

}

