/*using Decal.Adapter;
using Decal.Interop.Core;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using VirindiViewService;
using VirindiViewService.Controls;
*/

using System;
using System.Xml;
using System.IO;

using Decal.Adapter;
using Decal.Adapter.Wrappers;
using MyClasses.MetaViewWrappers;
using System.Text.RegularExpressions;

using VirindiViewService;
using VirindiViewService.Controls;
using VirindiViewService.XMLParsers;


/*
 * Created by Mag-nus. 8/19/2011, VVS added by Virindi-Inquisitor.
 * 
 * No license applied, feel free to use as you wish. H4CK TH3 PL4N3T? TR45H1NG 0UR R1GHT5? Y0U D3C1D3!
 * 
 * Notice how I use try/catch on every function that is called or raised by decal (by base events or user initiated events like buttons, etc...).
 * This is very important. Don't crash out your users!
 * 
 * In 2.9.6.4+ Host and Core both have Actions objects in them. They are essentially the same thing.
 * You sould use Host.Actions though so that your code compiles against 2.9.6.0 (even though I reference 2.9.6.5 in this project)
 * 
 * If you add this plugin to decal and then also create another plugin off of this sample, you will need to change the guid in
 * Properties/AssemblyInfo.cs to have both plugins in decal at the same time.
 * 
 * If you have issues compiling, remove the Decal.Adapater and VirindiViewService references and add the ones you have locally.
 * Decal.Adapter should be in C:\Games\Decal 3.0\
 * VirindiViewService should be in C:\Games\VirindiPlugins\VirindiViewService\
*/

namespace MelksLuminanceTracker
{
  
    //Attaches events from core
	[WireUpBaseEvents]

    //View (UI) handling
    [MVView("MelksLuminanceTracker.mainView.xml")]
    //[MVView("MelksLuminanceTracker.subView.xml")]
    [MVWireUpControlEvents]

	// FriendlyName is the name that will show up in the plugins list of the decal agent (the one in windows, not in-game)
	// View is the path to the xml file that contains info on how to draw our in-game plugin. The xml contains the name and icon our plugin shows in-game.
	// The view here is SamplePlugin.mainView.xml because our projects default namespace is SamplePlugin, and the file name is mainView.xml.
	// The other key here is that mainView.xml must be included as an embeded resource. If its not, your plugin will not show up in-game.
	[FriendlyName("MelksLuminanceTracker")]
	public class PluginCore : PluginBase
	{   
        internal static Decal.Adapter.Wrappers.PluginHost MyHost;
        private double version = 1.0;
		private double initialCoins = -1;
		private double initialLuminance = -1;
		private double currentCoins;
        private double curPyreals = 0;
        private double curWEnlCoin = 0;
        private double curLegKey = 0;
        private double curMythKey = 0;
		private double currentLuminance = 0;
        private double killLuminance = 0;
        private double otherLuminance = 0;
        private double effectiveCRate = 0;
        private double effectiveLRate = 0;
        private double hours;
        private double luminRate = 0;
        private double coinRate = 0;
        private double coinRateLum = 0;
        private double lumRateCoin = 0;
        private double lumdiff = 0;
        private double coindiff = 0;
        private double luminkillRate = 0;
        private double luminOtherRate = 0;
        private double coinRateKillLum = 0;
        private double coinRateOtherLum = 0;
        private double effectivekillRate = 0;
        private double effectiveOtherRate = 0;
        private double killsperhr = 0;
        private double killsTotal = 0;
        private double xpCur = -1;
        private double xpRate = 0;
        private double xpInitVal = -1;
        private double xpDiff = 0;
        private double xpToLevel = 0;
		private double conversionCRate = 66.2;
        private double conversionLRate = 66;
        private long current_luminance = 0;
        private int currentcoincount;
        private int curAetheria = 0;
        private int curBAetheria = 0;
        private int curTrinket = 0;
        private int coinclapavail = 0;
        private int pollRate = 1;
        private int autotxcoincnt = 50;
        private double autotxlumcnt = 8000000000;
        private int CoinMode = 0;  //0=red, 1=egg, 2=shells, 3=coins, 4=snowmen, 5=Jam, 6=Skulls
        private int curFaltTrinket = 0;
        private double curMMD = 0;
        private int curTimelostCoins = 0;
        private int curSlimyShells = 0;
        private int curPengEgg = 0;
        private int curJams = 0;
        private int curSkulls = 0;
        private int curWEC = 0;
        private TimeSpan elapsed;
        private TimeSpan timetolvl;
        private TimeSpan timeclap;
		private DateTime startTime;
		private System.Timers.Timer pollTimer;
        private System.Timers.Timer updateTimer;
        private System.Timers.Timer clrTimer;
		private bool autoResetEnabled = false;
        private bool coinusebank = false;
        private bool progenable = true;
        private bool buffenable = false;
        private bool eatbank = false;
        private bool eatxp = false;
        private bool bankdata = false;
        private bool isinitialized = false;
        private bool enbDebug = false;
        private bool savefilefnd = false;
        private bool autotxcoin = false;
        private bool autotxlum = false;
        private bool popupvis = false;
        private bool popupvis2 = false;
        private bool popupinit = false;
        private bool popupinit2 = false;
        private string txToName = "";
        private string configPath;
        private string characterKey;
        private string tmplumcurstr = "0";
        private string tmplumstr = "0";
        private string tmpothlumcurstr = "0";
        private string tmpkilllumcurstr = "0";
        private string tmpkilllumratestr = "0";
        private string tmpotherumratestr = "0";
        private string tmpeffectiveLRate = "0";
        private string tmplumRateCoin = "0";
        private string tmpXPRate = "0";
        private string tmpxpdiff = "0";
        private string tmpXPtoLevel = "0";
        private string tmpXPTotal = "0";
        //popup 
        [MVControlReference("XPPopupBtn")] private IButton XPPopupBtn;
        private MyClasses.MetaViewWrappers.IView xpView;
        //popup2
        [MVControlReference("XPPopupBtn2")] private IButton XPPopupBtn2;
        private MyClasses.MetaViewWrappers.IView xpView2;
        //controls
        [MVControlReference("luminCurrentLabel")] private IStaticText luminCurrentLabel = null;
		[MVControlReference("coinCurrentLabel")] private IStaticText coinCurrentLabel = null;
		[MVControlReference("luminRateLabel")] private IStaticText luminRateLabel = null;
		[MVControlReference("coinRateLabel")] private IStaticText coinRateLabel = null;
        [MVControlReference("lumRateCoinLabel")] private IStaticText lumRateCoinLabel = null;
		[MVControlReference("effectiveCRateLabel")] private IStaticText effectiveCRateLabel = null;
        [MVControlReference("effectiveLRateLabel")] private IStaticText effectiveLRateLabel = null;
		[MVControlReference("timeLabel")] private IStaticText timeLabel = null;
		[MVControlReference("ctimeLabel")] private IStaticText ctimeLabel = null;
		[MVControlReference("convRateInput")] private ITextBox convRateInput = null;
        [MVControlReference("convRateLInput")] private ITextBox convRateLInput = null;
        [MVControlReference("pollRateInput")] private ITextBox pollRateInput = null;
        [MVControlReference("txToInput")] private ITextBox txToInput = null;
        [MVControlReference("autoTxInput")] private ITextBox autoTxInput = null;
        [MVControlReference("AutoTxLumBtn")] private ICheckBox AutoTxLumBtn = null;
        [MVControlReference("BuffEnableChk")] private ICheckBox BuffEnableChk = null;
        [MVControlReference("autoTxLumInput")] private ITextBox autoTxLumInput = null;
        [MVControlReference("coinRateLumLabel")] private IStaticText coinRateLumLabel = null;
        [MVControlReference("atcCurLbl")] private IStaticText atcCurLbl = null;
        [MVControlReference("coinBankBtn")] private ICheckBox coinBankBtn = null;
        [MVControlReference("autoResetBtn")] private ICheckBox autoResetBtn = null;
        [MVControlReference("calcEnableBtn")] private ICheckBox calcEnableBtn = null;
        [MVControlReference("AutoTxCoinsBtn")] private ICheckBox AutoTxCoinsBtn = null;
        [MVControlReference("luminKillLabel")] private IStaticText luminKillLabel = null;
        [MVControlReference("luminOtherlLabel")] private IStaticText luminOtherlLabel = null;
        [MVControlReference("KillLabel")] private IStaticText KillLabel = null;
        [MVControlReference("KillHrLabel")] private IStaticText KillHrLabel = null;
        [MVControlReference("coinRateKillLumLabel")] private IStaticText coinRateKillLumLabel = null;
        [MVControlReference("effectiveKillRateLabel")] private IStaticText effectiveKillRateLabel = null;
        [MVControlReference("luminKillRateLabel")] private IStaticText luminKillRateLabel = null;
        [MVControlReference("luminOtherRateLabel")] private IStaticText luminOtherRateLabel = null;
        [MVControlReference("coinRateOtherLumLabel")] private IStaticText coinRateOtherLumLabel = null;
        [MVControlReference("effectiveOtherRateLabel")] private IStaticText effectiveOtherRateLabel = null;
        [MVControlReference("xpRateLabel")] private IStaticText xpRateLabel = null;
        [MVControlReference("xpTotalLabel")] private IStaticText xpTotalLabel = null;
        [MVControlReference("xpEarnedLabel")] private IStaticText xpEarnedLabel = null;
        [MVControlReference("xpToLvlLabel")] private IStaticText xpToLvlLabel = null;
        [MVControlReference("xpTimetoLvlLabel")] private IStaticText xpTimetoLvlLabel = null;
        //Poppup controls
        private IStaticText subtimeLabel;
        private IStaticText subluminRateLabel;
        private IStaticText subcoinRateLabel;
        private IStaticText subKillHrLabel;
        //Popup Controls for view 2
        private IStaticText subtimeLabel2;
        private IStaticText subluminRateLabel2;
        private IStaticText subcoinRateLabel2;
        private IStaticText subXPHrLabel;
        
