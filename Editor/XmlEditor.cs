using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CharacterBuilder.ChampionsOnline.Model;
using System.Xml.Serialization;
using System.IO;

namespace CharacterBuilder.ChampionsOnline.Editor
{
    //white is default
    //<p> for paragraphs
    //<br> for line break
    //<h1>  RESERVED FOR NAME, DONT USE MANUALLY
    //<h2>  subheadings (bright yellow)
    //<span>  text bright yellow (not a subheading)
    //<ul>  for whole list
    //<li>  one bullet item

    //images/powerhouse/Characteristic Focus/
    //images/powerhouse/Innate Talents/
    //images/powerhouse/Talents/
    //images/powerhouse/Travel Powers/
    //images/powerhouse/Travel Powers/Advantages/
    //images/powerhouse/Powers/Electricity/
    //images/powerhouse/Powers/Electricity/Advantages/


    public partial class XmlEditor : Form
    {
        private Statistics _mainStatistics = new Statistics();

        private TableLayoutPanel _activeTableLayout;


        private void ReloadTree()
        {
            foreach (TreeNode node in StatisticsTreeview.Nodes)
                node.Nodes.Clear();
            foreach (CharacterBuilder.ChampionsOnline.Model.Statistic statistic in _mainStatistics.Stats)
            {
                TreeNode node = new TreeNode(statistic.Name);
                node.Tag = statistic;
                StatisticsTreeview.Nodes["StatisticNode"].Nodes.Add(node);
            }
            foreach (CharacterBuilder.ChampionsOnline.Model.Attribute attribute in _mainStatistics.Characteristics)
            {
                TreeNode node = new TreeNode(attribute.Name);
                node.Tag = attribute;
                StatisticsTreeview.Nodes["CharacteristicNode"].Nodes.Add(node);
            }
            foreach (CharacterBuilder.ChampionsOnline.Model.Attribute attribute in _mainStatistics.InnateTalents)
            {
                TreeNode node = new TreeNode(attribute.Name);
                node.Tag = attribute;
                StatisticsTreeview.Nodes["InnateTalentNode"].Nodes.Add(node);
            }
            foreach (CharacterBuilder.ChampionsOnline.Model.Attribute attribute in _mainStatistics.Talents)
            {
                TreeNode node = new TreeNode(attribute.Name);
                node.Tag = attribute;
                StatisticsTreeview.Nodes["TalentNode"].Nodes.Add(node);
            }
            foreach (CharacterBuilder.ChampionsOnline.Model.Power power in _mainStatistics.TravelPowers)
            {
                TreeNode node_advantage_title = new TreeNode("Advantages");
                node_advantage_title.Tag = "Advantages";

                foreach (CharacterBuilder.ChampionsOnline.Model.Advantage advantage in power.Advantages)
                {
                    TreeNode node_advantage = new TreeNode(advantage.Name);
                    node_advantage.Tag = advantage;
                    node_advantage_title.Nodes.Add(node_advantage);
                }
                TreeNode node = new TreeNode(power.Name);
                node.Tag = power;
                
                node.Nodes.Add(node_advantage_title);

                StatisticsTreeview.Nodes["TravelPowerNode"].Nodes.Add(node);
            }
            foreach (Powerset powerset in _mainStatistics.Powersets)
            {
                TreeNode node_powerset = new TreeNode(powerset.Name);
                node_powerset.Tag = powerset;

                TreeNode node_powers_title = new TreeNode("Powers");
                node_powers_title.Name = "Powers";
                node_powers_title.Tag = "Powers";


                foreach (CharacterBuilder.ChampionsOnline.Model.Power power in powerset.Powers)
                {
                    TreeNode node_power = new TreeNode(power.Name);
                    node_power.Tag = power;

                    TreeNode node_advantage_title = new TreeNode("Advantages");
                    node_advantage_title.Tag = "Advantages";

                    foreach (CharacterBuilder.ChampionsOnline.Model.Advantage advantage in power.Advantages)
                    {
                        TreeNode node_advantage = new TreeNode(advantage.Name);
                        node_advantage.Tag = advantage;

                        node_advantage_title.Nodes.Add(node_advantage);
                    }

                    node_power.Nodes.Add(node_advantage_title);
                    node_powers_title.Nodes.Add(node_power);
                }

                node_powerset.Nodes.Add(node_powers_title);
                StatisticsTreeview.Nodes["PowersetNode"].Nodes.Add(node_powerset);
            }
        }

