using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using WebSpiderLib;

namespace WebSpiderForm
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();
        }

        private bool RunSpider()
        {
            try
            {
                Uri uri = new Uri(textBoxStartUri.Text);
                if (!WebsiteUp(uri))
                    return false;

                //WebCrawler crawler = new WebCrawler(Filter, WriteGreen, WriteRed);
                //crawler.Explore(uri);
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        private void WriteRed( string s)
        {
            labelLog.ForeColor = Color.Red;
            labelLog.Text = s;
        }

        private void WriteGreen(string s)
        {
            labelLog.ForeColor = Color.Lime;
            labelLog.Text = s;
        }

        private bool Filter(Uri uri)
        {
            string href = uri.AbsoluteUri;
            if (href.Contains(textBoxFilter1.Text) && href.Contains(textBoxFilter2.Text))
                if (href.Contains("?"))
                    if (checkBoxGet.Checked && href.Contains(textBoxGet1.Text) && href.Contains(textBoxGet2.Text) &&
                        href.Contains(textBoxGet3.Text) && href.Contains(textBoxGet4.Text) &&
                        href.Contains(textBoxGet5.Text))
                        return true;
                    else
                        return false;
                else
                    return true;
            return false;
        }

        private void checkBoxGet_CheckedChanged(object sender, EventArgs e)
        {
            textBoxGet1.Enabled = checkBoxGet.Checked;
            textBoxGet2.Enabled = checkBoxGet.Checked;
            textBoxGet3.Enabled = checkBoxGet.Checked;
            textBoxGet4.Enabled = checkBoxGet.Checked;
            textBoxGet5.Enabled = checkBoxGet.Checked;
        }

        private void textBoxStartUri_TextChanged(object sender, EventArgs e)
        {
            try
            {
                new Uri(textBoxStartUri.Text);
                textBoxStartUri.BackColor = Color.LimeGreen;
                buttonStart.Enabled = true;
            }
            catch (Exception)
            {
                textBoxStartUri.BackColor = Color.IndianRed;
                buttonStart.Enabled = false;
            }
        }

        private bool WebsiteUp(Uri uri)
        {
            try
            {
                WebRequest request = WebRequest.Create(uri);
                request.Timeout = 3000;
                request.Method = "HEAD";
                using (WebResponse response = request.GetResponse())
                {
                    HttpWebResponse hRes = response as HttpWebResponse;
                    if (hRes == null)
                        throw new ArgumentException("Not an HTTP or HTTPS request"); // you may want to have this specifically handle e.g. FTP, but I'm just throwing an exception for now.
                    return (int)hRes.StatusCode / 100 == 2;
                }
            }
            catch (WebException)
            {
                return false;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            buttonStart.Enabled = !RunSpider();
        }
    }
}