        //PopoutWindow tempPopoutwindow = new PopoutWindow();

		protected override void Startup()
		{
			try
			{
				// This initializes our static Globals class with references to the key objects your plugin will use, Host and Core.
				// The OOP way would be to pass Host and Core to your objects, but this is easier.
                MyHost = Host;
				Globals.Init("MelksLuminanceTracker", Host, Core);
				//Initialize the view.
				MVWireupHelper.WireupStart(this, Host);                
			}
			catch (Exception ex) {Util.WriteToChat($"Startup Error: {ex}");}
		}

		protected override void Shutdown()
		{
			try
			{
                		//Destroy the view.
                        
               			MVWireupHelper.WireupEnd(this);
			}
			catch (Exception ex) {Util.WriteToChat($"Shutdown Error: {ex}");}
		}

		[BaseEvent("LoginComplete", "CharacterFilter")]
		private void CharacterFilter_LoginComplete(object sender, EventArgs e)
		{
			try
			{
				// Subscribe to events here
				Util.WriteToChat($"Melks Luminance Tracker V {version:n1} Started");
                Util.WriteToChat("to see possible commands type /mlt help");
                characterKey = $"{CoreManager.Current.CharacterFilter.Server}-{CoreManager.Current.CharacterFilter.Name}";
                configPath = Util.FullPath("MLT_Settings.xml");
				startTime = DateTime.Now;
				pollTimer = new System.Timers.Timer();
                updateTimer = new System.Timers.Timer();
                clrTimer = new System.Timers.Timer();
				pollTimer.Elapsed += new System.Timers.ElapsedEventHandler(UpdateUI);
                updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(QuickUpdateUI);
                clrTimer.Elapsed += new System.Timers.ElapsedEventHandler(eatClear);
                pollTimer.Interval = 60000;
                updateTimer.Interval = 5000;
                clrTimer.Interval = 2000;
                pollTimer.AutoReset = true;
                pollTimer.Enabled = true;
                pollTimer.Start();
                updateTimer.Start();                
                initControls();
                LoadSettings();
                updateconversion();
                isinitialized = true;
                XPPopupBtn.Click += (s, es) => showpopup();
                XPPopupBtn2.Click += (s, es) => showpopup2();
                if (pollRate > 1) {updatePolling();}
                if (!progenable) {Util.WriteToChat("Program is currently Disabled");}
                bankPoll(true);
                if (!savefilefnd) {SaveSettings();}
			}
			catch (Exception ex) {Util.WriteToChat($"CharacterFilter_LoginComplete Error: {ex}");}
		}

		[BaseEvent("Logoff", "CharacterFilter")]
		private void CharacterFilter_Logoff(object sender, Decal.Adapter.Wrappers.LogoffEventArgs e)
		{
			try
			{
				// Unsubscribe to events here, but know that this event is not gauranteed to happen. I've never seen it not fire though.
				// This is not the proper place to free up resources, but... its the easy way. It's not proper because of above statement.
				//Globals.Core.WorldFilter.ChangeObject -= new EventHandler<ChangeObjectEventArgs>(WorldFilter_ChangeObject2);
                pollTimer?.Stop();
                clrTimer?.Stop();
                updateTimer?.Stop();
                pollTimer?.Dispose();
                clrTimer?.Dispose();
                updateTimer?.Dispose();
                if (popupinit) {xpView.Dispose();}
                if (popupinit2) {xpView2.Dispose();}
				pollTimer = null;
                clrTimer = null;
                updateTimer = null;                
			}
			catch (Exception ex) {Util.WriteToChat($"CharacterFilter_Logoff Error: {ex}");}
		}

        private void LoadSettings()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(configPath);
                XmlNode charNode = doc.SelectSingleNode($"/MLT_Settings/Character[@key='{characterKey}']");
                Util.WriteToChat($"Loading Settings: {characterKey}");
                if (charNode == null) {Util.WriteToChat("Loading Settings: No Char Node Found");return;}
                savefilefnd = true;
                // Load auto-reset
                XmlNode node = charNode.SelectSingleNode("AutoResetEnabled");
                if (node != null) 
                {
                    autoResetEnabled = bool.Parse(node.InnerText);
                    autoResetBtn.Checked = autoResetEnabled;
                    //autoResetBtn.Text = $"Auto-Reset Txfr: {(autoResetEnabled ? "On" : "Off")}";
                }
                // Load Coin by Bank
                XmlNode node5 = charNode.SelectSingleNode("coinusebank");
                if (node5 != null) 
                {
                    coinusebank = bool.Parse(node5.InnerText);
                    coinBankBtn.Checked = coinusebank;
                    //coinBankBtn.Text = $"Coin by Bank: {(coinusebank ? "On" : "Off")}";
                }
                // Load Program Enable
                XmlNode node6 = charNode.SelectSingleNode("progenable");
                if (node6 != null) 
                {
                    progenable = bool.Parse(node6.InnerText);
                    calcEnableBtn.Checked = progenable;
                    //calcEnableBtn.Text = $"{(progenable ? "Enabled" : "Disabled")}";
                }