        private void SetAdvantage(CharacterBuilder.ChampionsOnline.Model.Advantage advantage)
        {
            // populate the UI from the object
            NameTextbox.Text = advantage == null ? string.Empty : advantage.Name;
            DescriptionTextbox.Text = advantage == null ? string.Empty : advantage.Description;
            EncodingTextbox.Text = advantage == null ? string.Empty : advantage.Code;
            if (StatisticsTreeview.SelectedNode.Parent.Parent.Name == "TravelPowerNode")
                ImageNameTextbox.Text = "images/powerhouse/Travel Powers/Advantages/";
            else
                ImageNameTextbox.Text = advantage == null ? "images/powerhouse/Powers/" + StatisticsTreeview.SelectedNode.Parent.Parent.Parent.Text + "/Advantages/" : advantage.ImageName;

            CostTextBox.Text = advantage == null ? string.Empty : advantage.Cost.ToString();
        
            // create the list of advantages...this box will be populated the same regardless of function parameters...
            // ...but the selected item *will* be different based on function parameters

            // first, clear the listbox
            AdvantageReqCombobox.Items.Clear();
            
            // next, add a blank line to the listbox
            AdvantageReqCombobox.Items.Add(new Advantage());

            // then, add all the values for the ***power*** to the listbox
            if (advantage != null)
            {
                foreach (Advantage thisAdvantage in ((CharacterBuilder.ChampionsOnline.Model.Power)StatisticsTreeview.SelectedNode.Parent.Parent.Tag).Advantages)
                {
                    if (thisAdvantage != advantage)
                        AdvantageReqCombobox.Items.Add(thisAdvantage.Name);
                }
                // the advantage is not null, so set the required advantage to the correct one
                if (advantage.RequiredAdvantage == null)
                    AdvantageReqCombobox.SelectedIndex = 0;
                else
                    AdvantageReqCombobox.SelectedIndex = AdvantageReqCombobox.FindStringExact(advantage.RequiredAdvantage);
            }
            else
            {
                foreach (Advantage thisAdvantage in ((CharacterBuilder.ChampionsOnline.Model.Power)StatisticsTreeview.SelectedNode.Parent.Tag).Advantages)
                {
                    AdvantageReqCombobox.Items.Add(thisAdvantage.Name);
                }
                // the advantage is null, so just set the required advantage to the first one in the list
                AdvantageReqCombobox.SelectedIndex = 0;
            }


        }

        private void SetStatistic(CharacterBuilder.ChampionsOnline.Model.Statistic statistic)
        {
            // populate the UI from the object
            NameTextbox.Text = statistic == null ? string.Empty : statistic.Name;
            DescriptionTextbox.Text = statistic == null ? string.Empty : statistic.Description;
            ImageNameTextbox.Text = statistic == null ? string.Empty : statistic.ImageName;
            EncodingTextbox.Text = statistic == null ? string.Empty : statistic.Code;
        }

        private void SetAttribute(CharacterBuilder.ChampionsOnline.Model.Attribute attribute)
        {
            // populate the UI from the object
            NameTextbox.Text = attribute == null ? string.Empty : attribute.Name;
            DescriptionTextbox.Text = attribute == null ? string.Empty : attribute.Description;
            ImageNameTextbox.Text = attribute == null ? string.Empty : attribute.ImageName;
            EncodingTextbox.Text = attribute == null ? string.Empty : attribute.Code;
            StrengthTextbox.Text = attribute == null ? string.Empty : attribute.Strength.ToString();
            DexterityTextbox.Text = attribute == null ? string.Empty : attribute.Dexterity.ToString();
            ConstitutionTextbox.Text = attribute == null ? string.Empty : attribute.Constitution.ToString();
            EgoTextbox.Text = attribute == null ? string.Empty : attribute.Ego.ToString();
            PresenceTextbox.Text = attribute == null ? string.Empty : attribute.Presence.ToString();
            EnduranceTextbox.Text = attribute == null ? string.Empty : attribute.Endurance.ToString();
            IntelligenceTextbox.Text = attribute == null ? string.Empty : attribute.Intelligence.ToString();
            RecoveryTextbox.Text = attribute == null ? string.Empty : attribute.Recovery.ToString();
        }

