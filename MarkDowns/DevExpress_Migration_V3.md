# DevExpress Control Swap — Analiza (bez novih funkcionalnosti)

> **Cilj ovog dokumenta:** popis **svih Crownwood.DotNetMagic kontrola, tipova i stilskih API-ja** koji se trenutno koriste u Vektoru, te njihovo **1:1 mapiranje na DevExpress WinForms** ekvivalente. Ovo NIJE plan za detach ni bilo kakvo proširenje funkcionalnosti — samo zamjena baznih UI kontrola uz očuvanje **identičnog ponašanja**.
>
> **Odnos prema `DevExpress_Migration_V2.md`:** V2 opisuje detach, `IVvDocumentHost`, `VvFloatingForm`, decoupling `ZXC` infrastrukture. **Ovaj dokument (V3) pokriva samo Fazu 2, 3 (djelomično) i 5 iz V2** — sam "swap" kontrola. Apstrakcije iz V2 su **preporučene kao prethodni korak** ali nisu obavezne za uspješnu zamjenu Crownwood → DevExpress ako se ograničavamo na trenutnu funkcionalnost.

---

## 1. Inventar Crownwood tipova u projektu

Svi Crownwood tipovi dolaze iz dva namespace-a:

- `Crownwood.DotNetMagic.Forms`
- `Crownwood.DotNetMagic.Controls`
- `Crownwood.DotNetMagic.Common` (VisualStyle enum i boje)

### 1.1 Direktno naslijeđeni Crownwood tipovi (u Vektor bazne klase)

| Vektor klasa | Datoteka | Nasljeđuje | Broj instanci | Napomena |
|---|---|---|---|---|
| `VvForm` | `zVvForm\*.cs` (partial, ~7 fileova) | `Crownwood.DotNetMagic.Forms.DotNetMagicForm` | 1 (singleton via `ZXC.TheVvForm`) | Glavna forma, skin, caption style |
| `VvTabPage` | `Framework\VvTabPage.cs` | `Crownwood.DotNetMagic.Controls.TabPage` | N (po jedan po otvorenom modulu) | TabPage za glavni `TheTabControl` na `VvForm`-u |
| `VvInnerTabPage` | `Framework\VvTabPage.cs` (kraj fajla) | `Crownwood.DotNetMagic.Controls.TabPage` | N (unutar `VvRecordUC.TheTabControl`) | "Unutarnji" tabovi unutar DUC-ova (npr. ReportViewer tab, grid tab) |

### 1.2 Direktno instancirani Crownwood tipovi

| Crownwood tip | Mjesto u kodu | Uloga |
|---|---|---|
| `Crownwood.DotNetMagic.Controls.TabControl` | `VvForm.TheTabControl` (glavni kontejner tabova), `VvRecordUC.TheTabControl` (per-DUC unutarnji tab kontejneri), `FUG_PTG_UC.ThePolyGridTabControl`, `FakturExtDUC.TheTabControl` | Glavni i unutarnji tab kontejneri |
| `Crownwood.DotNetMagic.Controls.TreeControl` | `VvForm.TreeView_Modul` (lijevi/desni modul panel) | Modul navigacijsko stablo |
| `Crownwood.DotNetMagic.Common.VisualStyle` | `VvColors.cs`, `VvColorsStylsDlg.cs` | Enum za odabir skina (`IDE2005`, `Office2007Blue`, ...) |
| `Crownwood.DotNetMagic.Controls.TabPage.Selected` property | `VvTabPage` konstruktor, `VvTabPage_VisibleChanged` | Aktivacija taba |
| `Crownwood.DotNetMagic.Controls.TabPage.Title` / `.Image` | `VvTabPage.PaliGasiDirtyFlag()`, `VvInnerTabPage` ctor | Tekst i ikona taba |

### 1.3 Crownwood API pozivi raspršeni po kodu

Pretraga `Crownwood.` rezultirala je pojavama u sljedećim fajlovima (osim gore navedenih baznih):