                // Load conversion rate
                XmlNode node2 = charNode.SelectSingleNode("ConversionRate");
                if (node2 != null && double.TryParse(node2.InnerText, out double rate))
                {
                    conversionCRate = rate;
                    convRateInput.Text = conversionCRate.ToString();
                }
                XmlNode node3 = charNode.SelectSingleNode("ConversionLRate");
                if (node3 != null && double.TryParse(node3.InnerText, out double rate2))
                {
                    conversionCRate = rate2;
                    convRateLInput.Text = rate2.ToString();
                }
                // Load Poll Rate
                XmlNode node4 = charNode.SelectSingleNode("pollRate");
                if (node4 != null && int.TryParse(node4.InnerText, out int rate3))
                {
                    pollRate = rate3;
                    pollRateInput.Text = rate3.ToString();
                }
                // Load Poll Rate
                XmlNode node7 = charNode.SelectSingleNode("txToName");
                if (node7 != null)
                {
                    txToInput.Text = node7.InnerText;
                    txToName = node7.InnerText;
                }
                // Auto transfer coins
                XmlNode node8 = charNode.SelectSingleNode("autoTxInput");
                if (node8 != null && int.TryParse(node8.InnerText, out int rate4))
                {
                    autoTxInput.Text = node8.InnerText;
                    autotxcoincnt = rate4;
                }
                // Auto transfer Lum
                XmlNode node9 = charNode.SelectSingleNode("autoTxLumInput");
                if (node9 != null && double.TryParse(node9.InnerText, out double rate5))
                {
                    autoTxLumInput.Text = node9.InnerText;
                    autotxlumcnt = rate5;
                }
                // Auto transfer coin enable
                XmlNode node10 = charNode.SelectSingleNode("autotxcoin");
                if (node10 != null) 
                {
                    autotxcoin = bool.Parse(node10.InnerText);
                    AutoTxCoinsBtn.Checked = autotxcoin;
                    //calcEnableBtn.Text = $"{(progenable ? "Enabled" : "Disabled")}";
                }
                // Auto transfer lum enable
                XmlNode node11 = charNode.SelectSingleNode("autotxlum");
                if (node11 != null) 
                {
                    autotxlum = bool.Parse(node11.InnerText);
                    AutoTxLumBtn.Checked = autotxlum;
                    //calcEnableBtn.Text = $"{(progenable ? "Enabled" : "Disabled")}";
                }
                // BuffEnableChk
                XmlNode node12 = charNode.SelectSingleNode("buffenable");
                if (node11 != null) 
                {
                    buffenable = bool.Parse(node12.InnerText);
                    BuffEnableChk.Checked = buffenable;
                }
            }
            catch (Exception ex) {Util.WriteToChat($"LoadSettings Settings Error: {ex}");}
        }

        private void SaveSettings()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlElement root = null;                
                if (File.Exists(configPath))
                {
                    doc.Load(configPath);
                    root = doc.DocumentElement;
                }
                else
                {
                    root = doc.CreateElement("MLT_Settings");
                    doc.AppendChild(root);
                }

                // Find or create character node
                XmlNode charNode = root.SelectSingleNode($"Character[@key='{characterKey}']");
                if (charNode == null)
                {
                    charNode = doc.CreateElement("Character");
                    ((XmlElement)charNode).SetAttribute("key", characterKey);
                    root.AppendChild(charNode);
                }

                // Auto-reset
                XmlElement autoResetElem = doc.CreateElement("AutoResetEnabled");
                autoResetElem.InnerText = autoResetEnabled.ToString();
                ReplaceOrAppend(charNode, autoResetElem);

                // Coin by Bank
                XmlElement coinusebankElem = doc.CreateElement("coinusebank");
                coinusebankElem.InnerText = coinusebank.ToString();
                ReplaceOrAppend(charNode, coinusebankElem);

                // Program Enable 
                XmlElement progenableElem = doc.CreateElement("progenable");
                progenableElem.InnerText = progenable.ToString();
                ReplaceOrAppend(charNode, progenableElem);

                // Conversion rate
                XmlElement conversionElem = doc.CreateElement("ConversionRate");
                double tmpconv = conversionCRate / 1000000;
                conversionElem.InnerText = tmpconv.ToString();
                ReplaceOrAppend(charNode, conversionElem);
    
                // Luminance Conversion rate
                XmlElement conversionLElem = doc.CreateElement("ConversionLRate");
                double tmpconvl = conversionLRate / 1000000;
                conversionLElem.InnerText = tmpconvl.ToString();
                ReplaceOrAppend(charNode, conversionLElem);
                
                // Poll rate
                XmlElement pollRateElem = doc.CreateElement("pollRate");
                pollRateElem.InnerText = pollRate.ToString();
                ReplaceOrAppend(charNode, pollRateElem);
    
                // Transfer Name
                XmlElement txToNameElem = doc.CreateElement("txToName");
                txToNameElem.InnerText = txToInput.Text;
                ReplaceOrAppend(charNode, txToNameElem);

                // Auto transfer Coins Count
                XmlElement autoTxInputElem = doc.CreateElement("autoTxInput");
                autoTxInputElem.InnerText = autoTxInput.Text;
                ReplaceOrAppend(charNode, autoTxInputElem);

                // Auto transfer Lum count
                XmlElement autoTxLumInputElem = doc.CreateElement("autoTxLumInput");
                autoTxLumInputElem.InnerText = autoTxLumInput.Text;
                ReplaceOrAppend(charNode, autoTxLumInputElem);

                // Auto Transfer Coins Enable 
                XmlElement autotxcoinElem = doc.CreateElement("autotxcoin");
                autotxcoinElem.InnerText = autotxcoin.ToString();
                ReplaceOrAppend(charNode, autotxcoinElem);

                // Auto Transfer Lum Enable 
                XmlElement autotxlumElem = doc.CreateElement("autotxlum");
                autotxlumElem.InnerText = autotxlum.ToString();
                ReplaceOrAppend(charNode, autotxlumElem);

                // Buff Enable 
                XmlElement autobuffElem = doc.CreateElement("buffenable");
                autobuffElem.InnerText = buffenable.ToString();
                ReplaceOrAppend(charNode, autobuffElem);

                doc.Save(configPath);
            }
            catch (Exception ex) {Util.WriteToChat($"SaveSettings Error: {ex}");}
        }

        private void ReplaceOrAppend(XmlNode parent, XmlElement newElement)
        {
            XmlNode old = parent.SelectSingleNode(newElement.Name);
            if (old != null)
                parent.ReplaceChild(newElement, old);
            else
                parent.AppendChild(newElement);
        }
        
        private void UpdateUI(Object source, System.Timers.ElapsedEventArgs e)
		{
			try
			{
                if (!progenable){return;}
                bankPoll(true);
                if (enbDebug){Util.WriteToChat($"UpdateUI After progenable: {progenable}");}
			}
			catch (Exception ex) {Util.WriteToChat($"UpdateUI Error: {ex}");}
		}

        private void bankPoll(bool eb) 
        {
            if (enbDebug){Util.WriteToChat($"Bank Poll Entry. EB = {eb}");}
            if (!isinitialized) {return;}
            eatbank = eb;            
            clrTimer.Start();
            eatxp = true;
            Util.Command("/xp");  
            Util.Command("/b");
        }

        private void eatClear(object sender, EventArgs e)
        {
            try
			{
                eatbank = false;
                Util.WriteToChat("Eat Bank Data Timer Met!");
                clrTimer?.Stop();
            }
            catch (Exception ex) {Util.WriteToChat($"doCalcs Error: {ex}");}
        }

        private void QuickUpdateUI(object sender, EventArgs e)
        {
            try
			{
                if (enbDebug){Util.WriteToChat("QuickUpdateUI Entry.");}
                if (initialCoins == -1 || initialLuminance == -1) {return;}
                doCalcs();
                updateGUI();
                CheckInputs();
            }
            catch (Exception ex) {Util.WriteToChat($"QuickUpdateUI Error: {ex}");}
        }

        private void updateGUI()
        {   
            try
			{
                if (enbDebug){Util.WriteToChat("updateGUI Entry.");}
                //Main Tab
                luminCurrentLabel.Text = $"[Bank] Luminance: {tmplumcurstr}";
                coinCurrentLabel.Text = $"[Bank] Coins: {currentCoins}";
                int thours = (int)(Math.Floor(elapsed.TotalHours));
                timeLabel.Text = $"Run Time: {thours}:{elapsed.Minutes:D2}";
                string ctime = DateTime.Now.ToString("HH:mm");
                ctimeLabel.Text = $"Time: {ctime}";
				luminRateLabel.Text = $"Lum/hr: {tmplumstr}";
				coinRateLabel.Text = $"Coins/hr: {coinRate}";
                coinRateLumLabel.Text = $"Lum-Coins/hr: {coinRateLum}";
                lumRateCoinLabel.Text = $"Coins-Lum/hr: {tmplumRateCoin}";
				effectiveCRateLabel.Text = $"Effective C/hr: {effectiveCRate}";
				effectiveLRateLabel.Text = $"Effective L/hr: {tmpeffectiveLRate}";
                if (CoinMode == 0) {atcCurLbl.Text = $"A: {curAetheria} T: {curTrinket} C: {currentcoincount} CAvail: {coinclapavail}";} //0=red, 1=egg, 2=shells, 3=coins, 4=snowmen, 5=Jam
                else if (CoinMode == 1) {atcCurLbl.Text = $"Eggs: {curPengEgg} C: {currentcoincount} MMD: {curMMD:n0}";} 
                else if (CoinMode == 2) {atcCurLbl.Text = $"Shells: {curSlimyShells}  MMD: {curMMD:n0} MK: {curMythKey}";}
                else if (CoinMode == 3) {atcCurLbl.Text = $"Anc Pyreal: {curTimelostCoins}  C: {currentcoincount}";}
                else if (CoinMode == 4) {atcCurLbl.Text = $"BA: {curBAetheria} AFT: {curFaltTrinket} WC: {curWEC} MMD: {curMMD:n0} CAvail: {coinclapavail}";}
                else if (CoinMode == 5) {atcCurLbl.Text = $"Jams: {curJams} C: {currentcoincount}";}
                else if (CoinMode == 6) {atcCurLbl.Text = $"Skulls: {curSkulls} WEC: {currentcoincount:n0} MMD: {curMMD:n0}";}
                //Spec Tab
                KillLabel.Text = $"Kills: {killsTotal}";
                KillHrLabel.Text = $"Kills/hr: {killsperhr}";
                luminKillLabel.Text = $"Kill Lum: {tmpkilllumcurstr}";
                luminOtherlLabel.Text = $"Other Lum: {tmpothlumcurstr}";
                luminKillRateLabel.Text = $"Lum/hr K: {tmpkilllumratestr}";
                coinRateKillLumLabel.Text = $"Lum-Coins/hr K: {coinRateKillLum}";
                effectiveKillRateLabel.Text = $"Effective K: {effectivekillRate}";
                luminOtherRateLabel.Text = $"Lum/hr O: {tmpotherumratestr}";
                coinRateOtherLumLabel.Text = $"Lum-Coins/hr O: {coinRateOtherLum}";
                effectiveOtherRateLabel.Text = $"Effective O: {effectiveOtherRate}";
                //XP Tab
                xpTotalLabel.Text = $"Total XP: {tmpXPTotal}";
                xpEarnedLabel.Text = $"Earned XP: {tmpxpdiff}";
                xpRateLabel.Text = $"XP/hr: {tmpXPRate}";
                xpToLvlLabel.Text = $"XP to Level: {tmpXPtoLevel}";
                xpTimetoLvlLabel.Text = $"Time to Lvl: {timetolvl.TotalHours:n0}:{timetolvl.Minutes:D2}";

                //Pop Up View
                if (popupvis)
                {
                    subtimeLabel.Text = $"Run Time: {elapsed.TotalHours:n0}:{elapsed.Minutes:D2}";
                    subluminRateLabel.Text = $"Lum/hr: {tmplumstr}";
                    subcoinRateLabel.Text = $"Coins/hr: {coinRate}";
                    subKillHrLabel.Text = $"Kills/hr: {killsperhr}";
                }
                if (popupvis2)
                {
                    subtimeLabel2.Text = $"Run Time: {elapsed.TotalHours:n0}:{elapsed.Minutes:D2}";
                    subluminRateLabel2.Text = $"Lum/hr: {tmplumstr}";
                    subcoinRateLabel2.Text = $"Coins/hr: {coinRate}";
                    subXPHrLabel.Text = $"XP/hr: {tmpXPRate}";
                }
            }
            catch (Exception ex) {Util.WriteToChat($"QuickUpdateUI Error: {ex}");}
        }

        private void CheckInputs()
        {
            bool needsave = false;
            if(calcEnableBtn.Checked != progenable)
            {
                progenable = calcEnableBtn.Checked;
                Util.WriteToChat($"Program Enable - {progenable}");
                needsave = true;
            }
            if(AutoTxCoinsBtn.Checked != autotxcoin)
            {
                autotxcoin = AutoTxCoinsBtn.Checked;
                Util.WriteToChat($"Auto Transfer Coins - {autotxcoin}");
                needsave = true;
            }
            if(AutoTxLumBtn.Checked != autotxlum)
            {
                autotxlum = AutoTxLumBtn.Checked;
                Util.WriteToChat($"Auto Transfer Lum - {autotxlum}");
                needsave = true;
            }
            if(autoResetBtn.Checked != autoResetEnabled)
            {
                autoResetEnabled = autoResetBtn.Checked;
                Util.WriteToChat($"Auto Reset - {autoResetEnabled}");
                needsave = true;
            }
            if(coinBankBtn.Checked != coinusebank)
            {
                coinusebank = coinBankBtn.Checked;
                Util.WriteToChat($"Use Bank for Coin Count - {coinusebank}");
                needsave = true;
            }
            if(txToInput.Text != txToName)
            {
                txToName = txToInput.Text;
                Util.WriteToChat($"Transfer to updated to - {txToName}");
                needsave = true;
            }
            /*if(((int)(autoTxInput.Text) > autotxcoin) || ((int)(autoTxInput.Text) < autotxcoin))
            {
                autotxcoin = autoTxInput.Text;
                Util.WriteToChat($"Auto coin transfer amount updated to - {autotxcoin}");
                needsave = true;
            }
            if(((int)(autoTxLumInput.Text) != autotxlum) || ((int)(autoTxLumInput.Text) != autotxlum))
            {
                autotxlum = autoTxLumInput.Text;
                Util.WriteToChat($"Auto lum transfer amount updated to - {autotxlum}");
                needsave = true;
            }*/
            if(BuffEnableChk.Checked != buffenable)
            {
                buffenable = BuffEnableChk.Checked;
                Util.WriteToChat($"Enable Buffs - {buffenable}");
                needsave = true;
            }
            if(needsave){SaveSettings();}
            needsave = false;
        }

        private string StrValUpdate(double e)
        {
            try
            {
                double tmpval = e;
                if (tmpval < 0){tmpval = 0;}
                string tmpvalstr = $"{tmpval}";
                
                if ((tmpval >= 1000000) && (tmpval < 1000000000)) 
                {
                    tmpval = tmpval / 1000000;
                    tmpval = Math.Round(tmpval, 3);
                    tmpvalstr = $"{tmpval} Mil";
                }
                else if ((tmpval >= 1000000000) && (tmpval < 1000000000000))
                {
                    tmpval = tmpval / 1000000000;
                    tmpval = Math.Round(tmpval, 3); 
                    tmpvalstr = $"{tmpval} Bil";
                }
                else if ((tmpval >= 1000000000000) && (tmpval < 1000000000000000))
                {
                    tmpval = tmpval / 1000000000000;
                    tmpval = Math.Round(tmpval, 3); 
                    tmpvalstr = $"{tmpval} Tril";
                }
                else if ((tmpval >= 1000000000000000) && (tmpval < 1000000000000000000))
                {
                    tmpval = tmpval / 1000000000000;
                    tmpval = Math.Round(tmpval, 3); 
                    tmpvalstr = $"{tmpval} Quad";
                }
                else if (tmpval >= 1000000000000000000)
                {
                    tmpval = tmpval / 1000000000000000000;
                    tmpval = Math.Round(tmpval, 3); 
                    tmpvalstr = $"{tmpval} Quint";
                }                
                return tmpvalstr;
			}
			catch (Exception ex) {Util.WriteToChat($"StrValUpdate Error: {ex}");}
            return "0";
		}
               
        private void initControls()
		{
		    try
		    {
			    //Main Tab
				luminRateLabel.Text = "Lum/hr: 0";
				coinRateLabel.Text = "Coins/hr: 0";
                coinRateLumLabel.Text = "Lum-Coins/hr: 0";
                lumRateCoinLabel.Text = "Coins-Lum/hr: 0";
				effectiveCRateLabel.Text = "Effective C/hr: 0";
				effectiveLRateLabel.Text = "Effective L/hr: 0";
				timeLabel.Text = "Run Time: 00:00";          
                //Spec Tab
                KillLabel.Text = "Kills: 0";
                KillHrLabel.Text = "Kills/hr: 0";
                luminKillLabel.Text = "Kill Lum: 0";
                luminOtherlLabel.Text = "Other Lum: 0";
                luminKillRateLabel.Text = "Lum/hr K: 0";
                luminOtherRateLabel.Text = "Lum/hr O: 0";
                coinRateKillLumLabel.Text = "Lum-Coins/hr K: 0";
                coinRateOtherLumLabel.Text = "Lum-Coins/hr O: 0";
                effectiveKillRateLabel.Text = "Effective K: 0";
                effectiveOtherRateLabel.Text = "Effective O: 0";
                atcCurLbl.Text = "A: 0 T: 0 C: 0 CAvail: 0";
                //XP Tab
                xpTotalLabel.Text = "Total XP: 0";
                xpEarnedLabel.Text = "Earned XP: 0";
                xpRateLabel.Text = "XP/hr: 0";
                xpToLvlLabel.Text = "XP to Level: 0";
                xpTimetoLvlLabel.Text = "Time to Lvl: 00:00";
		    }
		    catch (Exception ex) {Util.WriteToChat($"initControls Error: {ex}");}
		}

        private void updateconversion()
        {
            try
			{
                if (convRateInput.Text == null) {conversionCRate = 66.2;}
				else{
				    double.TryParse(convRateInput.Text, out conversionCRate);
		            if (double.IsNaN(conversionCRate) || double.IsInfinity(conversionCRate)) {conversionCRate = 66.2;}
                }
                if (convRateLInput.Text == null) {conversionLRate = 66;}
				else{
				    double.TryParse(convRateLInput.Text, out conversionLRate);
		            if (double.IsNaN(conversionLRate) || double.IsInfinity(conversionLRate)) {conversionLRate = 66;}
                }
                conversionCRate = conversionCRate * 1000000;
                conversionLRate = conversionLRate * 1000000;
                Util.WriteToChat($"Conversion Rate set to 1 Enl Coin per {conversionCRate:n0} Luminance");
                Util.WriteToChat($"Conversion Rate set to {conversionLRate:n0} Luminance per coin");
            }
            catch (Exception ex) {Util.WriteToChat($"updateconversion Error: {ex}");}
		}

        private void totalReset()
        {
            try
            {
                if (enbDebug){Util.WriteToChat("totalReset Entry.");}
                initialCoins = -1;
				initialLuminance = -1;
                xpInitVal = -1;
                currentcoincount = 0;
                coinclapavail = 0;
                curAetheria = 0;                
                curTrinket = 0;
                killLuminance = 0;
                lumdiff = 0;
                coindiff = 0;
                luminkillRate = 0;
                luminOtherRate = 0;
                coinRateKillLum = 0;
                coinRateOtherLum = 0;
                effectivekillRate = 0;
                killsperhr = 0;
                killsTotal = 0;
                xpCur = 0;
                xpRate = 0;
                xpDiff = 0;
                curPyreals = 0;
                curWEnlCoin = 0;
                curLegKey = 0;
                curMythKey = 0;
                curBAetheria = 0;
                curFaltTrinket = 0;
                curTimelostCoins = 0;
                curSlimyShells = 0;
                curPengEgg = 0;
                curSkulls = 0;
                curJams = 0;
                curMMD = 0;
                curWEC = 0;
                
				startTime = DateTime.Now;
                //Main Tab
				luminRateLabel.Text = "Lum/hr: 0";
				coinRateLabel.Text = "Coins/hr: 0";
                coinRateLumLabel.Text = "Lum-Coins/hr: 0";
                lumRateCoinLabel.Text = "Coins-Lum/hr: 0";
				effectiveCRateLabel.Text = "Effective C/hr: 0";
				effectiveLRateLabel.Text = "Effective L/hr: 0";
				timeLabel.Text = "Run Time: 00:00";          
                //Spec Tab
                KillLabel.Text = "Kills: 0";
                KillHrLabel.Text = "Kills/hr: 0";
                luminKillLabel.Text = "Kill Lum: 0";
                luminOtherlLabel.Text = "Other Lum: 0";
                luminKillRateLabel.Text = "Lum/hr K: 0";
                luminOtherRateLabel.Text = "Lum/hr O: 0";
                coinRateKillLumLabel.Text = "Lum-Coins/hr K: 0";
                coinRateOtherLumLabel.Text = "Lum-Coins/hr O: 0";
                effectiveKillRateLabel.Text = "Effective K: 0";
                effectiveOtherRateLabel.Text = "Effective O: 0";
                if (CoinMode == 0) {atcCurLbl.Text = "A: 0 T: 0 C: 0 CAvail: 0";} //0=red, 1=egg, 2=shells, 3=coins, 4=snowmen, 5=Jam, 6=Skulls
                else if (CoinMode == 1) {atcCurLbl.Text = "Eggs: 0 C: 0 MMD: 0";} 
                else if (CoinMode == 2) {atcCurLbl.Text = "Shells: 0  MMD: 0 MK: 0";}
                else if (CoinMode == 3) {atcCurLbl.Text = "Anc Pyrl: 0  C: 0";}
                else if (CoinMode == 4) {atcCurLbl.Text = "BA: 0 AFT: 0 WC: 0 CAvail: 0";}
                else if (CoinMode == 5) {atcCurLbl.Text = "Jams: 0 C: 0";}
                else if (CoinMode == 6) {atcCurLbl.Text = "Skulls: 0 WEC: 0 MMD: 0";}
                //XP Tab
                xpTotalLabel.Text = "Total XP: 0";
                xpEarnedLabel.Text = "Earned XP: 0";
                xpRateLabel.Text = "XP/hr: 0";
                xpToLvlLabel.Text = "XP to Level: 0";
                xpTimetoLvlLabel.Text = "Time to Lvl: 00:00";
                if (!progenable){ return;}
                bankPoll(true);
            }
            catch (Exception ex) {Util.WriteToChat($"totalReset Error: {ex}");}
        }

        private void doCalcs()
        {
            try
			{
                if (!isinitialized) {return;}
                if (enbDebug){Util.WriteToChat("doCalcs Entry.");}
                elapsed = DateTime.Now - startTime;
                hours = elapsed.TotalHours;
                //Coin gen
                if (CoinMode == 0)//0=red, 1=egg, 2=shells, 3=coins, 4=snowmen, 5=Jam)
                {
                    if ((curAetheria >= 1) && (!coinusebank)){  
                        if (curTrinket >= 1){
                            currentcoincount += 1;
                            curAetheria -= 1;
                            curTrinket -= 1;
                        }
                    }
                }
                else if (CoinMode == 1) //curPengEgg 100 eggs = 10 coins 100 mmd
                {
                    if (curPengEgg >= 1){
                        currentcoincount = (int)(curPengEgg * .1);
                        curMMD = (int)(curPengEgg * 1);
                    }
                }
                else if (CoinMode == 2) //curSlimyShells  100 shells = 350 MMD and 5 mk
                {
                    if (curSlimyShells >= 1){
                        curMMD = (int)(curSlimyShells * 3.5);
                        curMythKey = (int)(curSlimyShells * .05);
                    }
                }
                else if (CoinMode == 3) //curTimelostCoins  100 tlc = 37 coins
                {
                    if (curTimelostCoins >= 1){
                        currentcoincount = (int)(curTimelostCoins * .37);
                    }
                }
                else if (CoinMode == 4) // curBAetheria, curFaltTrinket
                {
                    if (curBAetheria >= 1){
                        if (curFaltTrinket >= 1){
                            curWEC += 3;
                            curBAetheria -= 1;
                            curFaltTrinket -= 1;
                            currentcoincount += 3;
                        }
                    }
                }
                else if (CoinMode == 5) //curJams - 100 Jam = 10 Coins
                {
                    if (curJams >= 1){
                        currentcoincount = (int)(curJams * .1);
                    }
                }
                else if (CoinMode == 6)  //curSkulls 100 Skulls = 200 WEC 80 MMD
                {
                    if (curSkulls >= 1){
                        curMMD = (int)(curSkulls * .8);
                        currentcoincount = (int)(curSkulls * 2);
                    }
                }
                killsperhr = hours > 0 ? Math.Round(killsTotal / hours) : 0;
                //XP Calculations
                if (xpInitVal == -1)
                {
                    xpInitVal = CoreManager.Current.CharacterFilter.TotalXP; //CoreManager.Current.CharacterFilter.UnassignedXP 
                    xpCur = xpInitVal;
                    xpRate = 0;
                }
                xpCur = CoreManager.Current.CharacterFilter.TotalXP;
                xpDiff = xpCur - xpInitVal;
                xpRate = hours > 0 ? Math.Round(xpDiff / hours, 1) : 0;
                if ((xpToLevel > 0) && (xpRate > 0))
                {
                    double xplvlrate = xpToLevel / xpRate;
                    double tmphrs;
                    double tmpmin;
                    if (enbDebug){Util.WriteToChat($"XP To Level Rate = {xplvlrate}.");}
                    if (xplvlrate > 0) {
                        tmphrs = xplvlrate - (xplvlrate % 1);
                        tmpmin = ((xplvlrate % 1) * 100) * .6;
                        
                    } else {
                        tmphrs = 0;
                        tmpmin = 0;
                    }
                    if (tmphrs > 9999999) {tmphrs = 0; tmpmin=0;}
                    else if (tmpmin > 9999999) {tmpmin=0;}
                    TimeSpan tmptimehr;
                    TimeSpan tmptimemin;
                    tmptimehr = TimeSpan.FromHours(tmphrs);
                    tmptimemin = TimeSpan.FromMinutes(tmpmin);
                    timetolvl = tmptimehr + tmptimemin;
                }
                if (enbDebug){Util.WriteToChat($"doCalcs progenable = {progenable}.");}
                if (!progenable){ return;}
                // Luminance per hour
                lumdiff = currentLuminance - initialLuminance;    
                luminRate = hours > 0 ? Math.Round(lumdiff / hours, 1) : 0;
                // Coins per hour
                if (coinusebank)
                {
                    coindiff = currentCoins - initialCoins;
                }
                else
                {
                    coindiff = currentcoincount;
                    if (CoinMode == 1) //Eggs
                    {
                        coindiff = curPengEgg * .1;
                    }
                    if (CoinMode == 3) //TL Coins
                    {
                        coindiff = curTimelostCoins * .37;
                    }
                    if (CoinMode == 5) //Jams
                    {
                        coindiff = curJams * .1;
                    }
                }
	            coinRate = hours > 0 ? Math.Round(coindiff / hours, 1) : 0;
                effectiveCRate = coinRate + coinRateLum;
                // Lum per hour from Coins
                coinRateLum = hours > 0 ? Math.Round((luminRate / conversionLRate)) : 0;
                lumRateCoin =  hours > 0 ? Math.Round((coinRate * conversionCRate), 1) : 0;
                effectiveLRate = luminRate + lumRateCoin;
                // Other-kill Luminance
                otherLuminance = lumdiff - killLuminance;
                if (otherLuminance < 0){otherLuminance=0;}
                
                // Luminance hr rates for kill/other
                luminkillRate = hours > 0 ? Math.Round((lumdiff - otherLuminance) / hours, 1) : 0;
                luminOtherRate = hours > 0 ? Math.Round((lumdiff - killLuminance) / hours, 1) : 0;
                coinRateKillLum = hours > 0 ? Math.Round((luminkillRate / conversionCRate)) : 0;
                coinRateOtherLum = hours > 0 ? Math.Round((luminOtherRate / conversionCRate)) : 0;
                effectivekillRate = coinRate + coinRateKillLum;
                effectiveOtherRate = coinRate + coinRateOtherLum;
                // Coins clappable
                if (curPyreals < 250000) {coinclapavail = 0;}
                else{ 
                    coinclapavail = (int)curPyreals / 250000;
                    if (coinRate >= 1){                        
                        double clapRate = curPyreals / (coinRate * 250000);
                        double tmpclaphrs;
                        double tmpclapmin;
                        if (clapRate >= 1) {
                            tmpclaphrs = clapRate % 1;
                            tmpclapmin = (clapRate - (clapRate % 1)) * 60;
                        } else {
                            tmpclaphrs = 0;
                            tmpclapmin = clapRate * 60;
                        }                    
                        if (tmpclaphrs > 999) {tmpclaphrs = 999;}
                            TimeSpan tmpclaptimehr;
                            TimeSpan tmpclaptimemin;
                            tmpclaptimehr = TimeSpan.FromHours(tmpclaphrs);
                            tmpclaptimemin = TimeSpan.FromMinutes(tmpclapmin);
                            timeclap = tmpclaptimehr + tmpclaptimemin;
                    }
                }
                //Coin Transfer
                int.TryParse(autoTxInput.Text, out int autotxcoincnt);
                if (autotxcoin)
                {                    
                    if (currentCoins >= autotxcoincnt)
                    {
                        txStuff("Coins");
                    }
                }
                //Lum Transfer                
                double.TryParse(autoTxLumInput.Text, out double autotxlumcnt);
                if (autotxlum)
                {
                    if (currentLuminance >= autotxlumcnt)
                    {
                        txStuff("Lum");
                    }
                }
                // 0 if negative
                if (effectiveOtherRate < 0){effectiveOtherRate=0;}
                if (luminOtherRate < 0){luminOtherRate=0;}
                if (luminkillRate < 0){luminkillRate=0;}
                if (coinRateKillLum < 0){coinRateKillLum=0;}
                if (coinRateOtherLum < 0){coinRateOtherLum=0;}
                //String conversion for large values
                tmplumcurstr = StrValUpdate(currentLuminance);
                tmplumstr = StrValUpdate(luminRate);
                tmpothlumcurstr = StrValUpdate(otherLuminance);
                tmpkilllumcurstr = StrValUpdate(killLuminance);
                tmpkilllumratestr = StrValUpdate(luminkillRate);                
                tmpotherumratestr = StrValUpdate(luminOtherRate);
                tmpeffectiveLRate = StrValUpdate(effectiveLRate);
                tmplumRateCoin = StrValUpdate(lumRateCoin);
                tmpXPRate = StrValUpdate(xpRate);
                tmpXPtoLevel = StrValUpdate(xpToLevel);
                tmpXPTotal = StrValUpdate(xpCur);
                tmpxpdiff = StrValUpdate(xpDiff);
            }
            catch (Exception ex) {Util.WriteToChat($"doCalcs Error: {ex}");}
        }
        
        [MVControlEvent("resetBtn", "Click")]
		void reset_Btn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
				totalReset();
			}
			catch (Exception ex) {Util.WriteToChat($"reset_Btn_Click Error: {ex}");}
		}

        [MVControlEvent("qbankButton", "Click")]
		void qbankButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                bankPoll(true);
                updateGUI();
            }
			catch (Exception ex) {Util.WriteToChat($"bankButton_Click Error: {ex}");}
		}
            
        [MVControlEvent("bankButton", "Click")]
		void bankButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                bankPoll(false);
                updateGUI();
            }
			catch (Exception ex) {Util.WriteToChat($"bankButton_Click Error: {ex}");}
		}


        [MVControlEvent("clapButton", "Click")]
		void clapButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                Util.Command("/clap all");
            }
			catch (Exception ex) {Util.WriteToChat($"clapButton_Click Error: {ex}");}
		}

        [MVControlEvent("UpdatePoll", "Click")]
		void UpdatePoll_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                updatePolling();
                SaveSettings();
            }
			catch (Exception ex) {Util.WriteToChat($"UpdatePoll Error: {ex}");}
		}

        private void updatePolling()
        {
            try
            {
                if (pollRateInput.Text == null) {pollRate = 1;}
                else{
                    int.TryParse(pollRateInput.Text, out pollRate);
                }
                if (pollRate < 1){pollRate = 1;}
                pollTimer.Interval = pollRate * 60000;
                Util.WriteToChat("Updated Poll Rate Minutes: " + pollRate);
                
            }
			catch (Exception ex) {Util.WriteToChat($"updatePolling Error: {ex}");}
		}
        
        [MVControlEvent("applyConversionButton", "Click")]
		void applyConversionButton_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                Util.WriteToChat("Updating Conversion Factor");
                updateconversion();
                SaveSettings();
            }
			catch (Exception ex) {Util.WriteToChat($"applyConversionButton Error: {ex}");}
		}

        [MVControlEvent("reportLumBtn", "Click")]
		void reportLumBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("Lum");
			}
			catch (Exception ex) {Util.WriteToChat($"reportLumBtn_Click Error: {ex}");}
		}

        [MVControlEvent("reportLumEBtn", "Click")]
		void reportLumEBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("LumE");                    
			}
			catch (Exception ex) {Util.WriteToChat($"reportLumEBtn_Click Error: {ex}");}
		}

        [MVControlEvent("reportCoinBtn", "Click")]
		void reportCoinBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("Coin");                    
			}
			catch (Exception ex) {Util.WriteToChat($"reportCoinBtn_Click Error: {ex}");}
		}

        [MVControlEvent("reportCoinEBtn", "Click")]
		void reportCoinEBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("CoinE");                    
			}
			catch (Exception ex) {Util.WriteToChat($"reportCoinEBtn_Click Error: {ex}");}
		}
        
        [MVControlEvent("reportKillBtn", "Click")]
		void reportKillBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("Kill");                    
			}
			catch (Exception ex) {Util.WriteToChat($"reportKillBtn_Click Error: {ex}");}
		}

        [MVControlEvent("reportXPBtn", "Click")]
		void reportXPBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                reportsOutput("XP");
			}
			catch (Exception ex) {Util.WriteToChat($"reportKillBtn_Click Error: {ex}");}
		}

        [MVControlEvent("txCoinsBtn", "Click")]
		void txCoinsBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                txStuff("Coins");
                SaveSettings();
			}
			catch (Exception ex) {Util.WriteToChat($"txCoinsBtn_Click Error: {ex}");}
		}
    
        [MVControlEvent("txLumBtn", "Click")]
		void txLumBtn_Click(object sender, MVControlEventArgs e)
		{
			try
			{
                if (!isinitialized) {return;}
                txStuff("Lum");
                SaveSettings();
			}
			catch (Exception ex) {Util.WriteToChat($"txLumBtn_Click Error: {ex}");}
		}

        private void showpopup()
        {
			try
			{
                if (!popupinit){
                    xpView = MyClasses.MetaViewWrappers.ViewSystemSelector.CreateViewResource(MyHost, "MelksLuminanceTracker.subView.xml");
                    xpView.Activate();
                    subtimeLabel = (IStaticText)xpView["subtimeLabel"];
                    subluminRateLabel = (IStaticText)xpView["subluminRateLabel"];
                    subcoinRateLabel = (IStaticText)xpView["subcoinRateLabel"];
                    subKillHrLabel = (IStaticText)xpView["subKillHrLabel"];
                    xpView.Visible = true;
                    popupinit = true;
                    popupvis = true;}
                else{                    
                    xpView.Visible = false;
                    popupinit = false; 
                    xpView.Dispose();
                }
			}
			catch (Exception ex) {Util.WriteToChat($"showpopup Error: {ex}");}
		}

        private void showpopup2()
        {
			try
			{
                if (!popupinit2){
                    xpView2 = MyClasses.MetaViewWrappers.ViewSystemSelector.CreateViewResource(MyHost, "MelksLuminanceTracker.subView2.xml");
                    xpView2.Activate();
                    subtimeLabel2 = (IStaticText)xpView2["subtimeLabel2"];
                    subluminRateLabel2 = (IStaticText)xpView2["subluminRateLabel2"];
                    subcoinRateLabel2 = (IStaticText)xpView2["subcoinRateLabel2"];
                    subXPHrLabel = (IStaticText)xpView2["subXPHrLabel"];
                    xpView2.Visible = true;
                    popupinit2 = true;
                    popupvis2 = true;}
                else{
                    xpView2.Visible = false;
                    popupinit2 = false;
                    xpView2.Dispose();
                }
			}
			catch (Exception ex) {Util.WriteToChat($"showpopup Error: {ex}");}
		}
        
        private void txStuff(string totx)
        {
            try
			{
                if (txToInput.Text == ""){Util.WriteToChat("No Name Selected for Transfer"); return;}
                txToName = txToInput.Text;
                if (totx == "Coins")
                {
                    int tcointx = (int)currentCoins - 1;
                    if (enbDebug){Util.WriteToChat($"/b t e {tcointx} \"{txToName}\"");}
                    if (tcointx <= 1) {Util.WriteToChat("Not Enough Coins to Transfer"); return;}
                    Util.WriteToChat($"Transfering {tcointx} Coins to: {txToName}");
                    string tccmnd = $"/b t e {tcointx} \"{txToName}\"";
                    Util.Command(tccmnd);
                    currentCoins = 1;
                    if (coinusebank){totalReset();}
                    else if (autoResetEnabled){totalReset();}
                }
                else if (totx == "Lum")
                {
                    double tlumtx = currentLuminance - 1;
                    if (enbDebug){Util.WriteToChat($"/b t l {tlumtx:f0} \"{txToName}\"");}
                    if (tlumtx <= 1) {Util.WriteToChat("Not Enough Luminance to Transfer"); return;}
                    Util.WriteToChat($"Transfering {tlumtx:n0} Luminance to: {txToName}");
					string tlcmnd = $"/b t l {tlumtx:f0} \"{txToName}\"";
                    Util.Command(tlcmnd);
                    currentLuminance = 1;
                    if (autoResetEnabled){totalReset();}
                }
            }
			catch (Exception ex) {Util.WriteToChat($"txStuff Error: {ex}");}
        }

        private void reportsOutput(string rpt)  // Skippy's : You've gained 5,245,917 luminance in 15 seconds for 1,251,167,121 luminance per hour.
        {
            string tmplumdiff;
            string tmppyreal;
            if (rpt == "Lum")
            {
                tmplumdiff = StrValUpdate(lumdiff);
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%h} Hours {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
                else{
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
            }
            if (rpt == "LumE") 
            {
                tmplumdiff = StrValUpdate(lumdiff);
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%h} Hours {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
                else{
                    Util.WriteToChat($"You gained {tmplumdiff} Luminance in {elapsed:%m} Minutes for {tmplumstr} Luminance per hour.");}
                Util.WriteToChat($"You are earning {tmplumRateCoin} Luminance from Coins per hour @ {conversionLRate:n0} per coin");
                Util.WriteToChat($"Your effective Luminance per hour is : {tmpeffectiveLRate}");
            }
            if (rpt == "Coin") 
            {
                tmppyreal = StrValUpdate(curPyreals);
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%h} Hours {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                else {
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                if (timeclap.Days > 0){
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%d} Days {timeclap:%h} Hours {timeclap:%m} Minutes");}
                else if (timeclap.Hours > 0){
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%h} Hours {timeclap:%m} Minutes");}
                else{
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%m} Minutes");}

            }
            if (rpt == "CoinE") 
            {
                tmppyreal = StrValUpdate(curPyreals);
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%h} Hours {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                else {
                    Util.WriteToChat($"You gained {coindiff:n0} Coins in {elapsed:%m} Minutes for {coinRate:n} Coins per hour.");}
                Util.WriteToChat($"You are earning {coinRateLum:n} Coins from Luminancer per hour @ {conversionCRate:n0} per coin");
                if (timeclap.Days > 0){
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%d} Days {timeclap:%h} Hours {timeclap:%m} Minutes");}
                else if (timeclap.Hours > 0){
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%h} Hours {timeclap:%m} Minutes");}
                else{
                    Util.WriteToChat($"You have {tmppyreal} Pyreals and can continue Clapping for {timeclap:%m} Minutes");}
                Util.WriteToChat($"Your effective Coins per hour is : {effectiveCRate:n}");
            }
            if (rpt == "Kill") 
            {
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You have killed {killsTotal:n0} creatures in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {killsperhr:n0} Kills per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You have killed {killsTotal:n0} creatures in {elapsed:%h} Hours {elapsed:%m} Minutes for {killsperhr:n0} Kills per hour.");}
                else {
                    Util.WriteToChat($"You have killed {killsTotal:n0} creatures in {elapsed:%m} Minutes for {killsperhr:n0} Kills per hour.");}
            }
            if (rpt == "XP")
            {
                if (elapsed.Days > 0){
                    Util.WriteToChat($"You have earned {tmpxpdiff} XP in {elapsed:%d} Days {elapsed:%h} Hours {elapsed:%m} Minutes for {tmpXPRate} XP per hour.");}
                else if (elapsed.Hours > 0) {
                    Util.WriteToChat($"You have earned {tmpxpdiff} XP in {elapsed:%h} Hours {elapsed:%m} Minutes for {tmpXPRate} XP per hour.");}
                else{
                    Util.WriteToChat($"You have earned {tmpxpdiff} XP in {elapsed:%m} Minutes for {tmpXPRate} XP per hour.");}
                if (timetolvl.Days >0){
                    Util.WriteToChat($"You Need: {tmpXPtoLevel} XP to level and will take {timetolvl:%d} Days {timetolvl:%h} Hours {timetolvl:%m} Minutes at the current rate");}
                else if (timetolvl.Hours >0) {
                    Util.WriteToChat($"You Need: {tmpXPtoLevel} XP to level and will take {timetolvl:%h} Hours {timetolvl:%m} Minutes at the current rate");}
                else{
                    Util.WriteToChat($"You Need: {tmpXPtoLevel} XP to level and will take {timetolvl:%m} Minutes at the current rate");}
            }
        }

        [BaseEvent("ChatBoxMessage")]
        private void  Current_ChatBoxMessage(object sender, ChatTextInterceptEventArgs e)
        {
            try 
            {
                int startmode = CoinMode;
                //Util.WriteToChat($"Message: {e.Text.ToLower()}");
                string checkstr = e.Text.ToLower();
                if (checkstr.StartsWith("[[bank] your balances are:")){bankdata = true; }
                if (checkstr.StartsWith("[bank] pyreals: "))
                {
                    string tmppy = checkstr.Substring(16).Replace(",","");
                    curPyreals = double.Parse(tmppy);
                }
                if (checkstr.StartsWith("[bank] luminance: "))
                {
                    string lumtmp = checkstr.Substring(18).Replace(",","");
                    currentLuminance = double.Parse(lumtmp);
                    if (initialLuminance == -1)
                    {
                        initialLuminance = currentLuminance;
                    }
                }
                if (checkstr.StartsWith("[bank] legendary keys: "))
                {
                    string tmplk = checkstr.Substring(23).Replace(",","");
                    curLegKey = double.Parse(tmplk);
                }
                if (checkstr.StartsWith("[bank] Mythical Keys: "))
                {
                    string tmpmk = checkstr.Substring(22).Replace(",","");
                    curMythKey = double.Parse(tmpmk);
                }
                if (checkstr.StartsWith("[bank] enlightened coins: "))
                {
                    string cointmp = checkstr.Substring(26).Replace(",","");
                    currentCoins = double.Parse(cointmp);
                    if (initialCoins == -1)
                    {
                        initialCoins = currentCoins;
                    }
                }
                if (checkstr.StartsWith("you've banked ")) //You've banked 379,567 Luminance.
                {
                    checkstr = checkstr.Replace(",", "");
                    killLuminance += double.Parse(Regex.Match(checkstr, @"\d+").Value);
                    killsTotal += 1;
                }
                if ((eatbank == true) && (checkstr.StartsWith("[bank]"))) {e.Eat = true;}
                if (checkstr.StartsWith("[bank] weakly enlightened coins:"))
                {
                    string tmpwec = checkstr.Substring(33).Replace(",","");
                    curWEnlCoin = double.Parse(tmpwec);
                    eatbank = false; 
                    bankdata = false;
                    clrTimer.Stop();
                    if (!progenable){ return;}
                    doCalcs();
                    updateGUI();
                }
                if (checkstr.StartsWith("[xp] your xp to next level is:"))
                {
                    string xptoleveltmp = checkstr.Substring(30).Replace(",","");
                    xpToLevel = double.Parse(xptoleveltmp);
                    if (eatxp)
                    {
                        e.Eat = true;
                    }
                    eatxp = false;
                }
                if (autoResetEnabled){
                    bool isAugUsage= false;
                    if (checkstr.StartsWith("you have successfully increased your")) {isAugUsage = true;}                    
                    bool isCoinTransfer = Regex.IsMatch(checkstr,
                        @"^(transferred \d+ enlightened coins to .+|" +  //Transferred 25 Enlightened coins to Melka Summoner
                        @"received \d+ enlightened coins from .+)$");  //Received 25 Enlightend Coins from Melkoran
                    bool isLuminanceTransfer = Regex.IsMatch(checkstr,
                            @"^(transferred \d+ luminance to .+|" +
                            @"received \d+ luminance from .+)$");
                                        
                    if (isCoinTransfer || isLuminanceTransfer || isAugUsage)
                        {
                            totalReset();
                        }
                }                
                if (!coinusebank){
                    bool isShell = Regex.IsMatch(checkstr, @"olthoi gives you olthoi slimy shell.$"); //Repulsive Olthoi gives you Olthoi Slimy Shell.   350 MMDs and 5 Completed Mythical Keys per 100
                    bool isShell2 = Regex.IsMatch(checkstr, @"slug gives you olthoi slimy shell.$");
                    bool isShell3 = Regex.IsMatch(checkstr, @"parasite gives you olthoi slimy shell.$");
                    bool isSkull = Regex.IsMatch(checkstr, @"bones gives you badass skull.$"); //Slimy Badass Bones gives you Badass Skull.   80 MMDs 200 Weakly Coins
                    bool isPyreal = Regex.IsMatch(checkstr, @"warrior gives you ancient pyreal.$"); //Ancient Warrior gives you Ancient Pyreal. Time-Lost Warrior gives you Ancient Pyreal.  37 Enlightened Coins. per 100
                    bool isPyreal2 = Regex.IsMatch(checkstr, @"one gives you ancient pyreal.$");//Forgotten One gives you Ancient Pyreal.
                    bool isEgg = Regex.IsMatch(checkstr, @"penguin gives you dire siraluun egg.$"); //Dire Siraluun Penguin gives you Dire Siraluun Egg.  100 Trade Notes (250,000).10 Enlightened Coin. per 100
                    bool isSnowTrnk = Regex.IsMatch(checkstr, @"snowman gives you ancient falatacot trinket.$"); //Unhappy Snowman gives you Ancient Falatacot Trinket.
                    bool isSnowAeth = Regex.IsMatch(checkstr, @"snowman gives you blue aetheria chunk.$"); //Unhappy Snowman gives you Blue Aetheria Chunk.
                    bool isSnowMMD = Regex.IsMatch(checkstr, @"snowman gives you trade note (250,000).$"); //Unhappy Snowman gives you Trade Note (250,000).
                    bool isJam = Regex.IsMatch(checkstr, @"golem gives you strawberry jam jar.$"); //Strawberry Jam Golem gives you Strawberry Jam Jar.
                    bool isAetheria = Regex.IsMatch(checkstr, @"gives you red aetheria chunk.$"); //Coruscating Death gives you Coalesced Aetheria.    Pancake Liberator gives you Red Aetheria Chunk
                    bool isTrinket = Regex.IsMatch(checkstr, @"gives you ancient empyrean trinket.$"); //Coruscating Death gives you Ancient Empyrean Trinket.
                    
                    if (isSnowAeth) {CoinMode = 4; if (startmode != CoinMode){totalReset();} curBAetheria += 1;} //0=red, 1=egg, 2=shells, 3=coins, 4=snowmen, 5=Jam, 6=Skull  
                    if (isSnowTrnk) {CoinMode = 4; if (startmode != CoinMode){totalReset();} curFaltTrinket += 1;}
                    if (isSnowMMD) {CoinMode = 4; if (startmode != CoinMode){totalReset();} curMMD += 1;} 
                    if (isShell) {CoinMode = 2; if (startmode != CoinMode){totalReset();} curSlimyShells += 1;}
                    if (isPyreal || isPyreal2) {CoinMode = 3; if (startmode != CoinMode){totalReset();} curTimelostCoins += 1;}
                    if (isEgg) {CoinMode = 1; if (startmode != CoinMode){totalReset();} curPengEgg += 1;}
                    if (isJam) {CoinMode = 5; if (startmode != CoinMode){totalReset();} curJams += 1;}
                    if (isSkull) {CoinMode = 6; if (startmode != CoinMode){totalReset();} curSkulls += 1;}
                    if (isAetheria && !isSnowAeth){CoinMode = 0; if (startmode != CoinMode){totalReset();} curAetheria += 1;}
                    if (isTrinket) {CoinMode = 0; if (startmode != CoinMode){totalReset();} curTrinket += 1;}
                    if (startmode != CoinMode)
                    {
                        string tmpmodestr = "";
                        if (CoinMode == 0){tmpmodestr = "Normal Trinket/Aetheria";}
                        else if (CoinMode == 1){tmpmodestr = "Eggs";}
                        else if (CoinMode == 2){tmpmodestr = "Slimy Shells";}
                        else if (CoinMode == 3){tmpmodestr = "Ancient Pyreals";}
                        else if (CoinMode == 4){tmpmodestr = "Snowmen Faltacot Trinkets/Blue Aetheria";}
                        else if (CoinMode == 5){tmpmodestr = "Straberry Jam Jars";}
                        else if (CoinMode == 6){tmpmodestr = "Badass Bone Skulls";}
                        Util.WriteToChat($"Mode Changed to: {tmpmodestr}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                Util.WriteToChat($"Command Line Processing Error: {ex}");
            }
        }

        [BaseEvent("CommandLineText")]
        void Core_CommandLineText(object sender, ChatParserInterceptEventArgs e)
        {
            try
            {
                string text = e.Text;
                if (text.Length <= 0)
                {
                    return;
                }
                // Split into tokens for easier processing
                string[] tokens = text.Split(' ');
                string command = tokens[0].ToLower();

                if (command.StartsWith("@MelksLuminanceTracker") ||
                    command.StartsWith("@mlt") ||
                    command.StartsWith("/MelksLuminanceTracker") ||
                    command.StartsWith("/mlt"))
                {
                    e.Eat = true;
                    ProcessCommand(tokens);
                }
            }
            catch (Exception ex) {Util.WriteToChat($"Command Line Text Error: {ex}");}
        }

        private void ProcessCommand(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0)
                {
                    return;
                }
                if (tokens.Length < 2 || tokens[1].ToLower() == "help")
                {
                    Util.WriteToChat("Melk's Luminance Tracker");
                    Util.WriteToChat("/mlt reset");
                    Util.WriteToChat("/mlt stop");
                    Util.WriteToChat("/mlt start");
                    Util.WriteToChat("/mlt debug");
                    Util.WriteToChat("/mlt report lum");
                    Util.WriteToChat("/mlt report lume");
                    Util.WriteToChat("/mlt report coin");
                    Util.WriteToChat("/mlt report coine");
                    Util.WriteToChat("/mlt report kills");
                    Util.WriteToChat("/mlt txcoins");
                    Util.WriteToChat("/mlt txlum");
                    Util.WriteToChat("/mlt silentpoll");
                    Util.WriteToChat("/mlt killpop");
                    Util.WriteToChat("/mlt xppop");
                    Util.WriteToChat("/mlt mode [normal/eggs/shells/timelost/snowman/jams/skulls]");
                    Util.WriteToChat("The Lum-Coin Conversion value is the price of 1 coin in millions of luminance.");
                    Util.WriteToChat("The Coin-Lum Conversion value is the amount of luminance you get for 1 coin.");
                    Util.WriteToChat("It will calculate how many coins/hr you make just from luminance.");
                    Util.WriteToChat("The Auto Reset button enables/disables resetting all values automatically when there is a transfer of Lum or Coins or spending of luminance");
                    Util.WriteToChat("The Coin by Bank button uses the coins in your bank to calculate your hourly coins gained instead of atheria/trinket pickups");
                    Util.WriteToChat("The program Automatically polls the bank every minute but hides the output.");
                    Util.WriteToChat("the q/b button also polls the bank to update values and hides the output");
                    return;
                }
                if (tokens[1].ToLower() == "reset")
                {
                    Util.WriteToChat("Resetting Values");
                    totalReset();
                }
                if (tokens[1].ToLower() == "stop")
                {
                    if (progenable == false) {Util.WriteToChat("Already Stopped"); return;}
                    Util.WriteToChat("Calculations Stopped");
                    progenable = false;
                }
                if (tokens[1].ToLower() == "start")
                {
                    if (progenable == true) {Util.WriteToChat("Already Running"); return;}
                    Util.WriteToChat("Calculations Started");
                    progenable= true;
                }
                if (tokens[1].ToLower() == "mode")
                {
                    if (tokens.Length < 3 )
                    {
                        CoinMode = 0;
                        Util.WriteToChat("Mode Changed to: Normal Trinket/Aetheria");
                        totalReset();
                        return;
                    } 
                    if (tokens[2].ToLower() == "normal")
                    {
                        CoinMode = 0;}
                    if ((tokens[2].ToLower() == "eggs") || (tokens[2].ToLower() == "egg"))
                    {
                        CoinMode = 1;}
                    if ((tokens[2].ToLower() == "shells") || (tokens[2].ToLower() == "shell"))
                    {
                        CoinMode = 2;}
                    if ((tokens[2].ToLower() == "timelost") || (tokens[2].ToLower() == "timelostcoins"))
                    {
                        CoinMode = 3;}
                    if ((tokens[2].ToLower() == "timelost") || (tokens[2].ToLower() == "timelostcoins"))
                    {
                        CoinMode = 3;}
                    if ((tokens[2].ToLower() == "snowman") || (tokens[2].ToLower() == "snowmen"))
                    {
                        CoinMode = 4;}
                    if ((tokens[2].ToLower() == "jams") || (tokens[2].ToLower() == "jam"))
                    {
                        CoinMode = 5;}
                    if ((tokens[2].ToLower() == "skulls") || (tokens[2].ToLower() == "skull"))
                    {
                        CoinMode = 6;}
                    string tmpmodestr = "";
                    if (CoinMode == 0){tmpmodestr = "Normal Trinket/Aetheria";}
                    else if (CoinMode == 1){tmpmodestr = "Eggs";}
                    else if (CoinMode == 2){tmpmodestr = "Slimy Shells";}
                    else if (CoinMode == 3){tmpmodestr = "Ancient Pyreals";}
                    else if (CoinMode == 4){tmpmodestr = "Snowmen Faltacot Trinkets/Blue Aetheria";}
                    else if (CoinMode == 5){tmpmodestr = "Straberry Jam Jars";}
                    else if (CoinMode == 6){tmpmodestr = "Badass Bone Skulls";}
                    Util.WriteToChat($"Mode Changed to: {tmpmodestr}");
                    totalReset();
                }
                if (tokens[1].ToLower() == "debug")
                {
                    if (tokens.Length < 3 )
                    {
                        enbDebug = !enbDebug;
                        Util.WriteToChat($"Debugging {(enbDebug ? "Enabled" : "Disabled")}");
                        return;
                    }
                    if ((tokens[2].ToLower() == "true") || (tokens[2].ToLower() == "on"))
                    {
                        enbDebug = true;
                    }
                    if ((tokens[2].ToLower() == "false") || (tokens[2].ToLower() == "off"))
                    {
                        enbDebug = false;
                    }
                    Util.WriteToChat($"Debugging {(enbDebug ? "Enabled" : "Disabled")}");
                }
                if (tokens[1].ToLower() == "silentpoll")
                {
                    if (enbDebug){Util.WriteToChat($"silentpoll Activated");}
                    bankPoll(true);
                }
                if (tokens[1].ToLower() == "txcoins")
                {
                    txStuff("Coins");
                    SaveSettings();
                }
                if (tokens[1].ToLower() == "txlum")
                {
                    txStuff("Lum");
                    SaveSettings();
                }
                if (tokens[1].ToLower() == "killpop")
                {
                    showpopup();
                }
                if (tokens[1].ToLower() == "xppop")
                {
                    showpopup2();
                }
                if (tokens[1].ToLower() == "report")
                {
                    if (tokens.Length < 3 ){return;}
                    if (tokens[2].ToLower() == "lum")
                    {
                        reportsOutput("Lum");
                    }
                    if (tokens[2].ToLower() == "lume")
                    {
                        reportsOutput("LumE");
                    }
                    if (tokens[2].ToLower() == "coin")
                    {
                        reportsOutput("Coin");
                    }
                    if (tokens[2].ToLower() == "coine")
                    {
                        reportsOutput("CoinE");
                    }
                    if (tokens[2].ToLower() == "xp")
                    {
                        reportsOutput("XP");       
                    }       
                }
            }
            catch (Exception ex) {Util.WriteToChat($"Command Line Processing Error: {ex}");}
        }		
	}
}

/*


[BANK] Your balances are:
[BANK] Pyreals: 811,645,692
[BANK] Luminance: 25,653,561,269
[BANK] Legendary Keys: 178
[BANK] Mythical Keys: 605
[BANK] Enlightened Coins: 113
[BANK] Weakly Enlightened Coins: 3
*/