        private void SetPowerset(CharacterBuilder.ChampionsOnline.Model.Powerset powerset)
        {
            // populate the UI from the object
            NameTextbox.Text = powerset == null ? string.Empty : powerset.Name;
            DescriptionTextbox.Text = powerset == null ? string.Empty : powerset.Description;
            ImageNameTextbox.Text = powerset == null ? string.Empty : powerset.ImageName;
            EncodingTextbox.Text = powerset == null ? string.Empty : powerset.Code;
            PowersetGroupTextbox.Text = powerset == null ? string.Empty : powerset.PowersetGroup;
        }

        private void SetPower(CharacterBuilder.ChampionsOnline.Model.Power power)
        {
            // populate the UI from the object
            NameTextbox.Text = power == null ? string.Empty : power.Name;
            EncodingTextbox.Text = power == null ? string.Empty : power.Code;
            PowerTypeTextBox.Text = power == null ? string.Empty : power.PowerType;
            FloatingGroupPowerTextbox.Text = power == null ? string.Empty : power.FloatingGroupPower.ToString();

            if (power != null)
                DescriptionTextbox.Text = power.Description;
            else
                DescriptionTextbox.Text = string.Empty;

            //DescriptionTextbox.Text = power == null ? string.Empty : power.Description;
            if (StatisticsTreeview.SelectedNode.Name == "TravelPowerNode")
                ImageNameTextbox.Text = "images/powerhouse/Travel Powers/";
            else
                ImageNameTextbox.Text = power == null ? "images/powerhouse/Powers/" + StatisticsTreeview.SelectedNode.Parent.Text + "/" : power.ImageName;

            InsideReqTextbox.Text = power == null ? string.Empty : power.InsetRequirement.ToString();
            OutsideReqTextbox.Text = power == null ? string.Empty : power.OutsetRequirement.ToString();
            
            while (PowerTagsCheckedListbox.CheckedItems.Count > 0)
            {
                PowerTagsCheckedListbox.SetItemChecked(PowerTagsCheckedListbox.CheckedIndices[0], false);
            }

            if (power != null)
            {
                foreach (string tag in power.Tags.ToList())
                {
                    int position = PowerTagsCheckedListbox.FindStringExact(tag);
                    if (position != ListBox.NoMatches)
                        PowerTagsCheckedListbox.SetItemChecked(position, true);
                }
            }
        }

        private void GetAdvantage(CharacterBuilder.ChampionsOnline.Model.Advantage advantage)
        {
            int stat;
            
            // populate the advantage object from the UI
            advantage.Name = NameTextbox.Text;
            advantage.Description = DescriptionTextbox.Text;
            advantage.ImageName = ImageNameTextbox.Text;
            advantage.Code = EncodingTextbox.Text;

            if (int.TryParse(CostTextBox.Text, out stat))
                advantage.Cost = stat;

            if (AdvantageReqCombobox.Text == string.Empty)
                advantage.RequiredAdvantage = null;
            else
            {
                //save the required advantage    
                advantage.RequiredAdvantage = AdvantageReqCombobox.SelectedItem.ToString();
            }
        }

        private void GetAttribute(CharacterBuilder.ChampionsOnline.Model.Attribute attribute)
        {
            // populate the object from the UI
            int stat;

            attribute.Name = NameTextbox.Text;
            attribute.Description = DescriptionTextbox.Text;
            attribute.ImageName = ImageNameTextbox.Text;
            attribute.Code = EncodingTextbox.Text;

            if (int.TryParse(StrengthTextbox.Text, out stat))
                attribute.Strength = stat;
            if (int.TryParse(DexterityTextbox.Text, out stat))
                attribute.Dexterity = stat;
            if (int.TryParse(ConstitutionTextbox.Text, out stat))
                attribute.Constitution = stat;
            if (int.TryParse(EgoTextbox.Text, out stat))
                attribute.Ego = stat;
            if (int.TryParse(PresenceTextbox.Text, out stat))
                attribute.Presence = stat;
            if (int.TryParse(EnduranceTextbox.Text, out stat))
                attribute.Endurance = stat;
            if (int.TryParse(IntelligenceTextbox.Text, out stat))
                attribute.Intelligence = stat;
            if (int.TryParse(RecoveryTextbox.Text, out stat))
                attribute.Recovery = stat;
        }

