using System.Windows.Forms;

namespace BCILib.Amp
{
    public partial class FakeAmplifier_Config : Form
    {
        public FakeAmplifier_Config()
        {
            InitializeComponent();
        }

        public int NumChannels
        {
            get
            {
                return (int) numericUpDownNumChannel.Value;
            }

            set
            {
                numericUpDownNumChannel.Value = value;
            }
        }

        public int SampleRate
        {
            get
            {
                return (int)numericUpDownSplRate.Value;
            }

            set
            {
                numericUpDownSplRate.Value = value;
            }
        }

        public string ChannelNameString
        {
            get
            {
                return textBoxChannelString.Text;
            }

            set
            {
                textBoxChannelString.Text = value;
            }
        }

        private void buttonChangeChannel_Click(object sender, System.EventArgs e)
        {
            ChannelCfgForm cform = new ChannelCfgForm();
            cform.ChannelNames = textBoxChannelString.Text.Split(',');
            if (cform.ShowDialog() == DialogResult.OK) {
                textBoxChannelString.Text = string.Join(",", cform.ChannelNames);
            }
        }
    }
}
