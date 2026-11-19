using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace oyutnii_web
{
    public class Form1 : Form
    {
        class Student
        {
            public string Code = "", Name = "", Phone = "", Dept = "", Mode = "", Course = "";
            public string Subjects = "", Languages = "";
            public int Age;
            public DateTime Birth;
            public bool Scholarship;
            public Image? Photo;
        }

        readonly List<Student> students = new List<Student>();
        Image? currentPhoto;

        // ---- Control-ууд ----
        readonly TextBox txtCode = new TextBox { Location = new Point(31, 25), Size = new Size(160, 23), PlaceholderText = "Оюутны код" };
        readonly TextBox txtName = new TextBox { Location = new Point(237, 25), Size = new Size(158, 23), PlaceholderText = "Овог нэр" };
        readonly Label lblTotal = new Label { AutoSize = true, Location = new Point(453, 29), Text = "Нийт: 0" };
        readonly LinkLabel lnkClear = new LinkLabel { AutoSize = true, Location = new Point(578, 29), Text = "Цэвэрлэх" };
        readonly CheckBox chkScholarship = new CheckBox { AutoSize = true, Location = new Point(31, 85), Text = "тэтгэлэгтэй" };
        readonly RadioButton rbDay = new RadioButton { AutoSize = true, Checked = true, Location = new Point(31, 116), Text = "өдрийн сургалт" };
        readonly RadioButton rbEvening = new RadioButton { AutoSize = true, Location = new Point(31, 147), Text = "оройн сургалт" };
        readonly CheckedListBox clbCourses = new CheckedListBox { CheckOnClick = true, Location = new Point(31, 184), Size = new Size(182, 112) };
        readonly NumericUpDown numAge = new NumericUpDown { Minimum = 15, Maximum = 70, Value = 20, Location = new Point(31, 356), Size = new Size(150, 23) };
        readonly ComboBox cmbCourse = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(31, 403), Size = new Size(150, 23) };
        readonly TextBox txtPhone = new TextBox { MaxLength = 8, Location = new Point(31, 452), Size = new Size(150, 23), PlaceholderText = "утас (8 орон)" };
        readonly DateTimePicker dtpBirth = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(31, 506), Size = new Size(150, 23) };
        readonly Button btnRegister = new Button { Location = new Point(31, 569), Size = new Size(148, 55), Text = "Бүртгэх" };
        readonly ListBox lstLang = new ListBox { SelectionMode = SelectionMode.MultiSimple, Location = new Point(269, 82), Size = new Size(163, 142) };
        readonly PictureBox picPhoto = new PictureBox { BorderStyle = BorderStyle.FixedSingle, Cursor = Cursors.Hand, SizeMode = PictureBoxSizeMode.Zoom, Location = new Point(269, 258), Size = new Size(163, 198) };
        readonly DataGridView dgv = new DataGridView { Location = new Point(449, 82), Size = new Size(299, 187) };
        readonly TreeView tvDept = new TreeView { Location = new Point(449, 309), Size = new Size(299, 120) };
        readonly Label lblInfo = new Label { AutoSize = false, BorderStyle = BorderStyle.FixedSingle, Font = new Font("Segoe UI", 10F), Location = new Point(449, 481), Size = new Size(299, 219), Padding = new Padding(6) };

        public Form1()
        {
            Text = "Оюутны бүртгэл";
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(790, 730);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            Controls.AddRange(new Control[]
            {
                txtCode, txtName, lblTotal, lnkClear, chkScholarship, rbDay, rbEvening,
                clbCourses, numAge, cmbCourse, txtPhone, dtpBirth, btnRegister,
                lstLang, picPhoto, dgv, tvDept, lblInfo
            });

            clbCourses.Items.AddRange(new object[] { "Программчлал", "Өгөгдлийн бүтэц", "Биеийн тамир", "Монгол хэл" });
            lstLang.Items.AddRange(new object[] { "Монгол", "Англи", "Орос", "Хятад", "Солонгос", "Япон" });
            cmbCourse.Items.AddRange(new object[] { "1-р курс", "2-р курс", "3-р курс", "4-р курс" });
            cmbCourse.SelectedIndex = 0;

            // Хүснэгт
            dgv.Columns.Add("colCode", "Код");
            dgv.Columns.Add("colName", "Овог нэр");
            dgv.Columns.Add("colPhone", "Утас");
            dgv.Columns.Add("colAge", "Нас");
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Тэнхимийн мод
            tvDept.Nodes.Add("Компьютерийн ухаан");
            tvDept.Nodes.Add("Мэдээллийн технологи");
            tvDept.Nodes.Add("Программ хангамж");
            var nat = tvDept.Nodes.Add("Байгалийн ухааны сургууль");
            nat.Nodes.Add("Математик");
            nat.Nodes.Add("Физик");
            tvDept.ExpandAll();

            // Үйлдлүүд
            btnRegister.Click += (s, e) => Register();
            lnkClear.LinkClicked += (s, e) => ClearForm();
            picPhoto.Click += (s, e) => ChoosePhoto();
            dgv.SelectionChanged += (s, e) => ShowSelected();

            lblInfo.Text = "Оюутны мэдээлэл энд харагдана." + Environment.NewLine +
                           "Зураг сонгохдоо зургийн талбар дээр дарна.";
        }

        void Register()
        {
            string code = txtCode.Text.Trim();
            string name = txtName.Text.Trim();
            string phone = txtPhone.Text.Trim();

            if (code.Length == 0) { Warn("Оюутны кодоо бичнэ үү.", txtCode); return; }
            if (name.Length == 0) { Warn("Овог нэрээ бичнэ үү.", txtName); return; }
            if (phone.Length != 8 || !phone.All(char.IsDigit)) { Warn("Утасны дугаар 8 оронтой тоо байх ёстой.", txtPhone); return; }
            if (tvDept.SelectedNode == null) { Warn("Тэнхимээ мод дотроос сонгоно уу.", tvDept); return; }
            if (students.Any(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
            {
                Warn("Энэ кодтой оюутан бүртгэгдсэн байна.", txtCode);
                return;
            }

            var st = new Student
            {
                Code = code,
                Name = name,
                Phone = phone,
                Age = (int)numAge.Value,
                Birth = dtpBirth.Value.Date,
                Mode = rbDay.Checked ? "Өдрийн" : "Оройн",
                Course = cmbCourse.Text,
                Dept = tvDept.SelectedNode.Text,
                Scholarship = chkScholarship.Checked,
                Subjects = Join(clbCourses.CheckedItems.Cast<object>()),
                Languages = Join(lstLang.SelectedItems.Cast<object>()),
                Photo = currentPhoto
            };
            currentPhoto = null;

            students.Add(st);
            dgv.Rows.Add(st.Code, st.Name, st.Phone, st.Age);

            ClearForm();
            int last = dgv.Rows.Count - 1;
            dgv.ClearSelection();
            dgv.Rows[last].Selected = true;
            dgv.CurrentCell = dgv.Rows[last].Cells[0];
            ShowStudent(st);
            lblTotal.Text = "Нийт: " + students.Count;
        }

        static string Join(IEnumerable<object> items)
        {
            string s = string.Join(", ", items);
            return s.Length == 0 ? "-" : s;
        }

        void Warn(string message, Control focus)
        {
            MessageBox.Show(message, "Анхаар", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            focus.Focus();
        }

        void ClearForm()
        {
            txtCode.Clear();
            txtName.Clear();
            txtPhone.Clear();
            chkScholarship.Checked = false;
            rbDay.Checked = true;
            numAge.Value = 20;
            cmbCourse.SelectedIndex = 0;
            dtpBirth.Value = DateTime.Today;
            for (int i = 0; i < clbCourses.Items.Count; i++) clbCourses.SetItemChecked(i, false);
            lstLang.ClearSelected();
            tvDept.SelectedNode = null;
            currentPhoto?.Dispose();
            currentPhoto = null;
            picPhoto.Image = null;
            txtCode.Focus();
        }

        void ChoosePhoto()
        {
            using var dlg = new OpenFileDialog { Filter = "Зураг|*.png;*.jpg;*.jpeg;*.bmp" };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            using var tmp = Image.FromFile(dlg.FileName);
            currentPhoto?.Dispose();
            currentPhoto = new Bitmap(tmp);
            picPhoto.Image = currentPhoto;
        }

        void ShowSelected()
        {
            if (dgv.CurrentRow == null) return;
            int i = dgv.CurrentRow.Index;
            if (i >= 0 && i < students.Count) ShowStudent(students[i]);
        }

        void ShowStudent(Student st)
        {
            picPhoto.Image = st.Photo;
            lblInfo.Text = string.Join(Environment.NewLine, new[]
            {
                "Оюутны бүртгэл",
                "Код: " + st.Code,
                "Нэр: " + st.Name,
                "Утас: " + st.Phone,
                "Нас: " + st.Age,
                "Төрсөн өдөр: " + st.Birth.ToString("yyyy-MM-dd"),
                "Сургалт: " + st.Mode + ", " + st.Course,
                "Тэнхим: " + st.Dept,
                "Тэтгэлэг: " + (st.Scholarship ? "Тийм" : "Үгүй"),
                "Хичээл: " + st.Subjects,
                "Хэл: " + st.Languages
            });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                currentPhoto?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}