        private void GetStatistic(CharacterBuilder.ChampionsOnline.Model.Statistic statistic)
        {
            // populate the object from the UI

            statistic.Name = NameTextbox.Text;
            statistic.Description = DescriptionTextbox.Text;
            statistic.ImageName = ImageNameTextbox.Text;
            statistic.Code = EncodingTextbox.Text;

        }

        private void GetPower(CharacterBuilder.ChampionsOnline.Model.Power power)
        {
            // populate the object from the UI
            int stat;

            power.Name = NameTextbox.Text;
            power.PowerType = PowerTypeTextBox.Text;
            power.Description = DescriptionTextbox.Text;
            power.ImageName = ImageNameTextbox.Text;
            power.Code = EncodingTextbox.Text;

            if (int.TryParse(FloatingGroupPowerTextbox.Text, out stat))
                power.FloatingGroupPower = stat;
            if (int.TryParse(InsideReqTextbox.Text, out stat))
                power.InsetRequirement = stat;
            if (int.TryParse(OutsideReqTextbox.Text, out stat))
                power.OutsetRequirement = stat;

            power.Tags.Clear();
            for (int i = 0; i < PowerTagsCheckedListbox.CheckedItems.Count; i++)
            {
                power.Tags.Add(PowerTagsCheckedListbox.CheckedItems[i].ToString());
            }
        }

        private void GetPowerset(CharacterBuilder.ChampionsOnline.Model.Powerset powerset)
        {
            // populate the object from the UI
            powerset.Name = NameTextbox.Text;
            powerset.Description = DescriptionTextbox.Text;
            powerset.ImageName = ImageNameTextbox.Text;
            powerset.Code = EncodingTextbox.Text;
            powerset.PowersetGroup = PowersetGroupTextbox.Text;
        }

        private void ActivateTableLayout(int heightPixels, TableLayoutPanel panelToActivate)
        {
            if (_activeTableLayout != null)
            {
                _activeTableLayout.Dock = DockStyle.None;
                _activeTableLayout.Visible = false;
            }

            if (panelToActivate != null)
            {
                panelToActivate.Dock = DockStyle.Bottom;
                panelToActivate.Height = heightPixels;
                panelToActivate.Visible = true;
            }
            
            _activeTableLayout = panelToActivate;
            CommonTableLayoutPanel.BringToFront();    
        }

        public string FixStringForTextboxDisplay(string inString)
        {
            // this method is intended to fix a display problem that is occurring after a piece of text is saved
            // as XML and then deserialized back into an object...what is happening is this:
            //
            // i.e.  text entered as: 
            //         "line one\r\n\r\nlinethree"
            //       after it gets saved to XML and comes back, it looks like this:
            //         "line one\n\nlinethree"
            // and this looks all jacked in the textbox

            //turn just \n into newline, but NOT \r\n
            //outString = inString.Replace("\n", Environment.NewLine);

            // the above two lines are giving me problems...gonna loop through using this logic:
            // look for only "\n"  but NOT this:  "\r\n"   ...if TRUE then replace with \r\n

            string outString = "";
            string insertableString = "";

            for (int i = 0; i < inString.Length; i++)
            {
                // loop through the characters of the string and check this entry for "\n"

                if (inString.Substring(i, 1) == "\n")
                {
                    // we've found a "\n"...will probably replace it, but let's see if we need to...
                    insertableString = "\r\n";
                    if (i >= 1)
                    {
                        if (inString.Substring(i - 1, 2) == "\r\n")
                            insertableString = "\n";
                    }
                    // now catch up the existing string up to this point and add your change
                    outString = outString + insertableString;
                }
                else
                    outString = outString + inString.Substring(i, 1);
            }
            return outString;
        }