- `UtilsEtc\VvColors.cs` — cijeli sustav skin-boja vezan na `VisualStyle`
- `UtilsEtc\VvColorsStylsDlg.cs` — dialog za odabir skina
- `Framework\VvUserControlRecord_Sub.cs` (oko linija 1897–1909) — referenca na `Crownwood.TabControl` (vjerojatno za select / kolorizaciju)
- `VvUC\PrjUC\SkyRuleUC.cs` (oko linija 1079–1102) — `Crownwood.TabControl` kao član UC-a
- `VvUC\PlaUC\PersonUC.cs` (oko linija 140–152, 390–398, 890–893) — unutarnji `Crownwood.TabControl` u UC-u
- `VvUC\MixerUC\ZahtjeviDUC.cs`, `VvUC\MixerUC\UgovoriDUC.cs` — `Crownwood.TabControl` na DUC razini
- `VvUC\FinUC\Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs` — `Crownwood.TabControl`
- `UtilsEtc\VvAboutBox.cs` — vjerojatno `TabControl` za "About" tabove

**Ukupna procjena:** ~15–20 datoteka direktno referencira Crownwood tipove. Većina preko `VvTabPage` / `VvInnerTabPage` / `TheTabControl` — što znači da ako apstrakciju izvedemo ispravno, broj **direktnih dodira** Crownwood API-ja ostaje u jednoznamenkastom rasponu po fajlu.

---

## 2. Mapiranje Crownwood → DevExpress (1:1, bez novih feature-a)

Cilj je očuvati **identičnu funkcionalnost**. DevExpress nudi bogatiju API površinu, ali ju ne koristimo u ovoj fazi.

| Crownwood tip / API | DevExpress zamjena | Namespace | Komentar |
|---|---|---|---|
| `Crownwood.DotNetMagic.Forms.DotNetMagicForm` | `DevExpress.XtraEditors.XtraForm` | `DevExpress.XtraEditors` | Najjednostavniji izbor. Alternativa `RibbonForm` nije relevantna jer ne koristimo Ribbon |
| `Crownwood.DotNetMagic.Controls.TabControl` (glavni `TheTabControl`) | `DevExpress.XtraTab.XtraTabControl` | `DevExpress.XtraTab` | **Za V3 preferiramo `XtraTabControl`** (ne `DocumentManager`), jer `DocumentManager` uvodi floating/detach ponašanje koje u ovoj fazi **eksplicitno ne želimo** |
| `Crownwood.DotNetMagic.Controls.TabControl` (unutarnji tab kontejneri u UC-ovima) | `DevExpress.XtraTab.XtraTabControl` | `DevExpress.XtraTab` | Isti tip, manji obim |
| `Crownwood.DotNetMagic.Controls.TabPage` | `DevExpress.XtraTab.XtraTabPage` | `DevExpress.XtraTab` | 1:1 zamjena. `Title` → `Text`, `Selected = true` → `TabControl.SelectedTabPage = this` |
| `Crownwood.DotNetMagic.Controls.TreeControl` | `DevExpress.XtraTreeList.TreeList` ili `DevExpress.XtraBars.Navigation.AccordionControl` | `DevExpress.XtraTreeList` / `DevExpress.XtraBars.Navigation` | **Preporuka:** `TreeList` — najbliži API TreeControl-u (čvorovi, ikone, expand/collapse). `AccordionControl` je preoptimiziran i mijenja UX |
| `Crownwood.DotNetMagic.Common.VisualStyle` (enum) | `DevExpress.Skins.SkinManager` + `UserLookAndFeel.Default.SkinName` | `DevExpress.Skins`, `DevExpress.LookAndFeel` | Mapiranje pojedinih stilova u DevExpress skinove (vidi §4) |

**Ključne odluke:**

