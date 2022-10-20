using System;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Data.OleDb;

public struct PNRrasterStruct
{
   public DateTime PutDate    { get; set; }
   public string   Relacija   { get; set; }
   public int      KmStart    { get; set; }
   public int      KmEnd      { get; set; }
   public int      KmDiff     { get; set; }
   public string   BenzPumpa  { get; set; }
   public int      KmOnPumpa  { get; set; }
   public decimal  BenzLitara { get; set; }
}

public sealed class PNRraster
{
   #region Propertiez

   public List<PNRrasterStruct> ThePNRrasterList;
   public string                dateRasterMMYYYY;
   public string                napomena;
   public uint                  personCD;
   public string                voziloCD;
   public Person                thePerson;
   public VvLookUpItem          theVoziloLUI;

   public string FullPathFileName { get; set; }
   public string FileName         { get; set; }
   public string DirectoryName    { get; set; }

   public int? NumOfPNRrasterFileLines { get; private set; }

   private bool linesOK = true, fileOK = true;
   public  bool personOK = false, voziloOK = false;

   public bool BadData
   {
      get
      {
         return (!fileOK || !linesOK /*|| !personOK || !voziloOK*/);
      }
   }

   #endregion Propertiez

   #region Constructor

   public PNRraster(string _fullPathFileName, string _sheetName)
   {
      DirectoryInfo dInfo;

      FullPathFileName = _fullPathFileName;
      dInfo            = new DirectoryInfo(FullPathFileName);
      FileName         = dInfo.Name;
      DirectoryName    = FullPathFileName.Substring(0, FullPathFileName.Length - (FileName.Length + 1));

      NumOfPNRrasterFileLines = LoadPNRrasterFromExcel(FullPathFileName, _sheetName);

      if(NumOfPNRrasterFileLines == null) { fileOK = false; return; }

      //if(NumOfVipRasterFileLines < 5 || NumOfVipRasterFileLines - NumOfTransLines != 4) linesOK = false;
      //else                                                                              linesOK = true;
   }

   #endregion Constructor

   #region Methods

   private int? LoadPNRrasterFromExcel(string _fName, string _sheetName)
   {
      int? numOfExcelLines = 0;

      ThePNRrasterList = new List<PNRrasterStruct>();

      //http://www.connectionstrings.com/ 

      //string connectionString03  = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + dataSource2003 + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
      //string connectionString07  = @"Provider=Microsoft.ACE.OLEDB.12.0; Data Source=" + dataSource2007 + @"; Extended Properties=""Excel 12.0 Xml;HDR=YES;IMEX=1""";

    //string connectionStringVIP = @"Provider=Microsoft.Jet.OLEDB.4.0;  Data Source=" + _fName + @"; Extended Properties=""Excel 8.0;HDR=Yes;IMEX=1""";
      string connectionStringVIP = ZXC.GetExcelConnectionString(_fName, true, true);

      //string sheetName = "Sheet1";

      PNRrasterStruct PNRraster_rec;

      ZXC.LoadImportFile_HasErrors = false;
      uint line = 0;
      try
      {
         using(OleDbConnection connection = new OleDbConnection(connectionStringVIP))
         {
            using(OleDbCommand command = connection.CreateCommand())
            {
               // Qwerty$ comes from the name of the worksheet
               //command.CommandText = "SELECT * FROM [Qwerty$]";
               command.CommandText = @"SELECT * FROM [" + _sheetName + @"$]";

               connection.Open();

               using(OleDbDataReader reader = command.ExecuteReader())
               {
                  while(reader.Read())
                  {
                     line++;
                     PNRraster_rec = new PNRrasterStruct();

                     if(line == 1) // prvi red - meta data 
                     {
                      //dateRasterMMYYYY = ZXC.ValOr_01010001_DateTime_Import_MM_YYYY_Format(reader[0].ToString().Replace('_', ' ').Replace('.', ' '));
                        dateRasterMMYYYY =                                                  (reader[0].ToString().Replace("_", "").Replace(".", "").Replace(" ", ""));
                        personCD         = ZXC.ValOrZero_UInt(reader[1].ToString());
                        voziloCD         = reader[3].ToString().Replace(" ", "").ToUpper();

                        thePerson    = ZXC.TheVvForm.TheVvUC.Get_Person_FromVvUcSifrar(personCD);
                        theVoziloLUI = ZXC.luiListaMixerVozilo.SingleOrDefault(lui => lui.Cd.Replace(" ", "").ToUpper() == voziloCD);

                        personOK = thePerson    != null;
                        voziloOK = theVoziloLUI != null;
                     }
                     else if(line == 2) // zaglavlje tablice - skip it 
                     {
                     }
                     else // trans lines 
                     {
                        MixerDao.FillPNRrasterFromExcelDataReader_transLine(ZXC.TheVvForm.TheDbConnection, ref PNRraster_rec, reader, (uint)(++numOfExcelLines));
                        
                        if(PNRraster_rec.PutDate.NotEmpty() /*&& PNRraster_rec.CustID.NotEmpty()*/)
                        {
                           ThePNRrasterList.Add(PNRraster_rec);
                        
                           // some error check: 
                           //if(PNRraster_rec.TotalDug != PNRraster_rec.Transes.Sum(dtr => dtr.T_iznosDug))
                           //{
                           //   ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "Za custCode [" + PNRraster_rec.CustCode + "] razlika po ukupnom iznosu racuna!\n\n" +
                           //                                                           PNRraster_rec.CIme + " " + PNRraster_rec.CPrezime + " " + PNRraster_rec.CNaziv + "\n\n" +
                           //                                                           "Po Excel-u: " + PNRraster_rec.TotalDug + "   Dtrans Sum: " + PNRraster_rec.Transes.Sum(dtr => dtr.T_iznosDug));
                           //   ZXC.LoadImportFile_HasErrors = true;
                           //   break;
                           //}
                        }
                     }
                  } // while(reader.Read()) 
               }
            }
         }
      }
      catch(Exception ex)
      {
         ZXC.aim_emsg(System.Windows.Forms.MessageBoxIcon.Error, "LoadVipRasterFromExcel Exception. " + ex.Message);
         //isOK = false;
      }

      //return numOfExcelLines;
      return ThePNRrasterList.Count();
   }

   #endregion Methods

};