        public XmlEditor()
        {
            InitializeComponent();
            CommonTableLayoutPanel.Dock = DockStyle.Fill;

            
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {        
            this.Close();
        }

        private void StatisticsTreeview_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // start by enabling or disabling the PowersetGroup Label and Textbox
            if (e.Node == StatisticsTreeview.Nodes["PowersetNode"] || e.Node.Tag is CharacterBuilder.ChampionsOnline.Model.Powerset)
            {
                PowersetGroupLabel.Visible = true;
                PowersetGroupTextbox.Visible = true;
            }
            else
            {
                PowersetGroupLabel.Visible = false;
                PowersetGroupTextbox.Visible = false;
            }


            if (e.Node == StatisticsTreeview.Nodes["CharacteristicNode"])
            {
                ActivateTableLayout(120, AttributeTableLayoutPanel);
                SetAttribute(null);
            }
            else if (e.Node == StatisticsTreeview.Nodes["StatisticNode"])
            {
                ActivateTableLayout(1, null);
                SetStatistic(null);
            }
            else if (e.Node == StatisticsTreeview.Nodes["InnateTalentNode"])
            {
                ActivateTableLayout(120, AttributeTableLayoutPanel);
                SetAttribute(null);
            }
            else if (e.Node == StatisticsTreeview.Nodes["TalentNode"])
            {
                ActivateTableLayout(120, AttributeTableLayoutPanel);
                SetAttribute(null);
            }
            else if (e.Node == StatisticsTreeview.Nodes["TravelPowerNode"] || ((e.Node.Tag as string) == "Powers"))
            {
                ActivateTableLayout(190, PowerTableLayoutPanel);
                SetPower(null);
            }
            else if (e.Node == StatisticsTreeview.Nodes["PowersetNode"])
            {
                SetPowerset(null);
                ActivateTableLayout(0, null);
            }
            else if (e.Node.Tag is CharacterBuilder.ChampionsOnline.Model.Powerset)
            {
                SetPowerset((CharacterBuilder.ChampionsOnline.Model.Powerset)e.Node.Tag);
                ActivateTableLayout(0, null);
            }
            else if (e.Node.Tag is CharacterBuilder.ChampionsOnline.Model.Attribute)
            {
                SetAttribute((CharacterBuilder.ChampionsOnline.Model.Attribute)e.Node.Tag);
                ActivateTableLayout(120, AttributeTableLayoutPanel);
            }

            else if (e.Node.Tag.GetType().Equals(typeof(CharacterBuilder.ChampionsOnline.Model.Statistic)))
            {
                SetStatistic((CharacterBuilder.ChampionsOnline.Model.Statistic)e.Node.Tag);
                ActivateTableLayout(1, null);
            }
            else if (e.Node.Tag is CharacterBuilder.ChampionsOnline.Model.Power)
            {
                SetPower((CharacterBuilder.ChampionsOnline.Model.Power)e.Node.Tag);
                ActivateTableLayout(190, PowerTableLayoutPanel);
            }
            else if (e.Node.Tag.ToString() == "Advantages")
            {
                ActivateTableLayout(100, AdvantageTableLayoutPanel);
                SetAdvantage(null);
            }
            else if (e.Node.Tag is CharacterBuilder.ChampionsOnline.Model.Advantage)
            {
                ActivateTableLayout(100, AdvantageTableLayoutPanel);
                SetAdvantage((CharacterBuilder.ChampionsOnline.Model.Advantage)e.Node.Tag);

            }
        }