1. **`XtraTabControl`, NE `DocumentManager`** — `DocumentManager` + `TabbedView` se koristi u V2 za detach. U V3 ostajemo s jednostavnim `XtraTabControl` koji ima gotovo identičan API kao `Crownwood.TabControl` (TabPages kolekcija, `SelectedTabPage`, `SelectedTabPageIndex`, `SelectedIndexChanged` event, per-tab `Image`).
2. **`XtraForm`, NE `RibbonForm`** — projekat koristi klasičan `MenuStrip` + `ToolStrip` sustav (vidi §3). U V3 ga **ne diramo** (ostaje WinForms `MenuStrip`/`ToolStrip`). Migraciju na `BarManager` odgađamo jer spada u V2 Fazu 3.
3. **`TreeList`, NE `NavBarControl`** — najmanji rizik, najviše sličnosti s postojećim `TreeControl`.

---

## 3. Što se **NE mijenja** u V3 (eksplicitno)

Ovo je **ključno** da se obim ove faze ne proširi:

- **`MenuStrip` i svi `ToolStrip`-ovi ostaju WinForms** (`ts_Record`, `ts_Modul`, `ts_Report`, `ats_SubModulSet[][]`). Migracija na `BarManager` = posao V2 Faze 3.
- **`VvHamper` sustav layouta ostaje** (koristi `ZXC.QUN`, `ZXC.Redak`, `ZXC.Kolona`). Uklanjanje = V2 Faza 5.
- **`ZXC.TheVvForm` singleton ostaje** (nema `IVvDocumentHost`, nema decouplinga `PrjConnection`/`VvDB_Prefix`).
- **Ponašanje `VvTabPage_VisibleChanged` ostaje bit-po-bit identično** — samo bazna klasa se mijenja.
- **Nema detach-a, nema floating formi, nema multi-window.**
- **`VvTabPage.thisIsFirstAppereance`, `TsbEnabled` snapshot, `VvTabPage_Validating` za arhivu — sve ostaje.**
- **Crystal Reports `BackgroundWorker` na `VvTabPage` — ne dira se.**
- **`ZXC.*_InProgress` zastavice — ne diraju se** (one postaju per-host tek u V2).

---

## 4. Detalji mapiranja po kontroli

### 4.1 `DotNetMagicForm` → `XtraForm`

    // Prije (VvForm.cs):
    public partial class VvForm : Crownwood.DotNetMagic.Forms.DotNetMagicForm { ... }

    // Nakon:
    public partial class VvForm : DevExpress.XtraEditors.XtraForm { ... }

**Property diff (ono što `DotNetMagicForm` eksponira, a `XtraForm` nudi drugačije):**

| DotNetMagicForm property | XtraForm ekvivalent |
|---|---|
| `Style` (VisualStyle enum) | `UserLookAndFeel.Default.SkinName` (globalno) |
| `InertForm` / `CaptionBarColor` | Upravlja Skin; custom boja = `LookAndFeel.UseDefaultLookAndFeel=false` + posebni skin |
| `WindowActive`, `WindowInactive` boje | Skin |

**Rizik:** Boje title bar-a i okvira prelaze u ruke Skin sustava. Ako klijenti inzistiraju na određenim bojama, treba odabrati Skin + primijeniti `SkinSvgPalette` ili custom `UserLookAndFeel`.

### 4.2 `Crownwood.TabControl` → `XtraTabControl`

    // Prije (VvForm.Designer.cs ili programatski):
    this.TheTabControl = new Crownwood.DotNetMagic.Controls.TabControl();
    this.TheTabControl.Style = VisualStyle.Office2007Blue;
    this.TheTabControl.TabPages.Add(vvTabPage);
    this.TheTabControl.SelectedIndexChanged += ...;

    // Nakon:
    this.TheTabControl = new DevExpress.XtraTab.XtraTabControl();
    // Style ide preko skina (per-control override opcionalan)
    this.TheTabControl.TabPages.Add(vvTabPage);
    this.TheTabControl.SelectedPageChanged += ...;   // NAPOMENA: event ima drugačije ime i args!

**Event mapping:**

