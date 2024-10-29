using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobilMS
{
    public partial class Form1 : Form
    {
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // height of ellipse
            int nHeightEllipse // width of ellipse
        );

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));
        }

        private const int CS_DropShadow = 0x00020000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = CS_DropShadow;
                return cp;
            }
        }

        SqlConnection con = new SqlConnection("Data Source=LAPTOP-DKCQVSVB;Initial Catalog=MMS;Integrated Security=True");

        private void Form1_Load(object sender, EventArgs e)
        {
            grbResetPassword.Visible = false;
            grbSecurity.Visible = false;
            txtResetpasssuname.UseSystemPasswordChar = true;
            txtPassword.UseSystemPasswordChar = true;
            btnLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnLogin.Width, btnLogin.Height, 2, 2));
            btnConfirmResetPass.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnConfirmResetPass.Width, btnConfirmResetPass.Height, 2, 2));
            btnSignin1.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSignin1.Width, btnSignin1.Height, 2, 2));
            btnConfirm.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnConfirm.Width, btnConfirm.Height, 2, 2));
            btnGologin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnGologin.Width, btnGologin.Height, 2, 2));
        }

        public Point mouseLocation;

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLocation = new Point(-e.X, -e.Y);
        }

        private void label2_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePose = Control.MousePosition;
                mousePose.Offset(mouseLocation.X, mouseLocation.Y);
                Location = mousePose;
            }
        }

        private void btnClose_MouseEnter(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.Red;
            btnClose.ForeColor = Color.White;
            btnClose.FlatAppearance.BorderSize = 0;
        }

        private void btnClose_MouseLeave(object sender, EventArgs e)
        {
            btnClose.BackColor = Color.DarkGreen;
            btnClose.ForeColor = Color.White;
            btnClose.FlatAppearance.BorderSize = 0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnLogin_MouseEnter(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.DarkGreen;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatAppearance.BorderSize = 0;
        }

        private void btnLogin_MouseLeave(object sender, EventArgs e)
        {
            btnLogin.BackColor = Color.Green;
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatAppearance.BorderSize = 0;
        }

        int p = 0;

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            getValues();
            if (username == txtUserName.Text)
            {
                p = 1;
                checkanswer();
                if (answer == txtAnswerSecurity.Text)
                {
                    
                    grbResetPassword.Visible = true;
                    grbSecurity.Visible = true;
                }
                else
                {
                    MessageBox.Show("Your answer is wrong!");
                }
            }
            else if(UserName1 == txtUserName.Text)
            {
                p = 2;
                checkanswer();
                if (answer == txtAnswerSecurity.Text)
                {
                    
                    grbResetPassword.Visible = true;
                    grbSecurity.Visible = true;
                }
                else
                {
                    MessageBox.Show("Your answer is wrong!");
                }

            }
            else
            {
                MessageBox.Show("User name is not incvluded System");
            }

            
            
        }

        string answer;

        private void checkanswer()
        {
            con.Open();
            SqlCommand cmd2 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',Answer)) from LoginTB where ID = '" + p +"'", con);
            answer = Convert.ToString(cmd2.ExecuteScalar());
            con.Close();
        }

        private void btnSignin1_Click(object sender, EventArgs e)
        {
            grbResetPassword.Visible = false;
            grbSecurity.Visible = false;
            txtResetpasssuname.Clear();
            txtResetpassspass.Clear();
            txtAnswerSecurity.Clear();
        }

        
        string password;
        string username;
        string password1;
        string UserName1;

        private void getValues()
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',UserName)) from LoginTB where ID = 1", con);
            username = Convert.ToString(cmd1.ExecuteScalar());

            SqlCommand cmd2 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',Password)) from LoginTB where ID = 1", con);
            password = Convert.ToString(cmd2.ExecuteScalar());

            SqlCommand cmd3 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',UserName)) from LoginTB where ID = 2", con);
            UserName1 = Convert.ToString(cmd3.ExecuteScalar());

            SqlCommand cmd4 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',Password)) from LoginTB where ID = 2", con);
            password1 = Convert.ToString(cmd4.ExecuteScalar());
            con.Close();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            getValues();
            if (username == txtUserName.Text)
            {
                if (password == txtPassword.Text)
                {
                    DashBoard db = new DashBoard();
                    db.Show();

                    DashBoard.internce.lblLoginMessage1.Text = "Hey, Admin";
                    DashBoard.internce.lblVisibleUser1.Visible = true;
                    DashBoard.internce.lblVisibleAdmin1.Visible = false;
                    DashBoard.internce.btnSummery1.Visible = true;

                    DashBoard.internce.lblUseringDash1.Text = "A";
                    DashBoard.internce.lblUseringDash1.BackColor = Color.DarkGreen;
                    DashBoard.internce.lblUseringDash1.ForeColor = Color.White;
                    DashBoard.internce.lblUseringDash1.FlatAppearance.BorderSize = 0;

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("UserName or Password is incorrect!");
                }
            }
            else if (UserName1 == txtUserName.Text)
            {
                if (password1 == txtPassword.Text)
                {
                    DashBoard db = new DashBoard();
                    db.Show();

                    DashBoard.internce.lblLoginMessage1.Text = "Hey, User";
                    DashBoard.internce.lblVisibleUser1.Visible = false;
                    DashBoard.internce.lblVisibleAdmin1.Visible = true;
                    DashBoard.internce.btnSummery1.Visible = false;

                    DashBoard.internce.lblUseringDash1.Text = "U";
                    DashBoard.internce.lblUseringDash1.BackColor = Color.Maroon;
                    DashBoard.internce.lblUseringDash1.ForeColor = Color.White;
                    DashBoard.internce.lblUseringDash1.FlatAppearance.BorderSize = 0;

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("UserName or Password is incorrect!");
                }
            }
            else
            {
                MessageBox.Show("UserName or Password is incorrect!");
            }

        }

        private void linForgotPass_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (txtUserName.Text != "")
            {
                grbResetPassword.Visible = false;
                grbSecurity.Visible = true;
            }
            else
            {
                MessageBox.Show("Sorry Enter UserName first!");
            }
        }

        public static bool IsValidPassword(string plainText)
        {
            Regex regex = new Regex(@"^(.{0,7}|[^0-9]*|[^A-Z])$");
            Match match = regex.Match(plainText);
            return match.Success;
        }

        private void btnConfirmResetPass_Click(object sender, EventArgs e)
        {

            if (IsValidPassword(txtResetpasssuname.Text))
            {
                if (txtResetpasssuname.Text == txtResetpassspass.Text)
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("update LoginTB set Password = ENCRYPTBYPASSPHRASE('8', '" + txtResetpasssuname.Text + "') where ID = '" + p + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Password is completely Updated!");
                    con.Close();

                    grbResetPassword.Visible = false;
                    grbSecurity.Visible = false;
                    txtResetpasssuname.Clear();
                    txtResetpassspass.Clear();
                    txtAnswerSecurity.Clear();
                }
                else
                {
                    MessageBox.Show("Sorry Your Passswors not Matched!");
                }
            }
            else
            {
                MessageBox.Show("Sorry Your password is not valied!");
            }
            
        }

        private void btnConfirmResetPass_MouseEnter(object sender, EventArgs e)
        {
            btnConfirmResetPass.BackColor = Color.DarkGreen;
            btnConfirmResetPass.ForeColor = Color.White;
            btnConfirmResetPass.FlatAppearance.BorderSize = 0;
        }

        private void btnConfirmResetPass_MouseLeave(object sender, EventArgs e)
        {
            btnConfirmResetPass.BackColor = Color.Gainsboro;
            btnConfirmResetPass.ForeColor = Color.Black;
            btnConfirmResetPass.FlatAppearance.BorderSize = 0;
        }

        private void btnSignin1_MouseEnter(object sender, EventArgs e)
        {
            btnSignin1.BackColor = Color.Black;
            btnSignin1.ForeColor = Color.White;
            btnSignin1.FlatAppearance.BorderSize = 0;
        }

        private void btnSignin1_MouseLeave(object sender, EventArgs e)
        {
            btnSignin1.BackColor = Color.Gainsboro;
            btnSignin1.ForeColor = Color.Black;
            btnSignin1.FlatAppearance.BorderSize = 0;
        }

        private void txtResetpasssuname_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(IsValidPassword(txtResetpasssuname.Text))
            {
                lblMessagep.ForeColor = Color.Green;
                lblMessagep.Text = "Valied";
            }
            else
            {
                lblMessagep.ForeColor = Color.DarkRed;
                lblMessagep.Text = "Not Valied";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtResetpasssuname.UseSystemPasswordChar = false;
            }
            else
            {
                txtResetpasssuname.UseSystemPasswordChar = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                txtPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
            }
        }

        private void btnGologin_MouseEnter(object sender, EventArgs e)
        {
            btnGologin.BackColor = Color.Black;
            btnGologin.ForeColor = Color.White;
            btnGologin.FlatAppearance.BorderSize = 0;
        }

        private void btnGologin_MouseLeave(object sender, EventArgs e)
        {
            btnGologin.BackColor = Color.Gainsboro;
            btnGologin.ForeColor = Color.Black;
            btnGologin.FlatAppearance.BorderSize = 0;
        }

        private void btnGologin_Click(object sender, EventArgs e)
        {
            grbResetPassword.Visible = false;
            grbSecurity.Visible = false;
            txtAnswerSecurity.Clear();
            txtPassword.Clear();
            txtResetpasssuname.Clear();
            txtResetpassspass.Clear();
        }

        private void btnConfirm_MouseEnter(object sender, EventArgs e)
        {
            btnConfirm.BackColor = Color.DarkGreen;
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatAppearance.BorderSize = 0;
        }

        private void btnConfirm_MouseLeave(object sender, EventArgs e)
        {
            btnConfirm.BackColor = Color.Gainsboro;
            btnConfirm.ForeColor = Color.Black;
            btnConfirm.FlatAppearance.BorderSize = 0;
        }
    }
}
