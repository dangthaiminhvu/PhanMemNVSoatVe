using System.Configuration;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

public static class ConnectionHelper
{
    private const int FastTimeoutSeconds = 2;

    public static string GetConnectionString()
    {
        string connStr = AddOrUpdateTimeout(
            ConfigurationManager.ConnectionStrings["MyConnStr"].ConnectionString,
            FastTimeoutSeconds);

        if (TestConnection(connStr))
            return connStr;

        while (true)
        {
            using (var dlg = new ConnectionDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string newConnStr = $"server={dlg.Server};database=phanmemquanlybaiguixe;uid={dlg.User};pwd={dlg.Password};";
                    newConnStr = AddOrUpdateTimeout(newConnStr, FastTimeoutSeconds);

                    if (TestConnection(newConnStr))
                    {
                        SaveConnectionString(newConnStr);
                        return newConnStr;
                    }
                    else
                    {
                        MessageBox.Show("Kết nối không thành công. Hãy thử lại.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Application.Exit();
                    return null;
                }
            }
        }
    }

    private static bool TestConnection(string connStr)
    {
        try
        {
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private static void SaveConnectionString(string connStr)
    {
        var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.ConnectionStrings.ConnectionStrings["MyConnStr"].ConnectionString = connStr;
        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection("connectionStrings");
    }

    // Helper to add or update the timeout in the connection string
    private static string AddOrUpdateTimeout(string connStr, int timeoutSeconds)
    {
        var builder = new MySqlConnectionStringBuilder(connStr)
        {
            ConnectionTimeout = (uint)timeoutSeconds
        };
        return builder.ConnectionString;
    }
}