| Crownwood event | XtraTabControl event |
|---|---|
| `SelectedIndexChanged` (EventArgs) | `SelectedPageChanged` (`TabPageChangedEventArgs`) — daje i Page i PrevPage |
| `Selecting` (cancelable) | `SelectedPageChanging` (cancelable) |
| (TabPage) `Selected = true` | `tabControl.SelectedTabPage = tabPage` |
| `TabControl.SelectedIndex` | `XtraTabControl.SelectedTabPageIndex` |

**Važno:** `VvTabPage_Validating` (za blokadu izlaska iz arhive) je `CancelEventHandler` na **samom tab page-u**. U DevExpress modelu, validacija se radi preko `XtraTabControl.SelectedPageChanging` eventa na **kontroli**, s testom `e.Page` / `e.PrevPage`. Treba presjeći logiku iz `VvTabPage.VvTabPage_Validating` i prebaciti je u handler na `XtraTabControl`-u ili ostaviti per-page koristeći `XtraTabPage.Validating` (postoji, ali se ne diže pri klik-promjenama).

### 4.3 `Crownwood.TabPage` → `XtraTabPage`

    // Prije:
    public class VvTabPage : Crownwood.DotNetMagic.Controls.TabPage, IDisposable

    // Nakon:
    public class VvTabPage : DevExpress.XtraTab.XtraTabPage, IDisposable

**Property diff:**

| Crownwood.TabPage | XtraTabPage |
|---|---|
| `Title` | `Text` |
| `Image` (Bitmap) | `ImageOptions.Image` (Image) — trenutni `VvIco.Dirty.ToBitmap()` radi |
| `Selected` (bool, set → aktivira tab) | Nema direktnog setera; ide preko `parentTabControl.SelectedTabPage = this` |
| `VisibleChanged` event (UC lifecycle!) | **KRITIČNO — vidi §5.1** |

### 4.4 `VvInnerTabPage` → `XtraTabPage`

Jednak pattern kao `VvTabPage`, ali ~90 redaka ukupno. Očekivani dotak: promjena base tipa, i promjena `Title` → `Text`. `AutoScroll = true` radi identično.

### 4.5 `Crownwood.TreeControl` → `XtraTreeList.TreeList`

    // Trenutno (TreeView_Modul na VvForm):
    Crownwood.DotNetMagic.Controls.TreeControl tc = this.TreeView_Modul;
    Crownwood.DotNetMagic.Controls.Node node = new Node("Moduli");
    node.Nodes.Add(subNode);

    // Nakon (XtraTreeList):
    TreeList tl = this.TreeView_Modul;
    // TreeList koristi data-bound model (DataSource ili AppendNode)
    TreeListNode root = tl.AppendNode(new object[] { "Moduli" }, -1);
    tl.AppendNode(new object[] { "SubModul" }, root);

**Napomena:** `TreeList` ima **kolonski model** (kao grid). Za jednu kolonu konfigurira se jedna `TreeListColumn`. `AfterCheck`, `FocusedNodeChanged`, `CustomDrawNodeCell` eventi su ekvivalenti Crownwood `AfterSelect` / `DrawNode` eventa.

Alternativa: **`System.Windows.Forms.TreeView`** (native) — API je gotovo identičan Crownwood `TreeControl`-u i može biti **najjednostavnija zamjena** ako ne trebamo DevExpress izgled na tom panelu. **Preporuka: razmotriti native `TreeView`** za `TreeView_Modul` kako bismo smanjili rizik, jer modul-panel nije vizualno istaknuta komponenta.

### 4.6 `VisualStyle` enum → DevExpress Skin

Trenutni `VvColorsStylsDlg.cs` omogućuje useru odabir stila iz `Crownwood.Common.VisualStyle` enuma (npr. `IDE2005`, `Office2007Blue`, `Office2003`). Nakon migracije ovo postaje:

    // Prije:
    ZXC.TheVvForm.Style = VisualStyle.Office2007Blue;

    // Nakon:
    UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful);
    // ili, za dijalog odabira:
    new DefaultLookAndFeel().LookAndFeel.SkinName = "The Bezier";

