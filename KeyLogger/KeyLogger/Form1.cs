using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WordLoggerDemo
{
    public class Form1 : Form
    {
        private TextBox tbInput;
        private ListBox lbWords;
        private Label lblTotalKeys;
        private Button btnExport;
        private Button btnClear;

        private StringBuilder currentWord = new StringBuilder();
        private int totalKeyCount = 0;
        private string logFilePath = "wordlog.txt";

        public Form1()
        {
            SetupControls();
        }

        private void SetupControls()
        {
            this.Text = "Kelime Logger (Uygulama içi)";
            this.Width = 700;
            this.Height = 450;
            this.StartPosition = FormStartPosition.CenterScreen;

            tbInput = new TextBox
            {
                Multiline = true,
                Width = 420,
                Height = 300,
                Left = 10,
                Top = 10,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom
            };
            tbInput.KeyPress += TbInput_KeyPress;
            tbInput.KeyDown += TbInput_KeyDown;
            this.Controls.Add(tbInput);

            lbWords = new ListBox
            {
                Left = 440,
                Top = 10,
                Width = 230,
                Height = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom
            };
            this.Controls.Add(lbWords);

            lblTotalKeys = new Label
            {
                Left = 10,
                Top = 320,
                Width = 420,
                Height = 30,
                Text = "Toplam tuş: 0"
            };
            this.Controls.Add(lblTotalKeys);

            btnExport = new Button
            {
                Left = 440,
                Top = 320,
                Width = 110,
                Text = "Dışa Aktar"
            };
            btnExport.Click += BtnExport_Click;
            this.Controls.Add(btnExport);

            btnClear = new Button
            {
                Left = 560,
                Top = 320,
                Width = 110,
                Text = "Temizle"
            };
            btnClear.Click += BtnClear_Click;
            this.Controls.Add(btnClear);
        }

        private void TbInput_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
            {
                totalKeyCount++;
                lblTotalKeys.Text = $"Toplam tuş: {totalKeyCount}";
                currentWord.Append(e.KeyChar);
            }
        }

        private void TbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                string word = currentWord.ToString().Trim();
                if (word.Length > 0)
                {
                    string entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | \"{word}\" | length:{word.Length}";
                    lbWords.Items.Add(entry);
                    try { File.AppendAllText(logFilePath, entry + Environment.NewLine, Encoding.UTF8); } catch { }
                }
                currentWord.Clear();
            }
            else if (e.KeyCode == Keys.Back)
            {
                if (currentWord.Length > 0)
                    currentWord.Length--;
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.FileName = "wordlog_export.txt";
                    sfd.Filter = "Text Files|*.txt|All Files|*.*";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
                        {
                            sw.WriteLine($"Export zamanı: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                            sw.WriteLine($"Toplam tuş: {totalKeyCount}");
                            sw.WriteLine();
                            sw.WriteLine("Kelime kaydı:");
                            foreach (var item in lbWords.Items)
                                sw.WriteLine(item);
                        }
                        MessageBox.Show("Dışa aktarma tamamlandı.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Dışa aktarırken hata: " + ex.Message);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Verileri temizlemek istiyor musun?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                totalKeyCount = 0;
                currentWord.Clear();
                lbWords.Items.Clear();
                lblTotalKeys.Text = "Toplam tuş: 0";
                try { if (File.Exists(logFilePath)) File.Delete(logFilePath); } catch { }
            }
        }
    }
}