        private void SaveToolStripButton_Click(object sender, EventArgs e)
        {
            if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["CharacteristicNode"] ||
                StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["InnateTalentNode"] ||
                StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["TalentNode"])
            {
                CharacterBuilder.ChampionsOnline.Model.Attribute attribute = new CharacterBuilder.ChampionsOnline.Model.Attribute();

                GetAttribute(attribute);

                if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["CharacteristicNode"])
                    _mainStatistics.Characteristics.Add(attribute);
                else if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["InnateTalentNode"])
                    _mainStatistics.InnateTalents.Add(attribute);
                else if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["TalentNode"])
                    _mainStatistics.Talents.Add(attribute);

                TreeNode node = new TreeNode(attribute.Name);
                node.Tag = attribute;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node; 
            }
            else if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["StatisticNode"])
            {
                CharacterBuilder.ChampionsOnline.Model.Statistic statistic = new CharacterBuilder.ChampionsOnline.Model.Statistic();

                GetStatistic(statistic);

                _mainStatistics.Stats.Add(statistic);

                TreeNode node = new TreeNode(statistic.Name);
                node.Tag = statistic;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node; 
            }
            else if (StatisticsTreeview.SelectedNode.Tag is CharacterBuilder.ChampionsOnline.Model.Attribute)
            {
                GetAttribute((CharacterBuilder.ChampionsOnline.Model.Attribute)StatisticsTreeview.SelectedNode.Tag);
                StatisticsTreeview.SelectedNode.Text = ((CharacterBuilder.ChampionsOnline.Model.Attribute)StatisticsTreeview.SelectedNode.Tag).Name;
            }
            else if (StatisticsTreeview.SelectedNode.Tag.GetType().Equals(typeof(CharacterBuilder.ChampionsOnline.Model.Statistic)))
            {
                GetStatistic((CharacterBuilder.ChampionsOnline.Model.Statistic)StatisticsTreeview.SelectedNode.Tag);
                StatisticsTreeview.SelectedNode.Text = ((CharacterBuilder.ChampionsOnline.Model.Statistic)StatisticsTreeview.SelectedNode.Tag).Name;
            }
            else if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["TravelPowerNode"])
            {
                CharacterBuilder.ChampionsOnline.Model.Power power = new CharacterBuilder.ChampionsOnline.Model.Power();

                GetPower(power);

                _mainStatistics.TravelPowers.Add(power);

                TreeNode node = new TreeNode(power.Name);
                node.Tag = power;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node;

                TreeNode node2 = new TreeNode("Advantages");
                node2.Tag = "Advantages";
                StatisticsTreeview.SelectedNode.Nodes.Add(node2);
            }
            else if (StatisticsTreeview.SelectedNode.Tag.ToString() == "Powers")
            {
                CharacterBuilder.ChampionsOnline.Model.Power power = new CharacterBuilder.ChampionsOnline.Model.Power();

                GetPower(power);

                ((CharacterBuilder.ChampionsOnline.Model.Powerset)StatisticsTreeview.SelectedNode.Parent.Tag).Powers.Add(power);


                TreeNode node = new TreeNode(power.Name);
                node.Tag = power;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node;

                TreeNode node2 = new TreeNode("Advantages");
                node2.Tag = "Advantages";
                StatisticsTreeview.SelectedNode.Nodes.Add(node2);
            }
            else if (StatisticsTreeview.SelectedNode.Tag is CharacterBuilder.ChampionsOnline.Model.Power)
            //else if (StatisticsTreeview.SelectedNode.Tag is CharacterBuilder.ChampionsOnline.Model.Power &&
            //    StatisticsTreeview.SelectedNode.Parent == StatisticsTreeview.Nodes["TravelPowerNode"])
            {
                GetPower((CharacterBuilder.ChampionsOnline.Model.Power)StatisticsTreeview.SelectedNode.Tag);
                StatisticsTreeview.SelectedNode.Text = ((CharacterBuilder.ChampionsOnline.Model.Power)StatisticsTreeview.SelectedNode.Tag).Name;
            }
            else if (StatisticsTreeview.SelectedNode.Tag.ToString() == "Advantages")
            {
                CharacterBuilder.ChampionsOnline.Model.Advantage advantage = new CharacterBuilder.ChampionsOnline.Model.Advantage();

                GetAdvantage(advantage);

                ((CharacterBuilder.ChampionsOnline.Model.Power)StatisticsTreeview.SelectedNode.Parent.Tag).Advantages.Add(advantage);

                TreeNode node = new TreeNode(advantage.Name);
                node.Tag = advantage;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node;
            }

            else if (StatisticsTreeview.SelectedNode.Tag is CharacterBuilder.ChampionsOnline.Model.Advantage)
            {
                GetAdvantage((CharacterBuilder.ChampionsOnline.Model.Advantage)StatisticsTreeview.SelectedNode.Tag);
                StatisticsTreeview.SelectedNode.Text = ((CharacterBuilder.ChampionsOnline.Model.Advantage)StatisticsTreeview.SelectedNode.Tag).Name;
            }
            else if (StatisticsTreeview.SelectedNode == StatisticsTreeview.Nodes["PowersetNode"])
            {
                CharacterBuilder.ChampionsOnline.Model.Powerset powerset = new CharacterBuilder.ChampionsOnline.Model.Powerset();

                GetPowerset(powerset);

                _mainStatistics.Powersets.Add(powerset);

                TreeNode node = new TreeNode(powerset.Name);
                node.Tag = powerset;
                StatisticsTreeview.SelectedNode.Nodes.Add(node);
                StatisticsTreeview.SelectedNode = node;

                TreeNode node2 = new TreeNode("Powers");
                node2.Tag = "Powers";
                StatisticsTreeview.SelectedNode.Nodes.Add(node2);
                StatisticsTreeview.SelectedNode = node;
            }
            else if ((StatisticsTreeview.SelectedNode.Tag) is CharacterBuilder.ChampionsOnline.Model.Powerset)
            {
                GetPowerset((Powerset)StatisticsTreeview.SelectedNode.Tag);
                StatisticsTreeview.SelectedNode.Text = ((Powerset)StatisticsTreeview.SelectedNode.Tag).Name;
            }
        }

        private void SaveFileToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog.InitialDirectory = "C:\\workspace\\Phoenix\\trunk\\Character Builders\\Champions Online\\Web\\App_Data";
                //Application.ExecutablePath;
            if (SaveFileDialog.ShowDialog() == DialogResult.Cancel)
                return;
            _mainStatistics.Serialize(SaveFileDialog.FileName);  
        }

        private void LoadToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog.InitialDirectory = "C:\\workspace\\Phoenix\\trunk\\Character Builders\\Champions Online\\Web\\App_Data";
                //Application.ExecutablePath;
            if (OpenFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            _mainStatistics =  Statistics.Deserialize(OpenFileDialog.FileName);

            ReloadTree();
        }

        private void OpenFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int i;
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void PowersetGroupTextbox_TextChanged(object sender, EventArgs e)
        {

        }

        private void PowerTableLayoutPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDiagnostic_Click(object sender, EventArgs e)
        {
            frmReport newReport = new frmReport(_mainStatistics.RunDiagnostic());
            newReport.ShowDialog();
        }

        private void DownToolStripButton_Click(object sender, EventArgs e)
        {
            // only doing this for advantages now

            // see if the selected item is an advantage
            if (StatisticsTreeview.SelectedNode.Tag is Advantage)
            {
                // see if there is an advantage after this one (there needs to be to move this one down)
                if (StatisticsTreeview.SelectedNode != StatisticsTreeview.SelectedNode.Parent.LastNode)
                {
                    // move it in the tree
                    TreeNode parentNode = StatisticsTreeview.SelectedNode.Parent;
                    TreeNode thisNode = StatisticsTreeview.SelectedNode;
                    int insertIndex = StatisticsTreeview.SelectedNode.Index + 1;

                    parentNode.Nodes.RemoveAt(StatisticsTreeview.SelectedNode.Index);
                    parentNode.Nodes.Insert(insertIndex, thisNode);

                    StatisticsTreeview.SelectedNode = parentNode.Nodes[insertIndex];

                    // change it in the data
                    Advantage thisAdvantage = ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages[insertIndex - 1];
                    ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages.Remove(thisAdvantage);
                    ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages.Insert(insertIndex, thisAdvantage);

                }
            }
            else
                MessageBox.Show("At the moment, this only works for Advantages.", "Sorry");
        }

        private void UpToolStripButton_Click(object sender, EventArgs e)
        {
            // only doing this for advantages now

            // see if the selected item is an advantage
            if (StatisticsTreeview.SelectedNode.Tag is Advantage)
            {
                // see if there is an advantage after this one (there needs to be to move this one down)
                if (StatisticsTreeview.SelectedNode != StatisticsTreeview.SelectedNode.Parent.FirstNode)
                {
                    // move it in the tree
                    TreeNode parentNode = StatisticsTreeview.SelectedNode.Parent;
                    TreeNode thisNode = StatisticsTreeview.SelectedNode;
                    int insertIndex = StatisticsTreeview.SelectedNode.Index - 1;

                    parentNode.Nodes.RemoveAt(StatisticsTreeview.SelectedNode.Index);
                    parentNode.Nodes.Insert(insertIndex, thisNode);

                    StatisticsTreeview.SelectedNode = parentNode.Nodes[insertIndex];

                    // change it in the data
                    Advantage thisAdvantage = ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages[insertIndex + 1];
                    ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages.Remove(thisAdvantage);
                    ((CharacterBuilder.ChampionsOnline.Model.Power)parentNode.Parent.Tag).Advantages.Insert(insertIndex, thisAdvantage);

                }
            }
            else
                MessageBox.Show("At the moment, this only works for Advantages.", "Sorry");
        }








    }
}