**Mapiranje stilova (prijedlog):**

| Crownwood VisualStyle | DevExpress Skin (prijedlog) |
|---|---|
| `IDE2005` | `Visual Studio 2013 Light` |
| `Office2003` | `Office 2007 Silver` (DevExpress nema 2003) |
| `Office2007Blue` | `Office 2019 Colorful` |
| `Office2007Black` | `Dark Side` ili `The Bezier` |
| `Office2007Silver` | `Office 2019 Black` |

`VvColors.cs` i `VvColorsStylsDlg.cs` trebaju biti **u cijelosti preispisani** — umjesto mapiranja boja po nazivu stila, koriste se DevExpress skinovi. `VvEnvironmentDescriptor` perzistira `Style` string → migracijska konverzija: stari string → novi skin name (lookup tablica pri load-u).

---

## 5. Mjesta posebnog rizika

### 5.1 `VvTabPage.VisibleChanged` — OPSTAJE, ali sa zamjenom

**`Crownwood.TabPage` diže `VisibleChanged` kada tab postane aktivan/neaktivan**. To je **ključ kompletnog toolbar-switching mehanizma** (vidi V2 §1.3). `XtraTabPage` također ima `VisibleChanged`, ali semantika može biti drugačija jer DevExpress koristi lazy-render.

**Preporučeni pristup za V3:** NE oslanjati se na `XtraTabPage.VisibleChanged`. Umjesto toga, handle `XtraTabControl.SelectedPageChanged` event na parent kontroli i unutar njega ručno pozvati `((VvTabPage)e.Page).OnActivated()` i `((VvTabPage)e.PrevPage).OnDeactivated()` — gdje su te dvije metode točan izvod postojećeg `VvTabPage_VisibleChanged` koda podijeljenog na dvije grane (`if (Visible == false)` grana = `OnDeactivated`, `else` grana = `OnActivated`).

Ovo je **najznačajnija refactor operacija V3 faze** i mora se izvesti oprezno jer `VvTabPage_VisibleChanged` ima oko 100 redaka specijalne logike koja:

- Čuva/vraća snapshot enabled stanja toolbar gumba (`GetTSB_EnabledStateSnapshot` / `PutTSB_EnabledStateSnapshot`)
- Mijenja vidljivost `ats_SubModulSet[i][j]` toolstripova
- Mijenja `ts_Report.Visible` / `ts_Record.Visible`
- Poziva `TheVvForm.TH_CheckAndForceFiskalization()`
- Upravlja `VvDataBaseInfoInUse` promjenama između tabova
- Poseban handling za `F2_Izlaz_UC` / `F2_Ulaz_UC` refresh
- Za REPORT tab: `SetReportComboBox()`, `SetVisibilitiOfReportModulButton()`

Cilj: extraktati u `public void OnActivated()` i `public void OnDeactivated()` metode na `VvTabPage`, zadržati identičnu logiku, samo promijeniti trigger.

### 5.2 `Selected` property assignment

Na puno mjesta u kodu stoji:

    this.Selected = true;                                         // u VvTabPage konstruktoru
    TheVvForm.TheTabControl.SelectedIndex = ArhivaTabPageSelectedIndex;   // u VvTabPage_Validating

`XtraTabPage` **nema `Selected` setter**. Mora se zamijeniti s:

    ((XtraTabControl)this.Parent).SelectedTabPage = this;

i

    TheVvForm.TheTabControl.SelectedTabPageIndex = ArhivaTabPageSelectedIndex;

### 5.3 `TabPages.Add(this)` u `VvTabPage` konstruktoru

Trenutno:

    if(parentTabControl != null) parentTabControl.TabPages.Add(this);

DevExpress ima `XtraTabControl.TabPages` kolekciju (tip `XtraTabPageCollection`), API je isti (`.Add(page)`, `.Remove(page)`). **Nije problem.**

### 5.4 `VvTabPage.Image = VvIco.Dirty.ToBitmap()`

