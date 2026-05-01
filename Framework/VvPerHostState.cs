// =============================================================================
// VvPerHostState.cs
//
// Faza 1e (C15): Per-host flag bucket (DevExpress_Migration_V4.md §1.14, §3.1e).
//
// Drzi ~20 *_InProgress flagova klasificiranih kao "Per-DocumentHost" u §1.14:
//   - Record-level ops (5)
//   - Cross-DUC copy   (3)
//   - UI state         (5)
//   - RISK field ops   (7)
//
// Option B disciplina (isti princip kao C10 za ApplyWriteMode):
//   - U C15 SAMO infrastruktura: klasa postoji, IVvDocumentHost je izlaze,
//     ResetAll path iterira DocumentHosts. Tijela flagova (read/write na ~40
//     DUC call-siteovima) JOS uvijek ziva u ZXC staticima — fallback-safe,
//     zero call-site churn.
//   - U Fazi 3 (VvFloatingForm) flipat cemo call-siteove s ZXC.FlagName na
//     host.PerHost.FlagName. Do tada flagovi ostaju globalni, ali ResetAll
//     infrastruktura je vec iskoristiva i testirana.
//
// Vidi i Reset_PerHost_StatusVariables_ForAllHosts() u ZXC.cs — zove
// PerHost.ResetAll() na svakom registriranom hostu (primarni path) + drzi
// defanzivne ZXC static resete (fallback).
// =============================================================================

/// <summary>
/// Per-host flag bucket. Trenutno dormant kontrakt (Faza 1e / C15) — tijela
/// flagova zive u ZXC staticima do Faze 3 flipa. Vidi §1.14, §3.1e.
/// </summary>
public class VvPerHostState
{
   // --- Record-level ops (v. §1.14: Record-level) ---
   public bool RISK_SaveVvDataRecord_inProgress;
   public bool GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress;
   public bool RISK_FinalRn_inProgress;
   public bool RISK_Edit_RtranoOnly_InProgress;
   public bool DupCopyMenu_inProgress;

   // --- Cross-DUC copy (v. §1.14: Cross-DUC copy) ---
   public bool RISK_CopyToOtherDUC_inProgress;
   public bool RISK_CopyToMixerDUC_inProgress;
   public bool MIXER_CopyToOtherDUC_inProgress;

   // --- UI state (v. §1.14: UI state) ---
   public bool RESET_InitialLayout_InProgress;
   public bool MenuReset_SvDUH_ZAHonly_InProgress;
   public bool PutRiskFilterFieldsInProgress;
   public bool DumpChosenOtsList_OnNalogDUC_InProgress;
   public bool LoadIzvodDLG_isON;

   // --- RISK field ops (v. §1.14: RISK field ops) ---
   public bool RISK_ToggleKnDeviza_InProgress;
   public bool RISK_InitZPCvalues_InProgress;
   public bool RISK_PULXPIZX_Calc_InProgress;
   public bool RISK_CheckPrNabDokCij_inProgress;
   public bool RISK_CheckZPCkol_inProgress;
   public bool RISK_PromjenaNacPlac_inProgress;
   public bool RISK_AutoAddInventuraDiff_inProgress;

   // --- Status bar hint backup (Faza 3g) ---
   public string StatusTextBackup;

   /// <summary>
   /// Resetira sve per-host flagove na false. Zove se iz
   /// ZXC.Reset_PerHost_StatusVariables_ForAllHosts() (project-switch putanja)
   /// i kasnije iz UnregisterDocumentHost (Faza 3, detach close).
   /// </summary>
   public void ResetAll()
   {
      RISK_SaveVvDataRecord_inProgress                             = false;
      GetLineFlds_CalcTrans_PutLineFlds_PutSumFlds_FOR_ALL_ROWS_inProgress = false;
      RISK_FinalRn_inProgress                                      = false;
      RISK_Edit_RtranoOnly_InProgress                              = false;
      DupCopyMenu_inProgress                                       = false;

      RISK_CopyToOtherDUC_inProgress                               = false;
      RISK_CopyToMixerDUC_inProgress                               = false;
      MIXER_CopyToOtherDUC_inProgress                              = false;

      RESET_InitialLayout_InProgress                               = false;
      MenuReset_SvDUH_ZAHonly_InProgress                           = false;
      PutRiskFilterFieldsInProgress                                = false;
      DumpChosenOtsList_OnNalogDUC_InProgress                      = false;
      LoadIzvodDLG_isON                                            = false;

      RISK_ToggleKnDeviza_InProgress                               = false;
      RISK_InitZPCvalues_InProgress                                = false;
      RISK_PULXPIZX_Calc_InProgress                                = false;
      RISK_CheckPrNabDokCij_inProgress                             = false;
      RISK_CheckZPCkol_inProgress                                  = false;
      RISK_PromjenaNacPlac_inProgress                              = false;
      RISK_AutoAddInventuraDiff_inProgress                         = false;

      StatusTextBackup                                             = null;
   }
}
