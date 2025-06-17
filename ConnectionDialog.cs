using System;
using System.Windows.Forms;

public class ConnectionDialog : Form
{
    private Label lblServer;
    private Label lblUser;
    private Label lblPassword;
    private TextBox txtServer;
    private TextBox txtUser;
    private TextBox txtPassword;
    private Button btnOK;
    private Button btnCancel;

    public string Server => txtServer.Text.Trim();
    public string User => txtUser.Text.Trim();
    public string Password => txtPassword.Text;

    public ConnectionDialog()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.lblServer = new Label();
        this.lblUser = new Label();
        this.lblPassword = new Label();
        this.txtServer = new TextBox();
        this.txtUser = new TextBox();
        this.txtPassword = new TextBox();
        this.btnOK = new Button();
        this.btnCancel = new Button();

        // 
        // lblServer
        // 
        this.lblServer.AutoSize = true;
        this.lblServer.Location = new System.Drawing.Point(12, 15);
        this.lblServer.Name = "lblServer";
        this.lblServer.Size = new System.Drawing.Size(58, 13);
        this.lblServer.TabIndex = 0;
        this.lblServer.Text = "Server IP:";
        // 
        // txtServer
        // 
        this.txtServer.Location = new System.Drawing.Point(90, 12);
        this.txtServer.Name = "txtServer";
        this.txtServer.Size = new System.Drawing.Size(180, 20);
        this.txtServer.TabIndex = 1;
        // 
        // lblUser
        // 
        this.lblUser.AutoSize = true;
        this.lblUser.Location = new System.Drawing.Point(12, 45);
        this.lblUser.Name = "lblUser";
        this.lblUser.Size = new System.Drawing.Size(58, 13);
        this.lblUser.TabIndex = 2;
        this.lblUser.Text = "Username:";
        // 
        // txtUser
        // 
        this.txtUser.Location = new System.Drawing.Point(90, 42);
        this.txtUser.Name = "txtUser";
        this.txtUser.Size = new System.Drawing.Size(180, 20);
        this.txtUser.TabIndex = 3;
        // 
        // lblPassword
        // 
        this.lblPassword.AutoSize = true;
        this.lblPassword.Location = new System.Drawing.Point(12, 75);
        this.lblPassword.Name = "lblPassword";
        this.lblPassword.Size = new System.Drawing.Size(56, 13);
        this.lblPassword.TabIndex = 4;
        this.lblPassword.Text = "Password:";
        // 
        // txtPassword
        // 
        this.txtPassword.Location = new System.Drawing.Point(90, 72);
        this.txtPassword.Name = "txtPassword";
        this.txtPassword.Size = new System.Drawing.Size(180, 20);
        this.txtPassword.TabIndex = 5;
        this.txtPassword.UseSystemPasswordChar = true;
        // 
        // btnOK
        // 
        this.btnOK.Location = new System.Drawing.Point(90, 110);
        this.btnOK.Name = "btnOK";
        this.btnOK.Size = new System.Drawing.Size(80, 25);
        this.btnOK.TabIndex = 6;
        this.btnOK.Text = "OK";
        this.btnOK.DialogResult = DialogResult.OK;
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(190, 110);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(80, 25);
        this.btnCancel.TabIndex = 7;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.DialogResult = DialogResult.Cancel;
        // 
        // ConnectionDialog
        // 
        this.AcceptButton = this.btnOK;
        this.CancelButton = this.btnCancel;
        this.ClientSize = new System.Drawing.Size(290, 150);
        this.Controls.Add(this.lblServer);
        this.Controls.Add(this.txtServer);
        this.Controls.Add(this.lblUser);
        this.Controls.Add(this.txtUser);
        this.Controls.Add(this.lblPassword);
        this.Controls.Add(this.txtPassword);
        this.Controls.Add(this.btnOK);
        this.Controls.Add(this.btnCancel);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Name = "ConnectionDialog";
        this.Text = "Enter MySQL Connection";
    }
}
