using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Drawing.Printing;

namespace MobilMS
{
    public partial class DashBoard : Form
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

        public static DashBoard internce;
        public Button lblUseringDash1;
        public Label lblVisibleAdmin1;
        public Label lblVisibleUser1;
        public Label lblLoginMessage1;
        public Button btnSummery1;

        public DashBoard()
        {
            InitializeComponent();
            internce = this;
            lblUseringDash1 = btnAccount;
            lblVisibleAdmin1 = lblVisibleAdmin;
            lblVisibleUser1 = lblVisibleUser;
            lblLoginMessage1 = lblLoginMessage;
            btnSummery1 = btnSummery;
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 6, 6));

            printDocument1.PrintPage += new PrintPageEventHandler(printdoc1_PrintPage);

            cleardataDay();
        }

        private void cleardataDay()
        {
            con.Open();
            SqlCommand cmd2 = new SqlCommand("DELETE FROM InvoiceTodayData WHERE NowDate < GETDATE() - 1", con);
            cmd2.ExecuteNonQuery();

            SqlCommand cmd3 = new SqlCommand("DELETE FROM TodayCustomer WHERE NowDate < GETDATE() - 1", con);
            cmd3.ExecuteNonQuery();
            con.Close();
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

        private void btnSupplier_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = true;
            lblSelProduct.Visible = false;
            grbCustomer.Visible = false;
            grbInvoice.Visible = false;
            grbSummery.Visible = false;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;

            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            getMaxSuppierId();


            con.Open();
            SqlCommand cmd3 = new SqlCommand("Select SupplierTB.SupplierID, SupplierTB.SuplierName, SupplierTB.CompanyName, SupplierTB.ContactMobile, SupplierTB.ContactLand, SupplierTB.AddressNo, SupplierTB.AdddressLine, SupplierTB.AddressCity from SupplierTB order by 1 asc", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd3);
            DataTable dt = new DataTable();
            da.Fill(dt);
            datagridSupplierList.DataSource = dt;
            con.Close();
        }

        private void btnCustomer_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = false;
            grbCustomer.Visible = true;
            grbInvoice.Visible = false;
            grbSummery.Visible = false;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;
            cmbCustoemrBrand.Text = "";
            cmbCustomerSeries.Text = "";


            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ProductAddTB ", con);
                cmd.ExecuteNonQuery();

                SqlCommand cmd5 = new SqlCommand("Select ProductAddTB.ID, ProductAddTB.BillID, ProductAddTB.Brand, ProductAddTB.Series , ProductAddTB.Qty, ProductAddTB.TotPrice from ProductAddTB order by 1 asc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgwAddProducts.DataSource = dt;


            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }

            customerProductBrand();
            getMaxProductMIDId();
        }


        private void clearDetails()
        {
            txtCustomerName.Clear();
            txtCustomerNic.Clear();
            cmbCustoemrBrand.Text = "";
            cmbCustomerSeries.Text = "";
            txtCustomerMobile.Clear();
            txtCustomerLand.Clear();
            txtCustomerDiscount.Clear();
            txtCustomerPayment.Clear();
            txtCustomerQTY.Text = "1";
            lblCustomerSubTotal.Text = "";
            lblCustomerTotal.Text = "";
            lblCustomerBalance.Text = "";
        }

        private void CustomerColor()
        {
            if(grbCustomer.Visible == true)
            {
                btnCustomer.BackColor = Color.DarkGreen;
                btnCustomer.ForeColor = Color.White;
                btnCustomer.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnCustomer.BackColor = Color.Gainsboro;
                btnCustomer.ForeColor = Color.Black;
                btnCustomer.FlatAppearance.BorderSize = 0;
            }
        }

        private void SupplierColor()
        {
            if (grbSupplier.Visible == true)
            {
                btnSupplier.BackColor = Color.DarkGreen;
                btnSupplier.ForeColor = Color.White;
                btnSupplier.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnSupplier.BackColor = Color.Gainsboro;
                btnSupplier.ForeColor = Color.Black;
                btnSupplier.FlatAppearance.BorderSize = 0;
            }
        }

        private void invoiceColor()
        {
            if (grbInvoice.Visible == true)
            {
                btnInvoice.BackColor = Color.DarkGreen;
                btnInvoice.ForeColor = Color.White;
                btnInvoice.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnInvoice.BackColor = Color.Gainsboro;
                btnInvoice.ForeColor = Color.Black;
                btnInvoice.FlatAppearance.BorderSize = 0;
            }
        }

        private void HistoryColor()
        {
            if (grbHistory.Visible == true)
            {
                btnHistory.BackColor = Color.DarkGreen;
                btnHistory.ForeColor = Color.White;
                btnHistory.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnHistory.BackColor = Color.Gainsboro;
                btnHistory.ForeColor = Color.Black;
                btnHistory.FlatAppearance.BorderSize = 0;
            }
        }

        private void SummeruColor()
        {
            if (grbSummery.Visible == true)
            {
                btnSummery.BackColor = Color.DarkGreen;
                btnSummery.ForeColor = Color.White;
                btnSummery.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnSummery.BackColor = Color.Gainsboro;
                btnSummery.ForeColor = Color.Black;
                btnSummery.FlatAppearance.BorderSize = 0;
            }
        }

        private void ProductColor()
        {
            if (grbProduct.Visible == true)
            {
                btnProduct.BackColor = Color.DarkGreen;
                btnProduct.ForeColor = Color.White;
                btnProduct.FlatAppearance.BorderSize = 0;
            }
            else
            {
                btnProduct.BackColor = Color.Gainsboro;
                btnProduct.ForeColor = Color.Black;
                btnProduct.FlatAppearance.BorderSize = 0;
            }
        }

        private void hide()
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = false;
            grbCustomer.Visible = false;
            lblSelProduct.Visible = false;
            grbInvoice.Visible = false;
            grbSummery.Visible = false;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbLogindatabase.Visible = false;
            grbPrintinvoice.Visible = false;
        }

        double appluzero = 0;

        string Asid;

        int ie;

        public void getMaxIDonbacup()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM AddSeriesTB WHERE ID = (SELECT MAX(ID) FROM AddSeriesTB) ", con);
            ie = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            ie++;
            Asid = appluzero + ie.ToString();
        }

        double appluzero1 = 0;

        string Asid1;

        public void getMaxSuppierId()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM SupplierTB WHERE ID = (SELECT MAX(ID) FROM SupplierTB) ", con);
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            i++;
            Asid1 = appluzero1 + i.ToString();
            txtSuppilerSuppilerId.Text = "SUP" + Asid1;
        }

        double appluzero2 = 0;

        string Asid2;

        public void getMaxProductId()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProductTB WHERE ID = (SELECT MAX(ID) FROM ProductTB) ", con);
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            i++;
            Asid2 = appluzero2 + i.ToString();
            txtProductProductID.Text = "POD" + Asid2;
        }

        private void btnProduct_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = true;
            grbSupplier.Visible = false;
            grbCustomer.Visible = false;
            grbSummery.Visible = false;
            grbInvoice.Visible = false;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;

            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            combosel1();
            getMaxProductId();
        }

        private void DashBoard_Load(object sender, EventArgs e)
        {
            cornerradious();
            
            hide();
            combosel1();
            comboboxofCustomer();
            getMaxProductMIDId();
            countprofit();
            txtCustomerQTY.Text = "1";
            txtUpPassword.UseSystemPasswordChar = true;

        }

        private void cornerradious()
        {
            btnAccount.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnAccount.Width, btnAccount.Height, 2, 2));
            lblVisibleUser.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, lblVisibleUser.Width, lblVisibleUser.Height, 2, 2));
            lblVisibleAdmin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, lblVisibleAdmin.Width, lblVisibleAdmin.Height, 2, 2));
            btnCustomer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnCustomer.Width, btnCustomer.Height, 2, 2));
            btnSupplier.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplier.Width, btnSupplier.Height, 2, 2));
            btnProduct.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnProduct.Width, btnProduct.Height, 2, 2));
            btnInvoice.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnInvoice.Width, btnInvoice.Height, 2, 2));
            btnHistory.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnHistory.Width, btnHistory.Height, 2, 2));
            btnSummery.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSummery.Width, btnSummery.Height, 2, 2));
            ntnCustomerAdd.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, ntnCustomerAdd.Width, ntnCustomerAdd.Height, 2, 2));
            label43.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, label43.Width, label43.Height, 2, 2));
            btnClearonAddtb.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnClearonAddtb.Width, btnClearonAddtb.Height, 2, 2));
            btnListProductadd.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnListProductadd.Width, btnListProductadd.Height, 2, 2));
            txtClearCustomerDetails.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, txtClearCustomerDetails.Width, txtClearCustomerDetails.Height, 2, 2));
            label13.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, label13.Width, label13.Height, 2, 2));
            btnSupplierUpdate.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplierUpdate.Width, btnSupplierUpdate.Height, 2, 2));
            btnSupplierDelete.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplierDelete.Width, btnSupplierDelete.Height, 2, 2));
            btnSupplierAdd.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplierAdd.Width, btnSupplierAdd.Height, 2, 2));
            btnSupplierProductSeriesAdd.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplierProductSeriesAdd.Width, btnSupplierProductSeriesAdd.Height, 2, 2));
            btnSupplierProductSeriesDelete.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSupplierProductSeriesDelete.Width, btnSupplierProductSeriesDelete.Height, 2, 2));
            label23.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, label23.Width, label23.Height, 2, 2));
            btnProductUpdate.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnProductUpdate.Width, btnProductUpdate.Height, 2, 2));
            btnProductDelete.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnProductDelete.Width, btnProductDelete.Height, 2, 2));
            btnProductAdd.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnProductAdd.Width, btnProductAdd.Height, 2, 2));
            label35.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, label35.Width, label35.Height, 2, 2));
            label48.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, label48.Width, label48.Height, 2, 2));
            btnSecurityLogin.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnSecurityLogin.Width, btnSecurityLogin.Height, 2, 2));
            btnAdminClick.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnAdminClick.Width, btnAdminClick.Height, 2, 2));
            btnUserClick.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnUserClick.Width, btnUserClick.Height, 2, 2));
            btnApplySecurityDetails.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnApplySecurityDetails.Width, btnApplySecurityDetails.Height, 2, 2));
            lblAdminAcountlable.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, lblAdminAcountlable.Width, lblAdminAcountlable.Height, 2, 2));
            btnPrintInvoice.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, btnPrintInvoice.Width, btnPrintInvoice.Height, 2, 2));
        }


        private void countprofit()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select SUM(Total) from InvoiceTodayData", con);
                float x = Convert.ToInt32(cmd.ExecuteScalar());

                SqlCommand cmd1 = new SqlCommand("Select COUNT(ID) from TodayCustomer", con);
                int y = Convert.ToInt32(cmd1.ExecuteScalar());

                lblProfit.Text = Convert.ToString(x);
                lblTodaycustomers.Text = Convert.ToString(y);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }

            
            
        }

        private void btnSupplierProductSeriesAdd_Click(object sender, EventArgs e)
        {
            getMaxIDonbacup();
            productSeriesSel();
            if (selection == txtSupplierProductSeries.Text)
            {
                MessageBox.Show("Sorry Your Order Is Cant Added");
                return;
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into AddSeriesTB values(@ID,@ProductBrand)", con);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(Asid));
                    cmd.Parameters.AddWithValue("@ProductBrand", txtSupplierProductSeries.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Add Successfull!");
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    comboboxofCustomer();
                    txtSupplierProductSeries.Clear();
                }
            }


            
        }

        private void btnSupplierProductSeriesDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM AddSeriesTB WHERE ProductBrand = '" + txtSupplierProductSeries.Text + "'", con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Delete Successfull!");
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
                comboboxofCustomer();
                txtSupplierProductSeries.Clear();
            }
        }

        private void btnSupplierAdd_Click(object sender, EventArgs e)
        {
            if (txtSuppilerSuppilerId.Text == "" || txtSuplierName.Text == "" || txtSuppierCompanyName.Text == "" || txtSupplierMobile.Text == "" || txtSupplierLand.Text == "" || txtSupplierNo.Text == "" || txtSupplierADline.Text == "" || txtSupplierCity.Text == "" )
            {
                MessageBox.Show("Please Enter All Values");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into SupplierTB values(@ID,@SupplierID, @SuplierName, @CompanyName,@ContactMobile,@ContactLand,@AddressNo,@AdddressLine,@AddressCity)", con);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(Asid1));
                    cmd.Parameters.AddWithValue("@SupplierID", txtSuppilerSuppilerId.Text);
                    cmd.Parameters.AddWithValue("@SuplierName", txtSuplierName.Text);
                    cmd.Parameters.AddWithValue("@CompanyName", txtSuppierCompanyName.Text);
                    cmd.Parameters.AddWithValue("@ContactMobile", int.Parse(txtSupplierMobile.Text));
                    cmd.Parameters.AddWithValue("@ContactLand", int.Parse(txtSupplierLand.Text));
                    cmd.Parameters.AddWithValue("@AddressNo", txtSupplierNo.Text);
                    cmd.Parameters.AddWithValue("@AdddressLine", txtSupplierADline.Text);
                    cmd.Parameters.AddWithValue("@AddressCity", txtSupplierCity.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Add Successfull!");

                    SqlCommand cmd3 = new SqlCommand("Select SupplierTB.SupplierID,SupplierTB.SuplierName, SupplierTB.CompanyName, SupplierTB.ContactMobile, SupplierTB.ContactLand, SupplierTB.AddressNo, SupplierTB.AdddressLine, SupplierTB.AddressCity from SupplierTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagridSupplierList.DataSource = dt;

                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    getMaxSuppierId();
                }
            }
           
            
        }

        private void btnSupplierUpdate_Click(object sender, EventArgs e)
        {
            if (txtSuppilerSuppilerId.Text == "" || txtSuplierName.Text == "" || txtSuppierCompanyName.Text == "" || txtSupplierMobile.Text == "" || txtSupplierLand.Text == "" || txtSupplierNo.Text == "" || txtSupplierADline.Text == "" || txtSupplierCity.Text == "")
            {
                MessageBox.Show("Please Enter All Values");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("update SupplierTB set SuplierName= @SuplierName, CompanyName= @CompanyName, ContactMobile= @ContactMobile, ContactLand= @ContactLand, AddressNo= @AddressNo, AdddressLine= @AdddressLine, AddressCity= @AddressCity where SupplierID= '" + txtSuppilerSuppilerId.Text + "'", con);
                    cmd.Parameters.AddWithValue("@SupplierID", txtSuppilerSuppilerId.Text);
                    cmd.Parameters.AddWithValue("@SuplierName", txtSuplierName.Text);
                    cmd.Parameters.AddWithValue("@CompanyName", txtSuppierCompanyName.Text);
                    cmd.Parameters.AddWithValue("@ContactMobile", int.Parse(txtSupplierMobile.Text));
                    cmd.Parameters.AddWithValue("@ContactLand", int.Parse(txtSupplierLand.Text));
                    cmd.Parameters.AddWithValue("@AddressNo", txtSupplierNo.Text);
                    cmd.Parameters.AddWithValue("@AdddressLine", txtSupplierADline.Text);
                    cmd.Parameters.AddWithValue("@AddressCity", txtSupplierCity.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update Successfull!");

                    SqlCommand cmd3 = new SqlCommand("Select SupplierTB.SupplierID, SupplierTB.SuplierName, SupplierTB.CompanyName, SupplierTB.ContactMobile, SupplierTB.ContactLand, SupplierTB.AddressNo, SupplierTB.AdddressLine, SupplierTB.AddressCity from SupplierTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagridSupplierList.DataSource = dt;

                    
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    getMaxSuppierId();
                }
            }

            
        }

        private void btnSupplierDelete_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM SupplierTB WHERE SupplierID = '" + txtSuppilerSuppilerId.Text + "'", con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Delete Successfull!");

                SqlCommand cmd3 = new SqlCommand("Select SupplierTB.SupplierID, SupplierTB.SuplierName, SupplierTB.CompanyName, SupplierTB.ContactMobile, SupplierTB.ContactLand, SupplierTB.AddressNo, SupplierTB.AdddressLine, SupplierTB.AddressCity from SupplierTB order by 1 asc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd3);
                DataTable dt = new DataTable();
                da.Fill(dt);
                datagridSupplierList.DataSource = dt;
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
                getMaxSuppierId();
            }
            
        }

        private void combosel1()
        {
            try
            {
                con.Open();
                SqlCommand cmd3 = new SqlCommand("Select ProductTB.PID, ProductTB.Name, ProductTB.PBrand, ProductTB.SName, ProductTB.SN, ProductTB.Warrenty, ProductTB.Units, ProductTB.UnitPrice from ProductTB order by 1 asc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd3);
                DataTable dt = new DataTable();
                da.Fill(dt);
                datagirdProducts.DataSource = dt;

                SqlCommand cmd = new SqlCommand("Select SuplierName from SupplierTB order by 1 asc", con);
                SqlDataAdapter da1 = new SqlDataAdapter();
                da1.SelectCommand = cmd;
                DataTable table1 = new DataTable();
                da1.Fill(table1);
                cmbProductSuppilerName.DataSource = table1;
                cmbProductSuppilerName.DisplayMember = "SuplierName";
                cmbProductSuppilerName.ValueMember = "SuplierName";
                dataGridViewSuppliersList.DataSource = table1;

                SqlCommand cmd1 = new SqlCommand("Select ProductBrand from AddSeriesTB order by 1 asc", con);
                SqlDataAdapter da2 = new SqlDataAdapter();
                da2.SelectCommand = cmd1;
                DataTable table2 = new DataTable();
                da2.Fill(table2);
                cmbProductSeries.DataSource = table2;
                cmbProductSeries.DisplayMember = "ProductBrand";
                cmbProductSeries.ValueMember = "ProductBrand";
                dataGridViewProductBrands.DataSource = table2;
                con.Close();
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void btnProductAdd_Click(object sender, EventArgs e)
        {
            if (txtProductProductID.Text == "" || txtProductname.Text == "" || cmbProductSeries.Text == "" || cmbProductSuppilerName.Text == "" || txtWarrenty.Text == "" || txtProductUnits.Text == "" || txtProductUnitPrice.Text == "" )
            {
                MessageBox.Show("Please Enter All Values");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into ProductTB values(@ID,@PID, @Name, @PBrand,@SName,@SN, @Warrenty,@Units,@UnitPrice)", con);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(Asid2));
                    cmd.Parameters.AddWithValue("@PID", txtProductProductID.Text);
                    cmd.Parameters.AddWithValue("@Name", txtProductname.Text);
                    cmd.Parameters.AddWithValue("@PBrand", cmbProductSeries.Text);
                    cmd.Parameters.AddWithValue("@SName", cmbProductSuppilerName.Text);
                    cmd.Parameters.AddWithValue("@SN", txtProductSN.Text);
                    cmd.Parameters.AddWithValue("@Warrenty", int.Parse(txtWarrenty.Text));
                    cmd.Parameters.AddWithValue("@Units", int.Parse(txtProductUnits.Text));
                    cmd.Parameters.AddWithValue("@UnitPrice", float.Parse(txtProductUnitPrice.Text));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Add Successfull!");

                    SqlCommand cmd3 = new SqlCommand("Select ProductTB.PID, ProductTB.Name, ProductTB.PBrand, ProductTB.SName, ProductTB.SN, ProductTB.Warrenty, ProductTB.Units, ProductTB.UnitPrice from ProductTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagirdProducts.DataSource = dt;

                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    getMaxProductId();
                }
            }

            
        }

        private void btnProductUpdate_Click(object sender, EventArgs e)
        {
            if (txtProductProductID.Text == "" || txtProductname.Text == "" || cmbProductSeries.Text == "" || cmbProductSuppilerName.Text == "" || txtWarrenty.Text == "" || txtProductUnits.Text == "" || txtProductUnitPrice.Text == "")
            {
                MessageBox.Show("Please Enter All Values");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("update ProductTB set Name= @Name, PBrand= @PBrand, SName= @SName, SN= @SN,Warrenty= @Warrenty, Units= @Units, UnitPrice= @UnitPrice where PID= '" + txtProductProductID.Text + "'", con);
                    cmd.Parameters.AddWithValue("@PID", txtProductProductID.Text);
                    cmd.Parameters.AddWithValue("@Name", txtProductname.Text);
                    cmd.Parameters.AddWithValue("@PBrand", cmbProductSeries.Text);
                    cmd.Parameters.AddWithValue("@SName", cmbProductSuppilerName.Text);
                    cmd.Parameters.AddWithValue("@SN", txtProductSN.Text);
                    cmd.Parameters.AddWithValue("@Warrenty", int.Parse(txtWarrenty.Text));
                    cmd.Parameters.AddWithValue("@Units", int.Parse(txtProductUnits.Text));
                    cmd.Parameters.AddWithValue("@UnitPrice", float.Parse(txtProductUnitPrice.Text));
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Update Successfull!");

                    SqlCommand cmd3 = new SqlCommand("Select ProductTB.PID, ProductTB.Name, ProductTB.PBrand, ProductTB.SName, ProductTB.SN, ProductTB.Warrenty, ProductTB.Units, ProductTB.UnitPrice from ProductTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagirdProducts.DataSource = dt;

                    
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    getMaxProductId();
                }
            }
            
        }

        private void btnProductDelete_Click(object sender, EventArgs e)
        {
            if (txtProductProductID.Text == "")
            {
                MessageBox.Show("Please Enter Proiduct ID");
            }
            else
            {
                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("DELETE FROM ProductTB WHERE PID = '" + txtProductProductID.Text + "'", con);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Delete Successfull!");

                    SqlCommand cmd3 = new SqlCommand("Select ProductTB.PID, ProductTB.Name, ProductTB.PBrand, ProductTB.SName, ProductTB.SN, ProductTB.Warrenty, ProductTB.Units, ProductTB.UnitPrice from ProductTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    datagirdProducts.DataSource = dt;
                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    getMaxProductId();
                }
            }
            
        }

        string selection;

        private void productSeriesSel()
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("select * from AddSeriesTB where ProductBrand ='" + txtSupplierProductSeries.Text + "'", con);
            SqlDataReader srd1 = cmd1.ExecuteReader();
            while (srd1.Read())
            {
                selection = srd1.GetValue(1).ToString();
            }
            con.Close();
        }

        private void txtSupplierProductSeries_TextChanged(object sender, EventArgs e)
        {
            if (txtSupplierProductSeries.Text == "")
            {
                lblSelProduct.Visible = false;
                return;
            }
            else
            {
                productSeriesSel();
                if (selection == txtSupplierProductSeries.Text)
                {
                    lblSelProduct.Text = "Product is Always Added";
                    lblSelProduct.Visible = true;
                }
                else
                {
                    lblSelProduct.Text = "Ok! You Can Add";
                    lblSelProduct.Visible = true;
                }
            }
            
        }

        private void txtSupplierMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtSupplierLand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCustomerSearch_TextChanged(object sender, EventArgs e)
        {
            
            
        }

        private void txtCustomerSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void comboboxofCustomer()
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("Select ProductBrand from AddSeriesTB order by 1 asc", con);
            SqlDataAdapter da2 = new SqlDataAdapter();
            da2.SelectCommand = cmd1;
            DataTable table2 = new DataTable();
            da2.Fill(table2);
            cmbCustoemrBrand.DataSource = table2;
            cmbCustoemrBrand.DisplayMember = "ProductBrand";
            cmbCustoemrBrand.ValueMember = "ProductBrand";
            con.Close();
        }

        double appluzero3 = 0;

        string Asid3;

        string midID;

        string UnitPrice;

        public void getMaxProductMIDId()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM CustomerTB WHERE ID = (SELECT MAX(ID) FROM CustomerTB) ", con);
            int i = Convert.ToInt32(cmd.ExecuteScalar());
            con.Close();
            i++;
            Asid3 = appluzero3 + i.ToString();
            txtCustomerId.Text = midID = "CUS" + Asid3;
        }

        private void btnListProductadd_Click(object sender, EventArgs e)
        {
            selected();
            if (product1 == cmbCustomerSeries.Text)
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select * from ProductTB where Name ='" + cmbCustomerSeries.Text + "'", con);
                SqlDataReader srd1 = cmd1.ExecuteReader();
                while (srd1.Read())
                {
                    UnitPrice = srd1.GetValue(8).ToString();
                }
                con.Close();

                float x = Convert.ToInt32(txtCustomerQTY.Text);
                float y = Convert.ToInt32(UnitPrice);
                float U = x * y;

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("update ProductAddTB set ID= @ID,BillID= @BillID, Brand= @Brand, Qty= @Qty, TotPrice= @TotPrice where Series= @Series ", con);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                    cmd.Parameters.AddWithValue("@BillID", midID);
                    cmd.Parameters.AddWithValue("@Brand", cmbCustoemrBrand.Text);
                    cmd.Parameters.AddWithValue("@Series", cmbCustomerSeries.Text);
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(txtCustomerQTY.Text));
                    cmd.Parameters.AddWithValue("@TotPrice", U);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    con.Open();
                    SqlCommand cmd7 = new SqlCommand("select SUM(TotPrice) from ProductAddTB", con);
                    double ke = Convert.ToInt32(cmd7.ExecuteScalar());
                    con.Close();
                    lblCustomerSubTotal.Text = ke.ToString();

                    con.Open();
                    SqlCommand cmd3 = new SqlCommand("Select ProductAddTB.BillID, ProductAddTB.Brand, ProductAddTB.Series , ProductAddTB.Qty, ProductAddTB.TotPrice from ProductAddTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgwAddProducts.DataSource = dt;
                    con.Close();
                }
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select * from ProductTB where Name ='" + cmbCustomerSeries.Text + "'", con);
                SqlDataReader srd1 = cmd1.ExecuteReader();
                while (srd1.Read())
                {
                    UnitPrice = srd1.GetValue(8).ToString();
                }
                con.Close();

                float x = Convert.ToInt32(txtCustomerQTY.Text);
                float y = Convert.ToInt32(UnitPrice);
                float U = x * y;

                try
                {
                    con.Open();
                    SqlCommand cmd = new SqlCommand("insert into ProductAddTB values(@ID,@BillID, @Brand, @Series, @Qty, @TotPrice)", con);
                    cmd.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                    cmd.Parameters.AddWithValue("@BillID", midID);
                    cmd.Parameters.AddWithValue("@Brand", cmbCustoemrBrand.Text);
                    cmd.Parameters.AddWithValue("@Series", cmbCustomerSeries.Text);
                    cmd.Parameters.AddWithValue("@Qty", int.Parse(txtCustomerQTY.Text));
                    cmd.Parameters.AddWithValue("@TotPrice", U);
                    cmd.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    return;
                }
                finally
                {
                    con.Close();
                    con.Open();
                    SqlCommand cmd7 = new SqlCommand("select SUM(TotPrice) from ProductAddTB", con);
                    double ke = Convert.ToInt32(cmd7.ExecuteScalar());
                    con.Close();
                    lblCustomerSubTotal.Text = ke.ToString();

                    con.Open();
                    SqlCommand cmd3 = new SqlCommand("Select ProductAddTB.BillID, ProductAddTB.Brand, ProductAddTB.Series , ProductAddTB.Qty, ProductAddTB.TotPrice from ProductAddTB order by 1 asc", con);
                    SqlDataAdapter da = new SqlDataAdapter(cmd3);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgwAddProducts.DataSource = dt;
                    con.Close();
                }
            }


        }

        private void customerProductBrand()
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("Select Name from ProductTB where PBrand = '" + cmbCustoemrBrand.Text + "' order by 1 asc", con);
                SqlDataAdapter da1 = new SqlDataAdapter();
                da1.SelectCommand = cmd;
                DataTable table1 = new DataTable();
                da1.Fill(table1);
                cmbCustomerSeries.DataSource = table1;
                cmbCustomerSeries.DisplayMember = "Name";
                cmbCustomerSeries.ValueMember = "Name";
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void cmbCustoemrBrand_TextChanged(object sender, EventArgs e)
        {
            customerProductBrand();
        }

        private void cmbCustomerSeries_TextChanged(object sender, EventArgs e)
        {
            
        }

        string CheckCUSID;

        private void checkidcus()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from CustomerTB where CustomerID ='" + txtCustomerId.Text + "'", con);
            SqlDataReader srd = cmd.ExecuteReader();
            while (srd.Read())
            {
                CheckCUSID = srd.GetValue(2).ToString();

            }
            con.Close();
        }

        private void ntnCustomerAdd_Click(object sender, EventArgs e)
        {

            if (txtCustomerId.Text == "" || txtCustomerName.Text == "" || txtCustomerNic.Text == "" || txtCustomerMobile.Text == "" || txtCustomerLand.Text == "" || txtCustomerDiscount.Text == "" || txtCustomerPayment.Text == "")
            {
                MessageBox.Show("Please Enter All Values");
            }
            else
            {
                checkidcus();

                if (CheckCUSID == txtCustomerId.Text)
                {
                    try
                    {
                        con.Open();


                        String Phonemobile = txtCustomerMobile.Text;
                        String PhoneLand = txtCustomerLand.Text;

                        if (!ValidatePhoneNumber(Phonemobile))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        if (!ValidatePhoneNumber1(PhoneLand))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        SqlCommand cmd1 = new SqlCommand("Update CustomerTB set CustomerName= @CustomerName,CustomerNIC= @CustomerNIC,CustomerMobile= @CustomerMobile,CustomerLand= @CustomerLand where CustomerID ='" + txtCustomerId.Text + "'", con);
                        cmd1.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                        cmd1.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd1.Parameters.AddWithValue("@CustomerMobile", int.Parse(txtCustomerMobile.Text));
                        cmd1.Parameters.AddWithValue("@CustomerLand", int.Parse(txtCustomerLand.Text));
                        cmd1.ExecuteNonQuery();

                        SqlCommand cmd2 = new SqlCommand("Update TodayCustomer set CustomerName= @CustomerName,CustomerNIC= @CustomerNIC,CustomerMobile= @CustomerMobile,CustomerLand= @CustomerLand where CustomerID ='" + txtCustomerId.Text + "'", con);
                        cmd2.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                        cmd2.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd2.Parameters.AddWithValue("@CustomerMobile", int.Parse(txtCustomerMobile.Text));
                        cmd2.Parameters.AddWithValue("@CustomerLand", int.Parse(txtCustomerLand.Text));
                        cmd2.ExecuteNonQuery();

                        SqlCommand cmd3 = new SqlCommand("Update InvoiceTB set CustomerNIC= @CustomerNIC,SubTotal= @SubTotal,Discount= @Discount,Total= @Total,Payment= @Payment,Balance= @Balance where InvoiceID ='" + txtCustomerId.Text + "'", con);
                        cmd3.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd3.Parameters.AddWithValue("@SubTotal", float.Parse(lblCustomerSubTotal.Text));
                        cmd3.Parameters.AddWithValue("@Discount", float.Parse(txtCustomerDiscount.Text));
                        cmd3.Parameters.AddWithValue("@Total", float.Parse(lblCustomerTotal.Text));
                        cmd3.Parameters.AddWithValue("@Payment", float.Parse(txtCustomerPayment.Text));
                        cmd3.Parameters.AddWithValue("@Balance", float.Parse(lblCustomerBalance.Text));
                        cmd3.ExecuteNonQuery();

                        SqlCommand cmd4 = new SqlCommand("Update InvoiceTodayData set CustomerNIC= @CustomerNIC,SubTotal= @SubTotal,Discount= @Discount,Total= @Total,Payment= @Payment,Balance= @Balance where InvoiceID ='" + txtCustomerId.Text + "'", con);
                        cmd4.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd4.Parameters.AddWithValue("@SubTotal", float.Parse(lblCustomerSubTotal.Text));
                        cmd4.Parameters.AddWithValue("@Discount", float.Parse(txtCustomerDiscount.Text));
                        cmd4.Parameters.AddWithValue("@Total", float.Parse(lblCustomerTotal.Text));
                        cmd4.Parameters.AddWithValue("@Payment", float.Parse(txtCustomerPayment.Text));
                        cmd4.Parameters.AddWithValue("@Balance", float.Parse(lblCustomerBalance.Text));
                        cmd4.ExecuteNonQuery();
                        con.Close();

                        MessageBox.Show("Update SuccessFull!");
                        clearDetails();
                        getMaxProductMIDId();
                    }
                    catch (Exception)
                    {
                        return;
                    }
                    finally
                    {
                        con.Close();

                    }
                }
                else
                {
                    try
                    {
                        con.Open();

                        SqlCommand cmd8 = new SqlCommand("insert into HelpFindProductTB (ID,BillID, Brand,Series,Qty,TotPrice) select ID,BillID, Brand,Series,Qty,TotPrice from ProductAddTB", con);
                        cmd8.ExecuteNonQuery();

                        String Phonemobile = txtCustomerMobile.Text;
                        String PhoneLand = txtCustomerLand.Text;

                        if (!ValidatePhoneNumber(Phonemobile))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        if (!ValidatePhoneNumber1(PhoneLand))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        SqlCommand cmd1 = new SqlCommand("insert into CustomerTB values(@ID, @NowDate, @CustomerID, @CustomerName, @CustomerNIC, @CustomerMobile, @CustomerLand)", con);
                        cmd1.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                        cmd1.Parameters.AddWithValue("@NowDate", DateTime.Parse(DateTime.Now.ToLongDateString()));
                        cmd1.Parameters.AddWithValue("@CustomerID", midID);
                        cmd1.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                        cmd1.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd1.Parameters.AddWithValue("@CustomerMobile", int.Parse(txtCustomerMobile.Text));
                        cmd1.Parameters.AddWithValue("@CustomerLand", int.Parse(txtCustomerLand.Text));
                        cmd1.ExecuteNonQuery();


                       

                        if (!ValidatePhoneNumber(Phonemobile))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        if (!ValidatePhoneNumber1(PhoneLand))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        SqlCommand cmd2 = new SqlCommand("insert into TodayCustomer values(@ID, @NowDate, @CustomerID, @CustomerName, @CustomerNIC, @CustomerMobile, @CustomerLand)", con);
                        cmd2.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                        cmd2.Parameters.AddWithValue("@NowDate", DateTime.Parse(DateTime.Now.ToLongDateString()));
                        cmd2.Parameters.AddWithValue("@CustomerID", midID);
                        cmd2.Parameters.AddWithValue("@CustomerName", txtCustomerName.Text);
                        cmd2.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd2.Parameters.AddWithValue("@CustomerMobile", int.Parse(txtCustomerMobile.Text));
                        cmd2.Parameters.AddWithValue("@CustomerLand", int.Parse(txtCustomerLand.Text));
                        cmd2.ExecuteNonQuery();

                        

                        SqlCommand cmd3 = new SqlCommand("insert into InvoiceTB values(@ID,@InvoiceID, @NowDate, @NowTime, @CustomerNIC, @SubTotal, @Discount, @Total, @Payment, @Balance)", con);
                        cmd3.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                        cmd3.Parameters.AddWithValue("@InvoiceID", midID);
                        cmd3.Parameters.AddWithValue("@NowDate", DateTime.Parse(DateTime.Now.ToLongDateString()));
                        cmd3.Parameters.AddWithValue("@NowTime", DateTime.Parse(DateTime.Now.ToLongTimeString()));
                        cmd3.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd3.Parameters.AddWithValue("@SubTotal", float.Parse(lblCustomerSubTotal.Text));
                        cmd3.Parameters.AddWithValue("@Discount", float.Parse(txtCustomerDiscount.Text));
                        cmd3.Parameters.AddWithValue("@Total", float.Parse(lblCustomerTotal.Text));
                        cmd3.Parameters.AddWithValue("@Payment", float.Parse(txtCustomerPayment.Text));
                        cmd3.Parameters.AddWithValue("@Balance", float.Parse(lblCustomerBalance.Text));
                        cmd3.ExecuteNonQuery();

                        SqlCommand cmd4 = new SqlCommand("insert into InvoiceTodayData values(@ID,@InvoiceID, @NowDate, @NowTime, @CustomerNIC, @SubTotal, @Discount, @Total, @Payment, @Balance)", con);
                        cmd4.Parameters.AddWithValue("@ID", int.Parse(Asid3));
                        cmd4.Parameters.AddWithValue("@InvoiceID", midID);
                        cmd4.Parameters.AddWithValue("@NowDate", DateTime.Parse(DateTime.Now.ToLongDateString()));
                        cmd4.Parameters.AddWithValue("@NowTime", DateTime.Parse(DateTime.Now.ToLongTimeString()));
                        cmd4.Parameters.AddWithValue("@CustomerNIC", txtCustomerNic.Text);
                        cmd4.Parameters.AddWithValue("@SubTotal", float.Parse(lblCustomerSubTotal.Text));
                        cmd4.Parameters.AddWithValue("@Discount", float.Parse(txtCustomerDiscount.Text));
                        cmd4.Parameters.AddWithValue("@Total", float.Parse(lblCustomerTotal.Text));
                        cmd4.Parameters.AddWithValue("@Payment", float.Parse(txtCustomerPayment.Text));
                        cmd4.Parameters.AddWithValue("@Balance", float.Parse(lblCustomerBalance.Text));
                        cmd4.ExecuteNonQuery();

                        


                        string re = cmbCustoemrBrand.Text + cmbCustomerSeries.Text;
                        //getPrice();

                        SqlCommand CMD5 = new SqlCommand("insert into InvoicePrintTB values (@Description,@Qty,@Price)", con);
                        CMD5.Parameters.AddWithValue("@Description", re);
                        CMD5.Parameters.AddWithValue("@Qty", int.Parse(txtCustomerQTY.Text));
                        CMD5.Parameters.AddWithValue("@Price", float.Parse(UnitPrice));
                        CMD5.ExecuteNonQuery();

                        SqlCommand cmd = new SqlCommand("DELETE FROM ProductAddTB ", con);
                        cmd.ExecuteNonQuery();

                        SqlCommand cmd5 = new SqlCommand("Select ProductAddTB.BillID, ProductAddTB.Brand, ProductAddTB.Series , ProductAddTB.Qty, ProductAddTB.TotPrice from ProductAddTB order by 1 asc", con);
                        SqlDataAdapter da = new SqlDataAdapter(cmd5);
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        dgwAddProducts.DataSource = dt;

                        MessageBox.Show("Adding Successfull!");

                        con.Close();

                        rchprint.Text = "==========================================\n" +
                                        "\t\t NETZ Mobile\n" +
                                        "   \t         No.23, Minuwangoda\n" +
                                        "\t\t    Gampaha\n" +
                                        "\n" +
                                        "Mobile - 078-867 3895\n" +
                                        "Land - 011-229 9767\t\t" + "\n" +
                                        "==========================================";

                        rchThank.Text = "==========================================\n" +
                                        "\t\t Thnk You!\n" +
                                        "\t  Welcome Again! Netz Mobile\n\n" +
                                        DateTime.Now.ToString();

                        con.Open();
                        SqlCommand cmd9 = new SqlCommand("select * from InvoicePrintTB", con);
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd9);
                        DataTable dt1 = new DataTable();
                        da1.Fill(dt1);
                        dataGridView1.DataSource = dt1;
                        con.Close();
                        lblInvoicediscount.Text = txtCustomerDiscount.Text;
                        lblInvoiceToT.Text = lblCustomerTotal.Text;
                        grbPrintinvoice.Visible = true;
                        grbCustomer.Enabled = false;

                        clearDetails();
                        getMaxProductMIDId();

                        /*
                        String Phonemobile = txtCustomerMobile.Text;
                        String PhoneLand = txtCustomerLand.Text;

                        if (!ValidatePhoneNumber(Phonemobile))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }

                        if (!ValidatePhoneNumber1(PhoneLand))
                        {
                            MessageBox.Show("invalid phone number formate!");
                            return;
                        }*/


                    }
                    catch (Exception)
                    {
                        return;
                    }
                    finally
                    {
                        con.Close();

                    }
                }

            }

        
        }


        private bool ValidatePhoneNumber(string Phonemobile)
        {
            // Regular expression pattern for a 10-digit phone number
            string pattern = @"^\d{10}$";

            // Check if the phone number matches the pattern
            return Regex.IsMatch(Phonemobile, pattern);
        }
        private bool ValidatePhoneNumber1(string PhoneLand)
        {
            // Regular expression pattern for a 10-digit phone number
            string pattern = @"^\d{10}$";

            // Check if the phone number matches the pattern
            return Regex.IsMatch(PhoneLand, pattern);
        }

        private void txtCustomerDiscount_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomerDiscount.Text != "")
            {
                float c = Convert.ToInt32(lblCustomerSubTotal.Text);
                float r = Convert.ToInt32(txtCustomerDiscount.Text);
                float res = c - r;
                lblCustomerTotal.Text = Convert.ToString(res);
            }
            else
            {
                return;
            }
        }

        private void txtCustomerPayment_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomerPayment.Text != "")
            {
                float c = Convert.ToInt32(lblCustomerTotal.Text);
                float r = Convert.ToInt32(txtCustomerPayment.Text);
                float res = r - c;
                lblCustomerBalance.Text = Convert.ToString(res);
            }
            else
            {
                return;
            }
        }

        private void btnClearonAddtb_Click(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ProductAddTB where Series= '" + cmbCustomerSeries.Text + "'  ", con);
                cmd.ExecuteNonQuery();

                SqlCommand cmd5 = new SqlCommand("Select ProductAddTB.BillID, ProductAddTB.Brand, ProductAddTB.Series , ProductAddTB.Qty, ProductAddTB.TotPrice from ProductAddTB order by 1 asc", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgwAddProducts.DataSource = dt;
                MessageBox.Show("Delete From List Successfull!");
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
        }

        string product1;

        private void selected()
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from ProductAddTB where Series ='" + cmbCustomerSeries.Text + "'", con);
            SqlDataReader srd = cmd.ExecuteReader();
            while (srd.Read())
            {
                product1  = srd.GetValue(3).ToString();

            }
            con.Close();
        }

        /*string unitprices;

        private void getPrice()
        {
            SqlCommand cmd = new SqlCommand("select * from ProductAddTB where Series ='" + cmbCustomerSeries.Text + "'", con);
            SqlDataReader srd = cmd.ExecuteReader();
            while (srd.Read())
            {
                unitprices = srd.GetValue(8).ToString();

            }
        }*/

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = false;
            grbCustomer.Visible = false;
            grbInvoice.Visible = true;
            grbSummery.Visible = false;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;

            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            con.Open();
            SqlCommand cmd5 = new SqlCommand("Select * from InvoiceTB", con);
            SqlDataAdapter da = new SqlDataAdapter(cmd5);
            DataTable dt = new DataTable();
            da.Fill(dt);
            con.Close();
            dataGridViewInvoice.DataSource = dt;
        }

        private void btnSummery_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = false;
            grbCustomer.Visible = false;
            grbInvoice.Visible = false;
            grbSummery.Visible = true;
            grbHistory.Visible = false;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;

            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            countprofit();

            try
            {
                con.Open();
                SqlCommand cmd5 = new SqlCommand("Select TodayCustomer.CustomerID, TodayCustomer.CustomerName, TodayCustomer.CustomerNIC, TodayCustomer.CustomerMobile, TodayCustomer.CustomerLand from TodayCustomer", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);

                SqlCommand cmd = new SqlCommand("Select InvoiceTodayData.InvoiceID, InvoiceTodayData.NowDate, InvoiceTodayData.NowTime, InvoiceTodayData.CustomerNIC, InvoiceTodayData.SubTotal, InvoiceTodayData.Discount, InvoiceTodayData.Total, InvoiceTodayData.Payment, InvoiceTodayData.Balance from InvoiceTodayData", con);
                SqlDataAdapter db = new SqlDataAdapter(cmd);
                DataTable ds = new DataTable();
                db.Fill(ds);

                dataGridViewSummery.DataSource = dt;
                dataGridViewTodayInvoices.DataSource = ds;

            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
            
        }

        private void txtSummerySearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                string SearchQuarry = "select InvoiceTodayData.InvoiceID, InvoiceTodayData.NowDate, InvoiceTodayData.NowTime, InvoiceTodayData.CustomerNIC, InvoiceTodayData.SubTotal, InvoiceTodayData.Discount, InvoiceTodayData.Total, InvoiceTodayData.Payment, InvoiceTodayData.Balance from InvoiceTodayData where CONCAT(InvoiceID, NowDate,CustomerNIC) LIKE '%" + txtSummerySearch.Text + "%'";
                SqlDataAdapter adapter = new SqlDataAdapter(SearchQuarry, con);
                DataTable ta = new DataTable();
                adapter.Fill(ta);
                dataGridViewSummery.DataSource = ta;

                string SearchQuarry1 = "select TodayCustomer.CustomerID, TodayCustomer.CustomerName, TodayCustomer.CustomerNIC, TodayCustomer.CustomerMobile, TodayCustomer.CustomerLand from TodayCustomer where CONCAT(CustomerID, NowDate, CustomerNIC) LIKE '%" + txtSummerySearch.Text + "%'";
                SqlDataAdapter adapter1 = new SqlDataAdapter(SearchQuarry1, con);
                DataTable ta1 = new DataTable();
                adapter1.Fill(ta1);
                dataGridViewTodayInvoices.DataSource = ta1;

            }
            catch(Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void btnHistory_Click(object sender, EventArgs e)
        {
            grbProduct.Visible = false;
            grbSupplier.Visible = false;
            grbCustomer.Visible = false;
            grbInvoice.Visible = false;
            grbSummery.Visible = false;
            grbHistory.Visible = true;
            grbAccount.Visible = false;
            grbPrintinvoice.Visible = false;

            grbLogindatabase.Visible = false;
            grbAccount.Visible = false;
            y1 = 1;

            CustomerColor();
            getMaxSuppierId();
            SupplierColor();
            invoiceColor();
            HistoryColor();
            SummeruColor();
            ProductColor();

            try
            {
                con.Open();
                SqlCommand cmd5 = new SqlCommand("select CustomerTB.NowDate, CustomerTB.CustomerID, CustomerTB.CustomerName, CustomerTB.CustomerNIC, CustomerTB.CustomerMobile, CustomerTB.CustomerLand from CustomerTB", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewAllCustomers.DataSource = dt;

                SqlCommand cmd1 = new SqlCommand("select InvoiceTB.NowDate, InvoiceTB.NowTime, InvoiceTB.InvoiceID, InvoiceTB.CustomerNIC, InvoiceTB.SubTotal, InvoiceTB.Discount, InvoiceTB.Total, InvoiceTB.Payment, InvoiceTB.Balance  from InvoiceTB", con);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                dataGridViewAllInvoices.DataSource = dt1;

                SqlCommand cmd2 = new SqlCommand("Select HelpFindProductTB.BillID, HelpFindProductTB.Brand, HelpFindProductTB.Series, HelpFindProductTB.Qty, HelpFindProductTB.TotPrice from HelpFindProductTB", con);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                dataGridViewBuyProducts.DataSource = dt2;

            }
            catch(Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }

            
        }

        private void txtHistorySearch_TextChanged(object sender, EventArgs e)
        {
            try
            {
                con.Open();
                SqlCommand cmd5 = new SqlCommand("select CustomerTB.NowDate, CustomerTB.CustomerID, CustomerTB.CustomerName, CustomerTB.CustomerNIC, CustomerTB.CustomerMobile, CustomerTB.CustomerLand from CustomerTB  where CONCAT(CustomerID, NowDate, CustomerNIC) LIKE '%" + txtHistorySearch.Text + "%'", con);
                SqlDataAdapter da = new SqlDataAdapter(cmd5);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridViewAllCustomers.DataSource = dt;

                SqlCommand cmd1 = new SqlCommand("select InvoiceTB.NowDate, InvoiceTB.NowTime, InvoiceTB.InvoiceID, InvoiceTB.CustomerNIC, InvoiceTB.SubTotal, InvoiceTB.Discount, InvoiceTB.Total, InvoiceTB.Payment, InvoiceTB.Balance  from InvoiceTB where CONCAT(InvoiceID, NowDate, CustomerNIC) LIKE '%" + txtHistorySearch.Text + "%'", con);
                SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                DataTable dt1 = new DataTable();
                da1.Fill(dt1);
                dataGridViewAllInvoices.DataSource = dt1;

                SqlCommand cmd2 = new SqlCommand("Select HelpFindProductTB.BillID, HelpFindProductTB.Brand, HelpFindProductTB.Series, HelpFindProductTB.Qty, HelpFindProductTB.TotPrice from HelpFindProductTB where CONCAT(BillID,Brand, Series) LIKE '%" + txtHistorySearch.Text + "%'", con);
                SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
                DataTable dt2 = new DataTable();
                da2.Fill(dt2);
                dataGridViewBuyProducts.DataSource = dt2;

            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Form1 fm = new Form1();
            this.Close();
            fm.Show();
            
        }

        private void btnListProductadd_MouseEnter(object sender, EventArgs e)
        {
            btnListProductadd.BackColor = Color.DarkGreen;
            btnListProductadd.ForeColor = Color.White;
            btnListProductadd.FlatAppearance.BorderSize = 0;
        }

        private void btnListProductadd_MouseLeave(object sender, EventArgs e)
        {
            btnListProductadd.BackColor = Color.Gainsboro;
            btnListProductadd.ForeColor = Color.Black;
            btnListProductadd.FlatAppearance.BorderSize = 0;
        }

        private void btnClearonAddtb_MouseEnter(object sender, EventArgs e)
        {
            btnClearonAddtb.BackColor = Color.Maroon;
            btnClearonAddtb.ForeColor = Color.White;
            btnClearonAddtb.FlatAppearance.BorderSize = 0;
        }

        private void btnClearonAddtb_MouseLeave(object sender, EventArgs e)
        {
            btnClearonAddtb.BackColor = Color.Gainsboro;
            btnClearonAddtb.ForeColor = Color.Black;
            btnClearonAddtb.FlatAppearance.BorderSize = 0;
        }

        private void txtClearCustomerDetails_Click(object sender, EventArgs e)
        {
            clearDetails();
        }

        private void txtClearCustomerDetails_MouseEnter(object sender, EventArgs e)
        {
            txtClearCustomerDetails.BackColor = Color.Black;
            txtClearCustomerDetails.ForeColor = Color.White;
            txtClearCustomerDetails.FlatAppearance.BorderSize = 0;
        }

        private void txtClearCustomerDetails_MouseLeave(object sender, EventArgs e)
        {
            txtClearCustomerDetails.BackColor = Color.Gainsboro;
            txtClearCustomerDetails.ForeColor = Color.Black;
            txtClearCustomerDetails.FlatAppearance.BorderSize = 0;
        }

        private void ntnCustomerAdd_MouseEnter(object sender, EventArgs e)
        {
            ntnCustomerAdd.BackColor = Color.Maroon;
            ntnCustomerAdd.ForeColor = Color.White;
            ntnCustomerAdd.FlatAppearance.BorderSize = 0;
        }

        private void ntnCustomerAdd_MouseLeave(object sender, EventArgs e)
        {
            ntnCustomerAdd.BackColor = Color.Gainsboro;
            ntnCustomerAdd.ForeColor = Color.Black;
            ntnCustomerAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierProductSeriesAdd_MouseEnter(object sender, EventArgs e)
        {
            btnSupplierProductSeriesAdd.BackColor = Color.DarkGreen;
            btnSupplierProductSeriesAdd.ForeColor = Color.White;
            btnSupplierProductSeriesAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierProductSeriesAdd_MouseLeave(object sender, EventArgs e)
        {
            btnSupplierProductSeriesAdd.BackColor = Color.Gainsboro;
            btnSupplierProductSeriesAdd.ForeColor = Color.Black;
            btnSupplierProductSeriesAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierProductSeriesDelete_MouseEnter(object sender, EventArgs e)
        {
            btnSupplierProductSeriesDelete.BackColor = Color.Maroon;
            btnSupplierProductSeriesDelete.ForeColor = Color.White;
            btnSupplierProductSeriesDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierProductSeriesDelete_MouseLeave(object sender, EventArgs e)
        {
            btnSupplierProductSeriesDelete.BackColor = Color.Gainsboro;
            btnSupplierProductSeriesDelete.ForeColor = Color.Black;
            btnSupplierProductSeriesDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierUpdate_MouseEnter(object sender, EventArgs e)
        {
            btnSupplierUpdate.BackColor = Color.Black;
            btnSupplierUpdate.ForeColor = Color.White;
            btnSupplierUpdate.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierUpdate_MouseLeave(object sender, EventArgs e)
        {
            btnSupplierUpdate.BackColor = Color.Gainsboro;
            btnSupplierUpdate.ForeColor = Color.Black;
            btnSupplierUpdate.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierDelete_MouseEnter(object sender, EventArgs e)
        {
            btnSupplierDelete.BackColor = Color.Maroon;
            btnSupplierDelete.ForeColor = Color.White;
            btnSupplierDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierDelete_MouseLeave(object sender, EventArgs e)
        {
            btnSupplierDelete.BackColor = Color.Gainsboro;
            btnSupplierDelete.ForeColor = Color.Black;
            btnSupplierDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierAdd_MouseEnter(object sender, EventArgs e)
        {
            btnSupplierAdd.BackColor = Color.Goldenrod;
            btnSupplierAdd.ForeColor = Color.White;
            btnSupplierAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnSupplierAdd_MouseLeave(object sender, EventArgs e)
        {
            btnSupplierAdd.BackColor = Color.Gainsboro;
            btnSupplierAdd.ForeColor = Color.Black;
            btnSupplierAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnProductUpdate_MouseEnter(object sender, EventArgs e)
        {
            btnProductUpdate.BackColor = Color.Black;
            btnProductUpdate.ForeColor = Color.White;
            btnProductUpdate.FlatAppearance.BorderSize = 0;
        }

        private void btnProductUpdate_MouseLeave(object sender, EventArgs e)
        {
            btnProductUpdate.BackColor = Color.Gainsboro;
            btnProductUpdate.ForeColor = Color.Black;
            btnProductUpdate.FlatAppearance.BorderSize = 0;
        }

        private void btnProductDelete_MouseEnter(object sender, EventArgs e)
        {
            btnProductDelete.BackColor = Color.Maroon;
            btnProductDelete.ForeColor = Color.White;
            btnProductDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnProductDelete_MouseLeave(object sender, EventArgs e)
        {
            btnProductDelete.BackColor = Color.Gainsboro;
            btnProductDelete.ForeColor = Color.Black;
            btnProductDelete.FlatAppearance.BorderSize = 0;
        }

        private void btnProductAdd_MouseEnter(object sender, EventArgs e)
        {
            btnProductAdd.BackColor = Color.Goldenrod;
            btnProductAdd.ForeColor = Color.White;
            btnProductAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnProductAdd_MouseLeave(object sender, EventArgs e)
        {
            btnProductAdd.BackColor = Color.Gainsboro;
            btnProductAdd.ForeColor = Color.Black;
            btnProductAdd.FlatAppearance.BorderSize = 0;
        }

        private void btnAccount_MouseEnter(object sender, EventArgs e)
        {
            if (btnSummery.Visible == true)
            {
                btnAccount.BackColor = Color.DarkGreen;
                btnAccount.ForeColor = Color.White;
                btnAccount.FlatAppearance.BorderSize = 0;
            }
            else if (btnSummery.Visible == false)
            {
                btnAccount.BackColor = Color.Maroon;
                btnAccount.ForeColor = Color.White;
                btnAccount.FlatAppearance.BorderSize = 0;
            }
        }

        private void btnAccount_MouseLeave(object sender, EventArgs e)
        {
            if (btnSummery.Visible == true)
            {
                btnAccount.BackColor = Color.DarkGreen;
                btnAccount.ForeColor = Color.White;
                btnAccount.FlatAppearance.BorderSize = 0;
            }
            else if (btnSummery.Visible == false)
            {
                btnAccount.BackColor = Color.Maroon;
                btnAccount.ForeColor = Color.White;
                btnAccount.FlatAppearance.BorderSize = 0;
            }
        }

        int y1 = 1;

        private void btnAccount_Click(object sender, EventArgs e)
        {
            if (y1 == 1)
            {
                grbAccount.Visible = true;
                y1 = 0;
            }
            else if (y1 == 0)
            {
                grbAccount.Visible = false;
                y1 = 1;
            }
            grbLogindatabase.Visible = false;
        }

        private void btnSecurityLogin_MouseEnter(object sender, EventArgs e)
        {
            btnSecurityLogin.BackColor = Color.DarkGreen;
            btnSecurityLogin.ForeColor = Color.White;
            btnSecurityLogin.FlatAppearance.BorderSize = 0;
        }

        private void btnSecurityLogin_MouseLeave(object sender, EventArgs e)
        {
            btnSecurityLogin.BackColor = Color.Gainsboro;
            btnSecurityLogin.ForeColor = Color.Black;
            btnSecurityLogin.FlatAppearance.BorderSize = 0;
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

        private void btnSecurityLogin_Click(object sender, EventArgs e)
        {
            getValues();
            if (btnAccount.Text == "A")
            {
                if (username == txtSecurityUserName.Text)
                {
                    if (password == txtSecurityPassword.Text)
                    {
                        r = 1;
                        getlogindaetails();
                        grbLogindatabase.Visible = true;

                        lblAdminAcountlable.BackColor = Color.DarkGreen;
                        lblAdminAcountlable.ForeColor = Color.White;
                        lblAdminAcountlable.Text = "A";

                        btnAdminClick.BackColor = Color.DarkGreen;
                        btnAdminClick.ForeColor = Color.White;
                        btnAdminClick.FlatAppearance.BorderSize = 0;

                        btnUserClick.BackColor = Color.Gainsboro;
                        btnUserClick.ForeColor = Color.Black;
                        btnUserClick.FlatAppearance.BorderSize = 0;

                        txtSecurityUserName.Clear();
                        txtSecurityPassword.Clear();

                        grbAccount.Visible = false;

                        txtUpPassword.UseSystemPasswordChar = true;
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
            else if (btnAccount.Text == "U")
            {
                if (UserName1 == txtSecurityUserName.Text)
                {
                    if (password1 == txtSecurityPassword.Text)
                    {
                        r = 2;
                        getlogindaetails();
                        grbLogindatabase.Visible = true;
                        btnAdminClick.Visible = false;

                        lblAdminAcountlable.BackColor = Color.Maroon;
                        lblAdminAcountlable.ForeColor = Color.White;
                        lblAdminAcountlable.Text = "U";

                        btnUserClick.BackColor = Color.Maroon;
                        btnUserClick.ForeColor = Color.White;
                        btnUserClick.FlatAppearance.BorderSize = 0;

                        txtSecurityUserName.Clear();
                        txtSecurityPassword.Clear();

                        grbAccount.Visible = false;

                        txtUpPassword.UseSystemPasswordChar = true;
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
            else
            {
                return;
            }
            

        }

        int r = 1;

        private void btnApplySecurityDetails_Click(object sender, EventArgs e)
        {
            if (IsValidPassword(txtUpPassword.Text))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("update LoginTB set UserName= ENCRYPTBYPASSPHRASE('8','" + txtUpUserName.Text + "'), Password = ENCRYPTBYPASSPHRASE('8', '" + txtUpPassword.Text + "'), Answer = ENCRYPTBYPASSPHRASE('8','" + txtUpAnswer.Text + "') where ID = '" + r + "'", con);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Update Successsfull!");
                con.Close();
            }
            else
            {
                MessageBox.Show("Your Password is not Valid!");
            } 

            
        }

        private void btnAdminClick_Click(object sender, EventArgs e)
        {
            r = 1;
            getlogindaetails();
            lblAdminAcountlable.BackColor = Color.DarkGreen;
            lblAdminAcountlable.ForeColor = Color.White;
            lblAdminAcountlable.Text = "A";

            btnAdminClick.BackColor = Color.DarkGreen;
            btnAdminClick.ForeColor = Color.White;
            btnAdminClick.FlatAppearance.BorderSize = 0;

            btnUserClick.BackColor = Color.Gainsboro;
            btnUserClick.ForeColor = Color.Black;
            btnUserClick.FlatAppearance.BorderSize = 0;

        }

        private void btnUserClick_Click(object sender, EventArgs e)
        {
            r = 2;
            getlogindaetails();
            lblAdminAcountlable.BackColor = Color.Maroon;
            lblAdminAcountlable.ForeColor = Color.White;
            lblAdminAcountlable.Text = "U";

            btnUserClick.BackColor = Color.Maroon;
            btnUserClick.ForeColor = Color.White;
            btnUserClick.FlatAppearance.BorderSize = 0;

            btnAdminClick.BackColor = Color.Gainsboro;
            btnAdminClick.ForeColor = Color.Black;
            btnAdminClick.FlatAppearance.BorderSize = 0;
        }

        private void btnApplySecurityDetails_MouseEnter(object sender, EventArgs e)
        {
            btnApplySecurityDetails.BackColor = Color.DarkGreen;
            btnApplySecurityDetails.ForeColor = Color.White;
            btnApplySecurityDetails.FlatAppearance.BorderSize = 0;
        }

        private void btnApplySecurityDetails_MouseLeave(object sender, EventArgs e)
        {
            btnApplySecurityDetails.BackColor = Color.Gainsboro;
            btnApplySecurityDetails.ForeColor = Color.Black;
            btnApplySecurityDetails.FlatAppearance.BorderSize = 0;
        }

        int W = 1;

        private void button1_Click(object sender, EventArgs e)
        {
            if (W == 0)
            {
                txtUpPassword.UseSystemPasswordChar = false;
                W = 1;
            }
            else if(W  == 1)
            {
                txtUpPassword.UseSystemPasswordChar = false;
                W = 0;
            }
        }

        public static bool IsValidPassword(string plainText)
        {
            Regex regex = new Regex(@"^(.{0,7}|[^0-9]*|[^A-Z])$");
            Match match = regex.Match(plainText);
            return match.Success;
        }

        private void txtUpPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (IsValidPassword(txtUpPassword.Text))
            {
                lblValidP.ForeColor = Color.Green;
                lblValidP.Text = "Password is Valid!";
            }
            else
            {
                lblValidP.ForeColor = Color.Maroon;
                lblValidP.Text = "Password is not Valid!";
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtUpPassword.UseSystemPasswordChar = false;
            }
            else
            {
                txtUpPassword.UseSystemPasswordChar = true;
            }
        }

        private void txtCustomerMobile_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCustomerLand_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCustomerDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void txtCustomerPayment_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != 46))
            {
                e.Handled = true;
                return;
            }

            // checks to make sure only 1 decimal is allowed
            if (e.KeyChar == 46)
            {
                if ((sender as TextBox).Text.IndexOf(e.KeyChar) != -1)
                    e.Handled = true;
            }
        }

        private void txtSuppilerSuppilerId_TextChanged(object sender, EventArgs e)
        {
            if (txtSuppilerSuppilerId.Text == "")
            {
                return;
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select * from SupplierTB where SupplierID ='" + txtSuppilerSuppilerId.Text + "'", con);
                SqlDataReader srd1 = cmd1.ExecuteReader();
                while (srd1.Read())
                {
                    txtSuplierName.Text = srd1.GetValue(2).ToString();
                    txtSuppierCompanyName.Text = srd1.GetValue(3).ToString();
                    txtSupplierMobile.Text = srd1.GetValue(4).ToString();
                    txtSupplierLand.Text = srd1.GetValue(5).ToString();
                    txtSupplierNo.Text = srd1.GetValue(6).ToString(); 
                    txtSupplierADline.Text = srd1.GetValue(7).ToString();
                    txtSupplierCity.Text = srd1.GetValue(8).ToString();
                }
                con.Close();
            }
            
        }

        private void txtProductProductID_TextChanged(object sender, EventArgs e)
        {
            if (txtProductProductID.Text == "")
            {
                return;
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select * from ProductTB where PID ='" + txtProductProductID.Text + "'", con);
                SqlDataReader srd1 = cmd1.ExecuteReader();
                while (srd1.Read())
                {
                    txtProductname.Text = srd1.GetValue(2).ToString();
                    cmbProductSeries.Text = srd1.GetValue(3).ToString();
                    cmbProductSuppilerName.Text = srd1.GetValue(4).ToString();
                    txtProductSN.Text = srd1.GetValue(5).ToString();
                    txtWarrenty.Text = srd1.GetValue(6).ToString();
                    txtProductUnits.Text = srd1.GetValue(7).ToString();
                    txtProductUnitPrice.Text = srd1.GetValue(8).ToString();
                }
                con.Close();
            }
        }

        private void getlogindaetails()
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',UserName)) from LoginTB where ID = '"+ r +"'", con);
            string username2 = Convert.ToString(cmd1.ExecuteScalar());

            txtUpUserName.Text = username2;

            SqlCommand cmd2 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',Password)) from LoginTB where ID = '" + r + "'", con);
            string Password2 = Convert.ToString(cmd2.ExecuteScalar());

            txtUpPassword.Text = Password2;

            SqlCommand cmd3 = new SqlCommand("select CONVERT(varchar(3000), DECRYPTBYPASSPHRASE('8',Answer)) from LoginTB where ID = '" + r + "'", con);
            string Answer2 = Convert.ToString(cmd3.ExecuteScalar());

            txtUpAnswer.Text = Answer2;

            con.Close();
        }

        private void btnPrintInvoice_Click(object sender, EventArgs e)
        {
            Print(this.panel1);
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM InvoicePrintTB ", con);
            cmd.ExecuteNonQuery();
            con.Close();
            grbPrintinvoice.Visible = false;
            grbCustomer.Enabled = true;
            clearDetails();
            
        }

        Bitmap MemoryImage;
        private PrintDocument printDocument1 = new PrintDocument();
        private PrintPreviewDialog previewdlg = new PrintPreviewDialog();

        public void GetPrintArea(Panel pnl)
        {
            MemoryImage = new Bitmap(pnl.Width, pnl.Height);
            pnl.DrawToBitmap(MemoryImage, new Rectangle(0, 0, pnl.Width, pnl.Height));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (MemoryImage != null)
            {
                e.Graphics.DrawImage(MemoryImage, 0, 0);
                base.OnPaint(e);
            }
        }

        void printdoc1_PrintPage(object sender, PrintPageEventArgs e)
        {
            Rectangle pagearea = e.PageBounds;
            e.Graphics.DrawImage(MemoryImage, (pagearea.Width / 2) - (this.panel1.Width / 2), this.panel1.Location.Y);
        }

        public void Print(Panel pnl)
        {
            Panel pannel = pnl;
            GetPrintArea(pnl);
            previewdlg.Document = printDocument1;
            previewdlg.ShowDialog();
        }

        private void btnPrintInvoice_MouseEnter(object sender, EventArgs e)
        {
            btnPrintInvoice.BackColor = Color.Black;
            btnPrintInvoice.ForeColor = Color.White;
            btnPrintInvoice.FlatAppearance.BorderSize = 0;
        }

        private void btnPrintInvoice_MouseLeave(object sender, EventArgs e)
        {
            btnPrintInvoice.BackColor = Color.Gainsboro;
            btnPrintInvoice.ForeColor = Color.Black;
            btnPrintInvoice.FlatAppearance.BorderSize = 0;
        }

        private void txtCustomerId_TextChanged(object sender, EventArgs e)
        {
            if (txtCustomerId.Text == "")
            {
                return;
            }
            else
            {
                con.Open();
                SqlCommand cmd1 = new SqlCommand("select * from CustomerTB where CustomerID ='" + txtCustomerId.Text + "'", con);
                SqlDataReader srd1 = cmd1.ExecuteReader();
                while (srd1.Read())
                {
                    txtCustomerName.Text = srd1.GetValue(3).ToString();
                    txtCustomerNic.Text = srd1.GetValue(4).ToString();
                    txtCustomerMobile.Text = srd1.GetValue(5).ToString();
                    txtCustomerLand.Text = srd1.GetValue(6).ToString();
                }
                con.Close();

                con.Open();
                SqlCommand cmd2 = new SqlCommand("select * from InvoiceTB where InvoiceID ='" + txtCustomerId.Text + "'", con);
                SqlDataReader srd2 = cmd2.ExecuteReader();
                while (srd2.Read())
                {
                    lblCustomerSubTotal.Text = srd2.GetValue(5).ToString();
                    txtCustomerDiscount.Text = srd2.GetValue(6).ToString();
                    lblCustomerTotal.Text = srd2.GetValue(7).ToString();
                    txtCustomerPayment.Text = srd2.GetValue(8).ToString();
                    lblCustomerBalance.Text = srd2.GetValue(9).ToString();
                }
                con.Close();
            }
        }
    }
}