U DevExpress:

    this.ImageOptions.Image = VvIco.Dirty.ToBitmap();

Postojeća metoda `PaliGasiDirtyFlag` mora se prilagoditi samo u jednom retku.

### 5.5 Designer vs programatsko kreiranje

`VvForm` je **programatski kreirana** (partial class, ali bez Designer fajla za TheTabControl — sve se radi u `InitializeComponent()` ili ručnim kodom). Ovo je prednost za migraciju jer **nema .resx binarnih referenci na Crownwood tipove** koje bi se mogle razbiti.

**Provjeriti:** postoji li `VvForm.Designer.cs` sa `Crownwood.*` tipovima. Ako postoji, treba otvoriti `.resx` fajlove i ukloniti Crownwood-specific serijalizirane resurse.

### 5.6 Unutarnji `TheTabControl` u UC-ovima

Kod poput:

    ((VvInnerTabPage)(this.TheVvRecordUC.TheTabControl.SelectedTab)).TheInnerTabPageKindEnum

pristupa `SelectedTab` property-ju Crownwood `TabControl`-a. DevExpress ekvivalent je `SelectedTabPage`. **Ime property-ja se mijenja, tip rezultata se mijenja** — svaki ovakav cast treba revidirati. Mjesta s ovakvim kodom (prema pretrazi u §1.3):

- `Framework\VvTabPage.cs`
- `Framework\VvUserControlRecord_Sub.cs`
- `VvUC\PrjUC\SkyRuleUC.cs`
- `VvUC\PlaUC\PersonUC.cs`
- `VvUC\MixerUC\ZahtjeviDUC.cs`, `UgovoriDUC.cs`
- `VvUC\FinUC\Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs`
- `VvUC\RiskUC\PTG_DUC.cs`
- `UtilsEtc\VvAboutBox.cs`

### 5.7 `VvHamper.ApplyVVColorAndStyleTabCntrolChange(this)`

Ova metoda uzima `VvTabPage` i aplicira boje/stilove **na Crownwood API**. Signatura metode se mora prilagoditi da radi s `XtraTabPage` ili prosljeđuje `XtraTabControl`. Kako je VvHamper cijeli layout sistem i mora ostati (§3), **ne mijenjamo ga u cijelosti** — samo tu jednu metodu.

### 5.8 `Fld_Col8 { set { labSent.Visible = value; } }` i ostali Fld_ setteri

Oni ne dodiruju Crownwood — čisti WinForms `Label` / `VvTextBox`. **Nema dodira** s migracijom.

### 5.9 NuGet paketi i licenca

Potrebno je dodati sljedeće DevExpress NuGet pakete (ili referencirati DLL-ove iz DevExpress install foldera):

    DevExpress.Win.Design
    DevExpress.Win.Navigation        (XtraTab)
    DevExpress.Win.TreeList          (TreeList)
    DevExpress.Data
    DevExpress.Utils
    DevExpress.Images
    DevExpress.XtraEditors           (XtraForm)

Projekt cilja **.NET Framework 4.8**, verzija DevExpress-a mora biti kompatibilna (bilo koja aktuelna DX verzija 20.x+ podržava .NET Framework 4.5.2+).

Licenca: osigurati `licenses.licx` file u projektu sa unosima za korištene DevExpress komponente kako ne bi bilo runtime exceptiona o tamper-check-u licence.

---

## 6. Plan izvedbe (koraci u redoslijedu)

### Korak 1 — Infrastructural setup
1. Dodati DevExpress NuGet pakete
2. Dodati `licenses.licx`
3. Konfigurirati `UserLookAndFeel.Default.SkinName` pri startup-u (u `Program.cs` `Main()`)
4. Build solucije **bez ikakve druge izmjene** — provjera da DevExpress reference ne lome kompilaciju

### Korak 2 — VvForm bazna klasa
1. Zamijeniti `Crownwood.DotNetMagic.Forms.DotNetMagicForm` → `DevExpress.XtraEditors.XtraForm`
2. Očistiti sve property-je koji više ne postoje (`Style`, `InertForm` ako postoji)
3. Build + smoke test (forma se otvara, MenuStrip/ToolStrip rade normalno jer ih ne diramo)

### Korak 3 — Glavni `TheTabControl`
1. Zamijeniti `Crownwood.TabControl` → `XtraTabControl` u kodu gdje se kreira na `VvForm`
2. Promijeniti event wire-up: `SelectedIndexChanged` → `SelectedPageChanged`
3. Promijeniti sve pristupe `SelectedIndex`/`SelectedTab` → `SelectedTabPageIndex`/`SelectedTabPage`
4. Privremeno komentirati `VisibleChanged` subscription u `VvTabPage` konstruktoru
5. Premjestiti logiku iz `VvTabPage_VisibleChanged` u dvije nove metode: `OnActivated()` i `OnDeactivated()`
6. Dodati handler na `XtraTabControl.SelectedPageChanged` koji zove te dvije metode
7. Testirati kompletan tab-switching workflow (toolbar, menu, SubModul visibility)

### Korak 4 — `VvTabPage` bazna klasa
1. `Crownwood.DotNetMagic.Controls.TabPage` → `DevExpress.XtraTab.XtraTabPage`
2. `Title` → `Text` (refactor svih assignmenta i readera)
3. `Selected = true` → routing preko `SelectedTabPage` setter-a na parent kontroli
4. `Image = …` → `ImageOptions.Image = …` u `PaliGasiDirtyFlag`
5. Ukloniti `VisibleChanged` subscription (premješten u Korak 3)
6. Testirati arhiva validacija — `VvTabPage_Validating` treba migrirati u `XtraTabControl.SelectedPageChanging` handler

### Korak 5 — `VvInnerTabPage` bazna klasa
1. Isto kao VvTabPage, samo manji obim
2. Za svaki `UC.TheTabControl` koji se koristi kao `Crownwood.TabControl`, mijenjati na `XtraTabControl` — lokalno u konstruktoru UC-a

### Korak 6 — Svi UC-ovi s unutarnjim tab control-ima
Datoteke po prioritetu (najmanji → najveći rizik):

1. `UtilsEtc\VvAboutBox.cs` — izolirana, jednostavna
2. `VvUC\PlaUC\PersonUC.cs`
3. `VvUC\MixerUC\UgovoriDUC.cs`, `ZahtjeviDUC.cs`
4. `VvUC\PrjUC\SkyRuleUC.cs`
5. `VvUC\FinUC\Fin_Dlg_UC.cs`, `Fin_Dlg_UC_Q.cs`
6. `VvUC\RiskUC\PTG_DUC.cs`
7. `Framework\VvUserControlRecord_Sub.cs` (koristi `TheTabControl` preko `VvRecordUC`)
8. `VvUC\FakturExtDUC` (ima `TheTabControl` za zoom)
9. `VvUC\FUG_PTG_UC` (ima `ThePolyGridTabControl`)

### Korak 7 — `TreeView_Modul`
1. Zamijeniti `Crownwood.TreeControl` → `TreeList` (ili native `TreeView`, ako je sigurnije)
2. Ponovno osposobiti node-building logiku
3. Testirati klikove na module, expand/collapse, ikone

### Korak 8 — Skin / Visual style sustav
1. Izbaciti `Crownwood.Common.VisualStyle` iz `VvColors.cs` i `VvColorsStylsDlg.cs`
2. Napraviti mapping tablicu staro-ime → novi-skin
3. Prilagoditi `VvEnvironmentDescriptor` load/save (migracija starih vrijednosti)
4. `VvColorsStylsDlg` — nova lista DevExpress skinova umjesto Crownwood stilova

### Korak 9 — Final cleanup
1. Ukloniti sve `using Crownwood.DotNetMagic.*` direktive
2. Ukloniti Crownwood DLL iz references
3. Ukloniti `Crownwood.*.dll` iz deploy scripta
4. Full regression test svakog modula (FIR, FUR, plaće, amortizacija, izvještaji)

---

## 7. Procjena težine

| Korak | Obim (fajlovi) | Težina | Dani |
|---|---|---|---|
| 1. NuGet/licenca setup | 2–3 | Niska | 0.5 |
| 2. VvForm base swap | 1 klasa (7 partial fajlova) | Niska | 0.5 |
| 3. Glavni TheTabControl + VisibleChanged extrakcija | ~3 fajla | **Visoka** (srce sustava) | 2–3 |
| 4. VvTabPage base swap | 1 fajl | Srednja | 1 |
| 5. VvInnerTabPage base swap | 1 fajl | Niska | 0.5 |
| 6. Svi UC-ovi s unutarnjim tab-ovima | ~10 fajlova | Srednja | 2–3 |
| 7. TreeView_Modul swap | 1–2 fajla | Srednja | 1 |
| 8. Skin sustav | 2 fajla + mapiranje | Srednja | 1 |
| 9. Cleanup + regression | cijela solucija | Niska | 1–2 |
| **Ukupno** | | | **9–13 dana** |

**Usporedba s V2:** V2 Faza 2 (Tab Control) + Faza 5 (Crownwood Removal) procjenjuje 7–11 dana. V3 je komplementaran tim procjenama — ovdje je **gusćim redoslijedom** izveden zato što preskačemo apstrakcijsku fazu (Fazu 1) koja u V2 dodaje 12–17 dana.

---

## 8. Preporuka redoslijeda V2 vs V3

Dvije moguće strategije:

### Strategija A — "Swap first, then detach"
1. Prvo izvesti **V3 (ovaj dokument)** — zamijeniti Crownwood s DevExpress bez novih funkcionalnosti (~9–13 dana)
2. Zatim izvesti **V2 Phase 1** — decoupling `ZXC`, `IVvDocumentHost`, `VvToolbarFactory` (~12–17 dana)
3. Zatim **V2 Phase 3 + 4** — BarManager zamjena za MenuStrip/ToolStrip + detach (~10–16 dana)

**Prednost:** Brzo uklanjanje Crownwood-a (tehnički dug riješen). Detach dolazi kasnije ali na čistoj DX bazi.

**Mana:** Duplicirano dirajemo `VvTabPage_VisibleChanged` — jednom u V3 (extrakcija u `OnActivated/OnDeactivated`), drugi put u V2 (routing preko `IVvDocumentHost`).

### Strategija B — "Decouple first, then swap, then detach"
1. **V2 Phase 1** apstrakcije prvo
2. **V3 swap** — s već spremnim `IVvDocumentHost` pattern-om, swap je manje rizičan
3. **V2 Phase 3 + 4** — BarManager i detach

**Prednost:** Manji tehnički rizik, bez dvostruko diranja istog koda.

**Mana:** Dulji period tijekom kojeg postoji i Crownwood i nove apstrakcije — cognitive overhead.

**Osobna preporuka:** **Strategija A** ako ti je prioritet brzo riješiti "zastarjeli library" aspekt. **Strategija B** ako ti je prioritet čista detach funkcionalnost u budućnosti.

---

## 9. Sažetak (TL;DR)

- **5 Crownwood tipova** treba zamijeniti: `DotNetMagicForm`, `TabControl`, `TabPage`, `TreeControl`, `VisualStyle` enum.
- **Preporučene DX zamjene:** `XtraForm`, `XtraTabControl`, `XtraTabPage`, `TreeList`, `SkinManager`.
- **NE mijenjamo u ovoj fazi:** MenuStrip, ToolStrip, VvHamper, ZXC singleton, detach.
- **Najveći rizik:** extrakcija `VvTabPage_VisibleChanged` u `OnActivated`/`OnDeactivated` + reroute preko `SelectedPageChanged`.
- **Procjena:** 9–13 radnih dana za čisti swap.
- **Sinergija s V2:** V3 pokriva Fazu 2 + 5 iz V2; detach (V2 Faza 4) ostaje za kasnije. 